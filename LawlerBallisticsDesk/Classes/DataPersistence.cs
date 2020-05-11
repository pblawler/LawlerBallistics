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
        private string[] _DataFileList;
        #endregion

        #region "Properties"
        public string CartridgeFileFilter { get { return "Cartridge Data Files (*.cdf)|*.cdf"; } }
        public string BallisticFileFilter { get { return "Ballistic Data Files (*.bdf)|*.bdf"; } }
        #endregion

        #region "Constructor"
        public DataPersistence ()
        {
            _DataFileList = new string[]
            {
                "default.bdf",
                "CartridgeDB.cdf",
                "BulletDB.bdf",
                "CaseDB.cdf",
                "GunDB.gdf",
                "PowderDB.ddf",
                "PrimerDB.pdf",
                "RecipeDB.rdf"
            };
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
            

            //TODO: Add a restore default files function for all initially provided files.
            //Restore missing file from installation files.
            foreach (string lF in _DataFileList)
            {
                //Load default data file for ballistic solution menu view
                string lDatFile = LawlerBallisticsFactory.DataFolder + "\\" + lF;
                string lSource = LawlerBallisticsFactory.DefaultDataFolder + "\\" + lF;
                if (!File.Exists(lDatFile))
                {
                    File.Copy(lSource, lDatFile);
                }
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
            foreach(XmlNode lc in lCartridges)
            {
                lRtn.Add(LoadCartridgeData(lc));
            }

            return lRtn;
        }
        public int SaveBallisticSolutionData(Solution BallisticSolutionData, string FileName="")
        {
            int lRtn = 0;
            string lBF;         //Balistic file string variable.

            if (FileName == "") FileName = LawlerBallisticsFactory.DataFolder + "\\default.bdf";

            //try
            //{
            lBF = _XML_Header;
            lBF = lBF + SolutionDatXML(BallisticSolutionData);

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            File.WriteAllText(FileName, lBF);
            //}
            //catch
            //{
            //    lRtn = -1;
            //}
            return lRtn;
        }
        public Solution ParseBallisticSolution(string FileName="")
        {
            Solution lBSF;
            string lSF;

            lSF = File.ReadAllText(FileName);
            XmlDocument lXML = new XmlDocument();

            lXML.LoadXml(lSF);
            XmlNode lBF = lXML.SelectSingleNode("BallisticSolutionFile");
            lBSF = LoadSolution(lBF);

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
                ltg = LoadGunData(lgn);
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
                    ltg = LoadRecipeData(lgn);
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
                    ltg = LoadBulletData(lgn);
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
                lGF = lGF + "<casedatabasefile>" + Environment.NewLine;
                foreach (Case lg in LawlerBallisticsFactory.MyCases)
                {
                    lGF = lGF + CaseDatXML(lg);
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
            Case ltg;

            //try
            //{
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\CaseDB.cdf";
                lGF = File.ReadAllText(lGfilename);
                XmlDocument lXML = new XmlDocument();

                lXML.LoadXml(lGF);
                XmlNode lBullets = lXML.SelectSingleNode("casedatabasefile");
                foreach (XmlNode lgn in lBullets)
                {
                    ltg = LoadCaseData(lgn);
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
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\PrimerDB.pdf";
                lGfileBak = LawlerBallisticsFactory.DataFolder + "\\PrimerDB.BAK";
                lGF = _XML_Header;
                lGF = lGF + "<primerdatabasefile>" + System.Environment.NewLine;
                foreach (Primer lg in LawlerBallisticsFactory.MyPrimers)
                {
                    lGF = lGF + PrimerDatXML(lg);
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
            Primer ltg;

            try
            {
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\PrimerDB.pdf";
                lGF = File.ReadAllText(lGfilename);
                XmlDocument lXML = new XmlDocument();

                lXML.LoadXml(lGF);
                XmlNode lBullets = lXML.SelectSingleNode("primerdatabasefile");
                foreach (XmlNode lgn in lBullets)
                {
                    ltg = LoadPrimerData(lgn);
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
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\PowderDB.ddf";
                lGfileBak = LawlerBallisticsFactory.DataFolder + "\\PowderDB.BAK";
                lGF = _XML_Header;
                lGF = lGF + "<powderdatabasefile>" + System.Environment.NewLine;
                foreach (Powder lg in LawlerBallisticsFactory.MyPowders)
                {
                    lGF = lGF + PowderDatXML(lg);
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
            Powder ltg;

            try
            {
                lGfilename = LawlerBallisticsFactory.DataFolder + "\\PowderDB.ddf";
                lGF = File.ReadAllText(lGfilename);
                XmlDocument lXML = new XmlDocument();

                lXML.LoadXml(lGF);
                XmlNode lBullets = lXML.SelectSingleNode("powderdatabasefile");
                foreach (XmlNode lgn in lBullets)
                {
                    ltg = LoadPowderData(lgn);
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
            string lRTN;

            lRTN = "<case>" + Environment.NewLine;
            lRTN = lRTN + "<id value=\"" + TargetCase.ID.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lRTN = lRTN + "</id>" + Environment.NewLine;
            lRTN = lRTN + "<cartridgeid value=\"" + TargetCase.CartridgeID.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lRTN = lRTN + "</cartridgeid>" + Environment.NewLine;
            lRTN = lRTN + "<name value=\"" + TargetCase.Name.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lRTN = lRTN + "</name>" + Environment.NewLine;
            lRTN = lRTN + "<model value=\"" + TargetCase.Model.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lRTN = lRTN + "</model>" + Environment.NewLine;
            lRTN = lRTN + "<manufacturer value=\"" + TargetCase.Manufacturer.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lRTN = lRTN + "</manufacturer>" + Environment.NewLine;
            lRTN = lRTN + "<primersize value=\"" + TargetCase.PrimerSize.ToString() + "\" type=\"string\">" + Environment.NewLine;
            lRTN = lRTN + "</primersize>" + Environment.NewLine;
            lRTN = lRTN + "</case>" + Environment.NewLine;

            return lRTN;
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
        private string PrimerDatXML(Primer TargetPrimer)
        {
            string lRTN;

            lRTN = "<primer>" + System.Environment.NewLine;
            lRTN = lRTN + "<id value=\"" + TargetPrimer.ID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</id>" + System.Environment.NewLine;
            lRTN = lRTN + "<name value=\"" + TargetPrimer.Name.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</name>" + System.Environment.NewLine;
            lRTN = lRTN + "<model value=\"" + TargetPrimer.Model.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</model>" + System.Environment.NewLine;
            lRTN = lRTN + "<manufacturer value=\"" + TargetPrimer.Manufacturer.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</manufacturer>" + System.Environment.NewLine;
            lRTN = lRTN + "<type value=\"" + TargetPrimer.Type.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</type>" + System.Environment.NewLine;
            lRTN = lRTN + "</primer>" + System.Environment.NewLine;

            return lRTN;
        }
        private string PowderDatXML(Powder TargetPowder)
        {
            string lRTN;

            lRTN = "<powder>" + System.Environment.NewLine;
            lRTN = lRTN + "<id value=\"" + TargetPowder.ID.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</id>" + System.Environment.NewLine;
            lRTN = lRTN + "<name value=\"" + TargetPowder.Name.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</name>" + System.Environment.NewLine;
            lRTN = lRTN + "<model value=\"" + TargetPowder.Model.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</model>" + System.Environment.NewLine;
            lRTN = lRTN + "<manufacturer value=\"" + TargetPowder.Manufacturer.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</manufacturer>" + System.Environment.NewLine;
            lRTN = lRTN + "<basetype value=\"" + TargetPowder.BaseType.ToString() + "\" type='string'>" + System.Environment.NewLine;
            lRTN = lRTN + "</basetype>" + System.Environment.NewLine;
            lRTN = lRTN + "</powder>" + System.Environment.NewLine;

            return lRTN;
        }
        private string SolutionDatXML(Solution TargetSolution)
        {
            string lRTN;
            
            lRTN = "<BallisticSolutionFile>" + Environment.NewLine;
            lRTN = lRTN + ScenarioDatXML(TargetSolution.MyScenario);
            lRTN = lRTN + "</BallisticSolutionFile>" + Environment.NewLine;

            return lRTN;
        }
        private string ScenarioDatXML(Scenario TargetScenario)
        {
            string lRTN;

            lRTN = "<ScenarioData>" + Environment.NewLine;
            lRTN = lRTN + AtmosphericDatXML(TargetScenario.MyAtmospherics);
            lRTN = lRTN + ShooterDatXML(TargetScenario.MyShooter);
            lRTN = lRTN + "</ScenarioData>" + Environment.NewLine;

            return lRTN;
        }
        private string ShooterDatXML(Shooter TargetShooter)
        {
            string lRTN;

            lRTN = "<ShooterData>" + Environment.NewLine;
            lRTN = lRTN + LoadoutDatXML(TargetShooter.MyLoadOut);
            lRTN = lRTN + LocationDatXML(TargetShooter.MyLocation);
            lRTN = lRTN + "</ShooterData>" + Environment.NewLine;

            return lRTN;
        }
        private string LoadoutDatXML(LoadOut TargetLoadout)
        {
            string lRTN;

            lRTN = "<LoadoutData>" + Environment.NewLine;
            lRTN = lRTN + "<BSG value=\"" + TargetLoadout.BSG.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</BSG>" + System.Environment.NewLine;
            lRTN = lRTN + "<ID value=\"" + TargetLoadout.ID.ToString() + "\" type=\"string\">" + System.Environment.NewLine;
            lRTN = lRTN + "</ID>" + System.Environment.NewLine;
            lRTN = lRTN + "<MaxRange value=\"" + TargetLoadout.MaxRange.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</MaxRange>" + System.Environment.NewLine;
            lRTN = lRTN + "<MuzzleVelocity value=\"" + TargetLoadout.MuzzleVelocity.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</MuzzleVelocity>" + System.Environment.NewLine;
            lRTN = lRTN + "<ScopeHeight value=\"" + TargetLoadout.ScopeHeight.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</ScopeHeight>" + System.Environment.NewLine;
            lRTN = lRTN + "<SelectedBarrelID value=\"" + TargetLoadout.SelectedBarrelID + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</SelectedBarrelID>" + System.Environment.NewLine;
            lRTN = lRTN + "<SelectedCartridge>" + Environment.NewLine;
            lRTN = lRTN + RecipeDatXML(TargetLoadout.SelectedCartridge);
            lRTN = lRTN + "</SelectedCartridge>" + Environment.NewLine;
            lRTN = lRTN + GunDatXML(TargetLoadout.SelectedGun);
            lRTN = lRTN + ZeroDatXML(TargetLoadout.zeroData);
            lRTN = lRTN + DragSlopeDatXML(TargetLoadout.MyDragSlopeData);
            lRTN = lRTN + "</LoadoutData>" + Environment.NewLine;

            return lRTN;
        }
        private string ZeroDatXML(ZeroData TargetZeroData)
        {
            string lRTN;

            lRTN = "<ZeroData>" + Environment.NewLine;
            lRTN = lRTN + "<MidRange value=\"" + TargetZeroData.MidRange.ToString() + "\" type=\"double\">" + System.Environment.NewLine;                       
            lRTN = lRTN + "</MidRange>" + System.Environment.NewLine;
            lRTN = lRTN + "<NearZeroRange value=\"" + TargetZeroData.NearZeroRange.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</NearZeroRange>" + System.Environment.NewLine;
            lRTN = lRTN + "<PointBlankRange value=\"" + TargetZeroData.PointBlankRange.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</PointBlankRange>" + System.Environment.NewLine;
            lRTN = lRTN + "<UseMaxRise value=\"" + TargetZeroData.UseMaxRise.ToString() + "\" type=\"bool\">" + System.Environment.NewLine;
            lRTN = lRTN + "</UseMaxRise>" + System.Environment.NewLine;
            lRTN = lRTN + "<ZeroMaxRise value=\"" + TargetZeroData.ZeroMaxRise.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</ZeroMaxRise>" + System.Environment.NewLine;
            lRTN = lRTN + "<ZeroRange value=\"" + TargetZeroData.ZeroRange.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</ZeroRange>" + System.Environment.NewLine;
            lRTN = lRTN + "<ShotDirection value=\"" + TargetZeroData.ShotDirection.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</ShotDirection>" + Environment.NewLine;
            lRTN = lRTN + "<ShotAngle value=\"" + TargetZeroData.ShotAngle.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</ShotAngle>" + Environment.NewLine;
            lRTN = lRTN + "<ShotDistance value=\"" + TargetZeroData.ShotDistance.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</ShotDistance>" + Environment.NewLine;
            lRTN = lRTN + "<WindEffectiveDirection value=\"" + TargetZeroData.WindEffectiveDirection.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</WindEffectiveDirection>" + Environment.NewLine;
            lRTN = lRTN + "<MuzzleVelocity value=\"" + TargetZeroData.MuzzleVelocity.ToString() + "\" type=\"double\">" + System.Environment.NewLine;
            lRTN = lRTN + "</MuzzleVelocity>" + Environment.NewLine;
            lRTN = lRTN + "<ShooterLocationData>" + Environment.NewLine;
            lRTN = lRTN + LocationDatXML(TargetZeroData.ShooterLoc);
            lRTN = lRTN + "</ShooterLocationData>" + Environment.NewLine;
            lRTN = lRTN + "<TargetLocationData>" + Environment.NewLine;
            lRTN = lRTN + LocationDatXML(TargetZeroData.TargetLoc);
            lRTN = lRTN + "</TargetLocationData>" + Environment.NewLine;
            lRTN = lRTN + AtmosphericDatXML(TargetZeroData.atmospherics);
            lRTN = lRTN + "</ZeroData>" + Environment.NewLine;

            return lRTN;
        }
        private string LocationDatXML(LocationData TargetLocation)
        {
            string lRTN;

            lRTN = "<LocationData>" + Environment.NewLine;
            lRTN = lRTN + "<Altitude value=\"" + TargetLocation.Altitude + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Altitude>" + Environment.NewLine;
            lRTN = lRTN + "<Latitude value=\"" + TargetLocation.Latitude + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Latitude>" + Environment.NewLine;
            lRTN = lRTN + "<Longitude value=\"" + TargetLocation.Longitude + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Longitude>" + Environment.NewLine;
            lRTN = lRTN + "</LocationData>" + Environment.NewLine;

            return lRTN;
        }
        private string AtmosphericDatXML(Atmospherics TargetAtmospherics)
        {
            string lRTN;

            lRTN = "<AtmosphericData>" + Environment.NewLine;
            lRTN = lRTN + "<DensityAlt value=\"" + TargetAtmospherics.DensityAlt + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</DensityAlt>" + Environment.NewLine;
            lRTN = lRTN + "<FeelsLike value=\"" + TargetAtmospherics.FeelsLike + "\" type=\"double\" unit=\"" + TargetAtmospherics.FeelsLikeUnits + "\">" + Environment.NewLine;
            lRTN = lRTN + "</FeelsLike>" + Environment.NewLine;
            lRTN = lRTN + "<HumidityRel value=\"" + TargetAtmospherics.HumidityRel + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</HumidityRel>" + Environment.NewLine;
            lRTN = lRTN + "<LastUpdated value=\"" + TargetAtmospherics.LastUpdated + "\" type=\"DateTime\">" + Environment.NewLine;
            lRTN = lRTN + "</LastUpdated>" + Environment.NewLine;
            lRTN = lRTN + "<Precipitation value=\"" + TargetAtmospherics.Precipitation + "\" type=\"string\">" + Environment.NewLine;
            lRTN = lRTN + "</Precipitation>" + Environment.NewLine;
            lRTN = lRTN + "<Pressure value=\"" + TargetAtmospherics.Pressure + "\" type=\"double\" unit=\"" + TargetAtmospherics.PressureUnits + "\">" + Environment.NewLine;
            lRTN = lRTN + "</Pressure>" + Environment.NewLine;
            lRTN = lRTN + "<Station value=\"" + TargetAtmospherics.Station + "\" type=\"string\">" + Environment.NewLine;
            lRTN = lRTN + "</Station>" + Environment.NewLine;
            lRTN = lRTN + "<SunRise value=\"" + TargetAtmospherics.SunRise + "\" type=\"DateTime\">" + Environment.NewLine;
            lRTN = lRTN + "</SunRise>" + Environment.NewLine;
            lRTN = lRTN + "<SunSet value=\"" + TargetAtmospherics.SunSet + "\" type=\"DateTime\">" + Environment.NewLine;
            lRTN = lRTN + "</SunSet>" + Environment.NewLine;
            lRTN = lRTN + "<Temp value=\"" + TargetAtmospherics.Temp + "\" type=\"double\" unit=\"" + TargetAtmospherics.TempUnits + "\">" + Environment.NewLine;
            lRTN = lRTN + "</Temp>" + Environment.NewLine;
            lRTN = lRTN + "<Weather value=\"" + TargetAtmospherics.Weather + "\" type=\"string\">" + Environment.NewLine;
            lRTN = lRTN + "</Weather>" + Environment.NewLine;
            lRTN = lRTN + "<WindDirection value=\"" + TargetAtmospherics.WindDirection + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</WindDirection>" + Environment.NewLine;
            lRTN = lRTN + "<WindDirTxt value=\"" + TargetAtmospherics.WindDirTxt + "\" type=\"string\">" + Environment.NewLine;
            lRTN = lRTN + "</WindDirTxt>" + Environment.NewLine;
            lRTN = lRTN + "<WindSpeed value=\"" + TargetAtmospherics.WindSpeed + "\" type=\"double\" unit=\"" + TargetAtmospherics.WindSpeedUnits + "\">" + Environment.NewLine;
            lRTN = lRTN + "</WindSpeed>" + Environment.NewLine;
            lRTN = lRTN + "</AtmosphericData>" + Environment.NewLine;

            return lRTN;
        }
        private string DragSlopeDatXML(DragSlopeData TargetDragSlopeData)
        {
            string lRTN;

            lRTN = "<DragSlopeData>" + Environment.NewLine;
            lRTN = lRTN + "<BCg1 value=\"" + TargetDragSlopeData.BCg1.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</BCg1>" + Environment.NewLine;
            lRTN = lRTN + "<BCz2 value=\"" + TargetDragSlopeData.BCz2.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</BCz2>" + Environment.NewLine;
            lRTN = lRTN + "<D1 value=\"" + TargetDragSlopeData.D1.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</D1>" + Environment.NewLine;
            lRTN = lRTN + "<D2 value=\"" + TargetDragSlopeData.D2.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</D2>" + Environment.NewLine;
            lRTN = lRTN + "<F2 value=\"" + TargetDragSlopeData.F2.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</F2>" + Environment.NewLine;
            lRTN = lRTN + "<F3 value=\"" + TargetDragSlopeData.F3.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</F3>" + Environment.NewLine;
            lRTN = lRTN + "<F4 value=\"" + TargetDragSlopeData.F4.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</F4>" + Environment.NewLine;
            lRTN = lRTN + "<Fo value=\"" + TargetDragSlopeData.Fo.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Fo>" + Environment.NewLine;
            lRTN = lRTN + "<V1 value=\"" + TargetDragSlopeData.V1.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</V1>" + Environment.NewLine;
            lRTN = lRTN + "<V2 value=\"" + TargetDragSlopeData.V2.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</V2>" + Environment.NewLine;
            lRTN = lRTN + "<Zone1AngleFactor value=\"" + TargetDragSlopeData.Zone1AngleFactor.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Zone1AngleFactor>" + Environment.NewLine;
            lRTN = lRTN + "<Zone1MachFactor value=\"" + TargetDragSlopeData.Zone1MachFactor.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Zone1MachFactor>" + Environment.NewLine;
            lRTN = lRTN + "<Zone1Slope value=\"" + TargetDragSlopeData.Zone1Slope.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Zone1Slope>" + Environment.NewLine;
            lRTN = lRTN + "<Zone1SlopeMultiplier value=\"" + TargetDragSlopeData.Zone1SlopeMultiplier.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Zone1SlopeMultiplier>" + Environment.NewLine;
            lRTN = lRTN + "<Zone2MachFactor value=\"" + TargetDragSlopeData.Zone2MachFactor.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Zone2MachFactor>" + Environment.NewLine;
            lRTN = lRTN + "<Zone2Slope value=\"" + TargetDragSlopeData.Zone2Slope.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Zone2Slope>" + Environment.NewLine;
            lRTN = lRTN + "<Zone3MachFactor value=\"" + TargetDragSlopeData.Zone3MachFactor.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Zone3MachFactor>" + Environment.NewLine;
            lRTN = lRTN + "<Zone3Slope value=\"" + TargetDragSlopeData.Zone3Slope.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Zone3Slope>" + Environment.NewLine;
            lRTN = lRTN + "<Zone3SlopeMultiplier value=\"" + TargetDragSlopeData.Zone3SlopeMultiplier.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Zone3SlopeMultiplier>" + Environment.NewLine;
            lRTN = lRTN + "<Zone4Slope value=\"" + TargetDragSlopeData.Zone4Slope.ToString() + "\" type=\"double\">" + Environment.NewLine;
            lRTN = lRTN + "</Zone4Slope>" + Environment.NewLine;
            lRTN = lRTN + "</DragSlopeData>" + Environment.NewLine;

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
        private Cartridge LoadCartridgeData(XmlNode CartridgeNode)
        {
            Cartridge lRTN = new Cartridge();
            string lValue;
            XmlNode lcnode;

                lcnode = CartridgeNode.SelectSingleNode("id");
                lValue = lcnode.Attributes["value"].Value;
                lRTN.ID = lValue;
                lcnode = CartridgeNode.SelectSingleNode("name");
                lValue = lcnode.Attributes["value"].Value;
                lRTN.Name = lValue;
                lcnode = CartridgeNode.SelectSingleNode("bulletdiameter");
                lValue = lcnode.Attributes["value"].Value;
                lRTN.BulletDiameter = Convert.ToDouble(lValue);
                lcnode = CartridgeNode.SelectSingleNode("trimlength");
                lValue = lcnode.Attributes["value"].Value;
                lRTN.CaseTrimLngth = Convert.ToDouble(lValue);
                lcnode = CartridgeNode.SelectSingleNode("maxcaselength");
                lValue = lcnode.Attributes["value"].Value;
                lRTN.MaxCaseLngth = Convert.ToDouble(lValue);
                lcnode = CartridgeNode.SelectSingleNode("maxcoal");
                lValue = lcnode.Attributes["value"].Value;
                lRTN.MaxCOAL = Convert.ToDouble(lValue);
                lcnode = CartridgeNode.SelectSingleNode("headspacemax");
                lValue = lcnode.Attributes["value"].Value;
                lRTN.HeadSpaceMax = Convert.ToDouble(lValue);
                lcnode = CartridgeNode.SelectSingleNode("headspacemin");
                lValue = lcnode.Attributes["value"].Value;
                lRTN.HeadSpaceMin = Convert.ToDouble(lValue);
                try
                {
                    lcnode = CartridgeNode.SelectSingleNode("cartridgepic");
                    lValue = lcnode.Attributes["value"].Value;
                    byte[] lba = LawlerBallisticsFactory.StringToByteArray(lValue);
                    Image lx = (Bitmap)((new ImageConverter()).ConvertFrom(lba));
                    lRTN.CartridgePic = lx;
                }
                catch
                {
                    // Image issue or no image
                }
                lcnode = CartridgeNode.SelectSingleNode("powders");
                XmlNode lpwdrid;
                lRTN.PowderIDlist = new List<string>();
                foreach (XmlNode ln in lcnode)
                {
                    lValue = "";
                    lpwdrid = ln.SelectSingleNode("pwdrid");
                    lValue = lpwdrid.Attributes["value"].Value;
                    lRTN.PowderIDlist.Add(lValue);
                }

            return lRTN;
        }
        private Gun LoadGunData(XmlNode GunNode)
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
                lbarrel = LoadBarrelData(lbn);
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
        private Barrel LoadBarrelData(XmlNode BarrelNode)
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
        private Bullet LoadBulletData(XmlNode BulletNode)
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
        private Recipe LoadRecipeData(XmlNode RecipeNode)
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
                    lRLC = LoadRecipeLotData(lLot);
                    lRTN.Lots.Add(lRLC);
                }
            }

            return lRTN;
        }
        private RecipeLot LoadRecipeLotData(XmlNode RecipeLotNode)
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
                    lRnd = LoadRoundData(lrd);
                    lRTN.Rounds.Add(lRnd);
                }
            }
            return lRTN;
        }
        private Round LoadRoundData(XmlNode RoundNode)
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
        private Case LoadCaseData(XmlNode CaseNode)
        {
            Case lRTN = new Case();
            string lNR;

            XmlNode lid = CaseNode.SelectSingleNode("id");
            lNR = lid.Attributes["value"].Value;
            lRTN.ID = lNR;
            XmlNode lmk = CaseNode.SelectSingleNode("cartridgeid");
            lNR = lmk.Attributes["value"].Value;
            lRTN.CartridgeID = lNR;
            lRTN.CartridgeName = LawlerBallisticsFactory.GetCartridgeName(lRTN.CartridgeID);
            XmlNode lmdl = CaseNode.SelectSingleNode("name");
            lNR = lmdl.Attributes["value"].Value;
            lRTN.Name = lNR;
            XmlNode lbulletsbto = CaseNode.SelectSingleNode("model");
            lNR = lbulletsbto.Attributes["value"].Value;
            lRTN.Model = lNR;
            XmlNode lbulletsoal = CaseNode.SelectSingleNode("manufacturer");
            lNR = lbulletsoal.Attributes["value"].Value;
            lRTN.Manufacturer = lNR;
            lNR = "";
            lbulletsoal = CaseNode.SelectSingleNode("primersize");
            if (lbulletsoal != null) lNR = lbulletsoal.Attributes["value"].Value;
            lRTN.PrimerSize = lNR;
            return lRTN;
        }
        private Primer LoadPrimerData(XmlNode PrimerNode)
        {
            Primer lRTN = new Primer();
            string lNR;

            XmlNode lid = PrimerNode.SelectSingleNode("id");
            lNR = lid.Attributes["value"].Value;
            lRTN.ID = lNR;
            XmlNode lname = PrimerNode.SelectSingleNode("name");
            lNR = lname.Attributes["value"].Value;
            lRTN.Name = lNR;
            XmlNode lbulletsbto = PrimerNode.SelectSingleNode("model");
            lNR = lbulletsbto.Attributes["value"].Value;
            lRTN.Model = lNR;
            XmlNode lbulletsoal = PrimerNode.SelectSingleNode("manufacturer");
            lNR = lbulletsoal.Attributes["value"].Value;
            lRTN.Manufacturer = lNR;
            XmlNode ltype = PrimerNode.SelectSingleNode("type");
            lNR = ltype.Attributes["value"].Value;
            lRTN.Type = lNR;

            return lRTN;
        }
        private Powder LoadPowderData(XmlNode PowderNode)
        {
            Powder lRTN = new Powder();
            string lNR;

            XmlNode lid = PowderNode.SelectSingleNode("id");
            lNR = lid.Attributes["value"].Value;
            lRTN.ID = lNR;
            XmlNode lname = PowderNode.SelectSingleNode("name");
            lNR = lname.Attributes["value"].Value;
            lRTN.Name = lNR;
            XmlNode lbulletsbto = PowderNode.SelectSingleNode("model");
            lNR = lbulletsbto.Attributes["value"].Value;
            lRTN.Model = lNR;
            XmlNode lbulletsoal = PowderNode.SelectSingleNode("manufacturer");
            lNR = lbulletsoal.Attributes["value"].Value;
            lRTN.Manufacturer = lNR;
            XmlNode ltype = PowderNode.SelectSingleNode("basetype");
            lNR = ltype.Attributes["value"].Value;
            lRTN.BaseType = lNR;

            return lRTN;
        }
        private LocationData LoadLocationData(XmlNode LocationNode)
        {
            LocationData lRTN = new LocationData();

            string lNR;
            XmlNode lid;

            lid = LocationNode.SelectSingleNode("Altitude");
            lNR = lid.Attributes["value"].Value;
            lRTN.Altitude = Convert.ToDouble(lNR);
            lid = LocationNode.SelectSingleNode("Latitude");
            lNR = lid.Attributes["value"].Value;
            lRTN.Latitude = Convert.ToDouble(lNR);
            lid = LocationNode.SelectSingleNode("Longitude");
            lNR = lid.Attributes["value"].Value;
            lRTN.Longitude = Convert.ToDouble(lNR);

            return lRTN;
        }
        private Atmospherics LoadAtmosphericData(XmlNode AtmosphericNode)
        {
            Atmospherics lRTN = new Atmospherics();

            string lNR;
            XmlNode lid;

            lid = AtmosphericNode.SelectSingleNode("DensityAlt");
            lNR = lid.Attributes["value"].Value;
            lRTN.DensityAlt = Convert.ToDouble(lNR);
            lid = AtmosphericNode.SelectSingleNode("FeelsLike");
            lNR = lid.Attributes["value"].Value;
            lRTN.FeelsLike = Convert.ToDouble(lNR);
            lRTN.FeelsLikeUnits = lid.Attributes["unit"].Value;
            lid = AtmosphericNode.SelectSingleNode("HumidityRel");
            lNR = lid.Attributes["value"].Value;
            lRTN.HumidityRel = Convert.ToDouble(lNR);
            lid = AtmosphericNode.SelectSingleNode("LastUpdated");
            lNR = lid.Attributes["value"].Value;
            lRTN.LastUpdated = Convert.ToDateTime(lNR);
            lid = AtmosphericNode.SelectSingleNode("Precipitation");
            lRTN.Precipitation = lid.Attributes["value"].Value;
            lid = AtmosphericNode.SelectSingleNode("Pressure");
            lNR = lid.Attributes["value"].Value;
            lRTN.Pressure = Convert.ToDouble(lNR);
            lRTN.PressureUnits = lid.Attributes["unit"].Value;
            lid = AtmosphericNode.SelectSingleNode("Station");
            lRTN.Station = lid.Attributes["value"].Value;
            lid = AtmosphericNode.SelectSingleNode("SunRise");
            lNR = lid.Attributes["value"].Value;
            lRTN.SunRise = Convert.ToDateTime(lNR);
            lid = AtmosphericNode.SelectSingleNode("SunSet");
            lNR = lid.Attributes["value"].Value;
            lRTN.SunSet = Convert.ToDateTime(lNR);
            lid = AtmosphericNode.SelectSingleNode("Temp");
            lNR = lid.Attributes["value"].Value;
            lRTN.Temp = Convert.ToDouble(lNR);
            lRTN.TempUnits = lid.Attributes["unit"].Value;
            lid = AtmosphericNode.SelectSingleNode("Weather");
            lRTN.Weather = lid.Attributes["value"].Value;
            lid = AtmosphericNode.SelectSingleNode("WindDirection");
            lNR = lid.Attributes["value"].Value;
            lRTN.WindDirection = Convert.ToDouble(lNR);
            lid = AtmosphericNode.SelectSingleNode("WindDirTxt");
            lRTN.WindDirTxt = lid.Attributes["value"].Value;
            lid = AtmosphericNode.SelectSingleNode("WindSpeed");
            lNR = lid.Attributes["value"].Value;
            lRTN.WindSpeed = Convert.ToDouble(lNR);
            lRTN.WindSpeedUnits = lid.Attributes["unit"].Value;

            return lRTN;
        }
        private ZeroData LoadZeroData(XmlNode ZeroDataNode)
        {
            ZeroData lRTN = new ZeroData();
            XmlNode lGN;
            string lNR;

            lGN = ZeroDataNode.SelectSingleNode("MidRange");
            lNR = lGN.Attributes["value"].Value;
            lRTN.MidRange = Convert.ToDouble(lNR);
            lGN = ZeroDataNode.SelectSingleNode("NearZeroRange");
            lNR = lGN.Attributes["value"].Value;
            lRTN.NearZeroRange = Convert.ToDouble(lNR);
            lGN = ZeroDataNode.SelectSingleNode("PointBlankRange");
            lNR = lGN.Attributes["value"].Value;
            lRTN.PointBlankRange = Convert.ToDouble(lNR);
            lGN = ZeroDataNode.SelectSingleNode("UseMaxRise");
            lNR = lGN.Attributes["value"].Value;
            lRTN.UseMaxRise = Convert.ToBoolean(lNR);
            lGN = ZeroDataNode.SelectSingleNode("ZeroMaxRise");
            lNR = lGN.Attributes["value"].Value;
            lRTN.ZeroMaxRise = Convert.ToDouble(lNR);
            lGN = ZeroDataNode.SelectSingleNode("ZeroRange");
            lNR = lGN.Attributes["value"].Value;
            lRTN.ZeroRange = Convert.ToDouble(lNR);
            lGN = ZeroDataNode.SelectSingleNode("ShotDirection");
            lNR = lGN.Attributes["value"].Value;
            lRTN.ShotDirection = Convert.ToDouble(lNR);
            lGN = ZeroDataNode.SelectSingleNode("ShotAngle");
            lNR = lGN.Attributes["value"].Value;
            lRTN.ShotAngle = Convert.ToDouble(lNR);
            lGN = ZeroDataNode.SelectSingleNode("ShotDistance");
            lNR = lGN.Attributes["value"].Value;
            lRTN.ShotDistance = Convert.ToDouble(lNR);
            lGN = ZeroDataNode.SelectSingleNode("WindEffectiveDirection");
            lNR = lGN.Attributes["value"].Value;
            lRTN.WindEffectiveDirection = Convert.ToDouble(lNR);
            lGN = ZeroDataNode.SelectSingleNode("MuzzleVelocity");
            lNR = lGN.Attributes["value"].Value;
            lRTN.MuzzleVelocity = Convert.ToDouble(lNR);
            XmlNode lZDsl = ZeroDataNode.SelectSingleNode("ShooterLocationData");
            XmlNode lLN = lZDsl.SelectSingleNode("LocationData");
            lRTN.ShooterLoc = LoadLocationData(lLN);
            XmlNode lZDtl = ZeroDataNode.SelectSingleNode("TargetLocationData");
            lLN = lZDtl.SelectSingleNode("LocationData");
            lRTN.TargetLoc = LoadLocationData(lLN);
            XmlNode lZDatm = ZeroDataNode.SelectSingleNode("AtmosphericData");
            lRTN.atmospherics = LoadAtmosphericData(lZDatm);            

            return lRTN;
        }
        private Solution LoadSolution(XmlNode SolutionNode)
        {
            Solution lRTN = new Solution();
            
            XmlNode lZD = SolutionNode.SelectSingleNode("ScenarioData");
            lRTN.MyScenario = LoadScenarioData(lZD);

            return lRTN;
        }
        private Scenario LoadScenarioData(XmlNode ScenarioNode)
        {
            Scenario lRTN = new Scenario();
            XmlNode lGN;

            lGN = ScenarioNode.SelectSingleNode("AtmosphericData");
            lRTN.MyAtmospherics = LoadAtmosphericData(lGN);
            lGN = ScenarioNode.SelectSingleNode("ShooterData");
            lRTN.MyShooter = LoadShooterData(lGN);

            return lRTN;
        }
        private DragSlopeData LoadDragSlopeData(XmlNode DragSlopeNode)
        {
            DragSlopeData lRTN = new DragSlopeData();
            XmlNode lGN;
            string lNR;

            lGN = DragSlopeNode.SelectSingleNode("BCg1");
            lNR = lGN.Attributes["value"].Value;
            lRTN.BCg1 = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("BCz2");
            lNR = lGN.Attributes["value"].Value;
            lRTN.BCz2 = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("D1");
            lNR = lGN.Attributes["value"].Value;
            lRTN.D1 = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("D2");
            lNR = lGN.Attributes["value"].Value;
            lRTN.D2 = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("F2");
            lNR = lGN.Attributes["value"].Value;
            lRTN.F2 = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("F3");
            lNR = lGN.Attributes["value"].Value;
            lRTN.F3 = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("F4");
            lNR = lGN.Attributes["value"].Value;
            lRTN.F4 = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Fo");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Fo = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("V1");
            lNR = lGN.Attributes["value"].Value;
            lRTN.V1 = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("V2");
            lNR = lGN.Attributes["value"].Value;
            lRTN.V2 = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Zone1AngleFactor");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Zone1AngleFactor = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Zone1MachFactor");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Zone1MachFactor = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Zone1Slope");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Zone1Slope = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Zone1SlopeMultiplier");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Zone1SlopeMultiplier = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Zone2MachFactor");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Zone2MachFactor = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Zone2Slope");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Zone2Slope = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Zone3MachFactor");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Zone3MachFactor = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Zone3Slope");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Zone3Slope = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Zone3SlopeMultiplier");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Zone3SlopeMultiplier = Convert.ToDouble(lNR);
            lGN = DragSlopeNode.SelectSingleNode("Zone4Slope");
            lNR = lGN.Attributes["value"].Value;
            lRTN.Zone4Slope = Convert.ToDouble(lNR);

            return lRTN;
        }
        private Shooter LoadShooterData(XmlNode ShooterNode)
        {
            Shooter lRTN = new Shooter();
            XmlNode lGN;

            lGN = ShooterNode.SelectSingleNode("LocationData");
            lRTN.MyLocation = LoadLocationData(lGN);
            lGN = ShooterNode.SelectSingleNode("LoadoutData");
            lRTN.MyLoadOut = LoadLoadoutData(lGN);

            return lRTN;
        }
        private LoadOut LoadLoadoutData(XmlNode LoadoutNode)
        {
            LoadOut lRTN = new LoadOut();
            XmlNode lGN;
            string lNR;

            lGN = LoadoutNode.SelectSingleNode("BSG");
            lNR = lGN.Attributes["value"].Value;
            lRTN.BSG = Convert.ToDouble(lNR);
            lGN = LoadoutNode.SelectSingleNode("ID");
            lNR = lGN.Attributes["value"].Value;
            lRTN.SetID(lNR);
            lGN = LoadoutNode.SelectSingleNode("MaxRange");
            lNR = lGN.Attributes["value"].Value;
            lRTN.MaxRange = Convert.ToDouble(lNR);
            lGN = LoadoutNode.SelectSingleNode("MuzzleVelocity");
            lNR = lGN.Attributes["value"].Value;
            lRTN.MuzzleVelocity = Convert.ToDouble(lNR);
            lGN = LoadoutNode.SelectSingleNode("SelectedBarrelID");
            lNR = lGN.Attributes["value"].Value;
            lRTN.SelectedBarrelID = lNR;
            lGN = LoadoutNode.SelectSingleNode("SelectedCartridge");
            XmlNode lSC = lGN.SelectSingleNode("recipe");
            lRTN.SelectedCartridge = LoadRecipeData(lSC);
            lGN = LoadoutNode.SelectSingleNode("gun");
            lRTN.SelectedGun = LoadGunData(lGN);
            lGN = LoadoutNode.SelectSingleNode("ZeroData");
            lRTN.zeroData = LoadZeroData(lGN);
            lGN = LoadoutNode.SelectSingleNode("DragSlopeData");
            lRTN.MyDragSlopeData = LoadDragSlopeData(lGN);

            return lRTN;
        }
        #endregion

        #endregion
    }
}
