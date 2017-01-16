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
        ///     Buffer size to use when transferring data.
        /// </summary>
        const int BufferSize = 524288;

        /// <summary>
        ///     The asynchronous program entry-point.
        /// </summary>
        static async Task AsyncMain()
        {
            Uri targetUrl = new Uri("http://web4host.net/5MB.zip");
            string targetFileName = Path.GetFileName(targetUrl.LocalPath);

            ProgressHandler progressHandler = ProgressHandler.Create(
                progressTypes: HttpProgressTypes.Response,
                bufferSize: BufferSize
            );
            progressHandler.ResponseStarted.Subscribe(requestStarted =>
            {
                Console.WriteLine("Downloading '{0}'...", targetUrl);

                ConsoleProgressBar progressBar = new ConsoleProgressBar(targetFileName);
                requestStarted.Progress.Percentage(5).Subscribe(progressBar);
            });

            string targetFilePath = Path.Combine(
                Directory.GetCurrentDirectory(), targetFileName
            );
            using (HttpClient client = new HttpClient(progressHandler))
            using (FileStream targetStream = new FileStream(targetFilePath, FileMode.Create))
            using (HttpResponseMessage response = await client.GetAsync(targetUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                {
                    await responseStream.CopyToAsync(targetStream, BufferSize);
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
                task.Wait();
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
