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
        [Fact]
        public void Ctor_Total_100() 
        {
            IProgressSink<long> progressSink = new Int64ProgressSink(total: 100L);
            Assert.Equal(100L, progressSink.Total);
            Assert.Equal(0L, progressSink.Current);
        }

        [Fact]
        public void Ctor_Total_MaxValue() 
        {
            IProgressSink<long> progressSink = new Int64ProgressSink(total: Int64.MaxValue);
            Assert.Equal(Int64.MaxValue, progressSink.Total);
            Assert.Equal(0L, progressSink.Current);
        }

        [Fact]
        public void Ctor_Total_0() 
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int64ProgressSink(total: 0)
            );
        }

        [Fact]
        public void Ctor_Total_Negative1() 
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int64ProgressSink(total: -1)
            );
        }
    }
}
