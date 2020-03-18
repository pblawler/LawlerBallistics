using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class LocationData : INotifyPropertyChanged
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
        private double _Latitude;
        private double _Longitude;
        private double _Altitude;
        #endregion

        #region "Properties"
        public double Latitude { get { return _Latitude; } set { _Latitude = value; RaisePropertyChanged(nameof(Latitude)); } }
        public double Longitude { get { return _Longitude; } set { _Longitude = value; RaisePropertyChanged(nameof(Longitude)); } }
        public double Altitude { get { return _Altitude; } set { _Altitude = value; RaisePropertyChanged(nameof(Altitude)); } }
        #endregion

        #region "Constructor"
        public LocationData()
        {

        }
        #endregion
    }
}
