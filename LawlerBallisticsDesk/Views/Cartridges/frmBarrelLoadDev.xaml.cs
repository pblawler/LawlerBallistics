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
    public partial class frmBarrelLoadDev : Window
    {
        #region "Private Variables"
        private string _SelectedCartridgeName;
        private string _SelectedCaseName;
        private string _SelectedPrimerName;
        private string _SelectedPowderName;
        private string _SelectedBulletName;
        private double _CaseNeckClearance;
        private double _HeadSpaceClearance;
        private List<string> _CaseList;
        private Barrel _Barrel;
        private List<string> _BulletList;
        private List<string> _PrimerList;
        private List<string> _PowderList;
        private List<double> _NeckClearanceList = new List<double>{0.005, 0.010, 0.015};
        private List<double> _HeadspaceClearanceList = new List<double> { 0.000, 0.001, 0.0015,0.002, 0.0025, 0.003, 0.0035,0.004,0.0045,0.005};
        #endregion

        #region "Properties"
        public double CaseNeckClearance { get { return _CaseNeckClearance; } set { _CaseNeckClearance = value; } }
        public List<double> NeckClearanceList { get {return _NeckClearanceList; } }
        public double HeadSpaceClearance { get { return _HeadSpaceClearance; } set { _HeadSpaceClearance = value; } }
        public List<double> HeadspaceClearanceList { get { return _HeadspaceClearanceList; } }
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
        #endregion

        #region "Constructor"
        public frmBarrelLoadDev(string BarrelID)
        {
            InitializeComponent();
            _Barrel = LawlerBallisticsFactory.GetBarrel(BarrelID);
            _CaseList = LawlerBallisticsFactory.GetCaseList(_Barrel.CartridgeID);
            _PowderList = LawlerBallisticsFactory.GetCartridgePowderList(_Barrel.CartridgeID);
            _BulletList = LawlerBallisticsFactory.GetCartridgeBulletList(_Barrel.CartridgeID);
            _CaseNeckClearance = 0.005;
            cboNeckClearance.Text = "0.005";
            cboHeadClearance.Text = "0.0015";
            cboCase.ItemsSource = CaseList;
            cboPowder.ItemsSource = PowderList;
            cboBullet.ItemsSource = BulletList;
        }
        #endregion

        #region "Events"
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
        }
        private void cboCase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string lCaseID = LawlerBallisticsFactory.GetCaseID(cboCase.SelectedItem.ToString());
            _PrimerList = LawlerBallisticsFactory.GetPrimerList(lCaseID);
            cboPrimer.IsEnabled = true;
            cboPrimer.ItemsSource = PrimerList;
        }
        #endregion
    }
}
