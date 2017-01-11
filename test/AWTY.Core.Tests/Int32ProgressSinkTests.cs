using Xunit;

namespace AWTY.Core.Tests
{
    /// <summary>
    ///     Tests for <see cref="Sinks.Int32ProgressSink"/>.
    /// </summary>
    public partial class Int32ProgressSinkTests
        : IClassFixture<StrategyFactory>
    {
        /// <summary>
        ///     The progress strategy factory.
        /// </summary>
        readonly StrategyFactory _strategies;

        /// <summary>
        ///     Create a new <see cref="Sinks.Int32ProgressSink"/> test suite.
        /// </summary>
        /// <param name="strategies">
        ///     The progress strategy factory.
        /// </param>
        public Int32ProgressSinkTests(StrategyFactory strategies)
        {
            _strategies = strategies;
        }
    }
}
