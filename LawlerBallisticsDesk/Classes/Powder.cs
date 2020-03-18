using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class Powder : INotifyPropertyChanged
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
        private string _Manufacturer;
        private string _Model;
        private string _Name;
        private PowderBaseType _Type;
        #endregion

        #region "Properties"
        public string ID { get { return _ID; } set { _ID = value; RaisePropertyChanged(nameof(ID)); } }
        public string Name { get { return _Name; } set { _Name = value; RaisePropertyChanged(nameof(Name)); } }
        public string Manufacturer { get { return _Manufacturer; } set { _Manufacturer = value; RaisePropertyChanged(nameof(Manufacturer)); } }
        public string Model { get { return _Model; } set { _Model = value; RaisePropertyChanged(nameof(Model)); } }
        public string BaseType
        {
            get{return _Type.ToString();}
            set
            {
                _Type = (PowderBaseType)Enum.Parse(typeof(PowderBaseType), value);
                RaisePropertyChanged(nameof(BaseType));
            }
        }
        #endregion

        #region "Constructor"
        public Powder()
        {
            _ID = Guid.NewGuid().ToString();
            _Manufacturer = "";
            _Model = "";
            _Name = "";
        }
        #endregion
    }
}
