using System.Drawing;

using WindowsWizard.Filters.Common.Base;

namespace WindowsWizard.Filters
{
    internal sealed class SequentialPSX : Filter, IFilterParameterized<SequentialPSXPreferences>
    {
        private Bitmap? inputImage;
        private Bitmap? outputImage;

        internal override string Alias { get => "psx sequential"; }

        public Bitmap Apply(Bitmap image, in SequentialPSXPreferences preferences)
        {
            this.inputImage = image;
            this.outputImage = new Bitmap(image.Width, image.Height);

            int correctBlockWidth;
            int correctBlockHeight;

            for (int y = 0; y < image.Height; y += preferences.BlockSize)
            {
                for (int x = 0; x < image.Width; x += preferences.BlockSize)
                {
                    correctBlockWidth = Math.Min(preferences.BlockSize, image.Width - x);
                    correctBlockHeight = Math.Min(preferences.BlockSize, image.Height - y);

                    Color averageColor = this.GetAverageColor(x, y, correctBlockWidth, correctBlockHeight);
                    this.FillPixelBlock(x, y, correctBlockWidth, correctBlockHeight, averageColor);
                }
            }

            return this.outputImage;
        }

        private Color GetAverageColor(int startX, int startY, int blockWidth, int blockHeight)
        {
            int totalR = 0;
            int totalG = 0;
            int totalB = 0;
            int totalA = 0;

            Color pixelColor;

            int pixelsCount = blockWidth * blockHeight;

            for (int y = startY; y < (startY + blockHeight) && y < this.inputImage!.Height; ++y)
            {
                for (int x = startX; x < (startX + blockWidth) && x < this.inputImage.Width; ++x)
                {
                    pixelColor = this.inputImage.GetPixel(x, y);

                    totalR += pixelColor.R;
                    totalG += pixelColor.G;
                    totalB += pixelColor.B;
                    totalA += pixelColor.A;
                }
            }

            int averageR = totalR / pixelsCount;
            int averageG = totalG / pixelsCount;
            int averageB = totalB / pixelsCount;
            int averageA = totalA / pixelsCount;

            return Color.FromArgb(averageA, averageR, averageG, averageB);
        }

        private void FillPixelBlock(int startX, int startY, int blockWidth, int blockHeight, Color color)
        {
            for (int y = startY; y < (startY + blockHeight) && y < this.outputImage!.Height; ++y)
            {
                for (int x = startX; x < (startX + blockWidth) && x < this.outputImage.Width; ++x)
                {
                    this.outputImage.SetPixel(x, y, color);
                }
            }
        }
    }
}
