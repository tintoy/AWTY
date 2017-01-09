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
        /// <param name="progressObserver">
        ///     The <see cref="IObserver{TValue}"/> that receives raw progress data.
        /// </param>
        /// <param name="total">
        ///     An optional total used to calculate progress.
        ///
        ///     If not specified, the stream length is used.
        /// </param>
        /// <param name="ownsStream">
        ///     Should the <see cref="ProgressStream"/> close the inner stream when it is closed?
        /// </param>
        /// <returns>
        ///     The new <see cref="ProgressStream"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The <paramref name="stream"/> is already a <see cref="ProgressStream"/>.
        /// </exception>
        public static ProgressStream WithReadProgress(this Stream stream, IObserver<RawProgressData<long>> progressObserver, long? total = null, bool ownsStream = true)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (progressObserver == null)
                throw new ArgumentNullException(nameof(progressObserver));

            Int64ProgressSink sink = new Int64ProgressSink();
            if (total.HasValue)
                sink.Total = total.Value;

            sink.Subscribe(progressObserver);

            return new ProgressStream(stream, ownsStream, StreamDirection.Read, sink);
        }

        /// <summary>
        ///     Wrap the specified stream in a <see cref="ProgressStream"/> that will report progress for writes.
        /// </summary>
        /// <param name="stream">
        ///     The stream to wrap.
        /// </param>
        /// <param name="progressObserver">
        ///     The <see cref="IObserver{TValue}"/> that receives raw progress data.
        /// </param>
        /// <param name="total">
        ///     The total used to calculate progress.
        /// </param>
        /// <param name="ownsStream">
        ///     Should the <see cref="ProgressStream"/> close the inner stream when it is closed?
        /// </param>
        /// <returns>
        ///     The new <see cref="ProgressStream"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The <paramref name="stream"/> is already a <see cref="ProgressStream"/>.
        /// </exception>
        public static ProgressStream WithWriteProgress(this Stream stream, IObserver<RawProgressData<long>> progressObserver, long total, bool ownsStream = true)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (progressObserver == null)
                throw new ArgumentNullException(nameof(progressObserver));

            Int64ProgressSink sink = new Int64ProgressSink(total);
            sink.Subscribe(progressObserver);

            return new ProgressStream(stream, ownsStream, StreamDirection.Write, sink);
        }
    }
}