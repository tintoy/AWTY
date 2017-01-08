using System;
using System.Threading;

namespace AWTY.Core
{
    /// <summary>
    ///     A sink for reporting progress as a 32-bit integer.
    /// </summary>
    public class Int32ProgressSink
        : IProgressSink<int>
    {
        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        int _total;

        /// <summary>
        ///     The current progress value.
        /// </summary>
        int _current;

        /// <summary>
        ///     Create a new <see cref="Int32ProgressSink"/> with a <see cref="Total"/> of 100.
        /// </summary>
        public Int32ProgressSink()
            : this(total: 100)
        {
        }

        /// <summary>
        ///     Create a new <see cref="Int32ProgressSink"/>.
        /// </summary>
        /// <param name="total">
        ///     The initial progress total.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="total"/> is less than 1.
        /// </exception>
        public Int32ProgressSink(int total)
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
                int current = _current;
                int total = _total;

                if (current >= total)
                    return 100;
                
                return (int)(((float)current / total) * 100);
            }
        }

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

                _total = value;
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
            return Interlocked.Add(ref _current, value);
        }

        /// <summary>
        ///     Subtract the specified value from the current progress value.
        /// </summary>
        /// <returns>
        ///     The updated progress value.
        /// </returns>
        public int Subtract(int value)
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
