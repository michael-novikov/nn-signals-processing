using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Signals
{
    public class SignalProcessor
    {
        private static Random rand = new Random();

        public static int[] binarize(Bitmap bmp, Color backgroundColor, int grayscaleRange = 20)
        {
            return convert(bmp, backgroundColor, grayscaleRange, 0, 1);
        }

        public static int[] bipolarize(Bitmap bmp, Color backgroundColor, int grayscaleRange = 20)
        {
            return convert(bmp, backgroundColor, grayscaleRange, -1, 1);
        }

        private static int[] convert(Bitmap bmp, Color backgroundColor, int grayscaleRange, int val1 = 0, int val2 = 1)
        {
            var res = new int[bmp.Height * bmp.Width];

            //using (var lb = new LockBitmap.LockBitmap(bmp))
            var lb = bmp;
            {
                var backgroundGrayscale = colorToGrayscale(backgroundColor);
                var i = 0;
                for (var y = 0; y < lb.Height; y++)
                {
                    for (var x = 0; x < lb.Width; x++)
                    {
                        var color = lb.GetPixel(x, y);

                        res[i] = inRange(color, backgroundGrayscale, grayscaleRange) ? val1 : val2;
                        ++i;
                    }
                }
            }

            return res;
        }

        public static int colorToGrayscale(Color c)
        {
            return (int)Math.Round(0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B);
        }

        public static void bitmapToGrayscale(Bitmap bm)
        {
            double coefR = 0.21,
                   coefG = 0.71,
                   coefB = 0.07;

            using (var lb = new LockBitmap.LockBitmap(bm))
            {
                for (var y = 0; y < lb.Height; y++)
                {
                    for (var x = 0; x < lb.Width; x++)
                    {
                        var color = lb.GetPixel(x, y);

                        var intensity = (int)(coefR * color.R + coefG * color.G + coefB * color.B);
                        var newColor = Color.FromArgb(intensity, intensity, intensity);

                        lb.SetPixel(x, y, newColor);
                    }
                }
            }
        }

        private static bool inRange(Color c, int grayscaleValue, int grayscaleRange)
        {
            var min = grayscaleValue - grayscaleRange;
            var max = grayscaleValue + grayscaleRange;
            var grayscale = colorToGrayscale(c);

            return (min <= grayscale) && (grayscale <= max);
        }

        public static double[] scale(Bitmap bmp, double scaleFrom, double scaleTo)
        {
            var res = new double[bmp.Height * bmp.Width];

            using (var lb = new LockBitmap.LockBitmap(bmp))
            {
                var i = 0;
                for (var y = 0; y < lb.Height; y++)
                {
                    for (var x = 0; x < lb.Width; x++)
                    {
                        var color = lb.GetPixel(x, y);

                        var value = colorToGrayscale(color);
                        var scaledValue = value * (scaleTo - scaleFrom) / 255 + scaleFrom;
                        res[i] = scaledValue;

                        ++i;
                    }
                }
            }

            return res;
        }

        public static double[] scale(double[] data, double scaleFrom, double scaleTo)
        {
            var res = new double[data.Length];
            var min = data.Min();
            var max = data.Max();

            for (var i = 0; i < data.Length; i++)
            {
                res[i] = (data[i] - min) * (scaleTo - scaleFrom) / (max - min) + scaleFrom;
            }

            return res;
        }

        public static int[] addNoiseToBipolarizedArray(int[] probe, double noisePercent = 0.25)
        {
            var res = new int[probe.Length];
            probe.CopyTo(res, 0);

            for (var i = 0; i < res.Length; ++i)
            {
                if (rand.NextDouble() <= noisePercent)
                {
                    res[i] *= -1;
                }
            }

            return res;
        }
    }
}
