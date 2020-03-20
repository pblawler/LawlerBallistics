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

namespace LawlerBallisticsDesk.Views
{
    /// <summary>
    /// Interaction logic for frmGun.xaml
    /// </summary>
    public partial class frmGun : Window
    {
        public frmGun()
        {
            InitializeComponent();
        }

        public void RegClose()
        {
            GunsViewModel lvm;
            lvm = (GunsViewModel)this.DataContext;
            lvm.CloseGunAction = new Action(this.Close);

        }
    }
}
