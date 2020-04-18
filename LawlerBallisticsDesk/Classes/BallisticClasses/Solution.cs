using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes.BallisticClasses
{
    public class Solution : INotifyPropertyChanged
    {
        #region "Binding"
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void MyScenario_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            switch (e.PropertyName)
            {
                case "Message":
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region "Private Variables"
        private Scenario _MyScenario;
        private Ballistics _MyBallistics;
        #endregion

        #region "Properties"
        public Scenario MyScenario
        {
            get
            {
                return _MyScenario;
            }
            set
            {
                MyScenario.PropertyChanged -= MyScenario_PropertyChanged;
                _MyScenario = value;
                MyScenario.PropertyChanged += MyScenario_PropertyChanged;
                RaisePropertyChanged(nameof(MyScenario)); 
            }
        }
        public Ballistics MyBallistics { get { return _MyBallistics; } set { _MyBallistics = value; RaisePropertyChanged(nameof(MyBallistics)); } }
        #endregion

        #region "Constructor"
        public Solution()
        {
            _MyScenario = new Scenario();
            MyScenario.PropertyChanged += MyScenario_PropertyChanged;
            MyBallistics = new Ballistics(MyScenario);
        }
        #endregion

        #region "Destructor"
        ~Solution()
        {
            MyScenario.PropertyChanged -= MyScenario_PropertyChanged;
        }
        #endregion
    }
}
