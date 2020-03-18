using LawlerBallisticsDesk.Classes;
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
    /// Interaction logic for frmAddCartridgePowder.xaml
    /// </summary>
    public partial class frmAddCartridgePowder : Window
    {
        private List<string> _PowderList;
        private string _SelectedPowderName;

        public List<string> PowderList { get { return _PowderList; } }
        public string SelectedPowderName { get { return _SelectedPowderName; } set { _SelectedPowderName = value; } }
        public frmAddCartridgePowder()
        {
            InitializeComponent();
            _PowderList = LawlerBallisticsFactory.PowderNameList;
            cboPwdrList.ItemsSource = PowderList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SelectedPowderName = "";
            this.Close();
        }
    }
}
