using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace TextAnalysisTool.NET.Plugin
{
    public class LiveLogPlugin : TextReader
    {
        private List<string> _lines = new List<string>();
        private int _currentLine = 0;
        private Tat _tat;

        public LiveLogPlugin()
        {
            _tat = Tat.Instance;
            _tat.MainMenuCreated += (mainMenu) =>
            {
                var fileMenu = mainMenu.FindMenuItem("&File");
                if (fileMenu != null)
                {
                    for (int i = 0; i < fileMenu.DropDownItems.Count; ++i)
                    {
                        if (fileMenu.DropDownItems[i].Text == "&Open...")
                        {
                            var menu = new ToolStripMenuItem("Open Network Stream...", null, new EventHandler(OpenNetworkStream));
                            fileMenu.DropDownItems.Insert(i + 1, menu);
                            break;
                        }
                    }
                }
            };
        }

        public override string ReadLine()
        {
            if (_currentLine < _lines.Count)
            {
                return _lines[_currentLine++];
            }
            else
            {
                return null;
            }
        }

        private void OpenNetworkStream(object sender, EventArgs e)
        {
            _lines.Add("hello world");
            _currentLine = 0;
            _tat.Refresh(this);
        }
    }
}
