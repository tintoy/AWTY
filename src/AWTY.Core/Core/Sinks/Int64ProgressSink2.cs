using System;
using System.Reactive.Subjects;
using System.Threading;

namespace AWTY.Core.Sinks
{
    /// <summary>
    ///     A sink for reporting progress as a 64-bit integer.
    /// </summary>
    public sealed class Int64ProgressSink2
        : IProgressSink2<long>, IDisposable
    {
        /// <summary>
        ///     The subject used to publish raw progress data.
        /// </summary>
        readonly Subject<RawProgressData<long>>   _rawDataSubject = new Subject<RawProgressData<long>>();

        /// <summary>
        ///     The current progress value.
        /// </summary>
        long                                      _current;

        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        long                                      _total;

        /// <summary>
        ///     Create a new <see cref="Int64ProgressSink2"/> with a <see cref="Total"/> of 100.
        /// </summary>
        public Int64ProgressSink2()
            : this(total: 100)
        {
        }

        /// <summary>
        ///     Create a new <see cref="Int64ProgressSink2"/>.
        /// </summary>
        /// <param name="total">
        ///     The initial progress total.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="total"/> is less than 1.
        /// </exception>
        public Int64ProgressSink2(long total)
        {
            if (total < 1)
                throw new ArgumentOutOfRangeException(nameof(total), total, "Progress total cannot be less than 1.");

            _total = total;
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
            if (current == total) // AF: Do we want to make this behaviour optional?
                _rawDataSubject.OnCompleted();
        }
    }
}
