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
    /// Interaction logic for frmLoadRecipe.xaml
    /// </summary>
    public partial class frmLoadRecipe : Window
    {
        public frmLoadRecipe(bool IsNew, bool IsBarrelSpecific)
        {
            
            InitializeComponent();
            if(IsNew)
            {
                txtChrgWt.IsReadOnly = false;
            }
            else
            {
                txtChrgWt.IsReadOnly = true;
            }
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
    }
}
