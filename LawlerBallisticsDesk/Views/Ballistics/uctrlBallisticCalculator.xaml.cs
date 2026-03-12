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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mapsui;
using Mapsui.Tiling;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Nts;
using Mapsui.UI.Wpf;
using BruTile;
using BruTile.Web;
using BruTile.Predefined;
using LawlerBallisticsDesk.Classes;
using LawlerBallisticsDesk.Classes.BallisticClasses;
using NtsPoint = NetTopologySuite.Geometries.Point;

namespace LawlerBallisticsDesk.Views.Ballistics
{
    /// <summary>
    /// Interaction logic for uctrlBallisticCalculator.xaml
    /// TODO:  Code clean up, add relay commands to replace direct calls, etc...
    /// </summary>
    public partial class uctrlBallisticCalculator : UserControl
    {
        private SolutionViewModel lDC;
        private const string MapLayerName = "BaseMapLayer";
        private const string PinLayerName = "PinLayer";

        // ArcGIS tile service URLs
        private const string EsriWorldTopoUrl = "https://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}";
        private const string EsriWorldImageryUrl = "https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}";

        // Pin feature references
        private GenericCollectionLayer<List<IFeature>> _pinLayer;
        private GeometryFeature _shooterPin;
        private Dictionary<string, GeometryFeature> _targetPins = new Dictionary<string, GeometryFeature>();

        private bool _ShooterActive = true;
        private bool _TargetActive = false;
        private string _ActiveTargetName = "";

        private bool _ShooterLocDefined
        {
            get
            {
                return !((lDC.MySolution.ShooterLoc.Latitude == 0) &&
                    (lDC.MySolution.ShooterLoc.Longitude == 0));
            }
        }

        public uctrlBallisticCalculator()
        {
            InitializeComponent();

            lDC = (SolutionViewModel)this.DataContext;
            lDC.LoadDefaultSolution();
            chkMaxRise_Click(null, null);

            // Initialize the Mapsui map
            InitializeMap();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.DataContext = null;
            GC.Collect();
        }

        private void chkMaxRise_Click(object sender, RoutedEventArgs e)
        {
            var lconverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush lbg = (System.Windows.Media.Brush)lconverter.ConvertFromString("#FFEAEAEA");
            System.Windows.Media.Brush lbg1 = (System.Windows.Media.Brush)lconverter.ConvertFromString("#FFFFFFFF");

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

        #region "Scenario Map"

        private void InitializeMap()
        {
            // Default to Topo view
            SetMapLayer(EsriWorldTopoUrl);

            // Initialize pin layer
            _pinLayer = new GenericCollectionLayer<List<IFeature>>
            {
                Name = PinLayerName
            };
            ScenarioMap.Map.Layers.Add(_pinLayer);

            // Wire up map click event
            ScenarioMap.Map.Info += Map_Info;

            // Hide the info widget overlay
            ScenarioMap.Map.Widgets.Clear();

            // Load existing pins if shooter location is defined
            LoadExistingPins();
        }

        private void SetMapLayer(string tileUrl)
        {
            // Remove existing base map layer if present
            var existingLayer = ScenarioMap.Map.Layers.FirstOrDefault(l => l.Name == MapLayerName);
            if (existingLayer != null)
            {
                ScenarioMap.Map.Layers.Remove(existingLayer);
            }

            // Create tile source from URL
            var tileSource = new HttpTileSource(
                new GlobalSphericalMercator(),
                tileUrl,
                name: MapLayerName);

            // Add new tile layer
            var newLayer = new TileLayer(tileSource)
            {
                Name = MapLayerName
            };
            ScenarioMap.Map.Layers.Insert(0, newLayer);
        }

        private void cboMapType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScenarioMap?.Map == null) return;

            var selectedItem = cboMapType.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            var mapType = selectedItem.Content.ToString();
            switch (mapType)
            {
                case "Satellite":
                    SetMapLayer(EsriWorldImageryUrl);
                    break;
                case "Topo":
                default:
                    SetMapLayer(EsriWorldTopoUrl);
                    break;
            }
        }

        private void Map_Info(object sender, MapInfoEventArgs e)
        {
            if (e.WorldPosition == null) return;

            // Convert from SphericalMercator to WGS84 (lat/lon)
            var worldPos = e.WorldPosition;
            var (lon, lat) = SphericalMercator.ToLonLat(worldPos.X, worldPos.Y);
            double longitude = lon;
            double latitude = lat;

            if (_ShooterActive)
            {
                // Set shooter location
                lDC.MySolution.ShooterLoc.Latitude = latitude;
                lDC.MySolution.ShooterLoc.Longitude = longitude;

                AddOrUpdateShooterPin(latitude, longitude);
                _ShooterActive = false;

                // Recalculate all target ballistics since shooter location changed
                RecalculateAllTargetBallistics();
            }
            else if (_TargetActive)
            {
                // Add new target
                var target = new Target
                {
                    Name = string.IsNullOrEmpty(_ActiveTargetName) ? $"Target {lDC.MySolution.MyScenario.Targets.Count + 1}" : _ActiveTargetName
                };
                target.TargetLocation.Latitude = latitude;
                target.TargetLocation.Longitude = longitude;

                lDC.MySolution.MyScenario.Targets.Add(target);

                AddTargetPin(target.ID, target.Name, latitude, longitude);

                // Calculate ballistic solution for the new target
                CalculateTargetBallistics(target);

                // Select the newly added target
                lDC.MySolution.MyScenario.SelectedTarget = target;

                _TargetActive = false;
                _ActiveTargetName = "";
            }
        }

        private void LoadExistingPins()
        {
            if (_ShooterLocDefined)
            {
                AddOrUpdateShooterPin(lDC.MySolution.ShooterLoc.Latitude, lDC.MySolution.ShooterLoc.Longitude);
            }

            // Load existing targets
            if (lDC.MySolution.MyScenario?.Targets != null)
            {
                foreach (var target in lDC.MySolution.MyScenario.Targets)
                {
                    if (target.TargetLocation.Latitude != 0 || target.TargetLocation.Longitude != 0)
                    {
                        AddTargetPin(target.ID, target.Name, target.TargetLocation.Latitude, target.TargetLocation.Longitude);
                    }
                }
            }
        }

        private void AddOrUpdateShooterPin(double latitude, double longitude)
        {
            // Convert lat/lon to SphericalMercator
            var (x, y) = SphericalMercator.FromLonLat(longitude, latitude);
            var geometry = new NtsPoint(x, y);

            if (_shooterPin != null)
            {
                // Update existing pin
                _pinLayer.Features.Remove(_shooterPin);
            }

            // Create shooter pin (blue)
            _shooterPin = new GeometryFeature
            {
                Geometry = geometry
            };
            _shooterPin.Styles.Add(new SymbolStyle
            {
                SymbolScale = 0.5,
                Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Blue),
                Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.White, 2),
                SymbolType = SymbolType.Ellipse
            });
            _shooterPin["Type"] = "Shooter";

            _pinLayer.Features.Add(_shooterPin);
            _pinLayer.DataHasChanged();
        }

        private void AddTargetPin(string targetId, string targetName, double latitude, double longitude)
        {
            // Convert lat/lon to SphericalMercator
            var (x, y) = SphericalMercator.FromLonLat(longitude, latitude);
            var geometry = new NtsPoint(x, y);

            // Create target pin (red)
            var targetPin = new GeometryFeature
            {
                Geometry = geometry
            };
            targetPin.Styles.Add(new SymbolStyle
            {
                SymbolScale = 0.5,
                Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
                Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.White, 2),
                SymbolType = SymbolType.Ellipse
            });
            targetPin["Type"] = "Target";
            targetPin["TargetId"] = targetId;
            targetPin["Name"] = targetName;

            _targetPins[targetId] = targetPin;
            _pinLayer.Features.Add(targetPin);
            _pinLayer.DataHasChanged();
        }

        private void RemoveTargetPin(string targetId)
        {
            if (_targetPins.TryGetValue(targetId, out var pin))
            {
                _pinLayer.Features.Remove(pin);
                _targetPins.Remove(targetId);
                _pinLayer.DataHasChanged();
            }
        }

        #endregion

        #region "Ballistic Calculations"

        /// <summary>
        /// Calculates ballistic solution for a single target
        /// </summary>
        private void CalculateTargetBallistics(Target target)
        {
            if (!_ShooterLocDefined) return;
            if (target.TargetLocation.Latitude == 0 && target.TargetLocation.Longitude == 0) return;

            // Calculate range from shooter to target
            double range = BallisticFunctions.CalculateRange(lDC.MySolution.ShooterLoc, target.TargetLocation);
            target.BallisticSolution.Range = range;

            // Get full trajectory data for this range
            target.BallisticSolution.MuzzleDrop = lDC.MySolution.MuzzleDrop(range);
            target.BallisticSolution.SightDelta = lDC.MySolution.SightDelta(range);
            target.BallisticSolution.Velocity = lDC.MySolution.Velocity(range);
            target.BallisticSolution.Energy = lDC.MySolution.Energy(range);
            target.BallisticSolution.SpinRate = lDC.MySolution.SpinRate(range);
            target.BallisticSolution.GyroStability = lDC.MySolution.GyroscopicStability(range);
            target.BallisticSolution.HorizDev = lDC.MySolution.TotalHorizontalDrift(range);
            target.BallisticSolution.HorzComp = lDC.MySolution.ZeroTotalHorizontalComp(range);
            target.BallisticSolution.HorizErr = lDC.MySolution.GetHorizErr(range);
            target.BallisticSolution.CoriolisH = lDC.MySolution.GetCoriolisHoriz(range);
            target.BallisticSolution.CoriolisV = lDC.MySolution.GetCoriolisVert(range);
            target.BallisticSolution.SpinDrift = lDC.MySolution.GetSpinDrift(range);
            target.BallisticSolution.WindDeflect = lDC.MySolution.WindDrift(range);
            target.BallisticSolution.FlightTime = lDC.MySolution.FlightTime(range);
            target.BallisticSolution.Fdragfactor = lDC.MySolution.Fdrag(range);
            target.BallisticSolution.CDdragCoefficient = lDC.MySolution.CDdragCoefficient(range);
        }

        /// <summary>
        /// Recalculates ballistic solutions for all targets
        /// </summary>
        private void RecalculateAllTargetBallistics()
        {
            if (lDC.MySolution.MyScenario?.Targets == null) return;

            foreach (var target in lDC.MySolution.MyScenario.Targets)
            {
                CalculateTargetBallistics(target);
            }

            // Refresh the view model to update UI
            lDC.ReloadAllTargetData();
        }

        #endregion

        private void lblSetShooterLoc_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ShooterActive = true;
            _TargetActive = false;
            MessageBox.Show("Click on the map to set the shooter location.",
                "Set Shooter Location", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void lblAddTarget_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ShooterActive = false;
            _TargetActive = true;
            _ActiveTargetName = "";
            MessageBox.Show("Click on the map to add a target.",
                "Add Target", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void lblDeleteTarget_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedTarget = lDC.MySolution.MyScenario.SelectedTarget;
            if (selectedTarget != null)
            {
                RemoveTargetPin(selectedTarget.ID);
                lDC.MySolution.MyScenario.Targets.Remove(selectedTarget);

                // Select another target if available
                if (lDC.MySolution.MyScenario.Targets.Count > 0)
                {
                    lDC.MySolution.MyScenario.SelectedTarget = lDC.MySolution.MyScenario.Targets[0];
                }
            }
            else
            {
                MessageBox.Show("Please select a target from the list to delete.",
                    "Delete Target", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
