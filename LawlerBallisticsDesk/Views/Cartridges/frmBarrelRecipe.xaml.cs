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
    /// Interaction logic for frmBarrelRecipe.xaml
    /// </summary>
    public partial class frmBarrelRecipe : Window
    {
        public frmBarrelRecipe(BarrelRecipeViewModel DataSource)
        {
            this.DataContext = DataSource;
            InitializeComponent();
        }       

        public void SetTargetRecipe(string ID)
        {
            RecipeViewModel lDC;
            lDC = (RecipeViewModel) this.DataContext;
            lDC.SetLoadRecipe(ID);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            txtChrgWt.IsReadOnly = false;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            txtCBTO.IsReadOnly = false;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            txtCaseTrimLgth.IsReadOnly = false;
            txtCBTO.IsReadOnly = false;
            txtChrgWt.IsReadOnly = false;
            txtCOAL.IsReadOnly = false;
            txtHeadSpace.IsReadOnly = false;
            txtJump.IsReadOnly = false;
            txtName.IsReadOnly = false;
        }
    }
}
