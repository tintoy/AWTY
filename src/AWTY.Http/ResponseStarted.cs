using System;
using System.Net.Http;

namespace AWTY.Http
{
    /// <summary>
    ///     A notification indicating the availability of progress reporting for a newly-started response.
    /// </summary>
    /// <remarks>
    ///     When progress reporting is enabled for an <see cref="HttpClient"/>, then a new <see cref="ResponseStarted"/> will be sent for each request.
    /// </remarks>
    public class ResponseStarted
        : HttpStarted
    {
        /// <summary>
        ///     Create a new <see cref="ResponseStarted"/>.
        /// </summary>
        /// <param name="requestUri">
        ///     The request URI.
        /// </param>
        /// <param name="requestMethod">
        ///     The request method (e.g. GET, POST, PUT, etc)..
        /// </param>
        /// <param name="progressContextId">
        ///     The Id of the <see cref="ProgressContext"/> where the request was initiated.
        /// </param>
        /// <param name="progress">
        ///     An observable that can be used to monitor raw progress data for the response content.
        /// </param>
        public ResponseStarted(Uri requestUri, string requestMethod, string progressContextId, IObservable<RawProgressData<long>> progress)
            : base(requestUri, requestMethod, progressContextId)
        {
            Progress = progress;
        }

        /// <summary>
        ///     An observable that can be used to monitor raw progress data for the response content.
        /// </summary>
        public IObservable<RawProgressData<long>> Progress { get; }
    }
}