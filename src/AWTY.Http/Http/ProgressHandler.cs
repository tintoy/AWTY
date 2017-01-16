using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace AWTY.Http
{
    /// <summary>
    ///     An HTTP message handler that enables progress reporting for requests and responses.
    /// </summary>
    public sealed class ProgressHandler
        : DelegatingHandler
    {
        /// <summary>
        ///     Dummy observable used when no progress notifications will be sent.
        /// </summary>
        static readonly IObservable<RawProgressData<long>> NoProgress = Observable.Never<RawProgressData<long>>();

        /// <summary>
        ///     The subject used to publish <see cref="HttpProgressStarted"/> notifications.
        /// </summary>
        Subject<HttpProgressStarted>    _notificationSubject = new Subject<HttpProgressStarted>();

        /// <summary>
        ///     The type(s) of progress that will be reported by the handler.
        /// </summary>
        HttpProgressTypes               _progressTypes;

        /// <summary>
        ///     The buffer size to use when transferring content.
        /// </summary>
        int?                            _bufferSize;

        /// <summary>
        ///     Create a new <see cref="ProgressHandler"/>.
        /// </summary>
        /// <param name="progressTypes">
        ///     The type(s) of progress that will be reported by the handler.
        /// </param>
        /// <param name="bufferSize">
        ///     An optional buffer size to use when transferring content.
        /// 
        ///     If not specified, the default buffer size is used.
        /// </param>
        public ProgressHandler(HttpProgressTypes progressTypes = HttpProgressTypes.Both, int? bufferSize = null)
        {
            _progressTypes = progressTypes;
            _bufferSize = bufferSize;
        }

        /// <summary>
        ///     Create a new <see cref="ProgressHandler"/>.
        /// </summary>
        /// <param name="nextHandler">
        ///     The next handler in the chain (if any).
        /// </param>
        /// <param name="progressTypes">
        ///     The type(s) of progress that will be reported by the handler.
        /// </param>
        /// <param name="bufferSize">
        ///     An optional buffer size to use when transferring content.
        /// 
        ///     If not specified, the default buffer size is used.
        /// </param>
        public ProgressHandler(HttpMessageHandler nextHandler, HttpProgressTypes progressTypes = HttpProgressTypes.Both, int? bufferSize = null)
            : base(nextHandler)
        {
            _progressTypes = progressTypes;
            _bufferSize = bufferSize;
        }

        /// <summary>
        ///     Dispose of resources being used by the <see cref="ProgressHandler"/>.
        /// </summary>
        /// <param name="disposing">
        ///     Explicit disposal?
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Subject<HttpProgressStarted> notificationSubject = Interlocked.Exchange(ref _notificationSubject, null);
                using (notificationSubject)
                {
                    notificationSubject?.OnCompleted();
                }
            }
        }

        /// <summary>
        ///     Check if the <see cref="ProgressHandler"/> has been disposed.
        /// </summary>
        void CheckDisposed()
        {
            if (_notificationSubject == null)
                throw new ObjectDisposedException(nameof(ProgressHandler));
        }

        /// <summary>
        ///     An observable that can be used to receive notifications for progress of newly-started requests and responses.
        /// </summary>
        /// <remarks>
        ///     Cast to <see cref="RequestStarted"/> / <see cref="ResponseStarted"/>, as appropriate.
        /// </remarks>
        public IObservable<HttpProgressStarted> Started => NotificationSubject;

        /// <summary>
        ///     An observable that can be used to receive notifications for progress of newly-started requests.
        /// </summary>
        public IObservable<RequestStarted> RequestStarted => Started.OfType<RequestStarted>();

        /// <summary>
        ///     An observable that can be used to receive notifications for progress of newly-started responses.
        /// </summary>
        public IObservable<ResponseStarted> ResponseStarted => Started.OfType<ResponseStarted>();

        /// <summary>
        ///     The subject used to publish <see cref="HttpProgressStarted"/> notifications.
        /// </summary>
        Subject<HttpProgressStarted> NotificationSubject
        {
            get
            {
                CheckDisposed();

                // Potential race but, meh, good enough.

                return _notificationSubject;
            }
        }

        /// <summary>
        ///     Asynchronously process an HTTP request message and its response.
        /// </summary>
        /// <param name="request">
        ///     The request message.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        ///     The response message.
        /// </returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            cancellationToken.ThrowIfCancellationRequested();

            HttpResponseMessage response = null;
            try
            {
                string progressContextId = request.GetProgressContextId();
                if (progressContextId == null)
                {
                    progressContextId = ProgressContext.Current.Id;
                    request.SetProgressContextId(progressContextId);
                }

                if (IsProgressEnabled(HttpProgressTypes.Request))
                {
                    request.AddProgress(bufferSize: _bufferSize);

                    NotificationSubject.OnNext(new RequestStarted(
                        request.RequestUri,
                        request.Method.Method,
                        progressContextId,
                        progress: request.GetProgress()
                    ));

                    cancellationToken.ThrowIfCancellationRequested();
                }

                response = await base.SendAsync(request, cancellationToken);

                if (IsProgressEnabled(HttpProgressTypes.Response))
                {
                    response.AddProgress(bufferSize: _bufferSize);

                    NotificationSubject.OnNext(new ResponseStarted(
                        request.RequestUri,
                        request.Method.Method,
                        progressContextId,
                        progress: response.GetProgress()
                    ));

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (Exception requestError)
            {
                using (response)
                {
                    _notificationSubject.OnError(requestError);

                    throw;
                }
            }

            return response;
        }

        /// <summary>
        ///     Determine whether the specified type of progress reporting is enabled.
        /// </summary>
        /// <param name="progressType">
        ///     The progress type to test.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the progress type is enabled; otherwise, <c>false</c>.
        /// </returns>
        bool IsProgressEnabled(HttpProgressTypes progressType)
        {
            return (_progressTypes & progressType) == progressType;
        }

        /// <summary>
        ///     Create a new <see cref="ProgressHandler"/> linked to an <see cref="HttpClientHandler"/>.
        /// </summary>
        /// <param name="progressTypes">
        ///     The types of progress to report.
        /// </param>
        /// <param name="bufferSize">
        ///     The buffer size to use when copying data.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ProgressHandler"/>.
        /// </returns>
        public static ProgressHandler Create(HttpProgressTypes progressTypes = HttpProgressTypes.Both, int? bufferSize = null)
        {
            return new ProgressHandler(new HttpClientHandler(), progressTypes, bufferSize);
        }
    }
}