using LawlerBallisticsDesk.Classes.BallisticClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class LoadOut : INotifyPropertyChanged
    {
        #region "Binding"
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }        
        private void SelectedGun_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
        private void SelectedLoadRecipe_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //RaisePropertyChanged(nameof(SelectedCartridge));
        }
        private void zeroData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
        private void MyDragSlopeData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
        #endregion

        #region "Private Variables"
        private string _ID="";
        private Gun _SelectedGun;
        private Recipe _SelectedLoadRecipe;
        private string _BarrelID="";
        private Barrel _Barrel;
        private double _ScopeHeight;
        private double _MuzzleVelocity;
        private double _BSG;
        private double _MaxRange;
        private ZeroData _zeroData;
        private DragSlopeData _MyDragSlopeData;
        #endregion

        #region "Properties"
        public string ID { get { return _ID; } }
        public Gun SelectedGun {
            get
            {
                return _SelectedGun;
            }
            set
            {
                SelectedGun.PropertyChanged -= SelectedGun_PropertyChanged;
                _SelectedGun = value;
                SelectedGun.PropertyChanged += SelectedGun_PropertyChanged;
                RaisePropertyChanged(nameof(SelectedGun));
                _Barrel = _SelectedGun.SelectedBarrel;
                if (_Barrel != null)
                {
                    _BarrelID = _Barrel.ID;
                    RaisePropertyChanged(nameof(SelectedBarrel));
                    RaisePropertyChanged(nameof(SelectedBarrelID));
                }
            }
        }
        public string SelectedBarrelID { get { return _BarrelID; } set { _BarrelID = value; RaisePropertyChanged(nameof(SelectedBarrelID)); } }
        public Barrel SelectedBarrel { get { return _Barrel; } set { _Barrel = value; RaisePropertyChanged(nameof(SelectedBarrel)); } }
        public Recipe SelectedCartridge { get { return _SelectedLoadRecipe; } set { _SelectedLoadRecipe = value; RaisePropertyChanged(nameof(SelectedCartridge)); } }
        public double ScopeHeight
        {
            get
            {
                //TODO: add the ability for the scope to be mounted on the barrel or reciever.
                return SelectedGun.ScopeHeight;
            }
            set
            {
                SelectedGun.ScopeHeight = value;
                RaisePropertyChanged(nameof(ScopeHeight));
            }
        }
        public double MuzzleVelocity { get { return _MuzzleVelocity; } set { _MuzzleVelocity = value; RaisePropertyChanged(nameof(MuzzleVelocity)); } }
        /// <summary>
        /// Bullet stability factor
        /// </summary>
        public double BSG { get { return _BSG; } set { _BSG = value; RaisePropertyChanged(nameof(BSG)); } }
        public double MaxRange { get { return _MaxRange; } set { _MaxRange = value; RaisePropertyChanged(nameof(MaxRange)); } }
        public ZeroData zeroData
        {
            get
            {
                return _zeroData;
            }
            set
            {
                zeroData.PropertyChanged -= zeroData_PropertyChanged;
                _zeroData = value;
                zeroData.PropertyChanged += zeroData_PropertyChanged;
                RaisePropertyChanged(nameof(zeroData));
            }
        }
        public DragSlopeData MyDragSlopeData
        {
            get
            {
                return _MyDragSlopeData;
            }
            set
            {
                MyDragSlopeData.PropertyChanged -= MyDragSlopeData_PropertyChanged;
                _MyDragSlopeData = value;
                MyDragSlopeData.PropertyChanged += MyDragSlopeData_PropertyChanged;
                RaisePropertyChanged(nameof(MyDragSlopeData));
            }
        }
        #endregion

        #region "Constructor"
        public LoadOut()
        {
            _ID = Guid.NewGuid().ToString();
            _SelectedGun = new Gun();
            SelectedCartridge = new Recipe();
            _zeroData = new ZeroData();
            _MyDragSlopeData = new DragSlopeData();
            SelectedGun.PropertyChanged += SelectedGun_PropertyChanged;
            SelectedCartridge.PropertyChanged += SelectedLoadRecipe_PropertyChanged;
            zeroData.PropertyChanged += zeroData_PropertyChanged;
            MyDragSlopeData.PropertyChanged += MyDragSlopeData_PropertyChanged;
        }
        #endregion

        #region "Destructor"
        ~LoadOut()
        {
            SelectedGun.PropertyChanged -= SelectedGun_PropertyChanged;
            SelectedCartridge.PropertyChanged -= SelectedLoadRecipe_PropertyChanged;
            zeroData.PropertyChanged -= zeroData_PropertyChanged;
            MyDragSlopeData.PropertyChanged -= MyDragSlopeData_PropertyChanged;
        }
        #endregion

        #region "Public Routines"
        public void SetID(string IDvalue)
        {
            _ID = IDvalue;
        }
        #endregion
    }
}
