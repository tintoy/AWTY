using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AWTY.Http.IntegrationTests
{
    using TestApplication;

    /// <summary>
    ///     Integration tests for <see cref="ProgressHandler"/>.
    /// </summary>
    [Collection("NeedsTestServer")]
    public class ProgressHandlerTests
    {
        /// <summary>
        ///     Create a new HTTP progress handler integration-test suite.
        /// </summary>
        /// <param name="testServer">
        ///     The server used for test requests.
        /// </param>
        /// <param name="testOutput">
        ///     XUnit test output.
        /// </param>
        public ProgressHandlerTests(TestServer testServer, ITestOutputHelper testOutput)
        {
            if (testServer == null)
                throw new ArgumentNullException(nameof(testServer));

            if (testOutput == null)
                throw new ArgumentNullException(nameof(testOutput));

            TestServer = testServer;
            Output = testOutput;

            TestServer.Start(Output);
        }

        /// <summary>
        ///     Dispose of resources being used by the test suite.
        /// </summary>
        public void Dispose()
        {
            TestServer?.Dispose();
        }

        /// <summary>
        ///     The server used for test requests.
        /// </summary>
        TestServer TestServer { get; }

        /// <summary>
        ///     XUnit test output.
        /// </summary>
        ITestOutputHelper Output { get; }

        /// <summary>
        ///     Retrieve a 10KB response payload with progress reporting in chunks of 5%.
        /// </summary>
        [Fact]
        public async Task Get_Response_10K_5Percent()
        {
            const int chunkSize = 1024;
            const long responseSize = 10 * 1024;
            int[] expectedPercentages = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

            List<int> actualPercentages = new List<int>();

            ProgressHandler progressHandler = CreateResponseProgressHandler(actualPercentages, chunkSize);
            using (HttpClient client = TestServer.CreateClient(progressHandler))
            {
                using (HttpResponseMessage response = await client.GetAsync($"test/data?length={responseSize}", HttpCompletionOption.ResponseHeadersRead))
                {
                    foreach (var header in response.Headers)
                    {
                        Output.WriteLine("ResponseHeader: '{0}' -> '{1}'",
                            header.Key,
                            String.Join(",", header.Value)
                        );
                    }
                    foreach (var header in response.Content.Headers)
                    {
                        Output.WriteLine("ContentHeader: '{0}' -> '{1}'",
                            header.Key,
                            String.Join(",", header.Value)
                        );
                    }

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Assert.Equal(responseSize, responseContent.Length);
                }
            }

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        /// <summary>
        ///     Send a 10KB request payload with progress reporting in chunks of 5%.
        /// </summary>
        [Fact]
        public async Task Post_Request_10K_5Percent()
        {
            const int chunkSize = 1024;
            const int requestSize = 10 * chunkSize;
            int[] expectedPercentages = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

            List<int> actualPercentages = new List<int>();

            ProgressHandler progressHandler = CreateRequestProgressHandler(actualPercentages, chunkSize);
            using (HttpClient client = TestServer.CreateClient(progressHandler))
            {
                // Use a streaming request because buffering breaks progress reporting.
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "test/post-data")
                {
                    Content = new StreamContent(FillMemoryStream(requestSize)),
                    Headers =
                    {
                        TransferEncodingChunked = true
                    }
                };

                using (request)
                using (HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    Output.WriteLine("StatusCode: {0}", response.StatusCode);
                    foreach (var header in response.Headers)
                    {
                        Output.WriteLine("ResponseHeader: '{0}' -> '{1}'",
                            header.Key,
                            String.Join(",", header.Value)
                        );
                    }
                    foreach (var header in response.Content.Headers)
                    {
                        Output.WriteLine("ContentHeader: '{0}' -> '{1}'",
                            header.Key,
                            String.Join(",", header.Value)
                        );
                    }

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Assert.Equal(requestSize, responseContent.Length);
                }
            }

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        /// <summary>
        ///     Create a <see cref="ProgressHandler"/> for use in tests that publishes progress for requests.
        /// </summary>
        /// <param name="actualPercentages">
        ///     The list of completion percentages to update with captured values.
        /// </param>
        /// <param name="bufferSize">
        ///     The buffer size to use when transferring content.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ProgressHandler"/>.
        /// </returns>
        ProgressHandler CreateRequestProgressHandler(List<int> actualPercentages, int bufferSize)
        {
            if (actualPercentages == null)
                throw new ArgumentNullException(nameof(actualPercentages));

            ProgressHandler progressHandler = new ProgressHandler(
                nextHandler: new HttpClientHandler(),
                progressTypes: HttpProgressTypes.Request,
                bufferSize: bufferSize
            );
            progressHandler.RequestStarted.Subscribe(requestStarted =>
            {
                Output.WriteLine("Started sending {0} request for '{1}'...",
                    requestStarted.RequestMethod,
                    requestStarted.RequestUri
                );
                requestStarted.Progress.Percentage(5).Subscribe(progress =>
                {
                    actualPercentages.Add(progress.PercentComplete);

                    Output.WriteLine("{0} '{1}' (request {2}% complete)...",
                        requestStarted.RequestMethod,
                        requestStarted.RequestUri,
                        progress.PercentComplete
                    );
                });
            });

            return progressHandler;
        }

        /// <summary>
        ///     Create a <see cref="ProgressHandler"/> for use in tests that publishes progress for requests.
        /// </summary>
        /// <param name="actualPercentages">
        ///     The list of completion percentages to update with captured values.
        /// </param>
        /// <param name="bufferSize">
        ///     The buffer size to use when transferring content.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ProgressHandler"/>.
        /// </returns>
        ProgressHandler CreateResponseProgressHandler(List<int> actualPercentages, int bufferSize)
        {
            if (actualPercentages == null)
                throw new ArgumentNullException(nameof(actualPercentages));

            ProgressHandler progressHandler = new ProgressHandler(
                nextHandler: new HttpClientHandler(),
                progressTypes: HttpProgressTypes.Response,
                bufferSize: bufferSize
            );
            progressHandler.ResponseStarted.Subscribe(responseStarted =>
            {
                Output.WriteLine("Started receiving {0} response for '{1}'...",
                    responseStarted.RequestMethod,
                    responseStarted.RequestUri
                );
                responseStarted.Progress.Percentage(5).Subscribe(progress =>
                {
                    actualPercentages.Add(progress.PercentComplete);

                    Output.WriteLine("{0} '{1}' (response {2}% complete)...",
                        responseStarted.RequestMethod,
                        responseStarted.RequestUri,
                        progress.PercentComplete
                    );
                });
            });

            return progressHandler;
        }

        /// <summary>
        ///     Fill a <see cref="MemoryStream"/> with data.
        /// </summary>
        /// <param name="size">
        ///     The number of bytes to write to the stream.
        /// </param>
        /// <returns>
        ///     The <see cref="MemoryStream"/> (actually a <see cref="DumbMemoryStream"/>).
        /// </returns>
        MemoryStream FillMemoryStream(int size)
        {
            DumbMemoryStream memoryStream = new DumbMemoryStream();
            StreamWriter writer = new StreamWriter(memoryStream);
            writer.Write(
                new String('x', size)
            );
            writer.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
    }
}
