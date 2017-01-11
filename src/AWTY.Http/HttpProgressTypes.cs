namespace AWTY
{
    /// <summary>
    ///     The type of progress to report for an HTTP request / response.
    /// </summary>
    public enum HttpProgressTypes
    {
        /// <summary>
        ///     No progress reporting.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Report progress when sending requests.
        /// </summary>
        Request = 1,

        /// <summary>
        ///     Report progress when receiving response.
        /// </summary>
        Response = 2,

        /// <summary>
        ///     Report progress for requests and responses.
        /// </summary>
        Both = Request | Response
    }
}