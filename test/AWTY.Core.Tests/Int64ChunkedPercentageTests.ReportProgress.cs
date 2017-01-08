using System.Collections.Generic;
using Xunit;

namespace AWTY.Core.Tests
{
    using Strategies;

    /// <summary>
    ///     Tests for the <see cref="Int64ChunkedPercentage"/> progress-reporting strategy.
    /// </summary>
    public partial class Int64ChunkedPercentageTests
    {
        [Fact]
        public void Add_1_UpTo_100_ChunkSize_20()
        {
            const long increment = 1;
            const long total = 100;
            const long chunkSize = 20;
            int[] expectedPercentages = { 20, 40, 60, 80, 100 };

            List<int> actualPercentages = new List<int>();
            
            IProgressStrategy<long> strategy = new Int64ChunkedPercentage(chunkSize);
            strategy.ProgressChanged += (sender, args) =>
            {
                actualPercentages.Add(args.PercentComplete);
            };

            for (long currentProgress = 0; currentProgress <= total; currentProgress += increment)
                strategy.ReportProgress(currentProgress, total);

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        [Fact]
        public void Add_3_UpTo_100_ChunkSize_5()
        {
            const long increment = 3;
            const long total = 100;
            const long chunkSize = 5;
            int[] expectedPercentages = { 6, 12, 18, 24, 30, 36, 42, 48, 54, 60, 66, 72, 78, 84, 90, 96 };

            List<int> actualPercentages = new List<int>();
            
            IProgressStrategy<long> strategy = new Int64ChunkedPercentage(chunkSize);
            strategy.ProgressChanged += (sender, args) =>
            {
                actualPercentages.Add(args.PercentComplete);
            };

            for (long currentProgress = 0; currentProgress <= total; currentProgress += increment)
                strategy.ReportProgress(currentProgress, total);

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }
    }
}