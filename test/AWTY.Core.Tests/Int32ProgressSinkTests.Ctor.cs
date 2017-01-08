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
        public void Ctor_Strategy_Null() 
        {
            Assert.Throws<ArgumentNullException>(
                () => new Int32ProgressSink(total: 100, strategy: null)
            );
        }

        [Fact]
        public void Ctor_Total_100() 
        {
            IProgressSink<int> progressSink = new Int32ProgressSink(total: 100, strategy: ProgressStrategy.Never.Int32());
            Assert.Equal(100, progressSink.Total);
            Assert.Equal(0, progressSink.Current);
        }

        [Fact]
        public void Ctor_Total_MaxValue() 
        {
            IProgressSink<int> progressSink = new Int32ProgressSink(total: Int32.MaxValue, strategy: ProgressStrategy.Never.Int32());
            Assert.Equal(0, progressSink.Current);
            Assert.Equal(Int32.MaxValue, progressSink.Total);
        }

        [Fact]
        public void Ctor_Total_0() 
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int32ProgressSink(total: 0, strategy: ProgressStrategy.Never.Int32())
            );
        }

        [Fact]
        public void Ctor_Total_Negative1() 
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int32ProgressSink(total: -1, strategy: ProgressStrategy.Never.Int32())
            );
        }
    }
}
