using System.Drawing;
using System.Threading.Tasks;

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
			
            Pixels = new Pixel[height,width];
            for(var i = 0; i< height; ++i)
            for(var j = 0; j< width; ++j)
                Pixels[i, j] = new Pixel(0, 0, 0, PixelFormat.RGB);
        }

        public static explicit operator Matrix(Bitmap bmp)
        {
            var height = bmp.Height - bmp.Height % 8;
            var width = bmp.Width - bmp.Width % 8;
            var matrix = new Matrix(height, width);

            Parallel.For(0, height, y =>
            {
                Parallel.For(0, width, x =>
                {
                    var pixel = bmp.GetPixel(x, y);
                    matrix.Pixels[y, x] = new Pixel(pixel.R, pixel.G, pixel.B, PixelFormat.RGB);
                });
            });

            return matrix;
        }

        public static explicit operator Bitmap(Matrix matrix)
        {
            var bmp = new Bitmap(matrix.Width, matrix.Height);
            var width = bmp.Width;
            var height = bmp.Height;

            Parallel.For(0, height, y =>
            {
                Parallel.For(0, width, x =>
                {
                    var pixel = matrix.Pixels[y, x];
                    bmp.SetPixel(x, y, Color.FromArgb(ToByte(pixel.R), ToByte(pixel.G), ToByte(pixel.B)));
                });
            });
            

            return bmp;
        }

        public static int ToByte(double d)
        {
            var val = (int) d;
            if (val > byte.MaxValue)
                return byte.MaxValue;
            if (val < byte.MinValue)
                return byte.MinValue;
            return val;
        }
    }
}