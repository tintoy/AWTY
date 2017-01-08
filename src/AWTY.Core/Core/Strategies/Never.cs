using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS0067

namespace AWTY.Core.Strategies
{
    /// <summary>
    ///     A 32-bit integer progress-reporting strategy that never reports progress.
    /// </summary>
    public sealed class Never32
        : IProgressStrategy<int>
    {
        /// <summary>
        ///     The singleton instance of the <see cref="Never32"/> progress-reporting strategy.
        /// </summary>
        public static readonly Never32 Instance = new Never32();

        /// <summary>
        ///     Default constructor.
        /// </summary>
        Never32()
        {
        }

        /// <summary>
        ///     Raised when progress has changed.
        /// </summary>
        [SuppressMessage("", "CS0067")]
        public event EventHandler<DetailedProgressEventArgs<int>> ProgressChanged;

        /// <summary>
        ///     Report the current progress.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        public void ReportProgress(int current, int total)
        {
            // Progress never changes.
        }
    }

    /// <summary>
    ///     A 32-bit integer progress-reporting strategy that never reports progress.
    /// </summary>
    public sealed class Never64
        : IProgressStrategy<long>
    {
        /// <summary>
        ///     The singleton instance of the <see cref="Never64"/> progress-reporting strategy.
        /// </summary>
        public static readonly Never64 Instance = new Never64();

        /// <summary>
        ///     Default constructor.
        /// </summary>
        Never64()
        {
        }

        /// <summary>
        ///     Raised when progress has changed.
        /// </summary>
        public event EventHandler<DetailedProgressEventArgs<long>> ProgressChanged;

        /// <summary>
        ///     Report the current progress.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        public void ReportProgress(long current, long total)
        {
            // Progress never changes.
        }
    }
}

#pragma warning restore CS0067