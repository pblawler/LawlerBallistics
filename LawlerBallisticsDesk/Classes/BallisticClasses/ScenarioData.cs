using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes.BallisticClasses
{
    public class ScenarioData : INotifyPropertyChanged
    {
        #region "Binding"
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region "Private Variables"
        private double _ShooterLongitude;
        private double _ShooterLatitude;
        private double _ShotDistance;
        private double _ShotAngle;
        private double _TargetLatitude;
        private double _TargetLongitude;
        private double _RelHumidity;
        private double _BaroPressure;
        private double _DensityAlt;
        private double _TempF;
        private ZeroData _zeroData;
        #endregion

        #region "Properties"
        public ZeroData zeroData { get { return _zeroData; } set { _zeroData = value; RaisePropertyChanged(nameof(zeroData)); } }
        /// <summary>
        /// The longitude of the shooter in decimal degrees (DD).
        /// </summary>
        public double ShooterLon
        {
            get
            {
                if (_ShooterLongitude == 0) _ShooterLongitude = -86.988711;
                return _ShooterLongitude;
            }
            set
            {
                _ShooterLongitude = value;
                RaisePropertyChanged(nameof(ShooterLon));
            }
        }
        /// <summary>
        /// The latitude of the shooter in decimal degrees (DD).
        /// </summary>
        public double ShooterLat
        {
            get
            {
                if (_ShooterLatitude == 0) _ShooterLatitude = 34.681129;
                return _ShooterLatitude;
            }
            set
            {
                _ShooterLatitude = value;
                RaisePropertyChanged(nameof(ShooterLat));
            }
        }
        /// <summary>
        /// The line of sight distance to the target in yards.
        /// </summary>
        public double ShotDistance { get { return _ShotDistance; } set { _ShotDistance = value; RaisePropertyChanged(nameof(ShotDistance)); } }
        /// <summary>
        /// The absolute value of the vertical angle of the shot.
        /// </summary>
        public double ShotAngle { get { return _ShotAngle; } set { _ShotAngle = value; RaisePropertyChanged(nameof(ShotAngle)); } }
        /// <summary>
        /// The latitude of the target in decimal degrees (DD).
        /// </summary>
        public double TargetLat
        {
            get
            {
                if (_TargetLatitude == 0) _TargetLatitude = 34.681129;
                return _TargetLatitude;
            }
            set
            {
                _TargetLatitude = value;
                RaisePropertyChanged(nameof(TargetLat));
            }
        }
        /// <summary>
        /// The longitude of the target at decimal degrees (DD).
        /// </summary>
        public double TargetLon
        {
            get
            {
                if (_TargetLongitude == 0) _TargetLongitude = -86.987622;
                return _TargetLongitude;
            }
            set
            {
                _TargetLongitude = value;
                RaisePropertyChanged(nameof(TargetLon));
            }
        }
        /// <summary>
        /// The current relative humidity.
        /// </summary>
        public double RelHumidity
        {
            get
            {
                if (_RelHumidity == 0) _RelHumidity = 50.00;
                return _RelHumidity;
            }
            set
            {
                _RelHumidity = value;
                RaisePropertyChanged(nameof(RelHumidity));
            }
        }
        /// <summary>
        /// The barometric pressure in inches Hg.
        /// </summary>
        public double BaroPressure
        {
            get
            {
                if (_BaroPressure == 0) _BaroPressure = 29.92;
                return _BaroPressure;
            }
            set
            {
                _BaroPressure = value;
                RaisePropertyChanged(nameof(BaroPressure));
            }
        }
        /// <summary>
        /// Density Altitude
        /// </summary>
        public double DensityAlt
        {
            get
            {
                return _DensityAlt;
            }
            set
            {
                _DensityAlt = value;
                RaisePropertyChanged(nameof(DensityAlt));
            }
        }
        /// <summary>
        /// Ambient Temperature in fahrenheit.
        /// </summary>
        public double TempF
        {
            get
            {
                if (_TempF == 0) _TempF = 60.00;
                return _TempF;
            }
            set
            {
                _TempF = value;
                RaisePropertyChanged(nameof(TempF));
            }
        }
        #endregion
    }
}
