using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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
    }
}
