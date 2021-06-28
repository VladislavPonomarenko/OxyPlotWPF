using Caliburn.Micro;
using OxyPlotWPF.Screens.MainWindow;
using System.Windows;

namespace OxyPlotWPF.ApplicationContext
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }
    }
}
