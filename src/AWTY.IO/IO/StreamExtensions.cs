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
        ///     The <see cref="IProgressStrategy{TValue}"/> used to determine when stream progress should be reported.
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
        public static ProgressStream WithReadProgress(this Stream stream, IProgressStrategy<long> strategy, long? total = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));

            Int64ProgressSink sink;
            if (total.HasValue)
                sink = new Int64ProgressSink(strategy, total.Value);
            else
                sink = new Int64ProgressSink(strategy);

            return new ProgressStream(stream, StreamDirection.Read, new Int64ProgressSink(strategy));
        }

        /// <summary>
        ///     Wrap the specified stream in a <see cref="ProgressStream"/> that will report progress for writes.
        /// </summary>
        /// <param name="stream">
        ///     The stream to wrap.
        /// </param>
        /// <param name="strategy">
        ///     The <see cref="IProgressStrategy{TValue}"/> used to determine when stream progress should be reported.
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
        public static ProgressStream WithWriteProgress(this Stream stream, IProgressStrategy<long> strategy, long total)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));

            return new ProgressStream(stream, StreamDirection.Write, new Int64ProgressSink(strategy, total));
        }
    }
}