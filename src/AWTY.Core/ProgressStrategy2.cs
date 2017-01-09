namespace AWTY
{
    using Core.Strategies;
    
    /// <summary>
    ///     Well-known progress notification strategies.
    /// </summary>
    public static partial class ProgressStrategy2
    {
        /// <summary>
        ///     Well-known progress notification strategies that determine when to notify based on percentage completion.
        /// </summary>
        public static class PercentComplete
        {
            /// <summary>
            ///     Well-known progress notification strategies that determine when to notify based on minimum change in percentage completion.
            /// </summary>
            public static class Chunked
            {
                /// <summary>
                ///     Create a new chunked percentage progress notification strategy.
                /// </summary>
                /// <param name="chunkSize">
                ///     The minimum change in percentage completion to report.
                /// </param>
                /// <returns>
                ///     The new <see cref="IProgressStrategy{TValue}"/>.
                /// </returns>
                public static ProgressStrategy2<int> Int32(int chunkSize) => new Int32ChunkedPercentageStrategy2(chunkSize);

                /// <summary>
                ///     Create a new chunked percentage progress notification strategy.
                /// </summary>
                /// <param name="chunkSize">
                ///     The minimum change in percentage completion to report.
                /// </param>
                /// <returns>
                ///     The new <see cref="IProgressStrategy{TValue}"/>.
                /// </returns>
                public static ProgressStrategy2<long> Int64(int chunkSize) => new Int64ChunkedPercentageStrategy2(chunkSize);
            }
        }
    }
}