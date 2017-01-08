using System;

namespace AWTY
{
    /// <summary>
    ///     Represents a strategy for accumulating and reporting progress.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The type of value used to represent progress.
    /// </typeparam>
    public interface IProgressStrategy<TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        /// <summary>
        ///     Raised when the strategy has determined that progress has changed.
        /// </summary>
        event EventHandler<DetailedProgressEventArgs<TValue>> ProgressChanged;

        /// <summary>
        ///     Report the current progress.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        void ReportProgress(TValue current, TValue total);
    }
}