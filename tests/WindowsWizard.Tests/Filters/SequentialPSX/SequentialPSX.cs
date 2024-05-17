using System.Drawing;

using WindowsWizard.Filters;

namespace WindowsWizard.Tests.Filters.SequentialPSX
{
    [TestClass]
    public sealed class SequentialPSX
    {
        private Bitmap? imageInput;

        private Bitmap? imageResult1;
        private Bitmap? imageResult2;

        private WindowsWizard.Filters.ParallelPSX? parallelPSX;
        private WindowsWizard.Filters.SequentialPSX? sequentialPSX;

        [TestInitialize]
        public void Setup()
        {
            this.parallelPSX = new WindowsWizard.Filters.ParallelPSX();
            this.sequentialPSX = new WindowsWizard.Filters.SequentialPSX();
            this.imageInput = new Bitmap(@"../../../../../tests/Assets/1k.png");
        }

        [TestMethod]
        public void Apply_Image1k_BlockSize10_ThreadsCount2_ReturnsBitmap()
        {
            ParallelPSXPreferences parallelPSXPreferences = new ParallelPSXPreferences();

            parallelPSXPreferences.BlockSize = 10;
            parallelPSXPreferences.ThreadsCount = 2;

            SequentialPSXPreferences sequentialPSXPreferences = new SequentialPSXPreferences();

            sequentialPSXPreferences.BlockSize = 10;

            this.imageResult1 = this.parallelPSX!.Apply(this.imageInput!, parallelPSXPreferences);
            this.imageResult2 = this.sequentialPSX!.Apply(this.imageInput!, sequentialPSXPreferences);

            Assert.IsTrue(this.AreImagesEqual(this.imageResult1, this.imageResult2));
        }

        public void Apply_Image1k_BlockSize10_ThreadsCount3_ReturnsBitmap()
        {
            ParallelPSXPreferences parallelPSXPreferences = new ParallelPSXPreferences();

            parallelPSXPreferences.BlockSize = 10;
            parallelPSXPreferences.ThreadsCount = 3;

            SequentialPSXPreferences sequentialPSXPreferences = new SequentialPSXPreferences();

            sequentialPSXPreferences.BlockSize = 10;

            this.imageResult1 = this.parallelPSX!.Apply(this.imageInput!, parallelPSXPreferences);
            this.imageResult2 = this.sequentialPSX!.Apply(this.imageInput!, sequentialPSXPreferences);

            Assert.IsTrue(this.AreImagesEqual(this.imageResult1, this.imageResult2));
        }

        public void Apply_Image1k_BlockSize15_ThreadsCount2_ReturnsBitmap()
        {
            ParallelPSXPreferences parallelPSXPreferences = new ParallelPSXPreferences();

            parallelPSXPreferences.BlockSize = 10;
            parallelPSXPreferences.ThreadsCount = 3;

            SequentialPSXPreferences sequentialPSXPreferences = new SequentialPSXPreferences();

            sequentialPSXPreferences.BlockSize = 10;

            this.imageResult1 = this.parallelPSX!.Apply(this.imageInput!, parallelPSXPreferences);
            this.imageResult2 = this.sequentialPSX!.Apply(this.imageInput!, sequentialPSXPreferences);

            Assert.IsTrue(this.AreImagesEqual(this.imageResult1, this.imageResult2));
        }

        public void Apply_Image1k_BlockSize15_ThreadsCount3_ReturnsBitmap()
        {
            ParallelPSXPreferences parallelPSXPreferences = new ParallelPSXPreferences();

            parallelPSXPreferences.BlockSize = 10;
            parallelPSXPreferences.ThreadsCount = 3;

            SequentialPSXPreferences sequentialPSXPreferences = new SequentialPSXPreferences();

            sequentialPSXPreferences.BlockSize = 10;

            this.imageResult1 = this.parallelPSX!.Apply(this.imageInput!, parallelPSXPreferences);
            this.imageResult2 = this.sequentialPSX!.Apply(this.imageInput!, sequentialPSXPreferences);

            Assert.IsTrue(this.AreImagesEqual(this.imageResult1, this.imageResult2));
        }

        private bool AreImagesEqual(Bitmap image1, Bitmap image2)
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return false;
            }

            for (int x = 0; x < image1.Width; ++x)
            {
                for (int y = 0; y < image1.Height; ++y)
                {
                    if (image1.GetPixel(x, y) != image2.GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}