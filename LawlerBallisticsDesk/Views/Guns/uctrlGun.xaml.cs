using LawlerBallisticsDesk.Classes;
using LawlerBallisticsDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LawlerBallisticsDesk.Views.Guns
{
    /// <summary>
    /// Interaction logic for uctrlGun.xaml
    /// </summary>
    public partial class uctrlGun : System.Windows.Controls.UserControl
    {
        public uctrlGun()
        {
            InitializeComponent();
        }
       
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dgBarrels.CommitEdit(DataGridEditingUnit.Row, true);
        }
        private void Image_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                System.Drawing.Image img = System.Drawing.Image.FromFile(files[0]);
                Uri resourceUri = new Uri(files[0], UriKind.Absolute);
                imgGun.Source = new BitmapImage(resourceUri);
                GunsViewModel lDC = (GunsViewModel) this.DataContext;
                lDC.SelectedGun.GunPic = img;
            }
        }
        private void imgGun_Loaded(object sender, RoutedEventArgs e)
        {
            GunsViewModel lDC = (GunsViewModel)this.DataContext;
            imgGun.Source = LawlerBallisticsFactory.ConvertToBitmap(lDC.SelectedGun.GunPic);
        }
    }
}
