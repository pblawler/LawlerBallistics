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
    /// Interaction logic for frmPowders.xaml
    /// </summary>
    public partial class uctrlPowders : UserControl
    {
        public uctrlPowders()
        {
            InitializeComponent();
        }

        public static bool IsOpen { get; set; }

        private void DockableContent_Loaded(object sender, RoutedEventArgs e)
        {
            IsOpen = true;
        }

        private void DockableContent_Unloaded(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        private void DockableContent_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            IsOpen = false;
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    IsOpen = true;
        //}

        //private void Window_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    IsOpen = false;
        //}
    }
}
