using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LawlerBallisticsDesk.Classes;
using LawlerBallisticsDesk.Views.Cartridges;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LawlerBallisticsDesk.ViewModel
{
    public class BarrelRecipeViewModel : ViewModelBase
    {
        #region "Messaging"
        private void ReceiveMessage(PropertyChangedMsg msg)
        {
            string lsender = msg.Sender;
            string lProp = msg.PropName;
            string lmsg = msg.Msg;
            switch (lProp)
            {
                case "VD":
                    LoadCharts();
                    break;
                case "HD":
                    LoadCharts();
                    break;
                case "ChartDataUpdate":
                    LoadCharts();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region "RelayCommands"
        private RelayCommand _AddLotCommand;
        private RelayCommand _AddBarrelRecipeCommand;
        private RelayCommand _LoadRecipeCommand;
        private RelayCommand _OpenRecipeCommand;
        private RelayCommand _OpenRecipeLotCommand;
        private RelayCommand _ResetChartCommand;
        private RelayCommand _SaveRecipeDataCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpLotCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpRoundCommand;
        
        public RelayCommand AddBarrelRecipeCommand
        {
            get
            {
                return _AddBarrelRecipeCommand ?? (_AddBarrelRecipeCommand = new RelayCommand(() => AddBarrelRecipe()));
            }
        }
        public RelayCommand LoadRecipeCommand
        {
            get
            {
                return _LoadRecipeCommand ?? (_LoadRecipeCommand = new RelayCommand(() => LoadRecipe()));
            }
        }
        public RelayCommand OpenRecipeCommand
        {
            get
            {
                return _OpenRecipeCommand ?? (_OpenRecipeCommand = new RelayCommand(() => OpenRecipe()));
            }
        }
        public RelayCommand OpenRecipeLotCommand
        {
            get
            {
                return _OpenRecipeLotCommand ?? (_OpenRecipeLotCommand = new RelayCommand(() => OpenRecipeLot()));
            }
        }
        public RelayCommand ResetChartCommand
        {
            get
            {
                return _ResetChartCommand ?? (_ResetChartCommand = new RelayCommand(() => ResetChart()));
            }
        }
        public RelayCommand SaveRecipeDataCommand
        {
            get
            {
                return _SaveRecipeDataCommand ?? (_SaveRecipeDataCommand = new RelayCommand(() => SaveRecipe()));
            }
        }
        public RelayCommand<System.Windows.Input.KeyEventArgs> KeyUpCommand
        {
            get
            {
                return _KeyUpCommand ?? (_KeyUpCommand = new RelayCommand<System.Windows.Input.KeyEventArgs>((X) => KeyUp(X)));
            }
        }
        public RelayCommand<System.Windows.Input.KeyEventArgs> KeyUpLotCommand
        {
            get
            {
                return _KeyUpLotCommand ?? (_KeyUpLotCommand = new RelayCommand<System.Windows.Input.KeyEventArgs>((X) => KeyUpLot(X)));
            }
        }
        public RelayCommand<System.Windows.Input.KeyEventArgs> KeyUpRoundCommand
        {
            get
            {
                return _KeyUpRoundCommand ?? (_KeyUpRoundCommand = new RelayCommand<System.Windows.Input.KeyEventArgs>((X) => KeyUpRound(X)));
            }
        }
        public RelayCommand AddLotCommand
        {
            get
            {
                return _AddLotCommand ?? (_AddLotCommand = new RelayCommand(() => AddLot()));
            }
        }
        #endregion

        #region "Private Variables"
        private Recipe _SelectedRecipe;
        private frmBarrelRecipe _frmLoadRecipe;
        private frmRecipeLot _frmRecipeLot;
        private PlotModel _PerformancePlot;
        private string _BarrelID;
        private ObservableCollection<Recipe> _BarrelRecipes;
        private Barrel _Barrel;
        #endregion

        #region "Properties"
        public string BarrelID
        {
            get { return _BarrelID; }
            set { _BarrelID = value; RaisePropertyChanged(nameof(BarrelID)); }
        }
        public Recipe SelectedRecipe { get { return _SelectedRecipe; } set { _SelectedRecipe = value; LoadCharts(); RaisePropertyChanged(nameof(SelectedRecipe)); } }
        public ObservableCollection<Recipe> BarrelRecipes
        {
            get
            {
                return _BarrelRecipes;
            } 
            set
            { _BarrelRecipes = value; RaisePropertyChanged(nameof(BarrelRecipes)); }
        }
        public ObservableCollection<Case> Cases { get { return LawlerBallisticsFactory.MyCases; } }
        public PlotModel PerformancePlot { get { return _PerformancePlot; } }
        #endregion

        #region "Constructor"
        public BarrelRecipeViewModel(string BarrelIDval)
        {
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<PropertyChangedMsg>(this, (action ) => ReceiveMessage(action ));
            Messenger.Default.Register<PropertyChangedMsg>(this, (Msg) => ReceiveMessage(Msg));
            BarrelID = BarrelIDval;
            _PerformancePlot = new PlotModel();
            _PerformancePlot.Title = "Group Data";
            _BarrelRecipes = LawlerBallisticsFactory.BarrelRecipes(BarrelID);
            _Barrel = LawlerBallisticsFactory.GetBarrel(BarrelID);
        }
        #endregion

        #region "Public Routines"
        public void SetLoadRecipe(string ID)
        {
            SelectedRecipe = LawlerBallisticsFactory.GetRecipe(ID);
        }
        #endregion

        #region "Private Routines"
        private void AddBarrelRecipe()
        {
            frmBarrelLoadDev lfrmSC = new frmBarrelLoadDev(_BarrelID);
            lfrmSC.ShowDialog();
            if (lfrmSC.SelectedCartridgeName == null) return;
            SelectedRecipe = new Recipe();
            SelectedRecipe.BarrelID = BarrelID;
            string lbullet = lfrmSC.SelectedBulletName;
            string lCaseName = lfrmSC.SelectedCaseName;
            string lPwdrNm = lfrmSC.SelectedPowderName;
            string lPrmrName = lfrmSC.SelectedPrimerName;
            double lCaseNeckClearance = lfrmSC.CaseNeckClearance;
            double lHeadSpaceClearance = lfrmSC.HeadSpaceClearance;
            lfrmSC = null;
            //TODO: check for null return on all class gets and exit if a null is returned.
            SelectedRecipe.RecpCartridge = _Barrel.ParentCartridge;
            SelectedRecipe.CartridgeID = SelectedRecipe.RecpCartridge.ID;
            SelectedRecipe.RecpBullet = LawlerBallisticsFactory.GetBulletFromInfo(lbullet);
            SelectedRecipe.BulletID = SelectedRecipe.RecpBullet.ID;
            SelectedRecipe.RecpCase = LawlerBallisticsFactory.GetCaseFromName(lCaseName);
            SelectedRecipe.CaseID = SelectedRecipe.RecpCase.ID;
            SelectedRecipe.CaseTrimLength = _Barrel.NeckDepth - lCaseNeckClearance;
            SelectedRecipe.HeadSpace = _Barrel.HeadSpace - lHeadSpaceClearance;
            SelectedRecipe.RecpPowder = LawlerBallisticsFactory.GetPowderFromName(lPwdrNm);
            SelectedRecipe.PowderID = SelectedRecipe.RecpPowder.ID;
            SelectedRecipe.RecpPrimer = LawlerBallisticsFactory.GetPrimerFromName(lPrmrName);
            SelectedRecipe.PrimerID = SelectedRecipe.RecpPrimer.ID;
            SelectedRecipe.Name = "LoadRecipe_" + (LawlerBallisticsFactory.MyRecipes.Count + 1).ToString();
            SelectedRecipe.BarrelID = BarrelID;
            _frmLoadRecipe = new frmBarrelRecipe(this);
            _frmLoadRecipe.Show();
        }
        private void OpenRecipe()
        {
            _frmLoadRecipe = new frmBarrelRecipe(this);
            _frmLoadRecipe.Show();
        }
        private void OpenRecipeLot()
        {
            _frmRecipeLot = new frmRecipeLot();
            _frmRecipeLot.Show();
            LoadCharts();
        }
        private void LoadRecipe()
        {
            foreach(Recipe lR in BarrelRecipes)
            {
                if(lR.ID == SelectedRecipe.ID)
                {
                    BarrelRecipes.Remove(lR);
                    BarrelRecipes.Add(SelectedRecipe);
                    RaisePropertyChanged(nameof(BarrelRecipes));
                    return;
                }
            }
            BarrelRecipes.Add(SelectedRecipe);
            RaisePropertyChanged(nameof(BarrelRecipes));
            _frmLoadRecipe.Close();
        }
        private void SaveRecipe()
        {
            LawlerBallisticsFactory.SaveBarrelRecipes(BarrelRecipes);

        }
        private void KeyUp(System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                        string lmsg = "Delete the selected recipe?";
                        string lcaption = "Delete Recipe Data";
                        MessageBoxButtons lbtns = MessageBoxButtons.YesNo;
                        DialogResult lrst = MessageBox.Show(lmsg, lcaption, lbtns, MessageBoxIcon.Warning);
                        if (lrst == DialogResult.Yes)
                        {
                            foreach (Recipe lc in BarrelRecipes)
                            {
                                if (SelectedRecipe.ID == lc.ID)
                                {
                                    BarrelRecipes.Remove(lc);
                                    SelectedRecipe = null;
                                    break;
                                }
                            }
                        }
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        AddBarrelRecipe();
                        break;
                }
            }
            catch
            {

            }
        }
        private void KeyUpLot(System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                        string lmsg = "Delete the selected lot?";
                        string lcaption = "Delete Recipe Lot Data";
                        MessageBoxButtons lbtns = MessageBoxButtons.YesNo;
                        DialogResult lrst = MessageBox.Show(lmsg, lcaption, lbtns, MessageBoxIcon.Warning);
                        if (lrst == DialogResult.Yes)
                        {
                            foreach (RecipeLot lc in SelectedRecipe.Lots)
                            {
                                if (SelectedRecipe.SelectedLot.ID == lc.ID)
                                {
                                    SelectedRecipe.Lots.Remove(lc);
                                    SelectedRecipe.SelectedLot = null;
                                    break;
                                }
                            }
                        }
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        AddLot();
                        break;
                }
            }
            catch
            {

            }
        }
        private void KeyUpRound(System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Delete:
                        string lmsg = "Delete the selected round?";
                        string lcaption = "Delete Round Data";
                        MessageBoxButtons lbtns = MessageBoxButtons.YesNo;
                        DialogResult lrst = MessageBox.Show(lmsg, lcaption, lbtns, MessageBoxIcon.Warning);
                        if (lrst == DialogResult.Yes)
                        {
                            foreach (Round lc in SelectedRecipe.SelectedLot.Rounds)
                            {
                                if (SelectedRecipe.SelectedLot.SelectedRound.ID == lc.ID)
                                {
                                    SelectedRecipe.SelectedLot.Rounds.Remove(lc);
                                    SelectedRecipe.SelectedLot.SelectedRound = null;
                                    break;
                                }
                            }
                        }
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        AddLot();
                        break;
                }
            }
            catch
            {

            }
        }
        private void AddLot()
        {
            Int32 lRndNo = 0;
            frmCreateCartridgeLot lfrmCreatLot = new frmCreateCartridgeLot();
            lfrmCreatLot.TotalLotSize = 0;
            lfrmCreatLot.ShowDialog();
            if(lfrmCreatLot.TotalLotSize == 0)
            {
                lfrmCreatLot.Close();
                lfrmCreatLot = null;
                return;
            }
            RecipeLot lCartLt = new RecipeLot();
            lCartLt.LotDate = DateTime.Now.ToString();
            lCartLt.SerialNo = SelectedRecipe.Lots.Count.ToString();
            for (Int32 cnt = lCartLt.SerialNo.Length; cnt < 10; cnt++)
            {
                lCartLt.SerialNo = "0" + lCartLt.SerialNo;
            }
            lCartLt.TotalCount = lfrmCreatLot.TotalLotSize;
            //TODO: once inventory control is added. Decrement inventory by lot size here.
            Round lRnd;
            for(Int32 I = 0; I < (lfrmCreatLot.SampleSize); I++)
            {
                lRnd = new Round();
                lRndNo++;
                lRnd.RndNo = lRndNo;
                lCartLt.Rounds.Add(lRnd);
            }
            SelectedRecipe.Lots.Add(lCartLt);
            lfrmCreatLot.Close();
            lfrmCreatLot = null;
            RaisePropertyChanged(nameof(SelectedRecipe));
        }
        #endregion

        public void LoadCharts()
        {
            if (SelectedRecipe == null) return;
            if (SelectedRecipe.SelectedLot == null) return;
            if (SelectedRecipe.SelectedLot.Rounds == null) return;
            if (SelectedRecipe.SelectedLot.Rounds.Count == 0) return;
            _PerformancePlot.Series.Clear();
            _PerformancePlot.Axes.Clear();
            _PerformancePlot.Annotations.Clear();
            double lHspread = Math.Max(Math.Abs(SelectedRecipe.SelectedLot.HDmax), Math.Abs(SelectedRecipe.SelectedLot.HDmin));
            double lVspread = Math.Max(Math.Abs(SelectedRecipe.SelectedLot.VDmax), Math.Abs(SelectedRecipe.SelectedLot.VDmin));
            double lSpread = Math.Max(lHspread, lVspread);
            ScatterSeries lPt = new ScatterSeries();
            LinearAxis lVert = new LinearAxis();
            LinearAxis lHoriz = new LinearAxis();
            lHoriz.Position = AxisPosition.Bottom;
            lHoriz.Minimum = SelectedRecipe.SelectedLot.HDavg - (lSpread * 2);
            lHoriz.Maximum = SelectedRecipe.SelectedLot.HDavg + (lSpread * 2);
            lHoriz.MajorGridlineColor = OxyColors.Black;
            lHoriz.MajorGridlineStyle = LineStyle.Dot;
            _PerformancePlot.Axes.Add(lHoriz);
            lVert.Key = "Iloc";
            lVert.Position = AxisPosition.Left;
            lVert.Minimum = SelectedRecipe.SelectedLot.VDavg - (lSpread * 2);
            lVert.Maximum = SelectedRecipe.SelectedLot.VDavg + (lSpread * 2);
            lVert.MajorGridlineColor = OxyColors.Black;
            lVert.MajorGridlineStyle = LineStyle.Dot;
            _PerformancePlot.Axes.Add(lVert);
            lPt.Title = "Impact Location";
            lPt.YAxisKey = "Iloc";
            ScatterPoint lSP;
            foreach (Round lR in SelectedRecipe.SelectedLot.Rounds)
            {
                lSP = new ScatterPoint(lR.HD, lR.VD);
                var pointAnnotation1 = new PointAnnotation();
                pointAnnotation1.X = lR.HD;
                pointAnnotation1.Y = lR.VD;
                pointAnnotation1.Text = lR.HD.ToString() + ", " + lR.VD.ToString();
                _PerformancePlot.Annotations.Add(pointAnnotation1);
                lPt.Points.Add(lSP);
            }
            _PerformancePlot.Series.Add(lPt);
            _PerformancePlot.InvalidatePlot(true);
        }
        public void ResetChart()
        {
            _PerformancePlot.ResetAllAxes();
            _PerformancePlot.InvalidatePlot(true);
        }
    }
}
