using System.Drawing;

using WindowsWizard.Filters;

namespace WindowsWizard.Tests.Filters.ParallelPSX
{
    [TestClass]
    public sealed class ParallelPSXTests
    {
        private Bitmap? image1k;
        private Bitmap? image2k;
        private Bitmap? image3k;
        private Bitmap? image5k;
        private Bitmap? image10k;
        private Bitmap? image15k;
        private Bitmap? image20k;

        private Bitmap? imageResult;

        private WindowsWizard.Filters.ParallelPSX? parallelPSX;

        //private readonly int[] blockSizes = new int[] { 10, 20, 30, 40, 50 };
        //private readonly int[] threadsCounts = new int[] { 2, 3, 4, 6, 9, 12  };

        [TestInitialize]
        public void Setup()
        {
            this.parallelPSX = new WindowsWizard.Filters.ParallelPSX();

            this.image1k = new Bitmap(@"../../../../../tests/Assets/1k.png");
            this.image2k = new Bitmap(@"../../../../../tests/Assets/2k.png");
            this.image3k = new Bitmap(@"../../../../../tests/Assets/3k.png");
            this.image5k = new Bitmap(@"../../../../../tests/Assets/5k.png");
            this.image10k = new Bitmap(@"../../../../../tests/Assets/10k.png");
            this.image15k = new Bitmap(@"../../../../../tests/Assets/15k.png");
            this.image20k = new Bitmap(@"../../../../../tests/Assets/20k.png");
        }

        [TestMethod]
        public void Apply_Image1k_BlockSize10_ThreadsCount2_ReturnsBitmap()
        {
            ParallelPSXPreferences parallelPSXPreferences = new ParallelPSXPreferences();

            parallelPSXPreferences.BlockSize = 10;
            parallelPSXPreferences.ThreadsCount = 2;
            
            this.imageResult = this.parallelPSX!.Apply(this.image1k!, parallelPSXPreferences);

            Assert.IsNotNull(this.imageResult);
        }
    }
}