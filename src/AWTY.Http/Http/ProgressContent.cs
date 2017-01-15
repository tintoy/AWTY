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
        ///     The buffer size to use when transferring the inner content.
        /// </summary>
        int?                               _bufferSize;

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
        public ProgressContent(HttpContent innerContent)
            : this(innerContent, DefaultSink.Int64())
        {
        }

        /// <summary>
        ///     Create new progress content.
        /// </summary>
        /// <param name="innerContent">
        ///     The inner <see cref="HttpContent"/>.
        /// </param>
        /// <param name="bufferSize">
        ///     The buffer size to use when transferring the inner content.
        /// </param>
        public ProgressContent(HttpContent innerContent, int bufferSize)
            : this(innerContent, bufferSize, DefaultSink.Int64())
        {
        }

        /// <summary>
        ///     Create new progress content.
        /// </summary>
        /// <param name="innerContent">
        ///     The inner <see cref="HttpContent"/>.
        /// </param>
        /// <param name="sink">
        ///     The sink which will receive raw progress data.
        /// </param>
        public ProgressContent(HttpContent innerContent, IProgressSink<long> sink)
            : this(innerContent, null, sink)
        {
        }

        /// <summary>
        ///     Create new progress content.
        /// </summary>
        /// <param name="innerContent">
        ///     The inner <see cref="HttpContent"/>.
        /// </param>
        /// <param name="bufferSize">
        ///     An optional buffer size to use when transferring the inner content.
        /// 
        ///     Pass <c>null</c> to use the default buffer size.
        /// </param>
        /// <param name="sink">
        ///     The sink which will receive raw progress data.
        /// </param>
        public ProgressContent(HttpContent innerContent, int? bufferSize, IProgressSink<long> sink)
        {
            if (innerContent == null)
                throw new ArgumentNullException(nameof(innerContent));

            if (sink == null)
                throw new ArgumentNullException(nameof(sink));

            _innerContent = innerContent;
            _bufferSize = bufferSize;
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
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            ProgressStream progressStream = new ProgressStream(
                innerStream: stream,
                streamDirection: StreamDirection.Write,
                ownsStream: false,
                sink: _sink
            );
            using (progressStream)
            using (Stream innerStream = await _innerContent.ReadAsStreamAsync())
            {
                if (!innerStream.CanSeek)
                {
                    long total;
                    if (TryComputeLength(out total))
                        _sink.Total = total;
                }
                else
                    _sink.Total = innerStream.Length;

                if (_bufferSize.HasValue)
                    await innerStream.CopyToAsync(progressStream, _bufferSize.Value);
                else
                    await innerStream.CopyToAsync(progressStream);
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
            // DO NOT attempt to access this HttpContent's Headers property here (infinite recursion).
            // Instead, use the inner content's headers.

            length = _innerContent.Headers.ContentLength ?? -1;

            return _innerContent.Headers.ContentLength.HasValue;
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
