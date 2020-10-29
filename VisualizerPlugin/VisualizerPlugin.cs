using System;
using System.Windows.Forms;

namespace TextAnalysisTool.NET.Plugin
{
    public class VisualizerPlugin
    {
        private Tat _tat;
        private Visualizer _visualizer;

        public VisualizerPlugin()
        {
            _tat = Tat.Instance;
            _tat.MainMenuCreated += (mainMenu) =>
            {
                // Add custom menu items.
                var viewMenu = mainMenu.FindMenuItem("&View");
                if (viewMenu != null)
                {
                    for (int i = 0; i < viewMenu.DropDownItems.Count; ++i)
                    {
                        if (viewMenu.DropDownItems[i] is ToolStripSeparator)
                        {
                            var menu = new ToolStripMenuItem("Show Visualizer", null, new EventHandler(OpenVisualzier));
                            viewMenu.DropDownItems.Insert(i, menu);
                            break;
                        }
                    }
                }
            };
        }

        private void OpenVisualzier(object sender, EventArgs e)
        {
            if (_visualizer == null)
            {
                _visualizer = new Visualizer(_tat);
            }

            if (_visualizer.Visible)
            {
                if (_visualizer.WindowState == FormWindowState.Minimized)
                {
                    _visualizer.WindowState = FormWindowState.Normal;
                }
                _visualizer.BringToFront();
            }
            else
            {
                _visualizer.Show();
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
