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
    public class RecipeViewModel : ViewModelBase
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
        private RelayCommand _AddSAMMIRecipeCommand;
        private RelayCommand _OpenRecipeCommand;
        private RelayCommand _OpenRecipeLotCommand;
        private RelayCommand _ResetChartCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpLotCommand;
        private RelayCommand<System.Windows.Input.KeyEventArgs> _KeyUpRoundCommand;

        public RelayCommand AddSAMMIRecipeCommand
        {
            get
            {
                return _AddSAMMIRecipeCommand ?? (_AddSAMMIRecipeCommand = new RelayCommand(() => AddSAMMIRecipe()));
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
        public RelayCommand LoadRecipeCommand { get; set; }
        public RelayCommand SaveRecipeDataCommand { get; set; }
        public RelayCommand SaveRecipeCommand { get; set; }
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
        private frmRecipe _frmLoadRecipe;
        private frmRecipeLot _frmRecipeLot;
        private PlotModel _PerformancePlot;
        #endregion

        #region "Properties"
        public Recipe SelectedRecipe { get { return _SelectedRecipe; } set { _SelectedRecipe = value; LoadCharts(); RaisePropertyChanged(nameof(SelectedRecipe)); } }
        public ObservableCollection<Recipe> MyRecipes { get {return LawlerBallisticsFactory.MyRecipes; } 
            set { LawlerBallisticsFactory.MyRecipes = value; RaisePropertyChanged(nameof(MyRecipes)); } }
        //public ObservableCollection<Case> Cases { get { return LawlerBallisticsFactory.MyCases; } }
        public PlotModel PerformancePlot { get { return _PerformancePlot; } }
        #endregion

        #region "Constructor"
        public RecipeViewModel()
        {
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<PropertyChangedMsg>(this, (action ) => ReceiveMessage(action ));
            Messenger.Default.Register<PropertyChangedMsg>(this, (Msg) => ReceiveMessage(Msg));
            _PerformancePlot = new PlotModel();
            _PerformancePlot.Title = "Group Data";
            LoadRecipeCommand = new RelayCommand(LoadThisRecipe, null);
            SaveRecipeDataCommand = new RelayCommand(SaveRecipeCollection, null);
            SaveRecipeCommand = new RelayCommand(SaveRecipe, null);
        }
        #endregion

        #region "Public Routines"
        public void SetLoadRecipe(string ID)
        {
            SelectedRecipe = LawlerBallisticsFactory.GetRecipe(ID);
        }
        #endregion

        #region "Private Routines"
        private void OpenRecipe()
        {
            _frmLoadRecipe = new frmRecipe(false,false);
            _frmLoadRecipe.Show();
        }
        private void OpenRecipeLot()
        {
            _frmRecipeLot = new frmRecipeLot();
            _frmRecipeLot.Show();
            LoadCharts();
        }
        private void AddSAMMIRecipe()
        {
            frmSAAMI_LoadDev lfrmSC = new frmSAAMI_LoadDev();
            lfrmSC.ShowDialog();
            SelectedRecipe = new Recipe();
            if (lfrmSC.SelectedCartridgeName == null) return;
            string lcn = lfrmSC.SelectedCartridgeName;
            string lbullet = lfrmSC.SelectedBulletName;
            string lCaseName = lfrmSC.SelectedCaseName;
            string lPwdrNm = lfrmSC.SelectedPowderName;
            string lPrmrName = lfrmSC.SelectedPrimerName;
            lfrmSC = null;
            //TODO: check for null return on all class gets and exit if a null is returned.
            SelectedRecipe.RecpCartridge = LawlerBallisticsFactory.GetCartridgeFromName(lcn);
            SelectedRecipe.CartridgeID = SelectedRecipe.RecpCartridge.ID;
            SelectedRecipe.RecpBullet = LawlerBallisticsFactory.GetBulletFromInfo(lbullet);
            SelectedRecipe.BulletID = SelectedRecipe.RecpBullet.ID;
            SelectedRecipe.RecpCase = LawlerBallisticsFactory.GetCaseFromName(lCaseName);
            SelectedRecipe.CaseID = SelectedRecipe.RecpCase.ID;
            SelectedRecipe.CaseTrimLength = SelectedRecipe.RecpCartridge.CaseTrimLngth;
            SelectedRecipe.HeadSpace = ((SelectedRecipe.RecpCartridge.HeadSpaceMax - 
                SelectedRecipe.RecpCartridge.HeadSpaceMin)/2)+ SelectedRecipe.RecpCartridge.HeadSpaceMin;
            SelectedRecipe.COAL = SelectedRecipe.RecpCartridge.MaxCOAL;
            SelectedRecipe.RecpPowder = LawlerBallisticsFactory.GetPowderFromName(lPwdrNm);
            SelectedRecipe.PowderID = SelectedRecipe.RecpPowder.ID;
            SelectedRecipe.RecpPrimer = LawlerBallisticsFactory.GetPrimerFromName(lPrmrName);
            SelectedRecipe.PrimerID = SelectedRecipe.RecpPrimer.ID;
            SelectedRecipe.Name = "LoadRecipe_" + (LawlerBallisticsFactory.MyRecipes.Count + 1).ToString();
            _frmLoadRecipe = new frmRecipe(true, false);
            _frmLoadRecipe.Show();
        }
        private void LoadThisRecipe()
        {

        }
        private void SaveRecipeCollection()
        {
            LawlerBallisticsFactory.SaveMyRecipes();
        }
        private void SaveRecipe()
        {
            Cartridge lSC;
            foreach (Recipe lItr in MyRecipes)
            {
                if (lItr.ID == _SelectedRecipe.ID)
                {
                    _frmLoadRecipe.Close();
                    RaisePropertyChanged(nameof(MyRecipes));
                    RaisePropertyChanged(nameof(SelectedRecipe));
                    return;
                }
            }
            MyRecipes.Add(_SelectedRecipe);
            _frmLoadRecipe.Close();
            RaisePropertyChanged(nameof(MyRecipes));
            RaisePropertyChanged(nameof(SelectedRecipe));
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
                            foreach (Recipe lc in MyRecipes)
                            {
                                if (SelectedRecipe.ID == lc.ID)
                                {
                                    MyRecipes.Remove(lc);
                                    SelectedRecipe = null;
                                    break;
                                }
                            }
                        }
                        break;
                    case System.Windows.Input.Key.OemPlus:
                        AddSAMMIRecipe();
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
        #endregion
    }
}
