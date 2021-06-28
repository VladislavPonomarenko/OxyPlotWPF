using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OxyPlotWPF.Screens.MainWindow
{
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb == null) return;

            if (string.IsNullOrEmpty(tb.Text))
            {
                return;
            }

            if (!int.TryParse(tb.Text, out int result))
            {
               // new DialogInfo(new InfoDialogViewModel("Info", "Incorrect value")).ShowDialog();
            }
            else
                Validation.ClearInvalid(((TextBox)sender).GetBindingExpression(TextBox.TextProperty));
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
