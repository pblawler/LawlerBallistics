using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using LawlerBallisticsDesk.Classes;
using LawlerBallisticsDesk.Views.Ballistics;
using LawlerBallisticsDesk.Views.Maps;
using LawlerBallisticsDesk.Views.Guns;
using System.ComponentModel;
using LawlerBallisticsDesk.Views;

namespace LawlerBallisticsDesk.ViewModel
{
    public class GunsViewModel : ViewModelBase
    {
        #region "Binding"       
        private void SelectedGun_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshViewModel();
        }
        private void RefreshViewModel()
        {
            RaisePropertyChanged("SelectedGun");
            RaisePropertyChanged("MyGuns");
        }
        #endregion

        #region "Private Variables"
        Gun _SelectedGun;
        string _GunID;
        #endregion

        #region "Public Actions"
        public Action CloseAction { get; set; }
        public Action CloseGunAction { get; set; }
        public Action CloseBarrelAction { get; set; }

        #endregion

        #region "Public Properties"
        public Gun SelectedGun
        {
            get
            {
                return _SelectedGun;
            }
            set
            {
                _SelectedGun = value;
                RaisePropertyChanged(nameof(SelectedGun));
            }
        }
        public ObservableCollection<Gun> MyGuns
        {
            get
            {
                return LawlerBallisticsFactory.MyGuns;
            }
            set
            {
                LawlerBallisticsFactory.MyGuns = value;
                RaisePropertyChanged(nameof(MyGuns));
            }
        }
        public string GunID
        {
            get { return _GunID; }
            set { _GunID = value; }
        }
        public List<string> TwistDirectionNames 
        {
            get
            {
                return LawlerBallisticsFactory.TwistDirectionNames; 
            }
        }
        #endregion

        #region "Relay Commands"
        private RelayCommand _OpenBarrelCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpBarrelCommand;

        public RelayCommand OpenBarrelCommand
        {
            get
            {
                return _OpenBarrelCommand ?? (_OpenBarrelCommand = new RelayCommand(() => OpenBarrel()));
            }
        }
        public RelayCommand<System.Windows.Input.KeyEventArgs> KeyUpBarrelCommand
        {
            get
            {
                return _KeyUpBarrelCommand ?? (_KeyUpBarrelCommand = new RelayCommand<System.Windows.Input.KeyEventArgs>((X) => KeyUpBarrel(X)));
            }
        }
        public RelayCommand CloseWindowCommand { get; set; }
        public RelayCommand OpenGunCommand { get; set; }
        public RelayCommand AddGunCommand { get; set; }
        public RelayCommand LoadGunCommand { get; set; }
        public RelayCommand CloseGunWindowCommand { get; set; }
        public RelayCommand CloseBarrelWindowCommand { get; set; }
        public RelayCommand SaveGunDBCommand { get; set; }
        public RelayCommand AddBarrelCommand { get; set; }
        public RelayCommand LoadBarrelCommand { get; set; }
        #endregion

        #region "Constructor"
        public GunsViewModel()
        {
            GunID = Guid.NewGuid().ToString();
            CloseWindowCommand = new RelayCommand(CloseWindow, null);
            CloseBarrelWindowCommand = new RelayCommand(CloseBarrelWindow, null);
            CloseGunWindowCommand = new RelayCommand(CloseGunWindow, null);
            OpenGunCommand = new RelayCommand(OpenGun, null);
            AddGunCommand = new RelayCommand(AddGun, null);
            LoadGunCommand = new RelayCommand(LoadGun, null);
            SaveGunDBCommand = new RelayCommand(SaveGunDB, null);
            AddBarrelCommand = new RelayCommand(AddBarrel, null);
            LoadBarrelCommand = new RelayCommand(LoadBarrel, null);
        }
        #endregion

        #region "Private Routines"
        private void AddBarrel()
        {
            Barrel lb = new Barrel();

            SelectedGun.TargetBarrel = lb;
            frmBarrel lfb = new frmBarrel();
            lfb.RegisterClose();
            lfb.DataContext = this;
            lfb.Show();
        }
        private void LoadBarrel()
        {
            bool lF = false;

            foreach (Barrel lB in SelectedGun.Barrels)
            {
                if (lB.ID == SelectedGun.TargetBarrel.ID)
                {
                    lF = true;
                    break;
                }
            }
            SelectedGun.TargetBarrel.CartridgeID = SelectedGun.TargetBarrel.ParentCartridge.ID;
            if (!lF) SelectedGun.Barrels.Add(SelectedGun.TargetBarrel);
            RaisePropertyChanged(nameof(SelectedGun));
            CloseBarrelWindow();
        }
        private void CloseWindow()
        {
            CloseAction();
        }
        private void CloseGunWindow()
        {
            CloseGunAction();
        }
        private void CloseBarrelWindow()
        {
            CloseBarrelAction();
        }
        private void OpenGun()
        {
            frmGun lfrmGun = new frmGun();
            lfrmGun.DataContext = this;
            lfrmGun.RegClose();
            lfrmGun.Show();
        }
        private void OpenBarrel()
        {
            frmBarrel lfb = new frmBarrel();
            lfb.RegisterClose();
            lfb.DataContext = this;
            lfb.Show();
        }
        private void AddGun()
        {
            _SelectedGun = new Gun();
            frmGun lfrmGun = new frmGun();
            lfrmGun.DataContext = this;
            lfrmGun.RegClose();
            lfrmGun.Show();
        }
        private void LoadGun()
        {
            bool lF = false;

            foreach(Gun lG in MyGuns)
            {
                if(lG.ID == SelectedGun.ID)
                {
                    lF = true;
                    break;
                }
            }
            if(!lF) MyGuns.Add(SelectedGun);
            RaisePropertyChanged(nameof(MyGuns));
            CloseGunWindow();
        }
        private void SaveGunDB()
        {
            LawlerBallisticsFactory.SaveMyGuns();
        }
        private void KeyUpBarrel(System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                        string lmsg = "Delete the selected barrel?";
                        string lcaption = "Delete Barrel Data";
                        MessageBoxButtons lbtns = MessageBoxButtons.YesNo;
                        DialogResult lrst = MessageBox.Show(lmsg, lcaption, lbtns, MessageBoxIcon.Warning);
                        if (lrst == DialogResult.Yes)
                        {
                            //TODO: check to see if the barrel is used by other guns
                            foreach (Barrel lc in SelectedGun.Barrels)
                            {
                                if (SelectedGun.TargetBarrel.ID == lc.ID)
                                {
                                    SelectedGun.Barrels.Remove(lc);
                                    SelectedGun.TargetBarrel = null;
                                    break;
                                }
                            }
                        }
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        AddBarrel();
                        break;
                }
            }
            catch
            {

            }
        }
        #endregion

        #region "Public Routines"
        public void RefreshContent()
        {
            RaisePropertyChanged(nameof(MyGuns));
            RaisePropertyChanged(nameof(SelectedGun));
        }
        #endregion
    }
}
