using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using SimplexNoise;
using System.IO;
using System.Collections.Generic;

namespace WorldBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //----------------------------------------------GENERAL----------------------------------------------
        
        //Initializing the window
        public MainWindow()
        {
            InitializeComponent();

            //Populate textboxes with reasonable defaults
            txtKingdomName.Text = kingdomName;
            txtPhysicalArea.Text = physicalArea.ToString();
            cmbPopulationDensity.SelectedIndex = 3;
            txtKingdomAge.Text = kingdomAge.ToString();
        }

        //Random number generator
        Random rnd = new Random();

        //Ensure certain textboxes only accept numeric input
        private void txtEnsureNumeric(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //----------------------------------------------KINGDOM----------------------------------------------
        //Variables describing the kingdom
        string kingdomName = "The Kingdom";
        int physicalArea = 150000;
        int populationDensity = 80;
        int kingdomAge = 500;
        int percentArable = 45;

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
            calculateKingdomSettlements();
            calculateKingdomCastles();
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

        //Do calculations for the kingdom's settlements and display information
        private void calculateKingdomSettlements()
        {
            int totalPopulation = physicalArea * populationDensity;
            int remainingPopulation = totalPopulation;
            double populationRoot = Math.Sqrt(totalPopulation);
            double modifier = (rnd.Next(1, 5) + rnd.Next(1, 5) + 10); //Between 12-20, average of 15

            //The population of the largest settlement = the square root of the kingdom's total population multiplied by a random modifier
            int largestPopulation = (int)(populationRoot * modifier);
            remainingPopulation -= largestPopulation;

            //The second largest settlement will be 20-80% the size of the largest
            modifier = (rnd.Next(1, 5) + rnd.Next(1, 5)) * 0.1; //Between 0.2-0.8, average of 0.5
            int secondLargestPopulation = (int)(largestPopulation * modifier);
            remainingPopulation -= secondLargestPopulation;

            string settlementsString = "The largest settlement has a population of " + largestPopulation + " people. " +
                                       "The second largest has a population of " + secondLargestPopulation + " people. " +
                                       "The remaining " + remainingPopulation + " people live in numerous small towns, villages, " +
                                       "isolated dwellings, etc.";

            txtblockOutputSettlements.Text = settlementsString;

        }

        //Do calculations for the kingdom's castles and display information
        private void calculateKingdomCastles()
        {
            int totalPopulation = physicalArea * populationDensity;

            //Calculate the number of ruined castles
            int ruinedCastlesTotal = (int)(Math.Sqrt(kingdomAge) * (totalPopulation / 5000000f));
            int ruinedCastlesCivilized = (int)(0.75 * ruinedCastlesTotal);
            int ruinedCastlesWilderness = ruinedCastlesTotal - ruinedCastlesCivilized;

            //Calculate the number of active castles
            int activeCastlesTotal = totalPopulation / 50000;
            int activeCastlesCivilized = (int)(0.75 * activeCastlesTotal);
            int activeCastlesWilderness = activeCastlesTotal - activeCastlesCivilized;

            string castlesString = kingdomName + " has " + activeCastlesTotal + " active castles and " + ruinedCastlesTotal + " ruined castles. Of these, " +
                                   activeCastlesCivilized + " active castles and " + ruinedCastlesCivilized + " ruined castles are in civilized lands, and " +
                                   activeCastlesWilderness + " active castles and " + ruinedCastlesWilderness + " ruined castles are in the wilderness, along borders, etc.";

            txtblockOutputCastles.Text = castlesString;
        }

        //----------------------------------------------SETTLEMENT----------------------------------------------
        //Variables describing the settlement
        string settlementName = "The Settlement";
        int settlementPopulation = 10000;

        //Dictionary pairing the names and Support-Values (SVs) for professions in the settlement
        //An SV is the number of citizens in a settlement required for there to be 1 of that profession
        //For example, for every 800 people in a settlement, there will be 1 baker in that settlement
        Dictionary<string, int> SVDictionary = new Dictionary<string, int>()
        {
            {"Bakers", 800},            {"Barbers", 350},           {"Bathers", 1900},          {"Beer-sellers", 1400}, {"Blacksmiths", 1500},      {"Bleachers", 2100},
            {"Bookbinders", 3000},      {"Booksellers", 6300},      {"Buckle Makers", 1400},    {"Butchers", 1200},     {"Carpenters", 550},        {"Chandlers", 700},
            {"Chicken Butchers", 1000}, {"Coopers", 700},           {"Copyists", 2000},         {"Cutlers", 2300},      {"Doctors", 1700},          {"Fishmongers", 1200},
            {"Furriers", 250},          {"Glovemakers", 2400},      {"Harness-makers", 2000},   {"Hatmakers", 950},     {"Hay Merchants", 2300},    {"Illuminators", 3900},
            {"Inns", 2000},             {"Jewelers", 400},          {"Locksmiths", 1900},       {"Magic Shops", 2800},  {"Maidservants", 250},      {"Masons", 500},
            {"Mercers", 700},           {"Old Clothes", 400},       {"Painters", 1500},         {"Pastrycooks", 500},   {"Plasterers", 1400},       {"Pursemakers", 1100},
            {"Roofers", 1800},          {"Ropemakers", 1900},       {"Rugmakers", 2000},        {"Saddlers", 1000},     {"Scabbardmakers", 850},    {"Sculptors", 2000},
            {"Shoemakers", 150},        {"Spice Merchants", 1400},  {"Tailors", 250},           {"Tanners", 2000},      {"Taverns", 400},           {"Watercarriers", 850},
            {"Weavers", 600},           {"Wine-sellers", 900},      {"Woodcarvers", 2400},      {"Woodsellers", 2400}
        };
        //Dictionary pairing the names of and number of each profession in the settlement
        Dictionary<string, int> professionDictionary = new Dictionary<string, int>()
        {
            {"Bakers", 0},              {"Barbers", 0},             {"Bathers", 0},             {"Beer-sellers", 0},    {"Blacksmiths", 0},         {"Bleachers", 0},
            {"Bookbinders", 0},         {"Booksellers", 0},         {"Buckle Makers", 0},       {"Butchers", 0},        {"Carpenters", 0},          {"Chandlers", 0},
            {"Chicken Butchers", 0},    {"Coopers", 0},             {"Copyists", 0},            {"Cutlers", 0},         {"Doctors", 0},             {"Fishmongers", 0},
            {"Furriers", 0},            {"Glovemakers", 0},         {"Harness-makers", 0},      {"Hatmakers", 0},       {"Hay Merchants", 0},       {"Illuminators", 0},
            {"Inns", 0},                {"Jewelers", 0},            {"Locksmiths", 0},          {"Magic Shops", 0},     {"Maidservants", 0},        {"Masons", 0},
            {"Mercers", 0},             {"Old Clothes", 0},         {"Painters", 0},            {"Pastrycooks", 0},     {"Plasterers", 0},          {"Pursemakers", 0},
            {"Roofers", 0},             {"Ropemakers", 0},          {"Rugmakers", 0},           {"Saddlers", 0},        {"Scabbardmakers", 0},      {"Sculptors", 0},
            {"Shoemakers", 0},          {"Spice Merchants", 0},     {"Tailors", 0},             {"Tanners", 0},         {"Taverns", 0},             {"Watercarriers", 0},
            {"Weavers", 0},             {"Wine-sellers", 0},        {"Woodcarvers", 0},         {"Woodsellers", 0}
        };

        //----------------------------------------------WORLD----------------------------------------------
        //World map variables
        static int worldSize = 1024; //How many cells wide the world is
        static int octaves = 10; //How many octaves to use in noise generation
        static float[,] heightMap;
        static float[,] moistureMap;
        static float[,] borderMatrix;

        //World map Image variables
        public static int cellSize = 1; //How many pixels one cell in the grid is
        public static int stride = cellSize * (PixelFormats.Bgra32.BitsPerPixel / 8); //How many bytes are needed for one row of a cell, used for drawing
        static int imageWidth = worldSize * cellSize; //How many pixels wide the image is
        static int imageHeight = worldSize * cellSize; //How many pixels high the image is
        WriteableBitmap wb = new WriteableBitmap(imageWidth, imageHeight, 300, 300, PixelFormats.Bgra32, null);

        //Slider variables
        public float amplitude = 0.005f;
        public float persistence = 0.5f;

        //Update map size when text input is changed
        private void txtWorldSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(txtWorldSize.Text, out worldSize);
        }

        //Update octaves when text input is changed
        private void txtOctaves_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(txtOctaves.Text, out octaves);
        }

        //Update amplitude when slider is changed
        private void slAmplitude_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            amplitude = (float)slAmplitude.Value;
        }

        //Update persistence when slider is changed
        private void slPersistence_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            persistence = (float)slPersistence.Value;
        }

        //Function for calculating random noise ata given point with a given number of octaves
        private float noiseWithOctaves(int x, int y, int octaves, float persistence, float amplitude, int low, int high)
        {
            float maxAmp = 0;
            float amp = 1;
            float noise = 0;

            for (int i = 0; i < octaves; ++i)
            {
                noise += Noise.Generate(x * amplitude, y * amplitude) * amp;
                maxAmp += amp;
                amp *= persistence;
                amplitude *= 2;
            }

            noise /= maxAmp;

            noise = noise * (high - low) / 2 + (high + low) / 2;

            return noise;
        }

        private void btnDrawTest_Click(object sender, RoutedEventArgs e)
        {
            imageWidth = worldSize * cellSize;
            imageHeight = worldSize * cellSize;

            wb = new WriteableBitmap(imageWidth, imageHeight, 300, 300, PixelFormats.Bgra32, null);
            heightMap = new float[worldSize, worldSize];
            moistureMap = new float[worldSize, worldSize];
            borderMatrix = new float[worldSize, worldSize];

            image.Width = imageWidth;
            image.Height = imageHeight;
            image.Source = wb;

            //New seed every time
            byte[] noiseSeed = new byte[512];
            rnd.NextBytes(noiseSeed);
            Noise.perm = noiseSeed;

            //Create a border matrix to subtract from the height map
            //No border
            if(radioBorderNone.IsChecked == true)
            {
                for (int x = 0; x < worldSize; ++x)
                {
                    for (int y = 0; y < worldSize; ++y)
                    {
                        borderMatrix[x, y] = 0;
                    }
                }
            }
            //Square border
            else if (radioBorderSquare.IsChecked == true)
            {
                float half = worldSize / 2 + worldSize % 2;
                for (int x = 0; x < half; ++x)
                {
                    for (int y = 0; y < half; ++y)
                    {
                        byte a = (byte)(((1 - Math.Min(x, y) / (half - 1)) * 255));
                        borderMatrix[x, y] = a;
                        borderMatrix[(worldSize - x) - 1, y] = a;
                        borderMatrix[(worldSize - x) - 1, (worldSize - y) - 1] = a;
                        borderMatrix[x, (worldSize - y) - 1] = a;
                    }
                }
            }
            //Circle border
            else
            {
                int half = worldSize / 2;
                int borderDistance = (int)(worldSize * 0.66);
                float maxDistance = (float)(Math.Sqrt(worldSize * worldSize + worldSize * worldSize));
                for (int x = 0; x < worldSize; ++x)
                {
                    for (int y = 0; y < worldSize; ++y)
                    {
                        int adjustedX = x - half;
                        int adjustedY = y - half;
                        float distanceFromCenter = (float)(Math.Sqrt(adjustedX * adjustedX + adjustedY * adjustedY));
                        float matrixValue = (float)((distanceFromCenter / borderDistance) * 255);
                        borderMatrix[x, y] = matrixValue;
                    }
                }
            }
            

            //Create a base heightmap for the world, subtract square matrix to make an island
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    float heightValue = noiseWithOctaves(x, y, octaves, persistence, amplitude, 0, 255);
                    heightValue -= borderMatrix[x, y];
                    if (heightValue < 0)
                    {
                        heightValue = 0;
                    }
                    heightMap[x, y] = (byte)heightValue;
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
                    moistureMap[x, y] = (byte)(noiseWithOctaves(x, y, octaves, persistence, amplitude, 0, 255));
                }
            }

            //Debug, uncomment to draw the square matrix
            /*
            byte[] bytes = new byte[stride * cellSize];
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
                    {
                        bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = (byte)heightMap[x, y];
                        bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = (byte)heightMap[x, y];
                        bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = (byte)heightMap[x, y];
                        bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                    }
                    Int32Rect rect = new Int32Rect(x * cellSize, y * cellSize, cellSize, cellSize);
                    wb.WritePixels(rect, bytes, stride, 0);
                }
            }
            */
            
            //Define values for heights and moistures for different biomes
            //Heights range from 0-255
            int snowCapMinHeight = 160;
            int highMountainMinHeight = 140;
            int lowMountainMinHeight = 120;
            int lowlandMinHeight = 40;
            int beachMinHeight = 30;
            int shallowWaterMinHeight = 10; // Anything lower is deep water

            //Moistures range from 0-255
            int wetlandsMinMoisture = 200;
            int grasslandsMinMoisture = 150;
            int dryGrasslandsMinMoisture = 70;
            int savannaMinMoisture = 50; // Anything lower is desert

            //Color each cell according to its height and moisture
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    //Use an Int32Rect to choose the rectangular region to edit
                    //xy of top left corner plus width and height of edited region
                    byte[] bytes = new byte[stride * cellSize];

                    //Color each cell based on its height and moisture

                    //Snowcap
                    if (heightMap[x, y] >= snowCapMinHeight)
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
                    else if (heightMap[x, y] >= highMountainMinHeight)
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
                    else if (heightMap[x, y] >= lowMountainMinHeight)
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
                    else if (heightMap[x, y] >= lowlandMinHeight)
                    {
                        //Wetlands
                        if (moistureMap[x, y] >= wetlandsMinMoisture)
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
                        else if (moistureMap[x, y] >= grasslandsMinMoisture)
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
                        else if (moistureMap[x, y] >= dryGrasslandsMinMoisture)
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
                        else if (moistureMap[x, y] >= savannaMinMoisture)
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
                    else if (heightMap[x, y] >= beachMinHeight)
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
                    else if (heightMap[x, y] >= shallowWaterMinHeight)
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