using System;
using Xunit;

namespace AWTY.Core.Tests
{
    using Sinks;

    /// <summary>
    ///     Tests for <see cref="Int64ProgressSink"/>.
    /// </summary>
    public partial class Int64ProgressSinkTests
    {
        /// <summary>
        ///     Construct an <see cref="Int64ProgressSink"/> with a valid total.
        /// </summary>
        [Theory]
        [InlineData(1L)]
        [InlineData(100L)]
        [InlineData(Int64.MaxValue)]
        public void Ctor_Total(long initialTotal) 
        {
            IProgressSink<long> progressSink = new Int64ProgressSink(initialTotal);
            Assert.Equal(initialTotal, progressSink.Total);
            Assert.Equal(0, progressSink.Current);
        }

        /// <summary>
        ///     Construct an <see cref="Int64ProgressSink"/> with an invalid total (out-of-range).
        /// </summary>
        [Theory]
        [InlineData(0L)]
        [InlineData(-1L)]
        [InlineData(Int64.MinValue)]        
        public void Ctor_Total_ArgumentOutOfRange(long initialTotal)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int64ProgressSink(initialTotal: 0)
            );
        }
    }
}
