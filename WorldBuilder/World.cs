using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SimplexNoise;

namespace WorldBuilder
{
    internal class World
    {
        public World()
        {
            Size = 1024;
            Octaves = 10;
            Amplitude = 0.005f;
            Persistence = 0.5f;

            HeightMap = new float[Size, Size];
            MoistureMap = new float[Size, Size];
            BorderMatrix = new float[Size, Size];
        }

        //World variables
        public int Size { get; set; }//How many cells wide/tall the world is
        public int Octaves { get; set; }//How many rounds of noise to apply to world generation
        public float Amplitude { get; set; }//The "scale" of the noise
        public float Persistence { get; set; }//The "roughness" of the noise

        //Maps for noise generation
        public float[,] HeightMap { get; set; }
        public float[,] MoistureMap { get; set; }
        public float[,] BorderMatrix { get; set; }

        //Define values for heights and moistures for different biomes
        //Heights range from 0-255
        public int snowCapMinHeight = 160;
        public int highMountainMinHeight = 140;
        public int lowMountainMinHeight = 120;
        public int lowlandMinHeight = 40;
        public int beachMinHeight = 30;
        public int shallowWaterMinHeight = 10; // Anything lower is deep water

        //Moistures range from 0-255
        public int wetlandsMinMoisture = 200;
        public int grasslandsMinMoisture = 150;
        public int dryGrasslandsMinMoisture = 70;
        public int savannaMinMoisture = 50; // Anything lower is desert

        //Image drawing variables
        public static int cellSize = 1; //How many pixels one cell in the grid is
        public static int stride = cellSize * (PixelFormats.Bgra32.BitsPerPixel / 8); //How many bytes are needed for one row of a cell, used for drawing
        public int imageWidth { get; set; } //How many pixels wide the image is
        public int imageHeight { get; set; } //How many pixels high the image is
        WriteableBitmap wb;

        //Random number generator
        Random rnd = new Random();

        //Function for calculating random noise at a given point with a given number of octaves
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

        //Function for generating world data
        public void generateWorld(string borderType)
        {
            //New seed every time
            byte[] noiseSeed = new byte[512];
            rnd.NextBytes(noiseSeed);

            Noise.perm = noiseSeed;

            //Create a border matrix to subtract from the height map
            //No border
            if (borderType.Equals("none"))
            {
                for (int x = 0; x < Size; ++x)
                {
                    for (int y = 0; y < Size; ++y)
                    {
                        BorderMatrix[x, y] = 0;
                    }
                }
            }
            //Square border
            else if (borderType.Equals("square"))
            {
                float half = Size / 2 + Size % 2;
                for (int x = 0; x < half; ++x)
                {
                    for (int y = 0; y < half; ++y)
                    {
                        byte a = (byte)(((1 - Math.Min(x, y) / (half - 1)) * 255));
                        BorderMatrix[x, y] = a;
                        BorderMatrix[(Size - x) - 1, y] = a;
                        BorderMatrix[(Size - x) - 1, (Size - y) - 1] = a;
                        BorderMatrix[x, (Size - y) - 1] = a;
                    }
                }
            }
            //Circle border
            else
            {
                int half = Size / 2;
                int borderDistance = (int)(Size * 0.66);
                float maxDistance = (float)(Math.Sqrt(Size * Size + Size * Size));
                for (int x = 0; x < Size; ++x)
                {
                    for (int y = 0; y < Size; ++y)
                    {
                        int adjustedX = x - half;
                        int adjustedY = y - half;
                        float distanceFromCenter = (float)(Math.Sqrt(adjustedX * adjustedX + adjustedY * adjustedY));
                        float matrixValue = (float)((distanceFromCenter / borderDistance) * 255);
                        BorderMatrix[x, y] = matrixValue;
                    }
                }
            }

            //Create a base heightmap for the world, subtract border matrix to make an island if applicable
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    float heightValue = noiseWithOctaves(x, y, Octaves, Persistence, Amplitude, 0, 255);
                    heightValue -= BorderMatrix[x, y];
                    if (heightValue < 0)
                    {
                        heightValue = 0;
                    }
                    HeightMap[x, y] = (byte)heightValue;
                }
            }

            //New seed for moisture map
            rnd.NextBytes(noiseSeed);
            Noise.perm = noiseSeed;

            //Create a moisture map for the world
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    MoistureMap[x, y] = (byte)(noiseWithOctaves(x, y, Octaves, Persistence, Amplitude, 0, 255));
                }
            }
        }

        //Function for creating an image based on world data
        public WriteableBitmap generateImage()
        {
            imageWidth = Size * cellSize;
            imageHeight = Size * cellSize;
            WriteableBitmap wb = new WriteableBitmap(imageWidth, imageHeight, 300, 300, PixelFormats.Bgra32, null);

            //Color each cell according to its height and moisture
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    //Use an Int32Rect to choose the rectangular region to edit
                    //xy of top left corner plus width and height of edited region
                    byte[] bytes = new byte[stride * cellSize];

                    //Color each cell based on its height and moisture

                    //Snowcap
                    if (HeightMap[x, y] >= snowCapMinHeight)
                    {
                        for (int i = 0; i < cellSize * cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 255;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 255;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 255;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //High mountains
                    else if (HeightMap[x, y] >= highMountainMinHeight)
                    {
                        for (int i = 0; i < cellSize * cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 143;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 162;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 160;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //Mountainous low
                    else if (HeightMap[x, y] >= lowMountainMinHeight)
                    {
                        for (int i = 0; i < cellSize * cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 123;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 142;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 140;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //Lowlands
                    else if (HeightMap[x, y] >= lowlandMinHeight)
                    {
                        //Wetlands
                        if (MoistureMap[x, y] >= wetlandsMinMoisture)
                        {
                            for (int i = 0; i < cellSize * cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 0;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 77;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 40;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                        //Grasslands
                        else if (MoistureMap[x, y] >= grasslandsMinMoisture)
                        {
                            for (int i = 0; i < cellSize * cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 20;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 97;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 60;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                        //Dry grasslands
                        else if (MoistureMap[x, y] >= dryGrasslandsMinMoisture)
                        {
                            for (int i = 0; i < cellSize * cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 50;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 127;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 90;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                        //Savanna
                        else if (MoistureMap[x, y] >= savannaMinMoisture)
                        {
                            for (int i = 0; i < cellSize * cellSize; ++i)
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
                            for (int i = 0; i < cellSize * cellSize; ++i)
                            {
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 129;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 177;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 194;
                                bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                            }
                        }
                    }
                    //Beach
                    else if (HeightMap[x, y] >= beachMinHeight)
                    {
                        for (int i = 0; i < cellSize * cellSize; ++i)
                        {
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 129;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 177;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 194;
                            bytes[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
                        }
                    }
                    //Shallow water
                    else if (HeightMap[x, y] >= shallowWaterMinHeight)
                    {
                        for (int i = 0; i < cellSize * cellSize; ++i)
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
                        for (int i = 0; i < cellSize * cellSize; ++i)
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

            return wb;
        }
    }
}
