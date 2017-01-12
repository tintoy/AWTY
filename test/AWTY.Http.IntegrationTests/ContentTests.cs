using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AWTY.Http.IntegrationTests
{
    using TestApplication;

    /// <summary>
    ///     Integration tests for <see cref="ProgressContent"/>.
    /// </summary>
    [Collection("NeedsTestServer")]
    public class ContentTests
        : IClassFixture<TestServer>
    {
        /// <summary>
        ///     Create a new content integration-test suite.
        /// </summary>
        /// <param name="testServer">
        ///     The server used for test requests.
        /// </param>
        /// <param name="testOutput">
        ///     XUnit test output.
        /// </param>
        public ContentTests(TestServer testServer, ITestOutputHelper testOutput)
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
        ///     Verify that the "Content-Length" header is present when calling the test server.
        /// </summary>
        [Fact]
        public async Task TestServer_ContentLength_Present()
        {
            using (HttpClient client = TestServer.CreateClient())
            {
                using (HttpResponseMessage response = await client.GetAsync("test/data", HttpCompletionOption.ResponseHeadersRead))
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

                    Assert.True(
                        response.Content.Headers.Contains("Content-Length")
                    );
                }
            }
        }
    }
}