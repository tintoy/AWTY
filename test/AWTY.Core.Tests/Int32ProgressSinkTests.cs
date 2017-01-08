using System;
using Xunit;

namespace AWTY.Core.Tests
{
    /// <summary>
    ///     Tests for <see cref="Int32ProgressSink"/>.
    /// </summary>
    public class Int32ProgressSinkTests
    {
        [Fact]
        public void Ctor_Total_100() 
        {
            IProgressSink<int> progressSink = new Int32ProgressSink(total: 100);
            Assert.Equal(100, progressSink.Total);
            Assert.Equal(0, progressSink.Current);
            Assert.Equal(0, progressSink.PercentComplete);
        }

        [Fact]
        public void Ctor_Total_MaxValue() 
        {
            IProgressSink<int> progressSink = new Int32ProgressSink(total: Int32.MaxValue);
            Assert.Equal(0, progressSink.Current);
            Assert.Equal(Int32.MaxValue, progressSink.Total);
            Assert.Equal(0, progressSink.PercentComplete);
        }

        [Fact]
        public void Ctor_Total_0() 
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int32ProgressSink(total: 0)
            );
        }

        [Fact]
        public void Ctor_Total_Negative1() 
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int32ProgressSink(total: -1)
            );
        }
    }
}
