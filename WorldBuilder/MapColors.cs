using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WorldBuilder
{
    // A class containing information for various colors used for drawing the map
    // Colors are stored in BGRA32 format
    internal class MapColors
    {
        public byte[] water = new byte[MainWindow.stride * MainWindow.cellSize];
        public byte[] green = new byte[MainWindow.stride * MainWindow.cellSize];
        public MapColors()
        {
            //Water
            for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
            {
                water[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 234;
                water[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 226;
                water[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 189;
                water[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
            }
            //Green
            for (int i = 0; i < MainWindow.cellSize * MainWindow.cellSize; ++i)
            {
                green[i * (PixelFormats.Bgra32.BitsPerPixel / 8)] = 0;
                green[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 1] = 255;
                green[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 2] = 0;
                green[i * (PixelFormats.Bgra32.BitsPerPixel / 8) + 3] = 255;
            }
        }
    }
}
