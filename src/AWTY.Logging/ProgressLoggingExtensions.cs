using Microsoft.Extensions.Logging;
using System;

namespace AWTY
{
    using Logging;

    /// <summary>
    ///     Logging-related extension methods for progress.
    /// </summary>
    public static class ProgressLoggingExtensions
    {
        /// <summary>
        ///     Write progress data to a logger.
        /// </summary>
        /// <param name="source">
        ///     The sequence of progress data to log.
        /// </param>
        /// <param name="logger">
        ///     The <see cref="ILogger"/>.
        /// </param>
        /// <param name="name">
        ///     The name used to identify progress data in log entries.
        /// </param>
        /// <param name="level">
        ///     The level at which progress data should be logged.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the subscription; when disposed, progress data will no longer be logged.
        /// </returns>
        public static IDisposable LogTo<TValue>(this IObservable<ProgressData<TValue>> source, ILogger logger, string name, LogLevel level = LogLevel.Information)
            where TValue : IEquatable<TValue>, IComparable<TValue>
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Subscribe(
                new ProgressLoggingAdapter<TValue>(logger, name, level)
            );
        }

        /// <summary>
        ///     Write progress data to a logger.
        /// </summary>
        /// <param name="source">
        ///     The sequence of progress data to log.
        /// </param>
        /// <param name="logger">
        ///     The <see cref="ILogger"/>.
        /// </param>
        /// <param name="name">
        ///     The name used to identify progress data in log entries.
        /// </param>
        /// <param name="level">
        ///     The level at which progress data should be logged.
        /// </param>
        /// <returns>
        ///     The input sequence (enables inline use).
        /// </returns>
        /// <remarks>
        ///     This method can be used inline, but logging will continue for the lifetime for the sequence.
        ///     Use the non-inline <see cref="LogTo{TValue}"/> method if you want to control the lifetime of the logging subscription.
        /// </remarks>
        public static IObservable<ProgressData<TValue>> WithLogging<TValue>(this IObservable<ProgressData<TValue>> source, ILogger logger, string name, LogLevel level = LogLevel.Information)
            where TValue : IEquatable<TValue>, IComparable<TValue>
        {
            source.LogTo(logger, name, level);

            return source;
        }
    }
}