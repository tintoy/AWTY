using System;

namespace AWTY
{
    /// <summary>
    ///     Basic progress data.
    /// </summary>
    public class ProgressData
    {
        /// <summary>
        ///     Create new basic progress data.
        /// </summary>
        /// <param name="percentComplete">
        ///     The percentage completion.
        /// </param>
        public ProgressData(int percentComplete)
        {
            PercentComplete = percentComplete;
        }

        /// <summary>
        ///     The percentage completion.
        /// </summary>
        public int PercentComplete { get; }

        /// <summary>
        ///     Create new basic progress data.
        /// </summary>
        /// <param name="percentComplete">
        ///     The percentage completion.
        /// </param>
        /// <returns>
        ///     The progress data.
        /// </returns>
        public static ProgressData Create(int percentComplete)
        {
            return new ProgressData(percentComplete);
        }

        /// <summary>
        ///     Create new detailed progress data.
        /// </summary>
        /// <param name="percentComplete">
        ///     The percentage completion.
        /// </param>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        /// <returns>
        ///     The progress data.
        /// </returns>
        public static ProgressData<TValue> Create<TValue>(int percentComplete, TValue current, TValue total)
            where TValue : IEquatable<TValue>, IComparable<TValue>
        {
            return new ProgressData<TValue>(percentComplete, current, total);
        }

        /// <summary>
        ///     Create new detailed progress from raw progress data.
        /// </summary>
        /// <param name="raw">
        ///     The raw progress data.
        /// </param>
        /// <param name="percentComplete">
        ///     The percentage completion.
        /// </param>
        /// <returns>
        ///     The progress data.
        /// </returns>
        public static ProgressData<TValue> Create<TValue>(RawProgressData<TValue> raw, int percentComplete)
            where TValue : IEquatable<TValue>, IComparable<TValue>
        {
            return new ProgressData<TValue>(percentComplete, raw.Current, raw.Total);
        }
    }

    /// <summary>
    ///     Detailed progress data.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The type of value used to represent progress.
    /// </typeparam>
    public class ProgressData<TValue>
        : ProgressData
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        /// <summary>
        ///     Create new detailed progress data.
        /// </summary>
        /// <param name="percentComplete">
        ///     The percentage completion.
        /// </param>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        public ProgressData(int percentComplete, TValue current, TValue total)
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

        /// <summary>
        ///     Create a copy of the progress data, but with the specified percentage completion.
        /// </summary>
        /// <param name="percentComplete">
        ///     The new percentage completion.
        /// </param>
        /// <returns>
        ///     The new <see cref="ProgressData{TValue}"/>.
        /// </returns>
        public ProgressData<TValue> WithPercentComplete(int percentComplete)
        {
            if (percentComplete == PercentComplete)
                return this;

            return new ProgressData<TValue>(percentComplete, Current, Total);
        }

        /// <summary>
        ///     Create a copy of the progress data, but with the specified progress value.
        /// </summary>
        /// <param name="current">
        ///     The new progress value.
        /// </param>
        /// <returns>
        ///     The new <see cref="ProgressData{TValue}"/>.
        /// </returns>
        public ProgressData<TValue> WithCurrent(TValue current)
        {
            if (Current.Equals(current))
                return this;

            return new ProgressData<TValue>(PercentComplete, current, Total);
        }

        /// <summary>
        ///     Create a copy of the progress data, but with the specified total.
        /// </summary>
        /// <param name="total">
        ///     The new total.
        /// </param>
        /// <returns>
        ///     The new <see cref="ProgressData{TValue}"/>.
        /// </returns>
        public ProgressData<TValue> WithTotal(TValue total)
        {
            if (Total.Equals(total))
                return this;

            return new ProgressData<TValue>(PercentComplete, Current, total);
        }
    }
}