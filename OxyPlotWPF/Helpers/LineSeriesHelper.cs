using OxyPlot;
using System;

namespace OxyPlotWPF.Helpers
{
    public static class LineSeriesHelper
    {
        public static OxyColor GenerateColor()
        {
            Random random = new();

            byte red = (byte)random.Next(0, 240);
            byte green = (byte)random.Next(0, 240);
            byte blue = (byte)random.Next(0, 255);

            return OxyColor.FromRgb(red, green, blue);
        }
    }
}