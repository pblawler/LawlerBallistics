using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class Round : INotifyPropertyChanged
    {
        #region "Binding"
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            SendPropertyChangedMsg(name);
        }
        #endregion

        #region "Messaging"
        private void SendPropertyChangedMsg(string name)
        {
            var msg = new PropertyChangedMsg() { Sender = "Round", PropName = name, Msg="" };
            Messenger.Default.Send<PropertyChangedMsg>(msg);
        }
        #endregion

        #region "Private Variables"
        private bool _SetStat = false;
        private string _ID;
        private string _name;
        private Int32 _RndNo;
        private double _BBTO;   //Bullet Base to Ogive
        private double _BL;     //Bullet Length
        private double _BD;     //Bullet Diameter
        private double _BW;     //Bullet Weight
        private double _CW;     //Case Weight
        private double _CVW;    //Case Volume Weight
        private double _CL;     //Case Length
        private double _CHS;    //Case Head Space
        private double _CNOD;   //Case Neck Outter Diameter after sizing
        private double _CNID;   //Case Neck Inner Diameter after sizing
        private double _PCW;    //Powder Charge Weight
        private double _Crimp;  //Neck crimp on bullet
        private double _CBTO;   //Cartridge Base to Ogive
        private double _COAL;   //Cartridge Overall Length
        private double _MV;     //Muzzle Velocity
        private double _VD;     //Vertical Deviation
        private double _HD;     //Horizontal Deviation
        private double _VAD;    //Vertical Adjusted Deviation.   Individual vertical deviation minus group average vertical deviation.
        private double _HAD;    //Horizontal Adjusted Deviation.   Individual horizontal deviation minus group average horizontal deviation.
        private double _RMSD;   //Root Mean Square Deviation
        private double _GDV;    //Group Deviation Value (Shot location distance from group average + 1 SD). 
        private double _VELAD;  //Velocity Average Deviation
        #endregion

        #region "Properties"
        public string ID { get { return _ID; } set { _ID = value; RaisePropertyChanged(nameof(ID)); } }
        public string name { get { return _name; } set { _name = value; RaisePropertyChanged(nameof(name)); } }
        public Int32 RndNo { get { return _RndNo; } set { _RndNo = value; RaisePropertyChanged(nameof(RndNo)); } }
        /// <summary>
        /// Bullet Base to Ogive
        /// </summary>
        public double BBTO { get { return _BBTO; } set { _BBTO = value; RaisePropertyChanged(nameof(BBTO)); } }
        /// <summary>
        /// Bullet Length
        /// </summary>
        public double BL { get { return _BL; } set { _BL = value; RaisePropertyChanged(nameof(BL)); } }
        /// <summary>
        /// Bullet Diameter
        /// </summary>
        public double BD { get { return _BD; } set { _BD = value; RaisePropertyChanged(nameof(BD)); RaisePropertyChanged(nameof(Crimp)); } }
        /// <summary>
        /// Bullet Weight
        /// </summary>
        public double BW { get { return _BW; } set { _BW = value; RaisePropertyChanged(nameof(BW)); } }
        /// <summary>
        /// Case Weight
        /// </summary>
        public double CW { get { return _CW; } set { _CW = value; RaisePropertyChanged(nameof(CW)); } }
        /// <summary>
        /// Case Volume Weight
        /// </summary>
        public double CVW { get { return _CVW; } set { _CVW = value; RaisePropertyChanged(nameof(CVW)); } }
        /// <summary>
        /// Case Length
        /// </summary>
        public double CL { get { return _CL; } set { _CL = value; RaisePropertyChanged(nameof(CL)); } }
        /// <summary>
        /// Case Head Space
        /// </summary>
        public double CHS { get { return _CHS; } set { _CHS = value; RaisePropertyChanged(nameof(CHS)); } }
        /// <summary>
        /// Case Neck Outter Diameter after sizing
        /// </summary>
        public double CNOD { get { return _CNOD; } set { _CNOD = value; RaisePropertyChanged(nameof(CNOD)); } }
        /// <summary>
        /// Case Neck Inner Diameter after sizing
        /// </summary>
        public double CNID { get { return _CNID; } set { _CNID = value; RaisePropertyChanged(nameof(CNID)); RaisePropertyChanged(nameof(Crimp)); } }
        /// <summary>
        /// Powder Charge Weight
        /// </summary>
        public double PCW { get { return _PCW; } set { _PCW = value; RaisePropertyChanged(nameof(PCW)); } }
        /// <summary>
        /// Neck crimp on bullet
        /// </summary>
        public double Crimp { get {if(!_SetStat) _Crimp = _BD - CNID; _SetStat = false; return _Crimp; } }
        /// <summary>
        /// Cartridge Base to Ogive
        /// </summary>
        public double CBTO { get { return _CBTO; } set { _CBTO = value; RaisePropertyChanged(nameof(CBTO)); } }
        /// <summary>
        /// Cartridge Overall Length
        /// </summary>
        public double COAL { get { return _COAL; } set { _COAL = value; RaisePropertyChanged(nameof(COAL)); } }
        /// <summary>
        /// Muzzle Velocity
        /// </summary>
        public double MV { get { return _MV; } set { _MV = value; RaisePropertyChanged(nameof(MV)); } }
        /// <summary>
        /// Vertical Deviation
        /// </summary>
        public double VD { get { return _VD; } set { _VD = value; RaisePropertyChanged(nameof(VD)); } }
        /// <summary>
        /// Horizontal Deviation
        /// </summary>
        public double HD { get { return _HD; } set { _HD = value; RaisePropertyChanged(nameof(HD)); } }
        /// <summary>
        /// Vertical Adjusted Deviation.   Individual vertical deviation minus group average vertical deviation.
        /// </summary>
        public double VAD { get { return _VAD; } set { _VAD = value; RaisePropertyChanged(nameof(VAD)); } }
        /// <summary>
        /// Horizontal Adjusted Deviation.   Individual horizontal deviation minus group average horizontal deviation.
        /// </summary>
        public double HAD { get { return _HAD; } set { _HAD = value; RaisePropertyChanged(nameof(HAD)); } }
        /// <summary>
        /// Root Mean Square Deviation
        /// </summary>
        public double RMSD { get { return _RMSD; } set { _RMSD = value; RaisePropertyChanged(nameof(RMSD)); } }
        /// <summary>
        /// Group Deviation Value (Shot location distance from group average + SensitivtyFactor * SD).
        /// </summary>
        public double GDV { get { return _GDV; } set { _GDV = value; RaisePropertyChanged(nameof(GDV)); } }
        /// <summary>
        /// Velocity Average Deviation
        /// </summary>
        public double VELAD { get { return _VELAD; } set { _VELAD = value; RaisePropertyChanged(nameof(VELAD)); } }
        #endregion

        #region "Constructor"
        public Round()
        {
            ID = Guid.NewGuid().ToString(); 
        }
        #endregion

        #region "Public Routines"
        public void SilentSet(string PropName, object value)
        {
            switch (PropName)
            {
                case "BBTO":
                    _BBTO = (double)value;
                    break;
                case "BW":
                    _BW = (double)value;
                    break;
                case "BL":
                    _BL = (double)value;
                    break;
                case "BD":
                    _BD = (double)value;
                    break;
                case "CW":
                    _CW = (double)value;
                    break;
                case "CVW":
                    _CVW = (double)value;
                    break;
                case "CL":
                    _CL = (double)value;
                    break;
                case "CHS":
                    _CHS = (double)value;
                    break;
                case "CNOD":
                    _CNOD = (double)value;
                    break;
                case "CNID":
                    _CNID = (double)value;
                    break;
                case "PCW":
                    _PCW = (double)value;
                    break;
                case "Crimp":
                    _SetStat = true;
                    _Crimp = (double)value;
                    break;
                case "CBTO":
                    _SetStat = true;
                    _CBTO = (double)value;
                    break;
                case "COAL":
                    _SetStat = true;
                    _COAL = (double)value;
                    break;
                case "MV":
                    _SetStat = true;
                    _MV = (double)value;
                    break;
                case "VD":
                    _SetStat = true;
                    _VD = (double)value;
                    break;
                case "HD":
                    _SetStat = true;
                    _HD = (double)value;
                    break;
                case "VAD":
                    _SetStat = true;
                    _VAD = (double)value;
                    break;
                case "HAD":
                    _SetStat = true;
                    _HAD = (double)value;
                    break;
                case "RMSD":
                    _SetStat = true;
                    _RMSD = (double)value;
                    break;
                case "GDV":
                    _SetStat = true;
                    _GDV = (double)value;
                    break;
                case "VELAD":
                    _SetStat = true;
                    _VELAD = (double)value;
                    break;
            }
        }
        #endregion
    }
}
