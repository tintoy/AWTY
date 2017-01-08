using System;
using Xunit;

namespace AWTY.Core.Tests
{
    using Strategies;

    /// <summary>
    ///     Tests for the <see cref="Int64ChunkedPercentage"/> progress-reporting strategy.
    /// </summary>
    public partial class Int64ChunkedPercentageTests
    {
        [Fact]
        public void Ctor_ChunkSize_5()
        {
            Int64ChunkedPercentage strategy = new Int64ChunkedPercentage(chunkSize: 5);
            Assert.Equal(5, strategy.ChunkSize);
        }

        [Fact]
        public void Ctor_ChunkSize_0()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int64ChunkedPercentage(chunkSize: 0)
            );
        }
    }
}