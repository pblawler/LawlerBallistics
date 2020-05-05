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
        //TODO: Add zero windage compensation to wind drift.
        //TODO: Add property changed catch for bcg1 to fire property changed for bcz2 since they are linked need to update
        //TODO: when the cartridge is selected update BCg1 in drag slope


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
                    if (MySolution.UseMaxRise) MySolution.SolveZeroData();
                    break;
                case "ZeroRange":
                    if (!MySolution.UseMaxRise) MySolution.SolveZeroData();
                    break;
                case "ScopeHeight":
                    MySolution.SolveZeroData();
                    break;
                case "MuzzleVelocity":
                    MySolution.SolveZeroData();
                    break;
                case "Fo":
                    MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange = BallisticFunctions.MaxRange(MySolution.Fo);
                    break;
                case "SelectedGun":
                    RaisePropertyChanged(nameof(MySolution.MyBarrels));
                    break;
                case "SelectedBarrel":
                    RaisePropertyChanged(nameof(MySolution.MyCartridges));
                    break;
                case "ZeroMessage":
                    LoadZeroMessage(MySolution.ZeroMessage);
                    break;
                case "DragMessage":
                    LoadDragMessage(MySolution.DragMessage);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region "Private Variables"
        private int _ZeroMsgVal = 0;
        private string _ZeroMessage;
        private string _DragMessage;
        private string[] _ZeroMsg = new string[12];
        private string[] _DragMsg = new string[12];
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
        private PlotModel _DragPlot;
        private ObservableCollection<TrajectoryData> _MyTrajectories;
        private bool _PlotVelocity = true;
        private bool _PlotEnergy = true;
        private bool _PlotSpinRate = true;
        private bool _PlotHcoriolis = true;
        private bool _PlotSpinDrift = true;
        private bool _PlotWindDrift = true;
        private bool _PlotFlightTime = true;
        private bool _PlotComp = true;
        private bool _HorizDrift = true;
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
        public bool PlotHorizDrift
        {
            get
            {
                return _HorizDrift;
            }
            set
            {
                _HorizDrift = value;
                RaisePropertyChanged(nameof(PlotHorizDrift));
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
        public bool PlotComp
        {
            get
            {
                return _PlotComp;
            }
            set
            {
                _PlotComp = value;
                RaisePropertyChanged(nameof(PlotComp));
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
            MySolution.SolveZeroData();
            if((MySolution.Fo >0) & (MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange==0))
                MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange = BallisticFunctions.MaxRange(MySolution.Fo);
            RaisePropertyChanged(nameof(MySolution.MyBarrels));
            RaisePropertyChanged(nameof(MySolution.MyCartridges));
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
                if (lTD.Range <= MySolution.Zone1Range)
                {
                    lZ1.Points.Add(new DataPoint(lTD.Range, lTD.SightDelta));
                }
                else if ((lTD.Range > MySolution.Zone1Range) & (lTD.Range <= MySolution.Zone2Range))
                {
                    lZ2.Points.Add(new DataPoint(lTD.Range, lTD.SightDelta));
                }
                else if ((lTD.Range > MySolution.Zone2Range) & (lTD.Range <= MySolution.Zone3Range))
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
            LineSeries lHerr = new LineSeries();
            LineSeries lZcomp = new LineSeries();
            LinearAxis lTHDA = new LinearAxis();
            double lCMDV;
            lCMDV = Math.Abs(_MyTrajectories.Max(hd => hd.HorizDev));
            if (lCMDV < Math.Abs(_MyTrajectories.Min(hd => hd.HorizDev))) lCMDV = Math.Abs(_MyTrajectories.Min(hd => hd.HorizDev));
            lCMDV = lCMDV + lCMDV / 20;
            lTHDA.Maximum = lCMDV;
            lTHDA.Minimum = -lCMDV;
            lTHDA.StartPosition = 1;
            lTHDA.EndPosition = 0;
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
            lHerr.Title = "Horizontal Trajectory";
            lHerr.YAxisKey = "Hdrift";
            lZcomp.Title = "Zero Compensation";
            lZcomp.YAxisKey = "Hdrift";
            _WindagePlot.Axes.Add(lTHDA);
            foreach (TrajectoryData lTD in _MyTrajectories)
            {
                lTHD.Points.Add(new DataPoint(lTD.Range, lTD.HorizDev));
                lSD.Points.Add(new DataPoint(lTD.Range, lTD.SpinDrift));
                lWD.Points.Add(new DataPoint(lTD.Range, lTD.WindDeflect));
                lCHD.Points.Add(new DataPoint(lTD.Range, lTD.CoriolisH));
                lHSL.Points.Add(new DataPoint(lTD.Range, 0));
                lHerr.Points.Add(new DataPoint(lTD.Range, lTD.HorizErr));
                lZcomp.Points.Add(new DataPoint(lTD.Range, lTD.HorzComp));
            }
            _WindagePlot.Series.Add(lHSL);            
            _WindagePlot.Series.Add(lHerr);
            if (PlotSpinDrift) _WindagePlot.Series.Add(lSD);
            if(PlotWindDrift) _WindagePlot.Series.Add(lWD);
            if(PlotHcoriolis) _WindagePlot.Series.Add(lCHD);
            if(PlotComp) _WindagePlot.Series.Add(lZcomp);
            if(PlotHorizDrift) _WindagePlot.Series.Add(lTHD);
            _WindagePlot.InvalidatePlot(true);
        }        
        public void Dispose()
        {
            InstanceUnload();
        }
        #endregion

        #region "Private Routines"

        //TODO: Add drag chart function for 1/F and K to update with zone data.

        private void Shoot()
        {
            double lDR = 0;  //Trajectory Range increment.
            double lCR; //Current Range.
            double ldf;
            TrajectoryData lTD;

            lDR = 1;
            lCR = lDR;

            _MyTrajectories = new ObservableCollection<TrajectoryData>();
            while (lCR < MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange)
            {
                ldf = BallisticFunctions.Fa(lCR, MySolution.Fo, MySolution.F2, MySolution.F3, MySolution.F4, MySolution.Zone1Range,
                    MySolution.Zone2Range, MySolution.Zone3Range, MySolution.Zone1Slope, MySolution.Zone3Slope, MySolution.Zone1SlopeMultiplier,
                    MySolution.Zone3SlopeMultiplier);
                ldf = 1 / ldf;

                lTD = new TrajectoryData();
                lTD.Range = lCR;
                lTD.MuzzleDrop = MySolution.MuzzleDrop(lCR, true);
                lTD.SightDelta = MySolution.SightDelta(lCR, true);
                lTD.Velocity = MySolution.Velocity(lCR, true);
                lTD.Energy = MySolution.Energy(lCR, true);
                lTD.SpinRate = MySolution.SpinRate(lCR, true);
                lTD.GyroStability = MySolution.GyroscopicStability(lCR, true);
                lTD.HorizDev = MySolution.TotalHorizontalDrift(lCR, true);
                lTD.HorzComp = MySolution.ZeroTotalHorizontalComp(lCR);
                lTD.HorizErr = MySolution.GetHorizErr(lCR, true);
                lTD.CoriolisH = MySolution.GetCoriolisHoriz(lCR, true);
                lTD.CoriolisV = MySolution.GetCoriolisVert(lCR, true);
                lTD.SpinDrift = MySolution.GetSpinDrift(lCR, true);
                lTD.WindDeflect = MySolution.WindDrift(lCR, true);
                lTD.FlightTime = MySolution.FlightTime(lCR, true);
                lTD.Fdragfactor = MySolution.Fdrag(lCR, true);
                lTD.CDdragCoefficient = MySolution.CDdragCoefficient(lCR, true);
                lCR += lDR;
                if (lCR >= MySolution.MyScenario.MyShooter.MyLoadOut.MaxRange)
                {
                    lTD = new TrajectoryData();
                    lTD.Range = MySolution.MaxRange;
                    lTD.MuzzleDrop = MySolution.MuzzleDrop(MySolution.MaxRange, true);
                    lTD.SightDelta = MySolution.SightDelta(MySolution.MaxRange, true);
                    lTD.Velocity = MySolution.Velocity(MySolution.MaxRange, true);
                    lTD.Energy = MySolution.Energy(MySolution.MaxRange, true);
                    lTD.SpinRate = MySolution.SpinRate(MySolution.MaxRange, true);
                    lTD.GyroStability = MySolution.GyroscopicStability(MySolution.MaxRange, true);
                    lTD.HorizDev = MySolution.TotalHorizontalDrift(MySolution.MaxRange, true);
                    lTD.HorzComp = MySolution.ZeroTotalHorizontalComp(MySolution.MaxRange);
                    lTD.HorizErr = MySolution.GetHorizErr(MySolution.MaxRange, true);
                    lTD.CoriolisH = MySolution.GetCoriolisHoriz(MySolution.MaxRange, true);
                    lTD.CoriolisV = MySolution.GetCoriolisVert(MySolution.MaxRange, true);
                    lTD.SpinDrift = MySolution.GetSpinDrift(MySolution.MaxRange, true);
                    lTD.WindDeflect = MySolution.WindDrift(MySolution.MaxRange, true);
                    lTD.FlightTime = MySolution.FlightTime(MySolution.MaxRange, true);
                    lTD.Fdragfactor = MySolution.Fdrag(MySolution.MaxRange, true);
                    lTD.CDdragCoefficient = MySolution.CDdragCoefficient(MySolution.MaxRange, true);
                }
                _MyTrajectories.Add(lTD);
            }
            RaisePropertyChanged(nameof(MyTrajectories));
            LoadCharts();

        }

        private void CalculateFo()
        {
            MySolution.CalculateFo();
        }
        private void CalculateF2()
        {
            MySolution.CalculateF2();
        }
        private void CalculateF3()
        {
            MySolution.CalculateF3();
        }
        private void CalculateF4()
        {
            MySolution.CalculateF4();
        }
        private void CalculateMuzzleVelocity()
        {
            MySolution.CalculateMuzzleVelocity();
        }
        private void CalculateVelocityDistance()
        {
            MySolution.CalculateVelocityDistance();
        }

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

        #region "Messages"
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
            string lzm;

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
            string ldm;

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
