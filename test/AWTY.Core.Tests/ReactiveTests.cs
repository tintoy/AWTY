using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Xunit;

namespace AWTY.Core.Tests
{
    using Sinks;
    using Strategies;

    /// <summary>
    ///     Quick-and-dirty tests to play with reactive version of progress reporting.
    /// </summary>
    public class ReactiveTests
    {
        /// <summary>
        ///     Report progress directly to a <see cref="Int32ChunkedPercentageStrategy"/> progress-notification strategy.
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
        [MemberData(nameof(AddTheoryData))]
        public void ReportProgressToStrategy(int total, int increment, int chunkSize, int[] expectedPercentages)
        {
            List<int> actualPercentages = new List<int>();
            
            Int32ChunkedPercentageStrategy strategy = new Int32ChunkedPercentageStrategy(chunkSize);
            strategy.Subscribe(progressData =>
            {
                actualPercentages.Add(progressData.PercentComplete);
            });
            
            int adjustedTotal = AdjustTotalForIncrement(total, increment);
            for (int currentProgress = 0; currentProgress <= adjustedTotal; currentProgress += increment)
            {
                strategy.AsObserver().OnNext(
                    RawProgressData.Create(currentProgress, total)
                );
            }

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        /// <summary>
        ///     Report progress to a <see cref="Int32ChunkedPercentageStrategy"/> progress-notification strategy via an <see cref="Int32ProgressSink"/>.
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
        [MemberData(nameof(AddTheoryData))]
        public void ReportProgressToSink(int total, int increment, int chunkSize, int[] expectedPercentages)
        {
            List<int> actualPercentages = new List<int>();
            
            Int32ChunkedPercentageStrategy strategy = new Int32ChunkedPercentageStrategy(chunkSize);
            strategy.Subscribe(progressData =>
            {
                actualPercentages.Add(progressData.PercentComplete);
            });

            Int32ProgressSink sink = new Int32ProgressSink(initialTotal: total);
            sink.Subscribe(strategy);
            
            int iterationCount = total / increment;
            for (int iteration = 0; iteration <= iterationCount; iteration++)
                sink.Add(increment);

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        /// <summary>
        ///     Data for the <see cref="Add"/> theory test.
        /// </summary>
        public static IEnumerable<object[]> AddTheoryData => TestData.Theory.Int32ChunkedPercentage;

        /// <summary>
        ///     Adjust the total to yield the correct number of iterations, accounting for the specified increment.
        /// </summary>
        /// <remarks>
        ///     Handles the case where the increment value causes the last iteration's progress value to be less than the total.
        /// </remarks>
        static int AdjustTotalForIncrement(int total, int increment) => total + (total % increment) + 1;
    }
}