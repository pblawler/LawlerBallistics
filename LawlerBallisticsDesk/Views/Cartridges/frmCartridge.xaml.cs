using LawlerBallisticsDesk.Classes;
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

namespace LawlerBallisticsDesk.Views.Cartridges
{
    /// <summary>
    /// Interaction logic for frmCartridge.xaml
    /// </summary>
    public partial class frmCartridge : Window
    {
        public static bool IsOpen { get; private set; }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsOpen = true;
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }
        public frmCartridge()
        {
            InitializeComponent();
        }
        private void Image_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                System.Drawing.Image img = System.Drawing.Image.FromFile(files[0]);
                Uri resourceUri = new Uri(files[0], UriKind.Absolute);
                imgCartridge.Source = new BitmapImage(resourceUri);
                CartridgesViewModel lDC = (CartridgesViewModel) this.DataContext;
                lDC.SelectedCartridge.CartridgePic = img;
            }
        }
        private void imgCartridge_Loaded(object sender, RoutedEventArgs e)
        {
            CartridgesViewModel lDC = (CartridgesViewModel) this.DataContext;
            imgCartridge.Source = LawlerBallisticsFactory.ConvertToBitmap(lDC.SelectedCartridge.CartridgePic);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            frmAddCartridgePowder lfrm = new frmAddCartridgePowder();
            lfrm.ShowDialog();
            if(lfrm.SelectedPowderName !="")
            {
                CartridgesViewModel lDC = (CartridgesViewModel)this.DataContext;
                lDC.AddPowder(lfrm.SelectedPowderName);

            }
            
        }
    }
}
