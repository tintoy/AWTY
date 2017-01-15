using System;
using System.Reactive.Subjects;
using System.Threading;

namespace AWTY.Core.Sinks
{
    /// <summary>
    ///     A sink for reporting progress as a 32-bit integer.
    /// </summary>
    public sealed class Int32ProgressSink
        : IProgressSink<int>, IDisposable
    {
        /// <summary>
        ///     The default total for <see cref="Int32ProgressSink"/>s.
        /// </summary>
        public const int DefaultTotal = 100;

        /// <summary>
        ///     The subject used to publish raw progress data.
        /// </summary>
        readonly Subject<RawProgressData<int>>   _rawDataSubject = new Subject<RawProgressData<int>>();

        /// <summary>
        ///     The current progress value.
        /// </summary>
        int                                      _current;

        /// <summary>
        ///     The total value against which progress is measured.
        /// </summary>
        int                                      _total;

        /// <summary>
        ///     Create a new <see cref="Int32ProgressSink"/> with the default <see cref="Total"/> (<see cref="DefaultTotal"/>).
        /// </summary>
        public Int32ProgressSink()
            : this(initialTotal: DefaultTotal)
        {
        }

        /// <summary>
        ///     Create a new <see cref="Int32ProgressSink"/>.
        /// </summary>
        /// <param name="initialTotal">
        ///     The initial progress total.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="initialTotal"/> is less than 1.
        /// </exception>
        public Int32ProgressSink(int initialTotal)
            : this(initialTotal, initialValue: 0)
        {
        }
        
        /// <summary>
        ///     Create a new <see cref="Int32ProgressSink"/>.
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
        public Int32ProgressSink(int initialTotal, int initialValue)
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
                PublishRawData(current, total);
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
            PublishRawData(current, total);

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
            PublishRawData(current, total);

            return current;
        }

        /// <summary>
        ///     Set the current progress value.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        public void SetCurrent(int current)
        {
            int total = _total;
            _current = current;
            PublishRawData(current, total);
        }

        /// <summary>
        ///     Reset the current progress value to 0.
        /// </summary>
        public void Reset()
        {
            int total = _total;
            int previous = Interlocked.Exchange(ref _current, 0);
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
        public IDisposable Subscribe(IObserver<RawProgressData<int>> observer)
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
        void PublishRawData(int current, int total)
        {
            _rawDataSubject.OnNext(
                RawProgressData.Create(current, total)
            );
            
            // AF: Do we want to (optionally) call OnCompleted when current >= total?
        }
    }
}
