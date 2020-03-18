using LawlerBallisticsDesk.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LawlerBallisticsDesk.Views.Cartridges
{
    /// <summary>
    /// Interaction logic for frmSAAMI_LoadDev.xaml
    /// </summary>
    public partial class frmSAAMI_LoadDev : Window
    {
        private string _SelectedCartridgeName;
        private string _SelectedCaseName;
        private string _SelectedPrimerName;
        private string _SelectedPowderName;
        private string _SelectedBulletName;
        private List<string> _CaseList;
        private double _BulletDia;
        private List<string> _BulletList;
        private List<string> _PrimerList;
        private List<string> _PowderList;

        public string SelectedCartridgeName{ get { return _SelectedCartridgeName; } set { _SelectedCartridgeName = value; } }
        public string SelectedCaseName { get { return _SelectedCaseName; } set { _SelectedCaseName = value; } }
        public string SelectedPrimerName { get { return _SelectedPrimerName; } set { _SelectedPrimerName = value; } }
        public string SelectedPowderName { get { return _SelectedPowderName; } set { _SelectedPowderName = value; } }
        public string SelectedBulletName { get { return _SelectedBulletName; } set { _SelectedBulletName = value; } }
        public List<string> CartridgeList { get { return LawlerBallisticsFactory.CartridgeNames; } }
        public List<string> CaseList { get { return _CaseList; } }
        public List<string> BulletList { get { return _BulletList; } }
        public List<string> PrimerList { get { return _PrimerList; } }
        public List<string> PowderList { get { return _PowderList; } }

        public frmSAAMI_LoadDev()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SelectedCartridgeName = null;
            this.Close();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string lCid="";
            _CaseList = new List<string>();
            _BulletList = new List<string>();
            foreach(Cartridge lcart in LawlerBallisticsFactory.MyCartridges)
            {
                if (lcart.Name == _SelectedCartridgeName)
                {
                    lCid = lcart.ID;
                    _BulletDia = lcart.BulletDiameter;
                    _PowderList = new List<string>();
                    if (lcart.PowderIDlist != null)
                    {
                        foreach (string lpid in lcart.PowderIDlist)
                        {
                            _PowderList.Add(LawlerBallisticsFactory.GetPowderName(lpid));
                        }
                    }
                    cboPowder.ItemsSource = PowderList;
                    cboPowder.IsEnabled = true;
                    break;
                }
            }
            foreach(Case lc in LawlerBallisticsFactory.MyCases)
            {
                if(lc.CartridgeID == lCid)
                {
                    _CaseList.Add(lc.Name);
                }
            }
            foreach(Bullet lb in LawlerBallisticsFactory.MyBullets)
            {
                if(lb.Diameter == _BulletDia)
                {
                    _BulletList.Add((lb.Manufacturer + "|" + lb.Model + "|" + lb.Weight));
                }
            }
            cboBullet.ItemsSource = BulletList;
            cboCase.ItemsSource = CaseList;
            cboCase.IsEnabled = true;
            cboBullet.IsEnabled = true;
        }
        private void cboCase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Case lCase=null;
            _PrimerList = new List<string>();
            foreach(Case lc in LawlerBallisticsFactory.MyCases)
            {
                if(_SelectedCaseName == lc.Name)
                {
                    lCase = lc;
                    break;
                }
            }
            if(lCase==null)
            {
                MessageBox.Show("The selected case cannot be located.", "Case not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            foreach(Primer lp in LawlerBallisticsFactory.MyPrimers)
            {
                if(lCase.PrimerSize == lp.Type)
                {
                    _PrimerList.Add(lp.Name);
                }
            }
            cboPrimer.ItemsSource = PrimerList;
            cboPrimer.IsEnabled = true;
        }
    }
}
