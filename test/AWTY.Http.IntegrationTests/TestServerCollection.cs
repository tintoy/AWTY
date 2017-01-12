using Xunit;

namespace AWTY.Http.IntegrationTests
{
    using TestApplication;

    [CollectionDefinition("NeedsTestServer")]
    public sealed class TestServerCollection
        : ICollectionFixture<TestServer>
    {
    }
}