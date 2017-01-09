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
    }
}