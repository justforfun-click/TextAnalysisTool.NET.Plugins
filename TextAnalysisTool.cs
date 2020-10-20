using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TextAnalysisTool.NET.Plugin
{
    public class Tat : IDisposable
    {
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

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

        public Tat(Form mainForm)
        {
            tat = mainForm.GetType().GetField("m_textAnalysisTool", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mainForm);
            lineCollectionDisplay = (ListBox)tat.GetType().GetField("m_lineCollectionDisplay", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(tat);
            hookProc = new HookProc(OnListBox_SetCount);
            hook = SetWindowsHookEx(12, hookProc, (IntPtr)0, GetCurrentWin32ThreadId());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public LogLine[] GetFilteredLines()
        {
            var typ = lineCollectionDisplay.GetType().GetField("m_filteredLines", BindingFlags.NonPublic | BindingFlags.Instance);
            var filteredLines = lineCollectionDisplay.GetType().GetField("m_filteredLines", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lineCollectionDisplay);
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
            lineCollectionDisplay.GetType().GetMethod("SelectLineNumber").Invoke(lineCollectionDisplay, new object[] { linenumber });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (hook != 0)
            {
                UnhookWindowsHookEx(hook);
                hook = 0;
            }
        }

        private int hook;
        private HookProc hookProc;
        private object tat;
        private ListBox lineCollectionDisplay;

        // Tat uses MeasureString() to caculate the width of the horizontal scroll bar. That api has a bug that it can't return the enough width of the string for some font and font size.
        // The work around here is hooking LB_SETCOUNT(423) message and invoke MeasureString() with a bigger font size.
        private int OnListBox_SetCount(int nCode, IntPtr wParam, IntPtr lParam)
        {
            CWPSTRUCT p = (CWPSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPSTRUCT));
            if (p.message == 423 && p.hwnd == lineCollectionDisplay.Handle)
            {
                var currentDisplayLines = lineCollectionDisplay.GetType().GetProperty("CurrentDisplayLines", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lineCollectionDisplay);
                var maxDisplayWidth = (int)currentDisplayLines.GetType().GetProperty("MaxDisplayWidth").GetValue(currentDisplayLines);
                using (var g = lineCollectionDisplay.CreateGraphics())
                {
                    lineCollectionDisplay.HorizontalExtent = (int)Math.Ceiling(g.MeasureString(string.Empty.PadRight(maxDisplayWidth, 'W'), new Font(lineCollectionDisplay.Font.FontFamily, lineCollectionDisplay.Font.Size + 0.5f)).Width);
                }
            }
            return CallNextHookEx(0, nCode, wParam, lParam);
        }
    }
}
