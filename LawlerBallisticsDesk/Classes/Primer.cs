using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class Primer : INotifyPropertyChanged
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
        private string _ID="";
        private string _Name="";
        private string _Manufacturer="";
        private string _Model="";
        private PrimerType _Type;
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
        public string Type
        {
            get
            { return _Type.ToString(); }
            set
            {
                _Type = (PrimerType)Enum.Parse(typeof(PrimerType), value);
                RaisePropertyChanged(nameof(Type));
            }
        }
        #endregion

        #region "Constructor"
        public Primer()
        {
            ID = Guid.NewGuid().ToString();
        }
        #endregion

    }

}
