using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LawlerBallisticsDesk.Classes
{
    public class Gun : INotifyPropertyChanged
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
        private string _Name;
        private string _Desc;
        private string _Make;
        private string _Model;
        private Barrel _SelectedBarrel;
        private Barrel _TargetBarrel;
        private ObservableCollection<Barrel> _Barrels;
        private Image _GunPic;
        private string _ID;
        #endregion

        #region "Properties"
        public string Name { get { return _Name; } set { _Name = value; RaisePropertyChanged(nameof(Name)); } }
        public string Make { get { return _Make; } set { _Make = value; RaisePropertyChanged(nameof(Make)); } }
        public string Model { get { return _Model; } set { _Model = value; RaisePropertyChanged(nameof(Model)); } }
        public string Description { get { return _Desc; } set { _Desc = value; RaisePropertyChanged(nameof(Description)); } }
        public Barrel SelectedBarrel { get { return _SelectedBarrel; } set { _SelectedBarrel = value; RaisePropertyChanged(nameof(SelectedBarrel)); } }
        public Barrel TargetBarrel { get { return _TargetBarrel; } set { _TargetBarrel = value; RaisePropertyChanged(nameof(TargetBarrel)); } }
        public ObservableCollection<Barrel> Barrels { get { return _Barrels; } set { _Barrels = value; RaisePropertyChanged(nameof(Barrels)); } }
        public Image GunPic
        {
            get { return _GunPic; }
            set
            {
                _GunPic = value;
                RaisePropertyChanged(nameof(GunPic));
            }
        }
        public string ID { get { return _ID; } set { _ID = value; RaisePropertyChanged(nameof(ID)); } }
        public ObservableCollection<Cartridge> CatridgeList { get { return LawlerBallisticsFactory.MyCartridges; } }
        #endregion

        #region "Constructor"
        public Gun()
        {
            ID = Guid.NewGuid().ToString();
            _Desc = "";
            _Make = "";
            _Model = "";
            _Name = "";
            Barrels = new ObservableCollection<Barrel>();
        }
        #endregion
    }
}
