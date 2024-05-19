using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

using WindowsWizard.Filters.Common.Base;

namespace WindowsWizard.Filters
{
    internal sealed class ParallelPSX : Filter, IFilterParameterized<ParallelPSXPreferences>
    {
        int imageWidth;
        int imageHeight;
        private Bitmap? inputImage;
        private Bitmap? outputImage;
        private BitmapData? inputImageBitmapData;
        private BitmapData? outputImageBitmapData;

        internal override string Alias { get => "psx parallel"; }

        public Bitmap Apply(Bitmap image, in ParallelPSXPreferences preferences)
        {
            this.inputImage = image;
            this.outputImage = new Bitmap(image.Width, image.Height);

            this.imageWidth = image.Width;
            this.imageHeight = image.Height;

            int blockSize = preferences.BlockSize;
            int taskPayload = image.Height / blockSize / preferences.ThreadsCount;

            this.inputImageBitmapData = this.inputImage.Lock();
            this.outputImageBitmapData = this.outputImage.Lock();

            Task[] tasks = new Task[preferences.ThreadsCount];

            for (int i = 0; i < tasks.Length; ++i)
            {
                int indexStart = i * taskPayload * blockSize;
                int indexFinish = (i == tasks.Length - 1) ? image.Height : (indexStart + (taskPayload * blockSize));

                tasks[i] = Task.Run(() =>
                {
                    for (int y = indexStart; y < indexFinish; y += blockSize)
                    {
                        for (int x = 0; x < this.imageWidth; x += blockSize)
                        {
                            int correctBlockWidth = Math.Min(blockSize, this.imageWidth - x);
                            int correctBlockHeight = Math.Min(blockSize, this.imageHeight - y);

                            Color averageColor = this.GetAverageColor(x, y, correctBlockWidth, correctBlockHeight);
                            this.FillPixelBlock(x, y, correctBlockWidth, correctBlockHeight, averageColor);
                        }
                    }
                });
            }

            Task.WaitAll(tasks);

            this.inputImage.Unlock(this.inputImageBitmapData);
            this.outputImage.Unlock(this.outputImageBitmapData);

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

            for (int y = startY; y < (startY + blockHeight) && y < this.imageHeight; ++y)
            {
                for (int x = startX; x < (startX + blockWidth) && x < this.imageWidth; ++x)
                {
                    pixelColor = this.inputImage!.GetPixel(x, y, this.inputImageBitmapData!);

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
            for (int y = startY; y < (startY + blockHeight) && y < this.imageHeight; ++y)
            {
                for (int x = startX; x < (startX + blockWidth) && x < this.imageWidth; ++x)
                {
                    this.outputImage!.SetPixel(x, y, color, this.outputImageBitmapData!);
                }
            }
        }
    }
}
