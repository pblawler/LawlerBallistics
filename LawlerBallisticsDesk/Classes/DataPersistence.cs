using LawlerBallisticsDesk.Classes.BallisticClasses;
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
        private string _XML_Header = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>" + Environment.NewLine;
        #endregion

        #region "Properties"
        public string CartridgeFileFilter { get { return "Cartridge Data Files (*.cdf)|*.cdf"; } }
        public string BallisticFileFilter { get { return "Ballistic Data Files (*.bdf)|*.bdf"; } }
        #endregion

        #region "Constructor"
        public DataPersistence ()
        {

        }
        #endregion

        #region "Public Routines"
        //TODO: when deleting an object warn about dependent objects and if selected delete object and all
        //single dependencies (i.e. barrel specific recipes, etc...).


        /// <summary>
        /// If this is a first run or something happened to the data files load the prepackaged files.
        /// </summary>
        public void CheckDataFiles()
        {
            //Load default data file for ballistic solution menu view
            string lDatFile = LawlerBallisticsFactory.DataFolder + "\\default.bdf";

            //TODO: Add a restore default files function for all initially provided files.
            //Restore missing file from installation files.
            string lSource = "Data/default.bdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Cartridge data file
             lDatFile = LawlerBallisticsFactory.AppDataFolder + "\\CartridgeDB.cdf";
             lSource = "Data/CartridgeDB.cdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Bullet data file
            lDatFile = LawlerBallisticsFactory.AppDataFolder + "\\BulletDB.bdf";
            lSource = "Data/BulletDB.bdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Case data file
            lDatFile = LawlerBallisticsFactory.AppDataFolder + "\\CaseDB.cdf";
            lSource = "Data/CaseDB.cdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Gun data file
            lDatFile = LawlerBallisticsFactory.AppDataFolder + "\\GunDB.gdf";
            lSource = "Data/GunDB.gdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Powder data file
            lDatFile = LawlerBallisticsFactory.AppDataFolder + "\\PowderDB.ddf";
            lSource = "Data/PowderDB.ddf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Primer data file
            lDatFile = LawlerBallisticsFactory.AppDataFolder + "\\PrimerDB.pdf";
            lSource = "Data/PrimerDB.pdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }

            //Load Recipe data file
            lDatFile = LawlerBallisticsFactory.AppDataFolder + "\\RecipeDB.rdf";
            lSource = "Data/RecipeDB.rdf";
            if (!File.Exists(lDatFile))
            {
                File.Copy(lSource, lDatFile);
            }
        }
        public int SaveCartridgeData()
        {
            int lRtn = 0;
            string lCF; string lCfilename; string lCfileBak;

            try
            {
                lCF = _XML_Header;
                lCF = lCF + "<catridgedatabasefile>" + System.Environment.NewLine;
                foreach (Cartridge iC in LawlerBallisticsFactory.MyCartridges)
                {
                    lCF = lCF + CartridgeDatXML(iC);
                }
                lCF = lCF + "</catridgedatabasefile>" + System.Environment.NewLine;
                lCfilename = LawlerBallisticsFactory.DataFolder + "\\CartridgeDB.cdf";
                lCfileBak = LawlerBallisticsFactory.DataFolder + "\\CartridgeDB.BAK";
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
            string lCDF = LawlerBallisticsFactory.DataFolder + "\\CartridgeDB.cdf";
            if (LoadBAK) lCDF = LawlerBallisticsFactory.DataFolder + "\\CartridgeDB.BAK";
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
        public int SaveBallisticSolutionData(Solution BallisticSolutionData, string FileName)
        {
            int lRtn = 0;
            string lBfilename;  //Then path and file name of the the exported data file.
            string lBF;         //Balistic file string variable.

            //try
            //{
            //    lBF = "<Ballistic Solution File>" + System.Environment.NewLine;
            //    lBF = lBF + "<PROPERTIES>" + System.Environment.NewLine;
            //    lBF = lBF + "<zTargetLat>" + TargetBallisticData.zTargetLat.ToString() + "</zTargetLat>" + System.Environment.NewLine;
            //    lBF = lBF + "<zTargetLon>" + TargetBallisticData.zTargetLon.ToString() + "</zTargetLon>" + System.Environment.NewLine;
            //    lBF = lBF + "<zShooterLat>" + TargetBallisticData.zShooterLat.ToString() + "</zShooterLat>" + System.Environment.NewLine;
            //    lBF = lBF + "<zShooterLon>" + TargetBallisticData.zShooterLon.ToString() + "</zShooterLon>" + System.Environment.NewLine;
            //    lBF = lBF + "<ShooterLon>" + TargetBallisticData.ShooterLon.ToString() + "</ShooterLon>" + System.Environment.NewLine;
            //    lBF = lBF + "<ShooterLat>" + TargetBallisticData.ShooterLat.ToString() + "</ShooterLat>" + System.Environment.NewLine;
            //    lBF = lBF + "<ShotDistance>" + TargetBallisticData.ShotDistance.ToString() + "</ShotDistance>" + System.Environment.NewLine;
            //    lBF = lBF + "<ShotAngle>" + TargetBallisticData.ShotAngle.ToString() + "</ShotAngle>" + System.Environment.NewLine;
            //    lBF = lBF + "<TargetLat>" + TargetBallisticData.TargetLat.ToString() + "</TargetLat>" + System.Environment.NewLine;
            //    lBF = lBF + "<TargetLon>" + TargetBallisticData.TargetLon.ToString() + "</TargetLon>" + System.Environment.NewLine;
            //    lBF = lBF + "<RelHumidity>" + TargetBallisticData.RelHumidity.ToString() + "</RelHumidity>" + System.Environment.NewLine;
            //    lBF = lBF + "<zRelHumidity>" + TargetBallisticData.zRelHumidity.ToString() + "</zRelHumidity>" + System.Environment.NewLine;                
            //    lBF = lBF + "<zBaroPressure>" + TargetBallisticData.zBaroPressure.ToString() + "</zBaroPressure>" + System.Environment.NewLine;
            //    lBF = lBF + "<BaroPressure>" + TargetBallisticData.BaroPressure.ToString() + "</BaroPressure>" + System.Environment.NewLine;
            //    lBF = lBF + "<DensityAlt>" + TargetBallisticData.DensityAlt.ToString() + "</DensityAlt>" + System.Environment.NewLine;
            //    lBF = lBF + "<zDensityAlt>" + TargetBallisticData.zDensityAlt.ToString() + "</zDensityAlt>" + System.Environment.NewLine;
            //    lBF = lBF + "<TempF>" + TargetBallisticData.TempF.ToString() + "</TempF>" + System.Environment.NewLine;
            //    lBF = lBF + "<zTempF>" + TargetBallisticData.zTempF.ToString() + "</zTempF>" + System.Environment.NewLine;
            //    lBF = lBF + "<ScopeHeight>" + TargetBallisticData.ScopeHeight.ToString() + "</ScopeHeight>" + System.Environment.NewLine;
            //    lBF = lBF + "<MuzzleVelocity>" + TargetBallisticData.MuzzleVelocity.ToString() + "</MuzzleVelocity>" + System.Environment.NewLine;
            //    lBF = lBF + "<Fo>" + TargetBallisticData.Fo.ToString() + "</Fo>" + System.Environment.NewLine;
            //    lBF = lBF + "<F2>" + TargetBallisticData.F2.ToString() + "</F2>" + System.Environment.NewLine;
            //    lBF = lBF + "<F3>" + TargetBallisticData.F3.ToString() + "</F3>" + System.Environment.NewLine;
            //    lBF = lBF + "<F4>" + TargetBallisticData.F4.ToString() + "</F4>" + System.Environment.NewLine;
            //    lBF = lBF + "<V1>" + TargetBallisticData.V1.ToString() + "</V1>" + System.Environment.NewLine;
            //    lBF = lBF + "<V2>" + TargetBallisticData.V2.ToString() + "</V2>" + System.Environment.NewLine;
            //    lBF = lBF + "<D1>" + TargetBallisticData.D1.ToString() + "</D1>" + System.Environment.NewLine;
            //    lBF = lBF + "<D2>" + TargetBallisticData.D2.ToString() + "</D2>" + System.Environment.NewLine;
            //    lBF = lBF + "<BCg1>" + TargetBallisticData.BCg1.ToString() + "</BCg1>" + System.Environment.NewLine;
            //    lBF = lBF + "<BCz2>" + TargetBallisticData.BCz2.ToString() + "</BCz2>" + System.Environment.NewLine;
            //    lBF = lBF + "<ZeroMaxRise>" + TargetBallisticData.ZeroMaxRise.ToString() + "</ZeroMaxRise>" + System.Environment.NewLine;
            //    lBF = lBF + "<ZeroRange>" + TargetBallisticData.ZeroRange.ToString() + "</ZeroRange>" + System.Environment.NewLine;
            //    lBF = lBF + "<UseMaxRise>" + TargetBallisticData.UseMaxRise.ToString() + "</UseMaxRise>" + System.Environment.NewLine;

            //    //  These are now calculated by atmospheric conditions and Mach factors.
            //    //lBF = lBF + "<Zone1TransSpeed>" + TargetBallisticData.Zone1TransSpeed.ToString() + "</Zone1TransSpeed>" + System.Environment.NewLine;
            //    //lBF = lBF + "<Zone2TransSpeed>" + TargetBallisticData.Zone2TransSpeed.ToString() + "</Zone2TransSpeed>" + System.Environment.NewLine;
            //    //lBF = lBF + "<Zone3TransSpeed>" + TargetBallisticData.Zone3TransSpeed.ToString() + "</Zone3TransSpeed>" + System.Environment.NewLine;
                
            //    lBF = lBF + "<Zone1MachFactor>" + TargetBallisticData.Zone1MachFactor.ToString() + "</Zone1MachFactor>" + System.Environment.NewLine;
            //    lBF = lBF + "<Zone2MachFactor>" + TargetBallisticData.Zone2MachFactor.ToString() + "</Zone2MachFactor>" + System.Environment.NewLine;
            //    lBF = lBF + "<Zone3MachFactor>" + TargetBallisticData.Zone3MachFactor.ToString() + "</Zone3MachFactor>" + System.Environment.NewLine;
            //    lBF = lBF + "<Zone1SlopeMultiplier>" + TargetBallisticData.Zone1SlopeMultiplier.ToString() + "</Zone1SlopeMultiplier>" + System.Environment.NewLine;
            //    lBF = lBF + "<Zone3SlopeMultiplier>" + TargetBallisticData.Zone3SlopeMultiplier.ToString() + "</Zone3SlopeMultiplier>" + System.Environment.NewLine;
            //    lBF = lBF + "<Zone1Slope>" + TargetBallisticData.Zone1Slope.ToString() + "</Zone1Slope>" + System.Environment.NewLine;
            //    lBF = lBF + "<Zone1AngleFactor>" + TargetBallisticData.Zone1AngleFactor.ToString() + "</Zone1AngleFactor>" + System.Environment.NewLine;
            //    lBF = lBF + "<Zone3Slope>" + TargetBallisticData.Zone3Slope.ToString() + "</Zone3Slope>" + System.Environment.NewLine;
            //    lBF = lBF + "<BarrelTwist>" + TargetBallisticData.BarrelTwist.ToString() + "</BarrelTwist>" + System.Environment.NewLine;
            //    lBF = lBF + "<BarrelTwistDir>" + TargetBallisticData.BarrelTwistDir.ToString() + "</BarrelTwistDir>" + System.Environment.NewLine;
            //    lBF = lBF + "<BulletDiameter>" + TargetBallisticData.BulletDiameter.ToString() + "</BulletDiameter>" + System.Environment.NewLine;
            //    lBF = lBF + "<BulletLength>" + TargetBallisticData.BulletLength.ToString() + "</BulletLength>" + System.Environment.NewLine;
            //    lBF = lBF + "<BulletWeight>" + TargetBallisticData.BulletWeight.ToString() + "</BulletWeight>" + System.Environment.NewLine;

            //    // Calculated with bullet, barrel, and atmospheric conditions
            //    //lBF = lBF + "<BSG>" + TargetBallisticData.BSG.ToString() + "</BSG>" + System.Environment.NewLine;
                
            //    lBF = lBF + "<BulletShapeTyp>" + TargetBallisticData.BulletShapeTyp.ToString() + "</BulletShapeTyp>" + System.Environment.NewLine;
            //    lBF = lBF + "</PROPERTIES>" + System.Environment.NewLine;
            //    lBF = lBF + "</Ballistic Solution File>";
            //    lBfilename = FileName;
            //    if (File.Exists(lBfilename))
            //    {                    
            //        File.Delete(lBfilename);
            //    }
            //    File.WriteAllText(lBfilename, lBF);
            //}
            //catch
            //{
            //    lRtn = -1;
            //}
            return lRtn;
        }
        public Solution ParseBallisticSolution(string FileName)
        {
            Solution lBSF = new Solution();
            //string lBDF = FileName;
            //string lValue;
            //if (!File.Exists(lBDF) )
            //{
            //    return lBSF;
            //}            
            //string[] BDFL = File.ReadAllLines(lBDF, Encoding.UTF8);
            //if (!IsBallisticSol(BDFL))
            //{               
            //    return lBSF;
            //}
            //foreach (string L in BDFL)
            //{                
            //    if (L.StartsWith("<zTargetLat>"))
            //    {
            //        lValue = L.Replace("<zTargetLat>", "");
            //        lValue = lValue.Replace("</zTargetLat>", "");
            //        lBSF.zTargetLat = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<zTargetLon>"))
            //    {
            //        lValue = L.Replace("<zTargetLon>", "");
            //        lValue = lValue.Replace("</zTargetLon>", "");
            //        lBSF.zTargetLon = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<zShooterLat>"))
            //    {
            //        lValue = L.Replace("<zShooterLat>", "");
            //        lValue = lValue.Replace("</zShooterLat>", "");
            //        lBSF.zTargetLon = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<zShooterLon>"))
            //    {
            //        lValue = L.Replace("<zShooterLon>", "");
            //        lValue = lValue.Replace("</zShooterLon>", "");
            //        lBSF.zShooterLon = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<ShooterLon>"))
            //    {
            //        lValue = L.Replace("<ShooterLon>", "");
            //        lValue = lValue.Replace("</ShooterLon>", "");
            //        lBSF.ShooterLon = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<ShooterLat>"))
            //    {
            //        lValue = L.Replace("<ShooterLat>", "");
            //        lValue = lValue.Replace("</ShooterLat>", "");
            //        lBSF.ShooterLat = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<ShotDistance>"))
            //    {
            //        lValue = L.Replace("<ShotDistance>", "");
            //        lValue = lValue.Replace("</ShotDistance>", "");
            //        lBSF.ShotDistance = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<ShotAngle>"))
            //    {
            //        lValue = L.Replace("<ShotAngle>", "");
            //        lValue = lValue.Replace("</ShotAngle>", "");
            //        lBSF.ShotAngle = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<TargetLat>"))
            //    {
            //        lValue = L.Replace("<TargetLat>", "");
            //        lValue = lValue.Replace("</TargetLat>", "");
            //        lBSF.TargetLat = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<TargetLon>"))
            //    {
            //        lValue = L.Replace("<TargetLon>", "");
            //        lValue = lValue.Replace("</TargetLon>", "");
            //        lBSF.TargetLon = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<RelHumidity>"))
            //    {
            //        lValue = L.Replace("<RelHumidity>", "");
            //        lValue = lValue.Replace("</RelHumidity>", "");
            //        lBSF.RelHumidity = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<zRelHumidity>"))
            //    {
            //        lValue = L.Replace("<zRelHumidity>", "");
            //        lValue = lValue.Replace("</zRelHumidity>", "");
            //        lBSF.zRelHumidity = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<DensityAlt>"))
            //    {
            //        lValue = L.Replace("<DensityAlt>", "");
            //        lValue = lValue.Replace("</DensityAlt>", "");
            //        lBSF.DensityAlt = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<zDensityAlt>"))
            //    {
            //        lValue = L.Replace("<zDensityAlt>", "");
            //        lValue = lValue.Replace("</zDensityAlt>", "");
            //        lBSF.zDensityAlt = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<zBaroPressure>"))
            //    {
            //        lValue = L.Replace("<zBaroPressure>", "");
            //        lValue = lValue.Replace("</zBaroPressure>", "");
            //        lBSF.zBaroPressure = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<BaroPressure>"))
            //    {
            //        lValue = L.Replace("<BaroPressure>", "");
            //        lValue = lValue.Replace("</BaroPressure>", "");
            //        lBSF.BaroPressure = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<TempF>"))
            //    {
            //        lValue = L.Replace("<TempF>", "");
            //        lValue = lValue.Replace("</TempF>", "");
            //        lBSF.TempF = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<zTempF>"))
            //    {
            //        lValue = L.Replace("<zTempF>", "");
            //        lValue = lValue.Replace("</zTempF>", "");
            //        lBSF.zTempF = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<ScopeHeight>"))
            //    {
            //        lValue = L.Replace("<ScopeHeight>", "");
            //        lValue = lValue.Replace("</ScopeHeight>", "");
            //        lBSF.ScopeHeight = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<MuzzleVelocity>"))
            //    {
            //        lValue = L.Replace("<MuzzleVelocity>", "");
            //        lValue = lValue.Replace("</MuzzleVelocity>", "");
            //        lBSF.MuzzleVelocity = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<Fo>"))
            //    {
            //        lValue = L.Replace("<Fo>", "");
            //        lValue = lValue.Replace("</Fo>", "");
            //        lBSF.Fo = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<F2>"))
            //    {
            //        lValue = L.Replace("<F2>", "");
            //        lValue = lValue.Replace("</F2>", "");
            //        lBSF.F2 = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<F3>"))
            //    {
            //        lValue = L.Replace("<F3>", "");
            //        lValue = lValue.Replace("</F3>", "");
            //        lBSF.F3 = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<F4>"))
            //    {
            //        lValue = L.Replace("<F4>", "");
            //        lValue = lValue.Replace("</F4>", "");
            //        lBSF.F4 = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<V1>"))
            //    {
            //        lValue = L.Replace("<V1>", "");
            //        lValue = lValue.Replace("</V1>", "");
            //        lBSF.V1 = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<V2>"))
            //    {
            //        lValue = L.Replace("<V2>", "");
            //        lValue = lValue.Replace("</V2>", "");
            //        lBSF.V2 = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<D1>"))
            //    {
            //        lValue = L.Replace("<D1>", "");
            //        lValue = lValue.Replace("</D1>", "");
            //        lBSF.D1 = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<D2>"))
            //    {
            //        lValue = L.Replace("<D2>", "");
            //        lValue = lValue.Replace("</D2>", "");
            //        lBSF.D2 = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<BCg1>"))
            //    {
            //        lValue = L.Replace("<BCg1>", "");
            //        lValue = lValue.Replace("</BCg1>", "");
            //        lBSF.BCg1 = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<BCz2>"))
            //    {
            //        lValue = L.Replace("<BCz2>", "");
            //        lValue = lValue.Replace("</BCz2>", "");
            //        lBSF.BCz2 = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<ZeroMaxRise>"))
            //    {
            //        lValue = L.Replace("<ZeroMaxRise>", "");
            //        lValue = lValue.Replace("</ZeroMaxRise>", "");
            //        lBSF.ZeroMaxRise = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<ZeroRange>"))
            //    {
            //        lValue = L.Replace("<ZeroRange>", "");
            //        lValue = lValue.Replace("</ZeroRange>", "");
            //        lBSF.ZeroRange = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<UseMaxRise>"))
            //    {
            //        lValue = L.Replace("<UseMaxRise>", "");
            //        lValue = lValue.Replace("</UseMaxRise>", "");
            //        lBSF.UseMaxRise = Convert.ToBoolean(lValue);
            //    }
            //    //  These are calculated from atmospheric data and Mach factors now.
            //    //else if (L.StartsWith("<Zone1TransSpeed>"))
            //    //{
            //    //    lValue = L.Replace("<Zone1TransSpeed>", "");
            //    //    lValue = lValue.Replace("</Zone1TransSpeed>", "");
            //    //    lBSF.Zone1TransSpeed = Convert.ToDouble(lValue);
            //    //}
            //    //else if (L.StartsWith("<Zone2TransSpeed>"))
            //    //{
            //    //    lValue = L.Replace("<Zone2TransSpeed>", "");
            //    //    lValue = lValue.Replace("</Zone2TransSpeed>", "");
            //    //    lBSF.Zone2TransSpeed = Convert.ToDouble(lValue);
            //    //}
            //    //else if (L.StartsWith("<Zone3TransSpeed>"))
            //    //{
            //    //    lValue = L.Replace("<Zone3TransSpeed>", "");
            //    //    lValue = lValue.Replace("</Zone3TransSpeed>", "");
            //    //    lBSF.Zone3TransSpeed = Convert.ToDouble(lValue);
            //    //}
            //    else if (L.StartsWith("<Zone1MachFactor>"))
            //    {
            //        lValue = L.Replace("<Zone1MachFactor>", "");
            //        lValue = lValue.Replace("</Zone1MachFactor>", "");
            //        lBSF.Zone1MachFactor = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<Zone2MachFactor>"))
            //    {
            //        lValue = L.Replace("<Zone2MachFactor>", "");
            //        lValue = lValue.Replace("</Zone2MachFactor>", "");
            //        lBSF.Zone2MachFactor = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<Zone3MachFactor>"))
            //    {
            //        lValue = L.Replace("<Zone3MachFactor>", "");
            //        lValue = lValue.Replace("</Zone3MachFactor>", "");
            //        lBSF.Zone3MachFactor = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<Zone1SlopeMultiplier>"))
            //    {
            //        lValue = L.Replace("<Zone1SlopeMultiplier>", "");
            //        lValue = lValue.Replace("</Zone1SlopeMultiplier>", "");
            //        lBSF.Zone1SlopeMultiplier = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<Zone3SlopeMultiplier>"))
            //    {
            //        lValue = L.Replace("<Zone3SlopeMultiplier>", "");
            //        lValue = lValue.Replace("</Zone3SlopeMultiplier>", "");
            //        lBSF.Zone3SlopeMultiplier = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<Zone1Slope>"))
            //    {
            //        lValue = L.Replace("<Zone1Slope>", "");
            //        lValue = lValue.Replace("</Zone1Slope>", "");
            //        lBSF.Zone1Slope = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<Zone1AngleFactor>"))
            //    {
            //        lValue = L.Replace("<Zone1AngleFactor>", "");
            //        lValue = lValue.Replace("</Zone1AngleFactor>", "");
            //        lBSF.Zone1AngleFactor = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<Zone3Slope>"))
            //    {
            //        lValue = L.Replace("<Zone3Slope>", "");
            //        lValue = lValue.Replace("</Zone3Slope>", "");
            //        lBSF.Zone3Slope = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<BarrelTwist>"))
            //    {
            //        lValue = L.Replace("<BarrelTwist>", "");
            //        lValue = lValue.Replace("</BarrelTwist>", "");
            //        lBSF.BarrelTwist = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<BarrelTwistDir>"))
            //    {
            //        lValue = L.Replace("<BarrelTwistDir>", "");
            //        lValue = lValue.Replace("</BarrelTwistDir>", "");
            //        lBSF.BarrelTwistDir = Convert.ToString(lValue);
            //    }
            //    else if (L.StartsWith("<BulletDiameter>"))
            //    {
            //        lValue = L.Replace("<BulletDiameter>", "");
            //        lValue = lValue.Replace("</BulletDiameter>", "");
            //        lBSF.BulletDiameter = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<BulletLength>"))
            //    {
            //        lValue = L.Replace("<BulletLength>", "");
            //        lValue = lValue.Replace("</BulletLength>", "");
            //        lBSF.BulletLength = Convert.ToDouble(lValue);
            //    }
            //    else if (L.StartsWith("<BulletWeight>"))
            //    {
            //        lValue = L.Replace("<BulletWeight>", "");
            //        lValue = lValue.Replace("</BulletWeight>", "");
            //        lBSF.BulletWeight = Convert.ToDouble(lValue);
            //    }
            //    // Value is not a calculation
            //    //else if (L.StartsWith("<BSG>"))
            //    //{
            //    //    lValue = L.Replace("<BSG>", "");
            //    //    lValue = lValue.Replace("</BSG>", "");
            //    //    lBSF.BSG = Convert.ToDouble(lValue);
            //    //}
            //    else if (L.StartsWith("<BulletShapeTyp>"))
            //    {
            //        lValue = L.Replace("<BulletShapeTyp>", "");
            //        lValue = lValue.Replace("</BulletShapeTyp>", "");
            //        lBSF.BulletShapeTyp = (BulletShapeEnum) Enum.Parse(typeof(BulletShapeEnum), lValue);
            //    }
            //}
            return lBSF;
        }
        public int SaveGunDB()
        {
            int lRtn = 0;
            string lGfilename;  //The path and file name of the the exported data file.
            string lGfileBak;   //The path and file name for the the backup data file.
            string lGF;         //Gun file string variable.

            //try
            //{
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\GunDB.gdf";
                lGfileBak = LawlerBallisticsFactory.DataFolder + "\\GunDB.BAK";
                lGF = _XML_Header;
                lGF = lGF + "<gundatabasefile>" + System.Environment.NewLine;
                foreach (Gun lg in LawlerBallisticsFactory.MyGuns)
                {
                    lGF = lGF + GunDatXML(lg);
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
            Gun ltg;

            lGfilename = LawlerBallisticsFactory.DataFolder + "\\GunDB.gdf";
            lGF = File.ReadAllText(lGfilename);
            XmlDocument lXML = new XmlDocument();

            lXML.LoadXml(lGF);
            XmlNode lGun = lXML.SelectSingleNode("gundatabasefile");
            foreach (XmlNode lgn in lGun)
            {
                ltg = LoadGun(lgn);
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
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\RecipeDB.rdf";
                lGfileBak = LawlerBallisticsFactory.DataFolder + "\\RecipeDB.BAK";
                lGF = _XML_Header;
                lGF = lGF + "<recipedatabasefile>" + System.Environment.NewLine;
                foreach (Recipe lr in LawlerBallisticsFactory.MyRecipes)
                {
                    lGF = lGF + RecipeDatXML(lr);
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
            Recipe ltg;

            //try
            //{
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\RecipeDB.rdf";
                lGF = File.ReadAllText(lGfilename);
                XmlDocument lXML = new XmlDocument();

                lXML.LoadXml(lGF);
                XmlNode lRecipe = lXML.SelectSingleNode("recipedatabasefile");
                foreach (XmlNode lgn in lRecipe)
                {
                    ltg = LoadRecipe(lgn);
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
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\BulletDB.bdf";
                lGfileBak = LawlerBallisticsFactory.DataFolder + "\\BulletDB.BAK";
                lGF = _XML_Header;
                lGF = lGF + "<bulletdatabasefile>" + System.Environment.NewLine;
                foreach (Bullet lg in LawlerBallisticsFactory.MyBullets)
                {
                    lGF = lGF + BulletDatXML(lg);
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
            Bullet ltg;

            try
            {
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\BulletDB.bdf";
                lGF = File.ReadAllText(lGfilename);
                XmlDocument lXML = new XmlDocument();

                lXML.LoadXml(lGF);
                XmlNode lBullets = lXML.SelectSingleNode("bulletdatabasefile");
                foreach (XmlNode lgn in lBullets)
                {
                    ltg = LoadBullet(lgn);
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
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\CaseDB.cdf";
                lGfileBak = LawlerBallisticsFactory.DataFolder + "\\CaseDB.BAK";
                lGF = _XML_Header;
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
                lGfilename = LawlerBallisticsFactory.AppDataFolder + "\\CaseDB.cdf";
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
                lGfilename = LawlerBallisticsFactory.AppDataFolder + "\\PrimerDB.pdf";
                lGfileBak = LawlerBallisticsFactory.AppDataFolder + "\\PrimerDB.BAK";
                lGF = _XML_Header;
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
                lGfilename = LawlerBallisticsFactory.AppDataFolder + "\\PrimerDB.pdf";
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
                lGfilename = LawlerBallisticsFactory.AppDataFolder + "\\PowderDB.ddf";
                lGfileBak = LawlerBallisticsFactory.AppDataFolder + "\\PowderDB.BAK";
                lGF = _XML_Header;
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
                lGfilename = LawlerBallisticsFactory.AppDataFolder + "\\PowderDB.ddf";
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
        public int SaveLoadoutData(LoadOut TargetLoadout)
        {
            int lRTN = 0;



            return lRTN;
        }


        public int ImportCartridgeDat(string File)
        {
            int lRTN = 0;



            return lRTN;
        }
        public int ImportRecipeDat(string File)
        {
            int lRTN = 0;



            return lRTN;
        }
        public int ExportCartridgeDat(string File)
        {
            int lRTN = 0;



            return lRTN;
        }
        public int ExportRecipeDat(string File)
        {
            int lRTN = 0;



            return lRTN;
        }
        public int ExportThisRecipe(string File)
        {
            int lRTN = 0;



            return lRTN;
        }
        public int BackupAllData(string Folder)
        {
            int lRTN = 0;



            return lRTN;
        }
        #endregion

        #region "Private Routines"

        #region "Class to XML"
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
        private string CaseDatXML(Case TargetCase)
        {

        }
        private string GunDatXML(Gun TargetGun)
        {
            string lGDS;
            string lsbid;

            lsbid = "";
            lGDS = "<gun>" + System.Environment.NewLine;
            lGDS = lGDS + "<id value=\"" + TargetGun.ID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lGDS = lGDS + "</id>" + System.Environment.NewLine;
            lGDS = lGDS + "<name value=\"" + TargetGun.Name.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lGDS = lGDS + "</name>" + System.Environment.NewLine;
            lGDS = lGDS + "<description value=\"" + TargetGun.Description.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lGDS = lGDS + "</description>" + System.Environment.NewLine;
            lGDS = lGDS + "<make value=\"" + TargetGun.Make.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lGDS = lGDS + "</make>" + System.Environment.NewLine;
            lGDS = lGDS + "<model value=\"" + TargetGun.Model.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lGDS = lGDS + "</model>" + System.Environment.NewLine;
            lGDS = lGDS + "<scopeheight value=\"" + TargetGun.ScopeHeight.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lGDS = lGDS + "</scopeheight>" + System.Environment.NewLine;
            if (TargetGun.SelectedBarrel != null) lsbid = TargetGun.SelectedBarrel.ID;
            lGDS = lGDS + "<selectedbarrelid value=\"" + lsbid + "\" type=\"string\">" + System.Environment.NewLine;
            lGDS = lGDS + "</selectedbarrelid>" + System.Environment.NewLine;
            lGDS = lGDS + "<gunPic value=\"" + BitConverter.ToString(LawlerBallisticsFactory.ImageToBytes(TargetGun.GunPic)) + "\" type='bytearray'>" + System.Environment.NewLine;
            lGDS = lGDS + "</gunPic>" + Environment.NewLine;
            lGDS = lGDS + "<barrels>" + Environment.NewLine;
            foreach (Barrel lb in TargetGun.Barrels)
            {
                lGDS = lGDS + BarrelDatXML(lb);
            }
            lGDS = lGDS + "</barrels>" + Environment.NewLine;
            lGDS = lGDS + "</gun>" + Environment.NewLine;

            return lGDS;
        }
        private string BarrelDatXML(Barrel TargetBarrel)
        {
            string lBDS;

            lBDS = "<barrel>" + Environment.NewLine;
            lBDS = lBDS + "<id value=\"" + TargetBarrel.ID.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lBDS = lBDS + "</id>" + Environment.NewLine;
            lBDS = lBDS + "<name value=\"" + TargetBarrel.Name.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lBDS = lBDS + "</name>" + Environment.NewLine;
            lBDS = lBDS + "<make value=\"" + TargetBarrel.Make.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lBDS = lBDS + "</make>" + Environment.NewLine;
            lBDS = lBDS + "<model value=\"" + TargetBarrel.Model.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lBDS = lBDS + "</model>" + Environment.NewLine;
            lBDS = lBDS + "<description value=\"" + TargetBarrel.Description.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lBDS = lBDS + "</description>" + Environment.NewLine;
            try
            {
                lBDS = lBDS + "<cartridgeid value=\"" + TargetBarrel.ParentCartridge.ID.ToString() + "\" type=\"string\">" + Environment.NewLine;
                lBDS = lBDS + "</cartridgeid>" + Environment.NewLine;
            }
            catch
            {
                //Parent Cartridge not specified.
            }
            lBDS = lBDS + "<twist value=\"" + TargetBarrel.Twist.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lBDS = lBDS + "</twist>" + Environment.NewLine;
            lBDS = lBDS + "<twistdir value=\"" + TargetBarrel.RiflingTwistDirection.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lBDS = lBDS + "</twistdir>" + Environment.NewLine;
            lBDS = lBDS + "<length value=\"" + TargetBarrel.Length.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lBDS = lBDS + "</length>" + Environment.NewLine;
            lBDS = lBDS + "<headspace value=\"" + TargetBarrel.HeadSpace.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lBDS = lBDS + "</headspace>" + Environment.NewLine;
            lBDS = lBDS + "<neckdepth value=\"" + TargetBarrel.NeckDepth.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lBDS = lBDS + "</neckdepth>" + Environment.NewLine;
            lBDS = lBDS + "<neckdiameter value=\"" + TargetBarrel.NeckDiameter.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lBDS = lBDS + "</neckdiameter>" + Environment.NewLine;
            try
            {
                lBDS = lBDS + "<selectedloadid value=\"" + TargetBarrel.SelectedRecipe.ID.ToString() + "\" type=\"string\">" + Environment.NewLine;
                lBDS = lBDS + "</selectedloadid>" + Environment.NewLine;
            }
            catch
            {
                // No load selected
            }
            lBDS = lBDS + "</barrel>" + Environment.NewLine;

            return lBDS;
        }
        private string BulletDatXML(Bullet TargetBullet)
        {
            string lRTN;

            lRTN = "<bullet>" + System.Environment.NewLine;
            lRTN = lRTN + "<id value=\"" + TargetBullet.ID.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</id>" + System.Environment.NewLine;
            lRTN = lRTN + "<bcg1 value=\"" + TargetBullet.BCg1.ToString() + "\" type='double'>" + System.Environment.NewLine;
            lRTN = lRTN + "</bcg1>" + System.Environment.NewLine;
            lRTN = lRTN + "<bcg7 value=\"" + TargetBullet.BCg7.ToString() + "\" type='double'>" + System.Environment.NewLine;
            lRTN = lRTN + "</bcg7>" + System.Environment.NewLine;
            lRTN = lRTN + "<diameter value=\"" + TargetBullet.Diameter.ToString() + "\" type='double'>" + System.Environment.NewLine;
            lRTN = lRTN + "</diameter>" + System.Environment.NewLine;
            lRTN = lRTN + "<length value=\"" + TargetBullet.Length.ToString() + "\" type='double'>" + System.Environment.NewLine;
            lRTN = lRTN + "</length>" + System.Environment.NewLine;
            lRTN = lRTN + "<bto value=\"" + TargetBullet.BTO.ToString() + "\" type='double'>" + System.Environment.NewLine;
            lRTN = lRTN + "</bto>" + System.Environment.NewLine;
            lRTN = lRTN + "<model value=\"" + TargetBullet.Model.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</model>" + System.Environment.NewLine;
            lRTN = lRTN + "<manufacturer value=\"" + TargetBullet.Manufacturer.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</manufacturer>" + System.Environment.NewLine;
            lRTN = lRTN + "<type value=\"" + TargetBullet.Type.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</type>" + System.Environment.NewLine;
            lRTN = lRTN + "<weight value=\"" + TargetBullet.Weight.ToString() + "\" type='double'>" + System.Environment.NewLine;
            lRTN = lRTN + "</weight>" + System.Environment.NewLine;
            lRTN = lRTN + "</bullet>" + System.Environment.NewLine;

            return lRTN;
        }
        private string RecipeDatXML(Recipe TargetRecipe)
        {
            string lRTN;

            lRTN = "<recipe>" + System.Environment.NewLine;
            lRTN = lRTN + "<id value=\"" + TargetRecipe.ID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</id>" + System.Environment.NewLine;
            lRTN = lRTN + "<name value=\"" + TargetRecipe.Name.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</name>" + System.Environment.NewLine;
            lRTN = lRTN + "<notes value=\"" + TargetRecipe.Notes.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</notes>" + System.Environment.NewLine;
            lRTN = lRTN + "<barrelid value=\"" + TargetRecipe.BarrelID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</barrelid>" + System.Environment.NewLine;
            lRTN = lRTN + "<bulletid value=\"" + TargetRecipe.BulletID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</bulletid>" + System.Environment.NewLine;
            lRTN = lRTN + "<bulletsbto value=\"" + TargetRecipe.BulletSortBTO.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</bulletsbto>" + System.Environment.NewLine;
            lRTN = lRTN + "<bulletsoal value=\"" + TargetRecipe.BulletSortOAL.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</bulletsoal>" + System.Environment.NewLine;
            lRTN = lRTN + "<bulletswt value=\"" + TargetRecipe.BulletSortWt.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</bulletswt>" + System.Environment.NewLine;
            lRTN = lRTN + "<cartid value=\"" + TargetRecipe.CartridgeID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</cartid>" + System.Environment.NewLine;
            lRTN = lRTN + "<caseid value=\"" + TargetRecipe.CaseID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</caseid>" + System.Environment.NewLine;
            lRTN = lRTN + "<casetl value=\"" + TargetRecipe.CaseTrimLength.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</casetl>" + System.Environment.NewLine;
            lRTN = lRTN + "<caseml value=\"" + TargetRecipe.BarrelCaseMaxLength.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</caseml>" + System.Environment.NewLine;
            lRTN = lRTN + "<cbto value=\"" + TargetRecipe.CBTO.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</cbto>" + System.Environment.NewLine;
            lRTN = lRTN + "<chargewt value=\"" + TargetRecipe.ChargeWt.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</chargewt>" + System.Environment.NewLine;
            lRTN = lRTN + "<coal value=\"" + TargetRecipe.COAL.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</coal>" + System.Environment.NewLine;
            lRTN = lRTN + "<dorate value=\"" + TargetRecipe.FoRate.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</dorate>" + System.Environment.NewLine;
            lRTN = lRTN + "<gunid value=\"" + TargetRecipe.GunID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</gunid>" + System.Environment.NewLine;
            lRTN = lRTN + "<headspace value=\"" + TargetRecipe.HeadSpace.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</headspace>" + System.Environment.NewLine;
            lRTN = lRTN + "<jump value=\"" + TargetRecipe.JumpDistance.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</jump>" + System.Environment.NewLine;
            lRTN = lRTN + "<neckclearance value=\"" + TargetRecipe.NeckClearance.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</neckclearance>" + System.Environment.NewLine;
            lRTN = lRTN + "<powderid value=\"" + TargetRecipe.PowderID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</powderid>" + System.Environment.NewLine;
            lRTN = lRTN + "<primerid value=\"" + TargetRecipe.PrimerID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</primerid>" + System.Environment.NewLine;
            lRTN = lRTN + "<Lots>" + System.Environment.NewLine;
            foreach (RecipeLot ll in TargetRecipe.Lots)
            {
                lRTN = lRTN + RecipeLotDatXML(ll);
            }
            lRTN = lRTN + "</Lots>" + System.Environment.NewLine;
            lRTN = lRTN + "</recipe>" + System.Environment.NewLine;

            return lRTN;
        }
        private string RecipeLotDatXML(RecipeLot TargetLot)
        {
            string lRTN;

            lRTN = "<Lot>" + System.Environment.NewLine;
            lRTN = lRTN + "<RecipeID value=\"" + TargetLot.RecipeID + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</RecipeID>" + System.Environment.NewLine;
            lRTN = lRTN + "<BulletLot value=\"" + TargetLot.BulletLot + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</BulletLot>" + System.Environment.NewLine;
            lRTN = lRTN + "<CaseLot value=\"" + TargetLot.CaseLot + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</CaseLot>" + System.Environment.NewLine;
            lRTN = lRTN + "<ID value=\"" + TargetLot.ID + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</ID>" + System.Environment.NewLine;
            lRTN = lRTN + "<LotDate value=\"" + TargetLot.LotDate + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</LotDate>" + System.Environment.NewLine;
            lRTN = lRTN + "<PowderLot value=\"" + TargetLot.PowderLot + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</PowderLot>" + System.Environment.NewLine;
            lRTN = lRTN + "<PrimerLot value=\"" + TargetLot.PrimerLot + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</PrimerLot>" + System.Environment.NewLine;
            lRTN = lRTN + "<SerialNo value=\"" + TargetLot.SerialNo + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</SerialNo>" + System.Environment.NewLine;
            lRTN = lRTN + "<totalcnt value=\"" + TargetLot.TotalCount + "\" type=\"int\">" + System.Environment.NewLine;
            lRTN = lRTN + "</totalcnt>" + System.Environment.NewLine;
            lRTN = lRTN + "<rounds>" + System.Environment.NewLine;
            foreach (Round lrnd in TargetLot.Rounds)
            {
                lRTN = lRTN + RoundDatXML(lrnd);
            }
            lRTN = lRTN + "</rounds>" + System.Environment.NewLine;
            lRTN = lRTN + "</Lot>" + System.Environment.NewLine;

            return lRTN;
        }
        private string RoundDatXML(Round TargetRound)
        {
            string lRTN;

            lRTN = "<round>" + System.Environment.NewLine;
            lRTN = lRTN + "<ID value=\"" + TargetRound.ID + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</ID>" + System.Environment.NewLine;
            lRTN = lRTN + "<RndNo value=\"" + TargetRound.RndNo.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</RndNo>" + System.Environment.NewLine;
            lRTN = lRTN + "<BBTO value=\"" + TargetRound.BBTO.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</BBTO>" + System.Environment.NewLine;
            lRTN = lRTN + "<BD value=\"" + TargetRound.BD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</BD>" + System.Environment.NewLine;
            lRTN = lRTN + "<BL value=\"" + TargetRound.BL.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</BL>" + System.Environment.NewLine;
            lRTN = lRTN + "<BW value=\"" + TargetRound.BW.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</BW>" + System.Environment.NewLine;
            lRTN = lRTN + "<CHS value=\"" + TargetRound.CHS.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</CHS>" + System.Environment.NewLine;
            lRTN = lRTN + "<CL value=\"" + TargetRound.CL.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</CL>" + System.Environment.NewLine;
            lRTN = lRTN + "<CNOD value=\"" + TargetRound.CNOD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</CNOD>" + System.Environment.NewLine;
            lRTN = lRTN + "<CVW value=\"" + TargetRound.CVW.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</CVW>" + System.Environment.NewLine;
            lRTN = lRTN + "<CW value=\"" + TargetRound.CW.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</CW>" + System.Environment.NewLine;
            lRTN = lRTN + "<CNID value=\"" + TargetRound.CNID.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</CNID>" + System.Environment.NewLine;
            lRTN = lRTN + "<PCW value=\"" + TargetRound.PCW.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</PCW>" + System.Environment.NewLine;
            lRTN = lRTN + "<CBTO value=\"" + TargetRound.CBTO.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</CBTO>" + System.Environment.NewLine;
            lRTN = lRTN + "<COAL value=\"" + TargetRound.COAL.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</COAL>" + System.Environment.NewLine;
            lRTN = lRTN + "<MV value=\"" + TargetRound.MV.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</MV>" + System.Environment.NewLine;
            lRTN = lRTN + "<VD value=\"" + TargetRound.VD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</VD>" + System.Environment.NewLine;
            lRTN = lRTN + "<VD value=\"" + TargetRound.VD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</VD>" + System.Environment.NewLine;
            lRTN = lRTN + "<HD value=\"" + TargetRound.HD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</HD>" + System.Environment.NewLine;
            lRTN = lRTN + "<VAD value=\"" + TargetRound.VAD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</VAD>" + System.Environment.NewLine;
            lRTN = lRTN + "<HAD value=\"" + TargetRound.HAD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</HAD>" + System.Environment.NewLine;
            lRTN = lRTN + "<RMSD value=\"" + TargetRound.RMSD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</RMSD>" + System.Environment.NewLine;
            lRTN = lRTN + "<GDV value=\"" + TargetRound.GDV.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</GDV>" + System.Environment.NewLine;
            lRTN = lRTN + "<VELAD value=\"" + TargetRound.VELAD.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</VELAD>" + System.Environment.NewLine;
            lRTN = lRTN + "</round>" + System.Environment.NewLine;

            return lRTN;
        }
        #endregion

        //private bool IsBallisticSol(string[] FileLines)
        //{
        //    bool lRtn = false;
        //    foreach (string L in FileLines)
        //    {
        //        if (L.Length >= "<Ballistic Solution File>".Length)
        //        {
        //            if (L == "<Ballistic Solution File>")
        //            {
        //                // A valid ballistic solution file.
        //                lRtn = true;
        //                break;
        //            }
        //        }
        //    }
        //    return lRtn;
        //}

        #region "XML to Class"
        private ObservableCollection<Cartridge> LoadCartridges(XmlNode Cartridges)
        {
            ObservableCollection<Cartridge> lRtn = new ObservableCollection<Cartridge>();
            Cartridge lCartridge = null;
            string lValue;
            XmlNode lcnode;

            //TODO: break the cartridge parse out to an individual cartridge parse and loop
            // that function here.

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
        private Gun LoadGun(XmlNode GunNode)
        {
            string lsb = "";
            string lNR;
            Gun lRTN = new Gun();
            Barrel lbarrel;

            XmlNode lName = GunNode.SelectSingleNode("name");
            lNR = lName.Attributes["value"].Value;
            lRTN.Name = lNR;
            XmlNode lid = GunNode.SelectSingleNode("id");
            lNR = lid.Attributes["value"].Value;
            lRTN.ID = lNR;
            XmlNode lmk = GunNode.SelectSingleNode("make");
            lNR = lmk.Attributes["value"].Value;
            lRTN.Make = lNR;
            XmlNode lmdl = GunNode.SelectSingleNode("model");
            lNR = lmdl.Attributes["value"].Value;
            lRTN.Model = lNR;
            lmdl = GunNode.SelectSingleNode("scopeheight");
            lNR = lmdl.Attributes["value"].Value;
            lRTN.ScopeHeight = Convert.ToDouble(lNR);
            try
            {
                XmlNode lsbid = GunNode.SelectSingleNode("selectedbarrelid");
                lNR = lsbid.Attributes["value"].Value;
                lsb = lNR;
            }
            catch
            {
                //selectedbarrelid doesnt exist
            }
            try
            {
                XmlNode lgp = GunNode.SelectSingleNode("gunPic");
                lNR = lgp.Attributes["value"].Value;
                byte[] lba = LawlerBallisticsFactory.StringToByteArray(lNR);
                Image lx = (Bitmap)((new ImageConverter()).ConvertFrom(lba));
                lRTN.GunPic = lx;
            }
            catch
            {
                // Image issue or no image
            }
            XmlNode ldesc = GunNode.SelectSingleNode("description");
            lNR = ldesc.Attributes["value"].Value;
            lRTN.Description = lNR;
            XmlNode lbsn = GunNode.SelectSingleNode("barrels");
            foreach (XmlNode lbn in lbsn)
            {
                lbarrel = LoadBarrel(lbn);
                lRTN.Barrels.Add(lbarrel);
            }
            foreach (Barrel lb in lRTN.Barrels)
            {
                if (lb.ID == lsb)
                {
                    lRTN.SelectedBarrel = lb;
                    break;
                }
            }

            return lRTN;
        }
        private Barrel LoadBarrel(XmlNode BarrelNode)
        {
            Barrel lRTN = new Barrel();

            XmlNode lBid = BarrelNode.SelectSingleNode("id");
            if (lBid != null) lRTN.ID = lBid.Attributes["value"].Value;
            XmlNode BarrelNodeame = BarrelNode.SelectSingleNode("name");
            if (BarrelNodeame != null) lRTN.Name = BarrelNodeame.Attributes["value"].Value;
            XmlNode lBmake = BarrelNode.SelectSingleNode("make");
            if (lBmake != null) lRTN.Make = lBmake.Attributes["value"].Value;
            XmlNode lBmodel = BarrelNode.SelectSingleNode("model");
            if (lBmodel != null) lRTN.Model = lBmodel.Attributes["value"].Value;
            XmlNode lBdesc = BarrelNode.SelectSingleNode("description");
            if (lBdesc != null) lRTN.Description = lBdesc.Attributes["value"].Value;
            XmlNode lBtwist = BarrelNode.SelectSingleNode("twist");
            if (lBtwist != null) lRTN.Twist = Convert.ToDouble(lBtwist.Attributes["value"].Value);
            XmlNode ltd = BarrelNode.SelectSingleNode("twistdir");
            if (ltd != null) lRTN.RiflingTwistDirection = ltd.Attributes["value"].Value;
            XmlNode llgth = BarrelNode.SelectSingleNode("length");
            if (llgth != null) lRTN.Length = Convert.ToDouble(llgth.Attributes["value"].Value);
            XmlNode lhs = BarrelNode.SelectSingleNode("headspace");
            if (lhs != null) lRTN.HeadSpace = Convert.ToDouble(lhs.Attributes["value"].Value);
            XmlNode lnd = BarrelNode.SelectSingleNode("neckdepth");
            if (lnd != null) lRTN.NeckDepth = Convert.ToDouble(lnd.Attributes["value"].Value);
            XmlNode ldia = BarrelNode.SelectSingleNode("neckdiameter");
            if (ldia != null) lRTN.NeckDiameter = Convert.ToDouble(ldia.Attributes["value"].Value);
            XmlNode lcarid = BarrelNode.SelectSingleNode(" cartridgeid");
            if (lcarid != null) lRTN.CartridgeID = lcarid.Attributes["value"].Value;
            foreach (Cartridge lcartridge in LawlerBallisticsFactory.MyCartridges)
            {
                if (lRTN.CartridgeID == lcartridge.ID)
                {
                    lRTN.ParentCartridge = lcartridge;
                    break;
                }
            }

            return lRTN;
        }
        private Bullet LoadBullet(XmlNode BulletNode)
        {
            Bullet lRTN = new Bullet();
            string lNR;

            XmlNode lid = BulletNode.SelectSingleNode("id");
            lNR = lid.Attributes["value"].Value;
            lRTN.ID = lNR;
            XmlNode lmk = BulletNode.SelectSingleNode("bcg1");
            lNR = lmk.Attributes["value"].Value;
            lRTN.BCg1 = Convert.ToDouble(lNR);
            try
            {
                XmlNode lmkk = BulletNode.SelectSingleNode("bcg7");
                lNR = lmkk.Attributes["value"].Value;
                lRTN.BCg7 = Convert.ToDouble(lNR);
            }
            catch
            {
                //Older file does not have BCg7
            }
            XmlNode lmdl = BulletNode.SelectSingleNode("diameter");
            lNR = lmdl.Attributes["value"].Value;
            lRTN.Diameter = Convert.ToDouble(lNR);
            XmlNode ldesc = BulletNode.SelectSingleNode("length");
            lNR = ldesc.Attributes["value"].Value;
            lRTN.Length = Convert.ToDouble(lNR);
            XmlNode lbto = BulletNode.SelectSingleNode("bto");
            lNR = lbto.Attributes["value"].Value;
            lRTN.BTO = Convert.ToDouble(lNR);
            XmlNode lbulletsbto = BulletNode.SelectSingleNode("model");
            lNR = lbulletsbto.Attributes["value"].Value;
            lRTN.Model = lNR;
            XmlNode lbulletsoal = BulletNode.SelectSingleNode("manufacturer");
            lNR = lbulletsoal.Attributes["value"].Value;
            lRTN.Manufacturer = lNR;
            XmlNode lbulletswt = BulletNode.SelectSingleNode("type");
            lNR = lbulletswt.Attributes["value"].Value;
            lRTN.Type = lNR;
            XmlNode lcartid = BulletNode.SelectSingleNode("weight");
            lNR = lcartid.Attributes["value"].Value;
            lRTN.Weight = Convert.ToDouble(lNR);

            return lRTN;
        }
        private Recipe LoadRecipe(XmlNode RecipeNode)
        {
            string lNR;
            Recipe lRTN = new Recipe();

            XmlNode lName = RecipeNode.SelectSingleNode("name");
            lNR = lName.Attributes["value"].Value;
            lRTN.Name = lNR;
            XmlNode lid = RecipeNode.SelectSingleNode("id");
            lNR = lid.Attributes["value"].Value;
            lRTN.ID = lNR;
            XmlNode lmk = RecipeNode.SelectSingleNode("notes");
            lNR = lmk.Attributes["value"].Value;
            lRTN.Notes = lNR;
            XmlNode lmdl = RecipeNode.SelectSingleNode("barrelid");
            lNR = lmdl.Attributes["value"].Value;
            lRTN.BarrelID = lNR;
            XmlNode ldesc = RecipeNode.SelectSingleNode("bulletid");
            lNR = ldesc.Attributes["value"].Value;
            lRTN.BulletID = lNR;
            lRTN.RecpBullet = LawlerBallisticsFactory.GetBullet(lRTN.BulletID);
            XmlNode lbulletsbto = RecipeNode.SelectSingleNode("bulletsbto");
            lNR = lbulletsbto.Attributes["value"].Value;
            lRTN.BulletSortBTO = Convert.ToDouble(lNR);
            XmlNode lbulletsoal = RecipeNode.SelectSingleNode("bulletsoal");
            lNR = lbulletsoal.Attributes["value"].Value;
            lRTN.BulletSortOAL = Convert.ToDouble(lNR);
            XmlNode lbulletswt = RecipeNode.SelectSingleNode("bulletswt");
            lNR = lbulletswt.Attributes["value"].Value;
            lRTN.BulletSortWt = Convert.ToDouble(lNR);
            XmlNode lcartid = RecipeNode.SelectSingleNode("cartid");
            lNR = lcartid.Attributes["value"].Value;
            lRTN.CartridgeID = lNR;
            lRTN.RecpCartridge = LawlerBallisticsFactory.GetCartridge(lRTN.CartridgeID);
            XmlNode lcaseID = RecipeNode.SelectSingleNode("caseid");
            lNR = lcaseID.Attributes["value"].Value;
            lRTN.CaseID = lNR;
            lRTN.RecpCase = LawlerBallisticsFactory.GetCase(lRTN.CaseID);
            XmlNode lcasetl = RecipeNode.SelectSingleNode("casetl");
            lNR = lcasetl.Attributes["value"].Value;
            lRTN.CaseTrimLength = Convert.ToDouble(lNR);
            lcasetl = RecipeNode.SelectSingleNode("caseml");
            lNR = "";
            lNR = lcasetl.Attributes["value"].Value;
            lRTN.BarrelCaseMaxLength = Convert.ToDouble(lNR);
            XmlNode lcbto = RecipeNode.SelectSingleNode("cbto");
            lNR = lcbto.Attributes["value"].Value;
            lRTN.CBTO = Convert.ToDouble(lNR);
            XmlNode lchargewt = RecipeNode.SelectSingleNode("chargewt");
            lNR = lchargewt.Attributes["value"].Value;
            lRTN.ChargeWt = Convert.ToDouble(lNR);
            XmlNode lcoal = RecipeNode.SelectSingleNode("coal");
            lNR = lcoal.Attributes["value"].Value;
            lRTN.COAL = Convert.ToDouble(lNR);
            XmlNode ldorate = RecipeNode.SelectSingleNode("dorate");
            lNR = ldorate.Attributes["value"].Value;
            lRTN.FoRate = Convert.ToDouble(lNR);
            XmlNode lgunid = RecipeNode.SelectSingleNode("gunid");
            lNR = lgunid.Attributes["value"].Value;
            lRTN.GunID = lNR;
            XmlNode lheadspace = RecipeNode.SelectSingleNode("headspace");
            lNR = lheadspace.Attributes["value"].Value;
            lRTN.HeadSpace = Convert.ToDouble(lNR);
            XmlNode ljump = RecipeNode.SelectSingleNode("jump");
            lNR = ljump.Attributes["value"].Value;
            lRTN.JumpDistance = Convert.ToDouble(lNR);
            XmlNode lneckclearance = RecipeNode.SelectSingleNode("neckclearance");
            lNR = lneckclearance.Attributes["value"].Value;
            lRTN.NeckClearance = Convert.ToDouble(lNR);
            XmlNode lpowderid = RecipeNode.SelectSingleNode("powderid");
            lNR = lpowderid.Attributes["value"].Value;
            lRTN.PowderID = lNR;
            lRTN.RecpPowder = LawlerBallisticsFactory.GetPowder(lRTN.PowderID);
            XmlNode lprimermfgr = RecipeNode.SelectSingleNode("primerid");
            lNR = lprimermfgr.Attributes["value"].Value;
            lRTN.PrimerID = lNR;
            lRTN.RecpPrimer = LawlerBallisticsFactory.GetPrimer(lRTN.PrimerID);

            lRTN.Lots = new ObservableCollection<RecipeLot>();
            XmlNode lLots = RecipeNode.SelectSingleNode("Lots");
            RecipeLot lRLC;
            if (lLots != null)
            {
                foreach (XmlNode lLot in lLots)
                {
                    lRLC = LoadRecipeLot(lLot);
                    lRTN.Lots.Add(lRLC);
                }
            }

            return lRTN;
        }
        private RecipeLot LoadRecipeLot(XmlNode RecipeLotNode)
        {
            string lLotPropVal;
            RecipeLot lRTN = new RecipeLot();
            XmlNode lLotProp;

            lLotPropVal = "";
            lLotProp = RecipeLotNode.SelectSingleNode("RecipeID");
            lLotPropVal = lLotProp.Attributes["value"].Value;
            lRTN.RecipeID = lLotPropVal;
            lLotPropVal = "";
            lLotProp = RecipeLotNode.SelectSingleNode("totalcnt");
            lLotPropVal = lLotProp.Attributes["value"].Value;
            lRTN.TotalCount = Convert.ToInt32(lLotPropVal);
            lLotPropVal = "";
            lLotProp = RecipeLotNode.SelectSingleNode("BulletLot");
            lLotPropVal = lLotProp.Attributes["value"].Value;
            lRTN.BulletLot = lLotPropVal;
            lLotPropVal = "";
            lLotProp = RecipeLotNode.SelectSingleNode("CaseLot");
            lLotPropVal = lLotProp.Attributes["value"].Value;
            lRTN.CaseLot = lLotPropVal;
            lLotPropVal = "";
            lLotProp = RecipeLotNode.SelectSingleNode("ID");
            lLotPropVal = lLotProp.Attributes["value"].Value;
            lRTN.ID = lLotPropVal;
            lLotPropVal = "";
            lLotProp = RecipeLotNode.SelectSingleNode("LotDate");
            lLotPropVal = lLotProp.Attributes["value"].Value;
            lRTN.LotDate = lLotPropVal;
            lLotPropVal = "";
            lLotProp = RecipeLotNode.SelectSingleNode("PowderLot");
            lLotPropVal = lLotProp.Attributes["value"].Value;
            lRTN.PowderLot = lLotPropVal;
            lLotPropVal = "";
            lLotProp = RecipeLotNode.SelectSingleNode("PrimerLot");
            lLotPropVal = lLotProp.Attributes["value"].Value;
            lRTN.PrimerLot = lLotPropVal;
            lLotPropVal = "";
            lLotProp = RecipeLotNode.SelectSingleNode("SerialNo");
            lLotPropVal = lLotProp.Attributes["value"].Value;
            lRTN.SerialNo = lLotPropVal;

            lRTN.Rounds = new ObservableCollection<Round>();
            XmlNode lRds = RecipeLotNode.SelectSingleNode("rounds");
            Round lRnd;
            if (lRds != null)
            {
                foreach (XmlNode lrd in lRds)
                {
                    lRnd = LoadRound(lrd);
                    lRTN.Rounds.Add(lRnd);
                }
            }
            return lRTN;
        }
        private Round LoadRound(XmlNode RoundNode)
        {
            Round lRTN = new Round();
            XmlNode lRdprop;
            string lRdPropVal;

            lRdprop = RoundNode.SelectSingleNode("ID");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.ID = lRdPropVal;
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("RndNo");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.RndNo = Convert.ToInt32(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("BBTO");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.BBTO = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("BD");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.BD = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("BL");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.BL = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("BW");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.BW = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("CHS");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.CHS = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("CL");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.CL = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("CNOD");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.CNOD = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("CNID");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.CNID = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("CVW");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.CVW = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("CW");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.CW = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("PCW");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.PCW = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("CBTO");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.CBTO = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("COAL");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.COAL = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("MV");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.MV = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("VD");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.VD = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("HD");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.HD = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("VAD");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.VAD = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("HAD");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.HAD = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("RMSD");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.RMSD = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("GDV");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.GDV = Convert.ToDouble(lRdPropVal);
            lRdPropVal = "";
            lRdprop = RoundNode.SelectSingleNode("VELAD");
            lRdPropVal = lRdprop.Attributes["value"].Value;
            lRTN.VELAD = Convert.ToDouble(lRdPropVal);

            return lRTN;
        }
        #endregion
        #endregion
    }
}
