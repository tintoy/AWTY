using System;
using System.Net.Http;

namespace AWTY
{
    using Http;
    using IO;

    // TODO:
    //      DelegatingHandler that uses these extension methods. This can then be installed in the pipeline of an HttpClient.
    //      But first we'll need to work out how consumers will differentiate between progress notifications from different requests.
    //      Perhaps we could have an observable sequence of RequestProgress (which contains the observable for that request's progress)?

    /// <summary>
    ///     Progress-related extension methods for <see cref="HttpRequestMessage"/> / <see cref="HttpResponseMessage"/>.
    /// </summary>
    public static class HttpMessageExtensions
    {
        /// <summary>
        ///     Add progress reporting for the specified request message.
        /// </summary>
        /// <param name="request">
        ///     The HTTP request message.
        /// </param>
        /// <param name="progressObserver">
        ///     The observer that will receive raw progress data.
        /// </param>
        /// <returns>
        ///     The request message.
        /// </returns>
        public static HttpRequestMessage AddProgress(this HttpRequestMessage request, IObserver<RawProgressData<long>> progressObserver)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Content == null)
                return request;

            if (request.Content is ProgressContent)
                throw new InvalidOperationException("The HTTP request message has already been configured to report progress.");

            request.Content = new ProgressContent(request.Content, StreamDirection.Write, progressObserver);

            return request;
        }

        /// <summary>
        ///     Add progress reporting for the specified response message.
        /// </summary>
        /// <param name="response">
        ///     The HTTP response message.
        /// </param>
        /// <param name="progressObserver">
        ///     The observer that will receive raw progress data.
        /// </param>
        /// <returns>
        ///     The response message.
        /// </returns>
        public static HttpResponseMessage AddProgress(this HttpResponseMessage response, IObserver<RawProgressData<long>> progressObserver)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.Content == null)
                return response;

            if (response.Content is ProgressContent)
                throw new InvalidOperationException("The HTTP response message has already been configured to report progress.");

            response.Content = new ProgressContent(response.Content, StreamDirection.Read, progressObserver);

            return response;
        }
    }
}