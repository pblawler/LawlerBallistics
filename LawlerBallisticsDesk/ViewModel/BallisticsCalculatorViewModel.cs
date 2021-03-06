﻿using System;
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

namespace LawlerBallisticsDesk.ViewModel
{
    public class BallisticsCalculatorViewModel : ViewModelBase, IDisposable
    {
        #region "Private Variables"
        private Ballistics _MyBallisticsCalculator;
        private List<string> _BulletTypes;
        private double _TestBulletWeight;
        private double _TestBulletDiameter;
        private double _TestBulletLength;
        private double _TestBulletBC;
        private string _TestBulletType;
        private frmBCcalculator _frmBCcalc;
        private string[] _BarrelDirList = new string[2] { "R", "L" };
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
        public string[] BarrelDirList { get { return _BarrelDirList; } }
        public Ballistics MyBallisticsCalculator { get { return _MyBallisticsCalculator; } }
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
        #endregion

        #region "Relay Commands"
        public RelayCommand EstimateBCCommand { get; set; }
        public RelayCommand OpenBCestimatorCommand { get; set; }
        public RelayCommand RunPreShotCheckCommand { get; set; }
        public RelayCommand ShootCommand { get; set; }
        public RelayCommand SaveFileCommand { get; set; }
        public RelayCommand SaveFileAsCommand { get; set; }
        public RelayCommand OpenRangeFinderCommand { get; set; }
        public RelayCommand ZeroLocationCommand { get; set; }
        public RelayCommand ShotLocationCommand { get; set; }        
        #endregion

        #region "Constructor"
        public BallisticsCalculatorViewModel()
        {
            _FileName = "";
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
            SaveFileCommand = new RelayCommand(SaveFile, null);
            SaveFileAsCommand = new RelayCommand(SaveFileAs, null);
            OpenRangeFinderCommand = new RelayCommand(OpenRangeFinder, null);
            ZeroLocationCommand = new RelayCommand(ZeroLocation, null);
            ShotLocationCommand = new RelayCommand(ShotLocation, null);
            DataPersistence lDP = new DataPersistence();
            Ballistics lBCls;
            string lf = lDP.AppDataFolder + "\\default.bdf";
            lBCls = lDP.ParseBallisticSolution(lf);
            _MyBallisticsCalculator = lBCls;
            if (_MyBallisticsCalculator.ShotDistance == 0) _MyBallisticsCalculator.ShotDistance = _MyBallisticsCalculator.MaxRange() * 0.75;
            Shoot();
        }
        #endregion

        #region "Public Routines"      
        public void SetShooterLocation(double Lat, double Lon)
        {
            _MyBallisticsCalculator.ShooterLat = Lat;
            _MyBallisticsCalculator.ShooterLon = Lon;
        }
        public void SetTargetLocation(double Lat, double Lon)
        {
            _MyBallisticsCalculator.TargetLat = Lat;
            _MyBallisticsCalculator.TargetLon = Lon;
        }
        public void SetShooterZeroLocation(double Lat, double Lon)
        {
            _MyBallisticsCalculator.zShooterLat = Lat;
            _MyBallisticsCalculator.zShooterLon = Lon;
        }
        public void SetTargetZeroLocation(double Lat, double Lon)
        {
            _MyBallisticsCalculator.zTargetLat = Lat;
            _MyBallisticsCalculator.zTargetLon = Lon;
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
                if (lTD.Range <= _MyBallisticsCalculator.Zone1Range)
                {
                    lZ1.Points.Add(new DataPoint(lTD.Range, lTD.SightDelta));
                }
                else if ((lTD.Range > _MyBallisticsCalculator.Zone1Range) & (lTD.Range <= _MyBallisticsCalculator.Zone2Range))
                {
                    lZ2.Points.Add(new DataPoint(lTD.Range, lTD.SightDelta));
                }
                else if ((lTD.Range > _MyBallisticsCalculator.Zone2Range) & (lTD.Range <= _MyBallisticsCalculator.Zone3Range))
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
        private void OpenRangeFinder()
        {
            LocationFinder lLF = new LocationFinder();
            lLF.Show();
        }
        private void GetTestBulletBC()
        {
            double lBC = 0; BulletShapeEnum lShape;
            if (_frmBCcalc == null) return;
            _frmBCcalc.txtBulletDia.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource();
            _frmBCcalc.txtBulletWt.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource();
            lShape = (BulletShapeEnum) Enum.Parse(typeof(BulletShapeEnum), _TestBulletType);
            lBC = MyBallisticsCalculator.EstimateBC(_TestBulletWeight, _TestBulletDiameter, lShape);
            _TestBulletBC = Math.Round(lBC,4);
            RaisePropertyChanged(nameof(TestBulletBC));
        }
        private void InstanceUnload()
        {
            Cleanup();
        }
        private void OpenBCestimator()
        {
            _frmBCcalc = new frmBCcalculator();
            _frmBCcalc.DataContext = this;
            _frmBCcalc.Show();
        }
        private void RunPreShotCheck()
        {
            int lRtn;

            lRtn = _MyBallisticsCalculator.PreflightCheck();
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

            lRtn = _MyBallisticsCalculator.PreflightCheck();
            if (lRtn < 0)
            {
                PostFlightCheck(lRtn);
                return;
            }
            lDR = 1;
            lCR = lDR;
            _MyTrajectories = new ObservableCollection<TrajectoryData>();
            while(lCR < _MyBallisticsCalculator.ShotDistance)
            {
                lTD = new TrajectoryData();
                lTD.Range = lCR;
                lTD.MuzzleDrop = MyBallisticsCalculator.MuzzleDrop(lCR);
                lTD.SightDelta = MyBallisticsCalculator.SightDelta(lCR);
                lTD.Velocity = MyBallisticsCalculator.Velocity(lCR);
                lTD.Energy = MyBallisticsCalculator.Energy(MyBallisticsCalculator.BulletWeight, lCR);
                lTD.SpinRate = MyBallisticsCalculator.SpinRate(lCR);
                lTD.GyroStability = MyBallisticsCalculator.GyroscopicStability(MyBallisticsCalculator.Velocity(lCR), MyBallisticsCalculator.TempF, MyBallisticsCalculator.BaroPressure);
                lTD.HorizDev = MyBallisticsCalculator.TotalHorizontalDrift(lCR);
                lTD.CoriolisH = MyBallisticsCalculator.GetCoriolisHoriz(lCR);
                lTD.CoriolisV = MyBallisticsCalculator.GetCoriolisVert(lCR);
                lTD.SpinDrift = MyBallisticsCalculator.GetSpinDrift(lCR);
                lTD.WindDeflect = MyBallisticsCalculator.WindDriftDegrees(_MyBallisticsCalculator.WindSpeed, _MyBallisticsCalculator.WindDirectionDeg, lCR);
                lTD.FlightTime = MyBallisticsCalculator.FlightTime(lCR);
                lCR += lDR;
                if(lCR >= _MyBallisticsCalculator.ShotDistance)
                {
                    lTD = new TrajectoryData();
                    lTD.Range = _MyBallisticsCalculator.ShotDistance;
                    lTD.MuzzleDrop = MyBallisticsCalculator.MuzzleDrop(_MyBallisticsCalculator.ShotDistance);
                    lTD.SightDelta = MyBallisticsCalculator.SightDelta(_MyBallisticsCalculator.ShotDistance);
                    lTD.Velocity = MyBallisticsCalculator.Velocity(_MyBallisticsCalculator.ShotDistance);
                    lTD.Energy = MyBallisticsCalculator.Energy(MyBallisticsCalculator.BulletWeight, _MyBallisticsCalculator.ShotDistance);
                    lTD.SpinRate = MyBallisticsCalculator.SpinRate(lCR);
                    lTD.GyroStability = MyBallisticsCalculator.GyroscopicStability(MyBallisticsCalculator.Velocity(_MyBallisticsCalculator.ShotDistance), MyBallisticsCalculator.TempF, MyBallisticsCalculator.BaroPressure);
                    lTD.HorizDev = MyBallisticsCalculator.TotalHorizontalDrift(_MyBallisticsCalculator.ShotDistance);
                    lTD.CoriolisH = MyBallisticsCalculator.GetCoriolisHoriz(_MyBallisticsCalculator.ShotDistance);
                    lTD.CoriolisV = MyBallisticsCalculator.GetCoriolisVert(_MyBallisticsCalculator.ShotDistance);
                    lTD.SpinDrift = MyBallisticsCalculator.GetSpinDrift(_MyBallisticsCalculator.ShotDistance);
                    lTD.WindDeflect = MyBallisticsCalculator.WindDriftDegrees(_MyBallisticsCalculator.WindSpeed, _MyBallisticsCalculator.WindDirectionDeg, _MyBallisticsCalculator.ShotDistance);
                    lTD.FlightTime = MyBallisticsCalculator.FlightTime(_MyBallisticsCalculator.ShotDistance);
                }
                _MyTrajectories.Add(lTD);
            }
            RaisePropertyChanged(nameof(MyTrajectories));
            LoadCharts();

        }
        private void SaveFileAs()
        {
            SaveFileDialog lSFD = new SaveFileDialog();
            DataPersistence lDP = new DataPersistence();

            lSFD.Filter = lDP.BallisticFileFilter;
            lSFD.InitialDirectory = lDP.AppDataFolder;
            lSFD.RestoreDirectory = true;
            //lSFD.CheckFileExists = true;
            lSFD.OverwritePrompt = true;
            lSFD.Title = "Save Ballistic Solution";            
            if (lSFD.ShowDialog() == DialogResult.OK)
            {
                lDP.SaveBallisticSolutionData(_MyBallisticsCalculator, lSFD.FileName);
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
            lDP.SaveBallisticSolutionData(_MyBallisticsCalculator, _FileName);
        }
        private void ZeroLocation()
        {
            LocationFinder frmZlf = new LocationFinder();
            frmZlf.Latitude = _MyBallisticsCalculator.zShooterLat;
            frmZlf.Longitude = _MyBallisticsCalculator.zShooterLon;
            frmZlf.NavigateTo();
            frmZlf.Mode = "Zero";
            frmZlf.DataContext = this;
            frmZlf.Show();
        }
        private void ShotLocation()
        {
            LocationFinder frmZlf = new LocationFinder();
            frmZlf.Latitude = _MyBallisticsCalculator.ShooterLat;
            frmZlf.Longitude = _MyBallisticsCalculator.ShooterLon;
            frmZlf.NavigateTo();
            frmZlf.Mode = "Shot";
            frmZlf.DataContext = this;
            frmZlf.Show();
        }
        #endregion
    }
}
