using System;
using System.Reactive.Subjects;
using System.Threading;

namespace AWTY.Core.Sinks
{
    /// <summary>
    ///     A sink for reporting progress as a 64-bit integer.
    /// </summary>
    public sealed class Int64ProgressSink
        : IProgressSink<long>, IDisposable
    {
        /// <summary>
        ///     The default total for <see cref="Int64ProgressSink"/>s.
        /// </summary>
        public const long DefaultTotal = 100;

        /// <summary>
        ///     The subject used to publish raw progress data.
        /// </summary>
        readonly Subject<RawProgressData<long>> _rawDataSubject = new Subject<RawProgressData<long>>();

        /// <summary>
        ///     The current progress value.
        /// </summary>
        long                                    _current;

        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        long                                    _total;

        /// <summary>
        ///     Create a new <see cref="Int64ProgressSink"/> with the default <see cref="Total"/> (<see cref="DefaultTotal"/>).
        /// </summary>
        public Int64ProgressSink()
            : this(initialTotal: DefaultTotal)
        {
        }

        /// <summary>
        ///     Create a new <see cref="Int64ProgressSink"/>.
        /// </summary>
        /// <param name="initialTotal">
        ///     The initial progress total.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="initialTotal"/> is less than 1.
        /// </exception>
        public Int64ProgressSink(long initialTotal)
            : this(initialTotal, initialValue: 0)
        {
        }

        /// <summary>
        ///     Create a new <see cref="Int64ProgressSink"/>.
        /// </summary>
        /// <param name="initialTotal">
        ///     The initial progress total.
        /// </param>
        /// <param name="initialValue">
        ///     The initial progress value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="initialTotal"/> is less than 1.
        /// </exception>
        public Int64ProgressSink(long initialTotal, long initialValue)
        {
            if (initialTotal < 1)
                throw new ArgumentOutOfRangeException(nameof(initialTotal), initialTotal, "Progress total cannot be less than 1.");

            _total = initialTotal;
            _current = initialValue;
        }

        /// <summary>
        ///     Dispose of resources being used by the sink.
        /// </summary>
        public void Dispose()
        {
            _rawDataSubject.Dispose();
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
                PublishRawData(current, total);
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
            PublishRawData(current, total);

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
            PublishRawData(current, total);

            return current;
        }

        /// <summary>
        ///     Set the current progress value.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        public void SetCurrent(long current)
        {
            long total = _total;
            _current = current;
            PublishRawData(current, total);
        }

        /// <summary>
        ///     Reset the current progress value to 0.
        /// </summary>
        public void Reset()
        {
            long total = _total;
            long previous = Interlocked.Exchange(ref _current, 0);
            PublishRawData(0, total);
        }

        /// <summary>
        ///     Subscribe an observer to raw progress data notifications.
        /// </summary>
        /// <param name="observer">
        ///     The observer to subscribe.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the subscription.
        /// </returns>
        public IDisposable Subscribe(IObserver<RawProgressData<long>> observer)
        {
            return _rawDataSubject.Subscribe(observer);
        }

        /// <summary>
        ///     Publish raw progress data to subscribers.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        void PublishRawData(long current, long total)
        {
            _rawDataSubject.OnNext(
                RawProgressData.Create(current, total)
            );
            
            // AF: Do we want to (optionally) call OnCompleted when current >= total?
        }
    }
}
