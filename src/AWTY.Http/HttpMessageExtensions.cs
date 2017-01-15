using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AWTY
{
    using Http;

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
        /// <param name="progressSink">
        ///     An optional sink that will receive raw progress data.
        /// </param>
        /// <param name="bufferSize">
        ///     An optional buffer size to use when transferring content.
        /// 
        ///     If not specified, the default buffer size is used.
        /// </param>
        /// <returns>
        ///     The request message.
        /// </returns>
        public static HttpRequestMessage AddProgress(this HttpRequestMessage request, IProgressSink<long> progressSink = null, int? bufferSize = null)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // No response content? No progress to report.
            if (request.Content == null)
                return request;

            if (request.Content is ProgressContent)
                throw new InvalidOperationException("The HTTP request message has already been configured to report progress.");

            request.Content = new ProgressContent(
                innerContent: request.Content,
                sink: progressSink ?? DefaultSink.Int64(),
                bufferSize: bufferSize
            );
            request.SetProgressContextId(ProgressContext.Current.Id);

            return request;
        }

        /// <summary>
        ///     Add progress reporting for the specified response message.
        /// </summary>
        /// <param name="response">
        ///     The HTTP response message.
        /// </param>
        /// <param name="progressSink">
        ///     An optional sink that will receive raw progress data.
        /// </param>
        /// <param name="bufferSize">
        ///     An optional buffer size to use when transferring content.
        /// 
        ///     If not specified, the default buffer size is used.
        /// </param>
        /// <returns>
        ///     The response message.
        /// </returns>
        public static HttpResponseMessage AddProgress(this HttpResponseMessage response, IProgressSink<long> progressSink = null, int? bufferSize = null)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            // No response content? No progress to report.
            // Might be worth creating a dummy sink that just emits 100% progress.
            if (response.Content == null)
                return response;

            if (response.Content is ProgressContent)
                throw new InvalidOperationException("The HTTP response message has already been configured to report progress.");

            response.Content = new ProgressContent(
                innerContent: response.Content,
                sink: progressSink ?? DefaultSink.Int64(),
                bufferSize: bufferSize
            );

            return response;
        }

        /// <summary>
        ///     Add progress reporting to the HTTP response message.
        /// </summary>
        /// <param name="response">
        ///     A <see cref="Task{T}"/> that yields the response message.
        /// </param>
        /// <param name="sink">
        ///     An optional sink that will receive raw progress data.
        /// </param>
        /// <param name="bufferSize">
        ///     An optional buffer size to use when transferring content.
        /// 
        ///     If not specified, the default buffer size is used.
        /// </param>
        /// <returns>
        ///     The response message.
        /// </returns>
        public static async Task<HttpResponseMessage> WithProgress(this Task<HttpResponseMessage> response, IProgressSink<long> sink, int? bufferSize = null)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await response;
                responseMessage.AddProgress(sink, bufferSize);
            }
            catch
            {
                using (responseMessage)
                {
                    throw;
                }
            }

            return responseMessage;
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