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
        private Gun _MyGun;
        private Recipe _MyCartridge;
        private BallisticData _MyScenario = new BallisticData();
        #endregion

        #region "Properties"
        /// <summary>
        /// Measured from centerline of scope to centerline of barrel.
        /// </summary>
        public Gun MyGun
        {
            get
            {                
                return _MyGun;
            }
            set
            {
                _MyGun = value;
                RaisePropertyChanged(nameof(MyGun));
            }
        }
        public Recipe MyCartridge
        {
            get
            {
                return _MyCartridge;
            }
            set
            {
                _MyCartridge = value;
                RaisePropertyChanged(nameof(MyCartridge));
            }
        }
        public BallisticData MyScenario { get { return _MyScenario; } set { _MyScenario = value; RaisePropertyChanged(nameof(MyScenario)); } }
        #endregion

    }
}
