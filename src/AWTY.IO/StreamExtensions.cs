using System;
using System.IO;

namespace AWTY
{
    using Core.Sinks;
    using IO;

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
        /// <param name="total">
        ///     An optional total used to calculate progress.
        ///
        ///     If not specified, the stream length is used (providing the stream supports seeking).
        /// </param>
        /// <param name="progressObserver">
        ///     An optional <see cref="IObserver{TValue}"/> that receives raw progress data.
        /// </param>
        /// <param name="ownsStream">
        ///     Should the <see cref="ProgressStream"/> close the inner stream when it is closed?
        /// </param>
        /// <returns>
        ///     The new <see cref="ProgressStream"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The <paramref name="stream"/> is already a <see cref="ProgressStream"/>.
        ///     <paramref name="total"/> was not specified, and the stream does not support seeking.
        /// </exception>
        public static ProgressStream WithReadProgress(this Stream stream, long? total = null, IObserver<RawProgressData<long>> progressObserver = null, bool ownsStream = true)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            Int64ProgressSink sink = new Int64ProgressSink();
            if (total.HasValue)
                sink.Total = total.Value;
            else if (stream.CanSeek)
                sink.Total = stream.Length;
            else
                throw new InvalidOperationException("Total was not specified, and cannot be auto-detected because the specified stream does not support seeking.");

            if (progressObserver != null)
                sink.Subscribe(progressObserver);

            return new ProgressStream(stream, StreamDirection.Read, ownsStream, sink);
        }

        /// <summary>
        ///     Wrap the specified stream in a <see cref="ProgressStream"/> that will report progress for writes.
        /// </summary>
        /// <param name="stream">
        ///     The stream to wrap.
        /// </param>
        /// <param name="total">
        ///     The total used to calculate progress.
        /// </param>
        /// <param name="progressObserver">
        ///     An optional <see cref="IObserver{TValue}"/> that receives raw progress data.
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
        public static ProgressStream WithWriteProgress(this Stream stream, long total, IObserver<RawProgressData<long>> progressObserver = null, bool ownsStream = true)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            Int64ProgressSink sink = new Int64ProgressSink(total);
            if (progressObserver != null)
                sink.Subscribe(progressObserver);

            return new ProgressStream(stream, StreamDirection.Write, ownsStream, sink);
        }
    }
}