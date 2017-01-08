using System;

namespace AWTY.Core.Strategies
{
    /// <summary>
    ///     Progress reporting strategy that reports 32-bit integer progress when percentage completion changes exceed a specified value.
    /// </summary>
    public class Int32ChunkedPercentage
        : IProgressStrategy<int>
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
        ///     Raised when the strategy determines that progress has changed.
        /// </summary>
        public event EventHandler<DetailedProgressEventArgs<int>> ProgressChanged;

        /// <summary>
        ///     Create a new <see cref="Int32ChunkedPercentage"/> progress-reporting strategy.
        /// </summary>
        /// <param name="chunkSize">
        ///     The minimum change in percentage completion to report.
        /// </param>
        public Int32ChunkedPercentage(int chunkSize)
        {
            if (chunkSize < 1)
                throw new ArgumentOutOfRangeException(nameof(chunkSize), chunkSize, "Chunk size cannot be less than 1.");

            _chunkSize = chunkSize;
        }

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
            lock(_stateLock)
            {
                int percentComplete = CalculatePercentComplete(current, total);
                if (Math.Abs(percentComplete - _currentPercentComplete) < _chunkSize)
                    return;

                _currentPercentComplete = percentComplete;
                ProgressChanged?.Invoke(this, new DetailedProgressEventArgs<int>(
                    percentComplete,
                    current,
                    total
                ));
            }
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