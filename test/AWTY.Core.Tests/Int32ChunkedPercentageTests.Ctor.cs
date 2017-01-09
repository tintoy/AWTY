using System;
using Xunit;

namespace AWTY.Core.Tests
{
    using Strategies;

    /// <summary>
    ///     Tests for the <see cref="Int32ChunkedPercentageStrategy2"/> progress-notification strategy.
    /// </summary>
    public partial class Int32ChunkedPercentageTests
    {
        [Fact]
        public void Ctor_ChunkSize_5()
        {
            Int32ChunkedPercentageStrategy2 strategy = new Int32ChunkedPercentageStrategy2(chunkSize: 5);
            Assert.Equal(5, strategy.ChunkSize);
        }

        [Fact]
        public void Ctor_ChunkSize_0()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int32ChunkedPercentageStrategy2(chunkSize: 0)
            );
        }
    }
}