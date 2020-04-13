using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes.BallisticClasses
{
    public class BallisticData : INotifyPropertyChanged
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
        private Atmospherics _atmospherics = new Atmospherics();
       // private ZeroData _zeroData;
        private DragSlopeData _dragSlopwData;
        #endregion

        #region "Properties"
       // public ZeroData zeroData { get { return _zeroData; } set { _zeroData = value; RaisePropertyChanged(nameof(zeroData)); } }
        public DragSlopeData dragSlopeData { get { return _dragSlopwData; } set { _dragSlopwData = value; RaisePropertyChanged(nameof(dragSlopeData)); } }
        #endregion

        #region "Constructor"
        public BallisticData(Atmospherics atmospherics)
        {
            _atmospherics = atmospherics;
        }
        #endregion
    }
}
