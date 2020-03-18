using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LawlerBallisticsDesk.Classes;
using LawlerBallisticsDesk.Views.Cartridges;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace LawlerBallisticsDesk.ViewModel
{
    public class PrimersViewModel : ViewModelBase
    {
        #region "Relay Command"
        private RelayCommand _AddPrimerCommand;
        private RelayCommand _OpenPrimerCommand;
        private RelayCommand _SavePrimersCommand;
        private RelayCommand _SavePrimerCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpCommand;

        public RelayCommand AddPrimerCommand
        {
            get
            {
                return _AddPrimerCommand ?? (_AddPrimerCommand = new RelayCommand(() => AddPrimer()));
            }
        }
        public RelayCommand OpenPrimerCommand
        {
            get
            {
                return _OpenPrimerCommand ?? (_OpenPrimerCommand = new RelayCommand(() => OpenPrimer()));
            }
        }
        public RelayCommand SavePrimersCommand
        {
            get
            {
                return _SavePrimersCommand ?? (_SavePrimersCommand = new RelayCommand(() => SavePrimers()));
            }
        }
        public RelayCommand SavePrimerCommand
        {
            get
            {
                return _SavePrimerCommand ?? (_SavePrimerCommand = new RelayCommand(() => SavePrimer()));
            }
        }
        public RelayCommand<System.Windows.Input.KeyEventArgs> KeyUpCommand
        {
            get
            {
                return _KeyUpCommand ?? (_KeyUpCommand = new RelayCommand<System.Windows.Input.KeyEventArgs>((X) => KeyUp(X)));
            }
        }
        #endregion

        #region "Private Variables"
        private Primer _SelectedPrimer;
        private frmPrimer _frmPrimer;
        #endregion

        #region "Properties"
        public ObservableCollection<Primer> MyPrimers
        {
            get { return LawlerBallisticsFactory.MyPrimers; }
            set { LawlerBallisticsFactory.MyPrimers = value; RaisePropertyChanged(nameof(MyPrimers)); }
        }
        public Primer SelectedPrimer
        {
            get { return _SelectedPrimer; }
            set { _SelectedPrimer = value; RaisePropertyChanged(nameof(SelectedPrimer)); }
        }
        public List<String> PrimerType { get { return LawlerBallisticsFactory.PrimerTypeNames; } }
        #endregion

        #region "Constructor"
        public PrimersViewModel()
        {

        }
        #endregion

        #region "Private Routines"
        private void AddPrimer()
        {
            if (frmPrimer.IsOpen)
            {
                System.Windows.MessageBox.Show("An instance of the add primer window is currently open.");
                return;
            }
            _SelectedPrimer = new Primer();
            _frmPrimer = new frmPrimer();
            _frmPrimer.DataContext = this;
            _frmPrimer.ResizeMode = System.Windows.ResizeMode.NoResize;
            _frmPrimer.Show();
        }
        private void OpenPrimer()
        {
            _frmPrimer = new frmPrimer();
            _frmPrimer.Show();
        }
        private void SavePrimer()
        {
            bool ls = false;
            foreach (Primer lItr in MyPrimers)
            {
                if (lItr.ID == _SelectedPrimer.ID)
                {
                    ls = true;
                    break;
                }
            }
            if (!ls) MyPrimers.Add(_SelectedPrimer);
            _frmPrimer.Close();
            RaisePropertyChanged(nameof(MyPrimers));
            RaisePropertyChanged(nameof(SelectedPrimer));
        }
        private void SavePrimers()
        {
            LawlerBallisticsFactory.SaveMyPrimers();
        }
        private void KeyUp(System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                        string lmsg = "Delete the selected primer?";
                        string lcaption = "Delete Primer Data";
                        MessageBoxButtons lbtns = MessageBoxButtons.YesNo;
                        DialogResult lrst = System.Windows.Forms.MessageBox.Show(lmsg, lcaption, lbtns, MessageBoxIcon.Warning);
                        if (lrst == DialogResult.Yes)
                        {
                            foreach (Primer lc in MyPrimers)
                            {
                                if (SelectedPrimer.ID == lc.ID)
                                {
                                    MyPrimers.Remove(lc);
                                    SelectedPrimer = null;
                                    break;
                                }
                            }
                        }
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        AddPrimer();
                        break;
                }
            }
            catch
            {

            }
        }
        #endregion
    }
}
