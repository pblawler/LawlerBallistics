using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net;
using System.Xml;
using LawlerBallisticsDesk.Classes.BallisticClasses;

namespace LawlerBallisticsDesk.Classes
{

    //TODO: add error handling throughout application.

    //TODO: Add a zeroing function to establish all rates i.e. drift rate, coriolis correction rate, temp, density
    //  alt, etc....  must make zero and shot conditions the same.  There should not be any atmosphere correction or coriolis
    // etc at sight-in/zero because it is the baseline and every shot is under zero conditions.  May need to restructure
    // some routines to recieve more parameters rather than default to shot conditions so the same zero conditions can go in
    // twice.
    

    public class Ballistics : INotifyPropertyChanged
    {
        #region "Background"
        //___________________________________________________________
        //___________________________________________________________
        //___________________________________________________________

        //TODO:
        //           Incorporate custom zone slopes
        //           Add drop correction for zones, correct F and N ???
        //           Add F calculation for zones
        //           Solve zone issues with Velocity and Flight time

        //___________________________________________________________
        //___________________________________________________________
        //___________________________________________________________


        // N = 1/2 for V > ~1400 fps Zone 1
        // N = 0 for ~1200 < V< ~1400 fps Zone 2
        // N = -4 for ~1000 < V< ~1200 fps Zone 3
        // N = 0 for V< ~1000 fps Zone 4

        //________________ Zone 1  __________________________

        //  N = 1 / 2 for V > Zone1TransSpeed fps
        //   c = 166
        //   R = Fo/3N(1 - (V/Vo)^N)
        //   V = Vo(1 - 3RN/Fo)^(1/N)
        //   D = G / Vo / ((1 / R) - (1 / Fa)) ^ 2

        //________________ END of Zone 1  ____________________

        //________________  Zone 2  __________________________

        //  N = 0 for Zone2TransSpeed<V<Zone1TransSpeed fps

        //  c = 166

        //  R = F/(3ln(Vo/V))
        //   V = Vo(exp(-3R/F)
        //   D = G / Vo / ((1 / R) - (1 / Fa)) ^ 2
        //   F2 = (cV^0.5)*BC

        //________________  END of Zone 2  ____________________

        //________________  Zone 3  __________________________

        //   N = -4 for Zone3TransSpeed<V<Zone2TransSpeed fps
        //   R = (F2-F3)/3N
        //   V = Zone2TransSpeed / (1 - 3RN/F2)^(-1/N)
        //   F3 = c* Zone3TransSpeed^0.5
        //   Fa = F2 + (F3 - F2) / 4
        //   D = G / Vo / ((1 / R) - (1 / Fa)) ^ 2

        //________________ END of Zone 3  ____________________

        //________________  Zone 4  __________________________

        //  N = 0 for V<Zone3TransSpeed fps

        //  R = Fo/3N(1 - (V/Vo)^N)
        //   V = Vo(1 - 3RN/Fo)^(1/N)

        //________________ END of Zone 4  ____________________
        #endregion

        #region "Weather Query"
        private const string _GetWeather = "http://api.openweathermap.org/data/2.5/weather?lat=@latitude@&lon=@longitude@&mode=xml&units=imperial&APPID=";
        #endregion

        #region "Binding"
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region "Private Variables"       
        private BulletShapeEnum _BulletShapeType;
        private double _BarrelTwist;
        private string _BarrelTwistDir = "R";
        private Barrel _SelectedBarrel;
        private double _BCg1;
        private double _BCz2;
        private Bullet _SelectedBullet;
        private double _BSG; //Bullet Stability factor
        private double _BulletDiameter;
        private double _BulletLength;
        private double _BulletWeight;
        private double _D1;
        private double _D2;
        private double _DensityAlt;
        private double _zDensityAlt;
        private double _Fo;
        private double _F2;
        private double _F3;
        private double _F4;
        private const double _GravFt = 32.17405;
        private const double _GravIn = 386.0886;
        private const double _G = 41.68211487;      //G = 3*(g(in/s^2)/2)^0.5 = 41.68
        private double _Hm;
        private double _zShooterLatitude;
        private double _zShooterLongitude;
        private double _zTargetLatitude;
        private double _zTargetLongitude;
        private double _ShooterLatitude;
        private double _ShooterLongitude;
        private double _TargetLatitude;
        private double _TargetLongitude;
        private double _ShotDistance;
        private double _ShotAngle;
        private double _MuzzleVelocity;
        private double _RelHumidity;
        private double _zRelHumidity;
        private double _ScopeHeight;
        private double _TempF;
        private double _zTempF;
        private double _BaroPressure;
        private double _zBaroPressure;
        private bool _UseMaxRise;
        private double _V1;
        private double _V2;
        private double _ZeroRange;
        private double _Zone1Range;
        private double _Zone2Range;
        private double _Zone3Range;
        private double _Zone1TransSpeed;
        private double _Zone2TransSpeed;
        private double _Zone3TransSpeed;
        private double _Zone1MachFactor;
        private double _Zone2MachFactor;
        private double _Zone3MachFactor;
        private double _Zone1SlopeMultiplier;
        private double _Zone3SlopeMultiplier;
        private double _Zone1Slope;
        private double _Zone1AngleFactor;
        private double _Zone3Slope;
        private double _WindSpeed;
        private float _WindDirectionDeg;
        private double _WindDirectionClock;
        private bool _SpinDriftCalc;
        private bool _FoCalc;
        #endregion

        #region "Public Properties"
        /// <summary>
        /// Used to estimate a bullet's BC and for initial Zone scale multiplier value selection.
        /// </summary>
        public BulletShapeEnum BulletShapeType { get { return _BulletShapeType; } }
        public Barrel SelectedBarrel { get { return _SelectedBarrel; } }
        /// <summary>
        /// Number of inches of barrel length per rev of the rifling.
        /// </summary>
        public double BarrelTwist
        {
            get
            {
                if (_BarrelTwist ==0) _BarrelTwist = 9;
                return _BarrelTwist; 
            }
            set
            {
                _BarrelTwist = value;
                RaisePropertyChanged(nameof(BarrelTwist));
                RaisePropertyChanged(nameof(BSG));
            }
        }
        /// <summary>
        /// Direction of rifling, R or L.
        /// </summary>
        public string BarrelTwistDir
        {
            get
            {
                return _BarrelTwistDir;
            }
            set
            {
                _BarrelTwistDir = value;
                RaisePropertyChanged(nameof(BarrelTwistDir));
            }
        }
        public Bullet SelectedBullet { get { return _SelectedBullet; } }
        /// <summary>
        /// Diameter (in) of the bullet
        /// </summary>
        public double BulletDiameter { get { return _BulletDiameter; }
            set
            {
                _BulletDiameter = value;
                RaisePropertyChanged(nameof(BulletDiameter));
                RaisePropertyChanged(nameof(BSG));
            }
        }
        /// <summary>
        /// Length of the bullet (in), base to tip
        /// </summary>
        public double BulletLength
        {
            get { return _BulletLength; }
            set
            {
                _BulletLength = value;
                RaisePropertyChanged(nameof(BulletLength));
                RaisePropertyChanged(nameof(BSG));
            }
        }
        /// <summary>
        /// Weight of the bullet (grn)
        /// </summary>
        public double BulletWeight
        {
            get { return _BulletWeight; }
            set
            {
                _BulletWeight = value;
                RaisePropertyChanged(nameof(BulletWeight));
                RaisePropertyChanged(nameof(BSG));
            }
        }
        /// <summary>
        /// Bullet stability factor
        /// </summary>
        public double BSG
        {
            get
            {
                             
                _BSG = BallisticFunctions.GyroscopicStability(SelectedBullet, SelectedBarrel, MuzzleVelocity,TempF,BaroPressure);
                return _BSG;
            }
        }
        /// <summary>
        /// Rpm of the bullet at the muzzle.
        /// </summary>
        public double BulletRPM
        {
            get
            {
                double lBRPM = 0;

                lBRPM = (12 / BarrelTwist) * MuzzleVelocity * 60;

                return lBRPM;
            }
        }
        /// <summary>
        /// The spin drift correction rate. When zeroing it is automatically accounted for and continues in a
        /// straight line past the zero range. Since the bullet is slowing the rate will be over come past the
        /// zero range but this must be accounted for or the reported drift will be too much.
        /// </summary>
        public double SpindDriftCorrection
        {
            get
            {
                double lZSD;
                double lZDC;

                //Amount of drift corrected at sight in
                lZSD = GetRawSpinDrift(ZeroRange);
                //Correction factor (in/yards), the linear offset induced at sight in.
                lZDC = lZSD / ZeroRange;
                return lZDC;
            }
        }
        public double WindSpeed
        {
            get
            {
                return _WindSpeed;
            }
            set
            {
                _WindSpeed = value;
                RaisePropertyChanged(nameof(WindSpeed));
            }
        }
        public float WindDirectionDeg
        {
            get
            {
                return _WindDirectionDeg;
            }
            set
            {
                _WindDirectionDeg = value;
                RaisePropertyChanged(nameof(WindDirectionDeg));
            }
        }
        public double WindDirectionClock
        {
            get
            {
                return _WindDirectionClock;
            }
            set
            {
                _WindDirectionClock = value;
                RaisePropertyChanged(nameof(WindDirectionClock));
            }
        }
        /// <summary>
        /// The design geometry type of the bullet, i.e. SpitzerBoatTail, Semispitzer, etc...
        /// </summary>
        public BulletShapeEnum BulletShapeTyp { get { return _BulletShapeType; } set { _BulletShapeType = value; RaisePropertyChanged(nameof(BulletShapeTyp)); } }
        #endregion

        #region "Public Routines"
        /// <summary>
        /// The PreflightCheck returns an integer pertaining to the shot characteristics that can be
        /// calculated with the provided data.
        /// </summary>
        /// <returns></returns>
        public Int16 PreflightCheck()
        {
            Int16 lRtn = 0;
            int lFR; // Function return

            //GetWeather();

            // F0 Provided = 1
            // Velocity Measurements Provided = 2
            // BC and Muzzle Provided = 4
            //Check to see of Fo is directly provided or must be calculated.
            if ((_Fo > 0) & (_MuzzleVelocity > 0))
            {
                //Fo is provided.  Set First bit value to 1

                //TODO: Calculate V1, D1, V2, D2 so BC and other calculations can be solved.

                _FoCalc = true;
                lRtn = 1;
            }
            //Check to see if velocity-range or BC-MV method is used to find Fo.
            if ((_V1 > 0) & (_V2 > 0) & (_D2 > 0))
            {
                //Actual velocities and distances provided.
                lRtn += 2;
                _FoCalc = true;
            }
            else if ((_MuzzleVelocity > 0) & (_BCg1 > 0))
            {
                //BC and Muzzle velocity rather than Velocity-Range is used to calculate Fo
                lRtn += 4;
                _FoCalc = true;
            }
            if (lRtn < 1)
            {
                //Insufficent data to produce a solution.  Must have Velocity-Range or BC-MV.
                _FoCalc = false;
                lRtn = -1;
                return lRtn;
            }
            if (lRtn == 4)
            {
                lFR = CalculateV2FromBC();
                if (lFR != 0)
                {
                    lRtn = -2;
                    return lRtn;
                }
            }
            Fo = CalculateFo(D1, V1, D2, V2);
            BCg1 = CalculateBC(V1, V2, (D2 - D1));
            if (Fo <= 0)
            {
                //Invalid Fo
                lRtn = -3;
                return lRtn;
            }
            if (UseMaxRise & (ZeroMaxRise == 0))
            {
                //Cannot calculate sightline without a valid zero point.
                lRtn = -8;
                return lRtn;
            }
            else if ((UseMaxRise) & (ZeroMaxRise > 0))
            {
                ZeroRange = CalculateZeroRange(_Hm);
            }
            if ((!UseMaxRise) & (ZeroRange > 0))
            {
                CalculateHm();
            }
            
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //All required data is present to calculate raw muzzle drop just counting drag and gravity.
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            if (BSG != 0)
            {
                //Spin Drify calculation enabled.
                lRtn += 8;
            }


            return lRtn;
        }
        /// <summary>
        /// The range to target corrected for incline.  Line-of-sight distance will be greater than the distance affected
        /// by gravity.  The distance to shoot for is the range affected by gravity and drag which is returned by this function.
        /// </summary>
        /// <param name="ShotAngle">The angle absolute value of the shot angle.</param>
        /// <param name="LOSrange">The line-of-sight (LOS) range.</param>
        /// <returns></returns>
        public double AngleCompRange(double ShotAngle, double LOSrange)
        {
            double lTDA;    //True drop angle
            double lTDD;    //True drop distance
            double lLOSD;   //Line of sight drop distance
            double lPi;     //Value of Pi
            double lDIR;    //Drop iteration range
            double lDID;    //Drop iteration drop

            if (ShotAngle == 0)
            {
                return LOSrange;
            }
            lPi = Math.PI; //3.141592654
            lLOSD = MuzzleDrop(LOSrange);
            lTDA = 90 - ShotAngle;
            lTDD = Math.Abs(Math.Sin(lTDA * (lPi / 180)) * lLOSD);
            lDID = Math.Abs(lLOSD);
            lDIR = LOSrange - 0.25;
            //Iterate drop function to find the range that equals the corrected drop distance.
            while (lDID > lTDD)
            {
                lDID = Math.Abs(MuzzleDrop(lDIR));
                lDIR = lDIR - 0.5;
            }
            return lDIR;
        }
        /// <summary>
        /// The RPM of the bullet
        /// </summary>
        /// <param name="Range">Distance to return rpm at in yards.</param>
        /// <returns>Bullet RPM</returns>
        public double SpinRate(double Range)
        {
            double lSR = 0; //Spin Rate
            double lFT;     //Flight time

            //MV x (12/twist rate in inches) x 60 = Bullet RPM
            lSR = MuzzleVelocity * (12.00 / BarrelTwist) * 60;
            lFT = FlightTime(Range);

            //An approximating formula Geoffery Kolbe published for spin decay is:

            //Where:
            //Nm = the spin rate the bullet had at the muzzle
            //N = the spin rate of the bullet after your time of flight to the range of interest
            //t = time of flight in seconds from the muzzle to the current bullet position
            //d = bullet diameter in inches
            //e = natural logarithm base, 2.71828…
            //N = Nm × (-0.035 × t / d) ^e
            //There's other on line references to Kolbe's formula saying it's:
            //N = Nm exp(-0.035 t / d)
            lSR = lSR * Math.Exp((-0.035 * lFT / BulletDiameter));

            return lSR;
        }
        /// <summary>
        /// Calculates the drag adjustment for the change in air density.
        /// </summary>
        /// <param name="RetardCoef">The drag coefficent to adjust.</param>
        /// <returns>Corrected drag/retard coefficent.</returns>
        private double DensityAltCorrection(double RetardCoef)
        {
            double lFp;

            lFp = RetardCoef + RetardCoef * (DensityAlt - zDensityAlt) / 29733;
            return lFp;
        }
        /// <summary>
        /// The bullet drop from the muzzle of the barrel.
        /// </summary>
        /// <param name="Range">The range to calculate the muzzle drop for.</param>
        /// <returns>Muzzle drop distance in inches.</returns>
        public double MuzzleDrop(double Range, bool RawMagnatude = false)
        {
            double ld = 0;        //Temporary drop accumulation variable.
            double lD1 = 0;     //Component 1 of 2 drop distance in zone 1
            double lD1p = 0;    //Component 2 of 2 drop distance in zone 1 (Accounts for downward angle of bullet as a result of zone 1 arc for remaining distance.)
            double lD1t = 0;
            double lD2 = 0;     //Component 1 of 2 drop distance in zone 2
            double lD2p = 0;    //Component 2 of 2 drop distance in zone 2 (Accounts for downward angle of bullet as a result of zone 2 arc for remaining distance.)
            double lD3 = 0;     //Component 1 of 2 drop distance in zone 3
            double lD3p = 0;    //Component 2 of 2 drop distance in zone 3 (Accounts for downward angle of bullet as a result of zone 3 arc for remaining distance.)
            double lD4;     //Component 1 of 1 drop distance in zone 4
            double lFa;     //Drag/Retard coefficent for a specific range distance.

            if (Range <= Zone1Range)
            {
                //Zone 1

                //    'D = (G/Vo/(1/R - 1/Fa))^2
                lD1 = Math.Pow(((_G / MuzzleVelocity) / ((1 / Range) - (1 / DensityAltCorrection(Fa(Range))))), 2);
                ld = lD1;
            }
            else if ((Range > Zone1Range) & (Range <= Zone2Range))
            {
                //Zone 2

                if (Zone1Range > 0)
                {
                    //D = (G/Vo/(1/R - 1/Fa))^2
                    lD1 = MuzzleDrop(Zone1Range, true);
                    //D' = 3gFo(Rt - R)((1 - 3NR / Fo) ^ (1 - 2 / N))/ Vo ^ 2 / (2 - N)
                    //   Corrected to 3gFo(Rt-R)((1+3NR/Fo)^((2/N)-1))/Vo^2/(2-N)
                    //lD1p = lD1p * (Math.Pow(((1 + (3 * Zone1Slope * Zone1Range) / DensityAltCorrection(Fo))), ((2 / Zone1Slope) - 1)));
                    //   Updated to original formula with correction factor 0.696.  First mod moved incorrectly with F.
                    lD1p = 3 * _GravIn * DensityAltCorrection(Fo) * (Range - Zone1Range);
                    lD1p = lD1p * (Math.Pow((1 - ((3 * Zone1Slope * Zone1Range) / DensityAltCorrection(Fo))), (1 - (2 / Zone1Slope))));
                    lD1p = lD1p / (Math.Pow(MuzzleVelocity, 2));
                    lD1p = lD1p / (2 - Zone1Slope);
                    lD1p = lD1p * Zone1AngleFactor;
                }
                //    'D = (G/Vo/(1/R - 1/Fa))^2
                lD2 = Math.Pow(((_G / Zone1TransSpeed) / ((1 / (Range - Zone1Range)) - (1 / DensityAltCorrection(F2)))), 2);
                ld = lD1 + lD1p + lD2;
            }
            else if ((Range > Zone2Range) & (Range <= Zone3Range))
            {
                //Zone 3

                if (Zone2Range > 0)
                {
                    lD1t = MuzzleDrop(Zone2Range, true);
                    //D = (G/Vo/(1/R - 1/Fa))^2
                    lD2 = Math.Pow(((_G / Zone1TransSpeed) / ((1 / (Zone2Range - Zone1Range)) - (1 / DensityAltCorrection(F2)))), 2);
                    //D2' = 2D2(Rt - R1 - R2) / R2 / (1 - R2 / F)
                    lD2p = (2 * lD2) * (Range - Zone2Range) / (Zone2Range - Zone1Range) / (1 - ((Zone2Range - Zone1Range) / DensityAltCorrection(F2)));
                }
                lFa = DensityAltCorrection(F2) + (DensityAltCorrection(F3) - DensityAltCorrection(F2)) / 4.000;
                //D = (G/Vo/(1/R - 1/Fa))^2
                lD3 = Math.Pow(((_G / Zone2TransSpeed) / ((1.00 / (Range - Zone2Range)) - (1.00 / lFa))), 2);
                ld = lD1t + lD2p + lD3;
            }
            else if (Range > Zone3Range)
            {
                if (Zone3Range > 0)
                {
                    lD3 = MuzzleDrop(Zone3Range, true);
                    lFa = DensityAltCorrection(F2) + (DensityAltCorrection(F3) - DensityAltCorrection(F2)) / 4.000;
                    //D3' = gF3(Rt - (R1 + R2 + R3)) * 3((1 + 12 * R3 / F2) ^ (3 / 2) - 1) / 6 / (Zone2TransSpeed) ^ 2
                    lD3p = _GravIn * DensityAltCorrection(F3) * (Range - Zone3Range) * 3 * (Math.Pow((1 + 12 * (Zone3Range - Zone2Range) / lFa), (3 / 2)) - 1) / 6 / Math.Pow(Zone2TransSpeed, 2);
                }
                //D = (G/Vo/(1/R - 1/Fa))^2
                lD4 = Math.Pow((_G / Zone3TransSpeed / ((1.00 / (Range - Zone3Range) - 1 / DensityAltCorrection(F3)))), 2);
                ld = lD3 + lD3p + lD4;
            }

            //Add the small angle approximation error correction
            ld = ld + (12 * (Math.Pow((ld / 12), 3)) / (Math.Pow((Range * 3), 2)));

            if (RawMagnatude)
            {
                //This exit is used when called recursively to build ld for prior ranges before applying the
                //  vector and Coriolis corrections.
                return ld;
            }

            //Add the vector component, i.e. negative means it is falling lol
            ld = ld * (-1);

            //Add the earth rotation component
            ld = ld + GetCoriolisVert(Range);

            return ld;
        }
        /// <summary>
        /// Estimated Ballistic Coeficient
        /// </summary>
        /// <param name="Weight">Bullet weight in grains</param>
        /// <param name="Diameter">Bullet diameter</param>
        /// <param name="Shape">Shape type of bullet</param>
        /// <returns>BC</returns>
        public double EstimateBC(double Weight, double Diameter, BulletShapeEnum Shape)
        {
            double lBC; double lSC; double lDiv = 1;

            //'Shape Class:
            //'   Spitzer boat-tail = 1
            //'   Spitzer flat-base = 2
            //'   Semi-Spitzer = 3
            //'   Round Nose or Flat Nose = 4
            //'
            //'  BC = Wt(D + 0.5)/D^2/415/(SC^2 - 2SC + 8)
            //'
            switch (Shape)
            {
                case BulletShapeEnum.VLD_BoatTail:
                    lSC = 1;
                    lDiv = .8632;
                    break;

                default:
                    lSC = Convert.ToDouble(Shape);
                    lDiv = 1;
                    break;
            }
            // lBC = (((Weight * (Diameter + 0.5)) / ((Diameter) ^ 2)) / 415) / ((lSC) ^ 2 - 2 * lSC + 8)

            lBC = ((Weight * (Diameter + 0.5)) / (Math.Pow(Diameter, 2)) / 415) / (Math.Pow(lSC, 2) - 2 * lSC + 8);
            lBC = lBC / lDiv;
            return lBC;
        }
        /// <summary>
        /// Calculate the drag/retard coefficient at the muzzle.
        /// </summary>
        /// <param name="Range_1">Distance (ft) from the muzzle to where the V1 speed is recorded.</param>
        /// <param name="V1">Velocity (fps) at Range_1 ft from the muzzle.</param>
        /// <param name="Range_2">Distance (ft) from the muzzle to where the V2 speed is recorded.</param>
        /// <param name="V2">Velocity (fps) at Range_2 ft from the muzzle.</param>
        /// <returns>Fo</returns>
        public double CalculateFo(double Range_1, double V1, double Range_2, double V2)
        {
            double lFo = 0; double lVa; double ldV; double lF;

            try
            {

                //F=r*(Va/dV)
                lVa = (V1 + V2) / 2;
                ldV = (V1 - V2);
                lF = (Range_2 - Range_1) * (lVa / ldV);
                if (V1 >= Zone1TransSpeed)
                {
                    //Zone 1
                    //Fa=Fo-Zone1SlopeMultiplier*N*R
                    lFo = (lF + Zone1SlopeMultiplier * Zone1Slope * (Range_1 / 3));
                    Fo = lFo;
                    //V  = Vo(1-3RN/Fo)^2
                    //We must convert the provided range from feet to yards
                    // Solving for Vo we get the solution below.
                    // MuzzleVelocity = V1 / ((1 - (Range_1 / 3) * Zone1Slope / Fo) ^ (1 / Zone1Slope))
                    MuzzleVelocity = V1 / (Math.Pow((1 - (Range_1 / 3) * Zone1Slope / Fo), (1 / Zone1Slope)));
                }
                else if ((V1 < Zone1TransSpeed) & (V1 >= Zone2TransSpeed))
                {
                    //Zone 2

                    //F is constant in Zone 2, i.e. no slope N = 0.
                    lFo = lF;
                    Fo = lFo;
                    //Vo = V(exp(-3R/F))
                    MuzzleVelocity = V1 / (Math.Exp((-3 * ((Range_1 / 3) / Fo))));
                }
                else if ((V1 < Zone2TransSpeed) & (V1 >= Zone3TransSpeed))
                {
                    //Zone 3

                    //Fa=Fo-0.75*N*R
                    //F0 = Fa + Zone3SlopeMultiplier*N*R
                    lFo = lF + Zone3SlopeMultiplier * Zone3Slope * (Range_1 / 3);
                    F2 = lFo;
                    Fo = F2;
                    F3 = Fo - (Zone3SlopeMultiplier * Zone3Slope * Zone3Range);
                    //V  = Vo(1-3RN/Fo)^2
                    //We must convert the provided range from feet to yards
                    //Solving for Vo we get the solution below.
                    MuzzleVelocity = V1 / (Math.Pow((1 - (Range_1 / 3) * Zone3Slope / lFo), (1 / Zone3Slope)));
                }
                else
                {
                    //Zone 4

                    //F is constant in Zone 4
                    lFo = lF;
                    MuzzleVelocity = V1 / (Math.Exp((-3 * ((Range_1 / 3) / Fo))));
                }

                return lFo;
            }
            catch
            {
                lFo = 0;
                return lFo;
            }
        }
        /// <summary>
        /// Calculates the V2 velocity at a D2 of 300' using muzzle velocity as V1, D1 =0 ,and Ballistic Coefficient.
        /// </summary>
        /// <param name="MuzzleVelocity"></param>
        /// <param name="BC"></param>
        /// <returns></returns>
        public int CalculateV2FromBC()
        {
            double lc; double lR1; double lV2; int lRtn = 0;

            if ((_MuzzleVelocity == 0) || (_BCg1 == 0))
            {
                lRtn = -1;
                return lRtn;
            }
            D1 = 0;
            D2 = 300;
            V1 = MuzzleVelocity;
            //BC = r/(r2 - r1)
            //r = C*2*(3600^0.5-(V^0.5))
            lc = 166;
            lR1 = (lc * 2) * (Math.Pow(3600, 0.5) - (Math.Pow(_MuzzleVelocity, 0.5)));
            //We must solve r2 for velocity at 100 yards i.e. 300 ft
            //lR2 = (lc * 2) * (((3600#) ^ (0.5)) - (V2^(0.5)))
            //BC = 300 / (lR2 - lR1)
            //(lR2 - lR1) = 300 / BC
            //(lR2) = (300 / BC) + lR1
            //(lc * 2) * (((3600#) ^ (0.5)) - (V2^(0.5))) = (300 / BC) + lR1
            // - V2^(0.5) = (((300 / BC) + lR1)/(lc * 2)) - (3600# ^ (0.5))
            // V2^(0.5) = -(((300 / BC) + lR1)/(lc * 2)) - (3600# ^ (0.5))
            // V2 = ((((300 / BC) + lR1)/(lc * 2)) - (3600# ^ (0.5)))^2
            //Plug in the value for r1 calculated from provided V1
            lV2 = Math.Pow(((((300 / _BCg1) + lR1) / (lc * 2)) - (Math.Pow(3600, 0.5))), 2);
            V2 = lV2;
            return lRtn;
        }
        /// <summary>
        /// Calculates an estimate for the BCg1 of a bullet with the beginning and ending 
        /// velocities provided over the distance provided.
        /// </summary>
        /// <param name="V1">Velocity (fps) at the beginning of the distance provided.</param>
        /// <param name="V2">Velocity (fps) at the end of the distance provided.</param>
        /// <param name="DistanceFt">Distance (ft) between the two velocity values.  Should be 300' for more.</param>
        /// <returns></returns>
        public double CalculateBC(double V1, double V2, double DistanceFt)
        {
            double lc; double lR1; double lR2; double lBC;

            //R = C*2*(3600^0.5-(Vo^0.5))
            lc = 166;
            lR1 = (lc * 2) * ((Math.Pow(3600, 0.5)) - (Math.Pow(V1, 0.5)));
            lR2 = (lc * 2) * ((Math.Pow(3600, 0.5)) - (Math.Pow(V2, 0.5)));

            //BC = r/(r2 - r1)
            lBC = DistanceFt / (lR2 - lR1);
            return lBC;
        }
        /// <summary>
        /// Caclulates the bullet velocity at the provided range.
        /// </summary>
        /// <param name="Range">The range to calculate the velocity at.</param>
        /// <returns>Bullet velocity (fps)</returns>
        public double Velocity(double Range)
        {
            double lV; double lF; double lVd;

            //V = Vo(1-1.5R/Fo)^(1/N)
            //lVd = MuzzleVelocity * Math.Pow((1 - (1.5 * Range) / Fo), (1 / Zone1Slope));

            //TODO: Zone errors occur with the below code, needs investigation
            if ((Zone2Range >= Range) & (Range > Zone1Range))
            {
                //'    'Zone 2
                //'
                //'    lF = F2
                //'    'V = Vo(exp(-3R/F))                
                lVd = Zone1TransSpeed * Math.Exp((-3 * (Range-Zone1Range) / F2));                
            }
            else if ((Zone3Range >= Range) & (Range > Zone2Range))
            {
                //'    'Zone 3
                //'
                //'    lF = F3
                //'    'V = Vo(1-1.5R/Fo)^(1/N)
                //lVd = Zone2TransSpeed * Math.Pow((1 - (1.5 * (Range-Zone2Range)) / F2), (1 / -Zone3Slope));

                //      Zone 1

                //      V = Vo(1-1.5R/Fo)^(1/N)
                lVd = MuzzleVelocity * Math.Pow((1 - (1.5 * Range) / Fo), (1 / Zone1Slope));
            }
            else if (Zone1Range >= Range)
            {
                //      Zone 1

                //      V = Vo(1-1.5R/Fo)^(1/N)
                lVd = MuzzleVelocity * Math.Pow((1 - (1.5 * Range) / Fo), (1 / Zone1Slope));
            }
            else
            {
                //'    'Zone 4
                //'
                //'    'V = Vo(exp(-3R/F))
                //lVd = Zone3TransSpeed * Math.Exp((-3 * (Range-Zone3Range) / F4));

                //      Zone 1

                //      V = Vo(1-1.5R/Fo)^(1/N)
                lVd = MuzzleVelocity * Math.Pow((1 - (1.5 * Range) / Fo), (1 / Zone1Slope));
            }
            



            return lVd;
        }
        /// <summary>
        /// Calculates a bullets enery with the given weight at the provided range.
        /// </summary>
        /// <param name="Weight">Bullet weight (grns).</param>
        /// <param name="Range">Distance to calculate energy at (yrds).</param>
        /// <returns>Bullet energy (ft.lb.)</returns>
        public double Energy(double Weight, double Range)
        {
            double lE;

            //E=V^2Wt/450380 ftlb
            lE = (Math.Pow(Velocity(Range), 2) * Weight) / 450380;
            return lE;
        }
        /// <summary>
        /// Calculates a bullets enery with the given weight at the provided velocity.
        /// </summary>
        /// <param name="Weight">Bullet weight (grns).</param>
        /// <param name="Velocity">Bullet speed (fps)</param>
        /// <returns>Bullet energy (ft.lb.)</returns>
        public double EnergyVelocity(double Weight, double Velocity)
        {
            double lE;

            //E = V ^ 2Wt / 450380 ftlb
            lE = (Math.Pow(Velocity, 2) * Weight) / 450380;
            return lE;
        }
        /// <summary>
        /// Calculates the bullet flight time to the provided range.
        /// </summary>
        /// <param name="Range">Distance (yrds) to calculate the flight time for.</param>
        /// <returns>Bullet flight time in seconds to reach the provided range.</returns>
        public double FlightTime(double Range)
        {
            double lt; double lV; double lF;

            try
            {

                //T = 1/(1/3R - 1/2F)Vo sec
                lt = (1 / ((1 / (3 * Range)) - (1 / (2 * Fo)))) / MuzzleVelocity;
                if (lt < 0) lt = 0;

                //TODO: Zone errors occur with the below code, needs investigation
                //If (Zone2Range >= Range) And (Range > Zone1Range) Then
                //    'Zone 2
                //
                //    'T = Fo / (Vo(exp(3R / Fo) - 1)
                //    lT = F2 / (Zone1TransSpeed * (Exp((3 * Range / F2)) - 1))
                //ElseIf (Zone3Range >= Range) And (Range > Zone2Range) Then
                //    'Zone 3
                //
                //    'T = 3R / Vo / (1 - 3R / 2Fo + (1 - 2N)/ 130)
                //    lT = (3 * Range) / Zone2TransSpeed / (1 - (3 * Range) / (2 * F3) + (1 - 2 * Zone3Slope) / 130)
                //ElseIf (Zone1Range >= Range) Then
                //    'Zone 1
                //
                //    'T = 3R / Vo / (1 - 3R / 2Fo + (1 - 2N)/ 130)
                //    lT = (3 * Range) / Vo / (1 - (3 * Range) / (2 * Fo) + (1 - 2 * Zone1Slope) / 130)
                //Else
                //    'Zone 4
                //
                //    'T = Fo / (Vo(exp(3R / Fo) - 1)
                //    lT = F4 / (Zone3TransSpeed * (Exp((3 * Range / F4)) - 1))
                //End If

                return lt;
            }
            catch
            {
                return 0;
            }
        }
        public double TotalHorizontalDrift(double Range)
        {          
                double lTW;

            lTW =  GetCoriolisHoriz(Range) + GetSpinDrift(Range) + WindDriftDegrees(WindSpeed, WindDirectionDeg, Range);

            return lTW;
        }
        /// <summary>
        /// Vertical Coriolis affect at the provided range.
        /// </summary>
        /// <param name="Range">Distance to calculate Coriolis affect at.</param>
        /// <returns>Inches of rise or drop caused by Coriolis addect.</returns>
        public double GetCoriolisVert(double Range)
        {
            double lCorVert = 0; double lCorHoriz = 0;
            int lR;

            lR = GetCoriolisComponents(ref lCorVert, ref lCorHoriz, Range);
            return lCorVert;
        }
        /// <summary>
        /// Horizontal Coriolis affect at the provided range.
        /// </summary>
        /// <param name="Range">Distance to calculate Coriolis affect at.</param>
        /// <returns>Inches of horizontal drift caused by Coriolis addect.</returns>
        public double GetCoriolisHoriz(double Range)
        {
            double lCorVert = 0; double lCorHoriz = 0;
            int lR;

            lR = GetCoriolisComponents(ref lCorVert, ref lCorHoriz, Range);
            return lCorHoriz;
        }
        /// <summary>
        /// Maximum distance the bullet/rifle combination can achieve.
        /// </summary>
        /// <returns></returns>
        public double MaxRange()
        {
            double lMR = 0;

            //lV = Vo * (1 - (1.5 * Range) / lF) ^ (1 / Zone1Slope)
            //0 = Vo * (1 - (1.5 * Range) / lF) ^ (1 / Zone1Slope)
            //0 = (1 - (1.5 * Range) / lF) ^ (1 / Zone1Slope)
            //0 = (1 - (1.5 * Range) / lF)
            //0 = 1 - (1.5 * Range) / lF
            //(1.5 * Range) / lF = 1
            //(1.5 * Range) = lF
            //Range = lF/1.5
            lMR = Fo / 1.5;
            return lMR;
        }
        /// <summary>
        /// The bullet vertical location with respect to the sight plane.
        /// </summary>
        /// <param name="Range">The range to return the bullet location vs sight plane at.</param>
        /// <returns>Inches above or below the sight plane.</returns>
        public double SightDelta(double Range)
        {
            double lH;
            double lM;
            double lSD;

            //TODO: SightDelta should not be calculated with current shot density alt as it is what it was set
            //  during zero.  lM should be saved as a persistent parameter for the data set or the shot and zero
            // environmentals be set the same to calculate lM.

            //Bullet vertical location relative to scope sight line.           
            lH = (-MuzzleDrop(ZeroRange) + ScopeHeight);
            lM = (lH / 2) * (1 / (ZeroRange / 2));
            lSD = ((Range * lM) + (MuzzleDrop(Range) - ScopeHeight));
            return lSD;
        }
        /// <summary>
        /// Calculates the maximum bullet rise on the way to the zero range.
        /// </summary>
        public void CalculateHm()
        {
            //Must iterate flight path to find Hm
            double ld;
            double lInter;
            double lTmpH;
            double lR = 0;
            double lH;

            ld = MuzzleDrop(ZeroRange);
            lInter = (-ld + ScopeHeight) / 2;
            lTmpH = -100;
            lH = -100;
            //Iterate through range from 0 to Scope 0 range to find range at max rise.
            while (lH >= lTmpH)
            {
                if (lH > lTmpH)
                {
                    lTmpH = lH;
                }
                //Increment the range by 1/2 yard at a time to find the Hm range.
                lR = lR + 0.5;
                lH = (lInter * (1 / (ZeroRange / 2)) * lR) - (-MuzzleDrop(lR) + ScopeHeight);
                if (lR > ZeroRange)
                {
                    break;
                }
            }
            _Hm = lTmpH;
            RaisePropertyChanged(nameof(ZeroMaxRise));
        }
        /// <summary>
        /// Calculates the range where the vertical travel of the bullet and sight plane equal with a bullet
        /// travel that rises above the sight plan by exactly Hm.
        /// </summary>
        /// <param name="Hm">The maximum rise above the sight plan between the muzzle and zero range.</param>
        /// <returns>Zero Range (yrds)</returns>
        public double CalculateZeroRange(double Hm)
        {
            double lZ;

            if ((SH() == 0) || (MidRange == 0)) return 0;
            //Z = (1+SH)/(1/Fo + SH/M)
            lZ = (1 + SH()) / ((1 / Fo) + (SH() / MidRange));
            return lZ;
        }
        /// <summary>
        /// Returns the horizontal displacement related to the gyroscopic forces of the bullet.
        /// </summary>
        /// <param name="Range">The range to find the displacement for.</param>
        /// <returns>The horizontal displacement in inches related to the bullets gyroscopic force.</returns>
        public double GetSpinDrift(double Range)
        {
            double lDrift;

            lDrift = GetRawSpinDrift(Range);
            lDrift = lDrift - (SpindDriftCorrection * Range);
            return lDrift;
        }
        /// <summary>
        /// Returns the horizontal displacement caused by wind.
        /// </summary>
        /// <param name="WindSpeed">The speed of the wind in mph.</param>
        /// <param name="WindDirection">The direction of the wind in degrees with the target at 0 degrees.</param>
        /// <param name="Range">The distance to the target in yards.</param>
        /// <returns>Horizontal displacement in inches</returns>
        public double WindDriftDegrees(double WindSpeed, float WindDirection, double Range)
        {
            double lt;      //Flight time
            double lTF;     //Theoretical Flight time with no drag
            double lDT;     //Flight time due to drag
            double lWD;     //Wind direction in degrees
            double lCW;     //Cross wind magnitude
            double lWDR;    //Wind displacement rate
            double lHWD;    //Horizontal wind drift

            //Actual flight time
            lt = FlightTime(Range);
            //Theoretical Flight time with no drag
            lTF = (3 * Range) / MuzzleVelocity;
            //Flight time due to drag
            lDT = lt - lTF;

            //TODO: Need to add the affects of head and tail wind.  I know they are miniscule
            //  but a hair at 200 yards is feet at over a mile out.

            //Wind Angle
            lWD = WindDirection;
            if (lWD == 360) lWD = 0; // i.e. it is straight ahead

            //Find the cross wind component
            if (lWD < 90)
            {
                //Wind is pushing the bullet to the left i.e. negative
                lCW = Math.Sin((lWD * (Math.PI / 180))) * WindSpeed * (-1);
            }
            else if ((lWD > 90) & (lWD < 180))
            {
                //Wind is pushing the bullet to the left i.e. negative
                lCW = Math.Sin((180 - lWD) * (Math.PI / 180)) * WindSpeed * (-1);
            }
            else if ((lWD > 180) & (lWD < 270))
            {
                lCW = Math.Sin((lWD - 180) * (Math.PI / 180)) * WindSpeed;
            }
            else if ((lWD > 180) & (lWD < 270))
            {
                lCW = Math.Sin((lWD - 180) * (Math.PI / 180)) * WindSpeed;
            }
            else if ((lWD > 270) & (lWD < 360))
            {
                lCW = Math.Sin((360 - lWD) * (Math.PI / 180)) * WindSpeed;
            }
            else if (lWD == 90)
            {
                lCW = WindSpeed * (-1);
            }
            else if (lWD == 270)
            {
                lCW = WindSpeed;
            }
            else
            {
                lCW = 0;
            }
            //Convert from mph to fps
            lCW = lCW * 1.466667;
            //Calculate the wind displacement rate
            lWDR = lCW / MuzzleVelocity;
            //Horizontal wind drift multiplied by 12 to convert ft to inches.
            lHWD = lWDR * Range * 12;
            return lHWD;
        }
        /// <summary>
        /// Returns the horizontal displacement caused by wind.
        /// </summary>
        /// <param name="WindSpeed">The speed of the wind in mph.</param>
        /// <param name="Clock">The direction of the wind in clock readings where the target is at 12 O'clock.</param>
        /// <param name="Range">The distance to the target in yards.</param>
        /// <returns>Horizontal displacement in inches</returns>
        public double WindDriftDegrees(double WindSpeed, uint Clock, double Range)
    {
        float lDegP = 360/12;       //Degrees per clock reading
        float lWD = lDegP * Clock;  //Total degrees

        return WindDriftDegrees(WindSpeed, lWD, Range);

    }
        #endregion

        #region "Private Routines"
        private void GetWeather()
        {
            try
            {
                XmlDocument lWeatherXML = new XmlDocument();
                string lRtn = "";
                string lUrl = _GetWeather + Properties.Settings.Default.Openweathermap;
                string lLat = _ShooterLatitude.ToString();
                string lLon = _ShooterLongitude.ToString();
                lUrl = lUrl.Replace("@latitude@", lLat);
                lUrl = lUrl.Replace("@longitude@", lLon);
                WebClient lwc = new WebClient();
                lRtn = lwc.DownloadString(lUrl);
                lWeatherXML.LoadXml(lRtn);
                lWeatherXML.Save("C:\\Users\\QIS\\Desktop\\Temp\\Weather.xml");
                XmlNode lCurr = lWeatherXML.SelectSingleNode("current");
                lRtn = "temp = ";
                XmlNode lTmpN = lCurr.SelectSingleNode("temperature");
                lRtn = lRtn + lTmpN.Attributes["value"].Value + ", Pressure = ";
                XmlNode lPressN = lCurr.SelectSingleNode("pressure");
                lRtn = lRtn + lPressN.Attributes["value"].Value;
                XmlNode lHumN = lCurr.SelectSingleNode("humidity");
                lRtn = "";

            }
            catch(Exception ex)
            {

            }

        }
        /// <summary>
        /// Calculates the horizontal and vertical devation of the flight path due to the Coriolis effect.
        /// </summary>
        /// <param name="VerticalComponent">Vertical drop or rise (in.) caused by the Coriolis effect.</param>
        /// <param name="HorizontalComponent">Horizontal left or right drift (in.) caused by the Coriolis effect.</param>
        /// <param name="Range">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <returns>A routine trouble code and the horizontal and vertical effect of the earths rotation.</returns>
        private int GetCoriolisComponents(ref double VerticalComponent, ref double HorizontalComponent, double Range)
        {
            double lYrdPerDeg; double lLonDist; double lLatDist; double lVertMultiplier;
            double lHorizMultiplier; double lFT; double lCorMag; double lSVM; double lSHM;
            double lCorVert; double lCorHorz; double lLonSign; double lLatSign; double lLonDistZ;
            double lLonSignZ; double lLatDistZ; double lLatSignZ; double lZeroVertMultiplier;
            double lZeroHorizMultiplier; double lZSVM; double lZSHM; double lCorZero; double lCorZVS;
            double lCorZHS; double lFTZ;


            if ((zTargetLon == 0) || (zShooterLon == 0) || (zTargetLat == 0) || (zShooterLat == 0)
                || (ShooterLat == 0) || (ShooterLon == 0) || (TargetLat == 0) || (TargetLon == 0))
            {
                HorizontalComponent = 0;
                VerticalComponent = 0;
                return -1;
            }

            lYrdPerDeg = 121740.6652;
            lFT = FlightTime(Range);
            lFTZ = FlightTime(ZeroRange);
            lCorMag = 0.00262 * lFT * Range;
            lCorZero = 0.00262 * lFTZ * ZeroRange;

            //______________ Zero bias ___________________________
            //  Solve for the amount of Coriolis correction added during zero, this is
            //  like the sight line in that it is linear with distance and zero at the zero ranges.

            //Longitudal distance to target
            lLonDistZ = (zTargetLon - zShooterLon) * lYrdPerDeg;
            lLonSignZ = Math.Sign(lLonDistZ);  // + = E, - = W
            if (lLonSignZ == 0) lLonSignZ = 1;
            //Latitudal distance to target
            lLatDistZ = (zTargetLat - zShooterLat) * lYrdPerDeg;
            lLatSignZ = Math.Sign(lLatDistZ);  // + = N, - = S
            if (lLatSignZ == 0) lLatSignZ = 1;
            //Correction factors for location on earth, the larger the lat the less vertical affect and more horizontal,
            //     convert degrees to radians
            lZeroVertMultiplier = Math.Cos(zShooterLat * (Math.PI / 180));
            lZeroHorizMultiplier = 1 - lZeroVertMultiplier;
            //Vertical correction factor for Lat/Lon angle
            if (lLonDistZ == 0)
            {
                lZSVM = 0;
            }
            else
            {
                lZSVM = Math.Atan(Math.Abs(lLatDistZ / lLonDistZ));  //Radians of shot angle
                lZSVM = Math.Cos(lZSVM);
            }
            lZSHM = 1 - lZSVM;
            if (lLatDistZ == 0) lZSHM = 0;

            lZSVM = lZSVM * lLonSignZ;
            lZSHM = lZSHM * lLatSignZ;

            lCorZVS = (lCorZero * lZeroVertMultiplier * lZSVM) / ZeroRange;
            lCorZHS = (lCorZero * lZeroHorizMultiplier * lZSHM) / ZeroRange;

            //Positive = high, East
            //Negative = low, west

            //Longitudal distance to target
            lLonDist = (TargetLon - ShooterLon) * lYrdPerDeg;
            lLonSign = Math.Sign(lLonDist);  // + = E, - = W
            if (lLonSign == 0) lLonSign = 1;
            //Latitudal distance to target
            lLatDist = (TargetLat - ShooterLat) * lYrdPerDeg;
            lLatSign = Math.Sign(lLatDist);  // + = N, - = S
            if (lLatSign == 0) lLatSign = 1;
            //Correction factors for location on earth, convert degrees to radians
            lVertMultiplier = Math.Cos(ShooterLat * (Math.PI / 180));
            lHorizMultiplier = 1 - lVertMultiplier;
            //Vertical correction factor for Lat/Lon angle
            if (lLonDist == 0)
            {
                lSVM = 0;
            }
            else
            {
                lSVM = Math.Atan(Math.Abs(lLatDist / lLonDist)); //Radians of shot angle
                lSVM = Math.Cos(lSVM);
            }
            lSHM = 1 - lSVM;
            lSVM = lSVM * lLonSign;
            lSHM = lSHM * lLatSign;

            // __________________ Need to add in residual drop and windage that accumulates past zero distance _______
            // Zero condition correction is wrong.  should be a linear line with range.  It passes through the
            //  coriolis error at the zero range (i.e. 1" correction at 100 yards = 5" at 500 yet the coriolis
            //  error might be 6" at 500 due to the bullet lossing velocity.

            //Positive = high, East
            //Negative = low, west
            lCorVert = (lCorMag * lVertMultiplier * lSVM) - lCorZVS * Range;
            //Postive = right north
            //Negative = left south
            lCorHorz = (lCorMag * lHorizMultiplier * lSHM) - lCorZHS * Range;
            lSHM = 0;
            VerticalComponent = lCorVert;
            HorizontalComponent = lCorHorz;
            return 0;
        }
        /// <summary>
        /// Calculates the effective drag/retard coefficent for the provided range.
        /// </summary>
        /// <param name="Range">Distance to calculate the F value at.</param>
        /// <returns>F drag/retard coefficent</returns>
        private double Fa(double Range)
        {
            double lFa = 0;

            if (Zone1Range >= Range)
            {
                //F at the provided range.  Applies to Zone 1
                lFa = (Fo - Zone1SlopeMultiplier * Zone1Slope * Range);
            }
            else if ((Zone1Range < Range) & (Zone2Range >= Range))
            {
                //F at the provided range.  Applies to Zone 2
                lFa = F2;
            }
            else if ((Zone2Range < Range) & (Zone3Range >= Range))
            {
                //F at the provided range.  Applies to Zone 3
                lFa = (F3 - Zone3SlopeMultiplier * Zone3Slope * (Range - Zone3Range));
            }
            else if (Zone3Range < Range)
            {
                //F at the provided range.  Applies to Zone 4
                lFa = F4;
            }
            return lFa;
        }
        /// <summary>
        /// Magnitude of the horizontal displacement induced by the bullets gyroscopic force.
        /// </summary>
        /// <param name="Range">The range to find the displacement at.</param>
        /// <returns>Horizontal distance in inches.</returns>
        private double GetRawSpinDrift(double Range)
        {
            double lFT;
            double lDrift;
            double lZSD;
            double lZDC;
            string lcmp = BarrelTwistDir.ToLower();

            lFT = FlightTime(Range);
            lDrift = 1.25 * (BSG + 1.2) * Math.Pow(lFT, 1.83);
            if (lcmp != "r")
            {
                lDrift = lDrift * (-1);
            }
            return lDrift;
        }                     
        #endregion

        #region "Constructor"
        public Ballistics(Bullet bullet, Barrel barrel)
        {
            _BulletShapeType = new BulletShapeEnum();
            _SelectedBullet = bullet;
            _SelectedBarrel = barrel;
        }
        #endregion
    }

}
