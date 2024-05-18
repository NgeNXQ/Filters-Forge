using System;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;

using WindowsWizard.Filters;

namespace WindowsWizard.Benchmark.Filters
{
    internal sealed class Program
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

        static Program()
        {
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;

            Program.stopwatch = new Stopwatch();

            Program.blockSizes = new int[] { 5, 10, 15 };
            Program.threadsCounts = new int[] { 3, 6, 9, 12 };

            Program.image1k = new Bitmap(@"../../../../../tests/Assets/1k.png");
            Program.image2k = new Bitmap(@"../../../../../tests/Assets/2k.png");
            Program.image3k = new Bitmap(@"../../../../../tests/Assets/3k.png");
            Program.image5k = new Bitmap(@"../../../../../tests/Assets/5k.png");
            Program.image10k = new Bitmap(@"../../../../../tests/Assets/10k.png");
            Program.image15k = new Bitmap(@"../../../../../tests/Assets/15k.png");
            Program.image20k = new Bitmap(@"../../../../../tests/Assets/20k.png");

            Program.images = new Bitmap[7] { Program.image1k, Program.image2k, Program.image3k, Program.image5k, Program.image10k, Program.image15k, Program.image20k };
        }

        private static void Main()
        {
            Program.RunBenchmarkParallelPSX();
            Program.RunBenchmarkSequentialPSX();

            foreach (Bitmap image in Program.images)
            {
                image.Dispose();
            }
        }

        private static void RunBenchmarkParallelPSX()
        {
            double elapsedSeconds;
            double elapsedMilliseconds;

            ParallelPSX parallelPSX = new ParallelPSX();
            ParallelPSXPreferences parallelPSXPreferences = new ParallelPSXPreferences();

            foreach (Bitmap image in Program.images)
            {
                foreach (int blockSize in Program.blockSizes)
                {
                    foreach (int threadsCount in Program.threadsCounts)
                    {
                        parallelPSXPreferences.BlockSize = blockSize;
                        parallelPSXPreferences.ThreadsCount = threadsCount;

                        Program.stopwatch.Restart();

                        parallelPSX.Apply(image, parallelPSXPreferences);

                        Program.stopwatch.Stop();

                        elapsedMilliseconds = Program.stopwatch.Elapsed.TotalMilliseconds;
                        elapsedSeconds = elapsedMilliseconds / Program.MILLISECONDS_IN_SECOND;

                        Console.WriteLine($"{nameof(ParallelPSX)} (image: {image.Width}x{image.Height} blockSize: {blockSize} threadsCount: {threadsCount}) | {elapsedSeconds.ToString(Program.FORMAT)} s ({elapsedMilliseconds.ToString(Program.FORMAT)} ms).");
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

            foreach (Bitmap image in Program.images)
            {
                foreach (int blockSize in Program.blockSizes)
                {
                    sequentialPSXPreferences.BlockSize = blockSize;

                    Program.stopwatch.Restart();

                    sequentialPSX.Apply(image, sequentialPSXPreferences);

                    Program.stopwatch.Stop();

                    elapsedMilliseconds = Program.stopwatch.Elapsed.TotalMilliseconds;
                    elapsedSeconds = elapsedMilliseconds / Program.MILLISECONDS_IN_SECOND;

                    Console.WriteLine($"{nameof(SequentialPSX)} (image: {image.Width}x{image.Height} blockSize: {blockSize}) | {elapsedSeconds.ToString(Program.FORMAT)} s ({elapsedMilliseconds.ToString(Program.FORMAT)} ms).");
                }
            }
        }
    }
}
