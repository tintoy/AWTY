namespace AWTY.Core.Tests
{
    using Strategies;

    /// <summary>
    ///     Factory for progress strategies used in tests.
    /// </summary>
    public class StrategyFactory
    {
        /// <summary>
        ///     Create a new 32-bit <see cref="ProgressStrategy2{TValue}"/> that only notifies changes in percentage completion greater than the specified value.
        /// </summary>
        /// <param name="chunkSize">
        ///     The minimum change in percentage completion to notify.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ProgressStrategy2{TValue}"/>.
        /// </returns>
        public ProgressStrategy2<int> ChunkedPercentage32(int chunkSize)
        {
            return ProgressStrategy2.PercentComplete.Chunked.Int32(chunkSize);
        }

        /// <summary>
        ///     Create a new 64-bit <see cref="ProgressStrategy2{TValue}"/> that only notifies changes in percentage completion greater than the specified value.
        /// </summary>
        /// <param name="chunkSize">
        ///     The minimum change in percentage completion to notify.
        /// </param>
        /// <returns>
        ///     The configured <see cref="ProgressStrategy2{TValue}"/>.
        /// </returns>
        public ProgressStrategy2<long> ChunkedPercentage64(int chunkSize)
        {
            return ProgressStrategy2.PercentComplete.Chunked.Int64(chunkSize);
        }

        /// <summary>
        ///     Create a new 32-bit <see cref="ProgressStrategy2{TValue}"/> that never notifies.
        /// </summary>
        /// <returns>
        ///     The configured <see cref="ProgressStrategy2{TValue}"/>.
        /// </returns>
        public ProgressStrategy2<int> Never32()
        {
            return ProgressStrategy2.Never.Int32();
        }

        /// <summary>
        ///     Create a new 64-bit <see cref="ProgressStrategy2{TValue}"/> that never notifies.
        /// </summary>
        /// <returns>
        ///     The configured <see cref="ProgressStrategy2{TValue}"/>.
        /// </returns>
        public ProgressStrategy2<long> Never64()
        {
            return ProgressStrategy2.Never.Int64();
        }
    }
}