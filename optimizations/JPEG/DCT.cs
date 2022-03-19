using System;
using System.Collections.Concurrent;
using JPEG.Utilities;

namespace JPEG
{
    public class DCT
    {
        private static readonly double Sqrt2 = Math.Sqrt(2);

        private static readonly ConcurrentDictionary<double, double> CosCash = new();

        public static double[,] DCT2D(double[,] input)
        {
            var height = input.GetLength(0);
            var width = input.GetLength(1);
            var coeffs = new double[width, height];
            var beta = Beta(height, width);

            MathEx.LoopByTwoVariables(
                0, width,
                0, height,
                (u, v) =>
                {
                    var sum = MathEx
                        .SumByTwoVariables(
                            0, width,
                            0, height,
                            (x, y) => BasisFunction(input[x, y], u, v, x, y, height, width));

                    coeffs[u, v] = sum * beta * Alpha(u) * Alpha(v);
                });

            return coeffs;
        }

        public static void IDCT2D(double[,] coeffs, double[,] output)
        {
            var beta = Beta(coeffs.GetLength(0), coeffs.GetLength(1));
            var height = coeffs.GetLength(0);
            var width = coeffs.GetLength(1);

            for (var x = 0; x < coeffs.GetLength(1); x++)
            {
                for (var y = 0; y < coeffs.GetLength(0); y++)
                {
                    var sum = MathEx
                        .SumByTwoVariables(
                            0, width,
                            0, height,
                            (u, v) =>
                                BasisFunction(coeffs[u, v], u, v, x, y, coeffs.GetLength(0), coeffs.GetLength(1)) *
                                Alpha(u) * Alpha(v));

                    output[x, y] = sum * beta;
                }
            }
        }

        public static double BasisFunction(double a, double u, double v, double x, double y, int height, int width)
        {
            var xAngle = ((2d * x + 1d) * u * Math.PI) / (2 * width);
            var yAngle = ((2d * y + 1d) * v * Math.PI) / (2 * height);

            if (!CosCash.ContainsKey(xAngle))
            {
                CosCash[xAngle] = Math.Cos(xAngle);
            }

            if (!CosCash.ContainsKey(yAngle))
            {
                CosCash[yAngle] = Math.Cos(yAngle);
            }
            
            var b = CosCash[xAngle];

            var c = CosCash[yAngle];

            return a * b * c;
        }

        private static double Alpha(int u)
        {
            if (u == 0)
                return 1 / Sqrt2;
            return 1;
        }

        private static double Beta(int height, int width)
        {
            return 1d / width + 1d / height;
        }
    }
}