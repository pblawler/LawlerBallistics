using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using LawlerBallisticsDesk.Classes;
using LawlerBallisticsDesk.Views.Cartridges;

namespace LawlerBallisticsDesk.ViewModel
{
    public class CartridgesViewModel : ViewModelBase, IDisposable
    {

        #region "Binding"
        private void SelectedCartridge_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshViewModel();
        }
        private void RefreshViewModel()
        {
            RaisePropertyChanged("SelectedCartridge");
            RaisePropertyChanged("Cartridges");
        }
        #endregion

        #region "Private Variables"
        private Cartridge _TempCartridge;
        private Cartridge _SelectdedCartridge;
        private frmCartridge _frmCartridge;
        private List<string> _SelectedCartridgePowderList;
        private string _SelectedCartridgePowderName;
        #endregion

        #region "RelayCommands"
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _PwdrKeyUpCommand;
        public RelayCommand AddCartridgeCommand { get; set; }
        public RelayCommand LoadCartridgeCommand { get; set; }
        public RelayCommand SaveCartridgeCommand { get; set; }
        public RelayCommand SaveCartridgeDataCommand { get; set; }

        public RelayCommand<System.Windows.Input.KeyEventArgs> KeyUpCommand
        {
            get
            {
                return _KeyUpCommand ?? (_KeyUpCommand = new RelayCommand<System.Windows.Input.KeyEventArgs>((X) => KeyUp(X)));
            }
        }
        public RelayCommand<System.Windows.Input.KeyEventArgs> PwdrKeyUpCommand
        {
            get
            {
                return _PwdrKeyUpCommand ?? (_PwdrKeyUpCommand = new RelayCommand<System.Windows.Input.KeyEventArgs>((X) => PwdrKeyUp(X)));
            }
        }
        #endregion

        #region "Properties"
        public ObservableCollection<Cartridge> Cartridges { get { return LawlerBallisticsFactory.MyCartridges; } set { LawlerBallisticsFactory.MyCartridges = value; RaisePropertyChanged(nameof(Cartridges)); } }
        public Cartridge SelectedCartridge
        {
            get { return _SelectdedCartridge; }
            set
            {
                _SelectdedCartridge = value;
                _SelectedCartridgePowderList = new List<string>();
                if (_SelectdedCartridge.PowderIDlist != null)
                {
                    foreach (string lpid in _SelectdedCartridge.PowderIDlist)
                    {
                        _SelectedCartridgePowderList.Add(LawlerBallisticsFactory.GetPowderName(lpid));
                    }
                }
                RaisePropertyChanged(nameof(SelectedCartridge));
                RaisePropertyChanged(nameof(SelectedCartridgePowderList));
            }
        }
        public Cartridge TemporaryCartridge { get { return _TempCartridge; } set { _TempCartridge = value; RaisePropertyChanged(nameof(TemporaryCartridge)); } }
        public List<string> SelectedCartridgePowderList
        {
            get 
            {
                _SelectedCartridgePowderList = new List<string>();
                if (SelectedCartridge.PowderIDlist != null)
                {
                    foreach (string lp in SelectedCartridge.PowderIDlist)
                    {
                        _SelectedCartridgePowderList.Add(LawlerBallisticsFactory.GetPowderName(lp));
                    }
                }
                return _SelectedCartridgePowderList;
            }
            set { _SelectedCartridgePowderList = value; RaisePropertyChanged(nameof(SelectedCartridgePowderList)); }
        }
        public string SelectedCartridgePowderName { get { return _SelectedCartridgePowderName; } set { _SelectedCartridgePowderName = value; RaisePropertyChanged(nameof(SelectedCartridgePowderName)); } }
        #endregion

        #region "Constructor"
        public CartridgesViewModel()
        {
            AddCartridgeCommand = new RelayCommand(AddCartridge, null);
            LoadCartridgeCommand = new RelayCommand(LoadCartridge, null);
            SaveCartridgeCommand = new RelayCommand(SaveCartridge, null);
            SaveCartridgeDataCommand = new RelayCommand(SaveCartridgeCollection, null);
            _SelectdedCartridge = new Cartridge();
            SelectedCartridge.PropertyChanged += SelectedCartridge_PropertyChanged;
        }
        #endregion

        #region "Destructor"
        ~CartridgesViewModel()
        {
            SelectedCartridge.PropertyChanged -= SelectedCartridge_PropertyChanged;
        }
        #endregion

        #region "Private Routines"
        private void KeyUp(System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch(e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                        string lmsg = "Delete the selected cartridge?";
                        string lcaption = "Delete Cartridge Data";
                        MessageBoxButtons lbtns = MessageBoxButtons.YesNo;
                        DialogResult lrst = MessageBox.Show(lmsg, lcaption, lbtns, MessageBoxIcon.Warning);
                        if(lrst == DialogResult.Yes)
                        {
                            foreach(Cartridge lc in Cartridges)
                            {
                                if(SelectedCartridge.ID == lc.ID)
                                {
                                    Cartridges.Remove(lc);
                                    SelectedCartridge = null;
                                    break;
                                }
                            }
                        }                        
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        AddCartridge();
                        break;
                }
            }
            catch
            {

            }
        }
        private void PwdrKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                        string lmsg = "Delete the selected powder option?";
                        string lcaption = "Delete Cartridge Powder Option";
                        MessageBoxButtons lbtns = MessageBoxButtons.YesNo;
                        DialogResult lrst = MessageBox.Show(lmsg, lcaption, lbtns, MessageBoxIcon.Warning);
                        if (lrst == DialogResult.Yes)
                        {
                            foreach (string lp in SelectedCartridge.PowderIDlist)
                            {
                                if (LawlerBallisticsFactory.GetPowderName(lp) == SelectedCartridgePowderName)
                                {
                                    SelectedCartridge.PowderIDlist.Remove(lp);
                                    break;
                                }
                            }
                        }
                        SelectedCartridge.Refresh();
                        RaisePropertyChanged(nameof(SelectedCartridge));
                        RaisePropertyChanged(nameof(SelectedCartridgePowderList));
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        
                        break;
                }
            }
            catch
            {

            }
        }
        private void LoadCartridge()
        {
            if (frmCartridge.IsOpen)
            {
                MessageBox.Show("An instance of the cartridge window is currently open.");
                return;
            }            
            _frmCartridge = new frmCartridge();
            _frmCartridge.DataContext = this;
            _frmCartridge.ResizeMode = System.Windows.ResizeMode.NoResize;
            _frmCartridge.Show();
        }
        private void AddCartridge()
        {
            if (frmCartridge.IsOpen)
            {
                MessageBox.Show("An instance of the add cartridge window is currently open.");
                return;
            }
            _SelectdedCartridge = new Cartridge();
            _frmCartridge = new frmCartridge();
            _frmCartridge.DataContext = this;
            _frmCartridge.ResizeMode = System.Windows.ResizeMode.NoResize;
            _frmCartridge.Show();
        }
        private void SaveCartridge()
        {
            Cartridge lSC;
            foreach(Cartridge lItr in Cartridges)
            {
                if (lItr.ID == _SelectdedCartridge.ID)
                {
                    _frmCartridge.Close();
                    RaisePropertyChanged(nameof(Cartridges));
                    RaisePropertyChanged(nameof(SelectedCartridge));
                    return;
                }
            }
            Cartridges.Add(_SelectdedCartridge);
            _frmCartridge.Close();
            RaisePropertyChanged(nameof(Cartridges));
            RaisePropertyChanged(nameof(SelectedCartridge));
        }
        private void SaveCartridgeCollection()
        {
            LawlerBallisticsFactory.SaveMyCartridges();
        }
        private void InstanceUnload()
        {
            Cleanup();
            SimpleIoc.Default.Unregister<CartridgesViewModel>();
            SimpleIoc.Default.Register<CartridgesViewModel>();
        }
        #endregion

        #region "Public Routines"
        public void Dispose()
        {
            InstanceUnload();
        }
        public void AddPowder(string PowderName)
        {
            string lPwdrID;
            bool lLoaded = false;
            lPwdrID = LawlerBallisticsFactory.GetPowderID(PowderName);
            if (SelectedCartridge.PowderIDlist != null)
            {
                foreach (string lp in SelectedCartridge.PowderIDlist)
                {
                    if (lp == lPwdrID)
                    {
                        lLoaded = true;
                        break;
                    }
                }
            }
            if(!lLoaded) SelectedCartridge.PowderIDlist.Add(lPwdrID);
            SelectedCartridge.Refresh();
            RaisePropertyChanged(nameof(SelectedCartridgePowderList));
        }
        #endregion

    }
}
