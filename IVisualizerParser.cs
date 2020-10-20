using System;

namespace TextAnalysisTool.NET.Plugin
{
    public interface IVisualizerParser
    {
        // Return the name of the parser.
        string GetLabel();

        // Return the date time of the log line.
        DateTime? GetTime(string logline);

        // Return the value of the log line.
        double? GetValue(string logline);
    }
}
