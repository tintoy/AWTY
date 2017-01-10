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
    public class ContentIntegrationTests
        : IClassFixture<TestServer>
    {
        readonly TestServer _testServer;
        readonly ITestOutputHelper _testOutput;

        public ContentIntegrationTests(TestServer testServer, ITestOutputHelper testOutput)
        {
            _testServer = testServer;
            _testOutput = testOutput;
        }

        [Fact]
        public async Task TestServer_ContentLength_Present()
        {
            using (HttpClient client = CreateTestServerClient())
            {
                using (HttpResponseMessage response = await client.GetAsync("test/data", HttpCompletionOption.ResponseHeadersRead))
                {
                    foreach (var header in response.Headers)
                    {
                        _testOutput.WriteLine("ResponseHeader: '{0}' -> '{1}'",
                            header.Key,
                            String.Join(",", header.Value)
                        );
                    }
                    foreach (var header in response.Content.Headers)
                    {
                        _testOutput.WriteLine("ContentHeader: '{0}' -> '{1}'",
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

        HttpClient CreateTestServerClient()
        {
            return new HttpClient
            {
                BaseAddress = _testServer.BaseUri
            };
        }
    }
}