using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace LawlerBallisticsDesk.Classes
{
    public class Cartridge : INotifyPropertyChanged
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
        private double _BulletDiameter;
        private double _MaxCOAL; //Cartridge Overall length
        private double _MaxCaseLngth;
        private double _CaseTrimLngth;
        private string _ID;  //Guid for programmatic reference.
        private double _HeadSpaceMax;
        private double _HeadSpaceMin;
        private Image _CartridgePic;
        private List<string> _PowderIDlist;
        #endregion

        #region "Public Properties"
        public string Name
        { 
            get { return _Name; }
            set { _Name = value; RaisePropertyChanged(nameof(Name)); }
        }
        public double BulletDiameter { get { return _BulletDiameter; } set { _BulletDiameter = value; RaisePropertyChanged(nameof(BulletDiameter)); } }
        public double MaxCOAL { get { return _MaxCOAL; } set { _MaxCOAL = value; RaisePropertyChanged(nameof(MaxCOAL)); } }
        public double MaxCaseLngth { get { return _MaxCaseLngth; } set { _MaxCaseLngth = value; RaisePropertyChanged(nameof(MaxCaseLngth)); } }
        public double HeadSpaceMin { get { return _HeadSpaceMin; } set { _HeadSpaceMin = value; RaisePropertyChanged(nameof(HeadSpaceMin)); } }
        public double HeadSpaceMax { get { return _HeadSpaceMax; } set { _HeadSpaceMax = value; RaisePropertyChanged(nameof(HeadSpaceMax)); } }
        public double CaseTrimLngth { get { return _CaseTrimLngth; } set { _CaseTrimLngth = value; RaisePropertyChanged(nameof(CaseTrimLngth)); } }
        public string ID { get { return _ID; } set { _ID = value; RaisePropertyChanged(nameof(ID)); } }
        public Image CartridgePic
        {
            get { return _CartridgePic; }
            set
            {
                _CartridgePic = value;
                RaisePropertyChanged(nameof(CartridgePic));
            }
        }
        public List<string> PowderIDlist { get { return _PowderIDlist;} set { _PowderIDlist = value; RaisePropertyChanged(nameof(PowderIDlist)); } }
        #endregion

        #region "Public Routines"
        public void Refresh()
        {
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(BulletDiameter));
            RaisePropertyChanged(nameof(MaxCOAL));
            RaisePropertyChanged(nameof(MaxCaseLngth));
            RaisePropertyChanged(nameof(HeadSpaceMin));
            RaisePropertyChanged(nameof(HeadSpaceMax));
            RaisePropertyChanged(nameof(HeadSpaceMin));
            RaisePropertyChanged(nameof(CaseTrimLngth));
            RaisePropertyChanged(nameof(CartridgePic));
            RaisePropertyChanged(nameof(PowderIDlist));
        }

        #endregion

        #region "Constructor"
        public Cartridge()
        {
            _ID = Guid.NewGuid().ToString();
            _Name = "";
            _PowderIDlist = new List<string>();
            RaisePropertyChanged(nameof(ID));
        }
        #endregion

    }
}
