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
using Microsoft.Maps.MapControl.WPF;


namespace LawlerBallisticsDesk.Views.Ballistics
{
    /// <summary>
    /// Interaction logic for frmBallisticCalculator.xaml
    /// </summary>
    public partial class frmBallisticCalculator : Window
    {
        public frmBallisticCalculator()
        {
            InitializeComponent();

            SolutionViewModel lDC = (SolutionViewModel) this.DataContext;
            lDC.LoadDefaultSolution();
            chkMaxRise_Click(null, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.DataContext = null;
            GC.Collect();
        }

        private void chkMaxRise_Click(object sender, RoutedEventArgs e)
        {
            var lconverter = new System.Windows.Media.BrushConverter();
            Brush lbg = (Brush)lconverter.ConvertFromString("#FFEAEAEA");
            Brush lbg1 = (Brush)lconverter.ConvertFromString("#FFFFFFFF");

            if ((bool)chkMaxRise.IsChecked)
            {
                txtZeroR.Background = lbg;
                txtZeroR.IsReadOnly = true;
                txtMaxRise.Background = lbg1;
                txtMaxRise.IsReadOnly = false;
            }
            else
            {
                txtZeroR.Background = lbg1;
                txtZeroR.IsReadOnly = false;
                txtMaxRise.Background = lbg;
                txtMaxRise.IsReadOnly = true;
            }
        }

        #region "Scenario"
        Pushpin TargetLoc;
        Pushpin ShooterLoc;

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Determin the location to place the pushpin at on the map.

            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(ScenarioMap);

            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = ScenarioMap.ViewportPointToLocation(mousePosition);

            // The pushpin to add to the map.
            bool shooter = false;
            bool target = false;
            foreach (Pushpin lpp in ScenarioMap.Children)
            {
                if (lpp.Name == "Shooter")
                {
                    shooter = true;
                }
                if (lpp.Name == "Target")
                {
                    target = true;
                }
            }
            if (!shooter)
            {
                ShooterLoc = new Pushpin();
                ShooterLoc.Location = pinLocation;
                ShooterLoc.Name = "Shooter";
                ShooterLoc.Content = "Shooter";
                // Adds the pushpin to the map.
                ScenarioMap.Children.Add(ShooterLoc);

            }
            else if (!target)
            {
                TargetLoc = new Pushpin();
                TargetLoc.Location = pinLocation;
                TargetLoc.Name = "Target";
                TargetLoc.Content = "Target";
                // Adds the pushpin to the map.
                ScenarioMap.Children.Add(TargetLoc);
            }

        }

        #endregion
    }
}
