using System;
using System.Collections.Generic;
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
    public class ProgressHandlerTests
        : IClassFixture<TestServer>
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
            const long responseSize = 10 * 1024;
            int[] expectedPercentages = { 40, 80, 100 };

            List<int> actualPercentages = new List<int>();

            ProgressHandler progressHandler = new ProgressHandler(
                nextHandler: new HttpClientHandler(),
                progressTypes: HttpProgressTypes.Response
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

                    Output.WriteLine("{0} '{1}' ({2}% complete)...",
                        responseStarted.RequestMethod,
                        responseStarted.RequestUri,
                        progress.PercentComplete
                    );
                });
            });
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
    }
}