using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes.BallisticClasses
{
    public class LoadOutData : INotifyPropertyChanged
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
        private double _ScopeHeight;
        private double _MuzzleVelocity;
        #endregion

        #region "Properties"
        /// <summary>
        /// Measured from centerline of scope to centerline of barrel.
        /// </summary>
        public double ScopeHeight
        {
            get
            {
                if (_ScopeHeight == 0) _ScopeHeight = 2.50;
                return _ScopeHeight;
            }
            set
            {
                _ScopeHeight = value;
                RaisePropertyChanged(nameof(ScopeHeight));
            }
        }
        public double MuzzleVelocity
        {
            get
            {
                return _MuzzleVelocity;
            }
            set
            {
                _MuzzleVelocity = value;
                RaisePropertyChanged(nameof(MuzzleVelocity));
            }
        }
        /// <summary>
        /// Maximum achievable distance (yards)
        /// </summary>
        public double MaximumRange
        {
            get
            {
                return MaxRange();
            }
        }
        #endregion

    }
}
