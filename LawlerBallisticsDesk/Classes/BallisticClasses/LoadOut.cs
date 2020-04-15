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
            RaisePropertyChanged(nameof(SelectedGun));
        }
        private void SelectedLoadRecipe_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(SelectedCartridge));
        }
        #endregion

        #region "Private Variables"
        private string _ID;
        private Gun _SelectedGun;
        private Recipe _SelectedLoadRecipe;
        private string _BarrelID;
        private Barrel _Barrel;
        private double _ScopeHeight;
        private double _MuzzleVelocity;
        private double _BSG;
        private double _MaxRange;
        private ZeroData _zeroData;
        #endregion

        #region "Properties"
        public string ID { get { return _ID; } }
        public Gun SelectedGun { get { return _SelectedGun; } set { _SelectedGun = value;  } }
        public string SelectedBarrelID { get { return _BarrelID; } set { _BarrelID = value; RaisePropertyChanged(nameof(SelectedBarrelID)); } }
        public Barrel SelectedBarrel { get { return _Barrel; } }
        public Recipe SelectedCartridge { get { return _SelectedLoadRecipe; } set { _SelectedLoadRecipe = value; RaisePropertyChanged(nameof(SelectedCartridge)); } }
        public double ScopeHeight
        {
            get
            {
                //TODO: add the ability for the scope to be mounted on the barrel or reciever.
                return SelectedGun.ScopeHeight;
            }
        }
        public double MuzzleVelocity { get { return _MuzzleVelocity; } set { _MuzzleVelocity = value; RaisePropertyChanged(nameof(MuzzleVelocity)); } }
        /// <summary>
        /// Bullet stability factor
        /// </summary>
        public double BSG { get { return _BSG; } set { _BSG = value; RaisePropertyChanged(nameof(BSG)); } }
        public double MaxRange { get { return _MaxRange; } set { _MaxRange = value; RaisePropertyChanged(nameof(MaxRange)); } }
        public ZeroData zeroData { get { return _zeroData; } set { _zeroData = value; RaisePropertyChanged(nameof(zeroData)); } }
        #endregion

        #region "Constructor"
        public LoadOut()
        {
            _ID = Guid.NewGuid().ToString();
            SelectedGun = new Gun();
            SelectedCartridge = new Recipe();
            zeroData = new ZeroData();
            SelectedGun.PropertyChanged += SelectedGun_PropertyChanged;
            SelectedCartridge.PropertyChanged += SelectedLoadRecipe_PropertyChanged;
        }
        #endregion

        #region "Destructor"
        ~LoadOut()
        {
            SelectedGun.PropertyChanged -= SelectedGun_PropertyChanged;
            SelectedCartridge.PropertyChanged -= SelectedLoadRecipe_PropertyChanged;

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
