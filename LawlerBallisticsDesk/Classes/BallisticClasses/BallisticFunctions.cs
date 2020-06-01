using AvalonDock.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes.BallisticClasses
{
    public static class BallisticFunctions
    {
        public static double GravFt = 32.17405;
        public static double GravIn = 386.0886;
        public static double G = 41.68211487;      //G = 3*(g(in/s^2)/2)^0.5 = 41.68
        public static double YardsPerDegLatLon = 121740.6652;
        public static double in3Toft3 = 0.000578704;
        public static double in2Toft2 = 0.00694444;

        //TODO: Change all velocity calculations as zones to trans speeds rather than calculated velocity at zone range.
        //TODO: Add zone 2 and 4 calculations for slope > 0 to time velocity and drop.  Add slope multiplier for zones 2 and 4.
        //TODO: Add density alt correction for all calculations where applicable.


        #region "Gyro Functions"
        /// <summary>
        /// The gyroscopic stability measure of the bullet, a value of 1 is minimal stability.
        /// </summary>
        /// <param name="SelectedBullet">Bullet being analyzed.</param>
        /// <param name="SelectedBarrel">Barrel bullet is shot from.</param>
        /// <param name="Vel">Velocity in fps</param>
        /// <param name="Temp">Ambient temperature in F</param>
        /// <param name="BaroPressure">Barometric pressure in inHg</param>
        /// <returns></returns>
        public static double GyroscopicStability(Bullet SelectedBullet, Barrel SelectedBarrel, double Vel, double Temp, double BaroPressure)
        {
            //L = bullet length in inches.
            //d = bullet diameter in inches.
            //T = barrel inches per turn.
            //m = bullet mass in grains.
            //l = L/d
            //t = T/d
            //S = ((30m)/((t^2d^3(l)(1+l^2))) * (V/2800)^(1/3) * ((T + 460)/519) * (29.92/Baro)
            double lS; double lt; double lL;
            lL = SelectedBullet.Length / SelectedBullet.Diameter;
            lt = SelectedBarrel.Twist / SelectedBullet.Diameter;
            lS = (30 * SelectedBullet.Weight) / ((Math.Pow((lt), 2)) * (Math.Pow(SelectedBullet.Diameter, 3)) * lL * (lL + Math.Pow(lL, 2)));
            lS = lS * Math.Pow((Vel / 2800), (1.00 / 3.00));
            lS = lS * (Temp + 460) / 519.00;
            lS = lS * (29.92 / BaroPressure);

            return lS;
        }
        /// <summary>
        /// Rpm of the bullet at the muzzle.
        /// </summary>
        /// <param name="BarrelTwist">Number of inches per twist of the rifling.</param>
        /// <param name="MuzzleVelocity">Muzzle velocity of the bullet in question.</param>
        /// <returns>Rpm of the bullet at the muzzle.</returns>
        public static double BulletRPM(double BarrelTwist, double MuzzleVelocity)
        {
            double lRTN = 0;
            lRTN = (12 / BarrelTwist) * MuzzleVelocity * 60;

            return lRTN;
        }
        /// <summary>
        /// The RPM of the bullet
        /// </summary>
        /// <param name="Range">Distance to return rpm at in yards.</param>
        /// <param name="MuzzleVelocity">Bullet velocity at the barrel muzzle.</param>
        /// <param name="BarrelTwist">Number of inches per twist of the rifling.</param>
        /// <param name="BulletDiameter">The diameter of the bullet in question.</param>
        /// <param name="Fo">Initual drag factor at muzzle.</param>
        /// <returns>Bullet RPM</returns>
        public static double SpinRate(double Range, double MuzzleVelocity, double BarrelTwist, double BulletDiameter, double Fo,
            double F2, double F3, double F4, double Zone1Range, double Zone1TransSpeed, double Zone1Slope, double Zone2Range,
            double Zone2TransSpeed, double Zone2Slope, double Zone3Range, double Zone3TransSpeed, double Zone3Slope, double Zone1SlopeMultiplier,
            double Zone3SlopeMultiplier, double Zone4Slope)
        {
            double lSR = 0; //Spin Rate
            double lFT;     //Flight time

            //MV x (12/twist rate in inches) x 60 = Bullet RPM
            lSR = MuzzleVelocity * (12.00 / BarrelTwist) * 60;
            lFT = FlightTime(Range, Fo, F2, F3, F4, MuzzleVelocity, Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range,
                Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier,
                Zone4Slope);

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
        #endregion

        #region "Drag Functions"
        /// <summary>
        /// Calculates an estimate for the BCg1 of a bullet with the beginning and ending 
        /// velocities provided over the distance provided.
        /// </summary>
        /// <param name="V1">Velocity (fps) at the beginning of the distance provided.</param>
        /// <param name="V2">Velocity (fps) at the end of the distance provided.</param>
        /// <param name="DistanceFt">Distance (ft) between the two velocity values.  Should be 300' for more.</param>
        /// <returns></returns>
        public static double CalculateBC(double V1, double V2, double DistanceFt)
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
        /// Calculate the drag/retard coefficient at the muzzle.
        /// </summary>
        /// <param name="Range_1">Distance (ft) from the muzzle to where the V1 speed is recorded.</param>
        /// <param name="V1">Velocity (fps) at Range_1 ft from the muzzle.  Should be measured close to the barrel</param>
        /// <param name="Range_2">Distance (ft) from the muzzle to where the V2 speed is recorded.</param>
        /// <param name="V2">Velocity (fps) at Range_2 ft from the muzzle.</param>
        /// <returns>Fo</returns>
        public static double CalculateFo(double Range_1, double V1, double Range_2, double V2, double Zone1TransSpeed,
            double Zone1Slope, double Zone1SlopeMultiplier, double Zone2TransSpeed, double Zone3TransSpeed, 
            double Zone3Slope, double Zone3SlopeMultiplier)
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
                    //Where R is in feet so we must convert yards to feet by dividing by 3.
                    lFo = (lF + Zone1SlopeMultiplier * Zone1Slope * (Range_1 / 3));                  
                }
                else if ((V1 < Zone1TransSpeed) & (V1 >= Zone2TransSpeed))
                {
                    //Zone 2

                    //F is constant in Zone 2, i.e. no slope N = 0.
                    lFo = lF;
                }
                else if ((V1 < Zone2TransSpeed) & (V1 >= Zone3TransSpeed))
                {
                    //Zone 3

                    //Fa=Fo-0.75*N*R
                    //F0 = Fa + Zone3SlopeMultiplier*N*R
                    //Where R is in feet so we must convert yards to feet by dividing by 3.
                    lFo = lF + Zone3SlopeMultiplier * Zone3Slope * (Range_1 / 3);
                    //F2 = lFo;
                    //Fo = F2;
                    //F3 = Fo - (Zone3SlopeMultiplier * Zone3Slope * Zone3Range);
                }
                else
                {
                    //Zone 4

                    //F is constant in Zone 4
                    lFo = lF;
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
        /// <param name="BCg1"></param>
        /// <param name="D2">Feet to calculate V2 at. D1 will be zero.</param>
        /// <returns></returns>
        public static double CalculateV2FromBC(double MuzzleVelocity, double BCg1, double D2)
        {
            double lc; double lR1; double lR2; double lV2; int lRtn = 0;

            if ((MuzzleVelocity == 0) || (BCg1 == 0))
            {
                lRtn = -1;
                return lRtn;
            }
            lR1 = 0;
            lR2 = D2;
            //BC = r/(r2 - r1)
            //r = C*2*(3600^0.5-(V^0.5))
            lc = 166;
            lR1 = (lc * 2) * (Math.Pow(3600, 0.5) - (Math.Pow(MuzzleVelocity, 0.5)));
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
            lV2 = Math.Pow(((((300 / BCg1) + lR1) / (lc * 2)) -
                (Math.Pow(3600, 0.5))), 2);
            return lV2;
        }
        /// <summary>
        /// Calculates the effective drag/retard coefficent for the provided range.
        /// </summary>
        /// <param name="Range">Distance to calculate the F value at.</param>
        /// <returns>F drag/retard coefficent</returns>
        public static double Fa(double Range, double Fo, double F2, double F3, double F4, double Zone1Range, double Zone2Range, double Zone3Range,
            double Zone1Slope, double Zone2Slope, double Zone3Slope, double Zone4Slope, double Zone1SlopeMultiplier, double Zone3SlopeMultiplier)
        {
            double lFa = 0;

            if (Zone1Range >= Range)
            {
                //F at the provided range.  Applies to Zone 1
                lFa = (Fo - Zone1SlopeMultiplier * Zone1Slope * Range);
            }
            else if ((Zone1Range < Range) &
                (Zone2Range >= Range))
            {
                //F at the provided range.  Applies to Zone 2
                if (Zone2Slope == 0)
                {
                    lFa = F2;
                }
                else
                {
                    lFa = (F2 - Zone2Slope * (Range-Zone1Range));
                }
            }
            else if ((Zone2Range < Range) &
                (Zone3Range >= Range))
            {
                //F at the provided range.  Applies to Zone 3
                lFa = (F3 - Zone3SlopeMultiplier * Zone3Slope * (Range - Zone3Range));
            }
            else if (Zone3Range < Range)
            {
                //F at the provided range.  Applies to Zone 4
                if (Zone4Slope == 0)
                {
                    lFa = F4;
                }
                else
                {
                    lFa = (F4 - Zone4Slope * (Range-Zone3Range));
                }
            }
            return lFa;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Range">Yards</param>
        /// <param name="Fo"></param>
        /// <param name="F2"></param>
        /// <param name="F3"></param>
        /// <param name="F4"></param>
        /// <param name="Zone1Range">Yards</param>
        /// <param name="Zone2Range">Yards</param>
        /// <param name="Zone3Range">Yards</param>
        /// <param name="Zone1Slope"></param>
        /// <param name="Zone3Slope"></param>
        /// <param name="Zone1SlopeMultiplier"></param>
        /// <param name="Zone3SlopeMultiplier"></param>
        /// <param name="BulletDiameter">inches</param>
        /// <returns></returns>
        public static double FdragCoefficient(double Range, double Fo, double F2, double F3, double F4, double Zone1Range,
            double Zone2Range, double Zone3Range, double Zone1Slope, double Zone2Slope, double Zone3Slope,
            double Zone4Slope, double Zone1SlopeMultiplier, double Zone3SlopeMultiplier)
        {
            double lRTN = 0;
            double lFa;

            lFa = Fa(Range, Fo, F2, F3, F4, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope, Zone3Slope, Zone4Slope,
                Zone1SlopeMultiplier, Zone3SlopeMultiplier);
            lRTN = 1 / lFa;


            return lRTN;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MuzzleVelocity">fps</param>
        /// <param name="Range">yrds</param>
        /// <param name="Fo"></param>
        /// <param name="F2"></param>
        /// <param name="F3"></param>
        /// <param name="F4"></param>
        /// <param name="Zone1Range">yrds</param>
        /// <param name="Zone2Range"></param>
        /// <param name="Zone3Range"></param>
        /// <param name="Zone1Slope"></param>
        /// <param name="Zone3Slope"></param>
        /// <param name="Zone1SlopeMultiplier"></param>
        /// <param name="Zone3SlopeMultiplier"></param>
        /// <param name="Zone1TransSpeed">fps</param>
        /// <param name="BulletDiameter">in</param>
        /// <param name="AirDensity">lb/ft3</param>
        /// <returns></returns>
        public static double CDdragCoefficient(double MuzzleVelocity, double Range, double Fo, double F2, double F3, double F4,
            double Zone1Range, double Zone2Range, double Zone3Range, double Zone1Slope, double Zone2Slope, double Zone3Slope,
            double Zone4Slope, double Zone1SlopeMultiplier, double Zone3SlopeMultiplier, double Zone1TransSpeed, double Zone2TransSpeed,
            double Zone3TransSpeed, double BulletDiameter, double BulletWeight, double AirDensity)
        {
            double lRTN = 0;
            double lFa;
            double lba;
            double lVelocity;
            double lw = BulletWeight / 7000;

            lVelocity = Velocity(MuzzleVelocity, Range, Zone1Range, Zone1TransSpeed, Fo, Zone1Slope, Zone1SlopeMultiplier,
                Zone2Range, Zone2TransSpeed, F2, Zone2Slope, Zone3Range, Zone3Slope, Zone3TransSpeed, Zone3SlopeMultiplier,
                F3, F4, Zone4Slope);
            //lVelocity = lVelocity * 3;
            lba = (Math.PI/4) * (Math.Pow(BulletDiameter/12, 2));
            lFa = Fa(Range, Fo, F2, F3, F4, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope, Zone3Slope, Zone4Slope,
                Zone1SlopeMultiplier, Zone3SlopeMultiplier);

            // K = (1/F)/(((pi/4)*d^2)*(p/w))
            lRTN = (1/lFa)/(lba*(AirDensity/lw));

            return lRTN;
        }
        #endregion

        #region "Horizontal Trajectory"
        public static double TotalHorizontalDrift(double Range, double WindSpeed, double WindDirection, double MuzzleVelocity, double Fo,
            LocationData TargetLoc, LocationData ShooterLoc, string TwistDirection, double BSG,double F2, double F3,
            double F4, double Zone1Range, double Zone1TransSpeed, double Zone1Slope, double Zone2Range, double Zone2TransSpeed,
            double Zone2Slope, double Zone3Range, double Zone3TransSpeed, double Zone3Slope, double Zone1SlopeMultiplier,
            double Zone3SlopeMultiplier, double Zone4Slope)
        {
            double lTW;

            lTW = GetCoriolisHoriz(Range, MuzzleVelocity, TargetLoc, ShooterLoc, Fo, F2,F3,F4,Zone1Range, Zone1TransSpeed,
                Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier,
                Zone3SlopeMultiplier, Zone4Slope) + 
                GetSpinDrift(Range, TwistDirection, BSG, Fo, MuzzleVelocity, F2, F3, F4, Zone1Range, Zone1TransSpeed,
                Zone1Slope, Zone2Range,Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier,
                Zone3SlopeMultiplier, Zone4Slope) + 
                WindDrift(WindSpeed, WindDirection, Range, Fo, MuzzleVelocity, F2, F3,F4,Zone1Range,Zone1TransSpeed, 
                Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier,
                Zone3SlopeMultiplier, Zone4Slope);

            return lTW;
        }
        /// <summary>
        /// Horizontal Coriolis affect at the provided range.
        /// </summary>
        /// <param name="Range">Distance to calculate Coriolis affect at.</param>
        /// <returns>Inches of horizontal drift caused by Coriolis addect.</returns>
        public static double GetCoriolisHoriz(double Range, double MuzzleVelocity, 
            LocationData TargetLoc, LocationData ShooterLoc, double Fo, double F2, double F3, double F4, double Zone1Range,
            double Zone1TransSpeed, double Zone1Slope, double Zone2Range, double Zone2TransSpeed, double Zone2Slope,
            double Zone3Range, double Zone3TransSpeed, double Zone3Slope, double Zone1SlopeMultiplier, 
            double Zone3SlopeMultiplier, double Zone4Slope)
        {
            double lCorVert = 0; double lCorHoriz = 0;
            int lR;

            lR = GetCoriolisComponents(ref lCorVert, ref lCorHoriz, Range, TargetLoc,
                ShooterLoc, Fo, F2,F3,F4, MuzzleVelocity, Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed,
                Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone4Slope);
            return lCorHoriz;
        }
        /// <summary>
        /// Returns the horizontal displacement related to the gyroscopic forces of the bullet.
        /// </summary>
        /// <param name="Range">The range to find the displacement for.</param>
        /// <returns>The horizontal displacement in inches related to the bullets gyroscopic force.</returns>
        public static double GetSpinDrift(double Range, string TwistDirection, double BSG, double Fo, double MuzzleVelocity,
            double F2, double F3, double F4, double Zone1Range, double Zone1TransSpeed, double Zone1Slope, double Zone2Range,
            double Zone2TransSpeed, double Zone2Slope, double Zone3Range, double Zone3TransSpeed, double Zone3Slope, double Zone1SlopeMultiplier,
            double Zone3SlopeMultiplier, double Zone4Slope)
        {
            double lDrift;

            lDrift = GetRawSpinDrift(Range, TwistDirection, BSG, Fo, MuzzleVelocity, F2,F3,F4,Zone1Range, Zone1TransSpeed,
                Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier,
                Zone3SlopeMultiplier, Zone4Slope);

            return lDrift;
        }
        public static double WindDrift(double WindSpeed, double WindDirection, double Range, double Fo, double MuzzleVelocity,
            double F2, double F3, double F4, double Zone1Range, double Zone1TransSpeed, double Zone1Slope, double Zone2Range,
            double Zone2TransSpeed, double Zone2Slope, double Zone3Range, double Zone3TransSpeed, double Zone3Slope,
            double Zone1SlopeMultiplier, double Zone3SlopeMultiplier, double Zone4Slope)
        {
            double lt;      //Flight time
            double lTF;     //Theoretical Flight time with no drag
            double lDT;     //Flight time due to drag
            double lWD;     //Wind direction in degrees
            double lCW;     //Cross wind magnitude
            double lWDR;    //Wind displacement rate
            double lHWD;    //Horizontal wind drift

            //Actual flight time
            lt = FlightTime(Range,Fo, F2, F3, F4, MuzzleVelocity, Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range,
                Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier,
                Zone4Slope);
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
        /// <param name="Fo">Initual drag factor at muzzle.</param>
        /// <param name="MuzzleVelocity">Bullet velocity at the barrel muzzle.</param>
        /// <returns>Horizontal displacement in inches</returns>
        public static double WindDrift(double WindSpeed, uint Clock, double Range, double Fo, double MuzzleVelocity, double F2,
            double F3, double F4, double Zone1Range, double Zone1TransSpeed, double Zone1Slope, double Zone2Range,
            double Zone2TransSpeed, double Zone2Slope, double Zone3Range, double Zone3TransSpeed, double Zone3Slope,
            double Zone1SlopeMultiplier, double Zone3SlopeMultiplier, double Zone4Slope, double ZeroRange = 0)
        {
            double lDegP = 360 / 12;       //Degrees per clock reading
            double lWD = lDegP * Clock;  //Total degrees

            return WindDrift(WindSpeed, lWD, Range, Fo, MuzzleVelocity, F2, F3, F4, Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range,
                Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier,
                Zone3SlopeMultiplier, Zone4Slope);

        }
        #endregion

        #region "Other Flight Characteristics"
        /// <summary>
        /// Calculates a bullets enery with the given weight at the provided range.
        /// </summary>
        /// <param name="Weight">Bullet weight (grns).</param>
        /// <param name="Range">Distance to calculate energy at (yrds).</param>
        /// <returns>Bullet energy (ft.lb.)</returns>
        public static double Energy(double Weight, double Range, double MuzzleVelocity,double Zone1Range, double Zone1TransSpeed,
            double Fo, double Zone1Slope, double Zone1SlopeMultiplier, double Zone2Range, double Zone2TransSpeed, double F2, 
            double Zone2Slope, double Zone3Range, double Zone3Slope, double Zone3TransSpeed, double Zone3SlopeMultiplier,
            double F3, double F4, double Zone4Slope)
        {
            double lE;

            //E=V^2Wt/450380 ftlb
            lE = (Math.Pow(Velocity(MuzzleVelocity, Range, Zone1Range, Zone1TransSpeed, Fo, Zone1Slope, Zone1SlopeMultiplier, Zone2Range, 
                Zone2TransSpeed, F2, Zone2Slope, Zone3Range, Zone3Slope, Zone3TransSpeed, Zone3SlopeMultiplier, F3, F4, Zone4Slope), 2) * Weight) / 450380;
            return lE;
        }
        /// <summary>
        /// Calculates a bullets enery with the given weight at the provided velocity.
        /// </summary>
        /// <param name="Weight">Bullet weight (grns).</param>
        /// <param name="Velocity">Bullet speed (fps)</param>
        /// <returns>Bullet energy (ft.lb.)</returns>
        public static double Energy(double Weight, double Velocity)
        {
            double lE;

            //E = V ^ 2Wt / 450380 ftlb
            lE = (Math.Pow(Velocity, 2) * Weight) / 450380;
            return lE;
        }
        /// <summary>
        /// Estimated Ballistic Coeficient
        /// </summary>
        /// <param name="Weight">Bullet weight in grains</param>
        /// <param name="Diameter">Bullet diameter</param>
        /// <param name="Shape">Shape type of bullet</param>
        /// <returns>BC</returns>
        public static double EstimateBC(double Weight, double Diameter, BulletShapeEnum Shape)
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
        /// Calculates the bullet flight time to the provided range.
        /// </summary>
        /// <param name="Range">Distance (yrds) to calculate the flight time for.</param>
        /// <param name="Fo">Initual drag factor at muzzle.</param>
        /// <param name="MuzzleVelocity">Bullet velocity at the barrel muzzle.</param>
        /// <returns>Bullet flight time in seconds to reach the provided range.</returns>
        public static double FlightTime(double Range, double Fo, double F2, double F3, double F4, double MuzzleVelocity, 
            double Zone1Range, double Zone1TransSpeed, double Zone1Slope, double Zone2Range, double Zone2TransSpeed, 
            double Zone2Slope, double Zone3Range, double Zone3TransSpeed, double Zone3Slope, 
            double Zone1SlopeMultiplier, double Zone3SlopeMultiplier, double Zone4Slope)
        {
            double lT;
            double lV;
            double lDeltT;
            double lStartT;

            try
            {

                if ((Zone2Range >= Range) & (Range > Zone1Range))
                {
                    //Zone 2

                    //Get precise velocity at transition.
                    lV = Velocity(MuzzleVelocity, Zone1Range, Zone1Range, Zone1TransSpeed, Fo, Zone1Slope, Zone1SlopeMultiplier,
                        Zone2Range, Zone2TransSpeed, F2, Zone2Slope, Zone3Range, Zone3Slope, Zone3TransSpeed,
                        Zone3SlopeMultiplier, F3, F4, Zone4Slope);

                    //Get time of flight at transition
                    lDeltT = FltTimeZ1(Zone1Range,MuzzleVelocity, Fo, Zone1Slope);

                    //Get time of flight useing zone 2 calculation
                    lStartT = FltTimeZ2((Zone1Range+1), Zone1TransSpeed, F2);

                    //Remove delta from zone 2 calculation for zone 1.
                    lDeltT = lStartT - lDeltT;

                    //Get flight time for current range.
                    lT = FltTimeZ2(Range, Zone1TransSpeed, F2);

                    //Remove Zone 2 time calculation for first zone.
                    lT = lT - lDeltT;
                }
                else if ((Zone3Range >= Range) & (Range > Zone2Range))
                {
                    //Zone 3


                    lStartT = FlightTime(Zone2Range, Fo, F2, F3, F4, MuzzleVelocity, Zone1Range, Zone1TransSpeed, Zone1Slope,
                        Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier,
                        Zone3SlopeMultiplier, Zone4Slope);

                    lDeltT = FltTimeZ3((Zone2Range+1), Zone2TransSpeed, F3, Zone3Slope);

                    lDeltT = lDeltT - lStartT;

                    lT = FltTimeZ3(Range, Zone2TransSpeed, F3, Zone3Slope);

                    lT = lT - lDeltT;
                }
                else if (Zone1Range >= Range)
                {
                    //Zone 1

                    //T = 3R / Vo / (1 - (3R / 2Fo) + (1 - 2N)/ 130)
                    lT = (3 * Range) / MuzzleVelocity / (1 - ((3 * Range) / (2 * Fo)) + ((1 - 2 * Zone1Slope) / 130));
                }
                else
                {
                    //Zone 4

                    lStartT = FlightTime(Zone3Range, Fo, F2, F3, F4, MuzzleVelocity, Zone1Range, Zone1TransSpeed, Zone1Slope,
                        Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier,
                        Zone3SlopeMultiplier, Zone4Slope);

                    lDeltT = FltTimeZ4((Zone3Range + 1), Zone3TransSpeed, F4);

                    lDeltT = lDeltT - lStartT;


                    //T = Fo / (Vo(exp(3R / Fo) - 1)
                    lT = FltTimeZ4(Range, Zone3TransSpeed, F4);

                    lT = lT - lDeltT;
                }

                return lT;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Maximum distance the bullet/rifle combination can achieve.
        /// </summary>
        /// <returns></returns>
        public static double MaxRange(double Fo)
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
            lMR = lMR * .9;
            return lMR;
        }
        public static double MuzzleVelocity(double Velocity_1, double Range_1, double Fo, double Zone1Slope, double Zone1TransSpeed,
            double Zone2TransSpeed, double Zone3Slope, double Zone3TransSpeed)
        {
            double lRTN = 0;

            if (Velocity_1 >= Zone1TransSpeed)
            {
                //Zone 1
                //V  = Vo(1-3RN/Fo)^2
                //We must convert the provided range from feet to yards
                // Solving for Vo we get the solution below.
                // MuzzleVelocity = V1 / ((1 - (Range_1 / 3) * Zone1Slope / Fo) ^ (1 / Zone1Slope))
                lRTN = Velocity_1 / (Math.Pow((1 - (Range_1 / 3) * Zone1Slope / Fo), (1 / Zone1Slope)));
            }
            else if ((Velocity_1 < Zone1TransSpeed) & (Velocity_1 >= Zone2TransSpeed))
            {
                //Zone 2
                //Vo = V(exp(-3R/F))
                //Where R is in feet so we must convert yards to feet by dividing by 3.
                lRTN = Velocity_1 / (Math.Exp((-3 * ((Range_1 / 3) / Fo))));
            }
            else if ((Velocity_1 < Zone2TransSpeed) & (Velocity_1 >= Zone3TransSpeed))
            {
                //Zone 3
                //V  = Vo(1-3RN/Fo)^2
                //We must convert the provided range from feet to yards
                //Solving for Vo we get the solution below.
                lRTN = Velocity_1 / (Math.Pow((1 - (Range_1 / 3) * Zone3Slope / Fo), (1 / Zone3Slope)));
            }
            else
            {
                //Zone 4
                lRTN = Velocity_1 / (Math.Exp((-3 * ((Range_1 / 3) / Fo))));
            }

                return lRTN;
        }
        /// <summary>
        /// Caclulates the bullet velocity at the provided range.
        /// </summary>
        /// <param name="Range">The range to calculate the velocity at.</param>
        /// <returns>Bullet velocity (fps)</returns>
        public static double Velocity(double MuzzleVelocity, double Range, double Zone1Range, double Zone1TransSpeed, double Fo,
            double Zone1Slope, double Zone1SlopeMultiplier, double Zone2Range, double Zone2TransSpeed, double F2, double Zone2Slope,
            double Zone3Range, double Zone3Slope, double Zone3TransSpeed, double Zone3SlopeMultiplier, double F3, double F4, double Zone4Slope)
        {
            double lVd;
            double lFa;
            double lVo;

            if ((Zone2Range >= Range) & (Range > Zone1Range))
            {
                //'    'Zone 2
                //'
                //'    lF = F2
                //'    'V = Vo(exp(-3R/F))
                lFa = Fa(Range, Fo, F2, F3, F4, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope, Zone3Slope, Zone4Slope,
                    Zone1SlopeMultiplier, Zone3SlopeMultiplier);
                lVo = Velocity(MuzzleVelocity, Zone1Range, Zone1Range, Zone1TransSpeed, Fo, Zone1Slope, Zone1SlopeMultiplier,
                    Zone2Range, Zone2TransSpeed, F2, Zone2Slope, Zone3Range, Zone3Slope, Zone3TransSpeed, 
                    Zone3SlopeMultiplier, F3, F4, Zone4Slope);
                lVd = lVo * Math.Exp(((-3 * (Range - Zone1Range)) / F2));
            }
            else if ((Zone3Range >= Range) & (Range > Zone2Range))
            {
                //'    'Zone 3
                //'
                //'    lF = F3
                //'    'V = Vo(1-3R*N/Fo)^(1/N)
                lFa = Fa(Range, Fo, F2, F3, F4, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope, Zone3Slope,
                    Zone4Slope, Zone3SlopeMultiplier, Zone3SlopeMultiplier);
                lVo = Velocity(MuzzleVelocity, Zone2Range, Zone1Range, Zone1TransSpeed, Fo, Zone1Slope, Zone1SlopeMultiplier,
                    Zone2Range, Zone2TransSpeed, F2, Zone2Slope, Zone3Range, Zone3Slope, Zone3TransSpeed, 
                    Zone3SlopeMultiplier, F3, F4, Zone4Slope);

                lVd = lVo * Math.Pow((1 - ((3 * ((Range-Zone2Range)* Zone3Slope))) / F3), (1 / Zone3Slope));

            }
            else if (Zone1Range >= Range)
            {
                //      Zone 1

                //      V = Vo(1-3R*N/Fo)^(1/N)
                lVd = MuzzleVelocity * Math.Pow((1 - ((3 * Range*Zone1Slope) / Fo)), (1 / Zone1Slope));
            }
            else
            {
                //'    'Zone 4
                //'
                //'    'V = Vo(exp(-3R/F))
                lFa = Fa(Range, Fo, F2, F3, F4, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope, Zone3Slope,
                    Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier);
                lVo = Velocity(MuzzleVelocity, Zone3Range, Zone1Range, Zone1TransSpeed, Fo, Zone1Slope, Zone1SlopeMultiplier,
                    Zone2Range, Zone2TransSpeed, F2, Zone2Slope, Zone3Range, Zone3Slope, Zone3TransSpeed, Zone3SlopeMultiplier,
                    F3, F4, Zone4Slope);

                lVd = lVo * Math.Exp((-3 * (Range-Zone3Range) / lFa));
            }

            return lVd;
        }
        /// <summary>
        /// Midrange where PBR height (Hm) or maximum rise occurs
        /// </summary>
        public static double MidRange(double MuzzleVelocity, double MaxRise, double ScopeHeight, double Fo)
        {
            double lMR;

            //M = 1/(((G/Vo)/((Hm+S)^0.5)) + 2/Fo)
            lMR = 1 / (((G / MuzzleVelocity) / (Math.Pow((MaxRise + ScopeHeight), 0.5))) + 2 / Fo); ;
            return lMR;
        }
        /// <summary>
        /// Calculate Point Blank Range Distance
        /// </summary>
        public static double PointBlankRange(double MaxRise, double MuzzleVelocity, double ScopeHeight, double Fo)
        {
            double lSQ;
            double lP;
            double lMR;

            lMR = BallisticFunctions.MidRange(MuzzleVelocity, MaxRise, ScopeHeight, Fo);


            //P = (1 + SQ)/(1/Fo + SQ/M)
            //SQ  = SH/2^0.5
            lSQ = SH(MaxRise,ScopeHeight) / (Math.Pow(2, 0.5));
            lP = (1 + lSQ) / ((1 / Fo) + (lSQ / lMR));

            return lP;
        }
        /// <summary>
        /// The near range where the bullet crosses the sight plane on the way to the far zero vertical deviation.
        /// </summary>
        public static double ZeroNearRange(double MaxRise, double ScopeHeight, double MuzzleVelocity, double Fo)
        {
            double lNZ;
            double lMR;

            lMR = BallisticFunctions.MidRange(MuzzleVelocity, MaxRise, ScopeHeight, Fo);
            //Zn = (1 - SH) / (1 / Fo - SH / M)
            lNZ = (1 - SH(MaxRise, ScopeHeight)) / ((1 / Fo) - (SH(MaxRise, ScopeHeight) / lMR));

            return lNZ;
        }
        #endregion

        #region "Vertical Trajectory"
        /// <summary>
        /// The range to target corrected for incline.  Line-of-sight distance will be greater than the distance affected
        /// by gravity.  The distance to shoot for is the range affected by gravity and drag which is returned by this function.
        /// </summary>
        /// <param name="ShotAngle">The angle absolute value of the shot angle.</param>
        /// <param name="LOSrange">The line-of-sight (LOS) range.</param>
        /// <returns></returns>
        public static double AngleCompRange(double ShotAngle, double LOSrange, double MuzzleVelocity, double Range, double Zone1Range,
            double Zone2Range, double Zone3Range, double Zone1Slope, double Zone1SlopeMultiplier, double Zone1AngleFactor,
            double Zone2Slope, double Zone3Slope, double Zone4Slope, double Zone3SlopeMultiplier, double Zone1TransSpeed,
            double Zone2TransSpeed, double Zone3TransSpeed, double Fo, double F2, double F3, double F4, double DensityAlt,
            double DensityAltAtZero, LocationData TargetLoc, LocationData ShooterLoc)
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
            lLOSD = MuzzleDrop(MuzzleVelocity, LOSrange, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope, Zone3Slope,
                       Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed,
                        Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, TargetLoc, ShooterLoc);
            lTDA = 90 - ShotAngle;
            lTDD = Math.Abs(Math.Sin(lTDA * (lPi / 180)) * lLOSD);
            lDID = Math.Abs(lLOSD);
            lDIR = LOSrange - 0.25;
            //Iterate drop function to find the range that equals the corrected drop distance.
            while (lDID > lTDD)
            {
                lDID = Math.Abs(MuzzleDrop(MuzzleVelocity, lDIR, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope, Zone3Slope, 
                        Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed, 
                        Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero,TargetLoc, ShooterLoc));
                lDIR = lDIR - 0.5;
            }
            return lDIR;
        }
        /// <summary>
        /// Calculates the maximum bullet rise on the way to the zero range.
        /// </summary>
        public static double CalculateHm(double Range, double ZeroRange, double ScopeHeight, double MuzzleVelocity, double Zone1Range,
            double Zone2Range, double Zone3Range, double Zone1Slope, double Zone2Slope, double Zone3Slope, double Zone4Slope,
            double Zone1SlopeMultiplier, double Zone3SlopeMultiplier, double Zone1AngleFactor, double Zone1TransSpeed, 
            double Zone2TransSpeed, double Zone3TransSpeed, double Fo, double F2, double F3, double F4, double DensityAlt,
            double DensityAltAtZero, LocationData ZeroTargetLoc, LocationData ZeroShooterLoc, LocationData TargetLoc,
            LocationData ShooterLoc)
        {
            //Must iterate flight path to find Hm
            double ld;
            double lInter;
            double lTmpH;
            double lR = 0;
            double lH;

            ld = MuzzleDrop(MuzzleVelocity, ZeroRange, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope,
                Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed,
                Zone2TransSpeed, Zone3TransSpeed, Fo, F2,F3,F4,DensityAlt, DensityAltAtZero, TargetLoc,ShooterLoc);
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
                lH = (lInter * (1 / (ZeroRange / 2)) * lR) - ( -
                    MuzzleDrop(MuzzleVelocity, lR, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope, Zone3Slope,
                    Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, 
                    Zone2TransSpeed, Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, TargetLoc, ShooterLoc)
                    + ScopeHeight);
                if (lR > ZeroRange)
                {
                    break;
                }
            }
            return lTmpH;
        }
        /// <summary>
        /// The bullet drop from the muzzle of the barrel to include Coriolis vertical component.
        /// </summary>
        /// <param name="MuzzleVelocity"></param>
        /// <param name="Range"></param>
        /// <param name="Zone1Range"></param>
        /// <param name="Zone2Range"></param>
        /// <param name="Zone3Range"></param>
        /// <param name="Zone1Slope"></param>
        /// <param name="Zone1AngleFactor"></param>
        /// <param name="Zone1TransSpeed"></param>
        /// <param name="Zone2TransSpeed"></param>
        /// <param name="Zone3TransSpeed"></param>
        /// <param name="Fo"></param>
        /// <param name="F2"></param>
        /// <param name="F3"></param>
        /// <param name="DensityAlt">Density Altitude of the current location and atmospheric conditions.</param>
        /// <param name="DensityAltAtZero">Density Altitude for the zero location and atmospheric conditions.</param>
        /// <param name="ZeroTargetLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="ZeroShooterLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="TargetLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="ShooterLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="ZeroRange">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="RawMagnatude"></param>
        /// <returns>Muzzle drop distance in inches.</returns>
        public static double MuzzleDrop(double MuzzleVelocity, double Range, double Zone1Range, double Zone2Range, double Zone3Range,
            double Zone1Slope, double Zone2Slope, double Zone3Slope, double Zone4Slope, double Zone1SlopeMultiplier, 
            double Zone3SlopeMultiplier, double Zone1AngleFactor, double Zone1TransSpeed, double Zone2TransSpeed, 
            double Zone3TransSpeed, double Fo, double F2, double F3, double F4, double DensityAlt, double DensityAltAtZero,
            LocationData TargetLoc, LocationData ShooterLoc, bool RawMagnatude = false)
        {
            double ld = 0;      //Temporary drop accumulation variable.
            double lD1 = 0;     //Component 1 of 2 drop distance in zone 1
            double lD1p = 0;    //Component 2 of 2 drop distance in zone 1 (Accounts for downward angle of bullet as a result of zone 1 arc for remaining distance.)
            double lD1t = 0;
            double lD2 = 0;     //Component 1 of 2 drop distance in zone 2
            double lD2p = 0;    //Component 2 of 2 drop distance in zone 2 (Accounts for downward angle of bullet as a result of zone 2 arc for remaining distance.)
            double lD3 = 0;     //Component 1 of 2 drop distance in zone 3
            double lD3p = 0;    //Component 2 of 2 drop distance in zone 3 (Accounts for downward angle of bullet as a result of zone 3 arc for remaining distance.)
            double lD4;         //Component 1 of 1 drop distance in zone 4
            double lFa;         //Drag/Retard coefficent for a specific range distance.

            if (Range <= Zone1Range)
            {
                //Zone 1

                //    'D = (G/Vo/(1/R - 1/Fa))^2
                lD1 = Math.Pow(((BallisticFunctions.G / MuzzleVelocity) / ((1 / Range) - (1 /
                    DensityAltCorrection(Fa(Range, Fo, F2, F3,F4,Zone1Range, Zone2Range, Zone3Range, Zone1Slope,
                    Zone2Slope, Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier ),DensityAlt,DensityAltAtZero)))), 2);
                ld = lD1;
            }
            else if ((Range > Zone1Range) & (Range <= Zone2Range))
            {
                //Zone 2
                if (Zone1Range > 0)
                {
                    //D = (G/Vo/(1/R - 1/Fa))^2
                    lD1 = MuzzleDrop(MuzzleVelocity, Zone1Range, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope,
                        Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, 
                        Zone1TransSpeed, Zone2TransSpeed, Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, 
                        TargetLoc, ShooterLoc, true);

                    //D' = 3gFo(Rt - R)((1 - 3NR / Fo) ^ (1 - 2 / N))/ Vo ^ 2 / (2 - N)
                    //   Corrected to 3gFo(Rt-R)((1+3NR/Fo)^((2/N)-1))/Vo^2/(2-N)
                    //lD1p = lD1p * (Math.Pow(((1 + (3 * Zone1Slope * Zone1Range) / DensityAltCorrection(Fo))), ((2 / Zone1Slope) - 1)));
                    //   Updated to original formula with correction factor 0.696.  First mod moved incorrectly with F.
                    lD1p = 3 * BallisticFunctions.GravIn * DensityAltCorrection(Fo, DensityAlt, DensityAltAtZero) *
                        (Range - Zone1Range);
                    lD1p = lD1p * (Math.Pow((1 - ((3 * Zone1Slope *
                        Zone1Range) /
                        DensityAltCorrection(Fo, DensityAlt, DensityAltAtZero))),
                        (1 - (2 / Zone1Slope))));
                    lD1p = lD1p / (Math.Pow(MuzzleVelocity, 2));
                    lD1p = lD1p / (2 - Zone1Slope);
                    lD1p = lD1p * Zone1AngleFactor;
                }
                //    D = (G/Vo/(1/R - 1/Fa))^2

                //Old equation with slope fixed at 0.
                //lD2 = Math.Pow(((BallisticFunctions.G / Zone1TransSpeed) /
                    //((1 / (Range - Zone1Range)) -
                    //(1 / DensityAltCorrection(F2, DensityAlt, DensityAltAtZero)))), 2);

                lD2 = Math.Pow(((BallisticFunctions.G / Zone1TransSpeed) / ((1 / (Range - Zone1Range)) - (1 /
                   DensityAltCorrection(Fa(Range, Fo, F2, F3, F4, Zone1Range, Zone2Range, Zone3Range, Zone1Slope,
                   Zone2Slope, Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier), DensityAlt, DensityAltAtZero)))), 2);


                ld = lD1 + lD1p + lD2;
            }
            else if ((Range > Zone2Range) & (Range <= Zone3Range))
            {
                //Zone 3

                if (Zone2Range > 0)
                {
                    lD1t = MuzzleDrop(MuzzleVelocity, Zone2Range, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope,
                        Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, 
                        Zone1TransSpeed, Zone2TransSpeed, Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, 
                        DensityAltAtZero,TargetLoc, ShooterLoc, true);
                    //D = (G/Vo/(1/R - 1/Fa))^2
                    lD2 = Math.Pow(((BallisticFunctions.G / Zone1TransSpeed) /
                        ((1 / (Zone2Range - Zone1Range)) -
                        (1 / DensityAltCorrection(F2, DensityAlt, DensityAltAtZero)))), 2);
                    //D2' = 2D2(Rt - R1 - R2) / R2 / (1 - R2 / F)
                    lD2p = (2 * lD2) * (Range - Zone2Range) /
                        (Zone2Range - Zone1Range) /
                        (1 - ((Zone2Range - Zone1Range) /
                        DensityAltCorrection(F2, DensityAlt, DensityAltAtZero)));
                }
                lFa = DensityAltCorrection(F2, DensityAlt, DensityAltAtZero) +
                    (DensityAltCorrection(F3, DensityAlt, DensityAltAtZero) -
                    DensityAltCorrection(F2, DensityAlt, DensityAltAtZero)) / 4.000;
                //D = (G/Vo/(1/R - 1/Fa))^2
                lD3 = Math.Pow(((BallisticFunctions.G / Zone2TransSpeed) /
                    ((1.00 / (Range - Zone2Range)) - (1.00 / lFa))), 2);
                ld = lD1t + lD2p + lD3;
            }
            else if (Range > Zone3Range)
            {
                if (Zone3Range > 0)
                {
                    lD3 = MuzzleDrop(MuzzleVelocity, Zone3Range, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope,
                        Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier,  Zone1AngleFactor, 
                        Zone1TransSpeed, Zone2TransSpeed, Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero,
                        TargetLoc, ShooterLoc, true);
                    lFa = DensityAltCorrection(F2, DensityAlt, DensityAltAtZero) + (DensityAltCorrection(F3, DensityAlt, DensityAltAtZero) -
                        DensityAltCorrection(F2, DensityAlt, DensityAltAtZero)) / 4.000;
                    //D3' = gF3(Rt - (R1 + R2 + R3)) * 3((1 + 12 * R3 / F2) ^ (3 / 2) - 1) / 6 / (Zone2TransSpeed) ^ 2
                    lD3p = BallisticFunctions.GravIn * DensityAltCorrection(F3, DensityAlt, DensityAltAtZero) * (Range - Zone3Range) * 3 *
                        (Math.Pow((1 + 12 * (Zone3Range - Zone2Range) / lFa), (3 / 2)) - 1) / 6 / Math.Pow(Zone2TransSpeed, 2);
                }
                //D = (G/Vo/(1/R - 1/Fa))^2

                //Old equation with slope fixed at 0.
                //lD4 = Math.Pow((BallisticFunctions.G / Zone3TransSpeed /
                //    ((1.00 / (Range - Zone3Range) -
                //    1 / DensityAltCorrection(F3, DensityAlt, DensityAltAtZero)))), 2);

                lD4 = Math.Pow(((BallisticFunctions.G / Zone3TransSpeed) / ((1 / (Range - Zone3Range)) - (1 /
                   DensityAltCorrection(Fa(Range, Fo, F2, F3, F4, Zone1Range, Zone2Range, Zone3Range, Zone1Slope,
                   Zone2Slope, Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier), DensityAlt, DensityAltAtZero)))), 2);


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
            ld = ld + GetCoriolisVert(Range, TargetLoc, ShooterLoc, Fo, MuzzleVelocity,F2,F3,F4,Zone1Range, Zone1TransSpeed,
                Zone1Slope, Zone2Range, Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier,
                Zone3SlopeMultiplier, Zone4Slope);

            return ld;
        }
        /// <summary>
        /// Vertical Coriolis affect at the provided range.
        /// </summary>
        /// <param name="Range">Distance to calculate Coriolis affect at.</param>
        /// <param name="ZeroTargetLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="ZeroShooterLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="TargetLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="ShooterLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="ZeroRange">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="Fo">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="MuzzleVelocity">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <returns>Inches of rise or drop caused by Coriolis addect.</returns>
        public static double GetCoriolisVert(double Range, LocationData TargetLoc, LocationData ShooterLoc, 
            double Fo, double MuzzleVelocity, double F2, double F3, double F4, double Zone1Range,
            double Zone1TransSpeed, double Zone1Slope, double Zone2Range, double Zone2TransSpeed,
            double Zone2Slope, double Zone3Range, double Zone3TransSpeed, double Zone3Slope, double Zone1SlopeMultiplier,
            double Zone3SlopeMultiplier, double Zone4Slope)
            {
                double lCorVert = 0; double lCorHoriz = 0;
                int lR;

                lR = GetCoriolisComponents(ref lCorVert, ref lCorHoriz, Range, TargetLoc, ShooterLoc,
                    Fo, F2,F3,F4, MuzzleVelocity,Zone1Range,Zone1TransSpeed, Zone1Slope, Zone2Range, Zone2TransSpeed,
                    Zone2Slope, Zone3Range, Zone3TransSpeed,Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier,
                    Zone4Slope);

                return lCorVert;
            }
        /// <summary>
        /// The bullet vertical location with respect to the sight plane.
        /// </summary>
        /// <param name="Range">The range to return the bullet location vs sight plane at.</param>
        /// <returns>Inches above or below the sight plane.</returns>
        public static double SightDelta(double Range, double ZeroRange, double ScopeHeight, double ZeroMuzzleVelocity, double MuzzleVelocity, double Zone1Range,
            double Zone2Range, double Zone3Range, double Zone1Slope, double Zone2Slope, double Zone3Slope, double Zone4Slope,
            double Zone1SlopeMultiplier, double Zone3SlopeMultiplier, double Zone1AngleFactor, double Zone1TransSpeed,
            double Zone2TransSpeed, double Zone3TransSpeed, double Fo, double F2, double F3, double F4, double DensityAlt,
            double DensityAltAtZero, LocationData ZeroTargetLoc, LocationData ZeroShooterLoc, LocationData TargetLoc,
            LocationData ShooterLoc)
        {
            double lH;
            double lM;
            double lSD;

            //Bullet vertical location relative to scope sight line.   
            
            //Calculating sight in drop to define sight line.  We do not add coriolis correction as it is accounted for in
            //  the zero drop compenstaion.  Any change in shot direction will result in the appropriate delta from zero direction
            //  when the shot muzzle drop is calculated.
            lH = (-MuzzleDrop(ZeroMuzzleVelocity, ZeroRange, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope,
                Zone3Slope, Zone4Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier,Zone1AngleFactor, Zone1TransSpeed,
                Zone2TransSpeed, Zone3TransSpeed, Fo, F2,F3,F4, DensityAltAtZero, DensityAltAtZero, ZeroTargetLoc,
                ZeroShooterLoc) + ScopeHeight);
            //Sight line slope.
            lM = (lH / 2) * (1 / (ZeroRange / 2));
            lSD = ((Range * lM) + 
                MuzzleDrop(MuzzleVelocity, Range, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone2Slope, Zone3Slope,
                Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed,
                Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, TargetLoc, ShooterLoc) - ScopeHeight);

            return lSD;
        }
        /// <summary>
        /// Calculates the range where the vertical travel of the bullet and sight plane equal with a bullet
        /// travel that rises above the sight plan by exactly Hm.
        /// </summary>
        /// <param name="Hm">The maximum rise above the sight plan between the muzzle and zero range.</param>
        /// <returns>Zero Range (yrds)</returns>
        public static double CalculateZeroRange(double Fo, double MuzzleVelocity, double ZeroMaxRise, double ScopeHeight)
        {
            double lZ;

            if (SH(ZeroMaxRise, ScopeHeight) == 0) return 0;
            //Z = (1+SH)/(1/Fo + SH/M)
            lZ = (1 + SH(ZeroMaxRise,ScopeHeight)) / ((1 / Fo) + (SH(ZeroMaxRise, ScopeHeight) / 
                MidRange(MuzzleVelocity, ZeroMaxRise,ScopeHeight,Fo)));

            return lZ;
        }
        #endregion

        #region "Private Members"
        /// <summary>
        /// Calculates the drag adjustment for the change in air density.
        /// </summary>
        /// <param name="RetardCoef">The drag coefficent to adjust.</param>
        /// <param name="DensityAlt">Density Altitude of the current location and atmospheric conditions.</param>
        /// <param name="DensityAltAtZero">Density Altitude for the zero location and atmospheric conditions.</param>
        /// <returns>Corrected drag/retard coefficent.</returns>
        private static double DensityAltCorrection(double RetardCoef, double DensityAlt, double DensityAltAtZero)
        {
            double lFp;

            lFp = RetardCoef + RetardCoef * (DensityAlt - DensityAltAtZero) / 29733;
            return lFp;
        }
        /// <summary>
        /// Calculates the horizontal and vertical devation of the flight path due to the Coriolis effect.
        /// </summary>
        /// <param name="VerticalComponent">Vertical drop or rise (in.) caused by the Coriolis effect.</param>
        /// <param name="HorizontalComponent">Horizontal left or right drift (in.) caused by the Coriolis effect.</param>
        /// <param name="Range">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="ZeroTargetLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="ZeroShooterLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="TargetLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="ShooterLoc">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="ZeroRange">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="Fo">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <param name="MuzzleVelocity">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <returns>A routine trouble code and the horizontal and vertical effect of the earths rotation.</returns>
        private static int GetCoriolisComponents(ref double VerticalComponent, ref double HorizontalComponent, double Range,
            LocationData TargetLoc, LocationData ShooterLoc, double Fo, double F2, double F3, double F4, double MuzzleVelocity,
            double Zone1Range, double Zone1TransSpeed, double Zone1Slope, double Zone2Range, double Zone2TransSpeed,
            double Zone2Slope, double Zone3Range, double Zone3TransSpeed, double Zone3Slope, double Zone1SlopeMultiplier,
            double Zone3SlopeMultiplier, double Zone4Slope)
        {
            double lLonDist; double lLatDist; double lVertMultiplier;
            double lHorizMultiplier; double lFT; double lCorMag; double lSVM; double lSHM;
            double lCorVert; double lCorHorz; double lLonSign; double lLatSign;


            if ((ShooterLoc.Latitude == 0) || (ShooterLoc.Longitude == 0)
                || (TargetLoc.Latitude == 0) || (TargetLoc.Longitude == 0))
            {
                HorizontalComponent = 0;
                VerticalComponent = 0;
                return -1;
            }

            lFT = FlightTime(Range, Fo, F2, F3, F4, MuzzleVelocity, Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range,
                Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier,
                Zone4Slope);
           // lFTZ = FlightTime(ZeroRange, Fo, MuzzleVelocity);
            lCorMag = 0.00262 * lFT * Range;
            //lCorZero = 0.00262 * lFTZ * ZeroRange;

            //______________ Zero bias ___________________________
            //  Solve for the amount of Coriolis correction added during zero, this is
            //  like the sight line in that it is linear with distance and zero at the zero ranges.

            ////Longitudal distance to target
            //lLonDistZ = (ZeroTargetLoc.Longitude - ZeroShooterLoc.Longitude) * YardsPerDegLatLon;
            //lLonSignZ = Math.Sign(lLonDistZ);  // + = E, - = W
            //if (lLonSignZ == 0) lLonSignZ = 1;
            ////Latitudal distance to target
            //lLatDistZ = (ZeroTargetLoc.Latitude - ZeroShooterLoc.Latitude) * YardsPerDegLatLon;
            //lLatSignZ = Math.Sign(lLatDistZ);  // + = N, - = S
            //if (lLatSignZ == 0) lLatSignZ = 1;
            ////Correction factors for location on earth, the larger the lat the less vertical affect and more horizontal,
            ////     convert degrees to radians
            //lZeroVertMultiplier = Math.Cos(ZeroShooterLoc.Latitude * (Math.PI / 180));
            //lZeroHorizMultiplier = 1 - lZeroVertMultiplier;
            ////Vertical correction factor for Lat/Lon angle
            //if (lLonDistZ == 0)
            //{
            //    lZSVM = 0;
            //}
            //else
            //{
            //    lZSVM = Math.Atan(Math.Abs(lLatDistZ / lLonDistZ));  //Radians of shot angle
            //    lZSVM = Math.Cos(lZSVM);
            //}
            //lZSHM = 1 - lZSVM;
            //if (lLatDistZ == 0) lZSHM = 0;

            //lZSVM = lZSVM * lLonSignZ;
            //lZSHM = lZSHM * lLatSignZ;

            //lCorZVS = (lCorZero * lZeroVertMultiplier * lZSVM) / ZeroRange;
            //lCorZHS = (lCorZero * lZeroHorizMultiplier * lZSHM) / ZeroRange;

            //Positive = high, East
            //Negative = low, west

            //Longitudal distance to target
            lLonDist = (TargetLoc.Longitude - ShooterLoc.Longitude) * YardsPerDegLatLon;
            lLonSign = Math.Sign(lLonDist);  // + = E, - = W
            if (lLonSign == 0) lLonSign = 1;
            //Latitudal distance to target
            lLatDist = (TargetLoc.Latitude - ShooterLoc.Latitude) * YardsPerDegLatLon;
            lLatSign = Math.Sign(lLatDist);  // + = N, - = S
            if (lLatSign == 0) lLatSign = 1;
            //Correction factors for location on earth, convert degrees to radians
            lVertMultiplier = Math.Cos(ShooterLoc.Latitude * (Math.PI / 180));
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

            // __________________ Adding in residual drop and windage that accumulates past zero distance _______
            // Zero condition correction is wrong.  should be a linear line with range.  It passes through the
            //  coriolis error at the zero range (i.e. 1" correction at 100 yards = 5" at 500 yet the coriolis
            //  error might be 6" at 500 due to the bullet lossing velocity.

            //Positive = high, East
            //Negative = low, west
            lCorVert = (lCorMag * lVertMultiplier * lSVM);// - lCorZVS * Range;
            //Postive = right north
            //Negative = left south
            lCorHorz = (lCorMag * lHorizMultiplier * lSHM);// - lCorZHS * Range;
            lSHM = 0;
            VerticalComponent = lCorVert;
            HorizontalComponent = lCorHorz;
            return 0;
        }
        /// <summary>
        /// Magnitude of the horizontal displacement induced by the bullets gyroscopic force.
        /// </summary>
        /// <param name="Range">The range to find the displacement at.</param>
        /// <param name="TwistDirection">The direction of the barrel rifling twist i.e. Right or Left.</param>
        /// <param name="BSG">Bullet stability factor</param>
        /// <param name="Fo">Initual drag factor at muzzle.</param>
        /// <param name="MuzzleVelocity">Bullet velocity at the barrel muzzle.</param>
        /// <returns>Horizontal distance in inches.</returns>
        private static double GetRawSpinDrift(double Range, string TwistDirection,double BSG, double Fo, double MuzzleVelocity, 
            double F2, double F3, double F4, double Zone1Range, double Zone1TransSpeed, double Zone1Slope, double Zone2Range,
            double Zone2TransSpeed, double Zone2Slope, double Zone3Range, double Zone3TransSpeed, double Zone3Slope, 
            double Zone1SlopeMultiplier, double Zone3SlopeMultiplier, double Zone4Slope)
        {
            double lFT;
            double lDrift;
            string lcmp = TwistDirection.ToLower().Substring(0, 1);

            lFT = FlightTime(Range, Fo, F2, F3, F4, MuzzleVelocity, Zone1Range, Zone1TransSpeed, Zone1Slope, Zone2Range,
                Zone2TransSpeed, Zone2Slope, Zone3Range, Zone3TransSpeed, Zone3Slope, Zone1SlopeMultiplier, Zone3SlopeMultiplier,
                Zone4Slope);
            lDrift = 1.25 * (BSG + 1.2) * Math.Pow(lFT, 1.83);
            if (lcmp != "r")
            {
                lDrift = lDrift * (-1);
            }

            return lDrift;
        }
        /// <summary>
        /// Factor used to calculate Zero range, Near Zero Range, and Point-Blank-Range (PBR)
        /// </summary>
        /// <returns></returns>
        private static double SH(double ZeroMaxRise, double ScopeHeight)
        {
            double lSH;

            if (ZeroMaxRise == 0) return 0;
            //SH = (1 + S/Hm)^0.5
            lSH = Math.Pow((1 + (ScopeHeight / ZeroMaxRise)), 0.5);
            return lSH;
        }

        private static double FltTimeZ1(double Range, double MuzzleVelocity, double Fo, double Zone1Slope)
        {
            double lT;

            //Zone 1

            //T = 3R / Vo / (1 - (3R / 2Fo) + (1 - 2N)/ 130)
            lT = (3 * Range) / MuzzleVelocity / (1 - ((3 * Range) / (2 * Fo)) + ((1 - 2 * Zone1Slope) / 130));

            return lT;
        }
        private static double FltTimeZ2(double Range, double Zone1TransSpeed, double F2)
        {
            double lT;

            //Zone 2

            //T = (Fo / Vo)(exp(3R / Fo) - 1)
            lT = (F2 / Zone1TransSpeed) * (Math.Exp(((3 * Range) / F2)) - 1);

            return lT;
        }
        private static double FltTimeZ3(double Range, double Zone2TransSpeed, double F3, double Zone3Slope)
        {
            double lT;

            //Zone 3

            //T = 3R / Vo / (1 - (3R / 2Fo) + (1 - 2N)/ 130)

            lT = (3 * Range) / Zone2TransSpeed / (1 - ((3 * Range) / (2 * F3)) + ((1 - 2 * Zone3Slope) / 130));

            return lT;
        }
        private static double FltTimeZ4(double Range, double Zone3TransSpeed, double F4)
        {
            double lT;

            //Zone 4

            //T = (Fo / Vo)(exp(3R / Fo) - 1)
            lT = (F4 / Zone3TransSpeed) * (Math.Exp((3 * Range / F4)) - 1);

            return lT;
        }
        #endregion

    }
}
