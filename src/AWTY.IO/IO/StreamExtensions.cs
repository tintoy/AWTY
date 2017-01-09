using System;
using System.IO;

namespace AWTY.IO
{
    using Core.Sinks;

    /// <summary>
    ///     Progress-related extensions for streams.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        ///     Wrap the specified stream in a <see cref="ProgressStream"/> that will report progress for reads.
        /// </summary>
        /// <param name="stream">
        ///     The stream to wrap.
        /// </param>
        /// <param name="strategy">
        ///     The <see cref="IObserver{TValue}"/> used to determine when stream progress should be reported.
        /// </param>
        /// <param name="total">
        ///     An optional total used to calculate progress.
        ///
        ///     If not specified, the stream length is used.
        /// </param>
        /// <returns>
        ///     The new <see cref="ProgressStream"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The <paramref name="stream"/> is already a <see cref="ProgressStream"/>.
        /// </exception>
        public static ProgressStream WithReadProgress(this Stream stream, IObserver<RawProgressData<long>> strategy, long? total = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));

            Int64ProgressSink sink = new Int64ProgressSink();
            if (total.HasValue)
                sink.Total = total.Value;

            sink.Subscribe(strategy);

            return new ProgressStream(stream, StreamDirection.Read, sink);
        }

        /// <summary>
        ///     Wrap the specified stream in a <see cref="ProgressStream"/> that will report progress for writes.
        /// </summary>
        /// <param name="stream">
        ///     The stream to wrap.
        /// </param>
        /// <param name="strategy">
        ///     The <see cref="IObserver{TValue}"/> used to determine when stream progress should be reported.
        /// </param>
        /// <param name="total">
        ///     The total used to calculate progress.
        /// </param>
        /// <returns>
        ///     The new <see cref="ProgressStream"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The <paramref name="stream"/> is already a <see cref="ProgressStream"/>.
        /// </exception>
        public static ProgressStream WithWriteProgress(this Stream stream, IObserver<RawProgressData<long>> strategy, long total)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));

            Int64ProgressSink sink = new Int64ProgressSink(total);
            sink.Subscribe(strategy);

            return new ProgressStream(stream, StreamDirection.Write, sink);
        }
    }
}