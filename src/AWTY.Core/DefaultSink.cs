namespace AWTY
{
    using Core.Sinks;

    /// <summary>
    ///     Default implementations of <see cref="IProgressSink{TValue}"/>.
    /// </summary>
    public static class DefaultSink
    {
        /// <summary>
        ///     Create a new 32-bit integer progress sink.
        /// </summary>
        /// <param name="initialTotal">
        ///     The initial total for the progress sink.
        /// </param>
        /// <returns>
        ///     The <see cref="IProgressSink{TValue}"/>.
        /// </returns>
        public static IProgressSink<int> Int32(int initialTotal = Int32ProgressSink.DefaultTotal)
        {
            return new Int32ProgressSink(initialTotal);
        }

        /// <summary>
        ///     Create a new 64-bit integer progress sink.
        /// </summary>
        /// <param name="initialTotal">
        ///     The initial total for the progress sink.
        /// </param>
        /// <returns>
        ///     The <see cref="IProgressSink{TValue}"/>.
        /// </returns>
        public static IProgressSink<long> Int64(long initialTotal = Int64ProgressSink.DefaultTotal)
        {
            return new Int64ProgressSink(initialTotal);
        }
    }
}