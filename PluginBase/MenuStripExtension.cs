using System.Windows.Forms;

namespace TextAnalysisTool.NET.Plugin
{
    public static class MenuStripExtension
    {
        public static ToolStripMenuItem FindMenuItem(this MenuStrip menu, string menuText)
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
