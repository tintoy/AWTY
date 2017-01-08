namespace AWTY.Core.Tests
{
    /// <summary>
    ///     Factory for progress strategies used in tests.
    /// </summary>
    public class StrategyFactory
    {
        /// <summary>
        ///     Create a new 32-bit <see cref="IProgressStrategy{TValue}"/> that only notifies changes in percentage completion greater than the specified value.
        /// </summary>
        /// <param name="chunkSize">
        ///     The minimum change in percentage completion to notify.
        /// </param>
        /// <returns>
        ///     The configured <see cref="IProgressStrategy{TValue}"/>.
        /// </returns>
        public IProgressStrategy<int> ChunkedPercentage32(int chunkSize)
        {
            return ProgressStrategy.PercentComplete.Chunked.Int32(chunkSize);
        }

        /// <summary>
        ///     Create a new 64-bit <see cref="IProgressStrategy{TValue}"/> that only notifies changes in percentage completion greater than the specified value.
        /// </summary>
        /// <param name="chunkSize">
        ///     The minimum change in percentage completion to notify.
        /// </param>
        /// <returns>
        ///     The configured <see cref="IProgressStrategy{TValue}"/>.
        /// </returns>
        public IProgressStrategy<long> ChunkedPercentage64(int chunkSize)
        {
            return ProgressStrategy.PercentComplete.Chunked.Int64(chunkSize);
        }

        /// <summary>
        ///     Create a new 32-bit <see cref="IProgressStrategy{TValue}"/> that never notifies.
        /// </summary>
        /// <returns>
        ///     The configured <see cref="IProgressStrategy{TValue}"/>.
        /// </returns>
        public IProgressStrategy<int> Never32()
        {
            return ProgressStrategy.Never.Int32();
        }

        /// <summary>
        ///     Create a new 64-bit <see cref="IProgressStrategy{TValue}"/> that never notifies.
        /// </summary>
        /// <returns>
        ///     The configured <see cref="IProgressStrategy{TValue}"/>.
        /// </returns>
        public IProgressStrategy<long> Never64()
        {
            return ProgressStrategy.Never.Int64();
        }
    }
}