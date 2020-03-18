using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class Case : INotifyPropertyChanged
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
        private string _Manufacturer;
        private string _Model;
        private string _CartridgeID;
        private string _CartridgeName;
        private PrimerType _PrimerSize;
        #endregion

        #region "Properties"
        public string ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                _Name = LawlerBallisticsFactory.GetCartridgeName(_ID);
                RaisePropertyChanged(nameof(ID));
                RaisePropertyChanged(nameof(Name));
            }
        }
        public string Name { get { return _Name; } set { _Name = value; RaisePropertyChanged(nameof(Name)); } }
        public string Manufacturer { get { return _Manufacturer; } set { _Manufacturer = value; RaisePropertyChanged(nameof(Manufacturer)); } }
        public string Model { get { return _Model; } set { _Model = value; RaisePropertyChanged(nameof(Model)); } }
        public string CartridgeID { get { return _CartridgeID; } set { _CartridgeID = value; RaisePropertyChanged(nameof(CartridgeID)); } }
        public string CartridgeName
        {
            get { return _CartridgeName; }
            set
            {
                _CartridgeName = value;
                CartridgeID = LawlerBallisticsFactory.GetCartridgeID(_CartridgeName);
                RaisePropertyChanged(nameof(CartridgeName));
                RaisePropertyChanged(nameof(ID));
            }
        }
        public string PrimerSize 
        {
            get {return _PrimerSize.ToString(); }
            set 
            {
                if(value!="") _PrimerSize = (PrimerType) Enum.Parse(typeof(PrimerType), value);
                RaisePropertyChanged(nameof(PrimerSize));

            }
        }
        #endregion

        #region "Constructor"
        public Case()
        {
            ID = Guid.NewGuid().ToString();
            Name = "";
            Manufacturer = "";
            Model = "";
            CartridgeID = "";
            CartridgeName = "";
            PrimerSize = "";
        }
        #endregion
    }
}
