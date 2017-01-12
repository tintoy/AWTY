using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AWTY.Http.IntegrationTests
{
    /// <summary>
    ///     An unbuffered <see cref="HttpContent"/> implementation for request bodies; always streams its content.
    /// </summary>
    /// <remarks>
    ///     <see cref="HttpContent"/> really REALLY wants to buffer its content.
    ///     
    ///     For tests, this is painful because buffering always pulls in the entire stream content in a single read (which is useless from a progress-reporting perspective).
    ///     This is not an issue when reading responses (just use HttpCompletionOption.ResponseHeadersRead),
    ///     but for sending request bodies it makes things difficult.
    /// </remarks>
    public sealed class StreamingRequestContent
        : HttpContent
    {
        /// <summary>
        ///     The request content stream.
        /// </summary>
        readonly Stream _stream;

        /// <summary>
        ///     The request stream length.
        /// </summary>
        readonly long   _length;

        /// <summary>
        ///     The chunk size when streaming the content.
        /// </summary>
        readonly int    _chunkSize;

        /// <summary>
        ///     Create new streaming request content.
        /// </summary>
        /// <param name="stream">
        ///     The request content stream.
        /// </param>
        /// <param name="length">
        ///     The response content stream.
        /// </param>
        /// <param name="chunkSize">
        ///     The chunk size when streaming the content.
        /// </param>
        public StreamingRequestContent(Stream stream, long length, int chunkSize = 4096)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _stream = stream;
            _length = length;
            _chunkSize = chunkSize;
        }

        /// <summary>
        ///     Dispose of resources being used by the content.
        /// </summary>
        /// <param name="disposing">
        ///     Explicit disposal?
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _stream?.Dispose();
        }

        /// <summary>
        ///     Serialise the content to a stream, as an asynchronous operation.
        /// </summary>
        /// <param name="stream">
        ///     The stream to which data will be written.
        /// </param>
        /// <param name="context">
        ///     The request context.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            await _stream.CopyToAsync(stream, _chunkSize);
        }

        /// <summary>
        ///     Attempt to compute the content length.
        /// </summary>
        /// <param name="length">
        ///     Receives the content length, if available.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the content length was successfully computed; otherwise, <c>false</c>.
        /// </returns>
        protected override bool TryComputeLength(out long length)
        {
            length = _length;

            return true;
        }
    }
}