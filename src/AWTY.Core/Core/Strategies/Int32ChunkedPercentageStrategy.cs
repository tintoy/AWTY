using System;

namespace AWTY.Core.Strategies
{
    /// <summary>
    ///     Progress notification strategy that reports 32-bit integer progress when percentage completion changes exceed a specified value.
    /// </summary>
    public class Int32ChunkedPercentageStrategy
        : ProgressStrategy<int>
    {
        /// <summary>
        ///     An object used to synchronise access to state data.
        /// </summary>
        readonly object _stateLock = new object();

        /// <summary>
        ///     The minimum change in percentage completion to report.
        /// </summary>
        readonly int _chunkSize;

        /// <summary>
        ///     The current percentage of completion.
        /// </summary>
        int _currentPercentComplete;

        /// <summary>
        ///     Create a new <see cref="Int32ChunkedPercentageStrategy"/> progress-notification strategy.
        /// </summary>
        /// <param name="chunkSize">
        ///     The minimum change in percentage completion to report.
        /// </param>
        public Int32ChunkedPercentageStrategy(int chunkSize)
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
        public override void ReportProgress(int current, int total)
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
            
            OnProgressChanged(current, total, percentComplete);
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
        int CalculatePercentComplete(int current, int total)
        {
            if (current >= total)
                return 100;
            
            return (int)(((float)current / total) * 100);
        }
    }
}