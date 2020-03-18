using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class Bullet : INotifyPropertyChanged
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
        private string _Mfgr;
        private string _Model;
        private double _BCg1;
        private double _BCg7;
        private double _Diameter;
        private double _Length;
        private double _BTO;
        private double _Weight;
        private BulletShapeEnum _type;
        #endregion

        #region "Properties"
        public string ID { get { return _ID; } set { _ID = value; RaisePropertyChanged(nameof(ID)); } }
        public string Manufacturer { get { return _Mfgr; } set { _Mfgr = value; RaisePropertyChanged(nameof(Manufacturer)); } }
        public string Model { get { return _Model; } set { _Model = value; RaisePropertyChanged(nameof(Model)); } }
        public double BCg1 { get { return _BCg1; } set { _BCg1 = value; RaisePropertyChanged(nameof(BCg1)); } }
        public double BCg7 { get { return _BCg7; } set { _BCg7 = value; RaisePropertyChanged(nameof(BCg7)); } }
        public double Diameter { get { return _Diameter; } set { _Diameter = value; RaisePropertyChanged(nameof(Diameter)); } }
        public double Length { get { return _Length; } set { _Length = value; RaisePropertyChanged(nameof(Length)); } }
        public double BTO { get { return _BTO; } set { _BTO = value; RaisePropertyChanged(nameof(BTO)); } }
        public double Weight { get { return _Weight; } set { _Weight = value; RaisePropertyChanged(nameof(Weight)); } }
        public string Type
        { 
            get
            { return _type.ToString(); }
            set
            {
                _type = (BulletShapeEnum)Enum.Parse(typeof(BulletShapeEnum), value);
                RaisePropertyChanged(nameof(Type));
            }
        }
        #endregion

        #region "Constructor"
        public Bullet()
        {
            _ID = Guid.NewGuid().ToString();
            _Mfgr = "";
            _Model = "";
        }
        #endregion
    }
}
