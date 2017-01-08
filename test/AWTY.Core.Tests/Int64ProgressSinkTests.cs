using Xunit;

namespace AWTY.Core.Tests
{
    /// <summary>
    ///     Tests for Int64ProgressSink.
    /// </summary>
    public partial class Int64ProgressSinkTests
        : IClassFixture<StrategyFactory>
    {
        readonly StrategyFactory _strategies;

        public Int64ProgressSinkTests(StrategyFactory strategies)
        {
            _strategies = strategies;
        }
    }
}
