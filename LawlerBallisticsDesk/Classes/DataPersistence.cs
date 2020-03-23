using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LawlerBallisticsDesk.Classes
{
    public class DataPersistence : INotifyPropertyChanged
    {
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
        private string _AppDataFolder;
        private string _DataFolder;
        #endregion

        #region "Properties"
        public string AppDataFolder
        {
            get { return _AppDataFolder; }
            set {
                _AppDataFolder = value; 
                RaisePropertyChanged(nameof(AppDataFolder)); 
                }
        }
        public string CartridgeFileFilter { get { return "Cartridge Data Files (*.cdf)|*.cdf"; } }
        public string BallisticFileFilter { get { return "Ballistic Data Files (*.bdf)|*.bdf"; } }
        #endregion

        #region "Constructor"
        public DataPersistence ()
        {
            _AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _AppDataFolder = _AppDataFolder + "\\LawlerBallistics";
            if (!Directory.Exists(_AppDataFolder))
            {
                Directory.CreateDirectory(_AppDataFolder);
            }
            _DataFolder = "\\Data";
            _DataFolder = _DataFolder + "\\LawlerBallistics";
            if (!Directory.Exists(_DataFolder))
            {
                Directory.CreateDirectory(_DataFolder);
            }

        }
        #endregion

        #region "Public Routines"
        /// <summary>
        /// If this is a first run or something happened to the data files load the prepackaged files.
        /// </summary>
        public void CheckDataFiles()
        {
            //Load default data file for ballistic solution menu view
            string lDatFile = AppDataFolder + "\\default.bdf";
            string lSource = "Data/default.bdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Cartridge data file
             lDatFile = AppDataFolder + "\\CartridgeDB.cdf";
             lSource = "Data/CartridgeDB.cdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Bullet data file
            lDatFile = AppDataFolder + "\\BulletDB.bdf";
            lSource = "Data/BulletDB.bdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Case data file
            lDatFile = AppDataFolder + "\\CaseDB.cdf";
            lSource = "Data/CaseDB.cdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Gun data file
            lDatFile = AppDataFolder + "\\GunDB.gdf";
            lSource = "Data/GunDB.gdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Powder data file
            lDatFile = AppDataFolder + "\\PowderDB.ddf";
            lSource = "Data/PowderDB.ddf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Primer data file
            lDatFile = AppDataFolder + "\\PrimerDB.pdf";
            lSource = "Data/PrimerDB.pdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Recipe data file
            lDatFile = AppDataFolder + "\\RecipeDB.rdf";
            lSource = "Data/RecipeDB.rdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }
        }
        public int SaveCartridgeData()
        {
            int lRtn = 0;
            string lCdat; string lCF; string lCfilename; string lCfileBak;

            try
            {
                lCF = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>" + System.Environment.NewLine;
                lCF = lCF + "<catridgedatabasefile>" + System.Environment.NewLine;
                foreach (Cartridge iC in LawlerBallisticsFactory.MyCartridges)
                {
                    lCdat = CartridgeDatXML(iC);
                    lCF = lCF + lCdat;
                }
                lCF = lCF + "</catridgedatabasefile>" + System.Environment.NewLine;
                lCfilename = AppDataFolder + "\\CartridgeDB.cdf";
                lCfileBak = AppDataFolder + "\\CartridgeDB.BAK";
                if (File.Exists(lCfilename))
                {
                    if (File.Exists(lCfileBak))
                    {
                        File.Delete(lCfileBak);
                    }
                    File.Move(lCfilename, lCfileBak);
                    File.Delete(lCfilename);
                }
                File.WriteAllText(lCfilename, lCF);
            }            
            catch
            {
                lRtn = 1;
            }
            return lRtn;
        }
        public ObservableCollection<Cartridge> ParseCartridgeDB(bool LoadBAK = false)
        {
            ObservableCollection<Cartridge> lRtn = new ObservableCollection<Cartridge>();
            string lCDF = AppDataFolder + "\\CartridgeDB.cdf";
            if (LoadBAK) lCDF = AppDataFolder + "\\CartridgeDB.BAK";
            if(!File.Exists(lCDF)&!LoadBAK)
            {
                lRtn = ParseCartridgeDB(true);
                return lRtn;
            }
            else if(!File.Exists(lCDF) & LoadBAK)
            {
                return lRtn;
            }
            XmlDocument lXML = new XmlDocument();
            string lGF = File.ReadAllText(lCDF);

            lXML.LoadXml(lGF);
            XmlNode lCartridges = lXML.SelectSingleNode("catridgedatabasefile");         
            lRtn = LoadCartridges(lCartridges);

            return lRtn;
        }
        public int SaveBallisticSolutionData(Ballistics TargetBallisticData, string FileName)
        {
            int lRtn = 0;
            string lBfilename;  //Then path and file name of the the exported data file.
            string lBF;         //Balistic file string variable.

            try
            {
                lBF = "<Ballistic Solution File>" + System.Environment.NewLine;
                lBF = lBF + "<PROPERTIES>" + System.Environment.NewLine;
                lBF = lBF + "<zTargetLat>" + TargetBallisticData.zTargetLat.ToString() + "</zTargetLat>" + System.Environment.NewLine;
                lBF = lBF + "<zTargetLon>" + TargetBallisticData.zTargetLon.ToString() + "</zTargetLon>" + System.Environment.NewLine;
                lBF = lBF + "<zShooterLat>" + TargetBallisticData.zShooterLat.ToString() + "</zShooterLat>" + System.Environment.NewLine;
                lBF = lBF + "<zShooterLon>" + TargetBallisticData.zShooterLon.ToString() + "</zShooterLon>" + System.Environment.NewLine;
                lBF = lBF + "<ShooterLon>" + TargetBallisticData.ShooterLon.ToString() + "</ShooterLon>" + System.Environment.NewLine;
                lBF = lBF + "<ShooterLat>" + TargetBallisticData.ShooterLat.ToString() + "</ShooterLat>" + System.Environment.NewLine;
                lBF = lBF + "<ShotDistance>" + TargetBallisticData.ShotDistance.ToString() + "</ShotDistance>" + System.Environment.NewLine;
                lBF = lBF + "<ShotAngle>" + TargetBallisticData.ShotAngle.ToString() + "</ShotAngle>" + System.Environment.NewLine;
                lBF = lBF + "<TargetLat>" + TargetBallisticData.TargetLat.ToString() + "</TargetLat>" + System.Environment.NewLine;
                lBF = lBF + "<TargetLon>" + TargetBallisticData.TargetLon.ToString() + "</TargetLon>" + System.Environment.NewLine;
                lBF = lBF + "<RelHumidity>" + TargetBallisticData.RelHumidity.ToString() + "</RelHumidity>" + System.Environment.NewLine;
                lBF = lBF + "<zRelHumidity>" + TargetBallisticData.zRelHumidity.ToString() + "</zRelHumidity>" + System.Environment.NewLine;                
                lBF = lBF + "<zBaroPressure>" + TargetBallisticData.zBaroPressure.ToString() + "</zBaroPressure>" + System.Environment.NewLine;
                lBF = lBF + "<BaroPressure>" + TargetBallisticData.BaroPressure.ToString() + "</BaroPressure>" + System.Environment.NewLine;
                lBF = lBF + "<DensityAlt>" + TargetBallisticData.DensityAlt.ToString() + "</DensityAlt>" + System.Environment.NewLine;
                lBF = lBF + "<zDensityAlt>" + TargetBallisticData.zDensityAlt.ToString() + "</zDensityAlt>" + System.Environment.NewLine;
                lBF = lBF + "<TempF>" + TargetBallisticData.TempF.ToString() + "</TempF>" + System.Environment.NewLine;
                lBF = lBF + "<zTempF>" + TargetBallisticData.zTempF.ToString() + "</zTempF>" + System.Environment.NewLine;
                lBF = lBF + "<ScopeHeight>" + TargetBallisticData.ScopeHeight.ToString() + "</ScopeHeight>" + System.Environment.NewLine;
                lBF = lBF + "<MuzzleVelocity>" + TargetBallisticData.MuzzleVelocity.ToString() + "</MuzzleVelocity>" + System.Environment.NewLine;
                lBF = lBF + "<Fo>" + TargetBallisticData.Fo.ToString() + "</Fo>" + System.Environment.NewLine;
                lBF = lBF + "<F2>" + TargetBallisticData.F2.ToString() + "</F2>" + System.Environment.NewLine;
                lBF = lBF + "<F3>" + TargetBallisticData.F3.ToString() + "</F3>" + System.Environment.NewLine;
                lBF = lBF + "<F4>" + TargetBallisticData.F4.ToString() + "</F4>" + System.Environment.NewLine;
                lBF = lBF + "<V1>" + TargetBallisticData.V1.ToString() + "</V1>" + System.Environment.NewLine;
                lBF = lBF + "<V2>" + TargetBallisticData.V2.ToString() + "</V2>" + System.Environment.NewLine;
                lBF = lBF + "<D1>" + TargetBallisticData.D1.ToString() + "</D1>" + System.Environment.NewLine;
                lBF = lBF + "<D2>" + TargetBallisticData.D2.ToString() + "</D2>" + System.Environment.NewLine;
                lBF = lBF + "<BCg1>" + TargetBallisticData.BCg1.ToString() + "</BCg1>" + System.Environment.NewLine;
                lBF = lBF + "<BCz2>" + TargetBallisticData.BCz2.ToString() + "</BCz2>" + System.Environment.NewLine;
                lBF = lBF + "<ZeroMaxRise>" + TargetBallisticData.ZeroMaxRise.ToString() + "</ZeroMaxRise>" + System.Environment.NewLine;
                lBF = lBF + "<ZeroRange>" + TargetBallisticData.ZeroRange.ToString() + "</ZeroRange>" + System.Environment.NewLine;
                lBF = lBF + "<UseMaxRise>" + TargetBallisticData.UseMaxRise.ToString() + "</UseMaxRise>" + System.Environment.NewLine;

                //  These are now calculated by atmospheric conditions and Mach factors.
                //lBF = lBF + "<Zone1TransSpeed>" + TargetBallisticData.Zone1TransSpeed.ToString() + "</Zone1TransSpeed>" + System.Environment.NewLine;
                //lBF = lBF + "<Zone2TransSpeed>" + TargetBallisticData.Zone2TransSpeed.ToString() + "</Zone2TransSpeed>" + System.Environment.NewLine;
                //lBF = lBF + "<Zone3TransSpeed>" + TargetBallisticData.Zone3TransSpeed.ToString() + "</Zone3TransSpeed>" + System.Environment.NewLine;
                
                lBF = lBF + "<Zone1MachFactor>" + TargetBallisticData.Zone1MachFactor.ToString() + "</Zone1MachFactor>" + System.Environment.NewLine;
                lBF = lBF + "<Zone2MachFactor>" + TargetBallisticData.Zone2MachFactor.ToString() + "</Zone2MachFactor>" + System.Environment.NewLine;
                lBF = lBF + "<Zone3MachFactor>" + TargetBallisticData.Zone3MachFactor.ToString() + "</Zone3MachFactor>" + System.Environment.NewLine;
                lBF = lBF + "<Zone1SlopeMultiplier>" + TargetBallisticData.Zone1SlopeMultiplier.ToString() + "</Zone1SlopeMultiplier>" + System.Environment.NewLine;
                lBF = lBF + "<Zone3SlopeMultiplier>" + TargetBallisticData.Zone3SlopeMultiplier.ToString() + "</Zone3SlopeMultiplier>" + System.Environment.NewLine;
                lBF = lBF + "<Zone1Slope>" + TargetBallisticData.Zone1Slope.ToString() + "</Zone1Slope>" + System.Environment.NewLine;
                lBF = lBF + "<Zone1AngleFactor>" + TargetBallisticData.Zone1AngleFactor.ToString() + "</Zone1AngleFactor>" + System.Environment.NewLine;
                lBF = lBF + "<Zone3Slope>" + TargetBallisticData.Zone3Slope.ToString() + "</Zone3Slope>" + System.Environment.NewLine;
                lBF = lBF + "<BarrelTwist>" + TargetBallisticData.BarrelTwist.ToString() + "</BarrelTwist>" + System.Environment.NewLine;
                lBF = lBF + "<BarrelTwistDir>" + TargetBallisticData.BarrelTwistDir.ToString() + "</BarrelTwistDir>" + System.Environment.NewLine;
                lBF = lBF + "<BulletDiameter>" + TargetBallisticData.BulletDiameter.ToString() + "</BulletDiameter>" + System.Environment.NewLine;
                lBF = lBF + "<BulletLength>" + TargetBallisticData.BulletLength.ToString() + "</BulletLength>" + System.Environment.NewLine;
                lBF = lBF + "<BulletWeight>" + TargetBallisticData.BulletWeight.ToString() + "</BulletWeight>" + System.Environment.NewLine;

                // Calculated with bullet, barrel, and atmospheric conditions
                //lBF = lBF + "<BSG>" + TargetBallisticData.BSG.ToString() + "</BSG>" + System.Environment.NewLine;
                
                lBF = lBF + "<BulletShapeTyp>" + TargetBallisticData.BulletShapeTyp.ToString() + "</BulletShapeTyp>" + System.Environment.NewLine;
                lBF = lBF + "</PROPERTIES>" + System.Environment.NewLine;
                lBF = lBF + "</Ballistic Solution File>";
                lBfilename = FileName;
                if (File.Exists(lBfilename))
                {                    
                    File.Delete(lBfilename);
                }
                File.WriteAllText(lBfilename, lBF);
            }
            catch
            {
                lRtn = -1;
            }
            return lRtn;
        }
        public Ballistics ParseBallisticSolution(string FileName)
        {
            Ballistics lBSF = new Ballistics();
            string lBDF = FileName;
            string lValue;
            if (!File.Exists(lBDF) )
            {
                return lBSF;
            }            
            string[] BDFL = File.ReadAllLines(lBDF, Encoding.UTF8);
            if (!IsBallisticSol(BDFL))
            {               
                return lBSF;
            }
            foreach (string L in BDFL)
            {                
                if (L.StartsWith("<zTargetLat>"))
                {
                    lValue = L.Replace("<zTargetLat>", "");
                    lValue = lValue.Replace("</zTargetLat>", "");
                    lBSF.zTargetLat = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<zTargetLon>"))
                {
                    lValue = L.Replace("<zTargetLon>", "");
                    lValue = lValue.Replace("</zTargetLon>", "");
                    lBSF.zTargetLon = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<zShooterLat>"))
                {
                    lValue = L.Replace("<zShooterLat>", "");
                    lValue = lValue.Replace("</zShooterLat>", "");
                    lBSF.zTargetLon = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<zShooterLon>"))
                {
                    lValue = L.Replace("<zShooterLon>", "");
                    lValue = lValue.Replace("</zShooterLon>", "");
                    lBSF.zShooterLon = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<ShooterLon>"))
                {
                    lValue = L.Replace("<ShooterLon>", "");
                    lValue = lValue.Replace("</ShooterLon>", "");
                    lBSF.ShooterLon = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<ShooterLat>"))
                {
                    lValue = L.Replace("<ShooterLat>", "");
                    lValue = lValue.Replace("</ShooterLat>", "");
                    lBSF.ShooterLat = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<ShotDistance>"))
                {
                    lValue = L.Replace("<ShotDistance>", "");
                    lValue = lValue.Replace("</ShotDistance>", "");
                    lBSF.ShotDistance = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<ShotAngle>"))
                {
                    lValue = L.Replace("<ShotAngle>", "");
                    lValue = lValue.Replace("</ShotAngle>", "");
                    lBSF.ShotAngle = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<TargetLat>"))
                {
                    lValue = L.Replace("<TargetLat>", "");
                    lValue = lValue.Replace("</TargetLat>", "");
                    lBSF.TargetLat = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<TargetLon>"))
                {
                    lValue = L.Replace("<TargetLon>", "");
                    lValue = lValue.Replace("</TargetLon>", "");
                    lBSF.TargetLon = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<RelHumidity>"))
                {
                    lValue = L.Replace("<RelHumidity>", "");
                    lValue = lValue.Replace("</RelHumidity>", "");
                    lBSF.RelHumidity = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<zRelHumidity>"))
                {
                    lValue = L.Replace("<zRelHumidity>", "");
                    lValue = lValue.Replace("</zRelHumidity>", "");
                    lBSF.zRelHumidity = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<DensityAlt>"))
                {
                    lValue = L.Replace("<DensityAlt>", "");
                    lValue = lValue.Replace("</DensityAlt>", "");
                    lBSF.DensityAlt = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<zDensityAlt>"))
                {
                    lValue = L.Replace("<zDensityAlt>", "");
                    lValue = lValue.Replace("</zDensityAlt>", "");
                    lBSF.zDensityAlt = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<zBaroPressure>"))
                {
                    lValue = L.Replace("<zBaroPressure>", "");
                    lValue = lValue.Replace("</zBaroPressure>", "");
                    lBSF.zBaroPressure = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<BaroPressure>"))
                {
                    lValue = L.Replace("<BaroPressure>", "");
                    lValue = lValue.Replace("</BaroPressure>", "");
                    lBSF.BaroPressure = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<TempF>"))
                {
                    lValue = L.Replace("<TempF>", "");
                    lValue = lValue.Replace("</TempF>", "");
                    lBSF.TempF = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<zTempF>"))
                {
                    lValue = L.Replace("<zTempF>", "");
                    lValue = lValue.Replace("</zTempF>", "");
                    lBSF.zTempF = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<ScopeHeight>"))
                {
                    lValue = L.Replace("<ScopeHeight>", "");
                    lValue = lValue.Replace("</ScopeHeight>", "");
                    lBSF.ScopeHeight = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<MuzzleVelocity>"))
                {
                    lValue = L.Replace("<MuzzleVelocity>", "");
                    lValue = lValue.Replace("</MuzzleVelocity>", "");
                    lBSF.MuzzleVelocity = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<Fo>"))
                {
                    lValue = L.Replace("<Fo>", "");
                    lValue = lValue.Replace("</Fo>", "");
                    lBSF.Fo = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<F2>"))
                {
                    lValue = L.Replace("<F2>", "");
                    lValue = lValue.Replace("</F2>", "");
                    lBSF.F2 = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<F3>"))
                {
                    lValue = L.Replace("<F3>", "");
                    lValue = lValue.Replace("</F3>", "");
                    lBSF.F3 = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<F4>"))
                {
                    lValue = L.Replace("<F4>", "");
                    lValue = lValue.Replace("</F4>", "");
                    lBSF.F4 = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<V1>"))
                {
                    lValue = L.Replace("<V1>", "");
                    lValue = lValue.Replace("</V1>", "");
                    lBSF.V1 = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<V2>"))
                {
                    lValue = L.Replace("<V2>", "");
                    lValue = lValue.Replace("</V2>", "");
                    lBSF.V2 = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<D1>"))
                {
                    lValue = L.Replace("<D1>", "");
                    lValue = lValue.Replace("</D1>", "");
                    lBSF.D1 = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<D2>"))
                {
                    lValue = L.Replace("<D2>", "");
                    lValue = lValue.Replace("</D2>", "");
                    lBSF.D2 = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<BCg1>"))
                {
                    lValue = L.Replace("<BCg1>", "");
                    lValue = lValue.Replace("</BCg1>", "");
                    lBSF.BCg1 = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<BCz2>"))
                {
                    lValue = L.Replace("<BCz2>", "");
                    lValue = lValue.Replace("</BCz2>", "");
                    lBSF.BCz2 = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<ZeroMaxRise>"))
                {
                    lValue = L.Replace("<ZeroMaxRise>", "");
                    lValue = lValue.Replace("</ZeroMaxRise>", "");
                    lBSF.ZeroMaxRise = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<ZeroRange>"))
                {
                    lValue = L.Replace("<ZeroRange>", "");
                    lValue = lValue.Replace("</ZeroRange>", "");
                    lBSF.ZeroRange = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<UseMaxRise>"))
                {
                    lValue = L.Replace("<UseMaxRise>", "");
                    lValue = lValue.Replace("</UseMaxRise>", "");
                    lBSF.UseMaxRise = Convert.ToBoolean(lValue);
                }
                //  These are calculated from atmospheric data and Mach factors now.
                //else if (L.StartsWith("<Zone1TransSpeed>"))
                //{
                //    lValue = L.Replace("<Zone1TransSpeed>", "");
                //    lValue = lValue.Replace("</Zone1TransSpeed>", "");
                //    lBSF.Zone1TransSpeed = Convert.ToDouble(lValue);
                //}
                //else if (L.StartsWith("<Zone2TransSpeed>"))
                //{
                //    lValue = L.Replace("<Zone2TransSpeed>", "");
                //    lValue = lValue.Replace("</Zone2TransSpeed>", "");
                //    lBSF.Zone2TransSpeed = Convert.ToDouble(lValue);
                //}
                //else if (L.StartsWith("<Zone3TransSpeed>"))
                //{
                //    lValue = L.Replace("<Zone3TransSpeed>", "");
                //    lValue = lValue.Replace("</Zone3TransSpeed>", "");
                //    lBSF.Zone3TransSpeed = Convert.ToDouble(lValue);
                //}
                else if (L.StartsWith("<Zone1MachFactor>"))
                {
                    lValue = L.Replace("<Zone1MachFactor>", "");
                    lValue = lValue.Replace("</Zone1MachFactor>", "");
                    lBSF.Zone1MachFactor = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<Zone2MachFactor>"))
                {
                    lValue = L.Replace("<Zone2MachFactor>", "");
                    lValue = lValue.Replace("</Zone2MachFactor>", "");
                    lBSF.Zone2MachFactor = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<Zone3MachFactor>"))
                {
                    lValue = L.Replace("<Zone3MachFactor>", "");
                    lValue = lValue.Replace("</Zone3MachFactor>", "");
                    lBSF.Zone3MachFactor = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<Zone1SlopeMultiplier>"))
                {
                    lValue = L.Replace("<Zone1SlopeMultiplier>", "");
                    lValue = lValue.Replace("</Zone1SlopeMultiplier>", "");
                    lBSF.Zone1SlopeMultiplier = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<Zone3SlopeMultiplier>"))
                {
                    lValue = L.Replace("<Zone3SlopeMultiplier>", "");
                    lValue = lValue.Replace("</Zone3SlopeMultiplier>", "");
                    lBSF.Zone3SlopeMultiplier = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<Zone1Slope>"))
                {
                    lValue = L.Replace("<Zone1Slope>", "");
                    lValue = lValue.Replace("</Zone1Slope>", "");
                    lBSF.Zone1Slope = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<Zone1AngleFactor>"))
                {
                    lValue = L.Replace("<Zone1AngleFactor>", "");
                    lValue = lValue.Replace("</Zone1AngleFactor>", "");
                    lBSF.Zone1AngleFactor = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<Zone3Slope>"))
                {
                    lValue = L.Replace("<Zone3Slope>", "");
                    lValue = lValue.Replace("</Zone3Slope>", "");
                    lBSF.Zone3Slope = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<BarrelTwist>"))
                {
                    lValue = L.Replace("<BarrelTwist>", "");
                    lValue = lValue.Replace("</BarrelTwist>", "");
                    lBSF.BarrelTwist = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<BarrelTwistDir>"))
                {
                    lValue = L.Replace("<BarrelTwistDir>", "");
                    lValue = lValue.Replace("</BarrelTwistDir>", "");
                    lBSF.BarrelTwistDir = Convert.ToString(lValue);
                }
                else if (L.StartsWith("<BulletDiameter>"))
                {
                    lValue = L.Replace("<BulletDiameter>", "");
                    lValue = lValue.Replace("</BulletDiameter>", "");
                    lBSF.BulletDiameter = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<BulletLength>"))
                {
                    lValue = L.Replace("<BulletLength>", "");
                    lValue = lValue.Replace("</BulletLength>", "");
                    lBSF.BulletLength = Convert.ToDouble(lValue);
                }
                else if (L.StartsWith("<BulletWeight>"))
                {
                    lValue = L.Replace("<BulletWeight>", "");
                    lValue = lValue.Replace("</BulletWeight>", "");
                    lBSF.BulletWeight = Convert.ToDouble(lValue);
                }
                // Value is not a calculation
                //else if (L.StartsWith("<BSG>"))
                //{
                //    lValue = L.Replace("<BSG>", "");
                //    lValue = lValue.Replace("</BSG>", "");
                //    lBSF.BSG = Convert.ToDouble(lValue);
                //}
                else if (L.StartsWith("<BulletShapeTyp>"))
                {
                    lValue = L.Replace("<BulletShapeTyp>", "");
                    lValue = lValue.Replace("</BulletShapeTyp>", "");
                    lBSF.BulletShapeTyp = (BulletShapeEnum) Enum.Parse(typeof(BulletShapeEnum), lValue);
                }
            }
            return lBSF;
        }
        public int SaveGunDB()
        {
            int lRtn = 0;
            string lGfilename;  //The path and file name of the the exported data file.
            string lGfileBak;   //The path and file name for the the backup data file.
            string lGF;         //Gun file string variable.
            string lsbid;

            //try
            //{
                lGfilename = AppDataFolder + "\\GunDB.gdf";
                lGfileBak = AppDataFolder + "\\GunDB.BAK";
                lGF = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>" + System.Environment.NewLine;
                lGF = lGF + "<gundatabasefile>" + System.Environment.NewLine;
                foreach (Gun lg in LawlerBallisticsFactory.MyGuns)
                {
                    lsbid = "";
                    lGF = lGF + "<gun>" + System.Environment.NewLine;
                    lGF = lGF + "<id value=\"" + lg.ID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</id>" + System.Environment.NewLine;
                    lGF = lGF + "<name value=\"" + lg.Name.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</name>" + System.Environment.NewLine;
                    lGF = lGF + "<description value=\"" + lg.Description.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</description>" + System.Environment.NewLine;
                    lGF = lGF + "<make value=\"" + lg.Make.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</make>" + System.Environment.NewLine;
                    lGF = lGF + "<model value=\"" + lg.Model.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</model>" + System.Environment.NewLine;
                    if (lg.SelectedBarrel != null) lsbid = lg.SelectedBarrel.ID;
                    lGF = lGF + "<selectedbarrelid value=\"" + lsbid + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</selectedbarrelid>" + System.Environment.NewLine;
                    lGF = lGF + "<gunPic value=\"" + BitConverter.ToString(LawlerBallisticsFactory.ImageToBytes(lg.GunPic)) + "\" type='bytearray'>" + System.Environment.NewLine;
                    lGF = lGF + "</gunPic>" + System.Environment.NewLine;
                    lGF = lGF + "<barrels>" + System.Environment.NewLine;
                    foreach(Barrel lb in lg.Barrels)
                    {
                        lGF = lGF + "<barrel>" + System.Environment.NewLine;
                        lGF = lGF + "<id value=\"" + lb.ID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</id>" + System.Environment.NewLine;
                        lGF = lGF + "<name value=\"" + lb.Name.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</name>" + System.Environment.NewLine;
                        lGF = lGF + "<make value=\"" + lb.Make.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</make>" + System.Environment.NewLine;
                        lGF = lGF + "<model value=\"" + lb.Model.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</model>" + System.Environment.NewLine;
                        lGF = lGF + "<description value=\"" + lb.Description.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</description>" + System.Environment.NewLine;
                        try
                        {
                            lGF = lGF + "<cartridgeid value=\"" + lb.ParentCartridge.ID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                            lGF = lGF + "</cartridgeid>" + System.Environment.NewLine;
                        }
                        catch
                        {
                            //Parent Cartridge not specified.
                        }
                        lGF = lGF + "<twist value=\"" + lb.Twist.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                        lGF = lGF + "</twist>" + System.Environment.NewLine;
                        lGF = lGF + "<twistdir value=\"" + lb.RiflingTwistDirection.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</twistdir>" + System.Environment.NewLine;
                        lGF = lGF + "<length value=\"" + lb.Length.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                        lGF = lGF + "</length>" + System.Environment.NewLine;
                        lGF = lGF + "<headspace value=\"" + lb.HeadSpace.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                        lGF = lGF + "</headspace>" + System.Environment.NewLine;
                        lGF = lGF + "<neckdepth value=\"" + lb.NeckDepth.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                        lGF = lGF + "</neckdepth>" + System.Environment.NewLine;
                        lGF = lGF + "<neckdiameter value=\"" + lb.NeckDiameter.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                        lGF = lGF + "</neckdiameter>" + System.Environment.NewLine;
                    try
                    {
                            lGF = lGF + "<selectedloadid value=\"" + lb.SelectedRecipe.ID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                            lGF = lGF + "</selectedloadid>" + System.Environment.NewLine;
                        }
                        catch
                        {
                            // No load selected
                        }
                        lGF = lGF + "</barrel>" + System.Environment.NewLine;
                    }
                    lGF = lGF + "</barrels>" + System.Environment.NewLine;
                    lGF = lGF + "</gun>" + System.Environment.NewLine;
                }
                lGF = lGF + "</gundatabasefile>";
                if (File.Exists(lGfilename))
                {
                    if (File.Exists(lGfileBak))
                    {
                        File.Delete(lGfileBak);
                    }
                    File.Move(lGfilename, lGfileBak);
                    File.Delete(lGfilename);
                }
                File.WriteAllText(lGfilename, lGF);
            //}
            //catch
            //{
            //    lRtn = -1;
            //}

            return lRtn;
        }
        public ObservableCollection<Gun> ParseGunDB(bool LoadBAK = false)
        {
            ObservableCollection<Gun> lGDB = new ObservableCollection<Gun>();
            string lGfilename;  //The path and file name of the the exported data file.
            string lGF; // string containing gun data from file.
            string lNR;
            string lsb;

            lGfilename = AppDataFolder + "\\GunDB.gdf";
            lGF = File.ReadAllText(lGfilename);
            XmlDocument lXML = new XmlDocument();

            lXML.LoadXml(lGF);
            XmlNode lGun = lXML.SelectSingleNode("gundatabasefile");
            foreach (XmlNode lgn in lGun)
            {
                Gun ltg = new Gun();
                lsb = "";
                XmlNode lName = lgn.SelectSingleNode("name");
                lNR = lName.Attributes["value"].Value;
                ltg.Name = lNR;
                XmlNode lid = lgn.SelectSingleNode("id");
                lNR = lid.Attributes["value"].Value;
                ltg.ID = lNR;
                XmlNode lmk = lgn.SelectSingleNode("make");
                lNR = lmk.Attributes["value"].Value;
                ltg.Make = lNR;
                XmlNode lmdl = lgn.SelectSingleNode("model");
                lNR = lmdl.Attributes["value"].Value;
                ltg.Model = lNR;
                try
                {
                    XmlNode lsbid = lgn.SelectSingleNode("selectedbarrelid");
                    lNR = lsbid.Attributes["value"].Value;
                    lsb = lNR;
                }
                catch
                {
                    //selectedbarrelid doesnt exist
                }
                try
                {
                    XmlNode lgp = lgn.SelectSingleNode("gunPic");
                    lNR = lgp.Attributes["value"].Value;
                    byte[] lba = LawlerBallisticsFactory.StringToByteArray(lNR);
                    Image lx = (Bitmap)((new ImageConverter()).ConvertFrom(lba));
                    ltg.GunPic = lx;
                }
                catch
                {
                    // Image issue or no image
                }
                XmlNode ldesc = lgn.SelectSingleNode("description");
                lNR = ldesc.Attributes["value"].Value;
                ltg.Description = lNR;
                XmlNode lbsn = lgn.SelectSingleNode("barrels");
                foreach (XmlNode lbn in lbsn)
                {
                    Barrel lbarrel = new Barrel();
                    
                    XmlNode lBid = lbn.SelectSingleNode("id");
                    if (lBid != null) lbarrel.ID = lBid.Attributes["value"].Value;
                    XmlNode lBname = lbn.SelectSingleNode("name");
                    if (lBname != null) lbarrel.Name = lBname.Attributes["value"].Value;
                    XmlNode lBmake = lbn.SelectSingleNode("make");
                    if (lBmake != null) lbarrel.Make = lBmake.Attributes["value"].Value;
                    XmlNode lBmodel = lbn.SelectSingleNode("model");
                    if (lBmodel != null) lbarrel.Model = lBmodel.Attributes["value"].Value;
                    XmlNode lBdesc = lbn.SelectSingleNode("description");
                    if(lBdesc != null) lbarrel.Description = lBdesc.Attributes["value"].Value;
                    XmlNode lBtwist = lbn.SelectSingleNode("twist");
                    if (lBtwist != null) lbarrel.Twist = Convert.ToDouble(lBtwist.Attributes["value"].Value);
                    XmlNode ltd = lbn.SelectSingleNode("twistdir");
                    if (ltd != null) lbarrel.RiflingTwistDirection = ltd.Attributes["value"].Value;
                    XmlNode llgth = lbn.SelectSingleNode("length");
                    if (llgth != null) lbarrel.Length = Convert.ToDouble(llgth.Attributes["value"].Value);
                    XmlNode lhs = lbn.SelectSingleNode("headspace");
                    if (lhs != null) lbarrel.HeadSpace = Convert.ToDouble(lhs.Attributes["value"].Value);
                    XmlNode lnd = lbn.SelectSingleNode("neckdepth");
                    if (lnd != null) lbarrel.NeckDepth = Convert.ToDouble(lnd.Attributes["value"].Value);
                    XmlNode ldia = lbn.SelectSingleNode("neckdiameter");
                    if (ldia != null) lbarrel.NeckDiameter = Convert.ToDouble(ldia.Attributes["value"].Value);
                    XmlNode lcarid = lbn.SelectSingleNode(" cartridgeid");
                    if (lcarid != null) lbarrel.CartridgeID = lcarid.Attributes["value"].Value;
                    foreach(Cartridge lcartridge in LawlerBallisticsFactory.MyCartridges)
                    {
                        if(lbarrel.CartridgeID == lcartridge.ID)
                        {
                            lbarrel.ParentCartridge = lcartridge;
                            break;
                        }
                    }
                    ltg.Barrels.Add(lbarrel);
                }
                foreach(Barrel lb in ltg.Barrels)
                {
                    if(lb.ID == lsb)
                    {
                        ltg.SelectedBarrel = lb;
                        break;
                    }
                }
                lGDB.Add(ltg);
            }
            return lGDB;
        }
        public int SaveRecipeDB()
        {
            int lRtn = 0;
            string lGfilename;  //The path and file name of the the exported data file.
            string lGfileBak;   //The path and file name for the the backup data file.
            string lGF;         //Gun file string variable.

            //try
            //{
                lGfilename = AppDataFolder + "\\RecipeDB.rdf";
                lGfileBak = AppDataFolder + "\\RecipeDB.BAK";
                lGF = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>" + System.Environment.NewLine;
                lGF = lGF + "<recipedatabasefile>" + System.Environment.NewLine;
                foreach (Recipe lr in LawlerBallisticsFactory.MyRecipes)
                {
                    lGF = lGF + "<recipe>" + System.Environment.NewLine;
                    lGF = lGF + "<id value=\"" + lr.ID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</id>" + System.Environment.NewLine;
                    lGF = lGF + "<name value=\"" + lr.Name.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</name>" + System.Environment.NewLine;
                    lGF = lGF + "<notes value=\"" + lr.Notes.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</notes>" + System.Environment.NewLine;
                    lGF = lGF + "<barrelid value=\"" + lr.BarrelID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</barrelid>" + System.Environment.NewLine;
                    lGF = lGF + "<bulletid value=\"" + lr.BulletID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</bulletid>" + System.Environment.NewLine;
                    lGF = lGF + "<bulletsbto value=\"" + lr.BulletSortBTO.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</bulletsbto>" + System.Environment.NewLine;
                    lGF = lGF + "<bulletsoal value=\"" + lr.BulletSortOAL.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</bulletsoal>" + System.Environment.NewLine;
                    lGF = lGF + "<bulletswt value=\"" + lr.BulletSortWt.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</bulletswt>" + System.Environment.NewLine;
                    lGF = lGF + "<cartid value=\"" + lr.CartridgeID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</cartid>" + System.Environment.NewLine;
                    lGF = lGF + "<caseid value=\"" + lr.CaseID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</caseid>" + System.Environment.NewLine;
                    lGF = lGF + "<casetl value=\"" + lr.CaseTrimLength.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</casetl>" + System.Environment.NewLine;
                    lGF = lGF + "<cbto value=\"" + lr.CBTO.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</cbto>" + System.Environment.NewLine;
                    lGF = lGF + "<chargewt value=\"" + lr.ChargeWt.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</chargewt>" + System.Environment.NewLine;
                    lGF = lGF + "<coal value=\"" + lr.COAL.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</coal>" + System.Environment.NewLine;
                    lGF = lGF + "<dorate value=\"" + lr.FoRate.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</dorate>" + System.Environment.NewLine;
                    lGF = lGF + "<gunid value=\"" + lr.GunID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</gunid>" + System.Environment.NewLine;
                    lGF = lGF + "<headspace value=\"" + lr.HeadSpace.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</headspace>" + System.Environment.NewLine;
                    lGF = lGF + "<jump value=\"" + lr.JumpDistance.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</jump>" + System.Environment.NewLine;
                    lGF = lGF + "<neckclearance value=\"" + lr.NeckClearance.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</neckclearance>" + System.Environment.NewLine;
                    lGF = lGF + "<powderid value=\"" + lr.PowderID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</powderid>" + System.Environment.NewLine;
                    lGF = lGF + "<primerid value=\"" + lr.PrimerID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                    lGF = lGF + "</primerid>" + System.Environment.NewLine;
                    lGF = lGF + "<Lots>" + System.Environment.NewLine;
                    foreach(RecipeLot ll in lr.Lots)
                    {
                        lGF = lGF + "<Lot>" + System.Environment.NewLine;
                        lGF = lGF + "<RecipeID value=\"" + ll.RecipeID + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</RecipeID>" + System.Environment.NewLine;
                        lGF = lGF + "<BulletLot value=\"" + ll.BulletLot + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</BulletLot>" + System.Environment.NewLine;
                        lGF = lGF + "<CaseLot value=\"" + ll.CaseLot + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</CaseLot>" + System.Environment.NewLine;
                        lGF = lGF + "<ID value=\"" + ll.ID + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</ID>" + System.Environment.NewLine;
                        lGF = lGF + "<LotDate value=\"" + ll.LotDate + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</LotDate>" + System.Environment.NewLine;
                        lGF = lGF + "<PowderLot value=\"" + ll.PowderLot + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</PowderLot>" + System.Environment.NewLine;
                        lGF = lGF + "<PrimerLot value=\"" + ll.PrimerLot + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</PrimerLot>" + System.Environment.NewLine;
                        lGF = lGF + "<SerialNo value=\"" + ll.SerialNo + "\" type=\"string\">" + System.Environment.NewLine;
                        lGF = lGF + "</SerialNo>" + System.Environment.NewLine;
                        lGF = lGF + "<rounds>" + System.Environment.NewLine;
                        foreach(Round lrnd in ll.Rounds)
                        {
                            lGF = lGF + "<round>" + System.Environment.NewLine;
                            lGF = lGF + "<ID value=\"" + lrnd.ID + "\" type=\"string\">" + System.Environment.NewLine;
                            lGF = lGF + "</ID>" + System.Environment.NewLine;
                            lGF = lGF + "<RndNo value=\"" + lrnd.RndNo.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                            lGF = lGF + "</RndNo>" + System.Environment.NewLine;
                            lGF = lGF + "<BBTO value=\"" + lrnd.BBTO.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</BBTO>" + System.Environment.NewLine;
                            lGF = lGF + "<BD value=\"" + lrnd.BD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</BD>" + System.Environment.NewLine;
                            lGF = lGF + "<BL value=\"" + lrnd.BL.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</BL>" + System.Environment.NewLine;
                            lGF = lGF + "<BW value=\"" + lrnd.BW.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</BW>" + System.Environment.NewLine;
                            lGF = lGF + "<CHS value=\"" + lrnd.CHS.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</CHS>" + System.Environment.NewLine;
                            lGF = lGF + "<CL value=\"" + lrnd.CL.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</CL>" + System.Environment.NewLine;
                            lGF = lGF + "<CNOD value=\"" + lrnd.CNOD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</CNOD>" + System.Environment.NewLine;
                            lGF = lGF + "<CVW value=\"" + lrnd.CVW.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</CVW>" + System.Environment.NewLine;
                            lGF = lGF + "<CW value=\"" + lrnd.CW.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</CW>" + System.Environment.NewLine;
                            lGF = lGF + "<CNID value=\"" + lrnd.CNID.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</CNID>" + System.Environment.NewLine;
                            lGF = lGF + "<PCW value=\"" + lrnd.PCW.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</PCW>" + System.Environment.NewLine;
                            lGF = lGF + "<CBTO value=\"" + lrnd.CBTO.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</CBTO>" + System.Environment.NewLine;
                            lGF = lGF + "<COAL value=\"" + lrnd.COAL.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</COAL>" + System.Environment.NewLine;
                            lGF = lGF + "<MV value=\"" + lrnd.MV.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</MV>" + System.Environment.NewLine;
                            lGF = lGF + "<VD value=\"" + lrnd.VD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</VD>" + System.Environment.NewLine;
                            lGF = lGF + "<VD value=\"" + lrnd.VD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</VD>" + System.Environment.NewLine;
                            lGF = lGF + "<HD value=\"" + lrnd.HD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</HD>" + System.Environment.NewLine;
                            lGF = lGF + "<VAD value=\"" + lrnd.VAD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</VAD>" + System.Environment.NewLine;
                            lGF = lGF + "<HAD value=\"" + lrnd.HAD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</HAD>" + System.Environment.NewLine;
                            lGF = lGF + "<RMSD value=\"" + lrnd.RMSD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</RMSD>" + System.Environment.NewLine;
                            lGF = lGF + "<GDV value=\"" + lrnd.GDV.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</GDV>" + System.Environment.NewLine;
                            lGF = lGF + "<VELAD value=\"" + lrnd.VELAD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
                            lGF = lGF + "</VELAD>" + System.Environment.NewLine;
                            lGF = lGF + "</round>" + System.Environment.NewLine;
                        }
                        lGF = lGF + "</rounds>" + System.Environment.NewLine;
                        lGF = lGF + "</Lot>" + System.Environment.NewLine;
                    }
                    lGF = lGF + "</Lots>" + System.Environment.NewLine;
                    lGF = lGF + "</recipe>" + System.Environment.NewLine;
                }
                lGF = lGF + "</recipedatabasefile>";
                if (File.Exists(lGfilename))
                {
                    if (File.Exists(lGfileBak))
                    {
                        File.Delete(lGfileBak);
                    }
                    File.Move(lGfilename, lGfileBak);
                    File.Delete(lGfilename);
                }
                File.WriteAllText(lGfilename, lGF);
            //}
            //catch
            //{
            //    lRtn = -1;
            //}

            return lRtn;
        }
        public ObservableCollection<Recipe> ParseRecipeDB(bool LoadBak = false)
        {
            ObservableCollection<Recipe> lLR = new ObservableCollection<Recipe>();
            string lGfilename;  //The path and file name of the the exported data file.
            string lGF; // string containing recipe data from file.
            string lNR;

            //try
            //{
                lGfilename = AppDataFolder + "\\RecipeDB.rdf";
                lGF = File.ReadAllText(lGfilename);
                XmlDocument lXML = new XmlDocument();

                lXML.LoadXml(lGF);
                XmlNode lRecipe = lXML.SelectSingleNode("recipedatabasefile");
                foreach (XmlNode lgn in lRecipe)
                {
                    Recipe ltg = new Recipe();

                    XmlNode lName = lgn.SelectSingleNode("name");
                    lNR = lName.Attributes["value"].Value;
                    ltg.Name = lNR;
                    XmlNode lid = lgn.SelectSingleNode("id");
                    lNR = lid.Attributes["value"].Value;
                    ltg.ID = lNR;
                    XmlNode lmk = lgn.SelectSingleNode("notes");
                    lNR = lmk.Attributes["value"].Value;
                    ltg.Notes = lNR;
                    XmlNode lmdl = lgn.SelectSingleNode("barrelid");
                    lNR = lmdl.Attributes["value"].Value;
                    ltg.BarrelID = lNR;                    
                    XmlNode ldesc = lgn.SelectSingleNode("bulletid");
                    lNR = ldesc.Attributes["value"].Value;
                    ltg.BulletID = lNR;
                    ltg.RecpBullet = LawlerBallisticsFactory.GetBullet(ltg.BulletID);
                    XmlNode lbulletsbto = lgn.SelectSingleNode("bulletsbto");
                    lNR = lbulletsbto.Attributes["value"].Value;
                    ltg.BulletSortBTO = Convert.ToDouble(lNR);
                    XmlNode lbulletsoal = lgn.SelectSingleNode("bulletsoal");
                    lNR = lbulletsoal.Attributes["value"].Value;
                    ltg.BulletSortOAL = Convert.ToDouble(lNR);
                    XmlNode lbulletswt = lgn.SelectSingleNode("bulletswt");
                    lNR = lbulletswt.Attributes["value"].Value;
                    ltg.BulletSortWt = Convert.ToDouble(lNR);
                    XmlNode lcartid = lgn.SelectSingleNode("cartid");
                    lNR = lcartid.Attributes["value"].Value;
                    ltg.CartridgeID = lNR;
                    ltg.RecpCartridge = LawlerBallisticsFactory.GetCartridge(ltg.CartridgeID);
                    XmlNode lcaseID = lgn.SelectSingleNode("caseid");
                    lNR = lcaseID.Attributes["value"].Value;
                    ltg.CaseID = lNR;
                    ltg.RecpCase = LawlerBallisticsFactory.GetCase(ltg.CaseID);
                    XmlNode lcasetl = lgn.SelectSingleNode("casetl");
                    lNR = lcasetl.Attributes["value"].Value;
                    ltg.CaseTrimLength = Convert.ToDouble(lNR);
                    XmlNode lcbto = lgn.SelectSingleNode("cbto");
                    lNR = lcbto.Attributes["value"].Value;
                    ltg.CBTO = Convert.ToDouble(lNR);
                    XmlNode lchargewt = lgn.SelectSingleNode("chargewt");
                    lNR = lchargewt.Attributes["value"].Value;
                    ltg.ChargeWt = Convert.ToDouble(lNR);
                    XmlNode lcoal = lgn.SelectSingleNode("coal");
                    lNR = lcoal.Attributes["value"].Value;
                    ltg.COAL = Convert.ToDouble(lNR);
                    XmlNode ldorate = lgn.SelectSingleNode("dorate");
                    lNR = ldorate.Attributes["value"].Value;
                    ltg.FoRate = Convert.ToDouble(lNR);
                    XmlNode lgunid = lgn.SelectSingleNode("gunid");
                    lNR = lgunid.Attributes["value"].Value;
                    ltg.GunID = lNR;
                    XmlNode lheadspace = lgn.SelectSingleNode("headspace");
                    lNR = lheadspace.Attributes["value"].Value;
                    ltg.HeadSpace = Convert.ToDouble(lNR);
                    XmlNode ljump = lgn.SelectSingleNode("jump");
                    lNR = ljump.Attributes["value"].Value;
                    ltg.JumpDistance = Convert.ToDouble(lNR);
                    XmlNode lneckclearance = lgn.SelectSingleNode("neckclearance");
                    lNR = lneckclearance.Attributes["value"].Value;
                    ltg.NeckClearance = Convert.ToDouble(lNR);
                    XmlNode lpowderid = lgn.SelectSingleNode("powderid");
                    lNR = lpowderid.Attributes["value"].Value;
                    ltg.PowderID = lNR;
                    ltg.RecpPowder = LawlerBallisticsFactory.GetPowder(ltg.PowderID);
                    XmlNode lprimermfgr = lgn.SelectSingleNode("primerid");
                    lNR = lprimermfgr.Attributes["value"].Value;
                    ltg.PrimerID = lNR;
                    ltg.RecpPrimer = LawlerBallisticsFactory.GetPrimer(ltg.PrimerID);

                    ltg.Lots = new ObservableCollection<RecipeLot>();
                    XmlNode lLots = lgn.SelectSingleNode("Lots");
                    XmlNode lLotProp;
                    string lLotPropVal;
                    RecipeLot lRLC;
                    if(lLots != null)
                    {
                        foreach (XmlNode lLot in lLots)
                        {
                            lRLC = new RecipeLot();
                            lLotPropVal = "";
                            lLotProp = lLot.SelectSingleNode("RecipeID");
                            lLotPropVal = lLotProp.Attributes["value"].Value;
                            lRLC.RecipeID = lLotPropVal;
                            lLotPropVal = "";
                            lLotProp = lLot.SelectSingleNode("BulletLot");
                            lLotPropVal = lLotProp.Attributes["value"].Value;
                            lRLC.BulletLot = lLotPropVal;
                            lLotPropVal = "";
                            lLotProp = lLot.SelectSingleNode("CaseLot");
                            lLotPropVal = lLotProp.Attributes["value"].Value;
                            lRLC.CaseLot = lLotPropVal;
                            lLotPropVal = "";
                            lLotProp = lLot.SelectSingleNode("ID");
                            lLotPropVal = lLotProp.Attributes["value"].Value;
                            lRLC.ID = lLotPropVal;
                            lLotPropVal = "";
                            lLotProp = lLot.SelectSingleNode("LotDate");
                            lLotPropVal = lLotProp.Attributes["value"].Value;
                            lRLC.LotDate = lLotPropVal;
                            lLotPropVal = "";
                            lLotProp = lLot.SelectSingleNode("PowderLot");
                            lLotPropVal = lLotProp.Attributes["value"].Value;
                            lRLC.PowderLot = lLotPropVal;
                            lLotPropVal = "";
                            lLotProp = lLot.SelectSingleNode("PrimerLot");
                            lLotPropVal = lLotProp.Attributes["value"].Value;
                            lRLC.PrimerLot = lLotPropVal;
                            lLotPropVal = "";
                            lLotProp = lLot.SelectSingleNode("SerialNo");
                            lLotPropVal = lLotProp.Attributes["value"].Value;
                            lRLC.SerialNo = lLotPropVal;

                            lRLC.Rounds = new ObservableCollection<Round>();
                            XmlNode lRds = lLot.SelectSingleNode("rounds");
                            XmlNode lRdprop;
                            string lRdPropVal;
                            Round lRnd;
                            if (lRds != null)
                            {
                                foreach (XmlNode lrd in lRds)
                                {
                                    lRnd = new Round();
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("ID");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.ID = lRdPropVal;
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("RndNo");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.RndNo = Convert.ToInt32(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("BBTO");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.BBTO = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("BD");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.BD = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("BL");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.BL = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("BW");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.BW = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("CHS");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.CHS = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("CL");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.CL = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("CNOD");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.CNOD = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("CNID");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.CNID = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("CVW");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.CVW = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("CW");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.CW = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("PCW");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.PCW = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("CBTO");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.CBTO = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("COAL");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.COAL = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("MV");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.MV = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("VD");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.VD = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("HD");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.HD = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("VAD");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.VAD = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("HAD");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.HAD = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("RMSD");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.RMSD = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("GDV");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.GDV = Convert.ToDouble(lRdPropVal);
                                    lRdPropVal = "";
                                    lRdprop = lrd.SelectSingleNode("VELAD");
                                    lRdPropVal = lRdprop.Attributes["value"].Value;
                                    lRnd.VELAD = Convert.ToDouble(lRdPropVal);
                                    lRLC.Rounds.Add(lRnd);
                                }
                            }
                            ltg.Lots.Add(lRLC);
                        }
                    }
                    lLR.Add(ltg);
                }
            //}
            //catch
            //{
            //    //opps file likely doesn't exist yet.
            //}
            return lLR;
        }
        public int SaveBulletDB()
        {
            int lRtn = 0;
            string lGfilename;  //The path and file name of the the exported data file.
            string lGfileBak;   //The path and file name for the the backup data file.
            string lGF;         //Gun file string variable.

            try
            {
                lGfilename = AppDataFolder + "\\BulletDB.bdf";
                lGfileBak = AppDataFolder + "\\BulletDB.BAK";
                lGF = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>" + System.Environment.NewLine;
                lGF = lGF + "<bulletdatabasefile>" + System.Environment.NewLine;
                foreach (Bullet lg in LawlerBallisticsFactory.MyBullets)
                {
                    lGF = lGF + "<bullet>" + System.Environment.NewLine;
                    lGF = lGF + "<id value=\"" + lg.ID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</id>" + System.Environment.NewLine;
                    lGF = lGF + "<bcg1 value=\"" + lg.BCg1.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</bcg1>" + System.Environment.NewLine;
                    lGF = lGF + "<bcg7 value=\"" + lg.BCg7.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</bcg7>" + System.Environment.NewLine;
                    lGF = lGF + "<diameter value=\"" + lg.Diameter.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</diameter>" + System.Environment.NewLine;
                    lGF = lGF + "<length value=\"" + lg.Length.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</length>" + System.Environment.NewLine;
                    lGF = lGF + "<bto value=\"" + lg.BTO.ToString() + "\" type='double'>" + System.Environment.NewLine;
                    lGF = lGF + "</bto>" + System.Environment.NewLine;
                    lGF = lGF + "<model value=\"" + lg.Model.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</model>" + System.Environment.NewLine;
                    lGF = lGF + "<manufacturer value=\"" + lg.Manufacturer.ToString() + "\" type='bytearray'>" + System.Environment.NewLine;
                    lGF = lGF + "</manufacturer>" + System.Environment.NewLine;
                    lGF = lGF + "<type value=\"" + lg.Type.ToString() + "\" type='bytearray'>" + System.Environment.NewLine;
                    lGF = lGF + "</type>" + System.Environment.NewLine;
                    lGF = lGF + "<weight value=\"" + lg.Weight.ToString() + "\" type='bytearray'>" + System.Environment.NewLine;
                    lGF = lGF + "</weight>" + System.Environment.NewLine;
                    lGF = lGF + "</bullet>" + System.Environment.NewLine;
                }
                lGF = lGF + "</bulletdatabasefile>";
                if (File.Exists(lGfilename))
                {
                    if (File.Exists(lGfileBak))
                    {
                        File.Delete(lGfileBak);
                    }
                    File.Move(lGfilename, lGfileBak);
                    File.Delete(lGfilename);
                }
                File.WriteAllText(lGfilename, lGF);
            }
            catch
            {
                lRtn = -1;
            }

            return lRtn;
        }
        public ObservableCollection<Bullet> ParseBulletsDB(bool LoadBak = false)
        {
            ObservableCollection<Bullet> lLR = new ObservableCollection<Bullet>();
            string lGfilename;  //The path and file name of the the exported data file.
            string lGF; // string containing recipe data from file.
            string lNR;

            try
            {
                lGfilename = AppDataFolder + "\\BulletDB.bdf";
                lGF = File.ReadAllText(lGfilename);
                XmlDocument lXML = new XmlDocument();

                lXML.LoadXml(lGF);
                XmlNode lBullets = lXML.SelectSingleNode("bulletdatabasefile");
                foreach (XmlNode lgn in lBullets)
                {
                    Bullet ltg = new Bullet();

                    XmlNode lid = lgn.SelectSingleNode("id");
                    lNR = lid.Attributes["value"].Value;
                    ltg.ID = lNR;
                    XmlNode lmk = lgn.SelectSingleNode("bcg1");
                    lNR = lmk.Attributes["value"].Value;
                    ltg.BCg1 = Convert.ToDouble(lNR);
                    try
                    {
                        XmlNode lmkk = lgn.SelectSingleNode("bcg7");
                        lNR = lmkk.Attributes["value"].Value;
                        ltg.BCg7 = Convert.ToDouble(lNR);
                    }
                    catch
                    {
                        //Older file does not have BCg7
                    }
                    XmlNode lmdl = lgn.SelectSingleNode("diameter");
                    lNR = lmdl.Attributes["value"].Value;
                    ltg.Diameter = Convert.ToDouble(lNR);
                    XmlNode ldesc = lgn.SelectSingleNode("length");
                    lNR = ldesc.Attributes["value"].Value;
                    ltg.Length = Convert.ToDouble(lNR);
                    XmlNode lbto = lgn.SelectSingleNode("bto");
                    lNR = lbto.Attributes["value"].Value;
                    ltg.BTO = Convert.ToDouble(lNR);
                    XmlNode lbulletsbto = lgn.SelectSingleNode("model");
                    lNR = lbulletsbto.Attributes["value"].Value;
                    ltg.Model = lNR;
                    XmlNode lbulletsoal = lgn.SelectSingleNode("manufacturer");
                    lNR = lbulletsoal.Attributes["value"].Value;
                    ltg.Manufacturer = lNR;
                    XmlNode lbulletswt = lgn.SelectSingleNode("type");
                    lNR = lbulletswt.Attributes["value"].Value;
                    ltg.Type = lNR;
                    XmlNode lcartid = lgn.SelectSingleNode("weight");
                    lNR = lcartid.Attributes["value"].Value;
                    ltg.Weight = Convert.ToDouble(lNR);
                    lLR.Add(ltg);
                }
            }
            catch
            {
                //opps file likely doesn't exist yet.
            }
            return lLR;
        }
        public int SaveCaseDB()
        {
            int lRtn = 0;
            string lGfilename;  //The path and file name of the the exported data file.
            string lGfileBak;   //The path and file name for the the backup data file.
            string lGF;         //Gun file string variable.

            try
            {
                lGfilename = AppDataFolder + "\\CaseDB.cdf";
                lGfileBak = AppDataFolder + "\\CaseDB.BAK";
                lGF = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>" + System.Environment.NewLine;
                lGF = lGF + "<casedatabasefile>" + System.Environment.NewLine;
                foreach (Case lg in LawlerBallisticsFactory.MyCases)
                {
                    lGF = lGF + "<case>" + System.Environment.NewLine;
                    lGF = lGF + "<id value=\"" + lg.ID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                    lGF = lGF + "</id>" + System.Environment.NewLine;                    
                    lGF = lGF + "<cartridgeid value=\"" + lg.CartridgeID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                    lGF = lGF + "</cartridgeid>" + System.Environment.NewLine;
                    lGF = lGF + "<name value=\"" + lg.Name.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                    lGF = lGF + "</name>" + System.Environment.NewLine;                    
                    lGF = lGF + "<model value=\"" + lg.Model.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                    lGF = lGF + "</model>" + System.Environment.NewLine;
                    lGF = lGF + "<manufacturer value=\"" + lg.Manufacturer.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                    lGF = lGF + "</manufacturer>" + System.Environment.NewLine;
                    lGF = lGF + "<primersize value=\"" + lg.PrimerSize.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
                    lGF = lGF + "</primersize>" + System.Environment.NewLine;
                    lGF = lGF + "</case>" + System.Environment.NewLine;
                }
                lGF = lGF + "</casedatabasefile>";
                if (File.Exists(lGfilename))
                {
                    if (File.Exists(lGfileBak))
                    {
                        File.Delete(lGfileBak);
                    }
                    File.Move(lGfilename, lGfileBak);
                    File.Delete(lGfilename);
                }
                File.WriteAllText(lGfilename, lGF);
            }
            catch
            {
                lRtn = -1;
            }

            return lRtn;
        }
        public ObservableCollection<Case> ParseCaseDB(bool LoadBak = false)
        {
            ObservableCollection<Case> lLR = new ObservableCollection<Case>();
            string lGfilename;  //The path and file name of the the exported data file.
            string lGF; // string containing recipe data from file.
            string lNR;

            //try
            //{
                lGfilename = AppDataFolder + "\\CaseDB.cdf";
                lGF = File.ReadAllText(lGfilename);
                XmlDocument lXML = new XmlDocument();

                lXML.LoadXml(lGF);
                XmlNode lBullets = lXML.SelectSingleNode("casedatabasefile");
                foreach (XmlNode lgn in lBullets)
                {
                    Case ltg = new Case();

                    XmlNode lid = lgn.SelectSingleNode("id");
                    lNR = lid.Attributes["value"].Value;
                    ltg.ID = lNR;
                    XmlNode lmk = lgn.SelectSingleNode("cartridgeid");
                    lNR = lmk.Attributes["value"].Value;
                    ltg.CartridgeID = lNR;
                    ltg.CartridgeName = LawlerBallisticsFactory.GetCartridgeName(ltg.CartridgeID);
                    XmlNode lmdl = lgn.SelectSingleNode("name");
                    lNR = lmdl.Attributes["value"].Value;
                    ltg.Name = lNR;                   
                    XmlNode lbulletsbto = lgn.SelectSingleNode("model");
                    lNR = lbulletsbto.Attributes["value"].Value;
                    ltg.Model = lNR;
                    XmlNode lbulletsoal = lgn.SelectSingleNode("manufacturer");
                    lNR = lbulletsoal.Attributes["value"].Value;
                    ltg.Manufacturer = lNR;
                    lNR = "";
                    lbulletsoal = lgn.SelectSingleNode("primersize");
                    if(lbulletsoal!=null) lNR = lbulletsoal.Attributes["value"].Value;
                    ltg.PrimerSize = lNR;                    
                    lLR.Add(ltg);
                }
            //}
            //catch
            //{
            //    //opps file likely doesn't exist yet.
            //}
            return lLR;
        }
        public int SavePrimerDB()
        {
            int lRtn = 0;
            string lGfilename;  //The path and file name of the the exported data file.
            string lGfileBak;   //The path and file name for the the backup data file.
            string lGF;         //Gun file string variable.

            try
            {
                lGfilename = AppDataFolder + "\\PrimerDB.pdf";
                lGfileBak = AppDataFolder + "\\PrimerDB.BAK";
                lGF = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>" + System.Environment.NewLine;
                lGF = lGF + "<primerdatabasefile>" + System.Environment.NewLine;
                foreach (Primer lg in LawlerBallisticsFactory.MyPrimers)
                {
                    lGF = lGF + "<primer>" + System.Environment.NewLine;
                    lGF = lGF + "<id value=\"" + lg.ID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</id>" + System.Environment.NewLine;
                    lGF = lGF + "<name value=\"" + lg.Name.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</name>" + System.Environment.NewLine;
                    lGF = lGF + "<model value=\"" + lg.Model.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</model>" + System.Environment.NewLine;
                    lGF = lGF + "<manufacturer value=\"" + lg.Manufacturer.ToString() + "\" type='string'>" + System.Environment.NewLine;
                    lGF = lGF + "</manufacturer>" + System.Environment.NewLine;
                    lGF = lGF + "<type value=\"" + lg.Type.ToString() + "\" type='string'>" + System.Environment.NewLine;
                    lGF = lGF + "</type>" + System.Environment.NewLine;
                    lGF = lGF + "</primer>" + System.Environment.NewLine;
                }
                lGF = lGF + "</primerdatabasefile>";
                if (File.Exists(lGfilename))
                {
                    if (File.Exists(lGfileBak))
                    {
                        File.Delete(lGfileBak);
                    }
                    File.Move(lGfilename, lGfileBak);
                    File.Delete(lGfilename);
                }
                File.WriteAllText(lGfilename, lGF);
            }
            catch
            {
                lRtn = -1;
            }

            return lRtn;
        }
        public ObservableCollection<Primer> ParsePrimerDB(bool LoadBak = false)
        {
            ObservableCollection<Primer> lLR = new ObservableCollection<Primer>();
            string lGfilename;  //The path and file name of the the exported data file.
            string lGF; // string containing recipe data from file.
            string lNR;

            try
            {
                lGfilename = AppDataFolder + "\\PrimerDB.pdf";
                lGF = File.ReadAllText(lGfilename);
                XmlDocument lXML = new XmlDocument();

                lXML.LoadXml(lGF);
                XmlNode lBullets = lXML.SelectSingleNode("primerdatabasefile");
                foreach (XmlNode lgn in lBullets)
                {
                    Primer ltg = new Primer();

                    XmlNode lid = lgn.SelectSingleNode("id");
                    lNR = lid.Attributes["value"].Value;
                    ltg.ID = lNR;
                    XmlNode lname = lgn.SelectSingleNode("name");
                    lNR = lname.Attributes["value"].Value;
                    ltg.Name = lNR;
                    XmlNode lbulletsbto = lgn.SelectSingleNode("model");
                    lNR = lbulletsbto.Attributes["value"].Value;
                    ltg.Model = lNR;
                    XmlNode lbulletsoal = lgn.SelectSingleNode("manufacturer");
                    lNR = lbulletsoal.Attributes["value"].Value;
                    ltg.Manufacturer = lNR;
                    XmlNode ltype = lgn.SelectSingleNode("type");
                    lNR = ltype.Attributes["value"].Value;
                    ltg.Type = lNR;
                    lLR.Add(ltg);
                }
            }
            catch
            {
                //opps file likely doesn't exist yet.
            }
            return lLR;
        }
        public int SavePowderDB()
        {
            int lRtn = 0;
            string lGfilename;  //The path and file name of the the exported data file.
            string lGfileBak;   //The path and file name for the the backup data file.
            string lGF;         //Gun file string variable.

            try
            {
                lGfilename = AppDataFolder + "\\PowderDB.ddf";
                lGfileBak = AppDataFolder + "\\PowderDB.BAK";
                lGF = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>" + System.Environment.NewLine;
                lGF = lGF + "<powderdatabasefile>" + System.Environment.NewLine;
                foreach (Powder lg in LawlerBallisticsFactory.MyPowders)
                {
                    lGF = lGF + "<powder>" + System.Environment.NewLine;
                    lGF = lGF + "<id value=\"" + lg.ID.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</id>" + System.Environment.NewLine;
                    lGF = lGF + "<name value=\"" + lg.Name.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</name>" + System.Environment.NewLine;
                    lGF = lGF + "<model value=\"" + lg.Model.ToString() + "\">" + System.Environment.NewLine;
                    lGF = lGF + "</model>" + System.Environment.NewLine;
                    lGF = lGF + "<manufacturer value=\"" + lg.Manufacturer.ToString() + "\" type='string'>" + System.Environment.NewLine;
                    lGF = lGF + "</manufacturer>" + System.Environment.NewLine;
                    lGF = lGF + "<basetype value=\"" + lg.BaseType.ToString() + "\" type='string'>" + System.Environment.NewLine;
                    lGF = lGF + "</basetype>" + System.Environment.NewLine;
                    lGF = lGF + "</powder>" + System.Environment.NewLine;
                }
                lGF = lGF + "</powderdatabasefile>";
                if (File.Exists(lGfilename))
                {
                    if (File.Exists(lGfileBak))
                    {
                        File.Delete(lGfileBak);
                    }
                    File.Move(lGfilename, lGfileBak);
                    File.Delete(lGfilename);
                }
                File.WriteAllText(lGfilename, lGF);
            }
            catch
            {
                lRtn = -1;
            }

            return lRtn;
        }
        public ObservableCollection<Powder> ParsePowderDB(bool LoadBak = false)
        {
            ObservableCollection<Powder> lLR = new ObservableCollection<Powder>();
            string lGfilename;  //The path and file name of the the exported data file.
            string lGF; // string containing recipe data from file.
            string lNR;

            try
            {
                lGfilename = AppDataFolder + "\\PowderDB.ddf";
                lGF = File.ReadAllText(lGfilename);
                XmlDocument lXML = new XmlDocument();

                lXML.LoadXml(lGF);
                XmlNode lBullets = lXML.SelectSingleNode("powderdatabasefile");
                foreach (XmlNode lgn in lBullets)
                {
                    Powder ltg = new Powder();

                    XmlNode lid = lgn.SelectSingleNode("id");
                    lNR = lid.Attributes["value"].Value;
                    ltg.ID = lNR;
                    XmlNode lname = lgn.SelectSingleNode("name");
                    lNR = lname.Attributes["value"].Value;
                    ltg.Name = lNR;
                    XmlNode lbulletsbto = lgn.SelectSingleNode("model");
                    lNR = lbulletsbto.Attributes["value"].Value;
                    ltg.Model = lNR;
                    XmlNode lbulletsoal = lgn.SelectSingleNode("manufacturer");
                    lNR = lbulletsoal.Attributes["value"].Value;
                    ltg.Manufacturer = lNR;
                    XmlNode ltype = lgn.SelectSingleNode("basetype");
                    lNR = ltype.Attributes["value"].Value;
                    ltg.BaseType = lNR;
                    lLR.Add(ltg);
                }
            }
            catch
            {
                //opps file likely doesn't exist yet.
            }
            return lLR;
        }
        #endregion

        #region "Private Routines"
        private string CartridgeDatXML(Cartridge TargetCartridge)
        {
            string lCDS;
            lCDS = "<cartridgedata>" + Environment.NewLine;
            lCDS = lCDS + "<id value=\"" + TargetCartridge.ID + "\" type=\"string\">" + Environment.NewLine;
            lCDS = lCDS + "</id>" + System.Environment.NewLine;
            lCDS = lCDS + "<name value=\"" + TargetCartridge.Name + "\" type=\"string\">" + Environment.NewLine;
            lCDS = lCDS + "</name>" + System.Environment.NewLine;
            lCDS = lCDS + "<bulletdiameter value=\"" + TargetCartridge.BulletDiameter.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lCDS = lCDS + "</bulletdiameter>" + System.Environment.NewLine;
            lCDS = lCDS + "<trimlength value=\"" + TargetCartridge.CaseTrimLngth.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lCDS = lCDS + "</trimlength>" + System.Environment.NewLine;
            lCDS = lCDS + "<maxcaselength value=\"" + TargetCartridge.MaxCaseLngth.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lCDS = lCDS + "</maxcaselength>" + System.Environment.NewLine;
            lCDS = lCDS + "<maxcoal value=\"" + TargetCartridge.MaxCOAL.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lCDS = lCDS + "</maxcoal>" + System.Environment.NewLine;
            lCDS = lCDS + "<headspacemax value=\"" + TargetCartridge.HeadSpaceMax.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lCDS = lCDS + "</headspacemax>" + System.Environment.NewLine;
            lCDS = lCDS + "<headspacemin value=\"" + TargetCartridge.HeadSpaceMin.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lCDS = lCDS + "</headspacemin>" + System.Environment.NewLine;
            lCDS = lCDS + "<cartridgepic value=\"" + BitConverter.ToString(LawlerBallisticsFactory.ImageToBytes(TargetCartridge.CartridgePic)) + "\" type=\"bytearray\">" + System.Environment.NewLine;
            lCDS = lCDS + "</cartridgepic>" + System.Environment.NewLine;
            lCDS = lCDS + "<powders>" + System.Environment.NewLine;
            foreach(string lp in TargetCartridge.PowderIDlist)
            {
                lCDS = lCDS + "<powder>" + System.Environment.NewLine;

                lCDS = lCDS + "<pwdrid value=\"" + lp + "\" type=\"string\">" + Environment.NewLine;
                lCDS = lCDS + "</pwdrid>" + System.Environment.NewLine;
                lCDS = lCDS + "</powder>" + System.Environment.NewLine;
            }
            lCDS = lCDS + "</powders>" + System.Environment.NewLine;
            lCDS = lCDS + "</cartridgedata>" + Environment.NewLine;
            return lCDS;
        }
        private bool IsBallisticSol(string[] FileLines)
        {
            bool lRtn = false;
            foreach (string L in FileLines)
            {
                if (L.Length >= "<Ballistic Solution File>".Length)
                {
                    if (L == "<Ballistic Solution File>")
                    {
                        // A valid ballistic solution file.
                        lRtn = true;
                        break;
                    }
                }
            }
            return lRtn;
        }
        private ObservableCollection<Cartridge> LoadCartridges(XmlNode Cartridges)
        {
            ObservableCollection<Cartridge> lRtn = new ObservableCollection<Cartridge>();
            Cartridge lCartridge = null;
            string lValue;
            XmlNode lcnode;

            foreach (XmlNode lCart in Cartridges)
            {
                lCartridge = new Cartridge();
                lcnode = lCart.SelectSingleNode("id");
                lValue = lcnode.Attributes["value"].Value;
                lCartridge.ID = lValue;
                lcnode = lCart.SelectSingleNode("name");
                lValue = lcnode.Attributes["value"].Value;
                lCartridge.Name = lValue;
                lcnode = lCart.SelectSingleNode("bulletdiameter");
                lValue = lcnode.Attributes["value"].Value;
                lCartridge.BulletDiameter = Convert.ToDouble(lValue);
                lcnode = lCart.SelectSingleNode("trimlength");
                lValue = lcnode.Attributes["value"].Value;
                lCartridge.CaseTrimLngth = Convert.ToDouble(lValue);
                lcnode = lCart.SelectSingleNode("maxcaselength");
                lValue = lcnode.Attributes["value"].Value;
                lCartridge.MaxCaseLngth = Convert.ToDouble(lValue);
                lcnode = lCart.SelectSingleNode("maxcoal");
                lValue = lcnode.Attributes["value"].Value;
                lCartridge.MaxCOAL = Convert.ToDouble(lValue);
                lcnode = lCart.SelectSingleNode("headspacemax");
                lValue = lcnode.Attributes["value"].Value;
                lCartridge.HeadSpaceMax = Convert.ToDouble(lValue);
                lcnode = lCart.SelectSingleNode("headspacemin");
                lValue = lcnode.Attributes["value"].Value;
                lCartridge.HeadSpaceMin = Convert.ToDouble(lValue);
                try
                {
                    lcnode = lCart.SelectSingleNode("cartridgepic");
                    lValue = lcnode.Attributes["value"].Value;
                    byte[] lba = LawlerBallisticsFactory.StringToByteArray(lValue);
                    Image lx = (Bitmap)((new ImageConverter()).ConvertFrom(lba));
                    lCartridge.CartridgePic = lx;
                }
                catch
                {
                    // Image issue or no image
                }
                lcnode = lCart.SelectSingleNode("powders");
                XmlNode lpwdrid;
                lCartridge.PowderIDlist = new List<string>();
                foreach (XmlNode ln in lcnode)
                {
                    lValue = "";
                    lpwdrid = ln.SelectSingleNode("pwdrid");
                    lValue = lpwdrid.Attributes["value"].Value;
                    lCartridge.PowderIDlist.Add(lValue);
                }               
                lRtn.Add(lCartridge);
            }
            return lRtn;
        }
        #endregion
    }
}
