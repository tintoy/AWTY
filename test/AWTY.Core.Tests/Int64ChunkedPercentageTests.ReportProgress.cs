using System;
using System.Collections.Generic;
using System.Reactive;
using Xunit;

namespace AWTY.Core.Tests
{
    using Strategies;

    /// <summary>
    ///     Tests for the <see cref="Int64ChunkedPercentageStrategy"/> progress-notification strategy.
    /// </summary>
    public partial class Int64ChunkedPercentageTests
    {
        /// <summary>
        ///     Report progress to a <see cref="Int64ChunkedPercentageStrategy"/> progress-notification strategy.
        /// </summary>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        /// <param name="increment">
        ///     The value to increment progress by in each iteration.
        /// </param>
        /// <param name="chunkSize">
        ///     The minimum change in progress to notify.
        /// </param>
        /// <param name="expectedPercentages">
        ///     An array of the expected completion percentages from notifications.
        /// </param>
        [Theory]
        [MemberData(nameof(ReportProgressTheoryData))]
        public void ReportProgress(long total, long increment, int chunkSize, int[] expectedPercentages)
        {
            List<int> actualPercentages = new List<int>();
            
            ProgressStrategy2<long> strategy = ProgressStrategy2.PercentComplete.Chunked.Int64(chunkSize);
            strategy.Subscribe(progress =>
            {
                actualPercentages.Add(progress.PercentComplete);
            });

            long adjustedTotal = AdjustTotalForIncrement(total, increment);
            for (long currentProgress = 0; currentProgress <= adjustedTotal; currentProgress += increment)
            {
                strategy.AsObserver().OnNext(
                    RawProgressData.Create(currentProgress, total)
                );
            }

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        // TODO: Theory test for Remove.

        /// <summary>
        ///     Data for the <see cref="ReportProgress"/> theory test.
        /// </summary>
        public static IEnumerable<object[]> ReportProgressTheoryData => TestData.Theory.ChunkedPercentage64;

        /// <summary>
        ///     Adjust the total to yield the correct number of iterations, accounting for the specified increment.
        /// </summary>
        /// <remarks>
        ///     Handles the case where the increment value causes the last iteration's progress value to be less than the total.
        /// </remarks>
        static long AdjustTotalForIncrement(long total, long increment) => total + (total % increment) + 1;
    }
}