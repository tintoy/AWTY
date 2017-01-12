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
        ///     Retrieve a response payload with progress reporting.
        /// </summary>
        /// <param name="bufferSize">
        ///     The buffer size for transferring data.
        /// </param>
        /// <param name="payloadSize">
        ///     The number of bytes in the payload.
        /// </param>
        /// <param name="minPercentage">
        ///     The minimum change in percentage to capture.
        /// </param>
        /// <param name="expectedPercentages">
        ///     An array of expected percentage values.
        /// </param>
        [Theory]
        [MemberData(nameof(GetPostData))]
        public async Task Get(int bufferSize, int payloadSize, int minPercentage, int[] expectedPercentages)
        {
            List<int> actualPercentages = new List<int>();

            ProgressHandler progressHandler = CreateResponseProgressHandler(bufferSize);
            progressHandler.ResponseStarted.Subscribe(responseStarted =>
            {
                responseStarted.Progress.Percentage(minPercentage).Subscribe(progress =>
                {
                    Output.WriteLine("{0}% ({1}/{2})", progress.PercentComplete, progress.Current, progress.Total);

                    actualPercentages.Add(progress.PercentComplete);
                });
            });

            using (HttpClient client = TestServer.CreateClient(progressHandler))
            {
                using (HttpResponseMessage response = await client.GetAsync($"test/data?length={payloadSize}", HttpCompletionOption.ResponseHeadersRead))
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
                    Assert.Equal(payloadSize, responseContent.Length);
                }
            }

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        /// <summary>
        ///     Send a request payload with progress reporting.
        /// </summary>
        /// <param name="bufferSize">
        ///     The buffer size for transferring data.
        /// </param>
        /// <param name="payloadSize">
        ///     The number of bytes in the payload.
        /// </param>
        /// <param name="minPercentage">
        ///     The minimum change in percentage to capture.
        /// </param>
        /// <param name="expectedPercentages">
        ///     An array of expected percentage values.
        /// </param>
        [Theory]
        [MemberData(nameof(GetPostData))]
        public async Task Post(int bufferSize, int payloadSize, int minPercentage, int[] expectedPercentages)
        {
            List<int> actualPercentages = new List<int>();

            ProgressHandler progressHandler = CreateRequestProgressHandler(bufferSize);
            progressHandler.RequestStarted.Subscribe(requestStarted =>
            {
                requestStarted.Progress.Percentage(5).Subscribe(progress =>
                {
                    actualPercentages.Add(progress.PercentComplete);

                    Output.WriteLine("{0}% ({1}/{2})", progress.PercentComplete, progress.Current, progress.Total);
                });
            });

            using (HttpClient client = TestServer.CreateClient(progressHandler))
            {
                // Use a streaming request because buffering breaks progress reporting.
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "test/echo")
                {
                    Content = new StreamContent(FillMemoryStream(payloadSize))
                    {
                        Headers =
                        {
                            ContentLength = payloadSize
                        }
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
                    Assert.Equal(payloadSize, responseContent.Length);
                }
            }

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        /// <summary>
        ///     Data for the <see cref="Get"/> and <see cref="Post"/> theories.
        /// </summary>
        public static IEnumerable<object[]> GetPostData
        {
            get
            {
                yield return TestData.ProgressHandlerRow(
                    bufferSize: 1024,
                    responseSize: 10 * 1024,
                    minPercentage: 5,
                    expectedPercentages: new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }
                );

                yield return TestData.ProgressHandlerRow(
                    bufferSize: 4096,
                    responseSize: 50 * 1024 * 1024,
                    minPercentage: 5,
                    expectedPercentages: new int[] { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 }
                );
            }
        }

        /// <summary>
        ///     Create a <see cref="ProgressHandler"/> for use in tests that publishes progress for requests.
        /// </summary>
        /// <param name="bufferSize">
        ///     The buffer size to use when transferring content.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ProgressHandler"/>.
        /// </returns>
        ProgressHandler CreateRequestProgressHandler(int bufferSize)
        {
            ProgressHandler progressHandler = new ProgressHandler(
                nextHandler: new HttpClientHandler(),
                progressTypes: HttpProgressTypes.Request,
                bufferSize: bufferSize
            );

            return progressHandler;
        }

        /// <summary>
        ///     Create a <see cref="ProgressHandler"/> for use in tests that publishes progress for responses.
        /// </summary>
        /// <param name="bufferSize">
        ///     The buffer size to use when transferring content.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ProgressHandler"/>.
        /// </returns>
        ProgressHandler CreateResponseProgressHandler(int bufferSize)
        {
            ProgressHandler progressHandler = new ProgressHandler(
                nextHandler: new HttpClientHandler(),
                progressTypes: HttpProgressTypes.Response,
                bufferSize: bufferSize
            );

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
