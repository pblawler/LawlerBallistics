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
    public class Recipe : INotifyPropertyChanged
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
        private string _ID;
        private string _Name;
        private string _CartridgeID;
        private Cartridge _RecpCartridge;
        private string _GunID;
        private Gun _RecpGun;
        private string _BarrelID;
        private Barrel _RecpBarrel;
        private string _CaseID;
        private Case _RecpCase;
        private string _PrimerID;
        private Primer _RecpPrimer;
        private string _BulletID;
        private Bullet _RecpBullet;
        private double _BulletSortWt;
        private double _BulletSortBTO;
        private double _BulletSortOAL;
        private string _PowderID;
        private Powder _RecpPowder;
        private double _ChargeWt;
        private double _CaseTrimLength;
        private double _HeadSpace;
        private double _CBTO;
        private double _COAL;
        private double _NeckClearance;
        private double _JumpDistance;
        private double _DoRate;
        private string _Notes;
        private ObservableCollection<CartridgeLot> _Lots;
        private CartridgeLot _SelectedLot;
        #endregion

        #region "Properties"
        public string ID { get { return _ID; } set { _ID = value; RaisePropertyChanged(nameof(ID)); } }
        public string Name { get { return _Name; } set { _Name = value; RaisePropertyChanged(nameof(Name)); } }
        public string CartridgeID { get { return _CartridgeID; } set { _CartridgeID = value; RaisePropertyChanged(nameof(CartridgeID)); } }
        public Cartridge RecpCartridge { get { return _RecpCartridge; } set { _RecpCartridge = value; RaisePropertyChanged(nameof(RecpCartridge)); } }
        public string GunID { get { return _GunID; } set { _GunID = value; RaisePropertyChanged(nameof(GunID)); } }
        public Gun RecpGun { get { return _RecpGun; } set { _RecpGun = value; RaisePropertyChanged(nameof(RecpGun)); } }
        public string BarrelID { get { return _BarrelID; } set { _GunID = value; RaisePropertyChanged(nameof(BarrelID)); } }
        public Barrel RecpBarrel { get { return _RecpBarrel; } set { _RecpBarrel = value; RaisePropertyChanged(nameof(RecpBarrel)); } }
        public string CaseID { get { return _CaseID; } set { _CaseID = value; RaisePropertyChanged(nameof(CaseID)); } }
        public Case RecpCase { get { return _RecpCase; } set { _RecpCase = value; RaisePropertyChanged(nameof(RecpCase)); } }
        public string PrimerID { get { return _PrimerID; } set { _PrimerID = value; RaisePropertyChanged(nameof(PrimerID)); } }
        public Primer RecpPrimer { get { return _RecpPrimer; } set { _RecpPrimer = value; RaisePropertyChanged(nameof(RecpPrimer)); } }
        public string BulletID { get { return _BulletID; } set { _BulletID = value; RaisePropertyChanged(nameof(BulletID)); } }
        public Bullet RecpBullet { get { return _RecpBullet; } set { _RecpBullet = value; RaisePropertyChanged(nameof(RecpBullet)); } }
        public double BulletSortWt { get { return _BulletSortWt; } set { _BulletSortWt = value; RaisePropertyChanged(nameof(BulletSortWt)); } }
        public double BulletSortBTO { get { return _BulletSortBTO; } set { _BulletSortBTO = value; RaisePropertyChanged(nameof(BulletSortBTO)); } }
        public double BulletSortOAL { get { return _BulletSortOAL; } set { _BulletSortOAL = value; RaisePropertyChanged(nameof(BulletSortOAL)); } }
        public string PowderID { get { return _PowderID; } set { _PowderID = value; RaisePropertyChanged(nameof(PowderID)); } }
        public Powder RecpPowder { get { return _RecpPowder; } set { _RecpPowder = value; RaisePropertyChanged(nameof(RecpPowder)); } }
        public double ChargeWt { get { return _ChargeWt; } set { _ChargeWt = value; RaisePropertyChanged(nameof(ChargeWt)); } }
        public double CaseTrimLength { get { return _CaseTrimLength; } set { _CaseTrimLength = value; RaisePropertyChanged(nameof(CaseTrimLength)); } }
        public double HeadSpace { get { return _HeadSpace; } set { _HeadSpace = value; RaisePropertyChanged(nameof(HeadSpace)); } }
        public double CBTO { get { return _CBTO; } set { _CBTO = value; RaisePropertyChanged(nameof(CBTO)); } }
        public double COAL { get { return _COAL; } set { _COAL = value; RaisePropertyChanged(nameof(COAL)); } }
        public double NeckClearance { get { return _NeckClearance; } set { _NeckClearance = value; RaisePropertyChanged(nameof(NeckClearance)); } }
        public double JumpDistance { get { return _JumpDistance; } set { _JumpDistance = value; RaisePropertyChanged(nameof(JumpDistance)); } }
        public double FoRate { get { return _DoRate; } set { _DoRate = value; RaisePropertyChanged(nameof(FoRate)); } }
        public string Notes { get { return _Notes; } set { _Notes = value; RaisePropertyChanged(nameof(Notes)); } }
        public ObservableCollection<CartridgeLot> Lots { get { return _Lots; } set { _Lots = value; RaisePropertyChanged(nameof(Lots)); } }
        public CartridgeLot SelectedLot
        {
            get { return _SelectedLot; }
            set 
            {
                _SelectedLot = value;
                RaisePropertyChanged(nameof(SelectedLot));
            }
        }
        #endregion

        #region "Constructor"
        public Recipe()
        {
            _ID = Guid.NewGuid().ToString();
            _Lots = new ObservableCollection<CartridgeLot>();
            _BarrelID = "";
            _BulletID = "";
            _CartridgeID = "";
            _CaseID = "";
            _GunID = "";
            _Name = "";
            _Notes = "";
            _PowderID = "";
            _PrimerID = "";
        }
        #endregion
    }
}
