using LawlerBallisticsDesk.ViewModel;
// Microsoft.Maps.MapControl.WPF is not compatible with .NET 8
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
        // Map-related types are not available in .NET 8
        // Pushpin ShooterLoc;
        // Pushpin TargetLoc;
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
            // Map functionality disabled - Microsoft.Maps.MapControl.WPF is not compatible with .NET 8
            MessageBox.Show("Map functionality is temporarily unavailable. The Bing Maps WPF control is not compatible with .NET 8.",
                "Feature Unavailable", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Map functionality disabled - Microsoft.Maps.MapControl.WPF is not compatible with .NET 8
            MessageBox.Show("Map functionality is temporarily unavailable. The Bing Maps WPF control is not compatible with .NET 8.",
                "Feature Unavailable", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Map functionality disabled - Microsoft.Maps.MapControl.WPF is not compatible with .NET 8
            MessageBox.Show("Map functionality is temporarily unavailable. The Bing Maps WPF control is not compatible with .NET 8.",
                "Feature Unavailable", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Label_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            // Map functionality disabled - Microsoft.Maps.MapControl.WPF is not compatible with .NET 8
            MessageBox.Show("Map functionality is temporarily unavailable. The Bing Maps WPF control is not compatible with .NET 8.",
                "Feature Unavailable", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Label_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            // Map functionality disabled - Microsoft.Maps.MapControl.WPF is not compatible with .NET 8
            MessageBox.Show("Map functionality is temporarily unavailable. The Bing Maps WPF control is not compatible with .NET 8.",
                "Feature Unavailable", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
