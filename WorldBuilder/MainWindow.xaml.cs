using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using SimplexNoise;
using System.Diagnostics;

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
        string kingdomName = "The Kingdom";
        int physicalArea = 150000;
        int populationDensity = 80;
        int kingdomAge = 500;
        int percentArable = 45;

        //World map variables
        static int worldSize = 1024; //How many cells wide the world is
        static float[] heightMap = new float[worldSize * worldSize];
        static float[] moisture = new float[worldSize * worldSize];
        static float[] square = new float[worldSize * worldSize];

        //World map Image variables
        MapColors mapColors = new MapColors(); //Byte data used for colors in the map
        public static int cellSize = 1; //How many pixels one cell in the grid is
        public static int stride = cellSize * (PixelFormats.Bgra32.BitsPerPixel / 8); //How many bytes are needed for one row of a cell, used for drawing
        static int imageWidth = worldSize * cellSize; //How many pixels wide the image is
        static int imageHeight = worldSize * cellSize; //How many pixels high the image is

        public MainWindow()
        {
            InitializeComponent();

            //Populate textboxes with reasonable defaults
            txtKingdomName.Text = kingdomName;
            txtPhysicalArea.Text = physicalArea.ToString();
            cmbPopulationDensity.SelectedIndex = 3;
            txtKingdomAge.Text = kingdomAge.ToString();
        }

        //Ensure certain textboxes only accept numeric input
        private void txtEnsureNumeric(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Update output as kindgom name is changed
        private void txtKingdomName_TextChanged(object sender, TextChangedEventArgs e)
        {
            kingdomName = txtKingdomName.Text;
            lblDisplayKingdomName.Content = kingdomName;

            //Update output
            updateKingdomOutput();
        }

        //Update output as physical area is changed
        private void txtPhysicalArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(txtPhysicalArea.Text, out physicalArea);

            //Update output
            updateKingdomOutput();
        }

        //Update output as kingdom age is changed
        private void txtKingdomAge_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(txtKingdomAge.Text, out kingdomAge);

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

            //Update output
            updateKingdomOutput();
        }

        //Update output text blocks
        private void updateKingdomOutput()
        {
            calculateKingdomPhysicalArea();
            calculateKingdomTotalPopulation();
        }

        //Do calculations for kingdom's physical area and display information
        private void calculateKingdomPhysicalArea()
        {
            double arableLand = (double)physicalArea * percentArable / 100;
            double wilderness = physicalArea - arableLand;

            string physicalAreaString = kingdomName + " covers an area of " + physicalArea.ToString() + " square miles. Of this, " +
                                        percentArable.ToString() + "% (" + arableLand.ToString() + " square miles) is arable land, and " +
                                        (100 - percentArable).ToString() + "% (" + wilderness.ToString() + " square miles) is wilderness.";

            txtblockOutputPhysicalArea.Text = physicalAreaString;
        }

        //Do calculations for kingdom's total population and display information
        private void calculateKingdomTotalPopulation()
        {
            int totalPopulation = physicalArea * populationDensity;

            string totalPopulationString = kingdomName + " has a total population of " + totalPopulation.ToString() + " people.";

            txtblockOutputPopulation.Text = totalPopulationString;
        }

        //Function for calculating random noise ata given point with a given number of octaves
        private float noiseWithOctaves(int x, int y, int octaves, float persistence, float scale, int low, int high)
        {
            float maxAmp = 0;
            float amp = 1;
            float frequency = scale;
            float noise = 0;

            for(int i = 0; i < octaves; ++i)
            {
                noise += Noise.Generate(x * frequency, y * frequency) * amp;
                maxAmp += amp;
                amp *= persistence;
                frequency *= 2;
            }

            noise /= maxAmp;

            noise = noise * (high - low) / 2 + (high + low) / 2;

            return noise;
        }

        private void btnDrawTest_Click(object sender, RoutedEventArgs e)
        {
            //Create a new bitmap for drawing the world
            WriteableBitmap wb = new WriteableBitmap(imageWidth, imageHeight, 96, 96, PixelFormats.Bgra32, null);

            //New seed every time
            byte[] noiseSeed = new byte[512];
            rnd.NextBytes(noiseSeed);
            Noise.perm = noiseSeed;

            int half = worldSize / 2;
            //Create a square matrix to subtract from the height map
            float maxDistance = (float)(Math.Sqrt(worldSize * worldSize + worldSize * worldSize));
            for (int x = 0; x < worldSize; ++x)
            {
                for (int y = 0; y < worldSize; ++y)
                {
                    int adjustedX = x - half;
                    int adjustedY = y - half;
                    float distanceFromCenter = (float)(Math.Sqrt(adjustedX * adjustedX + adjustedY * adjustedY));
                    float matrixValue = (float)((distanceFromCenter/maxDistance) * 255);
                    square[(y * worldSize) + x] = matrixValue;
                }
            }

            //Create a base heightmap for the world, subtract square matrix to make an island
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    float heightValue = noiseWithOctaves(x, y, 10, 0.5f, 0.007f, 0, 255);
                    heightValue -= square[(y * worldSize) + x];
                    if(heightValue < 0)
                    {
                        heightValue = 0;
                    }
                    heightMap[(y * worldSize) + x] = (byte)heightValue;
                }
            }

            //New seed for moisture map
            rnd.NextBytes(noiseSeed);
            Noise.perm = noiseSeed;

            //Create a moisture map for the world
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    moisture[(y * worldSize) + x] = (byte)(noiseWithOctaves(x, y, 10, 0.5f, 0.007f, 0, 255));
                }
            }

            /*
            byte[] bytes = new byte[stride * cellSize];
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                    {
                        bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = (byte)square[(y * worldSize) + x];
                        bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = (byte)square[(y * worldSize) + x];
                        bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = (byte)square[(y * worldSize) + x];
                        bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                    }
                    Int32Rect rect = new Int32Rect(x * cellSize, y * cellSize, cellSize, cellSize);
                    wb.WritePixels(rect, bytes, stride, 0);
                }
            }
            */

            //Color each cell according to its height and moisture
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    //Use an Int32Rect to choose the rectangular region to edit
                    //xy of top left corner plus width and height of edited region
                    byte[] bytes = new byte[stride * cellSize];

                    //Color each cell based on its height and moisture

                    //Snow
                    if (heightMap[(y * worldSize) + x] >= 200)
                    {
                        for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 255;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 255;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 255;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //Mountainous high
                    else if (heightMap[(y * worldSize) + x] >= 175)
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
                    else if (heightMap[(y * worldSize) + x] >= 150)
                    {
                        for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 123;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 142;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 140;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //Grassy/Desert
                    else if (heightMap[(y * worldSize) + x] >= 70)
                    {
                        //High moisture grass
                        if (moisture[(y * worldSize) + x] >= 191)
                        {
                            for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 0;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 77;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 40;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                        //Medium moisture grass
                        else if (moisture[(y * worldSize) + x] >= 128)
                        {
                            for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 20;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 97;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 60;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                        //Low moisture grass
                        else if (moisture[(y * worldSize) + x] >= 63)
                        {
                            for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 50;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 127;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 90;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                        //High moisture desert
                        else if (moisture[(y * worldSize) + x] >= 32)
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
                    else if (heightMap[(y * worldSize) + x] >= 60)
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
                    else if (heightMap[(y * worldSize) + x] >= 30)
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
                    Int32Rect rect = new Int32Rect(x*cellSize, y*cellSize, cellSize, cellSize);
                    wb.WritePixels(rect, bytes, stride, 0);
                }
            }

            image.Width = imageWidth;
            image.Height = imageHeight;
            image.Source = wb;
        }
    }
}