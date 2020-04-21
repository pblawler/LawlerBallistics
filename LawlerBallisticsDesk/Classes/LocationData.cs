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

        #region "Constants"
        public static double YardsPerDegLatLon = 121740.6652;
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

        #region "Public Routines"
        public static double GetEffectiveWindDirection(LocationData TargetLoc, LocationData ShooterLoc, double WindDirection)
        {
            double lRTN = 0;
            double lShotDistance = 0;
            double lShotAngle = 0;
            double lShotDirection = 0;

            if (TargetLoc == null) return lRTN;
            //Target latitude minus shooter latitude to get positive for east.
            double lvert = (ShooterLoc.Latitude - TargetLoc.Latitude) * YardsPerDegLatLon;
            //Target longitude minus shooter longitude to get positive for north.
            double lhoriz = (TargetLoc.Longitude - ShooterLoc.Longitude) * YardsPerDegLatLon;
            double lShtAngl = Math.Atan(Math.Abs(lhoriz / lvert)) * (180 / Math.PI);
            double lhorzRange = lhoriz / Math.Sin((lShtAngl * (Math.PI / 180)));
            double lElev = (TargetLoc.Altitude - ShooterLoc.Altitude) / 3;
            double lElevAng = Math.Atan(Math.Abs(lElev / lhorzRange)) * (180 / Math.PI);
            if (lElev == 0)
            {
                lShotDistance = lhorzRange;
            }
            else
            {
                lShotDistance = lElev / Math.Sin((lElevAng * (Math.PI / 180)));
            }
            lShotAngle = lElevAng;
            if ((lhoriz < 0) & (lvert > 0))
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
            lShotDirection = lShtAngl;
            double lWE = WindDirection - lShotDirection;
            if (lWE < 0) lWE = 360 + lWE;
            lRTN = lWE;

            return lRTN;
        }
        public static double GetShotDistance(LocationData TargetLoc, LocationData ShooterLoc)
        {
            double lRTN = 0;

            if (TargetLoc == null) return lRTN;
            //Target latitude minus shooter latitude to get positive for east.
            double lvert = (ShooterLoc.Latitude - TargetLoc.Latitude) * YardsPerDegLatLon;
            //Target longitude minus shooter longitude to get positive for north.
            double lhoriz = (TargetLoc.Longitude - ShooterLoc.Longitude) * YardsPerDegLatLon;
            double lShtAngl = Math.Atan(Math.Abs(lhoriz / lvert)) * (180 / Math.PI);
            double lhorzRange = lhoriz / Math.Sin((lShtAngl * (Math.PI / 180)));
            double lElev = (TargetLoc.Altitude - ShooterLoc.Altitude) / 3;
            double lElevAng = Math.Atan(Math.Abs(lElev / lhorzRange)) * (180 / Math.PI);
            if (lElev == 0)
            {
                lRTN = lhorzRange;
            }
            else
            {
                lRTN = lElev / Math.Sin((lElevAng * (Math.PI / 180)));
            }
         
            return lRTN;
        }
        public static double GetShotHorizontalDistance(LocationData TargetLoc, LocationData ShooterLoc)
        {
            double lRTN = 0;

            if (TargetLoc == null) return lRTN;
            //Target latitude minus shooter latitude to get positive for east.
            double lvert = (ShooterLoc.Latitude - TargetLoc.Latitude) * LocationData.YardsPerDegLatLon;
            //Target longitude minus shooter longitude to get positive for north.
            double lhoriz = (TargetLoc.Longitude - ShooterLoc.Longitude) * LocationData.YardsPerDegLatLon;
            double lShtAngl = Math.Atan(Math.Abs(lhoriz / lvert)) * (180 / Math.PI);
            lRTN = lhoriz / Math.Sin((lShtAngl * (Math.PI / 180)));

            return lRTN;
        }
        public static double GetShotAngle(LocationData TargetLoc, LocationData ShooterLoc)
        {
            double lRTN = 0;

            if (TargetLoc == null) return lRTN;
            //Target latitude minus shooter latitude to get positive for east.
            double lvert = (ShooterLoc.Latitude - TargetLoc.Latitude) * YardsPerDegLatLon;
            //Target longitude minus shooter longitude to get positive for north.
            double lhoriz = (TargetLoc.Longitude - ShooterLoc.Longitude) * YardsPerDegLatLon;
            double lShtAngl = Math.Atan(Math.Abs(lhoriz / lvert)) * (180 / Math.PI);
            double lhorzRange = lhoriz / Math.Sin((lShtAngl * (Math.PI / 180)));
            double lElev = (TargetLoc.Altitude - ShooterLoc.Altitude) / 3;
            lRTN = Math.Atan(Math.Abs(lElev / lhorzRange)) * (180 / Math.PI);

            return lRTN;
        }
        public static double GetShotDirection(LocationData TargetLoc, LocationData ShooterLoc)
        {
            double lRTN = 0;

            if (TargetLoc == null) return lRTN;
            //Target latitude minus shooter latitude to get positive for east.
            double lvert = (ShooterLoc.Latitude - TargetLoc.Latitude) * YardsPerDegLatLon;
            //Target longitude minus shooter longitude to get positive for north.
            double lhoriz = (TargetLoc.Longitude - ShooterLoc.Longitude) * YardsPerDegLatLon;
            double lShtAngl = Math.Atan(Math.Abs(lhoriz / lvert)) * (180 / Math.PI);
            double lhorzRange = lhoriz / Math.Sin((lShtAngl * (Math.PI / 180)));
            double lElev = (TargetLoc.Altitude - ShooterLoc.Altitude) / 3;
            double lElevAng = Math.Atan(Math.Abs(lElev / lhorzRange)) * (180 / Math.PI);
            if ((lhoriz < 0) & (lvert > 0))
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
            lRTN = lShtAngl;

            return lRTN;
        }
        #endregion

        #region "Constructor"
        public LocationData()
        {

        }
        #endregion
    }
}
