using System;
using System.Reactive.Linq;

namespace AWTY
{
    /// <summary>
    ///     Rx-related extension methods for HTTP progress reporting.
    /// </summary>
    public static class HttpReactiveProgressExtensions
    {
        /// <summary>
        ///     Filter the progress-started notifications so only ones from the current <see cref="ProgressContext"/> are kept.
        /// </summary>
        /// <param name="progressStarted">
        ///     The progress-started notification sequence.
        /// </param>
        /// <returns>
        ///     A new sequence containing only notifications from the current progress context (where <see cref="ForCurrentProgressContext"/> was called).
        /// </returns>
        public static IObservable<TProgressStarted> ForCurrentProgressContext<TProgressStarted>(this IObservable<TProgressStarted> progressStarted)
            where TProgressStarted : HttpProgressStarted
        {
            return progressStarted.ForProgressContext(ProgressContext.Current.Id);
        }

        /// <summary>
        ///     Filter the progress-started notifications so only ones from the specified <see cref="ProgressContext"/> are kept.
        /// </summary>
        /// <param name="progressStarted">
        ///     The progress-started notification sequence.
        /// </param>
        /// <param name="progressContextId">
        ///     The If of the progress context to match.
        /// </param>
        /// <returns>
        ///     A new sequence containing only notifications from the current progress context.
        /// </returns>
        public static IObservable<TProgressStarted> ForProgressContext<TProgressStarted>(this IObservable<TProgressStarted> progressStarted, string progressContextId)
            where TProgressStarted : HttpProgressStarted
        {
            return progressStarted.Where(
                started => started.ProgressContextId == progressContextId
            );
        }
    }
}