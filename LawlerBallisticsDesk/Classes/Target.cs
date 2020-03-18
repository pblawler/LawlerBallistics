using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class Target : INotifyPropertyChanged
    {
        #region "Binding"
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void TargetLocation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TargetLocation));
        }       
        #endregion

        #region "Private Variables"
        private LocationData _TargetLocation;
        private double _WindSpeed;
        private double _WindDirection;
        #endregion

        #region "Properties"
        public LocationData TargetLocation { get { return _TargetLocation; } set { _TargetLocation = value; } }
        public double WindSpeed { get { return _WindSpeed; } set { _WindSpeed = value; RaisePropertyChanged(nameof(WindSpeed)); } }
        public double WindDirection { get { return _WindDirection; } set { _WindDirection = value; RaisePropertyChanged(nameof(WindDirection)); } }
        #endregion

        #region "Constructor"
        public Target()
        {
            TargetLocation.PropertyChanged += TargetLocation_PropertyChanged;
        }
        #endregion

        #region "Destructor"
        ~Target()
        {
            TargetLocation.PropertyChanged -= TargetLocation_PropertyChanged;
        }
        #endregion
    }
}
