using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace AWTY.Http
{
    using Core.Sinks;

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
        readonly Subject<HttpProgressStarted>   _notificationSubject = new Subject<HttpProgressStarted>();

        /// <summary>
        ///     The type(s) of progress that will be reported by the handler.
        /// </summary>
        RequestProgressTypes            _progressTypes;

        /// <summary>
        ///     Create a new <see cref="ProgressHandler"/>.
        /// </summary>
        /// <param name="progressTypes">
        ///     The type(s) of progress that will be reported by the handler.
        /// </param>
        public ProgressHandler(RequestProgressTypes progressTypes)
        {
            _progressTypes = progressTypes;
        }

        /// <summary>
        ///     Create a new <see cref="ProgressHandler"/>.
        /// </summary>
        /// <param name="progressTypes">
        ///     The type(s) of progress that will be reported by the handler.
        /// </param>
        /// <param name="nextHandler">
        ///     The next handler in the chain (if any).
        /// </param>
        public ProgressHandler(RequestProgressTypes progressTypes, DelegatingHandler nextHandler)
            : base(nextHandler)
        {
            _progressTypes = progressTypes;
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
                _notificationSubject.Dispose();
        }

        /// <summary>
        ///     An observable that can be used to receive notifications for progress of newly-started requests and responses.
        /// </summary>
        /// <remarks>
        ///     Cast to <see cref="RequestStarted"/> / <see cref="ResponseStarted"/>, as appropriate.
        /// </remarks>
        public IObservable<HttpProgressStarted> Started => _notificationSubject;

        /// <summary>
        ///     An observable that can be used to receive notifications for progress of newly-started requests.
        /// </summary>
        public IObservable<RequestStarted> RequestStarted => Started.OfType<RequestStarted>();

        /// <summary>
        ///     An observable that can be used to receive notifications for progress of newly-started responses.
        /// </summary>
        public IObservable<ResponseStarted> ResponseStarted => Started.OfType<ResponseStarted>();

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
            HttpResponseMessage response = null;
            try
            {
                string progressContextId = request.GetProgressContextId();
                if (progressContextId == null)
                {
                    progressContextId = ProgressContext.Current.Id;
                    request.SetProgressContextId(progressContextId);
                }

                Int64ProgressSink sink = null;

                if ((_progressTypes & RequestProgressTypes.Send) != 0)
                {
                    sink = new Int64ProgressSink();
                    request.AddProgress(sink);

                    _notificationSubject.OnNext(new RequestStarted(
                        request.RequestUri,
                        request.Method.Method,
                        progressContextId,
                        progress: sink
                    ));
                }

                response = await base.SendAsync(request, cancellationToken);

                if ((_progressTypes & RequestProgressTypes.Receive) != 0)
                {
                    sink = new Int64ProgressSink();
                    response.AddProgress(sink);

                    _notificationSubject.OnNext(new ResponseStarted(
                        request.RequestUri,
                        request.Method.Method,
                        progressContextId,
                        progress: sink
                    ));
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
    }
}