using System;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;

using WindowsWizard.Filters;

namespace WindowsWizard.Benchmark.Filters
{
    internal sealed class Benchmark
    {
        private const string FORMAT = "0.000";
        private const int MILLISECONDS_IN_SECOND = 1000;

        private readonly static Stopwatch stopwatch;

        private readonly static Bitmap? image1k;
        private readonly static Bitmap? image2k;
        private readonly static Bitmap? image3k;
        private readonly static Bitmap? image5k;
        private readonly static Bitmap? image10k;
        private readonly static Bitmap? image15k;
        private readonly static Bitmap? image20k;

        private static readonly Bitmap[] images;
        private static readonly int[] blockSizes;
        private static readonly int[] threadsCounts;

        static Benchmark()
        {
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;

            Benchmark.stopwatch = new Stopwatch();

            Benchmark.blockSizes = new int[] { 5, 10, 15 };
            Benchmark.threadsCounts = new int[] { 3, 6, 9, 12 };

            Benchmark.image1k = new Bitmap(@"../../../../../tests/Assets/1k.png");
            Benchmark.image2k = new Bitmap(@"../../../../../tests/Assets/2k.png");
            Benchmark.image3k = new Bitmap(@"../../../../../tests/Assets/3k.png");
            Benchmark.image5k = new Bitmap(@"../../../../../tests/Assets/5k.png");
            Benchmark.image10k = new Bitmap(@"../../../../../tests/Assets/10k.png");
            Benchmark.image15k = new Bitmap(@"../../../../../tests/Assets/15k.png");
            Benchmark.image20k = new Bitmap(@"../../../../../tests/Assets/20k.png");

            Benchmark.images = new Bitmap[7] { Benchmark.image1k, Benchmark.image2k, Benchmark.image3k, Benchmark.image5k, Benchmark.image10k, Benchmark.image15k, Benchmark.image20k };
        }

        private static void Main()
        {
            //Program.RunBenchmarkParallelPSX();
            Benchmark.RunBenchmarkSequentialPSX();

            foreach (Bitmap image in Benchmark.images)
            {
                image.Dispose();
            }

            Console.ReadKey();
        }

        private static void RunBenchmarkParallelPSX()
        {
            double elapsedSeconds;
            double elapsedMilliseconds;

            ParallelPSX parallelPSX = new ParallelPSX();
            ParallelPSXPreferences parallelPSXPreferences = new ParallelPSXPreferences();

            foreach (Bitmap image in Benchmark.images)
            {
                foreach (int blockSize in Benchmark.blockSizes)
                {
                    foreach (int threadsCount in Benchmark.threadsCounts)
                    {
                        parallelPSXPreferences.BlockSize = blockSize;
                        parallelPSXPreferences.ThreadsCount = threadsCount;

                        Benchmark.stopwatch.Restart();

                        parallelPSX.Apply(image, parallelPSXPreferences);

                        Benchmark.stopwatch.Stop();

                        elapsedMilliseconds = Benchmark.stopwatch.Elapsed.TotalMilliseconds;
                        elapsedSeconds = elapsedMilliseconds / Benchmark.MILLISECONDS_IN_SECOND;

                        Console.WriteLine($"{nameof(ParallelPSX)} (image: {image.Width}x{image.Height} blockSize: {blockSize} threadsCount: {threadsCount}) | {elapsedSeconds.ToString(Benchmark.FORMAT)} s ({elapsedMilliseconds.ToString(Benchmark.FORMAT)} ms).");
                    }  
                }
            }
        }

        private static void RunBenchmarkSequentialPSX()
        {
            double elapsedSeconds;
            double elapsedMilliseconds;

            SequentialPSX sequentialPSX = new SequentialPSX();
            SequentialPSXPreferences sequentialPSXPreferences = new SequentialPSXPreferences();

            foreach (Bitmap image in Benchmark.images)
            {
                foreach (int blockSize in Benchmark.blockSizes)
                {
                    sequentialPSXPreferences.BlockSize = blockSize;

                    Benchmark.stopwatch.Restart();

                    sequentialPSX.Apply(image, sequentialPSXPreferences);

                    Benchmark.stopwatch.Stop();

                    elapsedMilliseconds = Benchmark.stopwatch.Elapsed.TotalMilliseconds;
                    elapsedSeconds = elapsedMilliseconds / Benchmark.MILLISECONDS_IN_SECOND;

                    Console.WriteLine($"{nameof(SequentialPSX)} (image: {image.Width}x{image.Height} blockSize: {blockSize}) | {elapsedSeconds.ToString(Benchmark.FORMAT)} s ({elapsedMilliseconds.ToString(Benchmark.FORMAT)} ms).");
                }
            }
        }
    }
}
