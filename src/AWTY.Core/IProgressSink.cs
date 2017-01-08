using System;

namespace AWTY
{
    /// <summary>
    ///     Represents a sink for reporting progress.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The type of value used to represent progress.
    /// </typeparam>
    public interface IProgressSink<TValue>
        where TValue : IComparable<TValue>
    {
        /// <summary>
        ///     The strategy used to determine when progress should be reported.
        /// </summary>
        IProgressStrategy<TValue> Strategy { get; }

        /// <summary>
        ///     The current progress value.
        /// </summary>
        TValue Current { get; }

        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Attempted to set a value less than 1.
        /// </exception>
        TValue Total { get; set; }

        /// <summary>
        ///     Add the specified value to the current progress value.
        /// </summary>
        /// <returns>
        ///     The updated progress value.
        /// </returns>
        TValue Add(TValue value);

        /// <summary>
        ///     Subtract the specified value from the total.
        /// </summary>
        /// <returns>
        ///     The updated progress value.
        /// </returns>
        TValue Subtract(TValue value);

        /// <summary>
        ///     Reset the current progress value to 0.
        /// </summary>
        void Reset();
    }
}
