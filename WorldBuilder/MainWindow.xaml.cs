using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        Kingdom kingdom = new Kingdom();
        Settlement settlement = new Settlement();
        World world = new World();

        //Random number generator
        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();

            //Populate window controls with reasonable defaults
            txtKingdomName.Text = kingdom.Name;
            txtPhysicalArea.Text = kingdom.PhysicalArea.ToString();
            cmbPopulationDensity.SelectedIndex = 3;
            txtKingdomAge.Text = kingdom.Age.ToString();
        }

        //Ensure certain textboxes only accept numeric input
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
            if(Int32.TryParse(txtKingdomAge.Text, out result))
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
            if (densitySelection.Equals("Desolate"))
            {
                kingdom.PopulationDensity = 20;
                kingdom.PercentArable = rnd.Next(11, 21);
            }
            else if (densitySelection.Equals("Low"))
            {
                kingdom.PopulationDensity = 40;
                kingdom.PercentArable = rnd.Next(21, 31);
            }
            else if (densitySelection.Equals("Settled"))
            {
                kingdom.PopulationDensity = 60;
                kingdom.PercentArable = rnd.Next(31, 44);
            }
            else if (densitySelection.Equals("Average"))
            {
                kingdom.PopulationDensity = 80;
                kingdom.PercentArable = rnd.Next(44, 55);
            }
            else if (densitySelection.Equals("High"))
            {
                kingdom.PopulationDensity = 100;
                kingdom.PercentArable = rnd.Next(55, 66);
            }
            else if (densitySelection.Equals("Maximum"))
            {
                kingdom.PopulationDensity = 120;
                kingdom.PercentArable = rnd.Next(66, 76);
            }

            //Update label
            lblDisplayPopulationDensity.Content = "(" + kingdom.PopulationDensity.ToString() + " persons per sq. mile)";

            //Update output
            updateKingdomOutput();
        }

        //Update output text blocks
        private void updateKingdomOutput()
        {
            txtblockOutputPhysicalArea.Text = kingdom.CalculatePhysicalArea();
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
            if(Int32.TryParse(txtPopulation.Text, out result))
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

        //----------------------------------------------WORLD----------------------------------------------


        //World map Image variables
        public static int cellSize = 1; //How many pixels one cell in the grid is
        public static int stride = cellSize * (PixelFormats.Bgra32.BitsPerPixel / 8); //How many bytes are needed for one row of a cell, used for drawing
        static int imageWidth; //How many pixels wide the image is
        static int imageHeight; //How many pixels high the image is
        WriteableBitmap wb;

        //Update map size when text input is changed
        private void txtWorldSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if(Int32.TryParse(txtWorldSize.Text, out result))
            {
                world.Size = result;
                world.HeightMap = new float[world.Size, world.Size];
                world.MoistureMap = new float[world.Size, world.Size];
                world.BorderMatrix = new float[world.Size, world.Size];
            }
        }

        //Update octaves when text input is changed
        private void txtOctaves_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if(Int32.TryParse(txtOctaves.Text, out result))
            {
                world.Octaves = result;
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

        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            imageWidth = world.Size * cellSize;
            imageHeight = world.Size * cellSize;
            WriteableBitmap wb = new WriteableBitmap(imageWidth, imageHeight, 300, 300, PixelFormats.Bgra32, null);

            image.Width = imageWidth;
            image.Height = imageHeight;
            image.Source = wb;


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

            //Color each cell according to its height and moisture
            for (int x = 0; x < world.Size; x++)
            {
                for (int y = 0; y < world.Size; y++)
                {
                    //Use an Int32Rect to choose the rectangular region to edit
                    //xy of top left corner plus width and height of edited region
                    byte[] bytes = new byte[stride * cellSize];

                    //Color each cell based on its height and moisture

                    //Snowcap
                    if (world.HeightMap[x, y] >= world.snowCapMinHeight)
                    {
                        for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 255;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 255;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 255;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //High mountains
                    else if (world.HeightMap[x, y] >= world.highMountainMinHeight)
                    {
                        for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 143;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 162;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 160;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //Mountainous low
                    else if (world.HeightMap[x, y] >= world.lowMountainMinHeight)
                    {
                        for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 123;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 142;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 140;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //Lowlands
                    else if (world.HeightMap[x, y] >= world.lowlandMinHeight)
                    {
                        //Wetlands
                        if (world.MoistureMap[x, y] >= world.wetlandsMinMoisture)
                        {
                            for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 0;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 77;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 40;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                        //Grasslands
                        else if (world.MoistureMap[x, y] >= world.grasslandsMinMoisture)
                        {
                            for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 20;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 97;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 60;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                        //Dry grasslands
                        else if (world.MoistureMap[x, y] >= world.dryGrasslandsMinMoisture)
                        {
                            for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 50;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 127;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 90;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                        //Savanna
                        else if (world.MoistureMap[x, y] >= world.savannaMinMoisture)
                        {
                            for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 69;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 118;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 134;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                        //low moisture desert
                        else
                        {
                            for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 129;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 177;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 194;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                    }
                    //Beach
                    else if (world.HeightMap[x, y] >= world.beachMinHeight)
                    {
                        for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 129;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 177;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 194;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //Shallow water
                    else if (world.HeightMap[x, y] >= world.shallowWaterMinHeight)
                    {
                        for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 198;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 82;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 9;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //Deep water
                    else
                    {
                        for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 178;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 62;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 0;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    Int32Rect rect = new Int32Rect(x * cellSize, y * cellSize, cellSize, cellSize);
                    wb.WritePixels(rect, bytes, stride, 0);
                }
            }
        }

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