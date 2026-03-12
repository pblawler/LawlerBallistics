using LawlerBallisticsDesk.ViewModel;
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
// Microsoft.Maps.MapControl.WPF is not compatible with .NET 8
using LawlerBallisticsDesk.Classes;

namespace LawlerBallisticsDesk.Views.Ballistics
{
    /// <summary>
    /// Interaction logic for uctrlBallisticCalculator.xaml
    /// TODO:  Code clean up, add relay commands to replace direct calls, etc...
    /// </summary>
    public partial class uctrlBallisticCalculator : UserControl
    {
        private SolutionViewModel lDC;

        public uctrlBallisticCalculator()
        {
            InitializeComponent();

            lDC = (SolutionViewModel) this.DataContext;
            lDC.LoadDefaultSolution();
            chkMaxRise_Click(null, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.DataContext = null;
            GC.Collect();
        }

        private void chkMaxRise_Click(object sender, RoutedEventArgs e)
        {
            var lconverter = new System.Windows.Media.BrushConverter();
            Brush lbg = (Brush)lconverter.ConvertFromString("#FFEAEAEA");
            Brush lbg1 = (Brush)lconverter.ConvertFromString("#FFFFFFFF");

            if ((bool)chkMaxRise.IsChecked)
            {
                txtZeroR.Background = lbg;
                txtZeroR.IsReadOnly = true;
                txtMaxRise.Background = lbg1;
                txtMaxRise.IsReadOnly = false;
            }
            else
            {
                txtZeroR.Background = lbg1;
                txtZeroR.IsReadOnly = false;
                txtMaxRise.Background = lbg;
                txtMaxRise.IsReadOnly = true;
            }
        }

        #region "Scenario"
        // Map-related types are not available in .NET 8
        // private Pushpin _TargetLoc;
        // private Pushpin _ShooterLoc;
        private bool _AddTarget;
        private bool _ShooterActive = true;
        private bool _TargetActive = false;
        private string _ActiveTargetName = "";

        private bool _ShooterLocDefined { get { return !((lDC.MySolution.ShooterLoc.Latitude == 0) &
                    (lDC.MySolution.ShooterLoc.Longitude == 0)); } }

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Map functionality disabled - Microsoft.Maps.MapControl.WPF is not compatible with .NET 8
            // TODO: Consider using an alternative mapping solution like WebView2 with Bing Maps
            MessageBox.Show("Map functionality is temporarily unavailable. The Bing Maps WPF control is not compatible with .NET 8.", 
                "Feature Unavailable", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        private void lblSetShooterLoc_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ShooterActive = true;
            MessageBox.Show("Map functionality is temporarily unavailable. The Bing Maps WPF control is not compatible with .NET 8.",
                "Feature Unavailable", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void lblAddTarget_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ShooterActive = false;
            _TargetActive = true;
            _ActiveTargetName = "";
            MessageBox.Show("Map functionality is temporarily unavailable. The Bing Maps WPF control is not compatible with .NET 8.",
                "Feature Unavailable", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void lblDeleteTarget_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Map functionality disabled - Microsoft.Maps.MapControl.WPF is not compatible with .NET 8
            MessageBox.Show("Map functionality is temporarily unavailable. The Bing Maps WPF control is not compatible with .NET 8.",
                "Feature Unavailable", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
