using System;
using System.IO;

namespace AWTY.IO
{
    using Core.Sinks;

    /// <summary>
    ///     Progress-related extensions for streams.
    /// </summary>
    public static class StreamExtensions2
    {
        /// <summary>
        ///     Wrap the specified stream in a <see cref="ProgressStream2"/> that will report progress for reads.
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
        ///     The new <see cref="ProgressStream2"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The <paramref name="stream"/> is already a <see cref="ProgressStream2"/>.
        /// </exception>
        public static ProgressStream2 WithReadProgress(this Stream stream, IObserver<RawProgressData<long>> strategy, long? total = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));

            Int64ProgressSink2 sink = new Int64ProgressSink2();
            if (total.HasValue)
                sink.Total = total.Value;

            sink.Subscribe(strategy);

            return new ProgressStream2(stream, StreamDirection.Read, sink);
        }

        /// <summary>
        ///     Wrap the specified stream in a <see cref="ProgressStream2"/> that will report progress for writes.
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
        ///     The new <see cref="ProgressStream2"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The <paramref name="stream"/> is already a <see cref="ProgressStream2"/>.
        /// </exception>
        public static ProgressStream2 WithWriteProgress(this Stream stream, IObserver<RawProgressData<long>> strategy, long total)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));

            Int64ProgressSink2 sink = new Int64ProgressSink2(total);
            sink.Subscribe(strategy);

            return new ProgressStream2(stream, StreamDirection.Write, sink);
        }
    }
}