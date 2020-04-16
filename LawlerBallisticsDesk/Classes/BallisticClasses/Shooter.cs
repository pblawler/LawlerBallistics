using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class Shooter : INotifyPropertyChanged
    {
        #region "Binding"
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void MyLocation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(MyLocation));
        }
        private void MyLoadOut_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(MyLoadOut));
        }
        #endregion

        #region "Private Variables"
        private LocationData _MyLocation;
        private LoadOut _MyLoadOut;
        #endregion

        #region "Properties"
        public LocationData MyLocation { get { return _MyLocation; } set { _MyLocation = value; RaisePropertyChanged(nameof(MyLocation)); } }
        public LoadOut MyLoadOut { get { return _MyLoadOut; } set { _MyLoadOut = value; RaisePropertyChanged(nameof(MyLoadOut)); } }
        #endregion

        #region "Constructor"
        public Shooter()
        {
            MyLocation = new LocationData();
            MyLoadOut = new LoadOut();
            MyLocation.PropertyChanged += MyLocation_PropertyChanged;
            MyLoadOut.PropertyChanged += MyLoadOut_PropertyChanged;            
        }
        #endregion

        #region "Destructor"
        ~Shooter()
        {
            MyLocation.PropertyChanged -= MyLocation_PropertyChanged;
            MyLoadOut.PropertyChanged -= MyLoadOut_PropertyChanged;
        }
        #endregion
    }
}
