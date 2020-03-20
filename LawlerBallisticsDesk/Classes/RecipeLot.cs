using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class RecipeLot : INotifyPropertyChanged
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

        #region "Messaging"
        private void SendPropertyChangedMsg(string name)
        {
            var msg = new PropertyChangedMsg() { Sender = "RecipeLot", PropName = name, Msg = "" };
            Messenger.Default.Send<PropertyChangedMsg>(msg);
        }

        private void ReceiveMessage(PropertyChangedMsg msg)
        {
            string lsender = msg.Sender;
            string lProp = msg.PropName;
            string lmsg = msg.Msg;
            bool lRndData = false;
            switch (lProp)
            {
                case "BBTO":
                    BBTOstat();
                    lRndData = true;
                    foreach(Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("BBTO", BBTOes);
                                break;
                            case "AVG":
                                lr.SilentSet("BBTO", BBTOavg);
                                break;
                            case "MAX":
                                lr.SilentSet("BBTO", BBTOmax);
                                break;
                            case "MIN":
                                lr.SilentSet("BBTO", BBTOmin);
                                break;
                            case "SD":
                                lr.SilentSet("BBTO", BBTOsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "BL":
                    BLstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("BL", BLes);
                                break;
                            case "AVG":
                                lr.SilentSet("BL", BLavg);
                                break;
                            case "MAX":
                                lr.SilentSet("BL", BLmax);
                                break;
                            case "MIN":
                                lr.SilentSet("BL", BLmin);
                                break;
                            case "SD":
                                lr.SilentSet("BL", BLsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "BW":
                    BWstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("BW", BWes);
                                break;
                            case "AVG":
                                lr.SilentSet("BW", BWavg);
                                break;
                            case "MAX":
                                lr.SilentSet("BW", BWmax);
                                break;
                            case "MIN":
                                lr.SilentSet("BW", BWmin);
                                break;
                            case "SD":
                                lr.SilentSet("BW", BWsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "BD":
                    BDstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("BD", BDes);
                                break;
                            case "AVG":
                                lr.SilentSet("BD", BDavg);
                                break;
                            case "MAX":
                                lr.SilentSet("BD", BDmax);
                                break;
                            case "MIN":
                                lr.SilentSet("BD", BDmin);
                                break;
                            case "SD":
                                lr.SilentSet("BD", BDsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "CW":
                    CWstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("CW", CWes);
                                break;
                            case "AVG":
                                lr.SilentSet("CW", CWavg);
                                break;
                            case "MAX":
                                lr.SilentSet("CW", CWmax);
                                break;
                            case "MIN":
                                lr.SilentSet("CW", CWmin);
                                break;
                            case "SD":
                                lr.SilentSet("CW", CWsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "CVW":
                    CVWstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("CVW", CVWes);
                                break;
                            case "AVG":
                                lr.SilentSet("CVW", CVWavg);
                                break;
                            case "MAX":
                                lr.SilentSet("CVW", CVWmax);
                                break;
                            case "MIN":
                                lr.SilentSet("CVW", CVWmin);
                                break;
                            case "SD":
                                lr.SilentSet("CVW", CVWsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "CL":
                    CLstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("CL", CLes);
                                break;
                            case "AVG":
                                lr.SilentSet("CL", CLavg);
                                break;
                            case "MAX":
                                lr.SilentSet("CL", CLmax);
                                break;
                            case "MIN":
                                lr.SilentSet("CL", CLmin);
                                break;
                            case "SD":
                                lr.SilentSet("CL", CLsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "CHS":
                    CHSstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("CHS", CHSes);
                                break;
                            case "AVG":
                                lr.SilentSet("CHS", CHSavg);
                                break;
                            case "MAX":
                                lr.SilentSet("CHS", CHSmax);
                                break;
                            case "MIN":
                                lr.SilentSet("CHS", CHSmin);
                                break;
                            case "SD":
                                lr.SilentSet("CHS", CHSsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "CNOD":
                    CNODstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("CNOD", CNODes);
                                break;
                            case "AVG":
                                lr.SilentSet("CNOD", CNODavg);
                                break;
                            case "MAX":
                                lr.SilentSet("CNOD", CNODmax);
                                break;
                            case "MIN":
                                lr.SilentSet("CNOD", CNODmin);
                                break;
                            case "SD":
                                lr.SilentSet("CNOD", CNODsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "CNID":
                    CNIDstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("CNID", CNIDes);
                                break;
                            case "AVG":
                                lr.SilentSet("CNID", CNIDavg);
                                break;
                            case "MAX":
                                lr.SilentSet("CNID", CNIDmax);
                                break;
                            case "MIN":
                                lr.SilentSet("CNID", CNIDmin);
                                break;
                            case "SD":
                                lr.SilentSet("CNID", CNIDsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "PCW":
                    PCWstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("PCW", PCWes);
                                break;
                            case "AVG":
                                lr.SilentSet("PCW", PCWavg);
                                break;
                            case "MAX":
                                lr.SilentSet("PCW", PCWmax);
                                break;
                            case "MIN":
                                lr.SilentSet("PCW", PCWmin);
                                break;
                            case "SD":
                                lr.SilentSet("PCW", PCWsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "Crimp":
                    Crimpstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("Crimp", Crimpes);
                                break;
                            case "AVG":
                                lr.SilentSet("Crimp", Crimpavg);
                                break;
                            case "MAX":
                                lr.SilentSet("Crimp", Crimpmax);
                                break;
                            case "MIN":
                                lr.SilentSet("Crimp", Crimpmin);
                                break;
                            case "SD":
                                lr.SilentSet("Crimp", Crimpsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "CBTO":
                    CBTOstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("CBTO", CBTOes);
                                break;
                            case "AVG":
                                lr.SilentSet("CBTO", CBTOavg);
                                break;
                            case "MAX":
                                lr.SilentSet("CBTO", CBTOmax);
                                break;
                            case "MIN":
                                lr.SilentSet("CBTO", CBTOmin);
                                break;
                            case "SD":
                                lr.SilentSet("CBTO", CBTOsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "COAL":
                    COALstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("COAL", COALes);
                                break;
                            case "AVG":
                                lr.SilentSet("COAL", COALavg);
                                break;
                            case "MAX":
                                lr.SilentSet("COAL", COALmax);
                                break;
                            case "MIN":
                                lr.SilentSet("COAL", COALmin);
                                break;
                            case "SD":
                                lr.SilentSet("COAL", COALsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "MV":
                    MVstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("MV", MVes);
                                break;
                            case "AVG":
                                lr.SilentSet("MV", MVavg);
                                break;
                            case "MAX":
                                lr.SilentSet("MV", MVmax);
                                break;
                            case "MIN":
                                lr.SilentSet("MV", MVmin);
                                break;
                            case "SD":
                                lr.SilentSet("MV", MVsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "VD":
                    VDstat();
                    SendPropertyChangedMsg("ChartDataUpdate");
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("VD", VDes);
                                break;
                            case "AVG":
                                lr.SilentSet("VD", VDavg);
                                break;
                            case "MAX":
                                lr.SilentSet("VD", VDmax);
                                break;
                            case "MIN":
                                lr.SilentSet("VD", VDmin);
                                break;
                            case "SD":
                                lr.SilentSet("VD", VDsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "HD":
                    HDstat();
                    SendPropertyChangedMsg("ChartDataUpdate");
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("HD", HDes);
                                break;
                            case "AVG":
                                lr.SilentSet("HD", HDavg);
                                break;
                            case "MAX":
                                lr.SilentSet("HD", HDmax);
                                break;
                            case "MIN":
                                lr.SilentSet("HD", HDmin);
                                break;
                            case "SD":
                                lr.SilentSet("HD", HDsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "HAD":
                    HADstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("HAD", HADes);
                                break;
                            case "AVG":
                                lr.SilentSet("HAD", HADavg);
                                break;
                            case "MAX":
                                lr.SilentSet("HAD", HADmax);
                                break;
                            case "MIN":
                                lr.SilentSet("HAD", HADmin);
                                break;
                            case "SD":
                                lr.SilentSet("HAD", HADsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "VAD":
                    VADstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("VAD", VADes);
                                break;
                            case "AVG":
                                lr.SilentSet("VAD", VADavg);
                                break;
                            case "MAX":
                                lr.SilentSet("VAD", VADmax);
                                break;
                            case "MIN":
                                lr.SilentSet("VAD", VADmin);
                                break;
                            case "SD":
                                lr.SilentSet("VAD", VADsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "GDV":
                    GDVstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("GDV", GDVes);
                                break;
                            case "AVG":
                                lr.SilentSet("GDV", GDVavg);
                                break;
                            case "MAX":
                                lr.SilentSet("GDV", GDVmax);
                                break;
                            case "MIN":
                                lr.SilentSet("GDV", GDVmin);
                                break;
                            case "SD":
                                lr.SilentSet("GDV", GDVsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "RMSD":
                    RMSDstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("RMSD", RMSDes);
                                break;
                            case "AVG":
                                lr.SilentSet("RMSD", RMSDavg);
                                break;
                            case "MAX":
                                lr.SilentSet("RMSD", RMSDmax);
                                break;
                            case "MIN":
                                lr.SilentSet("RMSD", RMSDmin);
                                break;
                            case "SD":
                                lr.SilentSet("RMSD", RMSDsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "VELAD":
                    VELADstat();
                    lRndData = true;
                    foreach (Round lr in LotStats)
                    {
                        switch (lr.name)
                        {
                            case "ES":
                                lr.SilentSet("VELAD", VELADes);
                                break;
                            case "AVG":
                                lr.SilentSet("VELAD", VELADavg);
                                break;
                            case "MAX":
                                lr.SilentSet("VELAD", VELADmax);
                                break;
                            case "MIN":
                                lr.SilentSet("VELAD", VELADmin);
                                break;
                            case "SD":
                                lr.SilentSet("VELAD", VELADsd);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
            if (lRndData)
            {
                RaisePropertyChanged(nameof(LotStats));
            }
        }
        #endregion

        #region "Private Variables"
        private string _ID;
        private int _TotalCnt;
        private string _LotDate;
        private string _RecipeID;
        private string _SerialNo;
        private string _BulletLot;
        private string _CaseLot;
        private string _PrimerLot;
        private string _PowderLot;
        private ObservableCollection<Round> _Rounds;
        private ObservableCollection<Round> _LotStats;
        private Round _SelectedRound;
        private double _BBTOavg;
        private double _BBTOsd;
        private double _BBTOsdmulti;
        private double _BBTOes;
        private double _BBTOmax;
        private double _BBTOmin;
        private double _BLavg;
        private double _BLsd;
        private double _BLmax;
        private double _BLmin;
        private double _BLes;
        private double _BLsdmulti;
        private double _BWavg;
        private double _BWsd;
        private double _BWmax;
        private double _BWmin;
        private double _BWes;
        private double _BWsdmulti;
        private double _BDavg;
        private double _BDsd;
        private double _BDmax;
        private double _BDmin;
        private double _BDes;
        private double _BDsdmulti;
        private double _CWavg;
        private double _CWsd;
        private double _CWmax;
        private double _CWmin;
        private double _CWes;
        private double _CWsdmulti;
        private double _CVWavg;
        private double _CVWsd;
        private double _CVWmax;
        private double _CVWmin;
        private double _CVWes;
        private double _CVWsdmulti;
        private double _CLavg;
        private double _CLsd;
        private double _CLmax;
        private double _CLmin;
        private double _CLes;
        private double _CLsdmulti;
        private double _CHSavg;
        private double _CHSsd;
        private double _CHSmax;
        private double _CHSmin;
        private double _CHSes;
        private double _CHSsdmulti;
        private double _CNODavg;
        private double _CNODsd;
        private double _CNODmax;
        private double _CNODmin;
        private double _CNODes;
        private double _CNODsdmulti;
        private double _CNIDavg;
        private double _CNIDsd;
        private double _CNIDmax;
        private double _CNIDmin;
        private double _CNIDes;
        private double _CNIDsdmulti;
        private double _PCWavg;
        private double _PCWsd;
        private double _PCWmax;
        private double _PCWmin;
        private double _PCWes;
        private double _PCWsdmulti;
        private double _Crimpavg;
        private double _Crimpsd;
        private double _Crimpmax;
        private double _Crimpmin;
        private double _Crimpes;
        private double _Crimpsdmulti;
        private double _CBTOavg;
        private double _CBTOsd;
        private double _CBTOmax;
        private double _CBTOmin;
        private double _CBTOes;
        private double _CBTOsdmulti;
        private double _COALavg;
        private double _COALsd;
        private double _COALmax;
        private double _COALmin;
        private double _COALes;
        private double _COALsdmulti;
        private double _MVavg;
        private double _MVsd;
        private double _MVmax;
        private double _MVmin;
        private double _MVes;
        private double _MVsdmulti;
        private double _VDavg;
        private double _VDsd;
        private double _VDmax;
        private double _VDmin;
        private double _VDes;
        private double _VDsdmulti;
        private double _HDavg;
        private double _HDsd;
        private double _HDmax;
        private double _HDmin;
        private double _HDes;
        private double _HDsdmulti;
        private double _VADavg;
        private double _VADsd;
        private double _VADmax;
        private double _VADmin;
        private double _VADes;
        private double _VADsdmulti;
        private double _HADavg;
        private double _HADsd;
        private double _HADmax;
        private double _HADmin;
        private double _HADes;
        private double _HADsdmulti;
        private double _RMSDavg;
        private double _RMSDsd;
        private double _RMSDmax;
        private double _RMSDmin;
        private double _RMSDes;
        private double _RMSDsdmulti;
        private double _GDVavg;
        private double _GDVsd;
        private double _GDVmax;
        private double _GDVmin;
        private double _GDVes;
        private double _GDVsdmulti;
        private double _VELADavg;
        private double _VELADsd;
        private double _VELADmax;
        private double _VELADmin;
        private double _VELADes;
        private double _VELADsdmulti;
        #endregion

        #region "Public Properties"
        public string ID { get { return _ID; } set { _ID = value; RaisePropertyChanged(nameof(ID)); } }
        public string LotDate { get { return _LotDate; } set { _LotDate = value; RaisePropertyChanged(nameof(LotDate)); } }
        public string RecipeID { get { return _RecipeID;} set { _RecipeID = value; RaisePropertyChanged(nameof(RecipeID)); } }
        public Int32 SampleCount 
        {
            get
            {
                if (_Rounds != null)
                {
                    return _Rounds.Count;
                }
                else
                {
                    return 0;
                }
            }
        }
        //TODO: add totalcount to data persistance.
        public int TotalCount { get { return _TotalCnt; } set { _TotalCnt = value; RaisePropertyChanged(nameof(TotalCount)); } }
        public string SerialNo { get { return _SerialNo; } set { _SerialNo = value; RaisePropertyChanged(nameof(SerialNo)); } }
        public string BulletLot { get { return _BulletLot; } set { _BulletLot = value; RaisePropertyChanged(nameof(BulletLot)); } }
        public string CaseLot { get { return _CaseLot; } set { _CaseLot = value; RaisePropertyChanged(nameof(CaseLot)); } }
        public string PrimerLot { get { return _PrimerLot; } set { _PrimerLot = value; RaisePropertyChanged(nameof(PrimerLot)); } }
        public string PowderLot { get { return _PowderLot; } set { _PowderLot = value; RaisePropertyChanged(nameof(PowderLot)); } }
        public ObservableCollection<Round> Rounds { get { return _Rounds; } set { _Rounds = value; RaisePropertyChanged(nameof(Rounds)); } }
        public Round SelectedRound { get { return _SelectedRound; } set { _SelectedRound = value; RaisePropertyChanged(nameof(SelectedRound)); } }
        //TODO: add below to data persistance
        public double BBTOavg { get { return _BBTOavg; } set { _BBTOavg = value; RaisePropertyChanged(nameof(BBTOavg)); } }
        public double BBTOsd { get { return _BBTOsd; } set { _BBTOsd = value; RaisePropertyChanged(nameof(BBTOsd)); } }
        public double BBTOsdmulti { get { if (_BBTOsdmulti == 0) _BBTOsdmulti = 1; return _BBTOsdmulti; } set { _BBTOsdmulti = value; RaisePropertyChanged(nameof(BBTOsdmulti)); } }
        public double BBTOes { get { return _BBTOes; } set { _BBTOes = value; RaisePropertyChanged(nameof(BBTOes)); } }
        public double BBTOmax { get { return _BBTOmax; } set { _BBTOmax = value; RaisePropertyChanged(nameof(BBTOmax)); } }
        public double BBTOmin { get { return _BBTOmin; } set { _BBTOmin = value; RaisePropertyChanged(nameof(BBTOmin)); } }
        public double BLavg { get { return _BLavg; } set { _BLavg = value; RaisePropertyChanged(nameof(BLavg)); } }
        public double BLsd { get { return _BLsd; } set { _BLsd = value; RaisePropertyChanged(nameof(BLsd)); } }
        public double BLmax { get { return _BLmax; } set { _BLmax = value; RaisePropertyChanged(nameof(BLmax)); } }
        public double BLmin { get { return _BLmin; } set { _BLmin = value; RaisePropertyChanged(nameof(BLmin)); } }
        public double BLes { get { return _BLes; } set { _BLes = value; RaisePropertyChanged(nameof(BLes)); } }
        public double BLsdmulti { get { if (_BLsdmulti == 0) _BLsdmulti = 1; return _BLsdmulti; } set { _BLsdmulti = value; RaisePropertyChanged(nameof(BLsdmulti)); } }
        public double BWavg { get { return _BWavg; } set { _BWavg = value; RaisePropertyChanged(nameof(BWavg)); } }
        public double BWsd { get { return _BWsd; } set { _BWsd = value; RaisePropertyChanged(nameof(BWsd)); } }
        public double BWmax { get { return _BWmax; } set { _BWmax = value; RaisePropertyChanged(nameof(BWmax)); } }
        public double BWmin { get { return _BWmin; } set { _BWmin = value; RaisePropertyChanged(nameof(BWmin)); } }
        public double BWes { get { return _BWes; } set { _BWes = value; RaisePropertyChanged(nameof(BWes)); } }
        public double BWsdmulti { get { if (_BWsdmulti == 0) _BWsdmulti = 1; return _BWsdmulti; } set { _BWsdmulti = value; RaisePropertyChanged(nameof(BWsdmulti)); } }
        public double BDavg { get { return _BDavg; } set { _BDavg = value; RaisePropertyChanged(nameof(BDavg)); } }
        public double BDsd { get { return _BDsd; } set { _BDsd = value; RaisePropertyChanged(nameof(BDsd)); } }
        public double BDmax { get { return _BDmax; } set { _BDmax = value; RaisePropertyChanged(nameof(BDmax)); } }
        public double BDmin { get { return _BDmin; } set { _BDmin = value; RaisePropertyChanged(nameof(BDmin)); } }
        public double BDes { get { return _BDes; } set { _BDes = value; RaisePropertyChanged(nameof(BDes)); } }
        public double BDsdmulti { get { if (_BDsdmulti == 0) _BDsdmulti = 1; return _BDsdmulti; } set { _BDsdmulti = value; RaisePropertyChanged(nameof(BDsdmulti)); } }
        public double CWavg { get { return _CWavg; } set { _CWavg = value; RaisePropertyChanged(nameof(CWavg)); } }
        public double CWsd { get { return _CWsd; } set { _CWsd = value; RaisePropertyChanged(nameof(CWsd)); } }
        public double CWmax { get { return _CWmax; } set { _CWmax = value; RaisePropertyChanged(nameof(CWmax)); } }
        public double CWmin { get { return _CWmin; } set { _CWmin = value; RaisePropertyChanged(nameof(CWmin)); } }
        public double CWes { get { return _CWes; } set { _CWes = value; RaisePropertyChanged(nameof(CWes)); } }
        public double CWsdmulti { get { if (_CWsdmulti == 0) _CWsdmulti = 1; return _CWsdmulti; } set { _CWsdmulti = value; RaisePropertyChanged(nameof(CWsdmulti)); } }
        public double CVWavg { get { return _CVWavg; } set { _CVWavg = value; RaisePropertyChanged(nameof(CVWavg)); } }
        public double CVWsd { get { return _CVWsd; } set { _CVWsd = value; RaisePropertyChanged(nameof(CVWsd)); } }
        public double CVWmax { get { return _CVWmax; } set { _CVWmax = value; RaisePropertyChanged(nameof(CVWmax)); } }
        public double CVWmin { get { return _CVWmin; } set { _CVWmin = value; RaisePropertyChanged(nameof(CVWmin)); } }
        public double CVWes { get { return _CVWes; } set { _CVWes = value; RaisePropertyChanged(nameof(CVWes)); } }
        public double CVWsdmulti { get { if (_CVWsdmulti == 0) _CVWsdmulti = 1; return _CVWsdmulti; } set { _CVWsdmulti = value; RaisePropertyChanged(nameof(CVWsdmulti)); } }
        public double CLavg { get { return _CLavg; } set { _CLavg = value; RaisePropertyChanged(nameof(CLavg)); } }
        public double CLsd { get { return _CLsd; } set { _CLsd = value; RaisePropertyChanged(nameof(CLsd)); } }
        public double CLmax { get { return _CLmax; } set { _CLmax = value; RaisePropertyChanged(nameof(CLmax)); } }
        public double CLmin { get { return _CLmin; } set { _CLmin = value; RaisePropertyChanged(nameof(CLmin)); } }
        public double CLes { get { return _CLes; } set { _CLes = value; RaisePropertyChanged(nameof(CLes)); } }
        public double CLsdmulti { get { if (_CLsdmulti == 0) _CLsdmulti = 1; return _CLsdmulti; } set { _CLsdmulti = value; RaisePropertyChanged(nameof(CLsdmulti)); } }
        public double CHSavg { get { return _CHSavg; } set { _CHSavg = value; RaisePropertyChanged(nameof(CHSavg)); } }
        public double CHSsd { get { return _CHSsd; } set { _CHSsd = value; RaisePropertyChanged(nameof(CHSsd)); } }
        public double CHSmax { get { return _CHSmax; } set { _CHSmax = value; RaisePropertyChanged(nameof(CHSmax)); } }
        public double CHSmin { get { return _CHSmin; } set { _CHSmin = value; RaisePropertyChanged(nameof(CHSmin)); } }
        public double CHSes { get { return _CHSes; } set { _CHSes = value; RaisePropertyChanged(nameof(CHSes)); } }
        public double CHSsdmulti { get { if (_CHSsdmulti == 0) _CHSsdmulti = 1; return _CHSsdmulti; } set { _CHSsdmulti = value; RaisePropertyChanged(nameof(CHSsdmulti)); } }
        public double CNODavg { get { return _CNODavg; } set { _CNODavg = value; RaisePropertyChanged(nameof(CNODavg)); } }
        public double CNODsd { get { return _CNODsd; } set { _CNODsd = value; RaisePropertyChanged(nameof(CNODsd)); } }
        public double CNODmax { get { return _CNODmax; } set { _CNODmax = value; RaisePropertyChanged(nameof(CNODmax)); } }
        public double CNODmin { get { return _CNODmin; } set { _CNODmin = value; RaisePropertyChanged(nameof(CNODmin)); } }
        public double CNODes { get { return _CNODes; } set { _CNODes = value; RaisePropertyChanged(nameof(CNODes)); } }
        public double CNODsdmulti { get { if (_CNODsdmulti == 0) _CNODsdmulti = 1; return _CNODsdmulti; } set { _CNODsdmulti = value; RaisePropertyChanged(nameof(CNODsdmulti)); } }
        public double CNIDavg { get { return _CNIDavg; } set { _CNIDavg = value; RaisePropertyChanged(nameof(CNIDavg)); } }
        public double CNIDsd { get { return _CNIDsd; } set { _CNIDsd = value; RaisePropertyChanged(nameof(CNIDsd)); } }
        public double CNIDmax { get { return _CNIDmax; } set { _CNIDmax = value; RaisePropertyChanged(nameof(CNIDmax)); } }
        public double CNIDmin { get { return _CNIDmin; } set { _CNIDmin = value; RaisePropertyChanged(nameof(CNIDmin)); } }
        public double CNIDes { get { return _CNIDes; } set { _CNIDes = value; RaisePropertyChanged(nameof(CNIDes)); } }
        public double CNIDsdmulti { get { if (_CNIDsdmulti == 0) _CNIDsdmulti = 1; return _CNIDsdmulti; } set { _CNIDsdmulti = value; RaisePropertyChanged(nameof(CNIDsdmulti)); } }
        public double PCWavg { get { return _PCWavg; } set { _PCWavg = value; RaisePropertyChanged(nameof(PCWavg)); } }
        public double PCWsd { get { return _PCWsd; } set { _PCWsd = value; RaisePropertyChanged(nameof(PCWsd)); } }
        public double PCWmax { get { return _PCWmax; } set { _PCWmax = value; RaisePropertyChanged(nameof(PCWmax)); } }
        public double PCWmin { get { return _PCWmin; } set { _PCWmin = value; RaisePropertyChanged(nameof(PCWmin)); } }
        public double PCWes { get { return _PCWes; } set { _PCWes = value; RaisePropertyChanged(nameof(PCWes)); } }
        public double PCWsdmulti { get { if (_PCWsdmulti == 0) _PCWsdmulti = 1; return _PCWsdmulti; } set { _PCWsdmulti = value; RaisePropertyChanged(nameof(PCWsdmulti)); } }
        public double Crimpavg { get { return _Crimpavg; } set { _Crimpavg = value; RaisePropertyChanged(nameof(Crimpavg)); } }
        public double Crimpsd { get { return _Crimpsd; } set { _Crimpsd = value; RaisePropertyChanged(nameof(Crimpsd)); } }
        public double Crimpmax { get { return _Crimpmax; } set { _Crimpmax = value; RaisePropertyChanged(nameof(Crimpmax)); } }
        public double Crimpmin { get { return _Crimpmin; } set { _Crimpmin = value; RaisePropertyChanged(nameof(Crimpmin)); } }
        public double Crimpes { get { return _Crimpes; } set { _Crimpes = value; RaisePropertyChanged(nameof(Crimpes)); } }
        public double Crimpsdmulti { get { if (_Crimpsdmulti == 0) _Crimpsdmulti = 1; return _Crimpsdmulti; } set { _Crimpsdmulti = value; RaisePropertyChanged(nameof(Crimpsdmulti)); } }
        public double CBTOavg { get { return _CBTOavg; } set { _CBTOavg = value; RaisePropertyChanged(nameof(CBTOavg)); } }
        public double CBTOsd { get { return _CBTOsd; } set { _CBTOsd = value; RaisePropertyChanged(nameof(CBTOsd)); } }
        public double CBTOmax { get { return _CBTOmax; } set { _CBTOmax = value; RaisePropertyChanged(nameof(CBTOmax)); } }
        public double CBTOmin { get { return _CBTOmin; } set { _CBTOmin = value; RaisePropertyChanged(nameof(_CBTOmin)); } }
        public double CBTOes { get { return _CBTOes; } set { _CBTOes = value; RaisePropertyChanged(nameof(_CBTOes)); } }
        public double CBTOsdmulti { get { if (_CBTOsdmulti == 0) _CBTOsdmulti = 1; return _CBTOsdmulti; } set { _CBTOsdmulti = value; RaisePropertyChanged(nameof(CBTOsdmulti)); } }
        public double COALavg { get { return _COALavg; } set { _COALavg = value; RaisePropertyChanged(nameof(COALavg)); } }
        public double COALsd { get { return _COALsd; } set { _COALsd = value; RaisePropertyChanged(nameof(COALsd)); } }
        public double COALmax { get { return _COALmax; } set { _COALmax = value; RaisePropertyChanged(nameof(COALmax)); } }
        public double COALmin { get { return _COALmin; } set { _COALmin = value; RaisePropertyChanged(nameof(COALmin)); } }
        public double COALes { get { return _COALes; } set { _COALes = value; RaisePropertyChanged(nameof(COALes)); } }
        public double COALsdmulti { get { if (_COALsdmulti == 0) _COALsdmulti = 1; return _COALsdmulti; } set { _COALsdmulti = value; RaisePropertyChanged(nameof(COALsdmulti)); } }
        public double MVavg { get { return _MVavg; } set { _MVavg = value; UpdateVELAD(); RaisePropertyChanged(nameof(MVavg)); } }
        public double MVsd { get { return _MVsd; } set { _MVsd = value; RaisePropertyChanged(nameof(MVsd)); } }
        public double MVmax { get { return _MVmax; } set { _MVmax = value; RaisePropertyChanged(nameof(MVmax)); } }
        public double MVmin { get { return _MVmin; } set { _MVmin = value; RaisePropertyChanged(nameof(MVmin)); } }
        public double MVes { get { return _MVes; } set { _MVes = value; RaisePropertyChanged(nameof(MVes)); } }
        public double MVsdmulti { get { if (_MVsdmulti == 0) _MVsdmulti = 1; return _MVsdmulti; } set { _MVsdmulti = value; RaisePropertyChanged(nameof(MVsdmulti)); } }
        public double VDavg { get { return _VDavg; } set { _VDavg = value; UpdateVAD(); RaisePropertyChanged(nameof(VDavg)); } }
        public double VDsd { get { return _VDsd; } set { _VDsd = value; RaisePropertyChanged(nameof(VDsd)); } }
        public double VDmax { get { return _VDmax; } set { _VDmax = value; RaisePropertyChanged(nameof(VDmax)); } }
        public double VDmin { get { return _VDmin; } set { _VDmin = value; RaisePropertyChanged(nameof(VDmin)); } }
        public double VDes { get { return _VDes; } set { _VDes = value; RaisePropertyChanged(nameof(VDes)); } }
        public double VDsdmulti { get { if (_VDsdmulti == 0) _VDsdmulti = 1; return _VDsdmulti; } set { _VDsdmulti = value; RaisePropertyChanged(nameof(VDsdmulti)); } }
        public double HDavg { get { return _HDavg; } set { _HDavg = value; UpdateHAD(); RaisePropertyChanged(nameof(HDavg)); } }
        public double HDsd { get { return _HDsd; } set { _HDsd = value; RaisePropertyChanged(nameof(HDsd)); } }
        public double HDmax { get { return _HDmax; } set { _HDmax = value; RaisePropertyChanged(nameof(HDmax)); } }
        public double HDmin { get { return _HDmin; } set { _HDmin = value; RaisePropertyChanged(nameof(HDmin)); } }
        public double HDes { get { return _HDes; } set { _HDes = value; RaisePropertyChanged(nameof(HDes)); } }
        public double HDsdmulti { get { if (_HDsdmulti == 0) _HDsdmulti = 1; return _HDsdmulti; } set { _HDsdmulti = value; RaisePropertyChanged(nameof(HDsdmulti)); } }
        public double VADavg { get { return _VADavg; } set { _VADavg = value; RaisePropertyChanged(nameof(VADavg)); } }
        public double VADsd { get { return _VADsd; } set { _VADsd = value; RaisePropertyChanged(nameof(VADsd)); } }
        public double VADmax { get { return _VADmax; } set { _VADmax = value; RaisePropertyChanged(nameof(VADmax)); } }
        public double VADmin { get { return _VADmin; } set { _VADmin = value; RaisePropertyChanged(nameof(VADmin)); } }
        public double VADes { get { return _VADes; } set { _VADes = value; RaisePropertyChanged(nameof(VADes)); } }
        public double VADsdmulti { get { if (_VADsdmulti == 0) _VADsdmulti = 1; return _VADsdmulti; } set { _VADsdmulti = value; RaisePropertyChanged(nameof(VADsdmulti)); } }
        public double HADavg { get { return _HADavg; } set { _HADavg = value; RaisePropertyChanged(nameof(HADavg)); } }
        public double HADsd { get { return _HADsd; } set { _HADsd = value; RaisePropertyChanged(nameof(HADsd)); } }
        public double HADmax { get { return _HADmax; } set { _HADmax = value; RaisePropertyChanged(nameof(HADmax)); } }
        public double HADmin { get { return _HADmin; } set { _HADmin = value; RaisePropertyChanged(nameof(HADmin)); } }
        public double HADes { get { return _HADes; } set { _HADes = value; RaisePropertyChanged(nameof(HADes)); } }
        public double HADsdmulti { get { if (_HADsdmulti == 0) _HADsdmulti = 1; return _HADsdmulti; } set { _HADsdmulti = value; RaisePropertyChanged(nameof(HADsdmulti)); } }
        public double RMSDavg { get { return _RMSDavg; } set { _RMSDavg = value; RaisePropertyChanged(nameof(RMSDavg)); } }
        public double RMSDsd { get { return _RMSDsd; } set { _RMSDsd = value; RaisePropertyChanged(nameof(RMSDsd)); } }
        public double RMSDmax { get { return _RMSDmax; } set { _RMSDmax = value; RaisePropertyChanged(nameof(RMSDmax)); } }
        public double RMSDmin { get { return _RMSDmin; } set { _RMSDmin = value; RaisePropertyChanged(nameof(RMSDmin)); } }
        public double RMSDes { get { return _RMSDes; } set { _RMSDes = value; RaisePropertyChanged(nameof(RMSDes)); } }
        public double RMSDsdmulti { get { if (_RMSDsdmulti == 0) _RMSDsdmulti = 1; return _RMSDsdmulti; } set { _RMSDsdmulti = value; RaisePropertyChanged(nameof(RMSDsdmulti)); } }
        public double GDVavg { get { return _GDVavg; } set { _GDVavg = value; RaisePropertyChanged(nameof(GDVavg)); } }
        public double GDVsd { get { return _GDVsd; } set { _GDVsd = value; RaisePropertyChanged(nameof(GDVsd)); } }
        public double GDVmax { get { return _GDVmax; } set { _GDVmax = value; RaisePropertyChanged(nameof(GDVmax)); } }
        public double GDVmin { get { return _GDVmin; } set { _GDVmin = value; RaisePropertyChanged(nameof(GDVmin)); } }
        public double GDVes { get { return _GDVes; } set { _GDVes = value; RaisePropertyChanged(nameof(GDVes)); } }
        public double GDVsdmulti { get { if (_GDVsdmulti == 0) _GDVsdmulti = 1; return _GDVsdmulti; } set { _GDVsdmulti = value; RaisePropertyChanged(nameof(GDVsdmulti)); } }
        public double VELADavg { get { return _VELADavg; } set { _VELADavg = value; RaisePropertyChanged(nameof(VELADavg)); } }
        public double VELADsd { get { return _VELADsd; } set { _VELADsd = value; RaisePropertyChanged(nameof(VELADsd)); } }
        public double VELADmax { get { return _VELADmax; } set { _VELADmax = value; RaisePropertyChanged(nameof(VELADmax)); } }
        public double VELADmin { get { return _VELADmin; } set { _VELADmin = value; RaisePropertyChanged(nameof(VELADmin)); } }
        public double VELADes { get { return _VELADes; } set { _VELADes = value; RaisePropertyChanged(nameof(VELADes)); } }
        public double VELADsdmulti { get { if (_VELADsdmulti == 0) _VELADsdmulti = 1; return _VELADsdmulti; } set { _VELADsdmulti = value; RaisePropertyChanged(nameof(VELADsdmulti)); } }
        public ObservableCollection<Round> LotStats { get { return _LotStats; } set { _LotStats = value; RaisePropertyChanged(nameof(LotStats)); } }
        #endregion

        #region "Constructor"
        public RecipeLot()
        {
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<PropertyChangedMsg>(this, (action ) => ReceiveMessage(action ));
            Messenger.Default.Register<PropertyChangedMsg>(this, (Msg) => ReceiveMessage(Msg));
            _ID = Guid.NewGuid().ToString();
            _LotDate = "";
            _RecipeID="";
            _SerialNo="";
            _BulletLot="";
            _CaseLot="";
            _PrimerLot="";
            _PowderLot="";
            Rounds = new ObservableCollection<Round>();
            LotStats = new ObservableCollection<Round>();
            Round lstat = new Round();
            lstat.name = "SD";
            LotStats.Add(lstat);
            lstat = new Round();
            lstat.name = "ES"; 
            LotStats.Add(lstat);
            lstat = new Round();
            lstat.name = "MAX";
            LotStats.Add(lstat);
            lstat = new Round();
            lstat.name = "MIN";
            LotStats.Add(lstat);
            lstat = new Round();
            lstat.name = "AVG";
            LotStats.Add(lstat);
        }
        #endregion

        #region "Private Routines"
        #region "Property Stats"
        private void BBTOstat()
        {
            double lTot = 0, lmax=0, lmin=0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if(lCnt ==0)
            {
                BBTOavg = 0;
                BBTOsd = 0;
                BBTOmax = 0;
                BBTOmin = 0;
                BBTOes = 0;
                return;
            }
            foreach(Round lr in Rounds)
            {
                lVals[lI] = lr.BBTO;
                if(lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.BBTO;
                BBTOes = (lmax - lmin);
                BBTOmax = lmax;
                BBTOmin = lmin;
            }
            BBTOavg = lTot / lCnt;
            BBTOsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void BLstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                BLavg = 0;
                BLsd = 0;
                BLmax = 0;
                BLmin = 0;
                BLes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.BL;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.BL;
                BLes = (lmax - lmin);
                BLmax = lmax;
                BLmin = lmin;
            }
            BLavg = lTot / lCnt;
            BLsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void BWstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                BWavg = 0;
                BWsd = 0;
                BWmax = 0;
                BWmin = 0;
                BWes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.BW;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.BW;
                BWes = (lmax - lmin);
                BWmax = lmax;
                BWmin = lmin;
            }
            BWavg = lTot / lCnt;
            BWsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void BDstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                BDavg = 0;
                BDsd = 0;
                BDmax = 0;
                BDmin = 0;
                BDes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.BD;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.BD;
                BDes = (lmax - lmin);
                BDmax = lmax;
                BDmin = lmin;
            }
            BDavg = lTot / lCnt;
            BDsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void CWstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                CWavg = 0;
                CWsd = 0;
                CWmax = 0;
                CWmin = 0;
                CWes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.CW;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.CW;
                CWes = (lmax - lmin);
                CWmax = lmax;
                CWmin = lmin;
            }
            CWavg = lTot / lCnt;
            CWsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void CVWstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                CVWavg = 0;
                CVWsd = 0;
                CVWmax = 0;
                CVWmin = 0;
                CVWes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.CVW;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.CVW;
                CVWes = (lmax - lmin);
                CVWmax = lmax;
                CVWmin = lmin;
            }
            CVWavg = lTot / lCnt;
            CVWsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void CLstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                CLavg = 0;
                CLsd = 0;
                CLmax = 0;
                CLmin = 0;
                CLes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.CL;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.CL;
                CLes = (lmax - lmin);
                CLmax = lmax;
                CLmin = lmin;
            }
            CLavg = lTot / lCnt;
            CLsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void CHSstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                CHSavg = 0;
                CHSsd = 0;
                CHSmax = 0;
                CHSmin = 0;
                CHSes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.CHS;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.CHS;
                CHSes = (lmax - lmin);
                CHSmax = lmax;
                CHSmin = lmin;
            }
            CHSavg = lTot / lCnt;
            CHSsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void CNODstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                CNODavg = 0;
                CNODsd = 0;
                CNODmax = 0;
                CNODmin = 0;
                CNODes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.CNOD;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.CNOD;
                CNODes = (lmax - lmin);
                CNODmax = lmax;
                CNODmin = lmin;
            }
            CNODavg = lTot / lCnt;
            CNODsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void CNIDstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                CNIDavg = 0;
                CNIDsd = 0;
                CNIDmax = 0;
                CNIDmin = 0;
                CNIDes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.CNID;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.CNID;
                CNIDes = (lmax - lmin);
                CNIDmax = lmax;
                CNIDmin = lmin;
            }
            CNIDavg = lTot / lCnt;
            CNIDsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void PCWstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                PCWavg = 0;
                PCWsd = 0;
                PCWmax = 0;
                PCWmin = 0;
                PCWes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.PCW;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.PCW;
                PCWes = (lmax - lmin);
                PCWmax = lmax;
                PCWmin = lmin;
            }
            PCWavg = lTot / lCnt;
            PCWsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void Crimpstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                Crimpavg = 0;
                Crimpsd = 0;
                Crimpmax = 0;
                Crimpmin = 0;
                Crimpes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.Crimp;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.Crimp;
                Crimpes = (lmax - lmin);
                Crimpmax = lmax;
                Crimpmin = lmin;
            }
            Crimpavg = lTot / lCnt;
            Crimpsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void CBTOstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                CBTOavg = 0;
                CBTOsd = 0;
                CBTOmax = 0;
                CBTOmin = 0;
                CBTOes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.CBTO;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.CBTO;
                CBTOes = (lmax - lmin);
                CBTOmax = lmax;
                CBTOmin = lmin;
            }
            CBTOavg = lTot / lCnt;
            CBTOsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void COALstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                COALavg = 0;
                COALsd = 0;
                COALmax = 0;
                COALmin = 0;
                COALes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.COAL;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.COAL;
                COALes = (lmax - lmin);
                COALmax = lmax;
                COALmin = lmin;
            }
            COALavg = lTot / lCnt;
            COALsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void MVstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                MVavg = 0;
                MVsd = 0;
                MVmax = 0;
                MVmin = 0;
                MVes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.MV;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.MV;
                MVes = (lmax - lmin);
                MVmax = lmax;
                MVmin = lmin;
            }
            MVavg = lTot / lCnt;
            MVsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void VDstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                VDavg = 0;
                VDsd = 0;
                VDmax = 0;
                VDmin = 0;
                VDes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.VD;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.VD;
                VDes = (lmax - lmin);
                VDmax = lmax;
                VDmin = lmin;
            }
            VDavg = lTot / lCnt;
            VDsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void HDstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                HDavg = 0;
                HDsd = 0;
                HDmax = 0;
                HDmin = 0;
                HDes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.HD;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.HD;
                HDes = (lmax - lmin);
                HDmax = lmax;
                HDmin = lmin;
            }
            HDavg = lTot / lCnt;
            HDsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void VADstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                VADavg = 0;
                VADsd = 0;
                VADmax = 0;
                VADmin = 0;
                VADes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.VAD;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.VAD;
                VADes = (lmax - lmin);
                VADmax = lmax;
                VADmin = lmin;
            }
            VADavg = lTot / lCnt;
            VADsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void HADstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                HADavg = 0;
                HADsd = 0;
                HADmax = 0;
                HADmin = 0;
                HADes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.HAD;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.HAD;
                HADes = (lmax - lmin);
                HADmax = lmax;
                HADmin = lmin;
            }
            HADavg = lTot / lCnt;
            HADsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void RMSDstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                RMSDavg = 0;
                RMSDsd = 0;
                RMSDmax = 0;
                RMSDmin = 0;
                RMSDes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.RMSD;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.RMSD;
                RMSDes = (lmax - lmin);
                RMSDmax = lmax;
                RMSDmin = lmin;
            }
            RMSDavg = lTot / lCnt;
            RMSDsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void GDVstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                GDVavg = 0;
                GDVsd = 0;
                GDVmax = 0;
                GDVmin = 0;
                GDVes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.GDV;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.GDV;
                GDVes = (lmax - lmin);
                GDVmax = lmax;
                GDVmin = lmin;
            }
            GDVavg = lTot / lCnt;
            GDVsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        private void VELADstat()
        {
            double lTot = 0, lmax = 0, lmin = 0;
            double[] lVals = new double[Rounds.Count];
            Int32 lCnt = 0, lI = 0;

            lCnt = Rounds.Count;
            if (lCnt == 0)
            {
                VELADavg = 0;
                VELADsd = 0;
                VELADmax = 0;
                VELADmin = 0;
                VELADes = 0;
                return;
            }
            foreach (Round lr in Rounds)
            {
                lVals[lI] = lr.VELAD;
                if (lI == 0)
                {
                    lmin = lVals[lI];
                    lmax = lVals[lI];
                }
                if (lmax < lVals[lI]) lmax = lVals[lI];
                if (lmin > lVals[lI]) lmin = lVals[lI];
                lI++;
                lTot = lTot + lr.VELAD;
                VELADes = (lmax - lmin);
                VELADmax = lmax;
                VELADmin = lmin;
            }
            VELADavg = lTot / lCnt;
            VELADsd = LawlerBallisticsFactory.SampleStandardDeviation(lVals);
        }
        #endregion
        #region "Update Relative Properties"
        private void UpdateHAD()
        {
            foreach(Round lR in Rounds)
            {
                lR.HAD = Math.Abs(lR.HD - HDavg);
            }
            UpdateRMSD();
        }
        private void UpdateVAD()
        {
            foreach (Round lR in Rounds)
            {
                lR.VAD = Math.Abs(lR.VD - VDavg);
            }
            UpdateRMSD();
        }
        private void UpdateRMSD()
        {
            foreach (Round lR in Rounds)
            {
                lR.RMSD = Math.Pow((Math.Pow(lR.VAD, 2) + Math.Pow(lR.HAD, 2)), .5);
            }
            UpdateGDV();
        }
        private void UpdateGDV()
        {
            double lComp = RMSDavg + (RMSDsd * RMSDsdmulti);

            foreach (Round lR in Rounds)
            {
                lR.GDV = lComp - lR.RMSD;
            }
        }
        private void UpdateVELAD()
        {
            foreach (Round lR in Rounds)
            {
                lR.VELAD = Math.Abs(lR.MV - MVavg);
            }
        }
        #endregion
        #endregion
    }
}
