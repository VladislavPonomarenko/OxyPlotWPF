using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Series;
using OxyPlotWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OxyPlotWPF.Screens.MainWindow
{
    public class MainWindowViewModel : Screen
    {
        #region Consts

        private const byte MeanStep = 15;
        private const byte MaxSeriesCount = 20;

        #endregion

        #region Members

        private short _lastMeanValue = 5;
        private readonly List<LineSeries> _waves;
        private short _sampleRate = 1000;
        private static CancellationTokenSource _cancellationTokenSource = new();

        #endregion

        #region Properties

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
            set => Set(ref _frequency, value);
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

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            _waves = new();

            MultiplePlot = new PlotModel();

            for (int i = 0; i < 15; i++)
            {
                LineSeries lineSeries = new() { Color = LineSeriesHelper.GenerateColor() };
                _waves.Add(lineSeries);
                MultiplePlot.Series.Add(lineSeries);
            }

            FillUpWavesByDefault();
        }

        #endregion

        #region General functions

        public async void RunDiagramOneTimeAsync()
        {
            try
            {
                CancellationToken token = RestartThread();

                await Task.Run(() =>
                {
                    UpdateWavesValuesAsync(token).Wait();
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async void RunDiagramInLoopAsync()
        {
            CancellationToken token = RestartThread();

            try
            {
                while (!token.IsCancellationRequested)
                {
                   await Task.Run(() => 
                   {
                        UpdateWavesValuesAsync(token).Wait();
                   });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void StopDiagram()
        {
            _cancellationTokenSource?.Cancel();
        }

        public async void AddNewSeriesToDiagramAsync()
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

            LineSeries lineSeries = new() { Color = LineSeriesHelper.GenerateColor() };
            _waves.Add(lineSeries);
            MultiplePlot.Series.Add(lineSeries);

            SetDefaultValesForWave(lineSeries);
        } 
        
        public async void RemoveLastSeriesFromDiagramAsync()
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
       
        private void FillUpWavesByDefault()
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

        private async Task UpdateWavesValuesAsync(CancellationToken token)
        {
            
                for (short index = 0; index < 200; index++) 
            {
                short lastMeanValue = 5;

                foreach (var lineSeries in _waves)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    lineSeries.Points[index] = DataPoint.Undefined;
                    if(index != 199)
                        lineSeries.Points[index + 1] = DataPoint.Undefined;
                    
                    var sinResult = (double)(Amplitude * Math.Sin((2 * Math.PI * index * Frequency) / _sampleRate));
                    lineSeries.Points[index] = new DataPoint(index, sinResult + lastMeanValue);

                    MultiplePlot.InvalidatePlot(true);

                    lastMeanValue += MeanStep;
                }
                await Task.Delay(1);
            }
        }

        private CancellationToken RestartThread()
        {
            _cancellationTokenSource?.Cancel();

            _cancellationTokenSource = new CancellationTokenSource();

            return _cancellationTokenSource.Token;
        }

        #endregion
    }
}