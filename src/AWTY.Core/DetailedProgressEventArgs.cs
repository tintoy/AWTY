using System;

namespace AWTY
{
    /// <summary>
    ///     Arguments for detailed progress events.
    /// </summary>
    public class DetailedProgressEventArgs<TValue>
        : ProgressEventArgs<TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        /// <summary>
        ///     Create new <see cref="DetailedProgressEventArgs{TValue}"/>.
        /// </summary>
        /// <param name="percentComplete">
        ///     The percentage of completion.
        /// </param>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        public DetailedProgressEventArgs(int percentComplete, TValue current, TValue total)
            : base(percentComplete)
        {
            Current = current;
            Total = total;
        }

        /// <summary>
        ///     The current progress value.
        /// </summary>
        public TValue Current { get; }

        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        public TValue Total { get; }
    }
}