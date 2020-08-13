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
using Microsoft.Maps.MapControl.WPF;
using LawlerBallisticsDesk.Classes;

namespace LawlerBallisticsDesk.Views.Ballistics
{
    /// <summary>
    /// Interaction logic for frmBallisticCalculator.xaml
    /// </summary>
    public partial class frmBallisticCalculator : Window
    {
        private SolutionViewModel lDC;

        public frmBallisticCalculator()
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
        private Pushpin _TargetLoc;
        private Pushpin _ShooterLoc;
        private bool _AddTarget;
        private bool _ShooterActive = true;
        private bool _TargetActive = false;
        private string _ActiveTargetName = "";

        private bool _ShooterLocDefined { get { return !((lDC.MySolution.ShooterLoc.Latitude == 0) & 
                    (lDC.MySolution.ShooterLoc.Longitude == 0)); } }        

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Determin the location to place the pushpin at on the map.

            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(ScenarioMap);

            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = ScenarioMap.ViewportPointToLocation(mousePosition);
            Target TargetLocDat = new Target();

            if (_ShooterActive)
            {
                //Add the shooter location if it does not exist.
                foreach (Pushpin lpp in ScenarioMap.Children)
                {
                    if (lpp.Name == "Shooter")
                    {
                        ScenarioMap.Children.Remove(lpp);
                        break;
                    }
                }
                _ShooterLoc = new Pushpin();
                _ShooterLoc.Location = pinLocation;
                _ShooterLoc.Name = "Shooter";
                _ShooterLoc.Content = _ShooterLoc.Name;
                lDC.MySolution.ShooterLoc.Latitude = pinLocation.Latitude;
                lDC.MySolution.ShooterLoc.Longitude = pinLocation.Longitude;
                lDC.MySolution.MyScenario.MyShooter.MyLocation.Latitude = pinLocation.Latitude;
                lDC.MySolution.MyScenario.MyShooter.MyLocation.Longitude = pinLocation.Longitude;
                // Adds the pushpin to the map.
                ScenarioMap.Children.Add(_ShooterLoc);
            }
            else if(_TargetActive)
            {
                if(_ActiveTargetName == "")
                {
                    _TargetLoc = new Pushpin();
                    _TargetLoc.Location = pinLocation;
                    _TargetLoc.Name = "Target_" + lDC.MySolution.MyScenario.Targets.Count.ToString();
                    _TargetLoc.Content = _TargetLoc.Name;
                    ScenarioMap.Children.Add(_TargetLoc);
                    TargetLocDat.Name = _TargetLoc.Name;
                    TargetLocDat.TargetLocation.Latitude = _TargetLoc.Location.Latitude;
                    TargetLocDat.TargetLocation.Longitude = _TargetLoc.Location.Longitude;
                    lDC.MySolution.MyScenario.Targets.Add(TargetLocDat);
                    lDC.MySolution.MyScenario.SelectedTarget = TargetLocDat;
                    _ActiveTargetName = _TargetLoc.Name;
                }
            }
        }

        #endregion

        private void lblSetShooterLoc_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ShooterActive = true;
        }

        private void lblAddTarget_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ShooterActive = false;
            _TargetActive = true;
            _ActiveTargetName = "";
        }
    }
}
