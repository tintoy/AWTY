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
        ///     The name of the message property used to store the current progress context Id.
        /// </summary>
        const string ProgressContextIdProperty = "AWTY.ProgressContext";

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
            request.SetProgressContextId(ProgressContext.Current.Id);

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

        /// <summary>
        ///     Get the Id of the <see cref="ProgressContext"/> (if any) that the request is associated with.
        /// </summary>
        /// <param name="request">
        ///     The HTTP request message.
        /// </param>
        /// <returns>
        ///     The progress context Id, or <c>null</c> if the request is not associated with a progress context
        /// </returns>
        public static string GetProgressContextId(this HttpRequestMessage request)
        {
            object progressContextId;
            request.Properties.TryGetValue(ProgressContextIdProperty, out progressContextId);

            return (string)progressContextId;
        }

        /// <summary>
        ///     Set the Id of the <see cref="ProgressContext"/> (if any) that the request is associated with.
        /// </summary>
        /// <param name="request">
        ///     The HTTP request message.
        /// </param>
        /// <param name="progressContextId">
        ///     The progress context Id.
        /// </param>
        public static void SetProgressContextId(this HttpRequestMessage request, string progressContextId)
        {
            request.Properties[ProgressContextIdProperty] = progressContextId;
        }

        /// <summary>
        ///     Get the Id of the <see cref="ProgressContext"/> (if any) that the response is associated with.
        /// </summary>
        /// <param name="response">
        ///     The HTTP response message.
        /// </param>
        /// <returns>
        ///     The progress context Id, or <c>null</c> if the response is not associated with a progress context
        /// </returns>
        public static string GetProgressContextId(this HttpResponseMessage response)
        {
            // Context comes from the original request message.
            HttpRequestMessage request = response.RequestMessage;
            if (request == null)
                return null;

            return request.GetProgressContextId();
        }
    }
}