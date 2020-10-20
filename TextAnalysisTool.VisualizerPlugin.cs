using System;
using System.IO;
using System.Windows.Forms;

namespace TextAnalysisTool.NET.Plugin
{
    /// <summary>
    /// Interface that all TextAnalysisTool.NET plug-ins must implement
    /// </summary>
    internal interface ITextAnalysisToolPlugin
    {
        /// <summary>
        /// Gets a meaningful string describing the type of file supported by the plug-in
        /// </summary>
        /// <remarks>
        /// Used to populate the "Files of type" combo box in the Open file dialog
        /// </remarks>
        /// <example>
        /// "XML Files"
        /// </example>
        /// <returns>descriptive string</returns>
        string GetFileTypeDescription();

        /// <summary>
        /// Gets the file type pattern describing the type(s) of file supported by the plug-in
        /// </summary>
        /// <remarks>
        /// Used to populate the "Files of type" combo box in the Open file dialog
        /// </remarks>
        /// <example>
        /// "*.xml"
        /// </example>
        /// <returns>file type pattern</returns>
        string GetFileTypePattern();

        /// <summary>
        /// Indicates whether the plug-in is able to parse the specified file
        /// </summary>
        /// <param name="fileName">full path to the file</param>
        /// <remarks>
        /// Called whenever a file is being opened to give the plug-in a chance to handle it;
        /// ideally the result can be returned based solely on the file name, but it is
        /// acceptable to open, read, and close the file if necessary
        /// </remarks>
        /// <returns>true iff the file is supported</returns>
        bool IsFileTypeSupported(string fileName);

        /// <summary>
        /// Returns a TextReader instance that will be used to read the specified file
        /// </summary>
        /// <param name="fileName">full path to the file</param>
        /// <remarks>
        /// The only methods that will be called (and therefore need to be implemented) are
        /// TextReader.ReadLine() and IDisposable.Dispose()
        /// </remarks>
        /// <returns>TextReader instance</returns>
        System.IO.TextReader GetReaderForFile(string fileName);
    }

    public class LogVisualizerPlugin : ITextAnalysisToolPlugin
    {
        private bool mMenuInitialized = false;
        private Tat mTat = null;
        private Visualizer mVisualizer = null;

        public LogVisualizerPlugin()
        {
            Application.Idle += Application_Idle1;
        }

        private void Application_Idle1(object sender, EventArgs e)
        {
            if (mMenuInitialized)
            {
                Application.Idle -= Application_Idle1;
                return;
            }

            var mainForm = Application.OpenForms[0];
            if (mainForm != null && mainForm.Controls != null)
            {
                if (mTat == null)
                {
                    mTat = new Tat(mainForm);

                    mainForm.FormClosed += (s, ev) =>
                    {
                        if (mTat != null)
                        {
                            mTat.Dispose();
                            mTat = null;
                        }
                    };
                }

                foreach (var control in mainForm.Controls)
                {
                    if(mMenuInitialized)
                    {
                        break;
                    }

                    if (control is MenuStrip)
                    {
                        // Add custom menu items.
                        var fileMenu = FindMenuItem((MenuStrip)control, "&View");
                        if (fileMenu != null)
                        {
                            for (int i = 0; i < fileMenu.DropDownItems.Count; ++i)
                            {
                                if (fileMenu.DropDownItems[i] is ToolStripSeparator)
                                {
                                    var menu = new ToolStripMenuItem("Show Visualizer", null, new EventHandler(OpenVisualzier));
                                    fileMenu.DropDownItems.Insert(i, menu);
                                    mMenuInitialized = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OpenVisualzier(object sender, EventArgs e)
        {
            if (mTat == null)
            {
                MessageBox.Show("Internal error, can't open Visualizer.");
                return;
            }

            if (mVisualizer == null)
            {
                mVisualizer = new Visualizer(mTat);
            }

            if (mVisualizer.Visible)
            {
                if (mVisualizer.WindowState == FormWindowState.Minimized)
                {
                    mVisualizer.WindowState = FormWindowState.Normal;
                }
                mVisualizer.BringToFront();
            }
            else
            {
                mVisualizer.Show();
            }
        }

        private ToolStripMenuItem FindMenuItem(MenuStrip menu, string menuText)
        {
            foreach (var item in menu.Items)
            {
                if (item is ToolStripMenuItem)
                {
                    var m = (ToolStripMenuItem)item;
                    if (m.Text == menuText)
                    {
                        return m;
                    }
                }
            }
            return null;
        }

        public string GetFileTypeDescription()
        {
            return null;
        }

        public string GetFileTypePattern()
        {
            return null;
        }

        public TextReader GetReaderForFile(string fileName)
        {
            return null;
        }

        public bool IsFileTypeSupported(string fileName)
        {
            return false;
        }
    }
}
