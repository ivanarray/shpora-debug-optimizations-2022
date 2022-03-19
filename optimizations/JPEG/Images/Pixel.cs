using System;
using System.Linq;

namespace JPEG.Images
{
    public class Pixel
    {
        private readonly PixelFormat format;

        public Pixel(float firstComponent, float secondComponent, float thirdComponent, PixelFormat pixelFormat)
        {
            if (!new[]{PixelFormat.RGB, PixelFormat.YCbCr}.Contains(pixelFormat))
                throw new FormatException("Unknown pixel format: " + pixelFormat);
            format = pixelFormat;
            if (pixelFormat == PixelFormat.RGB)
            {
                r = firstComponent;
                g = secondComponent;
                b = thirdComponent;
            }
            if (pixelFormat == PixelFormat.YCbCr)
            {
                y = firstComponent;
                cb = secondComponent;
                cr = thirdComponent;
            }
        }

        private readonly float r;
        private readonly float g;
        private readonly float b;

        private readonly float y;
        private readonly float cb;
        private readonly float cr;

        public float R => format == PixelFormat.RGB ? r : (298.082f * y + 408.583f * Cr) / 256.0f - 222.921f;
        public float G => format == PixelFormat.RGB ? g : (298.082f * Y - 100.291f * Cb - 208.120f * Cr) / 256.0f + 135.576f;
        public float B => format == PixelFormat.RGB ? b : (298.082f * Y + 516.412f * Cb) / 256.0f - 276.836f;

        public float Y => format == PixelFormat.YCbCr ? y : 16.0f + (65.738f * R + 129.057f * G + 24.064f * B) / 256.0f;
        public float Cb => format == PixelFormat.YCbCr ? cb : 128.0f + (-37.945f * R - 74.494f * G + 112.439f * B) / 256.0f;
        public float Cr => format == PixelFormat.YCbCr ? cr : 128.0f + (112.439f * R - 94.154f * G - 18.285f * B) / 256.0f;
    }
}