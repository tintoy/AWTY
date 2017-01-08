using System;
using System.Threading;

namespace AWTY.Core.Sinks
{
    /// <summary>
    ///     A sink for reporting progress as a 32-bit integer.
    /// </summary>
    public class Int32ProgressSink
        : IProgressSink<int>
    {
        /// <summary>
        ///     The strategy used to determine when progress has changed.
        /// </summary>
        readonly IProgressStrategy<int> _strategy;

        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        int                             _total;

        /// <summary>
        ///     The current progress value.
        /// </summary>
        int                             _current;

        /// <summary>
        ///     Create a new <see cref="Int32ProgressSink"/> with a <see cref="Total"/> of 100.
        /// </summary>
        /// <param name="strategy">
        ///     The strategy used to determine when progress has changed.
        /// </param>
        public Int32ProgressSink(IProgressStrategy<int> strategy)
            : this(strategy, total: 100)
        {
        }

        /// <summary>
        ///     Create a new <see cref="Int32ProgressSink"/>.
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
        public Int32ProgressSink(IProgressStrategy<int> strategy, int total)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));

            if (total < 1)
                throw new ArgumentOutOfRangeException(nameof(total), total, "Progress total cannot be less than 1.");

            _strategy = strategy;
            _total = total;
        }

        /// <summary>
        ///     The strategy used to determine when progress should be reported.
        /// </summary>
        public IProgressStrategy<int> Strategy => _strategy;

        /// <summary>
        ///     The current progress value.
        /// </summary>
        public int Current => _current;

        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Attempted to set a value less than 1.
        /// </exception>
        public int Total
        {
            get
            {
                return _total;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(Total), value, "Progress total cannot be less than 1.");

                int current = _current;

                int total = Interlocked.Exchange(ref _total, value);
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
        public int Add(int value)
        {
            int total = _total;

            int current = Interlocked.Add(ref _current, value);
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
        public int Subtract(int value)
        {
            int total = _total;
            int current = Interlocked.Add(ref _current, -value);
            if (current != value)
                _strategy.ReportProgress(current, total);

            return current;
        }

        /// <summary>
        ///     Reset the current progress value to 0.
        /// </summary>
        public void Reset()
        {
            int total = _total;
            int previous = Interlocked.Exchange(ref _current, 0);
            if (previous != 0)
                _strategy.ReportProgress(0, total);
        }
    }
}
