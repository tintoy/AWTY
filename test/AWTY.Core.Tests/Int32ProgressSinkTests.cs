using Xunit;

namespace AWTY.Core.Tests
{
    /// <summary>
    ///     Tests for Int32ProgressSink.
    /// </summary>
    public partial class Int32ProgressSinkTests
        : IClassFixture<StrategyFactory>
    {
        readonly StrategyFactory _strategies;

        public Int32ProgressSinkTests(StrategyFactory strategies)
        {
            _strategies = strategies;
        }
    }
}
