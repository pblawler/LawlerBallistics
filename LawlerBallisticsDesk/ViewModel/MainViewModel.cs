using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LawlerBallisticsDesk.Classes;
using LawlerBallisticsDesk.Views;
using LawlerBallisticsDesk.Views.Ballistics;
using LawlerBallisticsDesk.Views.Cartridges;
using LawlerBallisticsDesk.Views.Guns;
using System;

namespace LawlerBallisticsDesk.ViewModel
{

    public class MainViewModel : ViewModelBase, IDisposable
    {
        //TODO:  Make sure all classes with a name property have unique names before loading to collection.
        #region "Private Variables"
        private bool _MovedGuns = false;
        private bool _MovedRecipes = false;
        uctrlGuns _Gunsform;
        #endregion

        #region "Public Properties"
        public Action CloseAction { get; set; }
        #endregion

        #region "Relay Commands"
        public RelayCommand OpenBCestimateCommand { get; set; }
        public RelayCommand OpenBcalculatorCommand { get; set; }
        public RelayCommand OpenDockCommand { get; set; }

        public RelayCommand ExitCommand { get; set; }        
        #endregion

        #region "Private Routines"
        private void OpenBCestimate()
        {
            frmBCcalculator lBCform = new frmBCcalculator();
            lBCform.Show();
            SolutionViewModel bcvm = (SolutionViewModel)lBCform.DataContext;
            bcvm.SetBCestimatefrm(lBCform);
        }
        private void OpenBcalculator()
        {
            frmBallisticCalculator lBCform = new frmBallisticCalculator();
            lBCform.Show();
        }
        private void ExitFunc()
        {
            //System.Environment.Exit(0);
            CloseAction();
        }
        #endregion

        #region "Constructor"
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            LawlerBallisticsFactory.InitializeFactory();
            OpenBCestimateCommand = new RelayCommand(OpenBCestimate, null);
            OpenBcalculatorCommand = new RelayCommand(OpenBcalculator, null);
            ExitCommand = new RelayCommand(ExitFunc, null);
        }
        #endregion

        #region "Public Routines"
        public void Dispose()
        {
            
        }
        #endregion

    }
}