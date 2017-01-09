using System;
using System.IO;

namespace AWTY.IO
{
    /// <summary>
    ///     A <see cref="Stream"/> that wraps an inner <see cref="Stream"/>, adding progress reporting.
    /// </summary>
    public class ProgressStream
        : Stream
    {
        /// <summary>
        ///     The inner stream.
        /// </summary>
        readonly Stream                 _innerStream;

        /// <summary>
        ///     Should the <see cref="ProgressStream"/> close the inner stream when it is closed? 
        /// </summary>
        readonly bool                   _ownsStream;

        /// <summary>
        ///     The direction in which the stream's data is expected to flow.
        /// </summary>
        readonly StreamDirection        _streamDirection;

        /// <summary>
        ///     The sink used to report stream progress.
        /// </summary>
        readonly IProgressSink<long>    _sink;

        /// <summary>
        ///     Create a new <see cref="ProgressStream"/> that wraps the specified inner <see cref="Stream"/>.
        /// </summary>
        /// <param name="innerStream">
        ///     The inner stream.
        /// </param>
        /// <param name="ownsStream">
        ///     Should the <see cref="ProgressStream"/> close the inner stream when it is closed?
        /// </param>
        /// <param name="streamDirection">
        ///     The direction in which the stream's data is expected to flow.
        /// </param>
        /// <param name="sink">
        ///     The sink used to report stream progress.
        /// </param>
        public ProgressStream(Stream innerStream, bool ownsStream, StreamDirection streamDirection, IProgressSink<long> sink)
        {
            if (innerStream == null)
                throw new ArgumentNullException(nameof(innerStream));

            if (innerStream is ProgressStream)
                throw new InvalidOperationException("Wrapping a ProgressStream2 in another ProgressStream2 is not currently supported.");

            if (streamDirection == StreamDirection.Unknown)
                throw new ArgumentOutOfRangeException(nameof(streamDirection), streamDirection, $"Invalid stream direction: '{streamDirection}'.");

            _innerStream = innerStream;
            _streamDirection = streamDirection;
            _sink = sink;

            // Override total, if appropriate.
            if (_streamDirection == StreamDirection.Read && _innerStream.CanSeek)
                _sink.Total = _innerStream.Length;
        }

        /// <summary>
        ///     Dispose if resources being used by the <see cref="ProgressStream"/>.
        /// </summary>
        /// <param name="disposing">
        ///     Explicit disposal?
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_ownsStream)
                    _innerStream.Dispose();
            }
        }

        /// <summary>
        ///     Does the stream support reading?
        /// </summary>
        public override bool CanRead => _innerStream.CanRead;

        /// <summary>
        ///     Does the stream support writing?
        /// </summary>
        public override bool CanWrite => _innerStream.CanWrite;

        /// <summary>
        ///     Does the stream support seeking?
        /// </summary>
        public override bool CanSeek => _innerStream.CanSeek;

        /// <summary>
        ///     The stream length.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The stream does not support seeking.
        /// </exception>
        public override long Length => _innerStream.Length;

        /// <summary>
        ///     The stream position.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The stream does not support seeking.
        /// </exception>
        public override long Position
        {
            get
            {
                return _innerStream.Position;
            }
            set
            {
                _innerStream.Position = value;
            }
        }

        /// <summary>
        ///     The direction in which the stream's data is expected to flow.
        /// </summary>
        public StreamDirection StreamDirection => _streamDirection;

        

        /// <summary>
        ///     Flush the stream.
        /// </summary>
        public override void Flush()
        {
            _innerStream.Flush();
        }

        /// <summary>
        ///     Seek to the specified position in the stream.
        /// </summary>
        /// <param name="offset">
        ///     The 0-based offset within the stream to seek to.
        /// </param>
        /// <param name="origin">
        ///     The position in the stream to which the <paramref name="offset"/> is relative.
        /// </param>
        /// <returns>
        ///     The new (0-based) stream position.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The stream does not support seeking.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            // TODO: Decide how we handle seeking in relation to progress (probably disallow it).
            // TODO: We might still be able to support seeking for streams with StreamDirection.Read.

            return _innerStream.Seek(offset, origin);
        }

        /// <summary>
        ///     Set the stream length.
        /// </summary>
        /// <param name="value">
        ///     The new stream length.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     The stream does not support both writing and seeking.
        /// </exception>
        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
            _sink.Total = Math.Max(1, value);
        }

        /// <summary>
        ///     Read data from the stream.
        /// </summary>
        /// <param name="buffer">
        ///     A buffer to be filled with data.
        /// </param>
        /// <param name="offset">
        ///     The 0-based offset in the buffer at which to start storing data.
        /// </param>
        /// <param name="count">
        ///     The number of bytes to read.
        /// </param>
        /// <returns>
        ///     The number of bytes that were actually read from the stream.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The stream does not support both reading.
        /// </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _innerStream.Read(buffer, offset, count);

            if (_streamDirection == StreamDirection.Read)
                _sink.Add(bytesRead);

            return bytesRead;
        }

        /// <summary>
        ///     Write data to the stream.
        /// </summary>
        /// <param name="buffer">
        ///     A buffer containing the data to write.
        /// </param>
        /// <param name="offset">
        ///     The 0-based offset within the buffer of the data to write.
        /// </param>
        /// <param name="count">
        ///     The number of bytes to write.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     The stream does not support both writing.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);

            if (_streamDirection == StreamDirection.Write)
                _sink.Add(count);
        }
    }
}