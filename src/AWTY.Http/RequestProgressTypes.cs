namespace AWTY
{
    /// <summary>
    ///     The type of progress to report for an HTTP request.
    /// </summary>
    public enum RequestProgressTypes
    {
        /// <summary>
        ///     No progress reporting.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Report progress when sending request.
        /// </summary>
        Send = 1,

        /// <summary>
        ///     Report progress when receiving response.
        /// </summary>
        Receive = 2
    }
}