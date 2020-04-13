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
        #endregion

        #region "Private Variables"
        private Scenario _MyScenario;
        private Ballistics _MyBallistics;
        #endregion

        #region "Properties"
        public Scenario MyScenario { get { return _MyScenario; } set { _MyScenario = value; RaisePropertyChanged(nameof(MyScenario)); } }
        public Ballistics MyBallistics { get { return _MyBallistics; } set { _MyBallistics = value; RaisePropertyChanged(nameof(MyBallistics)); } }
        #endregion

        #region "Creator"
        public Solution()
        {
            MyScenario = new Scenario();
            MyBallistics = new Ballistics(MyScenario);
        }
        #endregion
    }
}
