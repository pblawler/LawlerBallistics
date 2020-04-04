using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes.BallisticClasses
{
    public static class BallisticFunctions
    {
        /// <summary>
        /// The gyroscopic stability measure of the bullet, a value of 1 is minimal stability.
        /// </summary>
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

    }
}
