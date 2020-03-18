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

namespace LawlerBallisticsDesk.Views.Cartridges
{
    /// <summary>
    /// Interaction logic for frmCreateCartridgeLot.xaml
    /// </summary>
    public partial class frmCreateCartridgeLot : Window
    {
        private Int32 _LotSize;

        public Int32 LotSize { get { return _LotSize; } set { _LotSize = value; } }
        public frmCreateCartridgeLot()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LotSize = 0;
            this.Close();
        }
    }
}
