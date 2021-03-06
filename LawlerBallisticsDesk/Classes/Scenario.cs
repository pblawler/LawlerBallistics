﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class Scenario : INotifyPropertyChanged
    {
        #region "Binding"
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void MyAtmospherics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshViewModel();
        }
        private void MyShooter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(MyShooter));
        }
        private void RefreshViewModel()
        {
            RaisePropertyChanged(nameof(MyAtmospherics));
        }

        #endregion

        #region "Private Variables"
        private Atmospherics _MyAtmospherics;
        private Shooter _MyShooter;
        private ObservableCollection<Target> _Targets;
        private Target _SelectedTarget;
        #endregion

        #region "Properties"
        public Atmospherics MyAtmospherics
        {
            get
            {
                return _MyAtmospherics;
            }
            set
            {
                _MyAtmospherics = value;
                RaisePropertyChanged(nameof(MyAtmospherics));
            }
        }
        public Shooter MyShooter { get { return _MyShooter; } set { _MyShooter = value; RaisePropertyChanged(nameof(MyShooter)); } }
        public ObservableCollection<Target> Targets { get { return _Targets; } set { _Targets = value; RaisePropertyChanged(nameof(Targets)); } }
        public Target SelectedTarget { get { return _SelectedTarget; } set { _SelectedTarget = value; RaisePropertyChanged(nameof(SelectedTarget)); } }
        #endregion

        #region "Constructor"
        public Scenario()
        {
            MyAtmospherics.PropertyChanged += MyAtmospherics_PropertyChanged;
            MyShooter.PropertyChanged += MyShooter_PropertyChanged;
        }
        #endregion

        #region "Destructor"
        ~Scenario()
        {
            MyAtmospherics.PropertyChanged -= MyAtmospherics_PropertyChanged;
            MyShooter.PropertyChanged -= MyShooter_PropertyChanged;
        }
        #endregion

    }
}