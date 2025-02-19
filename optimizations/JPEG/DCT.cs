﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace JPEG
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class DCT
    {
        private static readonly float Sqrt2 = MathF.Sqrt(2);

        public static float[,] DCT2D(float[,] input)
        {
            var height = input.GetLength(0);
            var width = input.GetLength(1);
            var coeffs = new float[width, height];
            var beta = Beta(height, width);

            Parallel.For(0, width, u =>
            {
                var alpha = Alpha(u);
                Parallel.For(0, height, v =>
                {
                    var sum = DCT2DSum(input, u, v);
                    coeffs[u, v] = sum * beta * alpha * Alpha(v);
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
                    var sum = IDCT2DSum(coeffs, x, y);

                    output[x, y] = sum * beta;
                });
            });
        }


        private static float BasisFunction(float a, int u, int v, int x, int y, int height, int width)
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

        private static float DCT2DSum(float[,] input, int u, int v)
        {
            var width = input.GetLength(1);
            var height = input.GetLength(0);

            var sum = 0f;
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                sum += BasisFunction(input[x, y], u, v, x, y, height, width);
            }

            return sum;
        }

        private static float IDCT2DSum(float[,] coeffs, int x, int y)
        {
            var width = coeffs.GetLength(1);
            var height = coeffs.GetLength(0);

            var sum = 0f;
            for (int i = 0; i < width; i++)
            {
                var alpha = Alpha(i);
                for (int j = 0; j < height; j++)
                {
                    sum += BasisFunction(coeffs[i, j], i, j, x, y, height, width)
                           * alpha * Alpha(j);
                }
            }

            return sum;
        }
    }
}