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
        [Fact]
        public void Ctor_Total_100() 
        {
            IProgressSink2<int> progressSink = new Int32ProgressSink2(total: 100);
            Assert.Equal(100, progressSink.Total);
            Assert.Equal(0, progressSink.Current);
        }

        [Fact]
        public void Ctor_Total_MaxValue() 
        {
            IProgressSink2<int> progressSink = new Int32ProgressSink2(total: Int32.MaxValue);
            Assert.Equal(0, progressSink.Current);
            Assert.Equal(Int32.MaxValue, progressSink.Total);
        }

        [Fact]
        public void Ctor_Total_0() 
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int32ProgressSink2(total: 0)
            );
        }

        [Fact]
        public void Ctor_Total_Negative1() 
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int32ProgressSink2(total: -1)
            );
        }
    }
}
