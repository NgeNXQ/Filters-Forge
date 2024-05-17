using System.Drawing;

using WindowsWizard.Filters;

namespace WindowsWizard.Tests.Filters.SequentialPSX
{
    [TestClass]
    public sealed class SequentialPSX
    {
        private Bitmap? image1k;
        private Bitmap? image2k;
        private Bitmap? image3k;
        private Bitmap? image5k;
        private Bitmap? image10k;
        private Bitmap? image15k;
        private Bitmap? image20k;

        private Bitmap? imageResult;

        private WindowsWizard.Filters.SequentialPSX? sequentialPSX;

        [TestInitialize]
        public void Setup()
        {
            this.sequentialPSX = new WindowsWizard.Filters.SequentialPSX();

            this.image1k = new Bitmap(@"../../../../../tests/Assets/1k.png");
            this.image2k = new Bitmap(@"../../../../../tests/Assets/2k.png");
            this.image3k = new Bitmap(@"../../../../../tests/Assets/3k.png");
            this.image5k = new Bitmap(@"../../../../../tests/Assets/5k.png");
            this.image10k = new Bitmap(@"../../../../../tests/Assets/10k.png");
            this.image15k = new Bitmap(@"../../../../../tests/Assets/15k.png");
            this.image20k = new Bitmap(@"../../../../../tests/Assets/20k.png");
        }

        [TestMethod]
        public void Apply_Image1k_BlockSize10_ReturnsBitmap()
        {
            SequentialPSXPreferences sequentialPSXPreferences = new SequentialPSXPreferences();

            sequentialPSXPreferences.BlockSize = 10;

            this.imageResult = this.sequentialPSX!.Apply(this.image1k!, sequentialPSXPreferences);

            Assert.IsNotNull(this.imageResult);
        }
    }
}