using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TextAnalysisTool.NET.Plugin
{
    public class Tat
    {
        #region Statics
        public static Tat Instance { get; } = new Tat();
        #endregion

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void MainMenuCreatedHandler(MenuStrip mainMenu);
        public delegate void ActionHandler();

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 GetCurrentWin32ThreadId();

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lparam;
            public IntPtr wparam;
            public int message;
            public IntPtr hwnd;
        }

        public class LogLine
        {
            public string text { get; set; }
            public int lineNumber { get; set; }
        }

        #region Events
        public event MainMenuCreatedHandler MainMenuCreated;
        public event ActionHandler Closed;
        #endregion

        private Tat()
        {
            Application.Idle += Application_Idle;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            bool ok = HookupMainFrom();
            if (ok)
            {
                ok = HookupMainMenu();
            }
            if (ok)
            {
                Application.Idle -= Application_Idle;
            }
        }

        private bool HookupMainFrom()
        {
            if (_mainForm != null)
            {
                return true;
            }

            if (Application.OpenForms?.Count > 0)
            {
                var mainForm = Application.OpenForms[0];
                if (mainForm != null)
                {
                    _tat = mainForm.GetType().GetField("m_textAnalysisTool", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mainForm);
                    _linesChangedMethod = _tat.GetType().GetMethod("LinesChanged");
                    var lineCollectionType = _tat.GetType().Assembly.GetType("TextAnalysisTool.NET.LineCollection");
                    _lineCollectionConstructor = lineCollectionType.GetConstructor(new[] { typeof(TextReader) });
                    _lineCollectionField = _tat.GetType().GetField("m_lineCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                    _lineCollectionDisplay = (ListBox)_tat.GetType().GetField("m_lineCollectionDisplay", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_tat);
                    _hookProc = new HookProc(OnListBox_SetCount);
                    _hHook = SetWindowsHookEx(12, _hookProc, (IntPtr)0, GetCurrentWin32ThreadId());

                    mainForm.FormClosed += (s, e) =>
                    {
                        Closed?.Invoke();

                        if (_hHook != 0)
                        {
                            UnhookWindowsHookEx(_hHook);
                            _hHook = 0;
                        }
                    };

                    _mainForm = mainForm;
                    return true;
                }
            }
            return false;
        }

        private bool HookupMainMenu()
        {
            if (_isMainMenuHooked)
            {
                return true;
            }

            Debug.Assert(_mainForm != null);
            if (_mainForm.Controls != null)
            {
                foreach (var control in _mainForm.Controls)
                {
                    if (control is MenuStrip)
                    {
                        MainMenuCreated?.Invoke((MenuStrip)control);
                        _isMainMenuHooked = true;
                        break;
                    }
                }
            }

            return _isMainMenuHooked;
        }

        public LogLine[] GetFilteredLines()
        {
            var typ = _lineCollectionDisplay.GetType().GetField("m_filteredLines", BindingFlags.NonPublic | BindingFlags.Instance);
            var filteredLines = _lineCollectionDisplay.GetType().GetField("m_filteredLines", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_lineCollectionDisplay);
            var lines = (int[])filteredLines.GetType().GetProperty("LineNumbersInCollection").GetValue(filteredLines);
            var res = new string[lines.Count()];
            var getLineTextFunc = filteredLines.GetType().GetMethod("GetLineText");
            return lines.Select(i => new LogLine
            {
                text = (String)getLineTextFunc.Invoke(filteredLines, new object[] { i }),
                lineNumber = i
            }).ToArray();
        }

        public void ScrollToLineNumber(int linenumber)
        {
            _lineCollectionDisplay.GetType().GetMethod("SelectLineNumber").Invoke(_lineCollectionDisplay, new object[] { linenumber });
        }

        public void Refresh(TextReader reader)
        {
            var lineCollection = _lineCollectionConstructor.Invoke(new[] { reader });
            _lineCollectionField.SetValue(_tat, lineCollection);
            _linesChangedMethod.Invoke(_tat, null);
        }

        public void AddToolStrip(ToolStrip toolStrip)
        {
            if (_mainForm?.Controls != null)
            {
                if (!_mainForm.Controls.Contains(toolStrip))
                {
                    for (var i = 0; i < _mainForm.Controls.Count; ++i)
                    {
                        if (_mainForm.Controls[i] is MenuStrip)
                        {
                            var menuStrip = _mainForm.Controls[i];
                            _mainForm.Controls.RemoveAt(i);
                            _mainForm.Controls.Add(toolStrip);
                            _mainForm.Controls.Add(menuStrip);
                            break;
                        }
                    }
                }
            }
        }

        public void RemoveToolStrip(ToolStrip toolStrip)
        {
            _mainForm?.Controls.Remove(toolStrip);
        }

        public void RunAtMainThread(Action action)
        {
            _mainForm.Invoke(action);
        }

        public void Show(Form window)
        {
            if (window.Visible)
            {
                if (window.WindowState == FormWindowState.Minimized)
                {
                    window.WindowState = FormWindowState.Normal;
                }
                window.BringToFront();
            }
            else
            {
                window.StartPosition = FormStartPosition.Manual;
                window.Top = _mainForm.Top + (_mainForm.Height - window.Height) / 2;
                window.Left = _mainForm.Left + (_mainForm.Width - window.Width) / 2;
                window.Show();
            }
        }

        public void OpenReportPluginsIssueLink()
        {
            Process.Start("https://github.com/justforfun-click/TextAnalysisTool.NET.Plugins/issues");
        }

        private Form _mainForm;
        private bool _isMainMenuHooked;
        private int _hHook;
        private HookProc _hookProc;
        private object _tat;
        private ListBox _lineCollectionDisplay;
        private ConstructorInfo _lineCollectionConstructor;
        private FieldInfo _lineCollectionField;
        private MethodInfo _linesChangedMethod;

        // Tat uses MeasureString() to caculate the width of the horizontal scroll bar. That api has a bug that it can't return the enough width of the string for some font and font size.
        // The work around here is hooking LB_SETCOUNT(423) message and invoke MeasureString() with a bigger font size.
        private int OnListBox_SetCount(int nCode, IntPtr wParam, IntPtr lParam)
        {
            CWPSTRUCT p = (CWPSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPSTRUCT));
            if (p.message == 423 && p.hwnd == _lineCollectionDisplay.Handle)
            {
                var currentDisplayLines = _lineCollectionDisplay.GetType().GetProperty("CurrentDisplayLines", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_lineCollectionDisplay);
                var maxDisplayWidth = (int)currentDisplayLines.GetType().GetProperty("MaxDisplayWidth").GetValue(currentDisplayLines);
                using (var g = _lineCollectionDisplay.CreateGraphics())
                {
                    _lineCollectionDisplay.HorizontalExtent = (int)Math.Ceiling(g.MeasureString(string.Empty.PadRight(maxDisplayWidth, 'W'), new Font(_lineCollectionDisplay.Font.FontFamily, _lineCollectionDisplay.Font.Size + 0.5f)).Width);
                }
            }
            return CallNextHookEx(0, nCode, wParam, lParam);
        }
    }
}
