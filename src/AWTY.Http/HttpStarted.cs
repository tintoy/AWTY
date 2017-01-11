using System;
using System.Net.Http;

namespace AWTY.Http
{
    /// <summary>
    ///     The base class for XXXstarted notifications.
    /// </summary>
    public abstract class HttpStarted
    {
        /// <summary>
        ///     Create a new <see cref="HttpStarted"/>.
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
        protected HttpStarted(Uri requestUri, string requestMethod, string progressContextId)
        {
            RequestUri = requestUri;
            RequestMethod = requestMethod;
            ProgressContextId = progressContextId;
        }

        /// <summary>
        ///     The request URI.
        /// </summary>
        public Uri RequestUri { get; }

        /// <summary>
        ///     The request method (e.g. GET, POST, PUT, etc)..
        /// </summary>
        public string RequestMethod { get; }

        /// <summary>
        ///     The Id of the <see cref="ProgressContext"/> where the request was initiated.
        /// </summary>
        /// <remarks>
        ///     Useful when a single <see cref="HttpClient"/> is shared by multiple concurrent callers; you can use it to filter out requests from other tasks or threads.
        /// </remarks>
        public string ProgressContextId { get; }
    }
}