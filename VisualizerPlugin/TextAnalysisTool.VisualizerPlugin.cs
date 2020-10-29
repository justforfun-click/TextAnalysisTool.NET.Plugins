using System;
using System.Windows.Forms;

namespace TextAnalysisTool.NET.Plugin
{
    public class LogVisualizerPlugin
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
    }
}
