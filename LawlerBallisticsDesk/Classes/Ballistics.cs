using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        private Scenario _MyScenario;
        private bool _FoCalc;
        #endregion

        #region "Public Properties"
        public Scenario MyScenario { get { return _MyScenario; } }
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
            if ((MyScenario.MyBallisticData.zeroData.dragSlopeData.Fo > 0) & (MyScenario.MyShooter.MyLoadOut.MuzzleVelocity > 0))
            {
                //Fo is provided.  Set First bit value to 1

                //TODO: Calculate V1, D1, V2, D2 so BC and other calculations can be solved.

                _FoCalc = true;
                lRtn = 1;
            }
            //Check to see if velocity-range or BC-MV method is used to find Fo.
            if ((MyScenario.MyBallisticData.dragSlopeData.V1 > 0) & (MyScenario.MyBallisticData.dragSlopeData.V2 > 0) &
                (MyScenario.MyBallisticData.dragSlopeData.D2 > 0))
            {
                //Actual velocities and distances provided.
                lRtn += 2;
                _FoCalc = true;
            }
            else if ((MyScenario.MyShooter.MyLoadOut.MuzzleVelocity > 0) & (MyScenario.MyBallisticData.dragSlopeData.BCg1> 0))
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
            MyScenario.MyBallisticData.dragSlopeData.Fo = CalculateFo(MyScenario.MyBallisticData.dragSlopeData.D1, 
                MyScenario.MyBallisticData.dragSlopeData.V1, MyScenario.MyBallisticData.dragSlopeData.D2, MyScenario.MyBallisticData.dragSlopeData.V2);
            MyScenario.MyBallisticData.dragSlopeData.BCg1 = CalculateBC(MyScenario.MyBallisticData.dragSlopeData.V1, 
                MyScenario.MyBallisticData.dragSlopeData.V2, (MyScenario.MyBallisticData.dragSlopeData.D2 - MyScenario.MyBallisticData.dragSlopeData.D1));
            if (MyScenario.MyBallisticData.dragSlopeData.Fo <= 0)
            {
                //Invalid Fo
                lRtn = -3;
                return lRtn;
            }
            if (MyScenario.MyBallisticData.zeroData.UseMaxRise & (MyScenario.MyBallisticData.zeroData.ZeroMaxRise == 0))
            {
                //Cannot calculate sightline without a valid zero point.
                lRtn = -8;
                return lRtn;
            }
            else if ((MyScenario.MyBallisticData.zeroData.UseMaxRise) & (MyScenario.MyBallisticData.zeroData.ZeroMaxRise > 0))
            {
                MyScenario.MyBallisticData.zeroData.ZeroRange = CalculateZeroRange(MyScenario.MyBallisticData.zeroData.ZeroMaxRise);                
            }
            if ((!MyScenario.MyBallisticData.zeroData.UseMaxRise) & (MyScenario.MyBallisticData.zeroData.ZeroRange > 0))
            {
                CalculateHm();
            }
            
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //All required data is present to calculate raw muzzle drop just counting drag and gravity.
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            if (MyScenario.MyShooter.MyLoadOut.BSG != 0)
            {
                //Spin Drify calculation enabled.
                lRtn += 8;
            }


            return lRtn;
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
                if (V1 >= MyScenario.MyBallisticData.dragSlopeData.Zone1TransSpeed)
                {
                    //Zone 1
                    //Fa=Fo-Zone1SlopeMultiplier*N*R
                    lFo = (lF + MyScenario.MyBallisticData.dragSlopeData.Zone1SlopeMultiplier *
                        MyScenario.MyBallisticData.dragSlopeData.Zone1Slope * (Range_1 / 3));
                    MyScenario.MyBallisticData.dragSlopeData.Fo = lFo;
                    //V  = Vo(1-3RN/Fo)^2
                    //We must convert the provided range from feet to yards
                    // Solving for Vo we get the solution below.
                    // MuzzleVelocity = V1 / ((1 - (Range_1 / 3) * Zone1Slope / Fo) ^ (1 / Zone1Slope))
                    MyScenario.MyShooter.MyLoadOut.MuzzleVelocity = V1 / (Math.Pow((1 - (Range_1 / 3) * 
                        MyScenario.MyBallisticData.dragSlopeData.Zone1Slope / 
                        MyScenario.MyBallisticData.dragSlopeData.Fo),
                        (1 / MyScenario.MyBallisticData.dragSlopeData.Zone1Slope)));
                }
                else if ((V1 < MyScenario.MyBallisticData.dragSlopeData.Zone1TransSpeed) &
                    (V1 >= MyScenario.MyBallisticData.dragSlopeData.Zone2TransSpeed))
                {
                    //Zone 2

                    //F is constant in Zone 2, i.e. no slope N = 0.
                    lFo = lF;
                    MyScenario.MyBallisticData.dragSlopeData.Fo = lFo;
                    //Vo = V(exp(-3R/F))
                    MyScenario.MyShooter.MyLoadOut.MuzzleVelocity = V1 / (Math.Exp((-3 * ((Range_1 / 3) / 
                        MyScenario.MyBallisticData.dragSlopeData.Fo))));
                }
                else if ((V1 < MyScenario.MyBallisticData.dragSlopeData.Zone2TransSpeed) &
                    (V1 >= MyScenario.MyBallisticData.dragSlopeData.Zone3TransSpeed))
                {
                    //Zone 3

                    //Fa=Fo-0.75*N*R
                    //F0 = Fa + Zone3SlopeMultiplier*N*R
                    lFo = lF + MyScenario.MyBallisticData.dragSlopeData.Zone3SlopeMultiplier * 
                        MyScenario.MyBallisticData.dragSlopeData.Zone3Slope * (Range_1 / 3);
                    MyScenario.MyBallisticData.dragSlopeData.F2 = lFo;
                    MyScenario.MyBallisticData.dragSlopeData.Fo = MyScenario.MyBallisticData.dragSlopeData.F2;
                    MyScenario.MyBallisticData.dragSlopeData.F3 = MyScenario.MyBallisticData.dragSlopeData.Fo - (MyScenario.MyBallisticData.dragSlopeData.Zone3SlopeMultiplier * 
                        MyScenario.MyBallisticData.dragSlopeData.Zone3Slope * MyScenario.MyBallisticData.dragSlopeData.Zone3Range);
                    //V  = Vo(1-3RN/Fo)^2
                    //We must convert the provided range from feet to yards
                    //Solving for Vo we get the solution below.
                    MyScenario.MyShooter.MyLoadOut.MuzzleVelocity = V1 / (Math.Pow((1 - (Range_1 / 3) * 
                        MyScenario.MyBallisticData.dragSlopeData.Zone3Slope / lFo),
                        (1 / MyScenario.MyBallisticData.dragSlopeData.Zone3Slope)));
                }
                else
                {
                    //Zone 4

                    //F is constant in Zone 4
                    lFo = lF;
                    MyScenario.MyShooter.MyLoadOut.MuzzleVelocity = V1 / (Math.Exp((-3 * ((Range_1 / 3) / MyScenario.MyBallisticData.dragSlopeData.Fo))));
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

            if ((MyScenario.MyShooter.MyLoadOut.MuzzleVelocity == 0) || (MyScenario.MyBallisticData.dragSlopeData.BCg1 == 0))
            {
                lRtn = -1;
                return lRtn;
            }
            MyScenario.MyBallisticData.dragSlopeData.D1 = 0;
            MyScenario.MyBallisticData.dragSlopeData.D2 = 300;
            MyScenario.MyBallisticData.dragSlopeData.V1 = MyScenario.MyShooter.MyLoadOut.MuzzleVelocity;
            //BC = r/(r2 - r1)
            //r = C*2*(3600^0.5-(V^0.5))
            lc = 166;
            lR1 = (lc * 2) * (Math.Pow(3600, 0.5) - (Math.Pow(MyScenario.MyShooter.MyLoadOut.MuzzleVelocity, 0.5)));
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
            lV2 = Math.Pow(((((300 /MyScenario.MyBallisticData.dragSlopeData.BCg1) + lR1) / (lc * 2)) - 
                (Math.Pow(3600, 0.5))), 2);
            MyScenario.MyBallisticData.dragSlopeData.V2 = lV2;
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
            if ((MyScenario.MyBallisticData.dragSlopeData.Zone2Range >= Range) & (Range > MyScenario.MyBallisticData.dragSlopeData.Zone1Range))
            {
                //'    'Zone 2
                //'
                //'    lF = F2
                //'    'V = Vo(exp(-3R/F))                
                lVd = MyScenario.MyBallisticData.dragSlopeData.Zone1TransSpeed * 
                    Math.Exp((-3 * (Range- MyScenario.MyBallisticData.dragSlopeData.Zone1Range) / MyScenario.MyBallisticData.dragSlopeData.F2));                
            }
            else if ((MyScenario.MyBallisticData.dragSlopeData.Zone3Range >= Range) & (Range > MyScenario.MyBallisticData.dragSlopeData.Zone2Range))
            {
                //'    'Zone 3
                //'
                //'    lF = F3
                //'    'V = Vo(1-1.5R/Fo)^(1/N)
                //lVd = Zone2TransSpeed * Math.Pow((1 - (1.5 * (Range-Zone2Range)) / F2), (1 / -Zone3Slope));

                //      Zone 1

                //      V = Vo(1-1.5R/Fo)^(1/N)
                lVd = MyScenario.MyShooter.MyLoadOut.MuzzleVelocity * Math.Pow((1 - (1.5 * Range) / MyScenario.MyBallisticData.dragSlopeData.Fo), (1 / MyScenario.MyBallisticData.dragSlopeData.Zone1Slope));
            }
            else if (MyScenario.MyBallisticData.dragSlopeData.Zone1Range >= Range)
            {
                //      Zone 1

                //      V = Vo(1-1.5R/Fo)^(1/N)
                lVd = MyScenario.MyShooter.MyLoadOut.MuzzleVelocity * Math.Pow((1 - (1.5 * Range) / MyScenario.MyBallisticData.dragSlopeData.Fo), 
                    (1 / MyScenario.MyBallisticData.dragSlopeData.Zone1Slope));
            }
            else
            {
                //'    'Zone 4
                //'
                //'    'V = Vo(exp(-3R/F))
                //lVd = Zone3TransSpeed * Math.Exp((-3 * (Range-Zone3Range) / F4));

                //      Zone 1

                //      V = Vo(1-1.5R/Fo)^(1/N)
                lVd = MyScenario.MyShooter.MyLoadOut.MuzzleVelocity * Math.Pow((1 - (1.5 * Range) / MyScenario.MyBallisticData.dragSlopeData.Fo), 
                    (1 / MyScenario.MyBallisticData.dragSlopeData.Zone1Slope));
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
        public double TotalHorizontalDrift(double Range)
        {          
                double lTW;

            lTW =  GetCoriolisHoriz(Range) + GetSpinDrift(Range) + WindDriftDegrees(MyScenario.SelectedTarget.WindSpeed, MyScenario.SelectedTarget.WindDirection, Range);

            return lTW;
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
            lMR = MyScenario.MyBallisticData.dragSlopeData.Fo / 1.5;
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
            lH = (-MuzzleDrop(MyScenario.MyBallisticData.zeroData.ZeroRange) + MyScenario.MyShooter.MyLoadOut.ScopeHeight);
            lM = (lH / 2) * (1 / (MyScenario.MyBallisticData.zeroData.ZeroRange / 2));
            lSD = ((Range * lM) + (MuzzleDrop(Range) - MyScenario.MyShooter.MyLoadOut.ScopeHeight));
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

            ld = MuzzleDrop(MyScenario.MyBallisticData.zeroData.ZeroRange);
            lInter = (-ld + MyScenario.MyShooter.MyLoadOut.ScopeHeight) / 2;
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
                lH = (lInter * (1 / (MyScenario.MyBallisticData.zeroData.ZeroRange / 2)) * lR) - (-MuzzleDrop(lR) + MyScenario.MyShooter.MyLoadOut.ScopeHeight);
                if (lR > MyScenario.MyBallisticData.zeroData.ZeroRange)
                {
                    break;
                }
            }
            MyScenario.MyBallisticData.zeroData.ZeroMaxRise = lTmpH;
            RaisePropertyChanged(nameof(MyScenario.MyBallisticData.zeroData.ZeroMaxRise));
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

            if ((SH() == 0) || (MyScenario.MyBallisticData.zeroData.MidRange == 0)) return 0;
            //Z = (1+SH)/(1/Fo + SH/M)
            lZ = (1 + SH()) / ((1 / MyScenario.MyBallisticData.dragSlopeData.Fo) + (SH() / MyScenario.MyBallisticData.zeroData.MidRange));
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
        #endregion

        #region "Private Routines"
        /// <summary>
        /// Factor used to calculate Zero range, Near Zero Range, and Point-Blank-Range (PBR)
        /// </summary>
        /// <returns></returns>
        private double SH()
        {
            double lSH;

            if (MyScenario.MyBallisticData.zeroData.ZeroMaxRise == 0) return 0;
            //SH = (1 + S/Hm)^0.5
            lSH = Math.Pow((1 + (MyScenario.MyShooter.MyLoadOut.ScopeHeight / MyScenario.MyBallisticData.zeroData.ZeroMaxRise)), 0.5);
            return lSH;
        }
        #endregion

        #region "Constructor"
        public Ballistics(Scenario scenario)
        {
            _MyScenario = scenario;
        }
        #endregion
    }

}
