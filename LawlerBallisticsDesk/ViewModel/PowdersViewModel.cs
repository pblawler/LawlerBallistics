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
    public class PowdersViewModel : ViewModelBase
    {
        #region "Relay Command"
        private RelayCommand _AddPowderCommand;
        private RelayCommand _OpenPowderCommand;
        private RelayCommand _SavePowdersCommand;
        private RelayCommand _SavePowderCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpCommand;

        public RelayCommand AddPowderCommand
        {
            get
            {
                return _AddPowderCommand ?? (_AddPowderCommand = new RelayCommand(() => AddPowder()));
            }
        }
        public RelayCommand OpenPowderCommand
        {
            get
            {
                return _OpenPowderCommand ?? (_OpenPowderCommand = new RelayCommand(() => OpenPowder()));
            }
        }
        public RelayCommand SavePowdersCommand
        {
            get
            {
                return _SavePowdersCommand ?? (_SavePowdersCommand = new RelayCommand(() => SavePowders()));
            }
        }
        public RelayCommand SavePowderCommand
        {
            get
            {
                return _SavePowderCommand ?? (_SavePowderCommand = new RelayCommand(() => SavePowder()));
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
        private Powder _SelectedPowder;
        private frmPowder _frmPowder;
        #endregion

        #region "Properties"
        public ObservableCollection<Powder> MyPowders
        {
            get { return LawlerBallisticsFactory.MyPowders; }
            set { LawlerBallisticsFactory.MyPowders = value; RaisePropertyChanged(nameof(MyPowders)); }
        }
        public Powder SelectedPowder
        {
            get { return _SelectedPowder; }
            set { _SelectedPowder = value; RaisePropertyChanged(nameof(SelectedPowder)); }
        }
        public List<string> BaseTypeNames { get { return LawlerBallisticsFactory.PowderBaseTypeNames; } }
        #endregion

        #region "Constructor"
        public PowdersViewModel()
        {

        }
        #endregion

        #region "Private Routines"
        private void AddPowder()
        {
            if (frmPowder.IsOpen)
            {
                System.Windows.MessageBox.Show("An instance of the add powder window is currently open.");
                return;
            }
            _SelectedPowder = new Powder();
            _frmPowder = new frmPowder();
            _frmPowder.DataContext = this;
            _frmPowder.ResizeMode = System.Windows.ResizeMode.NoResize;
            _frmPowder.Show();
        }
        private void OpenPowder()
        {
            frmPowder lPowder = new frmPowder();
            lPowder.Show();
        }
        private void SavePowder()
        {
            bool ls = false;
            foreach (Powder lItr in MyPowders)
            {
                if (lItr.ID == _SelectedPowder.ID)
                {
                    ls = true;
                    break;
                }
            }
            if (!ls) MyPowders.Add(_SelectedPowder);
            _frmPowder.Close();
            RaisePropertyChanged(nameof(MyPowders));
            RaisePropertyChanged(nameof(SelectedPowder));
        }
        private void SavePowders()
        {
            LawlerBallisticsFactory.SaveMyPowders();
        }
        private void KeyUp(System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                        string lmsg = "Delete the selected powder?";
                        string lcaption = "Delete Powder Data";
                        MessageBoxButtons lbtns = MessageBoxButtons.YesNo;
                        DialogResult lrst = System.Windows.Forms.MessageBox.Show(lmsg, lcaption, lbtns, MessageBoxIcon.Warning);
                        if (lrst == DialogResult.Yes)
                        {
                            foreach (Powder lc in MyPowders)
                            {
                                if (SelectedPowder.ID == lc.ID)
                                {
                                    MyPowders.Remove(lc);
                                    SelectedPowder = null;
                                    break;
                                }
                            }
                        }
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        AddPowder();
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
