using System;

namespace AWTY.Core.Strategies
{
    /// <summary>
    ///     Progress notification strategy that reports 64-bit integer progress when percentage completion changes exceed a specified value.
    /// </summary>
    public class Int64ChunkedPercentageStrategy2
        : ProgressStrategy2<long>
    {
        /// <summary>
        ///     An object used to synchronise access to state data.
        /// </summary>
        readonly object _stateLock = new object();

        /// <summary>
        ///     The minimum change in percentage completion to report.
        /// </summary>
        readonly int    _chunkSize;

        /// <summary>
        ///     The current percentage of completion.
        /// </summary>
        int             _currentPercentComplete;

        /// <summary>
        ///     Create a new <see cref="Int64ChunkedPercentageStrategy2"/> progress-notification strategy.
        /// </summary>
        /// <param name="chunkSize">
        ///     The minimum change in percentage completion to report.
        /// </param>
        public Int64ChunkedPercentageStrategy2(int chunkSize)
        {
            if (chunkSize < 1)
                throw new ArgumentOutOfRangeException(nameof(chunkSize), chunkSize, "Chunk size cannot be less than 1.");

            _chunkSize = chunkSize;
        }

        /// <summary>
        ///     The minimum change in percentage completion to report.
        /// </summary>
        public int ChunkSize => _chunkSize;

        /// <summary>
        ///     Report the current progress.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        protected override void ReportProgress(long current, long total)
        {
            int percentComplete;
            lock(_stateLock)
            {
                percentComplete = CalculatePercentComplete(current, total);
                if (percentComplete == _currentPercentComplete)
                    return; // No change, so no notification.

                
                bool notify =
                    Math.Abs(percentComplete - _currentPercentComplete) >= _chunkSize
                    ||
                    percentComplete == 100; // Handle trailing partial chunk.

                if (!notify)
                    return;

                _currentPercentComplete = percentComplete;
            }
            
            NotifyProgressChanged(current, total, percentComplete);
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