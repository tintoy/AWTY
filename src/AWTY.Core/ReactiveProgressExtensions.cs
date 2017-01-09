using System;

namespace AWTY
{
    using Core.Strategies;

    /// <summary>
    ///     Rx-related extension methods for progress reporting.
    /// </summary>
    public static class ReactiveProgressExtensions
    {
        /// <summary>
        ///     Convert raw progress data to a percentage.
        /// </summary>
        /// <param name="rawProgress">
        ///     The sequence of raw progress data.
        /// </param>
        /// <param name="minimumChange">
        ///     The minimum change in percentage to report.
        /// </param>
        /// <returns>
        ///     An observable <see cref="ProgressStrategy{TValue}"/>.
        /// </returns>
        public static ProgressStrategy<int> Percentage(this IObservable<RawProgressData<int>> rawProgress, int minimumChange)
        {
            if (rawProgress == null)
                throw new ArgumentNullException(nameof(rawProgress));

            Int32ChunkedPercentageStrategy strategy = new Int32ChunkedPercentageStrategy(minimumChange);
            rawProgress.Subscribe(strategy);

            return strategy;
        }

        /// <summary>
        ///     Convert raw progress data to a percentage.
        /// </summary>
        /// <param name="rawProgress">
        ///     The sequence of raw progress data.
        /// </param>
        /// <param name="minimumChange">
        ///     The minimum change in percentage to report.
        /// </param>
        /// <returns>
        ///     An observable <see cref="ProgressStrategy{TValue}"/>.
        /// </returns>
        public static ProgressStrategy<long> Percentage(this IObservable<RawProgressData<long>> rawProgress, int minimumChange)
        {
            if (rawProgress == null)
                throw new ArgumentNullException(nameof(rawProgress));

            Int64ChunkedPercentageStrategy strategy = new Int64ChunkedPercentageStrategy(minimumChange);
            rawProgress.Subscribe(strategy);

            return strategy;
        }
    }
}