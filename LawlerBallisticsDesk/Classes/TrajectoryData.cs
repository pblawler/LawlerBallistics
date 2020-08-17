using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class TrajectoryData : INotifyPropertyChanged
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
        private double _MuzzleDrop;
        private double _Range;
        private double _SightDelta;
        private double _Velocity;
        private double _Energy;
        private double _SpinRate;
        private double _BSG;
        private double _HorizDev;
        private double _CoriolisH;
        private double _CoriolisV;
        private double _WindDeflect;
        private double _SpinDrift;
        private double _FlightTime;
        private double _HorzComp;
        private double _HorizErr;
        private double _Fdragfactor;
        private double _CDdragCoefficient;
        #endregion

        #region "Properties"
        public double CDdragCoefficient { get { return _CDdragCoefficient; } set { _CDdragCoefficient = value; RaisePropertyChanged(nameof(CDdragCoefficient)); } }
        public double Fdragfactor { get{ return _Fdragfactor; } set { _Fdragfactor = value; RaisePropertyChanged(nameof(Fdragfactor)); } }
        public double MuzzleDrop { get { return _MuzzleDrop; } set { _MuzzleDrop = value; RaisePropertyChanged(nameof(MuzzleDrop)); } }
        public double Range { get { return _Range; } set { _Range = value; RaisePropertyChanged(nameof(Range)); } }
        public double SightDelta
        {
            get{return _SightDelta; } 
            set
            { 
                _SightDelta = value;
                RaisePropertyChanged(nameof(SightDelta));
                RaisePropertyChanged(nameof(VertMOAcorrection));                
            }
        }
        public double Velocity { get { return _Velocity; } set { _Velocity = value; RaisePropertyChanged(nameof(Velocity)); } }
        public double Energy { get { return _Energy; } set { _Energy = value; RaisePropertyChanged(nameof(Energy)); } }
        public double SpinRate { get { return _SpinRate; } set { _SpinRate = value; RaisePropertyChanged(nameof(SpinRate)); } }
        public double GyroStability { get { return _BSG; } set { _BSG = value; RaisePropertyChanged(nameof(GyroStability)); } }
        public double HorizDev { get { return _HorizDev; } set { _HorizDev = value; RaisePropertyChanged(nameof(HorizDev));} }
        public double CoriolisV { get { return _CoriolisV; } set { _CoriolisV = value; RaisePropertyChanged(nameof(CoriolisV)); } }
        public double CoriolisH { get { return _CoriolisH; } set { _CoriolisH = value; RaisePropertyChanged(nameof(CoriolisH)); } }
        public double WindDeflect { get { return _WindDeflect; } set { _WindDeflect = value; RaisePropertyChanged(nameof(WindDeflect)); } }
        public double SpinDrift { get { return _SpinDrift; } set { _SpinDrift = value; RaisePropertyChanged(nameof(SpinDrift)); } }
        public double FlightTime { get { return _FlightTime; } set { _FlightTime = value; RaisePropertyChanged(nameof(FlightTime)); } }
        public double HorzComp { get { return _HorzComp; } set { _HorzComp = value; RaisePropertyChanged(nameof(HorzComp)); } }
        public double HorizErr { get { return _HorizErr; } set { _HorizErr = value; RaisePropertyChanged(nameof(HorizErr)); } }
        public double VertMOAcorrection
        {
            get
            {
                double rtn = 0;
                rtn = (-1 * _SightDelta) / ((_Range / 100) * 1.047);
                return rtn;
            }
        }
        public double HorizMOAcorrection
        {
            get
            {
                double rtn = 0;
                rtn = (-1 * HorizErr) / ((_Range / 100) * 1.047);
                return rtn;
            }
        }

        #endregion

        public TrajectoryData()
        {

        }
    }
}
