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
            RaisePropertyChanged(nameof(SelectedLoadRecipe));
        }
        #endregion

        #region "Private Variables"
        private Gun _SelectedGun;
        private Recipe _SelectedLoadRecipe;
        private string _SelectedBarrelID;
        private string _SelectedLoadRecipeID;
        private double _CoriolisHZeroRate;
        private double _CoriolisVZeroRate;
        private double _SpinDriftZeroRate;
        private double _Hm;  //Max rise to zero
        private double _HmRange;
        private double _ZeroRange;
        private double _NearZero;


        #endregion

        #region "Properties"
        public Gun SelectedGun { get { return _SelectedGun; } set { _SelectedGun = value;  } }
        public Recipe SelectedLoadRecipe { get { return _SelectedLoadRecipe; } set { _SelectedLoadRecipe = value; RaisePropertyChanged(nameof(SelectedLoadRecipe)); } }
        #endregion

        #region "Constructor"
        public LoadOut()
        {
            SelectedGun.PropertyChanged += SelectedGun_PropertyChanged;
            SelectedLoadRecipe.PropertyChanged += SelectedLoadRecipe_PropertyChanged;
        }
        #endregion

        #region "Destructor"
        ~LoadOut()
        {
            SelectedGun.PropertyChanged -= SelectedGun_PropertyChanged;
            SelectedLoadRecipe.PropertyChanged -= SelectedLoadRecipe_PropertyChanged;

        }
        #endregion
    }
}
