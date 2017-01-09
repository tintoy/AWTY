using System;

namespace AWTY
{
    /// <summary>
    ///     Factory methods for raw progress data.
    /// </summary>
    public static class RawProgressData
    {
        /// <summary>
        ///     Create new raw progress data.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        /// <returns>
        ///     The raw progress data.
        /// </returns>
        public static RawProgressData<TValue> Create<TValue>(TValue current, TValue total)
            where TValue: IEquatable<TValue>, IComparable<TValue>
        {
            return new RawProgressData<TValue>(current, total);
        }
    }

    /// <summary>
    ///     Raw (unprocessed) progress data.
    /// </summary>
    public struct RawProgressData<TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        /// <summary>
        ///     Create new raw progress data.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        public RawProgressData(TValue current, TValue total)
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

        /// <summary>
        ///     Create a copy of the raw progress data, but with the specified current progress value.
        /// </summary>
        /// <param name="current">
        ///     The new progress value.
        /// </param>
        /// <returns>
        ///     The new progress data.
        /// </returns>
        public RawProgressData<TValue> WithCurrent(TValue current)
        {
            if (Current.Equals(current))
                return this;

            return new RawProgressData<TValue>(current, Total);
        }

        /// <summary>
        ///     Create a copy of the raw progress data, but with the specified total.
        /// </summary>
        /// <param name="total">
        ///     The new total.
        /// </param>
        /// <returns>
        ///     The new progress data.
        /// </returns>
        public RawProgressData<TValue> WithTotal(TValue total)
        {
            if (Total.Equals(total))
                return this;

            return new RawProgressData<TValue>(Current, total);
        }
    }
}