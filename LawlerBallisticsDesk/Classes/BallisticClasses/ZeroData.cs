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
        private Atmospherics _atmospherics;
        private LocationData _TargetLoc;
        private LocationData _ShooterLoc;
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
        public Atmospherics atmospherics { get { return _atmospherics; } set { _atmospherics = value; RaisePropertyChanged(nameof(atmospherics)); } }
        public DragSlopeData dragSlopeData
        {
            get { return _dragSlopeData; }
            set { _dragSlopeData = value; RaisePropertyChanged(nameof(dragSlopeData)); }
        }
        public LoadOutData loadOutData { get { return _loadOutData; } set { _loadOutData = value; RaisePropertyChanged(nameof(loadOutData)); } }
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
