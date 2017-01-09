using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AWTY.Http
{
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
        ///     The observer which will receive raw progress data.
        /// </summary>
        IObserver<RawProgressData<long>>    _progressObserver;

        /// <summary>
        ///     Create new progress content.
        /// </summary>
        /// <param name="innerContent">
        ///     The inner <see cref="HttpContent"/>.
        /// </param>
        /// <param name="direction">
        ///     The expected stream direction.
        /// </param>
        /// <param name="progressObserver">
        ///     The observer which will receive raw progress data.
        /// </param>
        public ProgressContent(HttpContent innerContent, StreamDirection direction, IObserver<RawProgressData<long>> progressObserver)
        {
            if (innerContent == null)
                throw new ArgumentNullException(nameof(innerContent));

            if (progressObserver == null)
                throw new ArgumentNullException(nameof(progressObserver));

            _innerContent = innerContent;
            _direction = direction;
            _progressObserver = progressObserver;

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
            if (Headers.ContentLength.HasValue)
            {
                ProgressStream progressStream;
                if (_direction == StreamDirection.Read)
                {
                    progressStream = stream.WithReadProgress(_progressObserver,
                        total:Headers.ContentLength.Value,
                        ownsStream: false
                    );
                }
                else
                {
                    progressStream = stream.WithWriteProgress(_progressObserver,
                        total:Headers.ContentLength.Value,
                        ownsStream: false
                    );
                }

                using (progressStream)
                {
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
