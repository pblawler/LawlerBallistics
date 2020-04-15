using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;

namespace LawlerBallisticsDesk.Classes
{
    public class Atmospherics : INotifyPropertyChanged
    {

        //TODO: call raise property on dependent properties when independent values change

        #region "Binding"
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region "Weather Query"
        private const string _GetWeather = "http://api.openweathermap.org/data/2.5/weather?lat=@latitude@&lon=@longitude@&mode=xml&units=imperial&APPID=";
        #endregion

        #region "Private Variables"
        private const double _E = 6356.766;        // radius of earth KM
        private const double _GoMS = 9.80665;     //𝑔𝑜 is gravity 9.80665ms-2,
        private const double _Rd = 287.053;   //𝑅𝑑 is the gas constant for dry air (287.053 Jkg−1𝐾−1)
        private const double _R = 8.31432;     // gas constant, J/mol*deg K
        private const double _SpecGasCwatervapor = 461.52;    // J/(kgK)
        private const double _AirMolMass = 28.9647;   // grams/mole
        private const double _WaterVaporMolMass = 18.01528;   // grams/mole
        private const double _StandardTempK = 288.15;
        private const double _L = 6.50;    // is the standard tropospheric lapse rate of −6.5 K/km
        private const double _StandPressKpa = 101.325; // Standard atmospheric pressure kPa
        double _DensityAlt;
        double _Temp;
        string _TempUnits;
        double _FeelsLike;
        string _FeelsLikeUnits;
        double _Pressure;
        string _PressureUnits;
        double _HumidityRel;
        double _HumidityAbs;
        double _HumiditySpec;
        double _WindSpeed;
        string _WindSpeedUnits;
        double _WindDirection;
        string _WindDirTxt;
        string _Station;
        DateTime _Sunrise;
        DateTime _SunSet;
        DateTime _LastUpdated;
        string _Precipitation;
        string _Weather;
        #endregion

        #region "Properties"
        public double WindSpeed { get { return _WindSpeed; } set { _WindSpeed = value; RaisePropertyChanged(nameof(WindSpeed)); } }
        public string WindSpeedUnits { get { return _WindSpeedUnits; } set { _WindSpeedUnits = value; RaisePropertyChanged(nameof(WindSpeedUnits)); } }
        public double WindDirection { get { return _WindDirection; } set { _WindDirection = value; RaisePropertyChanged(nameof(WindDirection)); } }
        public string WindDirTxt { get { return _WindDirTxt; } set { _WindDirTxt = value; RaisePropertyChanged(nameof(WindDirTxt)); } }
        public double Temp { get { return _Temp; }
            set
            {
                _Temp = value;
                _DensityAlt = 0;
                RaisePropertyChanged(nameof(Temp));
            }
        }
        /// <summary>
        /// F, C, K
        /// </summary>
        public string TempUnits { get { return _TempUnits; } set { _TempUnits = value; RaisePropertyChanged(nameof(TempUnits)); } }
        public double FeelsLike
        {
            get { return _FeelsLike; }
            set
            {
                _FeelsLike = value;
                //TODO: recalculate density alt here
                _DensityAlt = 0;
                
                RaisePropertyChanged(nameof(Temp));
            }
        }
        /// <summary>
        /// F, C, K
        /// </summary>
        public string FeelsLikeUnits { get { return _FeelsLikeUnits; } set { _FeelsLikeUnits = value; RaisePropertyChanged(nameof(TempUnits)); } }
        public double Pressure 
        {
            get
            {
                return _Pressure;
            }
            set
            {
                _Pressure = value;
                _DensityAlt = 0;
                RaisePropertyChanged(nameof(Pressure)); } }
        /// <summary>
        /// inHg, kPa
        /// </summary>
        public string PressureUnits { get { return _PressureUnits; } set { _PressureUnits = value; RaisePropertyChanged(nameof(PressureUnits)); } }
        /// <summary>
        /// Water density grams/m3
        /// </summary>
        public double HumidityAbs 
        {
            get
            {
                    _HumidityAbs = 0;     //Absolute Humidity
                    double lTC = Temp;     //Temperature in Degrees C

                //Absolute Humidity(grams/m3) = (6.112 × e^[(17.67 × T)/(T+243.5)] × rh × 2.1674)/(273.15+T)
                // where T is in Celcius and rh = relative humidity

                //Convert Temp F to Temp C
                if ((TempUnits.ToLower() == "f") || (TempUnits.ToLower() == "fahrenheit"))
                {
                    lTC = DegFtoDegC(Temp);
                }
                else if ((TempUnits.ToLower() == "k") || (TempUnits.ToLower() == "kelvin"))
                {
                    lTC = DegKtoDegC(Temp);
                }
                _HumidityAbs = (6.112 * Math.Pow(Math.E, ((17.67 * lTC) / (lTC + 243.5))) * HumidityRel * 2.1674) / (273.15 + lTC);

                return _HumidityAbs;
            }
        }
        public double HumidityRel 
        {
            get
            {
                if( _HumidityRel > 1.0) _HumidityRel = _HumidityRel / 100.0;                
                return _HumidityRel;
            }
            set
            {
                _HumidityRel = value;
                _DensityAlt = 0;
                RaisePropertyChanged(nameof(HumidityRel));
                RaisePropertyChanged(nameof(HumidityAbs)); } }
        /// <summary>
        /// In a system of moist air, the (dimensionless) ratio of the mass of water vapor to the total mass of the system.
        /// </summary>
        public double HumiditySpec
        {
            get
            {
                _HumiditySpec = 0;

                ///  Specific_Humidity = Mixing Ration/( 1 + Mixing Ration)
                _HumiditySpec = AirVaporMixingRatio / (1 - AirVaporMixingRatio);

                return _HumiditySpec;
            }
        }
        /// <summary>
        /// Water Vapor saturation pressure in kPa
        /// </summary>
        public double WaterVaporSaturationPressure
        {
            get
            {
                double lWVSP = 0;
                double lTC = Temp;

                if ((TempUnits.ToLower() == "f") || (TempUnits.ToLower() == "fahrenheit"))
                {
                    lTC = DegFtoDegC(Temp);
                }
                else if ((TempUnits.ToLower() == "k") || (TempUnits.ToLower() == "kelvin"))
                {
                    lTC = DegKtoDegC(Temp);
                }

                /// H2O_Saturation_Pressure (kPa) = (0.61121 * EXP[(18.678 - (T/234.5)) * (T / (257.14 + T))] ; T is in celcius
                lWVSP = (0.61121 * Math.Exp(((18.678 - (lTC / 234.5)) * (lTC / (257.14 + lTC)))));

                return lWVSP;
            }
        }
        /// <summary>
        /// Water Vapor partial pressure in kPa
        /// </summary>
        public double WaterVaporPartialPressure
        {
            get
            {
                double lWVPP = 0;

                ///  H2O_PartialPressure = H2O_Saturation_Pressure * Relative Humidity
                lWVPP = WaterVaporSaturationPressure * HumidityRel;
                return lWVPP;
            }
        }
        /// <summary>
        /// The ratio of the mass of water vapor to the mass of dry air. 
        /// </summary>
        public double AirVaporMixingRatio
        {
            get
            {
                double lAVMR = 0;
                double lBP_kPa = Pressure;

                if ((PressureUnits.ToLower() == "inhg")) lBP_kPa = InHgTokPa(Pressure);

                ///  Mixing Ratio = (0.622 * H2O_PartialPressure)/(BaroPressure (kPa) - H2O_PartialPressure)
                lAVMR = (0.622 * WaterVaporPartialPressure) / (lBP_kPa - WaterVaporPartialPressure);

                return lAVMR;
            }
        }
        /// <summary>
        /// The molecular weight of air taking humidity into account.
        /// </summary>
        public double MolecularWeightOfAir
        {
            get
            {
                double lMWA = 0;
                double lSH = HumiditySpec;
                ///  Molecular Wt Air = 29 (g/mol) * (1 - Specific_Humidity) + 18 (g/mol) * (Specific_Humidity)
                lMWA = _AirMolMass * (1 - lSH) + _WaterVaporMolMass * lSH;

                return lMWA;
            }
        }
        /// <summary>
        /// Density Altitude in feet.
        /// </summary>
        public double DensityAlt
        {
            get
            {
                if (_DensityAlt == 0)
                {
                    double lTv = 0;
                    double lTK = 0;
                    double lPkpa = 1;
                    double DensityALTh = 0;
                    double lD = 0;

                    if ((TempUnits.ToLower() == "f") || (TempUnits.ToLower() == "fahrenheit"))
                    {
                        lTK = DegFtoDegK(Temp);
                    }
                    else if ((TempUnits.ToLower() == "c") || (TempUnits.ToLower() == "celsius"))
                    {
                        lTK = DegCtoDegK(Temp);
                    }
                    if ((PressureUnits.ToLower() == "inhg")) lPkpa = InHgTokPa(Pressure);

                    lD = ((lPkpa * 1000) * MolecularWeightOfAir) / (_R * lTK * 1000);

                    DensityALTh = (_StandardTempK / _L) * (1 - Math.Pow(((1000 * _R * _StandardTempK * lD) / (_AirMolMass * _StandPressKpa * 1000)), ((_L * _R) / (_GoMS * MolecularWeightOfAir - _L * _R))));
                    DensityALTh = (_E * DensityALTh) / (_E - DensityALTh);

                    _DensityAlt = KilometersToFeet(DensityALTh);
                }
                return _DensityAlt;
            }
            set
            {
                _DensityAlt = value;
                RaisePropertyChanged(nameof(DensityAlt));
            }
        }
        /// <summary>
        /// Speed-Of-Sound in fps
        /// </summary>
        public double SpeedOfSound
        {
            get
            {
                double lSOS = 0;

                double lT = Temp;
                if ((TempUnits.ToLower() == "f") || (TempUnits.ToLower() == "fahrenheit"))
                {
                    lT = DegFtoDegK(Temp);
                }
                else if ((TempUnits.ToLower() == "c") || (TempUnits.ToLower() == "celsius"))
                {
                    lT = DegCtoDegK(Temp);
                }

                double lRH = HumidityRel;
                double lBP = MolecularWeightOfAir;

                //Speed of Sound = [(1.4* ( 8.3145 J/mol K)*(Temp C))/(M = the molecular weight of the gas in kg/mol)]^0.5
                lSOS = Math.Pow(((1.4 * 8.3145 * lT) / (lBP / 1000)), 0.5);
                lSOS = M_S_to_fps(lSOS);

                return lSOS;
            }
        }
        public string Station { get { return _Station; } set { _Station = value; RaisePropertyChanged(nameof(Station)); } }
        public DateTime SunRise { get { return _Sunrise; } set { _Sunrise = value; RaisePropertyChanged(nameof(SunRise)); } }
        public DateTime SunSet { get { return _SunSet; } set { _SunSet = value; RaisePropertyChanged(nameof(SunSet)); } }
        public DateTime LastUpdated { get { return _LastUpdated; } set { _LastUpdated = value; RaisePropertyChanged(nameof(LastUpdated)); } }
        public string Precipitation { get { return _Precipitation; } set { _Precipitation = value; RaisePropertyChanged(nameof(Precipitation)); } }
        public string Weather { get { return _Weather; } set { _Weather = value; RaisePropertyChanged(nameof(Weather)); } }
        #endregion

        #region "Constructor"
        public Atmospherics()
        {
            _TempUnits = "Fahrenheit";
            _PressureUnits = "inHg";
            _WindSpeedUnits = "mph";
        }
        #endregion

        #region "Private Routines"
        private double DegFtoDegC(double T)
        {
            double lT = 0;
            lT = (T - 32) * (5.00 / 9.00);
            return lT;
        }
        private double DegCtoDegF(double T)
        {
            double lT = 0;

            lT = ((9.00/5.00)*T)+32;
            return lT;
        }
        private double DegKtoDegF(double T)
        {
            double lT = 0;
            lT = (T - 273.15);
            lT = DegCtoDegF(lT);
            return lT;
        }
        private double DegKtoDegC(double T)
        {
            double lT = 0;
            lT = (T - 273.15);
            return lT;
        }
         private double DegCtoDegK(double T)
        {
            double lT = 0;
            lT = (T + 273.15);
            return lT;
        }
        private double DegFtoDegK(double T)
        {
            double lT = 0;

            lT = DegFtoDegC(T) + 273.15;
            return lT;
        }                             
        /// <summary>
        /// Convert pressure in InchesHg to kPa
        /// </summary>
        /// <param name="PressureInHg">Pressure in inches of mercury</param>
        /// <returns>kPa</returns>
        private double InHgTokPa(double PressureInHg)
        {
            double lR = PressureInHg * 3.38639;
            return lR;
        }
        private double kPaToInHg(double PressureKPa)
        {
            double lR = PressureKPa / 3.38639;
            return lR;
        }
        private double hPaToInHg(double PressurehPa)
        {
            double lRTN = 0;

            lRTN = PressurehPa * 0.02953;

            return lRTN;
        }
        private double M_S_to_fps(double MetersPerSec)
        {
            double lfps;

            lfps = MetersPerSec * 3.28084;

            return lfps;
        }
        private double MetersToFeet(double meters)
        {
            double lFt = 0;

            lFt = meters * 3.28084;
            return lFt;
        }
        private double KilometersToFeet(double meters)
        {
            double lFt = 0;

            lFt = meters * 3.28084*1000;
            return lFt;
        }
        private DateTime ParseWeatherTime(string WeatherXMLdateTimeVal)
        {
            DateTime lRTN = DateTime.Now;
            string lDate;
            string lTime;
            string lDateTime;

            lDate = WeatherXMLdateTimeVal.Substring(0, WeatherXMLdateTimeVal.IndexOf("T"));
            lTime = WeatherXMLdateTimeVal.Substring(WeatherXMLdateTimeVal.IndexOf("T")+1);
            lDateTime = lDate + " " + lTime;
            lRTN = Convert.ToDateTime(lDateTime);
            return lRTN;
        }
        private void PropertysChanged()
        {
            RaisePropertyChanged when the principle properties change
        }
        #endregion

        #region "Public Routines"
        public void GetWeather(double Latitude, double Longitude)
        {
            string lTargetFolder = LawlerBallisticsFactory.WeatherFolder + "\\Weather.xml";
            try
            {
                XmlDocument lWeatherXML = new XmlDocument();
                string lRtn = "";
                string lUrl = _GetWeather + Properties.Settings.Default.Openweathermap;
                string lLat = Latitude.ToString();
                string lLon = Longitude.ToString();
                lUrl = lUrl.Replace("@latitude@", lLat);
                lUrl = lUrl.Replace("@longitude@", lLon);
                WebClient lwc = new WebClient();
                lRtn = lwc.DownloadString(lUrl);
                lWeatherXML.LoadXml(lRtn);
                lWeatherXML.Save(lTargetFolder);
                XmlNode lCurr = lWeatherXML.SelectSingleNode("current");
                XmlNode lNode = lCurr.SelectSingleNode("city");
                Station = lNode.Attributes["name"].Value;
                XmlNode lcNode = lNode.SelectSingleNode("timezone");
                lRtn = lcNode.InnerText;
                int lTo = Convert.ToInt32(lRtn);
                lTo = lTo / 3600;
                lcNode = lNode.SelectSingleNode("sun");
                lRtn = lcNode.Attributes["rise"].Value;
                SunRise = ParseWeatherTime(lRtn);
                lRtn = lcNode.Attributes["set"].Value;
                SunSet = ParseWeatherTime(lRtn);
                lNode = lCurr.SelectSingleNode("feels_like");
                lRtn = lNode.Attributes["value"].Value;
                FeelsLike = Convert.ToDouble(lRtn);
                FeelsLikeUnits = lNode.Attributes["unit"].Value;
                lNode = lCurr.SelectSingleNode("temperature");
                lRtn = lNode.Attributes["value"].Value;
                Temp = Convert.ToDouble(lRtn);
                TempUnits = lNode.Attributes["unit"].Value;
                lNode = lCurr.SelectSingleNode("pressure");
                lRtn = lNode.Attributes["value"].Value;
                Pressure = Convert.ToDouble(lRtn);
                PressureUnits = lNode.Attributes["unit"].Value;
                lNode = lCurr.SelectSingleNode("humidity");
                lRtn = lNode.Attributes["value"].Value;
                HumidityRel = Convert.ToDouble(lRtn);
                lNode = lCurr.SelectSingleNode("wind");
                lcNode = lNode.SelectSingleNode("speed");
                lRtn = lcNode.Attributes["value"].Value;
                WindSpeed = Convert.ToDouble(lRtn);
                WindSpeedUnits = lcNode.Attributes["unit"].Value;
                lcNode = lNode.SelectSingleNode("direction");
                lRtn = lcNode.Attributes["value"].Value;
                WindDirection = Convert.ToDouble(lRtn);
                //TODO: convert shooter and target location to deg and get wind dir relative to flight path from here.
                WindDirTxt = lcNode.Attributes["code"].Value;
                lNode = lCurr.SelectSingleNode("precipitation");
                Precipitation = lNode.Attributes["mode"].Value;
                lNode = lCurr.SelectSingleNode("weather");
                Weather = lNode.Attributes["value"].Value;
                lNode = lCurr.SelectSingleNode("lastupdate");
                lRtn = lNode.Attributes["value"].Value;
                LastUpdated = ParseWeatherTime(lRtn);                
                lRtn = "";
                switch(PressureUnits.ToLower())
                {
                    case "hpa":
                        Pressure = hPaToInHg(Pressure);
                        PressureUnits = "inHg";
                        break;
                    case "kpa":
                        Pressure = kPaToInHg(Pressure);
                        PressureUnits = "inHg";
                        break;
                    default:
                        break;
                }
                switch(TempUnits.ToLower().Substring(0,1))
                {
                    case "c":
                        Temp = DegCtoDegF(Temp);
                        TempUnits = "fahrenheit";
                        break;
                    case "k":
                        Temp = DegKtoDegF(Temp);
                        TempUnits = "fahrenheit";
                        break;
                    default:
                        break;
                }
                switch (WindSpeedUnits.ToLower())
                {
                    //TODO: convert wind speed to mph
                    case "ms":
                       
                        break;
                    case "fps":
                        
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
               WindDirTxt = "ERR";
            }

        }
        #endregion
    }
}
