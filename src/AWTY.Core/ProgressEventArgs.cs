using System;

namespace AWTY
{
    /// <summary>
    ///     Arguments for progress events.
    /// </summary>
    public class ProgressEventArgs<TValue>
        : EventArgs
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        /// <summary>
        ///     Create new <see cref="ProgressEventArgs{TValue}"/>.
        /// </summary>
        /// <param name="percentComplete">
        ///     The percentage of completion.
        /// </param>
        public ProgressEventArgs(int percentComplete)
        {
            PercentComplete = percentComplete;
        }
        
        /// <summary>
        ///     The percentage of completion.
        /// </summary>
        public int PercentComplete { get; }
    }
}