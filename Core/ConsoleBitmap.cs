using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLibrary.Core
{
    public static class ConsoleBitmap
    {
        private static Bitmap ReduceBitmapSize(Bitmap original, double scaleFactor)
        {
            int newWidth = (int)(original.Width * scaleFactor);
            int newHeight = (int)(original.Height * scaleFactor);

            Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(resizedBitmap))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(original, 0, 0, newWidth, newHeight);
            }

            return resizedBitmap;
        }

        public static void DisplayBitmapInConsole(Bitmap bitmap)
        {
            Console.Clear();

            bitmap = ReduceBitmapSize(bitmap, .009f); 

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    Console.ForegroundColor = GetClosestConsoleColor(pixelColor);
                    Console.Write("#");
                }
                Console.WriteLine();
            }

            Console.ResetColor();
        }

        static ConsoleColor GetClosestConsoleColor(Color color)
        {
            // Map the Color to a ConsoleColor
            if (color.GetBrightness() < 0.3)
                return ConsoleColor.DarkGray;
            if (color.GetBrightness() < 0.6)
                return ConsoleColor.Gray;

            return ConsoleColor.White;
        }
    }
}
