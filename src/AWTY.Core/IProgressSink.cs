using System;

namespace AWTY
{
    /// <summary>
    ///     Represents a sink for reporting progress.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The type of value used to represent progress.
    /// </typeparam>
    /// <remarks>
    ///     Progress sinks should not propagate values with a <see cref="RawProgressData{TValue}.Total"/> of less than 1.
    /// </remarks>
    public interface IProgressSink<TValue>
        : IObservable<RawProgressData<TValue>>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        /// <summary>
        ///     The current progress value.
        /// </summary>
        TValue Current { get; set; }

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
