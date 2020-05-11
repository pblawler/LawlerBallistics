using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes.BallisticClasses
{
    public enum Zone : short
    {
        Zone1 = 1,
        Zone2 = 2,
        Zone3 = 3,
        Zone4 = 4

    }

    public class Solution : INotifyPropertyChanged
    {
        //TODO: Verify all Zero calculation function calls are using zero data parameters.

        #region "Binding"
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void MyScenario_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName)
            {
                case "Message":
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region "Private Variables"
        private Scenario _MyScenario;
        private string _ZeroMessage;
        private string _DragMessage;
        private const double _kgm3Tolbft3 = 0.062428;
        #endregion

        #region "Properties"
        public Scenario MyScenario
        {
            get
            {
                return _MyScenario;
            }
            set
            {
                MyScenario.PropertyChanged -= MyScenario_PropertyChanged;
                _MyScenario = value;
                MyScenario.PropertyChanged += MyScenario_PropertyChanged;
                RaisePropertyChanged(nameof(MyScenario)); 
            }
        }
        public string ZeroMessage { get { return _ZeroMessage; } set { _ZeroMessage = value; RaisePropertyChanged(nameof(ZeroMessage)); } }
        public string DragMessage { get { return _DragMessage; } set { _DragMessage = value; RaisePropertyChanged(nameof(DragMessage)); } }
        #endregion

        #region "Constructor"
        public Solution()
        {
            _MyScenario = new Scenario();
            MyScenario.PropertyChanged += MyScenario_PropertyChanged;
        }
        #endregion

        #region "Destructor"
        ~Solution()
        {
            MyScenario.PropertyChanged -= MyScenario_PropertyChanged;
        }
        #endregion

        #region "Ballistic Routines"
        public void CalculateFo()
        {
            string lmsg;
            Atmospherics ltmpAt;

            ltmpAt = MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics;

            if (ZeroSpeedOfSound == 0)
            {
                lmsg = "Speed-of-Sound must be provided before the drag factor can be calculated.  Populate atmospheric data.";
                DragMessage = lmsg;
                return;
            }
            else if ((V1 == 0) || (D2 == 0))
            {
                lmsg = "An initial and second velocity and distance pair must be provided before the drag factor can be calculated." +
                    Environment.NewLine + "    Muzzle velocity and 0ft can be provied for the initial velocity/distance pair.";
                DragMessage = lmsg;
                return;
            }
            MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Fo =
                MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.CalculateFo(ZeroZone1TransSpeed, ZeroZone2TransSpeed,
                ZeroZone3TransSpeed);
        }
        public void CalculateF2()
        {
            string lmsg;
            if (ZeroSpeedOfSound == 0)
            {
                lmsg = "Speed-of-Sound must be provided before the drag factor can be calculated. Populate atmospheric data.";
                DragMessage  = lmsg;
                return;
            }
            else if (MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.BCz2 == 0)
            {
                lmsg = "Invalid Zone 2 Ballistic Coefficent.";
                DragMessage = lmsg;
                return;
            }
            MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.F2 = MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.CalculateF2(ZeroSpeedOfSound);
        }
        public void CalculateF3()
        {
            string lmsg;

            if (SpeedOfSound == 0)
            {
                lmsg = "Speed-of-Sound must be provided before the drag factor can be calculated.  Populate atmospheric data.";
                DragMessage = lmsg;
                return;
            }
            MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.F3 = MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.CalculateF3(ZeroSpeedOfSound);
        }
        public void CalculateF4()
        {
            string lmsg;
            if (SpeedOfSound == 0)
            {
                lmsg = "Speed-of-Sound must be provided before the drag factor can be calculated.  Populate atmospheric data.";
                DragMessage = lmsg;
                return;
            }
            MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.F4 = MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.CalculateF4(ZeroSpeedOfSound);
        }
        public void SolveZeroData()
        {
            if (UseMaxRise)
            {
                if (Fo == 0)
                {
                    ZeroMessage  = "Invalid Fo value.";
                    return;
                }
                else if (ZeroMaxRise == 0)
                {
                    ZeroMessage = "Invalid maximum rise value.";
                    return;
                }
                else if (MuzzleVelocity == 0)
                {
                    ZeroMessage = "Invalid muzzle velocity.";
                    return;
                }
                else if (ScopeHeight == 0)
                {
                    ZeroMessage = "Invalid scope height value.";
                }
                double lZd = BallisticFunctions.CalculateZeroRange(Fo, ZeroMuzzleVelocity, ZeroMaxRise, ScopeHeight);
                MyScenario.MyShooter.MyLoadOut.zeroData.ZeroRange = lZd;
            }
            else
            {
                if (ZeroRange <= 0)
                {

                    // A zero range must be provided.
                    ZeroMessage = "A zero range must be provided.";
                    return;
                }
                else if (ScopeHeight <= 0)
                {
                    // A scope height > 0 must be provided.
                    ZeroMessage = "A scope height > 0 must be provided.";
                    return;
                }
                else if (MuzzleVelocity <= 0)
                {
                    ZeroMessage = "Invalid muzzle velocity.";
                    return;
                }

                else
                {
                    MyScenario.MyShooter.MyLoadOut.zeroData.ZeroMaxRise = BallisticFunctions.CalculateHm(ZeroRange, ZeroRange, ScopeHeight, MuzzleVelocity,
                        Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope, Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor,
                        Zone2TransSpeed, Zone2TransSpeed, Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, ZeroTargetLoc, ZeroShooterLoc,
                       ZeroTargetLoc, ZeroShooterLoc);
                    //Reset all target solution data as this affects vertical drop.
                }
            }
            MyScenario.MyShooter.MyLoadOut.zeroData.NearZeroRange =
                BallisticFunctions.ZeroNearRange(ZeroMaxRise, ScopeHeight, MuzzleVelocity, Fo);
            MyScenario.MyShooter.MyLoadOut.zeroData.MidRange = BallisticFunctions.MidRange(MuzzleVelocity, ZeroMaxRise, ScopeHeight, Fo);
            MyScenario.MyShooter.MyLoadOut.zeroData.PointBlankRange = BallisticFunctions.PointBlankRange(ZeroMaxRise, MuzzleVelocity, ScopeHeight, Fo);
            RaisePropertyChanged(nameof(MidRange));
            RaisePropertyChanged(nameof(PointBlankRange));

        }
        public void CalculateMuzzleVelocity()
        {
            double lmv;
            Atmospherics ltmpAt;

            if (V1 == 0)
            {

                DragMessage = "Invalid V1 value.";
            }
            else if (Fo == 0)
            {
                DragMessage = "Invalid Fo value.";
                return;

            }
            else if (ZeroSpeedOfSound <= 0)
            {
                DragMessage = "Invalid atmospheric data.";
                return;
            }
            lmv = BallisticFunctions.MuzzleVelocity(V1, D1, Fo, Zone1Slope, ZeroZone1TransSpeed, ZeroZone2TransSpeed, Zone3Slope, ZeroZone3TransSpeed);
            MyScenario.MyShooter.MyLoadOut.zeroData.MuzzleVelocity = lmv;
            RaisePropertyChanged(nameof(ZeroMuzzleVelocity));
        }
        public void CalculateVelocityDistance()
        {
            double lV1;

            if (ZeroMuzzleVelocity == 0)
            {
                DragMessage = "Invalid muzzle velocity value.";
                return;
            }
            else if (BCg1 == 0)
            {
                DragMessage = "Invalid G1 ballistic coefficent value.";
                return;
            }
            MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.V1 = ZeroMuzzleVelocity;
            MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.D1 = 0;
            MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.D2 = 300;
            MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.V2 = BallisticFunctions.CalculateV2FromBC(ZeroMuzzleVelocity, BCg1, D2);
        }
        public void CheckMuzzleVelocity()
        {
            if (MuzzleVelocity == 0)
            {
                MyScenario.MyShooter.MyLoadOut.MuzzleVelocity = ZeroMuzzleVelocity;
                RaisePropertyChanged(nameof(MuzzleVelocity));
            }
        }
        #endregion

        #region "Solution Data Aliases"
        public bool UseMaxRise { get { return MyScenario.MyShooter.MyLoadOut.zeroData.UseMaxRise; } }
        public double BarrelTwist { get { return MyBarrel.Twist; } }
        public string BarrelTwistDirection { get { return MyBarrel.RiflingTwistDirection; } }
        public double BaroPressure { get { return MyAtmospherics.Pressure; } }
        public double BCg1 { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.BCg1; } }
        public double BSG { get { return MyScenario.MyShooter.MyLoadOut.BSG; } }
        public double BulletDiameter { get { return MyBullet.Diameter; } }
        public double BulletWeight { get { return MyBullet.Weight; } }
        public double DensityAlt { get { return MyScenario.MyAtmospherics.DensityAlt; } }
        public double DensityAltAtZero { get { return MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics.DensityAlt; } }
        public double EffectiveWindDirection { get { return LocationData.GetEffectiveWindDirection(TargetLoc, ShooterLoc, WindDirection); } }
        public double D1 { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.D1; } }
        public double D2 { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.D2; } }
        public double V1 { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.V1; } }
        public double V2 { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.V2; } }
        public double Fo { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Fo; } }
        public double F2 { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.F2; } }
        public double F3 { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.F3; } }
        public double F4 { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.F4; } }
        public double MaxRange { get { return MyScenario.MyShooter.MyLoadOut.MaxRange; } }
        public double MidRange { get { return MyScenario.MyShooter.MyLoadOut.zeroData.MidRange; } }
        public double MuzzleVelocity { get { return MyScenario.MyShooter.MyLoadOut.MuzzleVelocity; } }
        public double ZeroMuzzleVelocity { get { return MyScenario.MyShooter.MyLoadOut.zeroData.MuzzleVelocity; } }
        public Atmospherics MyAtmospherics { get { return MyScenario.MyAtmospherics; } }  // set { MyScenario.MyAtmospherics = value; RaisePropertyChanged(nameof(MyAtmospherics)); } }
        public Barrel MyBarrel { get { return MyScenario.MyShooter.MyLoadOut.SelectedBarrel; } }
        public ObservableCollection<Recipe> MyCartridges
        {
            get
            {
                if (MyBarrel != null)
                {
                    return LawlerBallisticsFactory.BarrelRecipes(MyBarrel.ID, true);
                }
                else
                {
                    ObservableCollection<Recipe> lempty = new ObservableCollection<Recipe>();
                    return lempty;
                }
            }
        }
        public Bullet MyBullet { get { return MyScenario.MyShooter.MyLoadOut.SelectedCartridge.RecpBullet; } }
        public double PointBlankRange { get { return MyScenario.MyShooter.MyLoadOut.zeroData.PointBlankRange; } }
        public double ScopeHeight { get { return MyScenario.MyShooter.MyLoadOut.ScopeHeight; } }
        public double SpeedOfSound { get { return MyScenario.MyAtmospherics.SpeedOfSound; } }
        public double ZeroSpeedOfSound { get { return MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics.SpeedOfSound; } }
        public double TempF { get { return MyScenario.MyAtmospherics.Temp; } }
        public double WindDirection { get { return MyScenario.MyAtmospherics.WindDirection; } }
        public double WindSpeed { get { return MyScenario.MyAtmospherics.WindSpeed; } }
        public double ZeroWindSpeed { get { return MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics.WindSpeed; } }
        public double ZeroEffectiveWindDirection { get { return MyScenario.MyShooter.MyLoadOut.zeroData.WindEffectiveDirection; } }
        public double Zone1Range { get { return ZoneRange(Zone.Zone1, MuzzleVelocity, SpeedOfSound); } }
        public double ZeroZone1Range { get { return ZoneRange(Zone.Zone1, ZeroMuzzleVelocity, ZeroSpeedOfSound); } }
        public double Zone1AngleFactor { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone1AngleFactor; } }
        public double Zone1Slope { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone1Slope; } }
        public double Zone1SlopeMultiplier { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone1SlopeMultiplier; } }
        public double Zone1TransSpeed { get { return ZoneTransSpeed(Zone.Zone1, MyAtmospherics.SpeedOfSound); } }
        public double ZeroZone1TransSpeed { get { return ZoneTransSpeed(Zone.Zone1, ZeroSpeedOfSound); } }
        public double Zone2Slope { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone2Slope; } }
        public double Zone2Range { get { return ZoneRange(Zone.Zone2, MuzzleVelocity, SpeedOfSound); } }
        public double ZeroZone2Range { get { return ZoneRange(Zone.Zone2, ZeroMuzzleVelocity, ZeroSpeedOfSound); } }
        public double Zone2TransSpeed { get { return ZoneTransSpeed(Zone.Zone2, MyAtmospherics.SpeedOfSound); } }
        public double ZeroZone2TransSpeed { get { return ZoneTransSpeed(Zone.Zone2, ZeroSpeedOfSound); } }
        public double Zone3Range { get { return ZoneRange(Zone.Zone3, MuzzleVelocity, SpeedOfSound); } }
        public double ZeroZone3Range { get { return ZoneRange(Zone.Zone3, ZeroMuzzleVelocity, ZeroSpeedOfSound); } }
        public double Zone3Slope { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone3Slope; } }
        public double Zone3SlopeMultiplier { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone3SlopeMultiplier; } }
        public double Zone3TransSpeed { get { return ZoneTransSpeed(Zone.Zone3, MyAtmospherics.SpeedOfSound); } }
        public double Zone4Slope { get { return MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone4Slope; } }
        public double ZeroZone3TransSpeed { get { return ZoneTransSpeed(Zone.Zone3, ZeroSpeedOfSound); } }
        public double ZeroMaxRise { get { return MyScenario.MyShooter.MyLoadOut.zeroData.ZeroMaxRise; } }
        public LocationData ZeroTargetLoc { get { return MyScenario.MyShooter.MyLoadOut.zeroData.TargetLoc; } }
        public LocationData ZeroShooterLoc { get { return MyScenario.MyShooter.MyLoadOut.zeroData.ShooterLoc; } }
        public LocationData TargetLoc
        {
            get
            {
                return MyScenario.SelectedTarget.TargetLocation;
            }
        }
        public LocationData ShooterLoc { get { return MyScenario.MyShooter.MyLocation; } }
        public double ZeroRange { get { return MyScenario.MyShooter.MyLoadOut.zeroData.ZeroRange; } }
        #endregion

        #region "Solution Routine Aliases"
        public double ZoneRange(Zone TargetZone, double MuzzleVelocity, double SpeedOfSound)
        {
            double lRTN = 0;

            switch (TargetZone)
            {
                case Zone.Zone1:
                    lRTN = MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone1Range(MuzzleVelocity, SpeedOfSound);
                    break;
                case Zone.Zone2:
                    lRTN = MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone2Range(MuzzleVelocity, SpeedOfSound);
                    break;
                case Zone.Zone3:
                    lRTN = MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone3Range(MuzzleVelocity, SpeedOfSound);
                    break;
                case Zone.Zone4:
                    lRTN = MaxRange;
                    break;
            }

            return lRTN;
        }
        public double ZoneTransSpeed(Zone TargetZone, double SpeedOfSound)
        {
            double lRTN = 0;

            switch (TargetZone)
            {
                case Zone.Zone1:
                    lRTN = MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone1TransSpeed(SpeedOfSound);
                    break;
                case Zone.Zone2:
                    lRTN = MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone2TransSpeed(SpeedOfSound);
                    break;
                case Zone.Zone3:
                    lRTN = MyScenario.MyShooter.MyLoadOut.MyDragSlopeData.Zone3TransSpeed(SpeedOfSound);
                    break;
                case Zone.Zone4:
                    lRTN = 0;
                    break;
            }

            return lRTN;
        }
        public double FlightTime(double Range, bool ZeroData)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.FlightTime(Range, Fo, F2, F3, F4, MuzzleVelocity, Zone1Range, Zone1TransSpeed,
                    Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier,
                    Zone3SlopeMultiplier, Zone4Slope);
            }
            else
            {
                lRTN = BallisticFunctions.FlightTime(Range, Fo, F2, F3, F4, ZeroMuzzleVelocity, ZeroZone1Range, ZeroZone1TransSpeed,
                    Zone1Slope, ZeroZone2Range, ZeroZone2TransSpeed, Zone2Slope, ZeroZone3Range, ZeroZone3TransSpeed, Zone3Slope,
                    Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }
            return lRTN;
        }
        public double GyroscopicStability(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.GyroscopicStability(MyBullet, MyBarrel, Velocity(Range), TempF, BaroPressure);
            }
            else
            {
                lRTN = BallisticFunctions.GyroscopicStability(MyBullet, MyBarrel, Velocity(Range, ZeroData), TempF, BaroPressure);
            }

            return lRTN;
        }
        public double SightDelta(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            //Coriolis vertical component is calculated in so do not add.

            CheckMuzzleVelocity();
            if (!ZeroData)
            {
                lRTN = BallisticFunctions.SightDelta(Range, ZeroRange, ScopeHeight, ZeroMuzzleVelocity, MuzzleVelocity, Zone1Range, Zone2Range, Zone3Range,
                    Zone1Slope, Zone2Slope, Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed,
                    Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, ZeroTargetLoc, ZeroShooterLoc, TargetLoc, ShooterLoc);
            }
            else if (ZeroData)
            {
                lRTN = BallisticFunctions.SightDelta(Range, ZeroRange, ScopeHeight, ZeroMuzzleVelocity, ZeroMuzzleVelocity, ZeroZone1Range, ZeroZone2Range, ZeroZone3Range,
                    Zone1Slope, Zone2Slope, Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, ZeroZone1TransSpeed, ZeroZone2TransSpeed,
                    ZeroZone3TransSpeed, Fo, F2, F3, F4, DensityAltAtZero, DensityAltAtZero, ZeroTargetLoc, ZeroShooterLoc, ZeroTargetLoc, ZeroShooterLoc);
            }
            return lRTN;
        }
        public double GetCoriolisHoriz(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.GetCoriolisHoriz(Range, MuzzleVelocity, TargetLoc, ShooterLoc, Fo, F2,F3,F4,Zone1Range,
                    Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope,
                    Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }
            else
            {
                lRTN = BallisticFunctions.GetCoriolisHoriz(Range, ZeroMuzzleVelocity, ZeroTargetLoc, ZeroShooterLoc, Fo, F2,F3,
                    F4,Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope,
                    Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }

            return lRTN;
        }
        public double GetCoriolisVert(double Range, bool ZeroData = false, bool SightDelta = false)
        {
            double lRTN = 0;
            double lZD = 0;

            CheckMuzzleVelocity();

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.GetCoriolisVert(Range, TargetLoc, ShooterLoc, Fo, MuzzleVelocity, F2, F3,F4,Zone1Range,
                    Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope,
                    Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }
            else
            {
                lRTN = BallisticFunctions.GetCoriolisVert(Range, ZeroTargetLoc, ZeroShooterLoc, Fo, ZeroMuzzleVelocity, F2,
                    F3,F4, Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed,
                    Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }
            if (SightDelta)
            {
                //Get coriolis devation at zero range then get the compensation per yard.  Next multiply by the requested range
                //  and subtract the compenstion from the raw at the requested range.  Raw can be different due to shot direction
                //  and shot atmospherics.
                lZD = BallisticFunctions.GetCoriolisVert(ZeroRange, ZeroTargetLoc, ZeroShooterLoc, Fo, ZeroMuzzleVelocity,
                    F2, F3,F4, Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range,
                    Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
                lZD = lZD / ZeroRange;
                lRTN = lRTN - (lZD * Range);
            }

            return lRTN;
        }
        public double GetSpinDrift(double Range, bool ZeroData = true)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.GetSpinDrift(Range, BarrelTwistDirection, BSG, Fo, MuzzleVelocity,
                    F2, F3,F4,Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range,
                    Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }
            else
            {
                lRTN = BallisticFunctions.GetSpinDrift(Range, BarrelTwistDirection, BSG, Fo, ZeroMuzzleVelocity,
                    F2,F3,F4,ZeroZone1Range, ZeroZone1TransSpeed, Zone1Slope, ZeroZone2Range, ZeroZone2TransSpeed,
                    Zone2Slope, ZeroZone3Range, ZeroZone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier,
                    Zone4Slope);
            }

            return lRTN;
        }
        public double GetHorizErr(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            lRTN = ZeroTotalHorizontalComp(Range) + TotalHorizontalDrift(Range, ZeroData);

            return lRTN;
        }
        public double SpinRate(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.SpinRate(Range, MuzzleVelocity, BarrelTwist, BulletDiameter, Fo, F2, F3,F4,
                    Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range,
                    Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }
            else
            {
                lRTN = BallisticFunctions.SpinRate(Range, ZeroMuzzleVelocity, BarrelTwist, BulletDiameter, Fo, F2,F3,F4,
                    ZeroZone1Range, ZeroZone1TransSpeed, Zone1Slope, ZeroZone2Range, ZeroZone2TransSpeed, Zone2Slope,
                    ZeroZone3Range, ZeroZone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }

            return lRTN;
        }
        public double TotalHorizontalDrift(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.TotalHorizontalDrift(Range, WindSpeed,
                    LocationData.GetEffectiveWindDirection(TargetLoc, ShooterLoc, WindDirection),
                    MuzzleVelocity, Fo, TargetLoc, ShooterLoc, BarrelTwistDirection, BSG, F2,F3,F4,
                    Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope,
                    Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }
            else
            {
                lRTN = BallisticFunctions.TotalHorizontalDrift(Range, ZeroWindSpeed, ZeroEffectiveWindDirection,
                    ZeroMuzzleVelocity, Fo, ZeroTargetLoc, ZeroShooterLoc, BarrelTwistDirection, BSG, F2, F3, F4,
                    ZeroZone1Range, ZeroZone1TransSpeed, Zone1Slope, ZeroZone2Range, ZeroZone2TransSpeed, Zone2Slope,
                    ZeroZone3Range, ZeroZone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }

            return lRTN;
        }
        public double ZeroTotalHorizontalComp(double Range)
        {
            double lRTN = 0;

            lRTN = ZeroSpinDriftComp(Range);
            lRTN = lRTN + ZeroWindComp(Range);
            lRTN = lRTN + ZeroCoriolisHorizComp(Range);

            return lRTN;
        }
        public double Velocity(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.Velocity(MuzzleVelocity, Range, Zone1Range, Zone1TransSpeed, Fo, Zone1Slope, 
                    Zone1SlopeMultiplier, Zone2Range, Zone2TransSpeed, F2, Zone2Slope, Zone3Range, Zone3Slope, Zone3TransSpeed,
                    Zone3SlopeMultiplier, F3, F4, Zone4Slope);
            }
            else
            {
                lRTN = BallisticFunctions.Velocity(ZeroMuzzleVelocity, Range, ZeroZone1Range, ZeroZone1TransSpeed, Fo, 
                    Zone1SlopeMultiplier, Zone1Slope, ZeroZone2Range, ZeroZone2TransSpeed, F2, Zone2Slope, ZeroZone3Range, 
                    Zone3Slope, ZeroZone3TransSpeed, Zone3SlopeMultiplier, F3, F4, Zone4Slope);
            }
            return lRTN;
        }
        public double WindDrift(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.WindDrift(WindSpeed, EffectiveWindDirection, Range, Fo, MuzzleVelocity, F2,F3,F4,
                    Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, 
                    Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }
            else
            {
                lRTN = BallisticFunctions.WindDrift(ZeroWindSpeed, ZeroEffectiveWindDirection, Range, Fo, ZeroMuzzleVelocity, F2,
                    F3,F4, ZeroZone1Range, ZeroZone1TransSpeed, Zone1Slope, ZeroZone2Range, ZeroZone2TransSpeed, Zone2Slope,
                    ZeroZone3Range, ZeroZone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            }

            return lRTN;
        }
        public double ZeroWindComp(double Range)
        {
            double lRTN = 0;
            double lZD = 0;

            lZD = BallisticFunctions.WindDrift(ZeroWindSpeed, ZeroEffectiveWindDirection, ZeroRange, Fo, ZeroMuzzleVelocity, F2,
                F3,F4, ZeroZone1Range, ZeroZone1TransSpeed, Zone1Slope, ZeroZone2Range, ZeroZone2TransSpeed, Zone2Slope, ZeroZone3Range,
                ZeroZone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            lZD = 0 - (lZD / ZeroRange);
            lRTN = lZD * Range;

            return lRTN;
        }
        public double ZeroCoriolisHorizComp(double Range)
        {
            double lRTN = 0;
            double lZCHD = 0;

            lZCHD = BallisticFunctions.GetCoriolisHoriz(ZeroRange, ZeroMuzzleVelocity, ZeroTargetLoc, ZeroShooterLoc, Fo, F2,F3,F4,
                Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope,
                Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            lZCHD = lZCHD / ZeroRange;
            //Zero sight adjustment is opposite of coriolis effect.
            lRTN = 0 - lZCHD;

            return lRTN;
        }
        public double ZeroSpinDriftComp(double Range)
        {
            double lRTN = 0;
            double lZD = 0;

            lZD = BallisticFunctions.GetSpinDrift(ZeroRange, BarrelTwistDirection, BSG, Fo, ZeroMuzzleVelocity, F2, F3, F4,
                Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed,
                Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            lZD = 0 - (lZD / ZeroRange);
            lRTN = lZD * Range;

            return lRTN;
        }
        public double MuzzleDrop(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.MuzzleDrop(MuzzleVelocity, Range, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope,
                    Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed,
                    Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, TargetLoc, ShooterLoc);
            }
            else if (ZeroData)
            {
                lRTN = BallisticFunctions.MuzzleDrop(ZeroMuzzleVelocity, Range, ZeroZone1Range, ZeroZone2Range, ZeroZone3Range, Zone1Slope, Zone2Slope,
                    Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, ZeroZone1TransSpeed, ZeroZone2TransSpeed, 
                    ZeroZone3TransSpeed, Fo, F2, F3, F4, DensityAltAtZero, DensityAltAtZero, ZeroTargetLoc, ZeroShooterLoc);
            }
            return lRTN;
        }
        public double Energy(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.Energy(BulletWeight, Velocity(Range));
            }
            else
            {
                lRTN = BallisticFunctions.Energy(BulletWeight, Velocity(Range, ZeroData));
            }

            return lRTN;
        }
        public double Fdrag(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.FdragCoefficient(Range, Fo, F2, F3, F4, Zone1Range, Zone2Range, Zone3Range,
                    Zone1Slope, Zone2Slope, Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier);
            }
            else
            {
                lRTN = BallisticFunctions.FdragCoefficient(Range, Fo, F2, F3, F4, ZeroZone1Range, ZeroZone2Range, 
                    ZeroZone3Range, Zone1Slope, Zone2Slope, Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, 
                    Zone3SlopeMultiplier);
            }

            return lRTN;
        }
        public double CDdragCoefficient(double Range, bool ZeroData = false)
        {
            double lRTN = 0;

            if (!ZeroData)
            {
                lRTN = BallisticFunctions.CDdragCoefficient(MuzzleVelocity, Range, Fo, F2, F3, F4, Zone1Range, Zone2Range, Zone3Range, Zone1Slope,
                    Zone2Slope, Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1TransSpeed, Zone2TransSpeed, Zone3TransSpeed, BulletDiameter,
                    BulletWeight, MyAtmospherics.AirDensity * _kgm3Tolbft3);
            }
            else
            {
                lRTN = BallisticFunctions.CDdragCoefficient(ZeroMuzzleVelocity, Range, Fo, F2, F3, F4, ZeroZone1Range, ZeroZone2Range, 
                    ZeroZone3Range, Zone1Slope, Zone2Slope, Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, ZeroZone1TransSpeed, ZeroZone2TransSpeed, ZeroZone3TransSpeed, 
                    BulletDiameter, BulletWeight, MyScenario.MyShooter.MyLoadOut.zeroData.atmospherics.AirDensity * _kgm3Tolbft3);
            }

            return lRTN;
        }
        #endregion

    }
}
