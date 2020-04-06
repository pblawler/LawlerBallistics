using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes.BallisticClasses
{
    public class DragSlopeData : INotifyPropertyChanged
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
        private double _MuzzleVelocity;
        private Atmospherics _Atmospherics = new Atmospherics();
        private double _Fo;
        private double _F2;
        private double _F3;
        private double _F4;
        private double _V1;
        private double _V2;
        private double _D1;
        private double _D2;
        private double _BCg1;
        private double _BCz2;
        private double _Zone1MachFactor;
        private double _Zone2MachFactor;
        private double _Zone3MachFactor;
        private double _Zone1TransSpeed;
        private double _Zone2TransSpeed;
        private double _Zone3TransSpeed;
        private double _Zone1SlopeMultiplier;
        private double _Zone3SlopeMultiplier;
        private double _Zone1Slope;
        private double _Zone1AngleFactor;
        private double _Zone3Slope;
        private double _Zone1Range;
        private double _Zone2Range;
        private double _Zone3Range;
        #endregion

        #region "Properties"

        #region "Drag Coefficents"
        /// <summary>
        /// Fo = the initial drag/retard factor starting at the muzzle.  Calculated from V1, V2, and the
        /// distance between where the two velocities were measured.  If V1 is not at the muzzle then
        /// the muzzle velocity will be calculated.  Alternatively can be calculated from the bullet BC
        /// and the measured muzzle velocity.
        /// </summary>
        public double Fo
        {
            get
            {
                return _Fo;
            }
            set
            {
                _Fo = value;
                RaisePropertyChanged(nameof(Fo));
                RaisePropertyChanged(nameof(Zone1Range));
                RaisePropertyChanged(nameof(Zone2Range));
                RaisePropertyChanged(nameof(Zone3Range));
            }
        }
        /// <summary>
        /// F2 = the initial drag/retard factor starting at the begining of Zone 2.
        /// </summary>
        public double F2
        {
            get
            {
                //F = BC(cV^0.5)
                if (_F2 == 0) _F2 = BCz2 * (166 * Math.Pow(Zone2TransSpeed, 0.5));
                return _F2;
            }
            set
            {
                _F2 = value;
                RaisePropertyChanged(nameof(F2));
                RaisePropertyChanged(nameof(Zone1Range));
                RaisePropertyChanged(nameof(Zone2Range));
                RaisePropertyChanged(nameof(Zone3Range));
            }
        }
        /// <summary>
        /// F3 = the initial drag/retard factor starting at the begining if zone 3.
        /// </summary>
        public double F3
        {
            get
            {
                //F = (cV^0.5)
                if (_F3 == 0) _F3 = (166 * Math.Pow(Zone3TransSpeed, 0.5));
                return _F3;
            }
            set
            {
                _F3 = value;
                RaisePropertyChanged(nameof(F3));
                RaisePropertyChanged(nameof(Zone1Range));
                RaisePropertyChanged(nameof(Zone2Range));
                RaisePropertyChanged(nameof(Zone3Range));
            }
        }
        /// <summary>
        /// F4 = the initial drag/retard factor starting at the end of zone 3.
        /// </summary>
        public double F4
        {
            get
            {
                //F = (cV^0.5)
                if (_F4 == 0) _F4 = (166 * Math.Pow(Zone3TransSpeed, 0.5));
                return _F4;
            }
            set
            {
                _F4 = value;
                RaisePropertyChanged(nameof(F4));
                RaisePropertyChanged(nameof(Zone1Range));
                RaisePropertyChanged(nameof(Zone2Range));
                RaisePropertyChanged(nameof(Zone3Range));
            }
        }
        #endregion

        #region "Velocity Distance Data for Drag Coefficent Calculation"
        /// <summary>
        /// The first velocity measured.  This value should be taken as close to the muzzle as possible.
        /// </summary>
        public double V1 { get { return _V1; } set { _V1 = value; RaisePropertyChanged(nameof(V1)); } }
        /// <summary>
        /// The second velocity measured.  Preferably 300 feet or more from V1 measurement location.
        /// </summary>
        public double V2 { get { return _V2; } set { _V2 = value; RaisePropertyChanged(nameof(V2)); } }
        /// <summary>
        /// Distance in feet from muzzle where V1 is measured.
        /// </summary>
        public double D1 { get { return _D1; } set { _D1 = value; RaisePropertyChanged(nameof(D1)); } }
        /// <summary>
        /// Distance in feet from the muzzle to where the second velocity reading (V2) was recorded.
        /// It is reccommeded to have atleast 300 ft between D1 and D2, 600 results in 1/2 the error
        /// as 300, 800 ft is ideal.
        /// </summary>
        public double D2 { get { return _D2; } set { _D2 = value; RaisePropertyChanged(nameof(D2)); } }
        #endregion

        #region "Ballistic Coefficents"
        /// <summary>
        /// Ballistic Coefficient G1
        /// </summary>
        public double BCg1 { get { return _BCg1; } set { _BCg1 = value; RaisePropertyChanged(nameof(BCg1)); } }
        /// <summary>
        /// Zone 2 ballistic coefficent
        /// </summary>
        public double BCz2
        {
            get
            {
                if (_BCz2 == 0) _BCz2 = BCg1;
                return _BCz2;
            }
            set { _BCz2 = value; RaisePropertyChanged(nameof(BCz2)); }
        }
        #endregion

        #region "Slope Data"
        /// <summary>
        /// A slope adjustment term based on the bullet design.  Default is 0.78.
        /// </summary>
        public double Zone1SlopeMultiplier
        {
            //TODO: in the interface recommend a multiplier based on the selected bullet type.
            //TODO  If this is zero used input the recommended value based on bullet type.
            //TODO if the bullet type selection changes and the value is a default of another
            // type then change it to the recommended value for the currently selected type.

            get
            {
                if (_Zone1SlopeMultiplier == 0) _Zone1SlopeMultiplier = 0.75;
                return _Zone1SlopeMultiplier;
            }
            set
            {
                _Zone1SlopeMultiplier = value;
                RaisePropertyChanged(nameof(Zone1SlopeMultiplier));
            }
        }
        /// <summary>
        /// A slope adjustment term based on the bullet design.  Default is 0.78.
        /// </summary>
        public double Zone3SlopeMultiplier
        {
            get
            {
                if (_Zone3SlopeMultiplier == 0) _Zone3SlopeMultiplier = 0.75;
                return _Zone3SlopeMultiplier;
            }
            set
            {
                _Zone3SlopeMultiplier = value;
                RaisePropertyChanged(nameof(Zone3SlopeMultiplier));
                RaisePropertyChanged(nameof(Zone1Range));
                RaisePropertyChanged(nameof(Zone2Range));
                RaisePropertyChanged(nameof(Zone3Range));
            }
        }
        /// <summary>
        /// Slope value for Zone 1 (supersonic) also represented as N for zone 1.
        /// </summary>
        public double Zone1Slope
        {
            get
            {
                if (_Zone1Slope == 0) _Zone1Slope = 0.5;
                return _Zone1Slope;
            }
            set
            {
                _Zone1Slope = value;
                RaisePropertyChanged(nameof(Zone1Slope));
                RaisePropertyChanged(nameof(Zone1Range));
                RaisePropertyChanged(nameof(Zone2Range));
                RaisePropertyChanged(nameof(Zone3Range));
            }
        }
        //TODO: The Zone1AngleFactor is a correction factor for the Pejsa D1' value.  Need to find the correct algorithm for D1'.
        /// <summary>
        /// A correction factor for drop resulting from the downward angle of the bullet as a result of zone 1 arc.
        /// </summary>
        public double Zone1AngleFactor
        {
            get
            {
                if (_Zone1AngleFactor == 0) _Zone1AngleFactor = 0.6957;
                return _Zone1AngleFactor;
            }
            set { _Zone1AngleFactor = value; RaisePropertyChanged(nameof(Zone1AngleFactor)); }
        }
        /// <summary>
        /// Slope value for Zone 3 (Transonic) also represented as N for zone 3.
        /// </summary>
        public double Zone3Slope
        {
            get
            {
                if (_Zone3Slope == 0) _Zone3Slope = -4;
                return _Zone3Slope;
            }
            set
            {
                _Zone3Slope = value;
                RaisePropertyChanged(nameof(Zone3Slope));
                RaisePropertyChanged(nameof(Zone1Range));
                RaisePropertyChanged(nameof(Zone2Range));
                RaisePropertyChanged(nameof(Zone3Range));

            }
        }
        #endregion

        #region "Sonic Zone Factors"
        /// <summary>
        /// The factor of the speed of sound to transition drag zones.
        /// </summary>
        public double Zone1MachFactor
        {
            get
            {
                if (_Zone1MachFactor == 0) _Zone1MachFactor = 1.2;
                return _Zone1MachFactor;
            }
            set
            {
                _Zone1MachFactor = value;
                RaisePropertyChanged(nameof(Zone1MachFactor));
            }
        }
        /// <summary>
        /// The factor of the speed of sound to transition drag zones.
        /// </summary>
        public double Zone2MachFactor
        {
            get
            {
                if (_Zone2MachFactor == 0) _Zone2MachFactor = 1;
                return _Zone2MachFactor;
            }
            set
            {
                _Zone2MachFactor = value;
                RaisePropertyChanged(nameof(Zone2MachFactor));
            }
        }
        /// <summary>
        /// The factor of the speed of sound to transition drag zones.
        /// </summary>
        public double Zone3MachFactor
        {
            get
            {
                if (_Zone3MachFactor == 0) _Zone3MachFactor = 0.8;
                return _Zone3MachFactor;
            }
            set
            {
                _Zone3MachFactor = value;
                RaisePropertyChanged(nameof(Zone3MachFactor));
            }
        }
        #endregion

        #region "Sonic Speed Data"
        /// <summary>
        /// The speed where Supersonic enters first transonic zone approx 1.2 Mach or 1340 fps.
        /// </summary>
        public double Zone1TransSpeed
        {
            get
            {
                double lZ1;
                double lDA;

                lZ1 = _Atmospherics.SpeedOfSound;
                lZ1 = lZ1 * Zone1MachFactor;
                lDA = _Atmospherics.DensityAlt;
                _Zone1TransSpeed = lZ1;
                if (_Zone1TransSpeed == 0) _Zone1TransSpeed = 1340;
                return _Zone1TransSpeed;
            }
        }
        /// <summary>
        /// The speed where the first transonic zone enters the second transonic zone, approx 1.0 Mach or 1125 fps.
        /// </summary>
        public double Zone2TransSpeed
        {
            get
            {
                double lZ2;
                lZ2 = _Atmospherics.SpeedOfSound;
                lZ2 = lZ2 * Zone2MachFactor;
                _Zone2TransSpeed = lZ2;
                if (_Zone2TransSpeed == 0) _Zone2TransSpeed = 1125;
                return _Zone2TransSpeed;
            }
        }
        /// <summary>
        /// The speed where the second transonic zone enters the subsonic zone, approx 890 fps.
        /// </summary>
        public double Zone3TransSpeed
        {
            get
            {
                double lZ3;
                lZ3 = _Atmospherics.SpeedOfSound;
                lZ3 = lZ3 * Zone3MachFactor;
                _Zone3TransSpeed = lZ3;
                if (_Zone3TransSpeed == 0) _Zone3TransSpeed = 890;
                return _Zone3TransSpeed;
            }
        }
        #endregion

        #region "Sonic Range Data"
        /// <summary>
        /// The range where the bullet passes our of Zone 1 into zone 2.
        /// </summary>
        public double Zone1Range
        {
            get
            {
                //R = (2/3)*Fo(1 - (V/Vo)^N)
                _Zone1Range = (2.00 / 3.00) * Fo * (1 - Math.Pow((Zone1TransSpeed / _MuzzleVelocity), Zone1Slope));
                if (_Zone1Range < 0) _Zone1Range = 0;
                return _Zone1Range;
            }
        }
        /// <summary>
        /// The range where the bullet passes our of Zone 2 into zone 3.
        /// </summary>
        public double Zone2Range
        {
            get
            {
                //R = F/(3ln(Vo/V))
                if (_MuzzleVelocity < Zone2TransSpeed)
                {
                    _Zone2Range = 0;
                }
                else
                {
                    _Zone2Range = (F2 / 3) * Math.Log(Zone1TransSpeed / Zone2TransSpeed);
                    _Zone2Range = _Zone2Range + Zone1Range;
                }
                return _Zone2Range;
            }
        }
        /// <summary>
        /// The range where the bullet passes our of Zone 3 into zone 4.
        /// </summary>
        public double Zone3Range
        {
            get
            {
                //R = (F2-F3)/3N
                if (_MuzzleVelocity < Zone3TransSpeed)
                {
                    _Zone3Range = 0;
                }
                else
                {
                    _Zone3Range = (F2 - F3) / (3 * Zone3Slope);
                    _Zone3Range = _Zone3Range + Zone2Range;
                }
                return _Zone3Range;
            }
        }
        #endregion

        #endregion

        #region "Constructor"
        public DragSlopeData(double MuzzleVelocity)
        {
            _MuzzleVelocity = MuzzleVelocity;
        }
        #endregion

        #region "Public Routines"
        public void SetAtmospherics(Atmospherics atmosherics)
        {
            _Atmospherics = atmosherics;
            RaisePropertyChanged(nameof(Zone1TransSpeed));
            RaisePropertyChanged(nameof(Zone2TransSpeed));
            RaisePropertyChanged(nameof(Zone3TransSpeed));
        }
        #endregion
    }
}
