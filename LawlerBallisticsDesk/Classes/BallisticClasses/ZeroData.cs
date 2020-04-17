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
            RaisePropertyChanged(nameof(atmospherics));
            SetWindDirection();
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
            RaisePropertyChanged(nameof(ShooterLoc));
            SetWindDirection();
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
            RaisePropertyChanged(nameof(TargetLoc));
            SetWindDirection();
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
        //private LoadOut _loadOutData;
        private Atmospherics _atmospherics;
        private LocationData _TargetLoc;
        private LocationData _ShooterLoc;
        private double _Hm;
        private double _ZeroRange;
        private double _NearZeroRange;
        private double _MidRange;
        private bool _UseMaxRise;
        private double _PointBlankRange;
        private string _Message;
        private string[] _MsgQ = new string[12];
        private double _ShotDirection;
        private double _ShotAngle;
        private double _ShotDistance;
        private double _WindEffectiveDirection;
        //private DragSlopeData _dragSlopeData;
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
                SetWindDirection();
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
                SetWindDirection();
                ShooterLoc.PropertyChanged += ShooterLoc_PropertyChanged;
                RaisePropertyChanged(nameof(ShooterLoc));
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
                SetWindDirection();
                TargetLoc.PropertyChanged += TargetLoc_PropertyChanged;
                RaisePropertyChanged(nameof(TargetLoc));
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
                return _ShotDirection;
            }
            set
            {
                _ShotDirection = value; 
                RaisePropertyChanged(nameof(ShotDirection));
            } 
        }
        /// <summary>
        /// Degrees in elevation, i.e. shooting up or down.
        /// </summary>
        public double ShotAngle { get { return _ShotAngle; } set { _ShotAngle = value; RaisePropertyChanged(nameof(ShotAngle)); } }
        /// <summary>
        /// LOS range to the target calculated from map data.
        /// </summary>
        public double ShotDistance { get { return _ShotDistance; } set { _ShotDistance = value; RaisePropertyChanged(nameof(ShotDistance)); } }
        public double WindEffectiveDirection { get { return _WindEffectiveDirection; } set { _WindEffectiveDirection = value; RaisePropertyChanged(nameof(WindEffectiveDirection)); } }
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
        private void SetWindDirection()
        {
            //TODO: break this function up to calculate the components independently
            if (TargetLoc == null) return;
            //Target latitude minus shooter latitude to get positive for east.
            double lvert = (ShooterLoc.Latitude - TargetLoc.Latitude) * LocationData.YardsPerDegLatLon;
            //Target longitude minus shooter longitude to get positive for north.
            double lhoriz = (TargetLoc.Longitude - ShooterLoc.Longitude) * LocationData.YardsPerDegLatLon;
            double lShtAngl = Math.Atan(Math.Abs(lhoriz / lvert))*(180/Math.PI);
            double lhorzRange = lhoriz / Math.Sin((lShtAngl * (Math.PI / 180)));
            double lElev = (TargetLoc.Altitude - ShooterLoc.Altitude)/3;
            double lElevAng = Math.Atan(Math.Abs(lElev / lhorzRange)) * (180 / Math.PI);
            ShotDistance = lElev / Math.Sin((lElevAng * (Math.PI / 180)));
            ShotAngle = lElevAng;
            if((lhoriz < 0) & (lvert > 0))
            {
                lShtAngl = 360 - lShtAngl;
            }
            else if ((lhoriz > 0) & (lvert < 0))
            {
                lShtAngl = 180 - lShtAngl;
            }
            else if ((lhoriz < 0) & (lvert < 0))
            {
                lShtAngl = 180 + lShtAngl;
            }
            ShotDirection = lShtAngl;
            double lWE = atmospherics.WindDirection - ShotDirection;
            if(lWE<0) lWE = 360 - lWE;
            WindEffectiveDirection = lWE;
        }
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
        #endregion


    }
}
