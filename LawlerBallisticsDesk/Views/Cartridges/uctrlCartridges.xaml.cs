using LawlerBallisticsDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;

namespace LawlerBallisticsDesk.Views.Cartridges
{
    /// <summary>
    /// Interaction logic for frmCartridges.xaml
    /// </summary>
    public partial class uctrlCartridges : UserControl
    {
        public uctrlCartridges()
        {
            InitializeComponent();
        }

        private void GoToHyperlink(object Sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void Label_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
