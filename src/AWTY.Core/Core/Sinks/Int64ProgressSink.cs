using System;
using System.Threading;

namespace AWTY.Core.Sinks
{
    /// <summary>
    ///     A sink for reporting progress as a 64-bit integer.
    /// </summary>
    public class Int64ProgressSink
        : IProgressSink<long>
    {
        /// <summary>
        ///     The strategy used to determine when progress has changed.
        /// </summary>
        readonly IProgressStrategy<long> _strategy;

        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        long                             _total;

        /// <summary>
        ///     The current progress value.
        /// </summary>
        long                             _current;

        /// <summary>
        ///     Create a new <see cref="Int64ProgressSink"/> with a <see cref="Total"/> of 100.
        /// </summary>
        /// <param name="strategy">
        ///     The strategy used to determine when progress has changed.
        /// </param>
        public Int64ProgressSink(IProgressStrategy<long> strategy)
            : this(strategy, total: 100)
        {
        }

        /// <summary>
        ///     Create a new <see cref="Int64ProgressSink"/>.
        /// </summary>
        /// <param name="strategy">
        ///     The strategy used to determine when progress has changed.
        /// </param>
        /// <param name="total">
        ///     The initial progress total.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="total"/> is less than 1.
        /// </exception>
        public Int64ProgressSink(IProgressStrategy<long> strategy, long total)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));

            if (total < 1)
                throw new ArgumentOutOfRangeException(nameof(total), total, "Progress total cannot be less than 1.");

            _strategy = strategy;
            _total = total;
        }

        /// <summary>
        ///     The current progress value.
        /// </summary>
        public long Current => _current;

        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Attempted to set a value less than 1.
        /// </exception>
        public long Total
        {
            get
            {
                return _total;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(Total), value, "Progress total cannot be less than 1.");

                long current = _current;

                long total = Interlocked.Exchange(ref _total, value);
                if (total != value)
                    _strategy.ReportProgress(current, total);
            }
        }

        /// <summary>
        ///     Add the specified value to the current progress value.
        /// </summary>
        /// <returns>
        ///     The updated progress value.
        /// </returns>
        public long Add(long value)
        {
            long total = _total;

            long current = Interlocked.Add(ref _current, value);
            if (current != value)
                _strategy.ReportProgress(current, total);

            return current;
        }

        /// <summary>
        ///     Subtract the specified value from the current progress value.
        /// </summary>
        /// <returns>
        ///     The updated progress value.
        /// </returns>
        public long Subtract(long value)
        {
            long total = _total;
            long current = Interlocked.Add(ref _current, -value);
            if (current != value)
                _strategy.ReportProgress(current, total);

            return current;
        }

        /// <summary>
        ///     Reset the current progress value to 0.
        /// </summary>
        public void Reset()
        {
            long total = _total;
            long previous = Interlocked.Exchange(ref _current, 0);
            if (previous != 0)
                _strategy.ReportProgress(0, total);
        }
    }
}
