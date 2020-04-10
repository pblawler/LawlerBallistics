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
        public static double SpinRate(double Range, double MuzzleVelocity, double BarrelTwist, double BulletDiameter, double Fo)
        {
            double lSR = 0; //Spin Rate
            double lFT;     //Flight time

            //MV x (12/twist rate in inches) x 60 = Bullet RPM
            lSR = MuzzleVelocity * (12.00 / BarrelTwist) * 60;
            lFT = FlightTime(Range, Fo, MuzzleVelocity);

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

        #region "Horizontal Trajectory"
        /// <summary>
        ///  The spin drift correction rate. When zeroing it is automatically accounted for and continues in a
        /// straight line past the zero range. Since the bullet is slowing the rate will be over come past the
        /// zero range but this must be accounted for or the reported drift will be too much.
        /// </summary>
        /// <param name="ZeroRange">The far distance where the sight elevation deviation is zero.</param>
        /// <param name="TwistDirection">The direction of the barrel rifling twist i.e. Right or Left.</param>
        /// <param name="BSG">Bullet stability factor</param>
        /// <param name="Fo">Initual drag factor at muzzle.</param>
        /// <param name="MuzzleVelocity">Bullet velocity at the barrel muzzle.</param>
        /// <returns></returns>
        public static double SpindDriftCorrection(double ZeroRange, string TwistDirection, double BSG, double Fo, double MuzzleVelocity)
        {
                double lZSD;
                double lZDC;

                //Amount of drift corrected at sight in
                lZSD = GetRawSpinDrift(ZeroRange,TwistDirection,BSG,Fo,MuzzleVelocity);
                //Correction factor (in/yards), the linear offset induced at sight in.
                lZDC = lZSD / ZeroRange;
                return lZDC;
        }
        /// <summary>
        /// Returns the horizontal displacement caused by wind.
        /// </summary>
        /// <param name="WindSpeed">The speed of the wind in mph.</param>
        /// <param name="WindDirection">The direction of the wind in degrees with the target at 0 degrees.</param>
        /// <param name="Range">The distance to the target in yards.</param>
        /// <param name="Fo">Initual drag factor at muzzle.</param>
        /// <param name="MuzzleVelocity">Bullet velocity at the barrel muzzle.</param>
        /// <returns>Horizontal displacement in inches</returns>
        public static double WindDrift(double WindSpeed, double WindDirection, double Range, double Fo, double MuzzleVelocity)
        {
            double lt;      //Flight time
            double lTF;     //Theoretical Flight time with no drag
            double lDT;     //Flight time due to drag
            double lWD;     //Wind direction in degrees
            double lCW;     //Cross wind magnitude
            double lWDR;    //Wind displacement rate
            double lHWD;    //Horizontal wind drift

            //Actual flight time
            lt = FlightTime(Range,Fo, MuzzleVelocity);
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
        public static double WindDrift(double WindSpeed, uint Clock, double Range, double Fo, double MuzzleVelocity)
        {
            double lDegP = 360 / 12;       //Degrees per clock reading
            double lWD = lDegP * Clock;  //Total degrees

            return WindDrift(WindSpeed, lWD, Range, Fo, MuzzleVelocity);

        }
        #endregion

        #region "Other Flight Characteristics"
        /// <summary>
        /// Calculates the bullet flight time to the provided range.
        /// </summary>
        /// <param name="Range">Distance (yrds) to calculate the flight time for.</param>
        /// <param name="Fo">Initual drag factor at muzzle.</param>
        /// <param name="MuzzleVelocity">Bullet velocity at the barrel muzzle.</param>
        /// <returns>Bullet flight time in seconds to reach the provided range.</returns>
        public static double FlightTime(double Range, double Fo, double MuzzleVelocity)
        {
            double lt;

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
        #endregion

        #region "Vertical Trajectory"
        /// <summary>
        /// The range to target corrected for incline.  Line-of-sight distance will be greater than the distance affected
        /// by gravity.  The distance to shoot for is the range affected by gravity and drag which is returned by this function.
        /// </summary>
        /// <param name="ShotAngle">The angle absolute value of the shot angle.</param>
        /// <param name="LOSrange">The line-of-sight (LOS) range.</param>
        /// <returns></returns>
        public static double AngleCompRange(double ShotAngle, double LOSrange, double MuzzleVelocity, double Range, double Zone1Range, double Zone2Range, double Zone3Range,
            double Zone1Slope, double Zone1AngleFactor, double Zone1TransSpeed, double Zone2TransSpeed, double Zone3TransSpeed,
            double Fo, double F2, double F3, double DensityAlt, double DensityAltAtZero)
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
            lLOSD = MuzzleDrop(MuzzleVelocity, LOSrange, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone1AngleFactor,
                        Zone1TransSpeed, Zone2TransSpeed, Zone3TransSpeed, Fo, F2, F3, DensityAlt, DensityAltAtZero);
            lTDA = 90 - ShotAngle;
            lTDD = Math.Abs(Math.Sin(lTDA * (lPi / 180)) * lLOSD);
            lDID = Math.Abs(lLOSD);
            lDIR = LOSrange - 0.25;
            //Iterate drop function to find the range that equals the corrected drop distance.
            while (lDID > lTDD)
            {
                lDID = Math.Abs(MuzzleDrop(MuzzleVelocity, lDIR, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone1AngleFactor,
                        Zone1TransSpeed, Zone2TransSpeed, Zone3TransSpeed, Fo, F2, F3, DensityAlt, DensityAltAtZero));
                lDIR = lDIR - 0.5;
            }
            return lDIR;
        }
        /// <summary>
        /// The bullet drop from the muzzle of the barrel.
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
        /// <param name="RawMagnatude"></param>
        /// <returns>Muzzle drop distance in inches.</returns>
        public static double MuzzleDrop(double MuzzleVelocity, double Range, double Zone1Range, double Zone2Range, double Zone3Range,
            double Zone1Slope, double Zone3Slope, double Zone1SlopeMultiplier, double Zone3SlopeMultiplier,
            double Zone1AngleFactor, double Zone1TransSpeed, double Zone2TransSpeed, double Zone3TransSpeed,
            double Fo, double F2, double F3, double F4, double DensityAlt, double DensityAltAtZero, bool RawMagnatude = false)
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
                    Zone3Slope,Zone1SlopeMultiplier, Zone3SlopeMultiplier ),DensityAlt,DensityAltAtZero)))), 2);
                ld = lD1;
            }
            else if ((Range > Zone1Range) &
                (Range <= Zone2Range))
            {
                //Zone 2

                if (Zone1Range > 0)
                {
                    //D = (G/Vo/(1/R - 1/Fa))^2
                    lD1 = MuzzleDrop(MuzzleVelocity, Zone1Range, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone3Slope,
                        Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed,
                        Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, true);
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
                //    'D = (G/Vo/(1/R - 1/Fa))^2
                lD2 = Math.Pow(((BallisticFunctions.G / Zone1TransSpeed) /
                    ((1 / (Range - Zone1Range)) -
                    (1 / DensityAltCorrection(F2, DensityAlt, DensityAltAtZero)))), 2);
                ld = lD1 + lD1p + lD2;
            }
            else if ((Range > Zone2Range) & (Range <= Zone3Range))
            {
                //Zone 3

                if (Zone2Range > 0)
                {
                    lD1t = MuzzleDrop(MuzzleVelocity, Zone2Range, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone3Slope,
                        Zone1SlopeMultiplier, Zone3SlopeMultiplier, Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed, 
                        Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, true);
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
                    lD3 = MuzzleDrop(MuzzleVelocity, Zone3Range, Zone1Range, Zone2Range, Zone3Range, Zone1Slope, Zone3Slope,
                        Zone1SlopeMultiplier, Zone3SlopeMultiplier,  Zone1AngleFactor, Zone1TransSpeed, Zone2TransSpeed, 
                        Zone3TransSpeed, Fo, F2, F3, F4, DensityAlt, DensityAltAtZero, true);
                    lFa = DensityAltCorrection(F2, DensityAlt, DensityAltAtZero) + (DensityAltCorrection(F3, DensityAlt, DensityAltAtZero) -
                        DensityAltCorrection(F2, DensityAlt, DensityAltAtZero)) / 4.000;
                    //D3' = gF3(Rt - (R1 + R2 + R3)) * 3((1 + 12 * R3 / F2) ^ (3 / 2) - 1) / 6 / (Zone2TransSpeed) ^ 2
                    lD3p = BallisticFunctions.GravIn * DensityAltCorrection(F3, DensityAlt, DensityAltAtZero) * (Range - Zone3Range) * 3 *
                        (Math.Pow((1 + 12 * (Zone3Range - Zone2Range) / lFa), (3 / 2)) - 1) / 6 / Math.Pow(Zone2TransSpeed, 2);
                }
                //D = (G/Vo/(1/R - 1/Fa))^2
                lD4 = Math.Pow((BallisticFunctions.G / Zone3TransSpeed /
                    ((1.00 / (Range - Zone3Range) -
                    1 / DensityAltCorrection(F3, DensityAlt, DensityAltAtZero)))), 2);
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
        /// Vertical Coriolis affect at the provided range.
        /// </summary>
        /// <param name="Range">Distance to calculate Coriolis affect at.</param>
        /// <returns>Inches of rise or drop caused by Coriolis addect.</returns>
        public static double GetCoriolisVert(double Range)
        {
            double lCorVert = 0; double lCorHoriz = 0;
            int lR;

            lR = GetCoriolisComponents(ref lCorVert, ref lCorHoriz, Range);
            return lCorVert;
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
        /// Calculates the effective drag/retard coefficent for the provided range.
        /// </summary>
        /// <param name="Range">Distance to calculate the F value at.</param>
        /// <returns>F drag/retard coefficent</returns>
        private static double Fa(double Range,double Fo, double F2, double F3, double F4, double Zone1Range, double Zone2Range, double Zone3Range,
            double Zone1Slope, double Zone3Slope, double Zone1SlopeMultiplier, double Zone3SlopeMultiplier)
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
                lFa = F2;
            }
            else if ((Zone2Range < Range) &
                (Zone3Range >= Range))
            {
                //F at the provided range.  Applies to Zone 3
                lFa = (F3 - Zone3SlopeMultiplier *
                    Zone3Slope * (Range - Zone3Range));
            }
            else if (Zone3Range < Range)
            {
                //F at the provided range.  Applies to Zone 4
                lFa = F4;
            }
            return lFa;
        }
        /// <summary>
        /// Calculates the horizontal and vertical devation of the flight path due to the Coriolis effect.
        /// </summary>
        /// <param name="VerticalComponent">Vertical drop or rise (in.) caused by the Coriolis effect.</param>
        /// <param name="HorizontalComponent">Horizontal left or right drift (in.) caused by the Coriolis effect.</param>
        /// <param name="Range">Distance (yrds) to caclculate the Coriolis effect for.</param>
        /// <returns>A routine trouble code and the horizontal and vertical effect of the earths rotation.</returns>
        private int GetCoriolisComponents(ref double VerticalComponent, ref double HorizontalComponent, double Range,
            LocationData ZeroTargetLoc)
        {
            double lYrdPerDeg; double lLonDist; double lLatDist; double lVertMultiplier;
            double lHorizMultiplier; double lFT; double lCorMag; double lSVM; double lSHM;
            double lCorVert; double lCorHorz; double lLonSign; double lLatSign; double lLonDistZ;
            double lLonSignZ; double lLatDistZ; double lLatSignZ; double lZeroVertMultiplier;
            double lZeroHorizMultiplier; double lZSVM; double lZSHM; double lCorZero; double lCorZVS;
            double lCorZHS; double lFTZ;


            if ((ZeroTargetLoc.Longitude == 0) || (MyScenario.MyBallisticData.zeroData.ShooterLoc.Longitude == 0)
                || (ZeroTargetLoc.Latitude == 0) || (MyScenario.MyBallisticData.zeroData.ShooterLoc.Latitude == 0)
                || (MyScenario.MyBallisticData.zeroData.ShooterLoc.Latitude == 0) || (MyScenario.MyBallisticData.zeroData.ShooterLoc.Longitude == 0)
                || (ZeroTargetLoc.Latitude == 0) || (ZeroTargetLoc.Longitude == 0))
            {
                HorizontalComponent = 0;
                VerticalComponent = 0;
                return -1;
            }

            lYrdPerDeg = 121740.6652;
            lFT = FlightTime(Range);
            lFTZ = FlightTime(MyScenario.MyBallisticData.zeroData.ZeroRange);
            lCorMag = 0.00262 * lFT * Range;
            lCorZero = 0.00262 * lFTZ * MyScenario.MyBallisticData.zeroData.ZeroRange;

            //______________ Zero bias ___________________________
            //  Solve for the amount of Coriolis correction added during zero, this is
            //  like the sight line in that it is linear with distance and zero at the zero ranges.

            //Longitudal distance to target
            lLonDistZ = (MyScenario.MyBallisticData.zeroData.TargetLoc.Longitude -
                MyScenario.MyBallisticData.zeroData.ShooterLoc.Longitude) * lYrdPerDeg;
            lLonSignZ = Math.Sign(lLonDistZ);  // + = E, - = W
            if (lLonSignZ == 0) lLonSignZ = 1;
            //Latitudal distance to target
            lLatDistZ = (MyScenario.MyBallisticData.zeroData.TargetLoc.Latitude -
                MyScenario.MyBallisticData.zeroData.ShooterLoc.Latitude) * lYrdPerDeg;
            lLatSignZ = Math.Sign(lLatDistZ);  // + = N, - = S
            if (lLatSignZ == 0) lLatSignZ = 1;
            //Correction factors for location on earth, the larger the lat the less vertical affect and more horizontal,
            //     convert degrees to radians
            lZeroVertMultiplier = Math.Cos(MyScenario.MyBallisticData.zeroData.ShooterLoc.Latitude * (Math.PI / 180));
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

            lCorZVS = (lCorZero * lZeroVertMultiplier * lZSVM) / MyScenario.MyBallisticData.zeroData.ZeroRange;
            lCorZHS = (lCorZero * lZeroHorizMultiplier * lZSHM) / MyScenario.MyBallisticData.zeroData.ZeroRange;

            //Positive = high, East
            //Negative = low, west

            //Longitudal distance to target
            lLonDist = (MyScenario.SelectedTarget.TargetLocation.Longitude -
                MyScenario.MyShooter.MyLocation.Longitude) * lYrdPerDeg;
            lLonSign = Math.Sign(lLonDist);  // + = E, - = W
            if (lLonSign == 0) lLonSign = 1;
            //Latitudal distance to target
            lLatDist = (MyScenario.SelectedTarget.TargetLocation.Latitude - MyScenario.MyShooter.MyLocation.Latitude) * lYrdPerDeg;
            lLatSign = Math.Sign(lLatDist);  // + = N, - = S
            if (lLatSign == 0) lLatSign = 1;
            //Correction factors for location on earth, convert degrees to radians
            lVertMultiplier = Math.Cos(MyScenario.MyShooter.MyLocation.Latitude * (Math.PI / 180));
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
        /// Magnitude of the horizontal displacement induced by the bullets gyroscopic force.
        /// </summary>
        /// <param name="Range">The range to find the displacement at.</param>
        /// <param name="TwistDirection">The direction of the barrel rifling twist i.e. Right or Left.</param>
        /// <param name="BSG">Bullet stability factor</param>
        /// <param name="Fo">Initual drag factor at muzzle.</param>
        /// <param name="MuzzleVelocity">Bullet velocity at the barrel muzzle.</param>
        /// <returns>Horizontal distance in inches.</returns>
        private static double GetRawSpinDrift(double Range, string TwistDirection,double BSG, double Fo, double MuzzleVelocity)
        {
            double lFT;
            double lDrift;
            string lcmp = TwistDirection.ToLower().Substring(0, 1);

            lFT = FlightTime(Range, Fo, MuzzleVelocity);
            lDrift = 1.25 * (BSG + 1.2) * Math.Pow(lFT, 1.83);
            if (lcmp != "r")
            {
                lDrift = lDrift * (-1);
            }
            return lDrift;
        }
        #endregion

    }
}
