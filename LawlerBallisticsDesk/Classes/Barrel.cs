using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LawlerBallisticsDesk.Classes
{
    public class Barrel : INotifyPropertyChanged
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
        private string _Name;
        private string _Make;
        private string _Model;
        private string _Desc;
        private double _Twist;
        private TwistDirection _TwistDirection;
        private string _CartridgeID;
        private double _NeckDepth;
        private double _NeckDiameter;
        private double _HeadSpace;
        private double _Length;
        private Cartridge _Cartridge;
        private ObservableCollection<Recipe> _Recipes;
        private Recipe _SelectedRecipe;
        #endregion

        #region "Properties"
        public string ID { get { return _ID; } set { _ID = value; RaisePropertyChanged(nameof(ID)); } }
        public string Name { get { return _Name; } set { _Name = value; RaisePropertyChanged(nameof(Name)); } }
        public string Make { get { return _Make; } set { _Make = value; RaisePropertyChanged(nameof(Make)); } }
        public string Model { get { return _Model; } set { _Model = value; RaisePropertyChanged(nameof(Model)); } }
        public string Description { get { return _Desc; } set { _Desc = value; RaisePropertyChanged(nameof(Description)); } }
        public double Twist { get { return _Twist; } set { _Twist = value; RaisePropertyChanged(nameof(Twist)); } }
        public string RiflingTwistDirection
        {
            get{return _TwistDirection.ToString(); } 
            set
            {
                _TwistDirection = (TwistDirection) Enum.Parse(typeof(TwistDirection), value);
                RaisePropertyChanged(nameof(RiflingTwistDirection));
            }
        }
        public string CartridgeID { get { return _CartridgeID; } set { _CartridgeID = value; RaisePropertyChanged(nameof(CartridgeID)); } }
        public double NeckDepth { get { return _NeckDepth; } set { _NeckDepth = value; RaisePropertyChanged(nameof(NeckDepth)); } }
        public double NeckDiameter { get { return _NeckDiameter; } set { _NeckDiameter = value; RaisePropertyChanged(nameof(NeckDiameter)); } }
        public double HeadSpace { get { return _HeadSpace; } set { _HeadSpace = value; RaisePropertyChanged(nameof(HeadSpace)); } }
        public double Length { get { return _Length; } set { _Length = value; RaisePropertyChanged(nameof(Length)); } }
        public Cartridge ParentCartridge { get { return _Cartridge; } set { _Cartridge = value; RaisePropertyChanged(nameof(ParentCartridge)); } }
        public ObservableCollection<Recipe> Recipes { get { return _Recipes; } set { _Recipes = value; RaisePropertyChanged(nameof(Recipes)); } }
        public Recipe SelectedRecipe { get { return _SelectedRecipe; } set { _SelectedRecipe = value; RaisePropertyChanged(nameof(SelectedRecipe)); } }
        public string[] RecipeList { get { return GetRecipeList(); } }
        #endregion

        #region "Constructor"
        public Barrel()
        {
            _ID = Guid.NewGuid().ToString();
            _CartridgeID = "";
            _Desc = "";
            _Make = "";
            _Model = "";
            _Name = "";
            _TwistDirection = TwistDirection.Right;
        }
        #endregion

        #region "Private Routines"

        private string[] GetRecipeList()
        {
            string[] lRTN = new string[Recipes.Count];
            int lcnt = 0;

            foreach(Recipe ln in Recipes)
            {
                lRTN[lcnt] = ln.Name;
                lcnt++;
            }
            return lRTN;
        }
        #endregion
    }
}
