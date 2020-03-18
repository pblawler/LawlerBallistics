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
using System.Windows.Forms;

namespace LawlerBallisticsDesk.ViewModel
{
    public class CasesViewModel : ViewModelBase
    {
        #region "Relay Command"
        private RelayCommand _AddCaseCommand;
        private RelayCommand _OpenCaseCommand;
        private RelayCommand _SaveCasesCommand;
        private RelayCommand _SaveCaseCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpCommand;

        public RelayCommand AddCaseCommand
        {
            get
            {
                return _AddCaseCommand ?? (_AddCaseCommand = new RelayCommand(() => AddCase()));
            }
        }
        public RelayCommand OpenCaseCommand
        {
            get
            {
                return _OpenCaseCommand ?? (_OpenCaseCommand = new RelayCommand(() => OpenCase()));
            }
        }
        public RelayCommand SaveCasesCommand
        {
            get
            {
                return _SaveCasesCommand ?? (_SaveCasesCommand = new RelayCommand(() => SaveCases()));
            }
        }
        public RelayCommand SaveCaseCommand
        {
            get
            {
                return _SaveCaseCommand ?? (_SaveCaseCommand = new RelayCommand(() => SaveCase()));
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
        private Case _SelectedCase;
        private frmCase _frmCase;
        #endregion

        #region "Properties"
        public ObservableCollection<Case> MyCases
        {
            get{ return LawlerBallisticsFactory.MyCases; }
            set { LawlerBallisticsFactory.MyCases = value; RaisePropertyChanged(nameof(MyCases)); } }
        public Case SelectedCase
        {
            get { return _SelectedCase; }
            set { _SelectedCase = value; RaisePropertyChanged(nameof(SelectedCase)); } 
        }
        public List<String> Cartridges { get { return LawlerBallisticsFactory.CartridgeNames; } }
        public List<string> PrimerTypes { get { return LawlerBallisticsFactory.PrimerTypeNames; } }
        #endregion

        #region "Constructor"
        public CasesViewModel()
        {

        }
        #endregion

        #region "Private Routines"
        private void AddCase()
        {
            if (frmCase.IsOpen)
            {
                MessageBox.Show("An instance of the add case window is currently open.");
                return;
            }
            _SelectedCase = new Case();
            _frmCase = new frmCase();
            _frmCase.DataContext = this;
            _frmCase.ResizeMode = System.Windows.ResizeMode.NoResize;
            _frmCase.Show();
        }
        private void OpenCase()
        {
            _frmCase = new frmCase();
            _frmCase.Show();
        }
        private void SaveCases()
        {
            LawlerBallisticsFactory.SaveMyCases();
        }
        private void SaveCase()
        {
            if (_frmCase != null) _frmCase.Close();
            bool ls = false;
            foreach (Case lItr in MyCases)
            {
                if (lItr.ID == _SelectedCase.ID)
                {
                    ls = true;
                    break;
                }
            }
            if (!ls) MyCases.Add(_SelectedCase);
            RaisePropertyChanged(nameof(MyCases));
            RaisePropertyChanged(nameof(SelectedCase));
        }
        private void KeyUp(System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                        string lmsg = "Delete the selected case?";
                        string lcaption = "Delete Case Data";
                        MessageBoxButtons lbtns = MessageBoxButtons.YesNo;
                        DialogResult lrst = MessageBox.Show(lmsg, lcaption, lbtns, MessageBoxIcon.Warning);
                        if (lrst == DialogResult.Yes)
                        {
                            foreach (Case lc in MyCases)
                            {
                                if (SelectedCase.ID == lc.ID)
                                {
                                    MyCases.Remove(lc);
                                    SelectedCase = null;
                                    break;
                                }
                            }
                        }
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        AddCase();
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
