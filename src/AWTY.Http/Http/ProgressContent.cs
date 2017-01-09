using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AWTY.Http
{
    using Core.Sinks;
    using IO;

    /// <summary>
    ///     <see cref="HttpContent"/> that reports progress when reading / writing its content.
    /// </summary>
    public class ProgressContent
        : HttpContent
    {
        /// <summary>
        ///     The inner <see cref="HttpContent"/>.
        /// </summary>
        HttpContent                         _innerContent;

        /// <summary>
        ///     The expected stream direction.
        /// </summary>
        /// <remarks>
        ///     For an <see cref="HttpRequestMessage"/>, this is <see cref="StreamDirection.Write"/>.
        ///     For an <see cref="HttpResponseMessage"/>, this is <see cref="StreamDirection.Read"/>.
        /// </remarks>
        StreamDirection                     _direction;

        /// <summary>
        ///     The sink which will receive raw progress data.
        /// </summary>
        IProgressSink<long>                _sink;

        /// <summary>
        ///     Create new progress content.
        /// </summary>
        /// <param name="innerContent">
        ///     The inner <see cref="HttpContent"/>.
        /// </param>
        /// <param name="direction">
        ///     The expected stream direction.
        /// </param>
        public ProgressContent(HttpContent innerContent, StreamDirection direction)
            : this(innerContent, direction, new Int64ProgressSink())
        {
        }

        /// <summary>
        ///     Create new progress content.
        /// </summary>
        /// <param name="innerContent">
        ///     The inner <see cref="HttpContent"/>.
        /// </param>
        /// <param name="direction">
        ///     The expected stream direction.
        /// </param>
        /// <param name="sink">
        ///     The sink which will receive raw progress data.
        /// </param>
        public ProgressContent(HttpContent innerContent, StreamDirection direction, IProgressSink<long> sink)
        {
            if (innerContent == null)
                throw new ArgumentNullException(nameof(innerContent));

            if (sink == null)
                throw new ArgumentNullException(nameof(sink));

            _innerContent = innerContent;
            _direction = direction;
            _sink = sink;

            LoadHeaders();
        }

        /// <summary>
        ///     Dispose of resources being used by the <see cref="ProgressContent"/>.
        /// </summary>
        /// <param name="disposing">
        ///     Explicit disposal?
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _innerContent.Dispose();
        }

        /// <summary>
        ///     An <see cref="IObservable{T}"/> that can be used to observe raw progress data.
        /// </summary>
        public IObservable<RawProgressData<long>> Progress => _sink;

        /// <summary>
        ///     Serialise the HTTP content to a stream as an asynchronous operation.
        /// </summary>
        /// <param name="stream">
        ///     The target stream.
        /// </param>
        /// <param name="context">
        ///     The transport context.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            // Must know the content length to report progress.
            if (Headers.ContentLength.HasValue)
            {
                ProgressStream progressStream = new ProgressStream(stream, _direction,
                    ownsStream: false,
                    sink: _sink
                );
                using (progressStream)
                {
                    long total;
                    if (TryComputeLength(out total))
                        _sink.Total = total;

                    await _innerContent.CopyToAsync(progressStream);
                }
            }
            else
            {
                await _innerContent.CopyToAsync(stream);
            }
        }

        /// <summary>
        ///     Attempt to compute the content length.
        /// </summary>
        /// <param name="length">
        ///     Receives the content length, or -1 if the content length could not be determined.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the content length was successfully computed; otherwise, <c>false</c>.
        /// </returns>
        protected override bool TryComputeLength(out long length)
        {
            length = Headers.ContentLength ?? -1;

            return Headers.ContentLength.HasValue;
        }

        /// <summary>
        ///     Copy headers from the inner content.
        /// </summary>
        void LoadHeaders()
        {
            Headers.Clear();
            foreach (var header in _innerContent.Headers)
                Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
    }
}
