using LawlerBallisticsDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LawlerBallisticsDesk.Views.Cartridges
{
    /// <summary>
    /// Interaction logic for uctrlRecipes.xaml
    /// </summary>
    public partial class uctrlBarrelRecipes : UserControl
    {
        BarrelRecipeViewModel _DC;

        public uctrlBarrelRecipes()
        {
            InitializeComponent();
        }

        public void SetViewModelRef(string BarrelID)
        {
            _DC = new BarrelRecipeViewModel(BarrelID);
            this.DataContext = _DC;
        }
    }
}
