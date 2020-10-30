using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextAnalysisTool.NET.Plugin
{
    public class LiveLogPlugin : TextReader
    {
        private List<string> _lines = new List<string>();
        private int _currentLine;
        private Tat _tat;
        private ToolStrip _toolbar;
        private ToolStripButton _startBtn;
        private ToolStripButton _stopBtn;
        private ToolStripButton _cleanBtn;
        private ToolStripButton _closeBtn;
        private ToolStripTextBox _portTextBox;
        private CancellationTokenSource _captureCancelSource;
        private Task _listenTask;

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
            _tat.Closed += () => StopCapturing();
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
            AddToolBar();
        }

        private void AddToolBar()
        {
            if (_toolbar == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resNames = assembly.GetManifestResourceNames();
                
                _startBtn = new ToolStripButton(Image.FromStream(assembly.GetManifestResourceStream(resNames.Single(str => str.EndsWith("Start-icon.png")))));
                _startBtn.ToolTipText = "start capturing network stream";
                _startBtn.Click += (s, e) => { StartCapturing(); UpdateButtonStates(); };

                _stopBtn = new ToolStripButton(Image.FromStream(assembly.GetManifestResourceStream(resNames.Single(str => str.EndsWith("Stop-red-icon.png")))));
                _stopBtn.ToolTipText = "stop capturing network stream";
                _stopBtn.Click += (s, e) => { StopCapturing(); UpdateButtonStates(); };

                _cleanBtn = new ToolStripButton(Image.FromStream(assembly.GetManifestResourceStream(resNames.Single(str => str.EndsWith("cancel-icon.png")))));
                _cleanBtn.ToolTipText = "clean network stream";
                _cleanBtn.Click += (s, e) => { Empty(); UpdateButtonStates(); };

                _closeBtn = new ToolStripButton(Image.FromStream(assembly.GetManifestResourceStream(resNames.Single(str => str.EndsWith("delete-icon.png")))));
                _closeBtn.ToolTipText = "close network stream";
                _closeBtn.Click += (s, e) => RemoveToolBar();

                var ts = new ToolStrip();
                ts.Items.Add(new ToolStripLabel()
                {
                    Text = "Udp listen port:"
                });
                ts.Items.Add(_portTextBox = new ToolStripTextBox()
                {
                    BorderStyle = BorderStyle.FixedSingle
                });
                ts.Items.Add(_startBtn);
                ts.Items.Add(_stopBtn);
                ts.Items.Add(_cleanBtn);
                ts.Items.Add(_closeBtn);

                // Add bug report btn.
                var bugReportBtn = new ToolStripButton(Image.FromStream(assembly.GetManifestResourceStream(resNames.Single(str => str.EndsWith("bug_error.png")))));
                bugReportBtn.ToolTipText = "report bug!";
                bugReportBtn.Click += (s, e) => _tat.OpenReportPluginsIssueLink();
                ts.Items.Add(bugReportBtn);

                // Save
                _toolbar = ts;
            }
            Tat.Instance.AddToolStrip(_toolbar);
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            _startBtn.Enabled = _listenTask == null;
            _stopBtn.Enabled = _listenTask != null;
            _cleanBtn.Enabled = _lines.Count > 0;
        }

        private void StartCapturing()
        {
            Debug.Assert(_listenTask == null);

            int port;
            if (!int.TryParse(_portTextBox.Text, out port) || port < 0 || port > 65535)
            {
                MessageBox.Show($"Invalid port: {_portTextBox.Text}");
                return;
            }

            _captureCancelSource = new CancellationTokenSource();
            _listenTask = Task.Run(() =>
            {
                try
                {
                    using (var udp = new UdpClient(port))
                    {
                        while (true)
                        {
                            try
                            {
                                var receiveTask = udp.ReceiveAsync();
                                receiveTask.Wait(_captureCancelSource.Token);
                                var receivedMsg = UTF8Encoding.UTF8.GetString(receiveTask.Result.Buffer);
                                _tat.RunAtMainThread(() => OnMsgReceived(receivedMsg));
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (SocketException)
                {
                    _tat.RunAtMainThread(() =>
                    {
                        StopCapturing();
                        MessageBox.Show($"Listen on port: {port} failed.");
                    });
                }
            });
        }

        private void StopCapturing()
        {
            if (_listenTask != null)
            {
                _captureCancelSource.Cancel();
                _listenTask.Wait();

                _captureCancelSource = null;
                _listenTask = null;
            }
        }

        private void Empty()
        {
            _lines.Clear();
            _currentLine = 0;
            _tat.Refresh(this);
        }

        private void RemoveToolBar()
        {
            StopCapturing();
            Empty();
            _tat.RemoveToolStrip(_toolbar);
        }

        private void OnMsgReceived(string msg)
        {
            _lines.Add(msg);
            _currentLine = 0;
            _cleanBtn.Enabled = true;
            _tat.Refresh(this);
            _tat.ScrollToLineNumber(_lines.Count);
        }
    }
}
