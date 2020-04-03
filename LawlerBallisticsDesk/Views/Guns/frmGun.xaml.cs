using GalaSoft.MvvmLight.Messaging;
using LawlerBallisticsDesk.Classes;
using LawlerBallisticsDesk.Classes.Guns;
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
        private GunsViewModel _DC;

        #region "Messaging"
        private void ReceiveGunMessage(GunBarrelMsg msg)
        {
            if (msg.GunID == _DC.GunID)
            {
                uctrlBRp.DataContext = new BarrelRecipeViewModel(msg.SelectedBarrelID, _DC.SelectedGun.ID);
            }
        }
        #endregion

        //TODO:  Add a message for when a barrel is selected and listen for it in this form to
        // send the barrelID to the recipes control for this form.
        public frmGun()
        {
            InitializeComponent();            
            Messenger.Default.Register<GunBarrelMsg>(this, (Msg) => ReceiveGunMessage(Msg));
            try
            {
                if (_DC.SelectedGun.SelectedBarrel.ID != "") uctrlBRp.DataContext = new BarrelRecipeViewModel(_DC.SelectedGun.SelectedBarrel.ID, _DC.SelectedGun.ID);
            }
            catch
            { }

        }

        public void RegClose()
        {
            _DC = (GunsViewModel)this.DataContext;
            _DC.CloseGunAction = new Action(this.Close);
            try
            {
                if (_DC.SelectedGun.SelectedBarrel.ID != "") uctrlBRp.DataContext = new BarrelRecipeViewModel(_DC.SelectedGun.SelectedBarrel.ID, _DC.SelectedGun.ID);
            }
            catch
            { }

        }

    }
}
