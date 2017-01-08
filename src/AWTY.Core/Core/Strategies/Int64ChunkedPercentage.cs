using System;

namespace AWTY.Core.Strategies
{
    /// <summary>
    ///     Progress reporting strategy that reports 64-bit integer progress when percentage completion changes exceed a specified value.
    /// </summary>
    public class Int64ChunkedPercentage
        : IProgressStrategy<long>
    {
        /// <summary>
        ///     An object used to synchronise access to state data.
        /// </summary>
        readonly object _stateLock = new object();

        /// <summary>
        ///     The minimum change in percentage completion to report.
        /// </summary>
        readonly long _chunkSize;

        /// <summary>
        ///     The current percentage of completion.
        /// </summary>
        int _currentPercentComplete;

        /// <summary>
        ///     Create a new <see cref="Int64ChunkedPercentage"/> progress-reporting strategy.
        /// </summary>
        /// <param name="chunkSize">
        ///     The minimum change in percentage completion to report.
        /// </param>
        public Int64ChunkedPercentage(long chunkSize)
        {
            if (chunkSize < 1)
                throw new ArgumentOutOfRangeException(nameof(chunkSize), chunkSize, "Chunk size cannot be less than 1.");

            _chunkSize = chunkSize;
        }

        /// <summary>
        ///     Raised when the strategy determines that progress has changed.
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
            int percentComplete;
            lock(_stateLock)
            {
                percentComplete = CalculatePercentComplete(current, total);
                if (Math.Abs(percentComplete - _currentPercentComplete) < _chunkSize)
                    return;

                _currentPercentComplete = percentComplete;
            }
            
            ProgressChanged?.Invoke(this, new DetailedProgressEventArgs<long>(
                percentComplete,
                current,
                total
            ));
        }

        /// <summary>
        ///     Calculate the percentage of completion, given current and total progress values.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        /// <returns>
        ///     The percentage of completion.
        /// </returns>
        int CalculatePercentComplete(long current, long total)
        {
            if (current >= total)
                return 100;
            
            return (int)(((double)current / total) * 100);
        }
    }
}