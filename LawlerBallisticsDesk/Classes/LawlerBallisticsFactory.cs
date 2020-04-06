using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LawlerBallisticsDesk.Classes
{
    #region "Enums"
    public enum BulletShapeEnum : short
    {
        VLD_BoatTail = -1,
        SpitzerBoatTail = 1,
        SpitzerFlatBase = 2,
        SemiSpitzer = 3,
        RoundNoseOrFlatNose = 4
    }
    public enum PrimerType : short
    {
        Large_Rifle = 1,
        Small_Rifle = 2,
        Small_Pistol = 3,
        Large_Pistol = 4
    }
    public enum PowderBaseType : short
    {
        ///http://www.sciencemadness.org/smwiki/index.php/Smokeless_powder
        ///
        SingleBase = 1,
        DoubleBase = 2,
        TripleBase = 3
    }
    public enum PowderGeometry : short
    {
        Ball = 1,
        Flake = 2,
        Cylinder = 3
    }
    public enum TwistDirection : short
    {
        Right = 1,
        Left = 2
    }
    #endregion

    public static class LawlerBallisticsFactory 
    {
        //TODO: Implement Docking
        // https://github.com/Dirkster99/AvalonDock

        //TODO: Verify Name uniqueness for all classes where the name is entered prior to saving.  Perferably on keyup.

        #region "Private Variables"
        private static ObservableCollection<Bullet> _MyBullets = new ObservableCollection<Bullet>();
        private static ObservableCollection<Cartridge> _MyCartridges = new ObservableCollection<Cartridge>();
        private static ObservableCollection<Gun> _MyGuns = new ObservableCollection<Gun>();
        private static ObservableCollection<Recipe> _MyRecipes = new ObservableCollection<Recipe>();
        private static ObservableCollection<Case> _MyCases = new ObservableCollection<Case>();
        private static ObservableCollection<Primer> _MyPrimers = new ObservableCollection<Primer>();
        private static ObservableCollection<Powder> _MyPowders = new ObservableCollection<Powder>();
        private static DataPersistence _MyData;
        private static BulletShapeEnum _BulletShapeType = new BulletShapeEnum();
        private static string _AppDataFolder;
        private static string _DataFolder;
        private static string _WeatherFolder;
        #endregion

        #region "Properties"
        public static bool Initialized;
        public static string AppDataFolder
        {
            get { return _AppDataFolder; }
        }
        public static string DataFolder
        {
            get { return _DataFolder; }
        }
        public static string WeatherFolder
        {
            get { return _WeatherFolder; }
        }
        public static ObservableCollection<Bullet> MyBullets { get { return _MyBullets; } set { _MyBullets = value; } }
        public static ObservableCollection<Cartridge> MyCartridges { get { return _MyCartridges; } set { _MyCartridges = value; } }
        public static ObservableCollection<Gun> MyGuns { get { return _MyGuns; } set { _MyGuns = value; } }
        public static ObservableCollection<Recipe> MyRecipes { get { return _MyRecipes; } set { _MyRecipes = value; } }
        public static ObservableCollection<string> CartridgeList
        {
            get
            {
                ObservableCollection<string> lrtn = new ObservableCollection<string>();
                foreach(Cartridge lC in MyCartridges)
                {
                    lrtn.Add(lC.Name);
                }
                return lrtn;
            }
        }
        public static ObservableCollection<Case> MyCases { get { return _MyCases; } set { _MyCases = value; } }
        public static ObservableCollection<Primer> MyPrimers { get { return _MyPrimers; } set { _MyPrimers = value; } }
        public static ObservableCollection<Powder> MyPowders { get { return _MyPowders; } set { _MyPowders = value; } }
        /// <summary>
        /// Used to estimate a bullet's BC and for initial Zone scale multiplier value selection.
        /// </summary>
        public static BulletShapeEnum MyBulletTypes
        {
            get { return _BulletShapeType; }
        }
        public static List<string> PowderNameList
        {
            get
            {
                List<string> lPL = new List<string>();
                if (MyPowders != null)
                {
                    foreach (Powder lp in MyPowders)
                    {
                        lPL.Add(lp.Name);
                    }
                }
                return lPL;
            }
        }
        public static List<string> CartridgeNames
        {
           get
            {
                List<string> lNL = new List<string>();

                foreach (Cartridge lc in MyCartridges)
                {
                    lNL.Add(lc.Name);
                }
                return lNL;
            }
        }      
        public static List<string> BulletTypeNames
        {
            get
            {
                List<string> lBTN = new List<string>();
                string[] values = Enum.GetNames(typeof(BulletShapeEnum));
                foreach (string B in values)
                {
                    lBTN.Add(B.ToString());
                }
                return lBTN;
            }
        }
        public static List<string> PrimerTypeNames
        {
            get
            {
                List<string> lBTN = new List<string>();
                string[] values = Enum.GetNames(typeof(PrimerType));
                foreach (string B in values)
                {
                    lBTN.Add(B.ToString());
                }
                return lBTN;
            }
        }
        public static List<string> PowderBaseTypeNames
        {
            get
            {
                List<string> lBTN = new List<string>();
                string[] values = Enum.GetNames(typeof(PowderBaseType));
                foreach (string B in values)
                {
                    lBTN.Add(B.ToString());
                }
                return lBTN;
            }
        }
        public static List<string> TwistDirectionNames
        {
            get
            {
                List<string> lBTN = new List<string>();
                string[] values = Enum.GetNames(typeof(TwistDirection));
                foreach (string B in values)
                {
                    lBTN.Add(B.ToString());
                }
                return lBTN;
            }
        }
        #endregion

        #region "Quazi Constructor, needs to be called at program start."
        public static void InitializeFactory()
        {
            if (Initialized) return;

            _AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _AppDataFolder = _AppDataFolder + "\\LawlerBallistics";
            if (!Directory.Exists(_AppDataFolder))
            {
                Directory.CreateDirectory(_AppDataFolder);
            }
            _DataFolder = _AppDataFolder + "\\Data";
            if (!Directory.Exists(_DataFolder))
            {
                Directory.CreateDirectory(_DataFolder);
            }
            _WeatherFolder = _DataFolder + "\\Weather";
            if (!Directory.Exists(_WeatherFolder))
            {
                Directory.CreateDirectory(_WeatherFolder);
            }

            _MyData = new DataPersistence();
            _MyData.CheckDataFiles();
            _MyCartridges = _MyData.ParseCartridgeDB();
            _MyGuns = _MyData.ParseGunDB();
            _MyBullets = _MyData.ParseBulletsDB();
            _MyCases = _MyData.ParseCaseDB();
            _MyPrimers = _MyData.ParsePrimerDB();
            _MyPowders = _MyData.ParsePowderDB();
            _MyRecipes = _MyData.ParseRecipeDB();
        }
        #endregion

        #region "Public Routines"

        #region "General Utility Routines"
        public static byte[] ImageToBytes(Image TargetImage)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(TargetImage, typeof(byte[]));
            return xByte;
        }
        public static byte[] StringToByteArray(string HexaDecimalString)
        {
            String[] arr = HexaDecimalString.Split('-');
            byte[] array = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++) array[i] = Convert.ToByte(arr[i], 16);
            return array;
        }
        /// <summary>
        /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
        /// </summary>
        /// <param name="src">A bitmap image</param>
        /// <returns>The image as a BitmapImage for WPF</returns>
        public static BitmapImage ConvertToBitmap(Image src)
        {
            BitmapImage image = new BitmapImage();

            try
            {
                MemoryStream ms = new MemoryStream();
                ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                image.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();
            }
            catch
            {
                image = null;
            }
            return image;
        }
        #endregion

        #region "Statistics"
        public static double StandardDeviation(double[] Values)
        {
            double lRTN = 0, lTot = 0, lAvg = 0, lSumSq=0;
            Int32 lCnt = Values.Length;
            try
            {
                for (Int32 I = 0; I <= (lCnt-1); I++)
                {
                    lTot = lTot + Values[I];
                }
                lAvg = lTot / lCnt;
                for (Int32 I = 0; I <= (lCnt-1); I++)
                {
                    lSumSq = lSumSq + Math.Pow((Values[I] - lAvg),2);
                }
                lRTN = Math.Pow((lSumSq / lCnt), 0.5);
            }
            catch
            {
                lRTN = 0;
            }
            return lRTN;
        }
        public static double SampleStandardDeviation(double[] Values)
        {
            double lRTN = 0, lTot = 0, lAvg = 0, lSumSq = 0;
            Int32 lCnt = Values.Length;
            try
            {
                for (Int32 I = 0; I <= (lCnt - 1); I++)
                {
                    lTot = lTot + Values[I];
                }
                lAvg = lTot / lCnt;
                for (Int32 I = 0; I <= (lCnt - 1); I++)
                {
                    lSumSq = lSumSq + Math.Pow((Values[I] - lAvg), 2);
                }
                lRTN = Math.Pow((lSumSq / (lCnt-1)), 0.5);
            }
            catch
            {
                lRTN = 0;
            }
            return lRTN;
        }

        #endregion

        #region "Class Routines"

        #region "Gun"
        public static Gun GetGun(string ID)
        {
            Gun lRTN = null;

            foreach(Gun lg in MyGuns)
            {
                if(lg.ID == ID)
                {
                    lRTN = lg;
                    break;
                }
            }
            return lRTN;
        }
        public static void SaveMyGuns()
        {
            DataPersistence lDataPersistence = new DataPersistence();
            lDataPersistence.SaveGunDB();
        }
        #endregion

        #region "Barrel"
        public static ObservableCollection<Recipe> BarrelRecipes(string BarrelID)
        {
            ObservableCollection<Recipe> lRTN = new ObservableCollection<Recipe>();

            foreach (Recipe lR in MyRecipes)
            {
                if (lR.BarrelID != null)
                {
                    if (lR.BarrelID == BarrelID)
                    {
                        lRTN.Add(lR);
                    }
                }
            }
            return lRTN;
        }
        public static Barrel GetBarrel(string BarrelID)
        {
            Barrel lRTN = new Barrel();
            foreach (Gun lg in MyGuns)
            {
                foreach (Barrel lb in lg.Barrels)
                {
                    if (lb.ID == BarrelID)
                    {
                        lRTN = lb;
                        return lRTN;
                    }
                }
            }
            return lRTN;
        }

        #endregion

        #region "Bullet"
        public static Bullet GetBullet(string ID)
        {
            Bullet lblt = new Bullet();

            try
            {
                foreach (Bullet lb in MyBullets)
                {
                    if (lb.ID == ID)
                    {
                        lblt = lb;
                        break;
                    }
                }
            }
            catch
            {
                lblt = null;
            }
            return lblt;
        }
        public static Bullet GetBulletFromInfo(string BulletInfo)
        {
            Bullet lblt = new Bullet();
            string lmfr, lmdl;
            double lwt;

            try
            {
                string[] lbltinfo = BulletInfo.Split('|');
                lmfr = lbltinfo[0];
                lmdl = lbltinfo[1];
                lwt = Convert.ToDouble(lbltinfo[2]);
                foreach (Bullet lb in MyBullets)
                {
                    if ((lb.Manufacturer == lmfr) & (lb.Model == lmdl) & (lb.Weight == lwt))
                    {
                        lblt = lb;
                        break;
                    }
                }
            }
            catch
            {
                lblt = null;
            }
            return lblt;
        }
        public static void SaveMyBullets()
        {
            DataPersistence lDataPersistence = new DataPersistence();
            lDataPersistence.SaveBulletDB();
        }
        #endregion

        #region "Cartridge"
        public static Cartridge GetCartridge(string ID)
        {

            foreach(Cartridge lC in MyCartridges)
            {
                if(lC.ID == ID)
                {
                    return lC;
                }
            }
            return null;
        }
        public static Cartridge GetCartridgeFromName(string Name)
        {

            foreach (Cartridge lC in MyCartridges)
            {
                if (lC.Name == Name)
                {
                    return lC;
                }
            }
            return null;
        }
        public static string GetCartridgeName(string ID)
        {
            string lRTN = "";

            foreach (Cartridge lC in MyCartridges)
            {
                if (ID == lC.ID)
                {
                    lRTN = lC.Name;
                    break;
                }
            }
            return lRTN;
        }
        public static string GetCartridgeID(string Name)
        {
            string lID = "";
            foreach(Cartridge lc in MyCartridges)
            {
                if(lc.Name == Name)
                {
                    lID = lc.ID;
                    break;
                }
            }
            return lID;
        }
        public static List<string> GetCartridgeBulletList(string CartridgeID)
        {
            Powder lp;
            string lBN;
            List<string> lRTN = new List<string>();
            Cartridge lc = GetCartridge(CartridgeID);

            foreach (Bullet lb in MyBullets)
            {
                lBN = "";
                if (lb.Diameter == lc.BulletDiameter)
                {
                    lBN = lb.Manufacturer + "|" + lb.Model + "|" + lb.Weight.ToString();
                    lRTN.Add(lBN);
                }
            }

            return lRTN;
        }
        public static List<string> GetCartridgePowderList(string CartridgeID)
        {
            Powder lp;
            List<string> lRTN = new List<string>();
            Cartridge lc = GetCartridge(CartridgeID);
            
            foreach(string lpid in lc.PowderIDlist)
            {
                lp = GetPowder(lpid);
                lRTN.Add(lp.Name);
            }

            return lRTN;
        }
        public static void SaveMyCartridges()
        {
            DataPersistence lDataPersistence = new DataPersistence();
            lDataPersistence.SaveCartridgeData();
        }
        #endregion

        #region "Case"
        public static Case GetCase(string ID)
        {
            Case lRTN = null;

            try
            {
                foreach(Case lc in MyCases)
                {
                    if(lc.ID == ID)
                    {
                        lRTN = lc;
                        break;
                    }
                }
            }
            catch
            {
                lRTN = null;
            }

            return lRTN;
        }
        public static Case GetCaseFromName(string Name)
        {
            Case lRTN = null;

            try
            {
                foreach (Case lc in MyCases)
                {
                    if (lc.Name == Name)
                    {
                        lRTN = lc;
                        break;
                    }
                }
            }
            catch
            {
                lRTN = null;
            }

            return lRTN;
        }
        public static string GetCaseID(string Name)
        {
            string lRtn = "";
            Case lc = GetCaseFromName(Name);
            lRtn = lc.ID;            

            return lRtn;
        }
        public static List<string> GetCaseList(string CartridgeID)
        {
            List<string> lRTN = new List<string>();
            foreach(Case lc in MyCases)
            {
                if(lc.CartridgeID == CartridgeID)
                {
                    lRTN.Add(lc.Name);
                }
            }

            return lRTN;
        }
        public static void SaveMyCases()
        {
            DataPersistence lDataPersistence = new DataPersistence();
            lDataPersistence.SaveCaseDB();
        }
        #endregion

        #region "Powder"
        public static string GetPowderName(string PowderID)
        {
            foreach(Powder lp in MyPowders)
            {
                if(lp.ID == PowderID)
                {
                    return lp.Name;
                }
            }
            return "";
        }
        public static string GetPowderID(string PowderName)
        {
            foreach (Powder lp in MyPowders)
            {
                if (lp.Name == PowderName)
                {
                    return lp.ID;
                }
            }
            return "";
        }
        public static Powder GetPowder(string ID)
        {
            Powder lRTN = null;
            
            foreach(Powder lp in MyPowders)
            {
                if(lp.ID == ID)
                {
                    lRTN = lp;
                    break;
                }
            }

            return lRTN;
        }
        public static Powder GetPowderFromName(string PowderName)
        {
            Powder lRTN = null;

            foreach (Powder lp in MyPowders)
            {
                if (lp.Name == PowderName)
                {
                    lRTN = lp;
                    break;
                }
            }

            return lRTN;
        }
        public static void SaveMyPowders()
        {
            DataPersistence lDataPersistence = new DataPersistence();
            lDataPersistence.SavePowderDB();
        }
        #endregion

        #region "Primer"
        public static Primer GetPrimer(string ID)
        {
            Primer lRTN = null;

            foreach(Primer lp in MyPrimers)
            {
                if(lp.ID == ID)
                {
                    lRTN = lp;
                    break;
                }
            }
            return lRTN;
        }
        public static Primer GetPrimerFromName(string Name)
        {
            Primer lRTN = null;

            foreach (Primer lp in MyPrimers)
            {
                if (lp.Name == Name)
                {
                    lRTN = lp;
                    break;
                }
            }
            return lRTN;
        }
        public static List<string> GetPrimerList(string CaseID)
        {
            List<string> lRTN = new List<string>();
            Case lc = GetCase(CaseID);
            string lPrimerTyp = lc.PrimerSize;
            
            foreach(Primer lp in MyPrimers)
            {
                if(lp.Type == lPrimerTyp)
                {
                    lRTN.Add(lp.Name);
                }
            }

            return lRTN;
        }
        public static void SaveMyPrimers()
        {
            DataPersistence lDataPersistence = new DataPersistence();
            lDataPersistence.SavePrimerDB();
        }
        #endregion

        #region "Recipes"
        public static Recipe GetRecipe(string ID)
        {
            Recipe lLR = new Recipe();

            foreach (Recipe ltLR in MyRecipes)
            {
                if (ltLR.ID == ID)
                {
                    return ltLR;
                    break;
                }
            }

            return lLR;
        }
        /// <summary>
        /// Saves the barrel specific recipes to the global recipe collection then persists to file.
        /// </summary>
        /// <param name="BarrelRecipes"></param>
        public static void SaveBarrelRecipes(ObservableCollection<Recipe> BarrelRecipes)
        {
            bool lfnd = false;

            foreach(Recipe lbr in BarrelRecipes)
            {
                lfnd = false;
                foreach(Recipe lmr in MyRecipes)
                {
                    if(lbr.ID == lmr.ID)
                    {
                        MyRecipes.Remove(lmr);
                        MyRecipes.Add(lbr);
                        lfnd = true;
                        break;
                    }
                }
                if (!lfnd) MyRecipes.Add(lbr);
            }
            SaveMyRecipes();
        }
        public static void SaveMyRecipes()
        {
            DataPersistence lDataPersistence = new DataPersistence();
            lDataPersistence.SaveRecipeDB();
        }
        #endregion

        #endregion

        #endregion

        #region "Private Routines"

        #endregion
    }
}
