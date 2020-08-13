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
        private void atmospherics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            RaiseDependentProperties();
            switch (e.PropertyName)
            {
                case "Message":
                    break;

                default:
                    break;
            }
        }
        private void ShooterLoc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //RaisePropertyChanged(nameof(_ShooterLoc));
            RaisePropertyChanged(e.PropertyName);
            RaiseDependentProperties();
            switch (e.PropertyName)
            {
                case "Message":
                    break;

                default:
                    break;
            }
        }
        private void TargetLoc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //RaisePropertyChanged(nameof(_TargetLoc));
            RaisePropertyChanged(e.PropertyName);
            RaiseDependentProperties();
            switch (e.PropertyName)
            {
                case "Message":
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region "Private Variables"
        private string _Message;
        private Atmospherics _atmospherics;
        private LocationData _TargetLoc;
        private LocationData _ShooterLoc;
        private double _Hm;
        private double _ZeroRange;
        private double _NearZeroRange;
        private double _MidRange;
        private bool _UseMaxRise;
        private double _PointBlankRange;
        private string[] _MsgQ = new string[12];
        private double _MuzzleVelocity;
        private double _ShotDirection;
        private double _ShotAngle;
        private double _ShotDistance;
        private double _WindEffectiveDirection;
        #endregion

        #region "Properties"
        public Atmospherics atmospherics
        {
            get 
            {
                return _atmospherics;
            } 
            set
            {
                _atmospherics = value;
                RaiseDependentProperties();
                atmospherics.PropertyChanged += atmospherics_PropertyChanged;
                RaisePropertyChanged(nameof(atmospherics));
            }
        }
        //public DragSlopeData dragSlopeData
        //{
        //    get { return _dragSlopeData; }
        //    set { _dragSlopeData = value; RaisePropertyChanged(nameof(dragSlopeData)); }
        //}
        //public LoadOut loadOutData { get { return _loadOutData; } set { _loadOutData = value; RaisePropertyChanged(nameof(loadOutData)); } }
        public LocationData ShooterLoc
        {
            get 
            {
                return _ShooterLoc;
            }
            set
            {
                _ShooterLoc = value;
                ShooterLoc.PropertyChanged += ShooterLoc_PropertyChanged;
                RaisePropertyChanged(nameof(ShooterLoc));
                RaiseDependentProperties();
            }
        }
        public LocationData TargetLoc
        {
            get
            {
                return _TargetLoc;
            }
            set
            {
                _TargetLoc = value;                
                TargetLoc.PropertyChanged += TargetLoc_PropertyChanged;
                RaisePropertyChanged(nameof(TargetLoc));
                RaiseDependentProperties();
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
        public double NearZeroRange { get { return _NearZeroRange; } set { _NearZeroRange = value; RaisePropertyChanged(nameof(NearZeroRange)); } }
        public double MidRange { get { return _MidRange; } set { _MidRange = value; RaisePropertyChanged(nameof(_MidRange)); } } 
        /// <summary>
        /// True if the zero range is to be calculated from the maximum desired rise above the sight plane
        /// on the trajectory to zero.
        /// </summary>
        public bool UseMaxRise { get { return _UseMaxRise; } set { _UseMaxRise = value; RaisePropertyChanged(nameof(UseMaxRise)); } }
        public double PointBlankRange { get { return _PointBlankRange; } set { _PointBlankRange = value; RaisePropertyChanged(nameof(PointBlankRange)); } }
        /// <summary>
        /// Degrees from true north.
        /// </summary>
        public double ShotDirection 
        {
            get
            {
                if(_ShotDirection == 0) _ShotDirection = LocationData.GetShotDirection(TargetLoc, ShooterLoc);

                return _ShotDirection;
            }
            set { _ShotDirection = value; RaisePropertyChanged(nameof(ShotDirection)); }
        }
        /// <summary>
        /// Degrees in elevation, i.e. shooting up or down.
        /// </summary>
        public double ShotAngle
        {
            get
            {
                if (_ShotAngle == 0) _ShotAngle = LocationData.GetShotAngle(TargetLoc, ShooterLoc);
                return _ShotAngle;
            }
            set
            {
                _ShotAngle = value;
                RaisePropertyChanged(nameof(ShotAngle));
            }
        }
        /// <summary>
        /// LOS range to the target calculated from map data.
        /// </summary>
        public double ShotDistance
        {
            get
            {
                if(_ShotDistance == 0) _ShotDistance = LocationData.GetShotDistance(TargetLoc, ShooterLoc);
                return _ShotDistance;
            }
            set
            {
                _ShotDistance = value;
                RaisePropertyChanged(nameof(ShotDistance));
            }
        }
        public double ShotHorizDistance { get { return LocationData.GetShotHorizontalDistance(TargetLoc, ShooterLoc); } }
        public double WindEffectiveDirection
        {
            get
            {
                if(_WindEffectiveDirection == 0) _WindEffectiveDirection = LocationData.GetEffectiveWindDirection(TargetLoc, ShooterLoc, atmospherics.WindDirection);
                return _WindEffectiveDirection;
            }
            set
            {
                _WindEffectiveDirection = value;
                RaisePropertyChanged(nameof(WindEffectiveDirection));
            }
        }
        public double MuzzleVelocity { get { return _MuzzleVelocity; } set { _MuzzleVelocity = value; RaisePropertyChanged(nameof(MuzzleVelocity)); } }
        public string Message { get { return _Message; } }
        #endregion

        #region "Constructor"
        public ZeroData()
        {
            _Message = "";
            atmospherics = new Atmospherics();
            atmospherics.PropertyChanged += atmospherics_PropertyChanged;
            ShooterLoc = new LocationData();
            ShooterLoc.PropertyChanged += ShooterLoc_PropertyChanged;
            TargetLoc = new LocationData();
            TargetLoc.PropertyChanged += TargetLoc_PropertyChanged;
        }
        #endregion

        #region "Destructor"
        ~ZeroData()
        {
            atmospherics.PropertyChanged -= atmospherics_PropertyChanged;
            ShooterLoc.PropertyChanged -= ShooterLoc_PropertyChanged;
            TargetLoc.PropertyChanged -= TargetLoc_PropertyChanged;

        }
        #endregion

        #region "Public Routines"
        public void LoadCurrentLocationWeather()
        {
            atmospherics.GetWeather(ShooterLoc.Latitude, ShooterLoc.Longitude);
        }
        #endregion

        #region "Private Routines"
        private void LoadMessage(string msg)
        {
            string lmsg = "";

            for (int I = 11; I > 0; I--)
            {
                _MsgQ[I] = _MsgQ[I - 1];
            }
            _MsgQ[0] = msg;
            for (int I = 0; I < 12; I++)
            {
                lmsg = lmsg + _MsgQ[I] + System.Environment.NewLine;
            }
            _Message = lmsg;
            RaisePropertyChanged(nameof(Message));

        }
        private void RaiseDependentProperties()
        {
            RaisePropertyChanged(nameof(WindEffectiveDirection));
            RaisePropertyChanged(nameof(ShotAngle));
            RaisePropertyChanged(nameof(ShotDirection));
            RaisePropertyChanged(nameof(ShotDistance));
            RaisePropertyChanged(nameof(ShotHorizDistance));
        }
        #endregion


    }
}
