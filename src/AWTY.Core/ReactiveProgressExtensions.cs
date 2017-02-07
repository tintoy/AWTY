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

        /// <summary>
        ///     Subscribe an <see cref="IProgress{TValue}"/> to progress percentage-completion notifications.
        /// </summary>
        /// <param name="source">
        ///     The sequence of progress notifications.
        /// </param>
        /// <param name="progress">
        ///     The <see cref="IProgress{TValue}"/>.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the subscription.
        /// </returns>
        public static IDisposable SubscribePercentComplete(this IObservable<ProgressData> source, IProgress<int> progress)
        {
            return source.Subscribe(
                progressData => progress.Report(progressData.PercentComplete)
            );
        }

        /// <summary>
        ///     Subscribe an <see cref="IProgress{TValue}"/> to detailed progress notifications.
        /// </summary>
        /// <param name="source">
        ///     The sequence of progress notifications.
        /// </param>
        /// <param name="progress">
        ///     The <see cref="IProgress{TValue}"/>.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the subscription.
        /// </returns>
        public static IDisposable Subscribe<TValue>(this IObservable<ProgressData<TValue>> source, IProgress<ProgressData<TValue>> progress)
            where TValue : IEquatable<TValue>, IComparable<TValue>
        {
            return source.Subscribe(
                progressData => progress.Report(progressData)
            );
        }

        /// <summary>
        ///     Subscribe an <see cref="IProgress{TValue}"/> to basic progress notifications.
        /// </summary>
        /// <param name="source">
        ///     The sequence of progress notifications.
        /// </param>
        /// <param name="progress">
        ///     The <see cref="IProgress{TValue}"/>.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the subscription.
        /// </returns>
        public static IDisposable Subscribe<TValue>(this IObservable<ProgressData<TValue>> source, IProgress<ProgressData> progress)
            where TValue : IEquatable<TValue>, IComparable<TValue>
        {
            return source.Subscribe(
                progressData => progress.Report(progressData)
            );
        }

        /// <summary>
        ///     Subscribe an <see cref="IProgress{TValue}"/> to progress value notifications.
        /// </summary>
        /// <param name="source">
        ///     The sequence of progress notifications.
        /// </param>
        /// <param name="progress">
        ///     The <see cref="IProgress{TValue}"/>.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the subscription.
        /// </returns>
        public static IDisposable Subscribe<TValue>(this IObservable<ProgressData<TValue>> source, IProgress<TValue> progress)
            where TValue : IEquatable<TValue>, IComparable<TValue>
        {
            return source.Subscribe(
                progressData => progress.Report(progressData.Current)
            );
        }
    }
}