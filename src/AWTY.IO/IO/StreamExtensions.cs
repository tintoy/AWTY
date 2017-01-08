using System;
using System.IO;

namespace AWTY.IO
{
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
        /// <param name="progressSink">
        ///     The <see cref="IProgressSink{TValue}"/> used to report stream progress.
        /// </param>
        /// <returns>
        ///     The new <see cref="ProgressStream"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The <paramref name="stream"/> is already a <see cref="ProgressStream"/>.
        /// </exception>
        public static ProgressStream WithReadProgress(this Stream stream, IProgressSink<long> progressSink)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (progressSink == null)
                throw new ArgumentNullException(nameof(progressSink));

            return new ProgressStream(stream, StreamDirection.Read, progressSink);
        }

        /// <summary>
        ///     Wrap the specified stream in a <see cref="ProgressStream"/> that will report progress for reads.
        /// </summary>
        /// <param name="stream">
        ///     The stream to wrap.
        /// </param>
        /// <param name="progressSink">
        ///     The <see cref="IProgressSink{TValue}"/> used to report stream progress.
        /// </param>
        /// <returns>
        ///     The new <see cref="ProgressStream"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The <paramref name="stream"/> is already a <see cref="ProgressStream"/>.
        /// </exception>
        public static ProgressStream WithWriteProgress(this Stream stream, IProgressSink<long> progressSink)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (progressSink == null)
                throw new ArgumentNullException(nameof(progressSink));

            return new ProgressStream(stream, StreamDirection.Write, progressSink);
        }
    }
}