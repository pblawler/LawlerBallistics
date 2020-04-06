using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes.BallisticClasses
{
    public class ZeroData : INotifyPropertyChanged
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
        private LoadOutData _loadOutData;
        private double _zTargetLatitude;
        private double _zTargetLongitude;
        private double _zShooterLatitude;
        private double _zShooterLongitude;
        private double _zRelHumidity;
        private double _zBaroPressure;
        private double _zDensityAlt;
        private double _zTempF;
        private double _Hm;
        private double _ZeroRange;
        private bool _UseMaxRise;
        private DragSlopeData _dragSlopeData;
        #endregion

        #region "Private Constants"
        private const double _GravFt = 32.17405;
        private const double _GravIn = 386.0886;
        private const double _G = 41.68211487;      //G = 3*(g(in/s^2)/2)^0.5 = 41.68
        #endregion

        #region "Properties"
        public DragSlopeData dragSlopeData
        {
            get { return _dragSlopeData; }
            set { _dragSlopeData = value; RaisePropertyChanged(nameof(dragSlopeData)); }
        }
        public LoadOutData loadOutData { get { return _loadOutData; } set { _loadOutData = value; RaisePropertyChanged(nameof(loadOutData)); } }
        /// <summary>
        /// Ambient Temperature in fahrenheit when zeroed.
        /// </summary>
        public double zTempF
        {
            get
            {
                if (_zTempF == 0) _zTempF = 60.00;
                return _zTempF;
            }
            set
            {
                _zTempF = value;
                RaisePropertyChanged(nameof(zTempF));
            }
        }
        /// <summary>
        /// Density Altitude at time of zero.
        /// </summary>
        public double zDensityAlt
        {
            get
            {
                return _zDensityAlt;
            }
            set
            {
                _zDensityAlt = value;
                RaisePropertyChanged(nameof(zDensityAlt));
            }
        }
        /// <summary>
        /// The relative humidity when zeroed.
        /// </summary>
        public double zRelHumidity
        {
            get
            {
                if (_zRelHumidity == 0) _zRelHumidity = 50.00;
                return _zRelHumidity;
            }
            set
            {
                _zRelHumidity = value;
                RaisePropertyChanged(nameof(zRelHumidity));
            }
        }
        /// <summary>
        /// The barometric pressure in inches Hg at sight in.
        /// </summary>
        public double zBaroPressure
        {
            get
            {
                if (_zBaroPressure == 0) _zBaroPressure = 29.92;
                return _zBaroPressure;
            }
            set
            {
                _zBaroPressure = value;
                RaisePropertyChanged(nameof(zBaroPressure));
            }
        }
        /// <summary>
        /// The latitude of the target at zero in decimal degrees (DD).
        /// </summary>
        public double zTargetLat
        {
            get
            {
                if (_zTargetLatitude == 0) _zTargetLatitude = 34.681129;
                return _zTargetLatitude;
            }
            set
            {
                _zTargetLatitude = value;
                RaisePropertyChanged(nameof(zTargetLat));
            }
        }
        /// <summary>
        /// The longitude of the target at zero in decimal degrees (DD).
        /// </summary>
        public double zTargetLon
        {
            get
            {
                if (_zTargetLongitude == 0) _zTargetLongitude = -86.987622;
                return _zTargetLongitude;
            }
            set
            {
                _zTargetLongitude = value;
                RaisePropertyChanged(nameof(zTargetLon));
            }
        }
        /// <summary>
        /// The latitude of the shooter at zero time in decimal degrees (DD).
        /// </summary>
        public double zShooterLat
        {
            get
            {
                if (_zShooterLatitude == 0) _zShooterLatitude = 34.681129;
                return _zShooterLatitude;
            }
            set
            {
                _zShooterLatitude = value;
                RaisePropertyChanged(nameof(zShooterLat));
            }
        }
        /// <summary>
        /// The longitude of the shooter at zero in decimal degrees (DD).
        /// </summary>
        public double zShooterLon
        {
            get
            {
                if (_zShooterLongitude == 0) _zShooterLongitude = -86.988711;
                return _zShooterLongitude;
            }
            set
            {
                _zShooterLongitude = value;
                RaisePropertyChanged(nameof(zShooterLon));
            }
        }
        /// <summary>
        /// The maximum bullet rise from the muzzle to the zero range.
        /// </summary>
        public double ZeroMaxRise
        {
            get
            {
                if ((UseMaxRise) & (_Hm == 0)) _Hm = 2.00;
                return _Hm;
            }
            set
            {
                _Hm = value;
                RaisePropertyChanged(nameof(ZeroMaxRise));
            }
        }
        /// <summary>
        /// The range the gun is sighted in to have zero vertical deviation.
        /// </summary>
        public double ZeroRange
        {
            get
            {
                if ((_ZeroRange == 0) & (!UseMaxRise)) _ZeroRange = 200.00;
                return _ZeroRange;
            }
            set
            {
                _ZeroRange = value;
                RaisePropertyChanged(nameof(ZeroRange));
            }
        }
        /// <summary>
        /// True if the zero range is to be calculated from the maximum desired rise above the sight plane
        /// on the trajectory to zero.
        /// </summary>
        public bool UseMaxRise { get { return _UseMaxRise; } set { _UseMaxRise = value; RaisePropertyChanged(nameof(UseMaxRise)); } }
        /// <summary>
        /// Midrange where PBR height (Hm) or maximum rise occurs
        /// </summary>
        public double MidRange
        {
            get
            {
                double lMR;
                //M = 1/(((G/Vo)/((Hm+S)^0.5)) + 2/Fo)
                lMR = 1 / (((_G /loadOutData.MuzzleVelocity) / (Math.Pow((_Hm + loadOutData.ScopeHeight), 0.5))) + 2 / Fo); ;
                return lMR;
            }
        }
        /// <summary>
        /// Calculate Point Blank Range Distance
        /// </summary>
        public double PointBlankRange
        {
            get
            {
                double lSQ; double lP;

                //P = (1 + SQ)/(1/Fo + SQ/M)
                //SQ  = SH/2^0.5
                lSQ = SH() / (Math.Pow(2, 0.5));
                lP = (1 + lSQ) / ((1 / Fo) + (lSQ / MidRange));
                return lP;
            }
        }
        /// <summary>
        /// The near range where the bullet crosses the sight plane on the way to the far zero vertical deviation.
        /// </summary>
        public double ZeroNearRange
        {
            get
            {
                double lNZ;

                //Zn = (1 - SH) / (1 / Fo - SH / M)
                lNZ = (1 - SH()) / ((1 / Fo) - (SH() / MidRange));

                return lNZ;
            }
        }
        #endregion

        #region "Private Routines"
        /// <summary>
        /// Factor used to calculate Zero range, Near Zero Range, and Point-Blank-Range (PBR)
        /// </summary>
        /// <returns></returns>
        private double SH()
        {
            double lSH;

            if (_Hm == 0) return 0;
            //SH = (1 + S/Hm)^0.5
            lSH = Math.Pow((1 + (loadOutData.ScopeHeight / _Hm)), 0.5);
            return lSH;
        }
        #endregion
    }
}
