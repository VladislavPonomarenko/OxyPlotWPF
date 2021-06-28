using OxyPlotWPF.ApplicationContext;
using System.Windows;

namespace OxyPlotWPF
{
    public partial class App : Application
    {
        private Bootstrapper _bootstrapper;

        public App()
        {
            _bootstrapper = new Bootstrapper();
        }
    }
}
