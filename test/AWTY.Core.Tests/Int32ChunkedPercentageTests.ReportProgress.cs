using System.Collections.Generic;
using Xunit;

namespace AWTY.Core.Tests
{
    using Strategies;

    /// <summary>
    ///     Tests for the <see cref="Int32ChunkedPercentage"/> progress-reporting strategy.
    /// </summary>
    public partial class Int32ChunkedPercentageTests
    {
        [Fact]
        public void Add_1_UpTo_100_ChunkSize_20()
        {
            const int increment = 1;
            const int total = 100;
            const int chunkSize = 20;
            int[] expectedPercentages = { 20, 40, 60, 80, 100 };

            List<int> actualPercentages = new List<int>();
            
            IProgressStrategy<int> strategy = new Int32ChunkedPercentage(chunkSize);
            strategy.ProgressChanged += (sender, args) =>
            {
                actualPercentages.Add(args.PercentComplete);
            };

            for (int currentProgress = 0; currentProgress <= total; currentProgress += increment)
                strategy.ReportProgress(currentProgress, total);

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        [Fact]
        public void Add_3_UpTo_100_ChunkSize_5()
        {
            const int increment = 3;
            const int total = 100;
            const int chunkSize = 5;
            int[] expectedPercentages = { 6, 12, 18, 24, 30, 36, 42, 48, 54, 60, 66, 72, 78, 84, 90, 96 };

            List<int> actualPercentages = new List<int>();
            
            IProgressStrategy<int> strategy = new Int32ChunkedPercentage(chunkSize);
            strategy.ProgressChanged += (sender, args) =>
            {
                actualPercentages.Add(args.PercentComplete);
            };

            for (int currentProgress = 0; currentProgress <= total; currentProgress += increment)
                strategy.ReportProgress(currentProgress, total);

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }
    }
}