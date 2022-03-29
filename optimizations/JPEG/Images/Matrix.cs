using System.Drawing;
using System.Drawing.Imaging;

namespace JPEG.Images
{
    class Matrix
    {
        public readonly Pixel[,] Pixels;
        public readonly int Height;
        public readonly int Width;

        public Matrix(int height, int width)
        {
            Height = height;
            Width = width;

            Pixels = new Pixel[height, width];
            for (var i = 0; i < height; ++i)
            for (var j = 0; j < width; ++j)
                Pixels[i, j] = new Pixel(0, 0, 0, PixelFormat.RGB);
        }

        public static unsafe explicit operator Matrix(Bitmap bmp)
        {
            var height = bmp.Height - bmp.Height % 8;
            var width = bmp.Width - bmp.Width % 8;
            var matrix = new Matrix(height, width);
            var data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var ptr = (byte*) data.Scan0;
            for (var j = 0; j < height; j++)
            {
                var ptr2 = ptr;
                for (var i = 0; i < width; i++)
                {
                    var b = *ptr2++;
                    var g = *ptr2++;
                    var r = *ptr2++;
                    matrix.Pixels[j, i] =
                        new Pixel(r, g, b, PixelFormat.RGB);
                }

                ptr += data.Stride;
            }

            return matrix;
        }

        public static unsafe explicit operator Bitmap(Matrix matrix)
        {
            var bmp = new Bitmap(matrix.Width, matrix.Height);
            var data = bmp.LockBits(new Rectangle(0, 0, matrix.Width, matrix.Height), ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            var ptr = (byte*) data.Scan0;
            for (var j = 0; j < bmp.Height; j++)
            {
                var ptr2 = ptr;
                for (var i = 0; i < bmp.Width; i++)
                {
                    var pixel = matrix.Pixels[j, i];
                    *ptr2++ = ToByte(pixel.B);
                    *ptr2++ = ToByte(pixel.G);
                    *ptr2++ = ToByte(pixel.R);
                }

                ptr += data.Stride;
            }

            bmp.UnlockBits(data);
            return bmp;
        }

        public static byte ToByte(double d)
        {
            var val = (int) d;
            if (val > byte.MaxValue)
                return byte.MaxValue;
            if (val < byte.MinValue)
                return byte.MinValue;
            return (byte)val;
        }
    }
}