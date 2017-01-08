using System;

namespace AWTY.Core.Strategies
{
    /// <summary>
    ///     The base class for progress-notification strategies.
    /// </summary>
    public abstract class ProgressStrategy<TValue>
        : IProgressStrategy<TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        /// <summary>
        ///     Raised when progress has changed.
        /// </summary>
        public event EventHandler<DetailedProgressEventArgs<TValue>> ProgressChanged;

        /// <summary>
        ///     Create a new progress strategy.
        /// </summary>
        protected ProgressStrategy()
        {
        }

        /// <summary>
        ///     Report the current progress.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        public abstract void ReportProgress(TValue current, TValue total);

        /// <summary>
        ///     Called when progress has changed; raises the <see cref="ProgressChanged"/> event.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        /// <param name="percentComplete">
        ///     The percentage of completion.
        /// </param>
        protected virtual void OnProgressChanged(TValue current, TValue total, int percentComplete)
        {
            ProgressChanged?.Invoke(this, new DetailedProgressEventArgs<TValue>(
                percentComplete, current, total 
            ));
        }
    }
}