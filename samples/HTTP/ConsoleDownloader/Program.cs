using AWTY;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace ConsoleDownloader
{
    /// <summary>
    ///     Console application that downloads a file over HTTP, reporting its progress with a progress bar.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     The main program entry-point.
        /// </summary>
        public static void Main()
        {
            Console.WriteLine("Downloading...");
            ConsoleProgressBar progressBar = new ConsoleProgressBar("Progress");

            // Quick-and-dirty demo to simulate progress so we can drive the ConsoleProgressBar.
            // TODO: Download file and use that progress instead.

            ManualResetEvent completion = new ManualResetEvent(false);
            long currentValue = 0;
            Observable.Interval(TimeSpan.FromMilliseconds(200))
                .Select(_ =>
                {
                    currentValue += 10;

                    return new ProgressData<long>(
                        percentComplete: (int)currentValue,
                        current: currentValue,
                        total: 100
                    );
                })
                .TakeWhile(progress =>
                {
                    if (progress.PercentComplete == 100)
                        completion.Set();
                    else if (progress.PercentComplete > 100)
                    {
                        // Cheat.
                        progressBar.AsObserver().OnCompleted();

                        return false; // Done.
                    }

                    return true; // Keep going.
                })
                .Subscribe(progressBar);

            completion.WaitOne();
            Thread.Sleep(1000);

            Console.WriteLine("Done.");
        }
    }
}
