using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TextAnalysisTool.NET.Plugin
{
    public partial class Visualizer : Form
    {
        public Visualizer(Tat tat)
        {
            this.tat = tat;
            InitializeComponent();
        }

        private class SeriesContext
        {
            public Type ParserType { get; set; }
            public Dictionary<int, int> PointIndexToLineNumber = new Dictionary<int, int>();
        }

        private class ListViewItemContext
        {
            public Type ParserType { get; set; }
        }

        private Tat tat;
        private PlotView plot;
        private Tat.LogLine[] logLines;
        private DateTime lastDoubleClickTime;
        private Series lastDoubleClickSeries;
        private int lastDoubleClickPointIndex;
        private string parserFilePath;

        private void Visualizer_Load(object sender, EventArgs e)
        {
            plot = new PlotView()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            plotPanel.Controls.Add(plot);

            // Load parser automatically.
            if (parserFilePath == null)
            {
                var parserFileName = Path.Combine(Path.GetDirectoryName(typeof(IVisualizerParser).Assembly.Location), "VisualizerParser.cs");
                if (File.Exists(parserFileName))
                {
                    parserFilePath = parserFileName;
                    ReloadParser();
                }
            }
        }

        private void CopyGraphToClipboard()
        {
            using (var bmp = new Bitmap(plot.Width, plot.Height))
            {
                plot.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));
                Clipboard.SetImage(bmp);
            }
        }

        private void newParserTemplateBtn_Click(object sender, EventArgs e)
        {
            using (var frm = new SaveFileDialog())
            {
                frm.DefaultExt = "cs";
                frm.Filter = "CSharp files (*.cs)|*.cs";
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    using (var sw = new StreamWriter(frm.FileName))
                    {
                        sw.Write(
@"using System.Text.RegularExpressions;

namespace TextAnalysisTool.NET.Plugin
{
    /*
    public interface IVisualizerParser
    {
        // Return the name of the parser.
        string GetLabel();

        // Return the date time of the log line.
        DateTime? GetTime(string logline);

        // Return the value of the log line.
        double? GetValue(string logline);
    }
    */

    // Write your parser class here. It is required to implement IVisualizerParser interface.
    // If you want to create multiple parsers, you can define multiple parser classes.
}
");
                    }

                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = frm.FileName,
                        UseShellExecute = true
                    });
                }
            }
        }

        private void parsersListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
            {
                AddPoints(((ListViewItemContext)e.Item.Tag).ParserType);
            }
            else
            {
                RemovePoints(((ListViewItemContext)e.Item.Tag).ParserType);
            }
        }

        private void AddPoints(Type parserType, bool redraw = true)
        {
            var parser = (IVisualizerParser)Activator.CreateInstance(parserType);
            var lineSeries = new LineSeries()
            {
                Title = parser.GetLabel(),
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                Tag = new SeriesContext()
                {
                    ParserType = parserType
                }
            };

            lineSeries.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    int indexOfNearestPoint = (int)Math.Round(e.HitTestResult.Index);
                    var nearestPoint = lineSeries.Transform(lineSeries.Points[indexOfNearestPoint]);
                    if ((nearestPoint - e.Position).Length < 5)
                    {
                        if (lastDoubleClickSeries == lineSeries && lastDoubleClickPointIndex == indexOfNearestPoint && (DateTime.Now - lastDoubleClickTime) < TimeSpan.FromMilliseconds(300))
                        {
                            tat.ScrollToLineNumber(((SeriesContext)lineSeries.Tag).PointIndexToLineNumber[indexOfNearestPoint]);
                            lastDoubleClickSeries = null;
                            lastDoubleClickPointIndex = -1;
                        }
                        else
                        {
                            lastDoubleClickSeries = lineSeries;
                            lastDoubleClickPointIndex = indexOfNearestPoint;
                            lastDoubleClickTime = DateTime.Now;
                        }
                    }
                }
            };

            if (logLines == null)
            {
                logLines = tat.GetFilteredLines();
            }

            foreach (var line in logLines)
            {
                var time = parser.GetTime(line.text);
                if (!time.HasValue)
                {
                    continue;
                }

                var value = parser.GetValue(line.text);
                if (!value.HasValue)
                {
                    continue;
                }

                lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(time.Value, value.Value));
                ((SeriesContext)lineSeries.Tag).PointIndexToLineNumber.Add(lineSeries.Points.Count - 1, line.lineNumber);
            }

            if (plot.Model == null)
            {
                plot.Model = new PlotModel
                {
                    Axes =
                    {
                        new DateTimeAxis()
                    }
                };
            }
            plot.Model.Series.Add(lineSeries);

            if (redraw)
            {
                plot.OnModelChanged();
            }
        }

        private void RemovePoints(Type parserType, bool redraw = true)
        {
            if (plot.Model == null)
            {
                return;
            }

            var it = plot.Model.Series.First(s => ((SeriesContext)s.Tag).ParserType == parserType);
            if (it != null)
            {
                plot.Model.Series.Remove(it);
            }

            if (plot.Model.Series.Count == 0)
            {
                plot.Model = null;
                logLines = null;
            }

            if (redraw)
            {
                plot.OnModelChanged();
            }
        }

        private void reloadBtn_Click(object sender, EventArgs e)
        {
            ReloadLogLines();
        }

        private void ReloadLogLines()
        {
            // Cleanup plot.
            plot.Model = null;

            // Add points.
            logLines = tat.GetFilteredLines();
            foreach (var item in parsersListView.CheckedItems)
            {
                AddPoints(((ListViewItemContext)((ListViewItem)item).Tag).ParserType, redraw: false);
            }

            // Redraw.
            plot.OnModelChanged();
        }

        private void loadParserBtn_Click(object sender, EventArgs e)
        {
            using (var frm = new OpenFileDialog())
            {
                frm.DefaultExt = "cs";
                frm.Filter = "CSharp files (*.cs)|*.cs";
                frm.InitialDirectory = Path.GetDirectoryName(parserFilePath == null ? typeof(IVisualizerParser).Assembly.Location : parserFilePath);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    parserFilePath = frm.FileName;
                    ReloadParser();
                }
            }
        }

        private void Visualizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Visualizer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                ReloadLogLines();
            }
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                CopyGraphToClipboard();
            }
        }

        private void reloadParserBtn_Click(object sender, EventArgs e)
        {
            ReloadParser();
        }

        private void ReloadParser()
        {
            // Cleanup plot.
            plot.Model = null;
            plot.OnModelChanged();

            // Cleanup parsers view.
            parsersListView.Items.Clear();

            // Clean data cache.
            logLines = null;

            // Add parsers.
            if (parserFilePath == null)
            {
                return;
            }

            using (var sr = new StreamReader(parserFilePath))
            {
                var context = sr.ReadToEnd();

                // Compile parser file.
                var ass = new string[]
                {
                            typeof(System.Text.RegularExpressions.Regex).Assembly.Location,
                            typeof(IVisualizerParser).Assembly.Location
                };

                var parameters = new CompilerParameters(ass)
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                };

                var res = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters, context);
                if (res.Errors.Count > 0)
                {
                    var err = res.Errors[0];
                    MessageBox.Show(string.Format("Compile failed! Line: {0}: {1}", err.Line, err.ErrorText));
                    return;
                }

                // Save parser type.
                try
                {
                    foreach (var parserType in res.CompiledAssembly.GetTypes().Where(t => typeof(IVisualizerParser).IsAssignableFrom(t) && !t.IsAbstract))
                    {
                        parsersListView.Items.Add(new ListViewItem()
                        {
                            Tag = new ListViewItemContext()
                            {
                                ParserType = parserType
                            },

                            SubItems =
                                    {
                                        new ListViewItem.ListViewSubItem()
                                        {
                                            Text = ((IVisualizerParser)Activator.CreateInstance(parserType)).GetLabel()
                                        }
                                    }
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Create parser failed! {0}", ex.Message));
                }
            }
        }

        private void copyBtn_Click(object sender, EventArgs e)
        {
            CopyGraphToClipboard();
        }
    }
}
