using LawlerBallisticsDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LawlerBallisticsDesk.Views.Ballistics
{
    /// <summary>
    /// Interaction logic for frmBallisticCalculator.xaml
    /// </summary>
    public partial class frmBallisticCalculator : Window
    {
        public frmBallisticCalculator()
        {
            InitializeComponent();

            SolutionViewModel lDC = (SolutionViewModel) this.DataContext;
            lDC.LoadDefaultSolution();
            chkMaxRise_Click(null, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.DataContext = null;
            GC.Collect();
        }

        private void chkMaxRise_Click(object sender, RoutedEventArgs e)
        {
            var lconverter = new System.Windows.Media.BrushConverter();
            Brush lbg = (Brush)lconverter.ConvertFromString("#FFEAEAEA");
            Brush lbg1 = (Brush)lconverter.ConvertFromString("#FFFFFFFF");

            if ((bool)chkMaxRise.IsChecked)
            {
                txtZeroR.Background = lbg;
                txtZeroR.IsReadOnly = true;
                txtMaxRise.Background = lbg1;
                txtMaxRise.IsReadOnly = false;
            }
            else
            {
                txtZeroR.Background = lbg1;
                txtZeroR.IsReadOnly = false;
                txtMaxRise.Background = lbg;
                txtMaxRise.IsReadOnly = true;
            }
        }
    }
}
