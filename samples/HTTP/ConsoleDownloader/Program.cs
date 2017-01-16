using AWTY;
using AWTY.Http;
using System;
using System.Reactive.Linq;
using System.IO;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleDownloader
{
    /// <summary>
    ///     Console application that downloads a file over HTTP, reporting its progress with a progress bar.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     The asynchronous program entry-point.
        /// </summary>
        static async Task AsyncMain()
        {
            Uri targetUrl = new Uri("http://www.foo.com/bar.txt");
            string targetFileName = Path.GetFileName(targetUrl.LocalPath);

            Console.WriteLine("Downloading '{0}'...", targetUrl);

            ProgressHandler progressHandler = ProgressHandler.Create(
                progressTypes: HttpProgressTypes.Request,
                bufferSize: 4096
            );
            progressHandler.RequestStarted.Subscribe(requestStarted =>
            {
                ConsoleProgressBar progressBar = new ConsoleProgressBar(targetFileName);
                requestStarted.Progress.Percentage(10).Subscribe(progressBar);
            });

            // AF: Why is this retrying the request when it fails (duplicate progress bars because progressHandler.RequestStarted is called twice)?
            using (HttpClient client = new HttpClient(progressHandler))
            using (FileStream targetStream = new FileStream(targetFileName, FileMode.Create))
            using (HttpResponseMessage response = await client.GetAsync(targetUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                {
                    responseStream.CopyTo(targetStream, 4096);
                }
            }

            Console.WriteLine("Done.");
        }

        /// <summary>
        ///     The main program entry-point.
        /// </summary>
        public static void Main()
        {
            SynchronizationContext.SetSynchronizationContext(
                new SynchronizationContext()
            );
            try
            {
                SyncWait(
                    AsyncMain()
                );
            }
            catch (Exception unhandledError)
            {
                Console.WriteLine(unhandledError);
            }
        }
        
        /// <summary>
        ///     Synchronously wait for the specified task to complete.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        /// <remarks>
        ///     Unwraps the <see cref="AggregateException"/> (if any) thrown by the task.
        /// </remarks>
        static void SyncWait(Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            try
            {
                AsyncMain().Wait();
            }
            catch (AggregateException asyncMainError)
            {
                AggregateException flattened = asyncMainError.Flatten();
                if (flattened.InnerExceptions.Count > 1)
                    throw; // Genuine aggregate.

                ExceptionDispatchInfo.Capture(flattened.InnerExceptions[0])
                    .Throw();
            }
        }
    }
}
