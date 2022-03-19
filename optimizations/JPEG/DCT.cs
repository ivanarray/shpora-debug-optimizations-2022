using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using JPEG.Utilities;

namespace JPEG
{
    public class DCT
    {
        private static readonly float Sqrt2 = MathF.Sqrt(2);
        
        public static float[,] DCT2D(float[,] input)
        {
            var height = input.GetLength(0);
            var width = input.GetLength(1);
            var coeffs = new float[width, height];
            var beta = Beta(height, width);

            Parallel.For(0, width, x =>
            {
                Parallel.For(0, height, y =>
                {
                    var sum = MathEx.SumByTwoVariables(
                        0, width,
                        0, height,
                        (u, v) =>
                            BasisFunction(input[u, v], x, y, u, v, height, width)
                    );
                    coeffs[x, y] = sum * beta * Alpha(x) * Alpha(x);
                });
            });
            

            return coeffs;
        }

        public static void IDCT2D(float[,] coeffs, float[,] output)
        {
            var beta = Beta(coeffs.GetLength(0), coeffs.GetLength(1));
            var height = coeffs.GetLength(0);
            var width = coeffs.GetLength(1);

            Parallel.For(0, width, x =>
            {
                Parallel.For(0, height, y =>
                {
                    var sum = MathEx
                        .SumByTwoVariables(
                            0, width,
                            0, height,
                            (u, v) =>
                                BasisFunction(coeffs[u, v], u, v, x, y, height, width) *
                                Alpha(u) * Alpha(v));

                    output[x, y] = sum * beta;
                });
            });
        }


        public static float BasisFunction(float a, int u, int v, int x, int y, int height, int width)
        {
            var xAngle = ((2f * x + 1f) * u * MathF.PI) / (2 * width);
            var yAngle = ((2f * y + 1f) * v * MathF.PI) / (2 * height);

            var b = MathF.Cos(xAngle);

            var c = MathF.Cos(yAngle);

            return a * b * c;
        }

        private static float Alpha(int u)
        {
            if (u == 0)
                return 1 / Sqrt2;
            return 1;
        }

        private static float Beta(int height, int width)
        {
            return 1f / width + 1f / height;
        }
    }
}