using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OxyPlotWPF.Screens.MainWindow
{
    public class MainWindowViewModel : Screen
    {
        private const byte MeanStep = 15;
        private const byte MaxSeriesCount = 20;


        private OxyColor[] DefaultColors = { OxyColor.FromRgb(139, 0, 0), OxyColor.FromRgb(255, 160, 122) };
        private short _lastMeanValue = 5;
        private readonly List<LineSeries> _waves;
        private List<double[]> _generatedSinusoidals;

        private static CancellationTokenSource _cancellationTokenSource = new();

        private string _invalidData;
        public string InvalidData
        {
            get => _invalidData;
            set => Set(ref _invalidData, value);
        } 

        protected Visibility _visibilityInvalidData = Visibility.Hidden;
        public Visibility VisibilityInvalidData
        {
            get => _visibilityInvalidData;
            set => Set(ref _visibilityInvalidData, value);
        }

        private double _frequency = 100;

        public double Frequency
        {
            get => _frequency;
            set
            {
                //add regex "^-?\\d*(\\.\\d+)?$"

                var regex = "^-?\\d*(\\.\\d+)?$";
                var match = Regex.Match(value.ToString(), regex, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    Set(ref _frequency, value);
                }

            }
        }

        private double _amplitude = 5;

        public double Amplitude
        {
            get => _amplitude;
            set
            {
                Set(ref _amplitude, value);
            }
        }

        public PlotModel MultiplePlot { get; set; }

        #region Constructors

        public MainWindowViewModel()
        {
            _waves = new();
            _generatedSinusoidals = new();

            MultiplePlot = new PlotModel();

            Random random = new();

            for (int i = 0; i < 15; i++)
            {
                byte red = (byte)random.Next(0, 240);
                byte green = (byte)random.Next(0, 240);
                byte blue = (byte)random.Next(0, 255);

                var color = OxyColor.FromRgb(red, green, blue);

                LineSeries lineSeries = new() { Color = color };
                _waves.Add(lineSeries);
                MultiplePlot.Series.Add(lineSeries);
            }
            FillUpWaves();
        }

        #endregion

        #region General functions

        public void RunDiagramOneTime()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            int count = 5;

            foreach (var line in _waves)
            {
                UpdateWavesValues(line, count, token).ConfigureAwait(false);
                count += 15;
            }
        }

        public async void RunDiagramInLoop()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    int count = 5;

                    foreach (var line in _waves)
                    {
                        UpdateWavesValues(line, count, token).ConfigureAwait(false);
                        count += 15;
                    }

                     await Task.Delay(4000).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void StopDiagram()
        {
            _cancellationTokenSource.Cancel();
        }

        public async void AddNewSeriesToDiagram()
        {
            if (_waves.Count == MaxSeriesCount)
            {
                InvalidData = "You cannot to add more 20 series";
                VisibilityInvalidData = Visibility.Visible;

                await Task.Delay(5000);

                InvalidData = string.Empty;
                VisibilityInvalidData = Visibility.Hidden;

                return;
            }

            Random random = new();

            byte red = (byte)random.Next(0, 240);
            byte green = (byte)random.Next(0, 240);
            byte blue = (byte)random.Next(0, 255);

            LineSeries lineSeries = new() { Color = OxyColor.FromRgb(red, green, blue) };
            _waves.Add(lineSeries);
            MultiplePlot.Series.Add(lineSeries);

            SetDefaultValesForWave(lineSeries);
        } 
        
        public async void RemoveLastSeriesFromDiagram()
        {
            if (_waves.Count == 1)
            {
                InvalidData = "You cannot to remove all series";
                VisibilityInvalidData = Visibility.Visible;

                await Task.Delay(5000);

                InvalidData = string.Empty;
                VisibilityInvalidData = Visibility.Hidden;

                return;
            }

            _waves.Remove(_waves.Last());
            MultiplePlot.Series.Remove(MultiplePlot.Series.Last());

            _lastMeanValue -= MeanStep;

            MultiplePlot.InvalidatePlot(true);
        }

        #endregion

        #region Internal functions

        private void FillUpWaves()
        {
            foreach (var wave in _waves)
            {
                SetDefaultValesForWave(wave);
            }
        }

        private void SetDefaultValesForWave(LineSeries series)
        {
            for (int n = 0; n < 200; n++)
            {
                series.Points.Add(new DataPoint(n, _lastMeanValue));
            }

            _lastMeanValue += MeanStep;

            MultiplePlot.InvalidatePlot(true);
        }

        private async Task UpdateWavesValues(LineSeries lineSeries, int countValue, CancellationToken token)
        {
            int sampleRate = 1000;

            for (int n = 0; n < 200; n++)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                lineSeries.Points[n] = DataPoint.Undefined;
                lineSeries.Points[n + 1] = DataPoint.Undefined;
                await Task.Delay(1, token).ConfigureAwait(false);

                var t = (double)(Amplitude * Math.Sin((2 * Math.PI * n * Frequency) / sampleRate));
                lineSeries.Points[n] = new DataPoint(n, t + countValue);
                MultiplePlot.InvalidatePlot(true);
            }
        }

        #endregion
    }
}
