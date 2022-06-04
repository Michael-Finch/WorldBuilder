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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace WorldBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Random number generator
        Random rnd = new Random();

        //Variables describing the kingdom
        string kingdomName = "Kingdom Name";
        int physicalArea = 150000;
        int populationDensity = 80;
        int kingdomAge = 500;
        int percentArable = 45;

        public MainWindow()
        {
            InitializeComponent();
        }

        //Update kingdom name when the text is changed
        private void txtKingdomName_TextChanged(object sender, TextChangedEventArgs e)
        {
            kingdomName = txtKingdomName.Text;
            lblDisplayKingdomName.Content = kingdomName;
        }

        //Ensure certain textboxes only accept numerical input
        private void txtNumerical_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Update population density and arable land when the combobox is changed
        private void cmbPopulationDensity_DropDownClosed(object sender, EventArgs e)
        {
            //Get the selected population density
            ComboBoxItem typeItem = (ComboBoxItem)cmbPopulationDensity.SelectedItem;
            string densitySelection = typeItem.Content.ToString();

            //Update the population density and determine an appropriate percentage of arable land
            if (densitySelection.Equals("Desolate"))
            {
                populationDensity = 20;
                percentArable = rnd.Next(11, 21);
            }
            else if (densitySelection.Equals("Low"))
            {
                populationDensity = 40;
                percentArable = rnd.Next(21, 31);
            }
            else if (densitySelection.Equals("Settled"))
            {
                populationDensity = 60;
                percentArable = rnd.Next(31, 44);
            }
            else if (densitySelection.Equals("Average"))
            {
                populationDensity = 80;
                percentArable = rnd.Next(44, 55);
            }
            else if (densitySelection.Equals("High"))
            {
                populationDensity = 100;
                percentArable = rnd.Next(55, 66);
            }
            else if (densitySelection.Equals("Maximum"))
            {
                populationDensity = 120;
                percentArable = rnd.Next(66, 76);
            }

            //Update label
            lblDisplayPopulationDensity.Content = "(" + populationDensity.ToString() + " persons per sq. mile)";
        }

        //Do calculations for kingdom's physical area and display information
        private void kingdomCalculatePhysicalArea()
        {
            double arableLand = physicalArea * percentArable / 100;
            double wilderness = physicalArea - arableLand;

            string physicalAreaString = kingdomName + " covers an area of " + physicalArea.ToString() + " square miles. Of this, " +
                                        arableLand.ToString() + "% (" + arableLand.ToString() + " square miles) is arable land, and " +
                                        (100 - percentArable).ToString() + "% (" + wilderness.ToString() + " square miles) is wilderness.";

            txtblockOutputPhysicalArea.Text = physicalAreaString;
        }
    }
}
