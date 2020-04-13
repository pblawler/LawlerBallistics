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
        //private DragSlopeData _dragSlopeData;
        #endregion

        #region "Properties"
        public Atmospherics atmospherics { get { return _atmospherics; } set { _atmospherics = value; RaisePropertyChanged(nameof(atmospherics)); } }
        //public DragSlopeData dragSlopeData
        //{
        //    get { return _dragSlopeData; }
        //    set { _dragSlopeData = value; RaisePropertyChanged(nameof(dragSlopeData)); }
        //}
        //public LoadOut loadOutData { get { return _loadOutData; } set { _loadOutData = value; RaisePropertyChanged(nameof(loadOutData)); } }
        public LocationData ShooterLoc { get { return _ShooterLoc; } set { _ShooterLoc = value; RaisePropertyChanged(nameof(ShooterLoc)); } }
        public LocationData TargetLoc { get { return _TargetLoc; } set { _TargetLoc = value; RaisePropertyChanged(nameof(TargetLoc)); } }
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
        #endregion

        #region "Constructor"
        public ZeroData()
        {
            atmospherics = new Atmospherics();
            ShooterLoc = new LocationData();
            TargetLoc = new LocationData();
        }
        #endregion

        #region "Public Routines"
        public void LoadCurrentLocationWeather()
        {
            atmospherics.GetWeather(ShooterLoc.Latitude, ShooterLoc.Longitude);
        }
        #endregion

    }
}
