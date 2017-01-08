using System;
using Xunit;

namespace AWTY.Core.Tests
{
    using Strategies;

    /// <summary>
    ///     Tests for the <see cref="Int32ChunkedPercentage"/> progress-reporting strategy.
    /// </summary>
    public partial class Int32ChunkedPercentageTests
    {
        [Fact]
        public void Ctor_ChunkSize_5()
        {
            Int32ChunkedPercentage strategy = new Int32ChunkedPercentage(chunkSize: 5);
            Assert.Equal(5, strategy.ChunkSize);
        }

        [Fact]
        public void Ctor_ChunkSize_0()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int32ChunkedPercentage(chunkSize: 0)
            );
        }
    }
}