using System;
using Xunit;

namespace AWTY.Core.Tests
{
    using Strategies;

    /// <summary>
    ///     Tests for the <see cref="Int64ChunkedPercentageStrategy"/> progress-notification strategy.
    /// </summary>
    public partial class Int64ChunkedPercentageTests
    {
        [Fact]
        public void Ctor_ChunkSize_5()
        {
            Int64ChunkedPercentageStrategy2 strategy = new Int64ChunkedPercentageStrategy2(chunkSize: 5);
            Assert.Equal(5, strategy.ChunkSize);
        }

        [Fact]
        public void Ctor_ChunkSize_0()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int64ChunkedPercentageStrategy2(chunkSize: 0)
            );
        }
    }
}