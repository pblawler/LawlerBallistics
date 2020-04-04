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
    /// Interaction logic for uctrlLotPerformance.xaml
    /// </summary>
    public partial class uctrlLotPerformance : UserControl
    {
        private RecipeViewModel _DC;

        public uctrlLotPerformance()
        {
            InitializeComponent();
            _DC = (RecipeViewModel)this.DataContext;

        }

        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            dgdStat.ItemsSource = null;
            dgdStat.ItemsSource = _DC.SelectedRecipe.SelectedLot.LotStats;
        }
    }
}
