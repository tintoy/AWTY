namespace AWTY.IO
{
    /// <summary>
    ///     Indicates the direction in which a stream's data is expected to flow.
    /// </summary>
    public enum StreamDirection
    {
        /// <summary>
        ///     An unknown stream direction.
        /// </summary>
        /// <remarks>
        ///     Used to detect uninitialised values; do not use directly.
        /// </remarks>
        Unknown = 0,

        /// <summary>
        ///     Data is read from the stream.
        /// </summary>
        Read = 1,

        /// <summary>
        ///     The data is written to the stream.
        /// </summary>
        Write = 2
    }
}