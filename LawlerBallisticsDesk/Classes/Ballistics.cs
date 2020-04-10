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

        #endregion

        #region "Public Properties"

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
