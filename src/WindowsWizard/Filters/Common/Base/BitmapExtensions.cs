using System.Drawing;
using System.Drawing.Imaging;

namespace WindowsWizard.Filters.Common.Base
{
    internal static class BitmapExtensions
    {
        private const int COLOR_CHANNEL_BLUE = 0;
        private const int COLOR_CHANNEL_GREEN = 1;
        private const int COLOR_CHANNEL_RED = 2;
        private const int COLOR_CHANNEL_ALPHA = 3;

        private static readonly object locker;

        static BitmapExtensions()
        {
            BitmapExtensions.locker = new object();
        }

        public static int GetWidthSynchronized(this Bitmap bitmap)
        {
            lock (BitmapExtensions.locker)
            {
                return bitmap.Width;
            }
        }

        public static int GetHeightSynchronized(this Bitmap bitmap)
        {
            lock (BitmapExtensions.locker)
            {
                return bitmap.Height;
            }
        }

        public unsafe static Color GetPixelSynchronized(this Bitmap bitmap, int x, int y, BitmapData bitmapData)
        {
            int bytesPerPixel = BitmapExtensions.CalculateBytesPerPixel(bitmapData);
            byte* row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
            byte* pixel = row + (x * bytesPerPixel);

            switch (bitmapData.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                    return Color.FromArgb(pixel[BitmapExtensions.COLOR_CHANNEL_RED], pixel[BitmapExtensions.COLOR_CHANNEL_GREEN], pixel[BitmapExtensions.COLOR_CHANNEL_BLUE]);

                case PixelFormat.Canonical:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    return Color.FromArgb(pixel[BitmapExtensions.COLOR_CHANNEL_ALPHA], pixel[BitmapExtensions.COLOR_CHANNEL_RED], pixel[BitmapExtensions.COLOR_CHANNEL_GREEN], pixel[BitmapExtensions.COLOR_CHANNEL_BLUE]);

                default:
                    throw new NotSupportedException($"Image type {bitmapData.PixelFormat} is not supported");
            }
        }

        public unsafe static void SetPixelSynchronized(this Bitmap bitmap, int x, int y, Color color, BitmapData bitmapData)
        {
            int bytesPerPixel = BitmapExtensions.CalculateBytesPerPixel(bitmapData);
            byte* row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
            byte* pixel = row + (x * bytesPerPixel);

            switch (bitmapData.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                    {
                        pixel[BitmapExtensions.COLOR_CHANNEL_BLUE] = color.B;
                        pixel[BitmapExtensions.COLOR_CHANNEL_GREEN] = color.G;
                        pixel[BitmapExtensions.COLOR_CHANNEL_RED] = color.R;
                    }
                    break;

                case PixelFormat.Canonical:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    {
                        pixel[BitmapExtensions.COLOR_CHANNEL_BLUE] = color.B;
                        pixel[BitmapExtensions.COLOR_CHANNEL_GREEN] = color.G;
                        pixel[BitmapExtensions.COLOR_CHANNEL_RED] = color.R;
                        pixel[BitmapExtensions.COLOR_CHANNEL_ALPHA] = color.A;
                    }
                    break;

                default:
                    throw new NotSupportedException($"Image type {bitmapData.PixelFormat} is not supported");
            }
        }

        private static int CalculateBytesPerPixel(BitmapData bitmapData)
        {
            const int BYTES_PER_PIXEL_24BPP = 3;
            const int BYTES_PER_PIXEL_32BPP = 4;

            switch (bitmapData.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return BYTES_PER_PIXEL_24BPP;

                case PixelFormat.Canonical:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    return BYTES_PER_PIXEL_32BPP;

                default:
                    throw new NotSupportedException($"Image type {bitmapData.PixelFormat} is not supported.");
            }
        }

        public static BitmapData Lock(this Bitmap bitmap)
        {
            return bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        }

        public static void Unlock(this Bitmap bitmap, BitmapData bitmapData)
        {
            if (bitmapData == null)
            {
                throw new ArgumentNullException($"{nameof(bitmapData)} is null.");
            }

            bitmap.UnlockBits(bitmapData);
        }
    }
}
