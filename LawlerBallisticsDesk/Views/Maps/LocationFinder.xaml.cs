using LawlerBallisticsDesk.ViewModel;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LawlerBallisticsDesk.Views.Maps
{
    /// <summary>
    /// Interaction logic for LocationFinder.xaml
    /// </summary>
    public partial class LocationFinder : Window
    {
        #region "Private Variables"
        private double _Latitude;
        private double _Longitude;
        private string _Mode;
        Pushpin ShooterLoc;
        Pushpin TargetLoc;
        #endregion

        #region "Properties"
        public double Latitude { get { return _Latitude; } set { _Latitude = value; } }
        public double Longitude { get { return _Longitude; } set { _Longitude = value; } }
        public string Mode { get { return _Mode; } set { _Mode = value; } }
        #endregion

        public LocationFinder()
        {
            InitializeComponent();
        }

        #region "Public Routines"
        public void NavigateTo()
        {
            Location lLoc = new Location();
            lLoc.Latitude = Latitude;
            lLoc.Longitude = Longitude;
            RangeMap.Center = lLoc;
            RangeMap.LocationToViewportPoint(lLoc);
            RangeMap.ZoomLevel = 16;
        }
        #endregion

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Determin the location to place the pushpin at on the map.

            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(this);
            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = RangeMap.ViewportPointToLocation(mousePosition);

            // The pushpin to add to the map.
            bool shooter = false;
            bool target = false;
            foreach(Pushpin lpp in RangeMap.Children)
            {
                if(lpp.Name == "Shooter")
                {
                    shooter = true;
                }
                if(lpp.Name == "Target")
                {
                    target = true;
                }
            }
            if (!shooter)
            {
                Pushpin ShooterLoc = new Pushpin();
                ShooterLoc.Location = pinLocation;
                ShooterLoc.Name = "Shooter";
                ShooterLoc.Content = "Shooter";
                // Adds the pushpin to the map.
                RangeMap.Children.Add(ShooterLoc);

            }
            else if (!target)
            {
                Pushpin TargetLoc = new Pushpin();
                TargetLoc.Location = pinLocation;
                TargetLoc.Name = "Target";
                TargetLoc.Content = "Target";
                // Adds the pushpin to the map.
                RangeMap.Children.Add(TargetLoc);
            }
            
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        { 
            SolutionViewModel mDC = (SolutionViewModel) this.DataContext;
            int lchk = 0;
            //TODO: get elevation data
            //https://docs.microsoft.com/en-us/bingmaps/rest-services/elevations/get-elevations
            switch (Mode)
            {
                case "Zero":
                    foreach (Pushpin lpp in RangeMap.Children)
                    {
                        if (lpp.Name == "Shooter")
                        {
                            mDC.SetShooterZeroLocation(lpp.Location.Altitude, lpp.Location.Latitude, lpp.Location.Longitude);
                            lchk += 1;
                        }
                        if (lpp.Name == "Target")
                        {
                            mDC.SetTargetZeroLocation(lpp.Location.Altitude, lpp.Location.Latitude, lpp.Location.Longitude);
                            lchk += 1;
                        }
                    }
                    break;
                case "Shot":
                    foreach (Pushpin lpp in RangeMap.Children)
                    {
                        if (lpp.Name == "Shooter")
                        {
                            mDC.SetShooterLocation(lpp.Location.Altitude, lpp.Location.Latitude, lpp.Location.Longitude);
                            lchk += 1;
                        }
                        if (lpp.Name == "Target")
                        {
                            mDC.SetTargetLocation(lpp.Location.Altitude, lpp.Location.Latitude, lpp.Location.Longitude);
                            lchk += 1;
                        }
                    }
                    break;
            
            }
            if (lchk > 0) this.Close();
        }

        private void Label_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
           foreach(Pushpin lpp in RangeMap.Children)
            {
                if (lpp.Name == "Shooter")
                {
                    RangeMap.Children.Remove(lpp);
                    ShooterLoc = null;
                    break;
                }
            }
        }

        private void Label_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            foreach (Pushpin lpp in RangeMap.Children)
            {
                if (lpp.Name == "Target")
                {
                    RangeMap.Children.Remove(lpp);
                    ShooterLoc = null;
                    break;
                }
            }
        }
    }
}
