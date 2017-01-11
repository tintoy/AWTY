using System;
using Xunit;

namespace AWTY.Core.Tests
{
    using Sinks;

    /// <summary>
    ///     Tests for <see cref="Int32ProgressSink"/>.
    /// </summary>
    public partial class Int32ProgressSinkTests
    {
        /// <summary>
        ///     Construct an <see cref="Int32ProgressSink"/> with a valid total.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(Int32.MaxValue)]
        public void Ctor_Total(int initialTotal) 
        {
            IProgressSink<int> progressSink = new Int32ProgressSink(initialTotal);
            Assert.Equal(initialTotal, progressSink.Total);
            Assert.Equal(0, progressSink.Current);
        }

        /// <summary>
        ///     Construct an <see cref="Int32ProgressSink"/> with an invalid total (out-of-range).
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(Int32.MinValue)]        
        public void Ctor_Total_ArgumentOutOfRange(int initialTotal)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int32ProgressSink(initialTotal: 0)
            );
        }
    }
}
