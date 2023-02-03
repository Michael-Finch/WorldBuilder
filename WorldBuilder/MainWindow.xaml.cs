using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.IO;

namespace WorldBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //----------------------------------------------GENERAL----------------------------------------------
        //Kingdom, Settlement, and World objects
        Kingdom kingdom = new Kingdom();
        Settlement settlement = new Settlement();
        World world = new World();

        //Create the main window
        public MainWindow()
        {
            InitializeComponent();

            //Populate window controls with reasonable defaults
            txtKingdomName.Text = kingdom.Name;
            txtPhysicalArea.Text = kingdom.PhysicalArea.ToString();
            cmbPopulationDensity.SelectedIndex = 3;
            txtKingdomAge.Text = kingdom.Age.ToString();

            txtSettlementName.Text = settlement.Name;
            txtPopulation.Text = settlement.Population.ToString();
        }

        //Method to ensure certain textboxes only accept numeric input
        private void txtEnsureNumeric(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //----------------------------------------------KINGDOM TAB----------------------------------------------
        //Update output as kindgom name is changed
        private void txtKingdomName_TextChanged(object sender, TextChangedEventArgs e)
        {
            kingdom.Name = txtKingdomName.Text;
            lblDisplayKingdomName.Content = kingdom.Name;

            //Update output
            updateKingdomOutput();
        }

        //Update output as physical area is changed
        private void txtPhysicalArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if (Int32.TryParse(txtPhysicalArea.Text, out result))
            {
                kingdom.PhysicalArea = result;
            }

            //Update output
            updateKingdomOutput();
        }

        //Update output as kingdom age is changed
        private void txtKingdomAge_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if (Int32.TryParse(txtKingdomAge.Text, out result))
            {
                kingdom.Age = result;
            }

            //Update output
            updateKingdomOutput();
        }

        //Update output when population density is changed
        private void cmbPopulationDensity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get the selected population density
            ComboBoxItem typeItem = (ComboBoxItem)cmbPopulationDensity.SelectedItem;
            string densitySelection = typeItem.Content.ToString();

            //Update the population density and determine an appropriate percentage of arable land
            kingdom.setDensity(densitySelection);

            //Update label
            lblDisplayPopulationDensity.Content = "(" + kingdom.PopulationDensity.ToString() + " persons per sq. mile)";

            //Update output
            updateKingdomOutput();
        }

        //Update output text blocks
        private void updateKingdomOutput()
        {
            txtblockOutputPhysicalArea.Text = kingdom.calculatePhysicalArea();
            txtblockOutputPopulation.Text = kingdom.calculateTotalPopulation();
            txtblockOutputSettlements.Text = kingdom.calculateSettlements();
            txtblockOutputCastles.Text = kingdom.calculateCastles();
        }

        //----------------------------------------------SETTLEMENT TAB----------------------------------------------
        //Update output as settlement name is changed
        private void txtSettlementName_TextChanged(object sender, TextChangedEventArgs e)
        {
            settlement.Name = txtSettlementName.Text;
            lblDisplaySettlementName.Content = settlement.Name;

            //Update output
            updateSettlementOutput();
        }

        //Update output as population is changed
        private void txtPopulation_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if (Int32.TryParse(txtPopulation.Text, out result))
            {
                settlement.Population = result;
            }

            //Update output
            updateSettlementOutput();
        }

        //Update output text blocks
        private void updateSettlementOutput()
        {
            txtblockOutputSize.Text = settlement.calculateSettlementSize();
            txtblockOutputTrades.Text = settlement.calculateSettlementTrades();
            txtblockOutputMisc.Text = settlement.calculateSettlementMisc();
        }

        //----------------------------------------------WORLD TAB----------------------------------------------
        WriteableBitmap wb;

        //Update map size when text input is changed
        private void txtWorldSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if (Int32.TryParse(txtWorldSize.Text, out result))
            {
                world.resize(result);
            }
        }

        //Update octaves when text input is changed
        private void txtOctaves_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if (Int32.TryParse(txtOctaves.Text, out result))
            {
                world.Octaves = result;
            }
        }

        //Update cell size when text input is changed
        private void txtCellSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if (Int32.TryParse(txtCellSize.Text, out result))
            {
                world.resizeCells(result);
            }
        }

        //Update amplitude when slider is changed
        private void slAmplitude_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            world.Amplitude = (float)slAmplitude.Value;
        }

        //Update persistence when slider is changed
        private void slPersistence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            world.Persistence = (float)slPersistence.Value;
        }

        //Generate a new world and draw it
        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            //Pick what type of border the map has
            if (radioBorderNone.IsChecked == true)
            {
                world.generateWorld("none");
            }
            else if (radioBorderSquare.IsChecked == true)
            {
                world.generateWorld("square");
            }
            else
            {
                world.generateWorld("circle");
            }

            //Generate the world and update the image in the window
            wb = world.generateImage();
            image.Width = world.imageWidth;
            image.Height = world.imageHeight;
            image.Source = wb;
        }

        //Save the currently drawn world
        private void btnSaveMap_Click(object sender, RoutedEventArgs e)
        {
            //Create a save file dialog
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "World"; //Default file name
            dlg.DefaultExt = ".png"; //Default file extension
            dlg.Filter = "PNG |*.png"; //Filter files by extension

            //Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            //Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(wb.Clone())); //Use a clone of the image control as the source of data
                    encoder.Save(stream);
                }
            }
        }
    }
}