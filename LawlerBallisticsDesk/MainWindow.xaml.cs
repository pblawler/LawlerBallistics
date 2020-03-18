using LawlerBallisticsDesk.ViewModel;
using LawlerBallisticsDesk.Views.Cartridges;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LawlerBallisticsDesk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel lvm;
        public MainWindow()
        {
            InitializeComponent();                       
            lvm = (MainViewModel)this.DataContext;            
            lvm.CloseAction = new Action(this.Close);            
        }
    }
}
