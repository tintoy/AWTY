using System;
using System.Threading;

namespace AWTY.Core
{
    /// <summary>
    ///     A sink for reporting progress as a 64-bit integer.
    /// </summary>
    public class Int64ProgressSink
        : IProgressSink<long>
    {
        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        long _total;

        /// <summary>
        ///     The current progress value.
        /// </summary>
        long _current;

        /// <summary>
        ///     Create a new <see cref="Int64ProgressSink"/> with a <see cref="Total"/> of 100.
        /// </summary>
        public Int64ProgressSink()
            : this(total: 100)
        {
        }

        /// <summary>
        ///     Create a new <see cref="Int64ProgressSink"/>.
        /// </summary>
        /// <param name="total">
        ///     The initial progress total.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="total"/> is less than 1.
        /// </exception>
        public Int64ProgressSink(long total)
        {
            if (total < 1)
                    throw new ArgumentOutOfRangeException(nameof(total), total, "Progress total cannot be less than 1.");

            _total = total;
        }

        /// <summary>
        ///     The percentage of completion represented by the progress value.
        /// </summary>
        public int PercentComplete
        {
            get
            {
                long current = _current;
                long total = _total;

                if (current >= total)
                    return 100;
                
                return (int)(((double)current / total) * 100);
            }
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

                _total = value;
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
            return Interlocked.Add(ref _current, value);
        }

        /// <summary>
        ///     Subtract the specified value from the current progress value.
        /// </summary>
        /// <returns>
        ///     The updated progress value.
        /// </returns>
        public long Subtract(long value)
        {
            return Interlocked.Add(ref _current, -value);
        }

        /// <summary>
        ///     Reset the current progress value to 0.
        /// </summary>
        public void Reset()
        {
            _current = 0;
        }
    }
}
