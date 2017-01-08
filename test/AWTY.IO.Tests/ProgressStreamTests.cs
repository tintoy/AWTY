using System.Collections.Generic;
using System.IO;
using Xunit;

namespace AWTY.IO.Tests
{
    /// <summary>
    ///     Tests for <see cref="ProgressStream"/>.
    /// </summary>
    public class ProgressStreamTests
        : IClassFixture<StrategyFactory>, IClassFixture<StreamFactory>
    {
        /// <summary>
        ///     The factory for progress notification strategies used in tests.
        /// </summary>
        readonly StrategyFactory _strategies;

        /// <summary>
        ///     The factory for streams used in tests.
        /// </summary>
        readonly StreamFactory _streams;

        /// <summary>
        ///     Create a new <see cref="ProgressStream"/> test suite.
        /// </summary>
        /// <param name="strategies">
        ///     The factory for progress notification strategies used in tests.
        /// </param>
        /// <param name="streams">
        ///     The factory for streams used in tests.
        /// </param>
        public ProgressStreamTests(StrategyFactory strategies, StreamFactory streams)
        {
            _strategies = strategies;
            _streams = streams;
        }

        /// <summary>
        ///     Read bytes via a ProgressStream using a chunked percentage strategy.
        /// </summary>
        /// <param name="total">
        ///     The total number of bytes in the stream.
        /// </param>
        /// <param name="increment">
        ///     The number of to read in each iteration.
        /// </param>
        /// <param name="chunkSize">
        ///     The minimum change in progress to notify.
        /// </param>
        /// <param name="expectedPercentages">
        ///     An array of the expected completion percentages from notifications.
        /// </param>
        [Theory]
        [MemberData(nameof(ReadTestData))]
        public void Read_ChunkedPercentage(long total, long increment, int chunkSize, int[] expectedPercentages) 
        {
            List<int> actualPercentages = new List<int>();

            using (MemoryStream input = _streams.FillMemoryStream(total))
            using (ProgressStream progress = input.WithReadProgress(_strategies.ChunkedPercentage(chunkSize)))
            {
                progress.ProgressChanged += (sender, args) =>
                {
                    actualPercentages.Add(args.PercentComplete);
                };

                byte[] buffer = new byte[increment];
                int bytesRead = 0;
                do
                {
                    bytesRead = progress.Read(buffer, offset: 0, count: buffer.Length);
                } while(bytesRead > 0);
            }

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        /// <summary>
        ///     Data for the stream-read test theory.
        /// </summary>
        public static IEnumerable<object> ReadTestData => TestData.Theory.ProgressStreamRead;

        // TODO: Make these into fixtures.
    }
}
