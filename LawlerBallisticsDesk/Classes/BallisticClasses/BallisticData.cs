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
        private Barrel _barrel;
        private Bullet _bullet;
        private ZeroData _zeroData;
        #endregion

        #region "Properties"
        public Barrel barrel { get { return _barrel; } set { _barrel = value; RaisePropertyChanged(nameof(barrel)); } }
        public Bullet bullet { get { return _bullet; } set { _bullet = value; RaisePropertyChanged(nameof(bullet)); } }
        public ZeroData zeroData { get { return _zeroData; } set { _zeroData = value; RaisePropertyChanged(nameof(zeroData)); } }
        #endregion
    }
}
