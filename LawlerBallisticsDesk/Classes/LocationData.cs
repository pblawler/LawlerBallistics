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
        /// <summary>
        /// The latitude in decimal degrees (DD).
        /// </summary>
        public double Latitude
        {
            get
            {
                if (_Latitude == 0) _Latitude = 34.681129;
                return _Latitude;
            }
            set
            {
                _Latitude = value; RaisePropertyChanged(nameof(Latitude));
            }
        }
        /// <summary>
        /// The longitude in decimal degrees (DD).
        /// </summary>
        public double Longitude { get { return _Longitude; } set { _Longitude = value; RaisePropertyChanged(nameof(Longitude)); } }
        /// <summary>
        /// The altitude in feet.
        /// </summary>
        public double Altitude { get { return _Altitude; } set { _Altitude = value; RaisePropertyChanged(nameof(Altitude)); } }
        #endregion

        #region "Constructor"
        public LocationData()
        {

        }
        #endregion
    }
}
