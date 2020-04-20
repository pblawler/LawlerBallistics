using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using LawlerBallisticsDesk.Classes;
using LawlerBallisticsDesk.Views.Ballistics;
using LawlerBallisticsDesk.Views.Maps;
using LawlerBallisticsDesk.Classes.BallisticClasses;
using System.ComponentModel;

namespace LawlerBallisticsDesk.ViewModel
{
    public class SolutionViewModel : ViewModelBase, IDisposable
    {
        //TODO: Add warnings before changing drag slope data when not 0
        //TODO: reset all solution data when flight parameters change.
        //TODO: Subscribe to zerodata property changes and calculate values when properties change.

        //TODO: Post load sanity check (i.e. calculate zero data etc....).

        #region "Binding"
        private void MySolution_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            
            switch (e.PropertyName)
            {
                case "UseMaxRise":
                    //SolveZeroData(); causes endless loop
                    break;
                case "ZeroMaxRise":
                    if (UseMaxRise) SolveZeroData();
                    break;
                case "ZeroRange":
                    if (!UseMaxRise) SolveZeroData();
                    break;
                case "ScopeHeight":
                    SolveZeroData();
                    break;
                case "MuzzleVelocity":
                    SolveZeroData();
                    break;
                case "Fo":
                    MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange = BallisticFunctions.MaxRange(Fo);
                    break;
                case "SelectedGun":
                    RaisePropertyChanged(nameof(MyBarrels));
                    break;
                case "SelectedBarrel":
                    RaisePropertyChanged(nameof(MyCartridges));
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region "Private Variables"
        private int _ZeroMsgVal = 0;
        private string[] _ZeroMsg = new string[12];
        private string[] _DragMsg = new string[12];
        private string _ZeroMessage;
        private string _DragMessage;
        private Solution _MySolution;
        private List<string> _BulletTypes;
        private double _TestBulletWeight;
        private double _TestBulletDiameter;
        private double _TestBulletLength;
        private double _TestBulletBC;
        private string _TestBulletType;
        private frmBCcalculator _frmBCcalc;
        private string _ShotMsg;
        private string _FileName;
        private PlotModel _TrajectoryPlot;
        private PlotModel _WindagePlot;
        private ObservableCollection<TrajectoryData> _MyTrajectories;
        private bool _PlotVelocity = true;
        private bool _PlotEnergy = true;
        private bool _PlotSpinRate = true;
        private bool _PlotHcoriolis = true;
        private bool _PlotSpinDrift = true;
        private bool _PlotWindDrift = true;
        private bool _PlotFlightTime = true;
        #endregion

        #region "Properties"
        public Solution MySolution
        {
            get
            {
                return _MySolution;
            }
            set
            {
                MySolution.PropertyChanged -= MySolution_PropertyChanged;
                _MySolution = value;
                MySolution.PropertyChanged += MySolution_PropertyChanged;
                RaisePropertyChanged(nameof(MySolution));
            }
        }
        public PlotModel TrajectoryPlot { get { return _TrajectoryPlot; } }
        public PlotModel WindagePlot { get { return _WindagePlot; } }
        public bool PlotWindDrift
        {
            get
            {
                return _PlotWindDrift;
            }
            set
            {
                _PlotWindDrift = value;
                RaisePropertyChanged(nameof(PlotWindDrift));
                LoadCharts();
            }
        }
        public bool PlotSpinDrift
        {
            get
            {
                return _PlotSpinDrift;
            }
            set
            {
                _PlotSpinDrift = value;
                RaisePropertyChanged(nameof(PlotSpinDrift));
                LoadCharts();
            }
        }
        public bool PlotHcoriolis
        {
            get
            {
                return _PlotHcoriolis;
            }
            set
            {
                _PlotHcoriolis = value;
                RaisePropertyChanged(nameof(PlotHcoriolis));
                LoadCharts();
            }
        }
        public bool PlotVelocity
        {
            get
            {
                return _PlotVelocity;
            }
            set
            {
                _PlotVelocity = value;
                RaisePropertyChanged(nameof(PlotVelocity));
                LoadCharts();
            }
        }
        public bool PlotEnergy
        {
            get
            {
                return _PlotEnergy;
            }
            set
            {
                _PlotEnergy = value;
                RaisePropertyChanged(nameof(PlotEnergy));
                LoadCharts();
            }
        }
        public bool PlotSpinRate
        {
            get
            {
                return _PlotSpinRate;
            }
            set
            {
                _PlotSpinRate = value;
                RaisePropertyChanged(nameof(PlotSpinRate));
                LoadCharts();
            }
        }
        public bool PlotFlightTime
        {
            get
            {
                return _PlotFlightTime;
            }
            set
            {
                _PlotFlightTime = value;
                RaisePropertyChanged(nameof(PlotFlightTime));
                LoadCharts();
            }
        }
        public ObservableCollection<TrajectoryData> MyTrajectories { get { return _MyTrajectories; } set { _MyTrajectories = value; RaisePropertyChanged(nameof(MyTrajectories)); } }
        public string ShotMsg { get { return _ShotMsg; } set { _ShotMsg = value; RaisePropertyChanged(nameof(ShotMsg)); } }
        public List<string> BarrelDirList { get { return LawlerBallisticsFactory.BarrelRiflingDirection; } }
        public List<string> BulletTypes { get { return _BulletTypes; } }
        public double TestBulletWeight {
            get { return _TestBulletWeight; } 
            set { _TestBulletWeight = value; 
                RaisePropertyChanged(nameof(TestBulletWeight)); } 
        }
        public double TestBulletDiameter { get { return _TestBulletDiameter; } set { _TestBulletDiameter = value; RaisePropertyChanged(nameof(TestBulletDiameter)); } }
        public double TestBulletLength { get { return _TestBulletLength; } set { _TestBulletLength = value; RaisePropertyChanged(nameof(TestBulletLength)); } }
        public double TestBulletBC { get { return _TestBulletBC; } set { _TestBulletBC = value; RaisePropertyChanged(nameof(TestBulletBC)); } }
        public string TestBulletType { get { return _TestBulletType; } set { _TestBulletType = value; RaisePropertyChanged(nameof(TestBulletType)); } }
        public string ZeroMessage { get {return _ZeroMessage; } }
        public string DragMessage { get { return _DragMessage; } }
        #endregion

        #region "Relay Commands"
        private RelayCommand _ZeroAtmosphericsCommand;
        private RelayCommand _SaveFileCommand;
        private RelayCommand _SaveFileAsCommand;
        private RelayCommand _CalculateFoCommand;
        private RelayCommand _CalculateF2Command;
        private RelayCommand _CalculateF3Command;
        private RelayCommand _CalculateF4Command;
        private RelayCommand _CalculateMuzzleVelocityCommand;
        private RelayCommand _CalculateVelocityDistanceCommand;

        public RelayCommand CalculateFoCommand
        {
            get
            {
                return _CalculateFoCommand ?? (_CalculateFoCommand = new RelayCommand(() => CalculateFo()));
            }
        }
        public RelayCommand CalculateF2Command
        {
            get
            {
                return _CalculateF2Command ?? (_CalculateF2Command = new RelayCommand(() => CalculateF2()));
            }
        }
        public RelayCommand CalculateF3Command
        {
            get
            {
                return _CalculateF3Command ?? (_CalculateF3Command = new RelayCommand(() => CalculateF3()));
            }
        }
        public RelayCommand CalculateF4Command
        {
            get
            {
                return _CalculateF4Command ?? (_CalculateF4Command = new RelayCommand(() => CalculateF4()));
            }
        }

        public RelayCommand CalculateVelocityDistanceCommand
        {
            get
            {
                return _CalculateVelocityDistanceCommand ?? (_CalculateVelocityDistanceCommand = new RelayCommand(() => CalculateVelocityDistance()));
            }
        }
        public RelayCommand CalculateMuzzleVelocityCommand
        {
            get
            {
                return _CalculateMuzzleVelocityCommand ?? (_CalculateMuzzleVelocityCommand = new RelayCommand(() => CalculateMuzzleVelocity()));
            }
        }
        public RelayCommand EstimateBCCommand { get; set; }
        public RelayCommand OpenBCestimatorCommand { get; set; }
        public RelayCommand RunPreShotCheckCommand { get; set; }
        public RelayCommand ShootCommand { get; set; }
        public RelayCommand SaveFileCommand
        {
            get
            {
                return _SaveFileCommand ?? (_SaveFileCommand = new RelayCommand(() => SaveFile()));
            }
        }
        public RelayCommand SaveFileAsCommand
        {
            get
            {
                return _SaveFileAsCommand ?? (_SaveFileAsCommand = new RelayCommand(() => SaveFileAs()));
            }
        }
        public RelayCommand OpenLocationFinderCommand { get; set; }
        public RelayCommand ZeroLocationCommand { get; set; }
        public RelayCommand ShotLocationCommand { get; set; }
        public RelayCommand ZeroAtmosphericsCommand
        {
            get
            {
                return _ZeroAtmosphericsCommand ?? (_ZeroAtmosphericsCommand = new RelayCommand(() => ZeroAtmospherics()));
            }
        }
        #endregion

        #region "Constructor"
        public SolutionViewModel()
        {            
            _MySolution = new Solution();
            MySolution.PropertyChanged += MySolution_PropertyChanged;
            _TrajectoryPlot = new PlotModel();
            _TrajectoryPlot.Title = "Trajectory";
            _WindagePlot = new PlotModel();
            _WindagePlot.Title = "Horizontal Deviation";
            RaisePropertyChanged(nameof(TrajectoryPlot));
            _BulletTypes = new List<string>();
            string[] values = Enum.GetNames(typeof(BulletShapeEnum));
            foreach (string B in values)
            {
                _BulletTypes.Add(B.ToString());
            }
            EstimateBCCommand = new RelayCommand(GetTestBulletBC, null);
            OpenBCestimatorCommand = new RelayCommand(OpenBCestimator, null);
            RunPreShotCheckCommand = new RelayCommand(RunPreShotCheck, null);
            ShootCommand = new RelayCommand(Shoot, null);
            OpenLocationFinderCommand = new RelayCommand(OpenLocationFinder, null);
            ZeroLocationCommand = new RelayCommand(ZeroLocation, null);
            ShotLocationCommand = new RelayCommand(ShotLocation, null);
        }
        #endregion

        #region "Destructor"
        ~SolutionViewModel()
        {
            MySolution.PropertyChanged -= MySolution_PropertyChanged;
        }
        #endregion

        #region "Public Routines"
        public void LoadDefaultSolution()
        {
            DataPersistence lDP = new DataPersistence();

            string lf = LawlerBallisticsFactory.DataFolder + "\\default.bdf";
            _FileName = lf;
            MySolution = lDP.ParseBallisticSolution(lf);
            SolveZeroData();
            if((Fo>0) & (MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange==0))
                MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange = BallisticFunctions.MaxRange(Fo);
            RaisePropertyChanged(nameof(MyBarrels));
            RaisePropertyChanged(nameof(MyCartridges));
        }
        public void SetShooterLocation(double Alt, double Lat, double Lon)
        {
            MySolution.MyScenario.MyShooter.MyLocation.Altitude = Alt;
            MySolution.MyScenario.MyShooter.MyLocation.Latitude = Lat;
            MySolution.MyScenario.MyShooter.MyLocation.Longitude = Lon;
        }
        public void SetTargetLocation(double Alt, double Lat, double Lon)
        {
            MySolution.MyScenario.SelectedTarget.TargetLocation.Altitude = Alt;
            MySolution.MyScenario.SelectedTarget.TargetLocation.Latitude = Lat;
            MySolution.MyScenario.SelectedTarget.TargetLocation.Longitude = Lon;
        }
        public void SetShooterZeroLocation(double Alt, double Lat, double Lon)
        {
            MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ShooterLoc.Altitude = Alt;
            MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ShooterLoc.Latitude = Lat;
            MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ShooterLoc.Longitude = Lon;
        }
        public void SetTargetZeroLocation(double Alt, double Lat, double Lon)
        {
            MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.TargetLoc.Altitude = Alt;
            MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.TargetLoc.Latitude = Lat;
            MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.TargetLoc.Longitude = Lon;
        }
        public void SetBCestimatefrm(frmBCcalculator TargetBCcalc)
        {
            _frmBCcalc = TargetBCcalc;
        }
        public void LoadCharts()
        {
            //_TrajectoryPlot = new PlotModel();
            //Sight delta for series data.
            _TrajectoryPlot.Series.Clear();
            _TrajectoryPlot.Axes.Clear();
            AreaSeries lZ1 = new AreaSeries()
            {
                StrokeThickness = 2,
                LineStyle = OxyPlot.LineStyle.Solid,
                Color = OxyColors.Blue,
                Color2 = OxyColors.Transparent,
                Fill = OxyColor.FromArgb(100, 50, 168, 82),
                DataFieldX2 = "X",
                ConstantY2 = _MyTrajectories.Min(sd => sd.SightDelta)
            };
            AreaSeries lZ2 = new AreaSeries()
            {
                StrokeThickness = 2,
                LineStyle = OxyPlot.LineStyle.Solid,
                Color = OxyColors.Blue,
                Color2 = OxyColors.Transparent,
                Fill = OxyColor.FromArgb(100, 168, 155, 50),
                DataFieldX2 = "X",
                ConstantY2 = _MyTrajectories.Min(sd => sd.SightDelta)
            };
            AreaSeries lZ3 = new AreaSeries()
            {
                StrokeThickness = 2,
                LineStyle = OxyPlot.LineStyle.Solid,
                Color = OxyColors.Blue,
                Color2 = OxyColors.Transparent,
                Fill = OxyColor.FromArgb(100, 168, 50, 50),
                DataFieldX2 = "X",
                ConstantY2 = _MyTrajectories.Min(sd => sd.SightDelta)
            };
            AreaSeries lZ4 = new AreaSeries()
            {
                StrokeThickness = 2,
                LineStyle = OxyPlot.LineStyle.Solid,
                Color = OxyColors.Blue,
                Color2 = OxyColors.Transparent,
                Fill =  OxyColor.FromArgb(100, 56, 50, 168),
                DataFieldX2 = "X",
                ConstantY2 = _MyTrajectories.Min(sd => sd.SightDelta)
            };
            LineSeries lFT = new LineSeries();
            LineSeries lTPS = new LineSeries();
            LineSeries lZsl = new LineSeries();
            LineSeries lENRGY = new LineSeries();
            LineSeries lSR = new LineSeries();
            LineSeries lBSG = new LineSeries();
            LinearAxis lTLA = new LinearAxis();
            lTLA.Key = "Traj";            
            lTLA.Position = AxisPosition.Left;
            lTLA.MajorGridlineColor = OxyColors.Black;
            lTLA.MajorGridlineStyle = LineStyle.Dot;
            _TrajectoryPlot.Axes.Add(lTLA);           
            lZ1.Title = "Zone 1 Trajectory";
            lZ1.YAxisKey = "Traj";
            lZ2.Title = "Zone 2 Trajectory";
            lZ2.YAxisKey = "Traj";
            lZ3.Title = "Zone 3 Trajectory";
            lZ3.YAxisKey = "Traj";
            lZ4.Title = "Zone 4 Trajectory";
            lZ4.YAxisKey = "Traj";
            lZsl.Title = "Sight line";
            lZsl.YAxisKey = "Traj";
            lTPS.Title = "Velocity (fps)";
            lTPS.Color = OxyColors.Yellow;
            lENRGY.Title = "Energy (ft.lb.)";
            lSR.Title = "Bullet Spin Rate (rpm)";
            lBSG.Title = "Gyroscopic Stability (SG)";           
            foreach (TrajectoryData lTD in _MyTrajectories)
            {
                if (lTD.Range <= Zone1Range)
                {
                    lZ1.Points.Add(new DataPoint(lTD.Range, lTD.SightDelta));
                }
                else if ((lTD.Range > Zone1Range) & (lTD.Range <= Zone2Range))
                {
                    lZ2.Points.Add(new DataPoint(lTD.Range, lTD.SightDelta));
                }
                else if ((lTD.Range > Zone2Range) & (lTD.Range <= Zone3Range))
                {
                    lZ3.Points.Add(new DataPoint(lTD.Range, lTD.SightDelta));
                }
                else
                {
                    lZ4.Points.Add(new DataPoint(lTD.Range, lTD.SightDelta));
                }
                lTPS.Points.Add(new DataPoint(lTD.Range, lTD.Velocity));
                lENRGY.Points.Add(new DataPoint(lTD.Range, lTD.Energy));
                lSR.Points.Add(new DataPoint(lTD.Range, lTD.SpinRate));
                lBSG.Points.Add(new DataPoint(lTD.Range, lTD.GyroStability));
                lZsl.Points.Add(new DataPoint(lTD.Range, 0));
                lFT.Points.Add(new DataPoint(lTD.Range, lTD.FlightTime));
            }                      
            _TrajectoryPlot.Series.Add(lZ1);
            _TrajectoryPlot.Series.Add(lZ2);
            _TrajectoryPlot.Series.Add(lZ3);
            _TrajectoryPlot.Series.Add(lZ4);
            _TrajectoryPlot.Series.Add(lZsl);
            //LinearAxis lTa = new LinearAxis();
            //lTa.MajorGridlineColor = OxyColors.LightGray;
            //lTa.MajorGridlineStyle = LineStyle.Dot;
            //lTa.Key = "Trajectory";
            //_TrajectoryPlot.Axes.Add(lTa);
            if ((PlotEnergy)||(PlotVelocity))
            {
                LinearAxis lVa = new LinearAxis();
                lVa.Key = "VelEng";
                lVa.Position = AxisPosition.Right;
                _TrajectoryPlot.Axes.Add(lVa);
            }
            if (PlotVelocity)
            {                
                lTPS.YAxisKey = "VelEng";
                _TrajectoryPlot.Series.Add(lTPS);
            }
            if (PlotEnergy)
            {
                lENRGY.YAxisKey = "VelEng";
                _TrajectoryPlot.Series.Add(lENRGY);
            }
            if((PlotSpinRate)||(PlotFlightTime))
            {
                LinearAxis lBSGA = new LinearAxis();
                lBSGA.Key = "BSG";
                lBSGA.Position = AxisPosition.Right;
                _TrajectoryPlot.Axes.Add(lBSGA);
            }
            if (PlotFlightTime)
            {
                lFT.Title = "Flight Time";
                lFT.YAxisKey = "BSG";
                _TrajectoryPlot.Series.Add(lFT);

            }
            if (PlotSpinRate)
            {
                LinearAxis lSRA = new LinearAxis();
                lSRA.Key = "SpinRate";
                lSRA.Position = AxisPosition.Right;
                lSR.YAxisKey = "SpinRate";
                _TrajectoryPlot.Axes.Add(lSRA);
                _TrajectoryPlot.Series.Add(lSR);                
                lBSG.YAxisKey = "BSG";               
                _TrajectoryPlot.Series.Add(lBSG);
            }
            _TrajectoryPlot.InvalidatePlot(true);

            if(_WindagePlot.Series.Count>0) _WindagePlot.Series.Clear();
            _WindagePlot.Axes.Clear();
            LineSeries lTHD = new LineSeries()
            {
                StrokeThickness = 2,
                LineStyle = OxyPlot.LineStyle.Solid,
                Color = OxyColors.Blue,                
            };
            LineSeries lSD = new LineSeries();
            LineSeries lWD = new LineSeries();
            LineSeries lCHD = new LineSeries();
            LineSeries lHSL = new LineSeries();
            LinearAxis lTHDA = new LinearAxis();
            double lCMDV;
            lCMDV = Math.Abs(_MyTrajectories.Max(hd => hd.HorizDev));
            if (lCMDV < Math.Abs(_MyTrajectories.Min(hd => hd.HorizDev))) lCMDV = Math.Abs(_MyTrajectories.Min(hd => hd.HorizDev));
            lCMDV = lCMDV + lCMDV / 20;
            lTHDA.Maximum = lCMDV;
            lTHDA.Minimum = -lCMDV;
            lTHDA.Key = "Hdrift";
            lHSL.Title = "Sight Line";
            lHSL.YAxisKey = "Hdrift";
            lTHD.Title = "Horiz. Drift";
            lTHD.YAxisKey = "Hdrift";
            lSD.Title = "Spin Drift";
            lSD.YAxisKey = "Hdrift";
            lWD.Title = "Wind Drift";
            lWD.YAxisKey = "Hdrift";
            lCHD.Title = "Coriolis";
            lCHD.YAxisKey = "Hdrift";
            _WindagePlot.Axes.Add(lTHDA);
            foreach (TrajectoryData lTD in _MyTrajectories)
            {
                lTHD.Points.Add(new DataPoint(lTD.Range, -lTD.HorizDev));
                lSD.Points.Add(new DataPoint(lTD.Range, -lTD.SpinDrift));
                lWD.Points.Add(new DataPoint(lTD.Range, -lTD.WindDeflect));
                lCHD.Points.Add(new DataPoint(lTD.Range, -lTD.CoriolisH));
                lHSL.Points.Add(new DataPoint(lTD.Range, 0));
            }
            _WindagePlot.Series.Add(lHSL);
            _WindagePlot.Series.Add(lTHD);
            if(PlotSpinDrift) _WindagePlot.Series.Add(lSD);
            if(PlotWindDrift) _WindagePlot.Series.Add(lWD);
            if(PlotHcoriolis) _WindagePlot.Series.Add(lCHD);
            _WindagePlot.InvalidatePlot(true);
        }        
        public void Dispose()
        {
            InstanceUnload();
        }
        #endregion

        #region "Private Routines"

        #region "Location Routines"
        private void OpenLocationFinder()
        {
            LocationFinder lLF = new LocationFinder();
            lLF.Show();
        }
        private void ZeroLocation()
        {
            LocationFinder frmZlf = new LocationFinder();
            frmZlf.Latitude = MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ShooterLoc.Latitude;
            frmZlf.Longitude = MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ShooterLoc.Longitude;
            frmZlf.NavigateTo();
            frmZlf.Mode = "Zero";
            frmZlf.DataContext = this;
            frmZlf.Show();
        }
        private void ShotLocation()
        {
            LocationFinder frmZlf = new LocationFinder();
            frmZlf.Latitude = MySolution.MyScenario.MyShooter.MyLocation.Latitude;
            frmZlf.Longitude = MySolution.MyScenario.MyShooter.MyLocation.Longitude;
            frmZlf.NavigateTo();
            frmZlf.Mode = "Shot";
            frmZlf.DataContext = this;
            frmZlf.Show();
        }
        #endregion

        #region "Bullet Routines"
        private void GetTestBulletBC()
        {
            double lBC = 0; BulletShapeEnum lShape;
            if (_frmBCcalc == null) return;
            _frmBCcalc.txtBulletDia.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource();
            _frmBCcalc.txtBulletWt.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource();
            lShape = (BulletShapeEnum) Enum.Parse(typeof(BulletShapeEnum), _TestBulletType);
            lBC = BallisticFunctions.EstimateBC(_TestBulletWeight, _TestBulletDiameter, lShape);
            _TestBulletBC = Math.Round(lBC,4);
            RaisePropertyChanged(nameof(TestBulletBC));
        }
        private void OpenBCestimator()
        {
            _frmBCcalc = new frmBCcalculator();
            _frmBCcalc.DataContext = this;
            _frmBCcalc.Show();
        }
        #endregion

        private void InstanceUnload()
        {
            Cleanup();
        }

        #region "Ballistic Routines"
        private void CalculateFo()
        {
            string lmsg;
            Atmospherics ltmpAt;

            ltmpAt = MyAtmospherics;
            MyAtmospherics = MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics;

            if (SpeedOfSound == 0)
            {
                lmsg = "Speed-of-Sound must be provided before the drag factor can be calculated.";
                LoadDragMessage(lmsg);
                return;
            }
            else if((V1 == 0) || (D2 == 0))
            {
                lmsg = "An initial and second velocity and distance pair must be provided before the drag factor can be calculated." +
                    Environment.NewLine + "    Muzzle velocity and 0ft can be provied for the initial velocity/distance pair.";
                LoadDragMessage(lmsg);
                return;
            }
            MySolution.MyScenario.MyDragSlopeData.Fo = MySolution.MyScenario.MyDragSlopeData.CalculateFo(Zone1TransSpeed, Zone2TransSpeed,
                Zone3TransSpeed);
            MyAtmospherics = ltmpAt;
        }
        private void CalculateF2()
        {
            string lmsg;
            Atmospherics ltmpAt;

            ltmpAt = MyAtmospherics;
            MyAtmospherics = MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics;
            if (SpeedOfSound == 0)
            {
                lmsg = "Speed-of-Sound must be provided before the drag factor can be calculated.";
                LoadDragMessage(lmsg);
                return;
            }
            else if(MySolution.MyScenario.MyDragSlopeData.BCz2 == 0)
            {
                lmsg = "Invalid Zone 2 Ballistic Coefficent.";
                LoadDragMessage(lmsg);
                return;
            }
            MySolution.MyScenario.MyDragSlopeData.F2 = MySolution.MyScenario.MyDragSlopeData.CalculateF2(SpeedOfSound);
            MyAtmospherics = ltmpAt;
        }
        private void CalculateF3()
        {
            string lmsg;
            Atmospherics ltmpAt;

            ltmpAt = MyAtmospherics;
            MyAtmospherics = MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics;
            if (SpeedOfSound == 0)
            {
                lmsg = "Speed-of-Sound must be provided before the drag factor can be calculated.";
                LoadDragMessage(lmsg);
                return;
            }
            MySolution.MyScenario.MyDragSlopeData.F3 = MySolution.MyScenario.MyDragSlopeData.CalculateF3(SpeedOfSound);
            MyAtmospherics = ltmpAt;
        }
        private void CalculateF4()
        {
            string lmsg;
            Atmospherics ltmpAt;

            ltmpAt = MyAtmospherics;
            MyAtmospherics = MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics;
            if (SpeedOfSound == 0)
            {
                lmsg = "Speed-of-Sound must be provided before the drag factor can be calculated.";
                LoadDragMessage(lmsg);
                return;
            }
            MySolution.MyScenario.MyDragSlopeData.F4 = MySolution.MyScenario.MyDragSlopeData.CalculateF4(SpeedOfSound);
            MyAtmospherics = ltmpAt;
        }

        private void RunPreShotCheck()
        {
            int lRtn;

            lRtn = MySolution.MyBallistics.PreflightCheck();
            PostFlightCheck(lRtn);
        }
        private void PostFlightCheck(int FightCheckReturn)
        {
            switch (FightCheckReturn)
            {
                case -1:
                    ShotMsg = "Insufficent data.  Need muzzle velocity and BCg1 or V1, D1, V2, and D2.";
                    break;
                case -2:

                    break;
                case 2:

                    break;
                case 3:
                    ShotMsg = "Fo and Muzzle velocity provided.  V1, D1, V2, D2 provided.  Fo recalculated using velocities and distances. BCg1 recalculated with V1, V2, and (D2-D1) distance.";
                    break;
                case 4:
                    ShotMsg = "V1, D1, V2, and D2 calculated from BCg1 and Muzzle velocity.  Fo calculated from derived velocity data.";
                    break;
            }
        }
        private void Shoot()
        {
            int lRtn;
            double lDR=0;  //Trajectory Range increment.
            double lCR; //Current Range.
            TrajectoryData lTD;

           // lRtn = MySolution.MyBallistics.PreflightCheck();
            //if (lRtn < 0)
            //{
            //    PostFlightCheck(lRtn);
            //    return;
            //}
            lDR = 1;
            lCR = lDR;
            _MyTrajectories = new ObservableCollection<TrajectoryData>();
            while(lCR < MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange)
            {
                lTD = new TrajectoryData();
                lTD.Range = lCR;
                lTD.MuzzleDrop = MuzzleDrop(lCR,true);
                lTD.SightDelta = SightDelta(lCR,true);
                lTD.Velocity = Velocity(lCR);
                lTD.Energy = Energy(lCR);
                lTD.SpinRate = SpinRate(lCR);
                lTD.GyroStability = GyroscopicStability(lCR);
                lTD.HorizDev = TotalHorizontalDrift(lCR, true);
                lTD.CoriolisH = GetCoriolisHoriz(lCR, true);
                lTD.CoriolisV = GetCoriolisVert(lCR, true);
                lTD.SpinDrift = GetSpinDrift(lCR);
                lTD.WindDeflect = WindDrift(lCR);
                lTD.FlightTime = FlightTime(lCR);
                lCR += lDR;
                if(lCR >= MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange)
                {
                    lTD = new TrajectoryData();
                    lTD.Range = MaxRange;
                    lTD.MuzzleDrop = MuzzleDrop(MaxRange, true);
                    lTD.SightDelta = SightDelta(MaxRange, true);
                    lTD.Velocity = Velocity(MaxRange);
                    lTD.Energy = Energy(MaxRange);
                    lTD.SpinRate = SpinRate(MaxRange);
                    lTD.GyroStability = GyroscopicStability(MaxRange);
                    lTD.HorizDev = TotalHorizontalDrift(MaxRange, true);
                    lTD.CoriolisH = GetCoriolisHoriz(MaxRange, true);
                    lTD.CoriolisV = GetCoriolisVert(MaxRange, true);
                    lTD.SpinDrift = GetSpinDrift(MaxRange);
                    lTD.WindDeflect = WindDrift(MaxRange);
                    lTD.FlightTime = FlightTime(MaxRange);
                }
                _MyTrajectories.Add(lTD);
            }
            RaisePropertyChanged(nameof(MyTrajectories));
            LoadCharts();

        }
        private void SolveZeroData()
        {
            if(UseMaxRise)
            {
                if(Fo==0)
                {
                    LoadZeroMessage("Invalid Fo value.");
                    return;
                }
                else if (ZeroMaxRise == 0)
                {
                    LoadZeroMessage("Invalid maximum rise value.");
                    return;
                }
                else if (MuzzleVelocity == 0)
                {
                    LoadZeroMessage("Invalid muzzle velocity.");
                    return;
                }
                else if(ScopeHeight == 0)
                {
                    LoadZeroMessage("Invalid scope height value.");
                }
                double lZd = BallisticFunctions.CalculateZeroRange(Fo, MuzzleVelocity, ZeroMaxRise, ScopeHeight);
                MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ZeroRange = lZd;
            }
            else
            {
                if(ZeroRange <= 0)
                {

                    // A zero range must be provided.
                    LoadZeroMessage("A zero range must be provided.");
                    return;
                }
                else if(ScopeHeight <= 0)
                {
                    // A scope height > 0 must be provided.
                    LoadZeroMessage("A scope height > 0 must be provided.");
                    return;
                }
                else if(MuzzleVelocity <= 0)
                {
                    LoadZeroMessage("Invalid muzzle velocity.");
                    return;
                }

                else
                {
                    MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ZeroMaxRise = BallisticFunctions.CalculateHm(ZeroRange, ZeroRange, ScopeHeight, MuzzleVelocity,
                        Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor,
                        Zone2TransSpeed, Zone2TransSpeed, Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, ZeroTargetLoc, ZeroShooterLoc,
                       ZeroTargetLoc, ZeroShooterLoc);
                    //Reset all target solution data as this affects vertical drop.
                }
            }
            MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.NearZeroRange =
                BallisticFunctions.ZeroNearRange(ZeroMaxRise, ScopeHeight, MuzzleVelocity, Fo);
            MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.MidRange = BallisticFunctions.MidRange(MuzzleVelocity, ZeroMaxRise, ScopeHeight, Fo);
            MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.PointBlankRange = BallisticFunctions.PointBlankRange(ZeroMaxRise, MuzzleVelocity, ScopeHeight, Fo);
            RaisePropertyChanged(nameof(MidRange));
            RaisePropertyChanged(nameof(PointBlankRange));

        }
        private void CalculateMuzzleVelocity()
        {
            double lmv;
            Atmospherics ltmpAt;

            ltmpAt = MyAtmospherics;
            MyAtmospherics = MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics;
            if (V1 ==0)
            {

                LoadDragMessage("Invalid V1 value.");
            }
            else if(Fo == 0)
            {
                LoadDragMessage("Invalid Fo value.");
                return;

            }
            else if(MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics.SpeedOfSound <= 0)
            {
                LoadDragMessage("Invalid atmospheric data.");
                return;
            }
            lmv = BallisticFunctions.MuzzleVelocity(V1, D1, Fo, Zone1Slope, Zone1TransSpeed, Zone2TransSpeed, Zone3Slope, Zone3TransSpeed);
            MyAtmospherics = ltmpAt;
            MySolution.MyScenario.MyShooter.MyLoadOut.MuzzleVelocity = lmv;
            RaisePropertyChanged(nameof(MuzzleVelocity));
        }
        private void CalculateVelocityDistance()
        {
            double lV1;

            if(MuzzleVelocity == 0)
            {
                LoadDragMessage("Invalid muzzle velocity value.");
                return;
            }
            else if (BCg1 == 0)
            {
                LoadDragMessage("Invalid G1 ballistic coefficent value.");
                return;
            }
            MySolution.MyScenario.MyDragSlopeData.V1 = MuzzleVelocity;
            MySolution.MyScenario.MyDragSlopeData.D1 = 0;
            MySolution.MyScenario.MyDragSlopeData.D2 = 300;
            MySolution.MyScenario.MyDragSlopeData.V2 = BallisticFunctions.CalculateV2FromBC(MuzzleVelocity, BCg1, D2);
        }
        #endregion

        #region "File Routines"
        private void SaveFileAs()
        {
            SaveFileDialog lSFD = new SaveFileDialog();
            DataPersistence lDP = new DataPersistence();

            lSFD.Filter = lDP.BallisticFileFilter;
            lSFD.InitialDirectory = LawlerBallisticsFactory.DataFolder;
            lSFD.RestoreDirectory = true;
            //lSFD.CheckFileExists = true;
            lSFD.OverwritePrompt = true;
            lSFD.Title = "Save Ballistic Solution";            
            if (lSFD.ShowDialog() == DialogResult.OK)
            {
                lDP.SaveBallisticSolutionData(MySolution, lSFD.FileName);
            }
        }
        private void SaveFile()
        {                        
            if (_FileName == "")
            {
                SaveFileAs();
                return;
            }
            DataPersistence lDP = new DataPersistence();
            lDP.SaveBallisticSolutionData(MySolution, _FileName);
        }
        #endregion

        #region "Atmospheric Routines"
        private void ZeroAtmospherics()
        {
            if ((MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ShooterLoc.Latitude == 0) ||
                    (MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ShooterLoc.Longitude == 0))
            {
                _ZeroMsgVal = 1;
                ParseZeroMessages();
            }
            else
            {
                MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.LoadCurrentLocationWeather();
            }
        }
        #endregion

        #endregion

        #region "Solution Data Aliases"
        public ObservableCollection<Gun> MyGuns { get { return LawlerBallisticsFactory.MyGuns; } }
        public bool UseMaxRise { get { return MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.UseMaxRise; } }
        public double BarrelTwist { get { return MyBarrel.Twist; } }
        public string BarrelTwistDirection { get { return MyBarrel.RiflingTwistDirection; } }
        public double BaroPressure { get { return MyAtmospherics.Pressure; } }
        public double BCg1 { get { return MySolution.MyScenario.MyDragSlopeData.BCg1; } }
        public double BSG { get { return MySolution.MyScenario.MyShooter.MyLoadOut.BSG; } }
        public double BulletDiameter { get { return MyBullet.Diameter; } }
        public double BulletWeight { get { return MyBullet.Weight; } }
        public double DensityAlt { get { return MySolution.MyScenario.MyAtmospherics.DensityAlt; } }
        public double DensityAltAtZero { get { return MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics.DensityAlt; } }
        public double D1 { get { return MySolution.MyScenario.MyDragSlopeData.D1; } }
        public double D2 { get { return MySolution.MyScenario.MyDragSlopeData.D2; } }
        public double V1 { get { return MySolution.MyScenario.MyDragSlopeData.V1; } }
        public double V2 { get { return MySolution.MyScenario.MyDragSlopeData.V2; } }
        public double Fo { get { return MySolution.MyScenario.MyDragSlopeData.Fo; } }
        public double F2 { get { return MySolution.MyScenario.MyDragSlopeData.F2; } }
        public double F3 { get { return MySolution.MyScenario.MyDragSlopeData.F3; } }
        public double F4 { get { return MySolution.MyScenario.MyDragSlopeData.F4; } }
        public double MaxRange { get { return MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange; } }
        public double MidRange { get { return MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.MidRange; } }
        public double MuzzleVelocity { get { return MySolution.MyScenario.MyShooter.MyLoadOut.MuzzleVelocity; } }
        public Atmospherics MyAtmospherics { get { return MySolution.MyScenario.MyAtmospherics; } set { MySolution.MyScenario.MyAtmospherics = value; RaisePropertyChanged(nameof(MyAtmospherics)); } }
        public Barrel MyBarrel { get { return MySolution.MyScenario.MyShooter.MyLoadOut.SelectedBarrel; } }
        public ObservableCollection<Barrel> MyBarrels { get { return MySolution.MyScenario.MyShooter.MyLoadOut.SelectedGun.Barrels; } }
        public ObservableCollection<Recipe> MyCartridges { get { return LawlerBallisticsFactory.BarrelRecipes(MyBarrel.ID); } }
        public Bullet MyBullet { get { return MySolution.MyScenario.MyShooter.MyLoadOut.SelectedCartridge.RecpBullet; } }
        public double PointBlankRange { get { return MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.PointBlankRange; } }
        public double ScopeHeight { get { return MySolution.MyScenario.MyShooter.MyLoadOut.ScopeHeight; } }
        public double SpeedOfSound { get { return MySolution.MyScenario.MyAtmospherics.SpeedOfSound; } }
        public double TempF { get { return MySolution.MyScenario.MyAtmospherics.Temp; } }
        public double WindDirection { get { return MySolution.MyScenario.MyAtmospherics.WindDirection; } }
        public double WindSpeed { get { return MySolution.MyScenario.MyAtmospherics.WindSpeed; } }
        public double Zone1Range { get { return MySolution.MyScenario.MyDragSlopeData.Zone1Range(MuzzleVelocity,SpeedOfSound); } }
        public double Zone1AngleFactor { get { return MySolution.MyScenario.MyDragSlopeData.Zone1AngleFactor; } }
        public double Zone1Slope { get { return MySolution.MyScenario.MyDragSlopeData.Zone1Slope; } }
        public double Zone1SlopeMultiplier { get { return MySolution.MyScenario.MyDragSlopeData.Zone1SlopeMultiplier; } }
        public double Zone1TransSpeed { get { return MySolution.MyScenario.MyDragSlopeData.Zone1TransSpeed(MyAtmospherics.SpeedOfSound); } }
        public double Zone2Range { get { return MySolution.MyScenario.MyDragSlopeData.Zone2Range(MuzzleVelocity, SpeedOfSound); } }
        public double Zone2TransSpeed { get { return MySolution.MyScenario.MyDragSlopeData.Zone2TransSpeed(MyAtmospherics.SpeedOfSound); } }
        public double Zone3Range { get { return MySolution.MyScenario.MyDragSlopeData.Zone3Range(MuzzleVelocity,SpeedOfSound); } }
        public double Zone3Slope { get { return MySolution.MyScenario.MyDragSlopeData.Zone3Slope; } }
        public double Zone3SlopeMultiplier { get { return MySolution.MyScenario.MyDragSlopeData.Zone3SlopeMultiplier; } }
        public double Zone3TransSpeed { get { return MySolution.MyScenario.MyDragSlopeData.Zone3TransSpeed(MyAtmospherics.SpeedOfSound); } }
        public double ZeroMaxRise { get { return MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ZeroMaxRise; } }
        public LocationData ZeroTargetLoc { get { return MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.TargetLoc; } }
        public LocationData ZeroShooterLoc { get { return MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ShooterLoc; } }
        public LocationData TargetLoc { get { return MySolution.MyScenario.SelectedTarget.TargetLocation; } }
        public LocationData ShooterLoc { get { return MySolution.MyScenario.MyShooter.MyLocation; } }
        public double ZeroRange { get { return MySolution.MyScenario.MyShooter.MyLoadOut.zeroData.ZeroRange; } }
        #endregion

        #region "Solution Routine Aliases"
        public double FlightTime(double Range)
        {
            double lRTN = 0;

            lRTN = BallisticFunctions.FlightTime(Range, Fo, MuzzleVelocity);

            return lRTN;
        }
        public double GyroscopicStability(double Range)
        {
            double lRTN = 0;

            lRTN = BallisticFunctions.GyroscopicStability(MyBullet, MyBarrel, Velocity(Range), TempF, BaroPressure);

            return lRTN;
        }
        public double SightDelta(double Range, bool ZeroData = false)
        {
            double lRTN = 0;
            if (!ZeroData)
            {
                lRTN = BallisticFunctions.SightDelta(Range, ZeroRange, ScopeHeight, MuzzleVelocity, Zone1Range, Zone2Range, Zone3Range,
                    Zone1Slope, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed,
                    Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, ZeroTargetLoc, ZeroShooterLoc, TargetLoc, ShooterLoc);
            }
            else if(ZeroData)
            {
                lRTN = BallisticFunctions.SightDelta(Range, ZeroRange, ScopeHeight, MuzzleVelocity, Zone1Range, Zone2Range, Zone3Range,
                    Zone1Slope, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed,
                    Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, ZeroTargetLoc, ZeroShooterLoc, ZeroTargetLoc, ZeroShooterLoc);
            }
            return lRTN;
        }
        public double GetCoriolisHoriz(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.GetCoriolisHoriz(Range, MuzzleVelocity, ZeroTargetLoc, ZeroShooterLoc,
                TargetLoc, ShooterLoc, ZeroRange, Fo);
            }
            else
            {
                lRTN = BallisticFunctions.GetCoriolisHoriz(Range, MuzzleVelocity, ZeroTargetLoc, ZeroShooterLoc,
                ZeroTargetLoc, ZeroShooterLoc, ZeroRange, Fo);
            }

            return lRTN;
        }
        public double GetCoriolisVert(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.GetCoriolisVert(Range, ZeroTargetLoc, ZeroShooterLoc,
                    TargetLoc, ShooterLoc, ZeroRange, Fo, MuzzleVelocity);
            }
            else
            {
                lRTN = BallisticFunctions.GetCoriolisVert(Range, ZeroTargetLoc, ZeroShooterLoc,
                    ZeroTargetLoc, ZeroShooterLoc, ZeroRange, Fo, MuzzleVelocity);
            }
            return lRTN;
        }
        public double GetSpinDrift(double Range)
        {
            double lRTN = 0;

            lRTN = BallisticFunctions.GetSpinDrift(Range, BarrelTwistDirection, BSG, Fo, MuzzleVelocity, ZeroRange);

            return lRTN;
        }
        public double SpinRate(double Range)
        {
            double lRTN = 0;

            lRTN = BallisticFunctions.SpinRate(Range, MuzzleVelocity, BarrelTwist, BulletDiameter, Fo);

            return lRTN;
        }
        public double TotalHorizontalDrift(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.TotalHorizontalDrift(Range, WindSpeed, WindDirection, MuzzleVelocity, Fo, ZeroTargetLoc,
                    ZeroShooterLoc, TargetLoc, ShooterLoc, ZeroRange, BarrelTwistDirection, BSG);
            }
            else
            {
                lRTN = BallisticFunctions.TotalHorizontalDrift(Range, WindSpeed, WindDirection, MuzzleVelocity, Fo, ZeroTargetLoc,
                    ZeroShooterLoc, ZeroTargetLoc, ZeroShooterLoc, ZeroRange, BarrelTwistDirection, BSG);
            }
            return lRTN;
        }
        public double Velocity(double Range)
        {
            double lRTN = 0;

            lRTN = BallisticFunctions.Velocity(MuzzleVelocity, Range, Zone1Range, Zone1TransSpeed, Fo, Zone1Slope, Zone2Range,
                Zone2TransSpeed, F2, Zone3Range);

            return lRTN;
        }
        public double WindDrift(double Range)
        {
            double lRTN = 0;

            lRTN = BallisticFunctions.WindDrift(WindSpeed, WindDirection, Range, Fo, MuzzleVelocity);

            return lRTN;
        }
        public double MuzzleDrop(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.MuzzleDrop(MuzzleVelocity, Range, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone3Slope,
                    Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed, Zone3TransSpeed, Fo,
                    F2, F3, F4, DensityAlt, DensityAltAtZero, ZeroTargetLoc, ZeroShooterLoc, TargetLoc, ShooterLoc, ZeroRange);
            }
            else if(ZeroData)
            {
                lRTN = BallisticFunctions.MuzzleDrop(MuzzleVelocity, Range, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone3Slope,
                    Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed, Zone3TransSpeed, Fo,
                    F2, F3, F4, DensityAlt, DensityAltAtZero, ZeroTargetLoc, ZeroShooterLoc, ZeroTargetLoc, ZeroShooterLoc, ZeroRange);
            }
            return lRTN;
        }
        public double Energy(double Range)
        {
            double lRTN = 0;

            lRTN = BallisticFunctions.Energy(BulletWeight, Velocity(Range));

            return lRTN;
        }
        #endregion

        #region "Zero Messages"
        private void ParseZeroMessages()
        {
            string lmsg;

            switch(_ZeroMsgVal)
            {
                case 1:
                    lmsg = "A shooter location must be provided before retrieving the weather data.";
                    LoadZeroMessage(lmsg);
                    break;
                default:
                    lmsg = "";
                    break;
            }
            RaisePropertyChanged(nameof(ZeroMessage));
        }
        private void LoadZeroMessage(string msg)
        {
            string lmsg="";

            for (int I = 11; I > 0; I--)
            {
                _ZeroMsg[I] = _ZeroMsg[I - 1];
            }
            _ZeroMsg[0] = "::> " + msg;
            for(int I=0; I < 12; I++)
            {
                lmsg = lmsg + _ZeroMsg[I] + System.Environment.NewLine;
            }
            _ZeroMessage = lmsg;
            RaisePropertyChanged(nameof(ZeroMessage));
        }
        private void LoadDragMessage(string msg)
        {
            string lmsg = "";

            for (int I = 11; I > 0; I--)
            {
                _DragMsg[I] = _DragMsg[I - 1];
            }
            _DragMsg[0] = "::> " + msg;
            for (int I = 0; I < 12; I++)
            {
                lmsg = lmsg + _DragMsg[I] + System.Environment.NewLine;
            }
            _DragMessage = lmsg;
            RaisePropertyChanged(nameof(DragMessage));
        }
        #endregion
    }
}
