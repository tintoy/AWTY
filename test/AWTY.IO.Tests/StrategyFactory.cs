namespace AWTY.IO.Tests
{
    /// <summary>
    ///     Factory for progress strategies used in tests.
    /// </summary>
    public class StrategyFactory
    {
        /// <summary>
        ///     Create a new <see cref="IProgressStrategy{TValue}"/> that only notifies changes in percentage completion greater than the specified value.
        /// </summary>
        /// <param name="chunkSize">
        ///     The minimum change in percentage completion to notify.
        /// </param>
        /// <returns>
        ///     The configured <see cref="IProgressStrategy{TValue}"/>.
        /// </returns>
        public IProgressStrategy<long> ChunkedPercentage(int chunkSize)
        {
            return ProgressStrategy.PercentComplete.Chunked.Int64(chunkSize);
        }
    }
}