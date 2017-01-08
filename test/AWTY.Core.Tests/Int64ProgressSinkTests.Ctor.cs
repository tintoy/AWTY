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
        public void Ctor_Strategy_Null() 
        {
            Assert.Throws<ArgumentNullException>(
                () => new Int64ProgressSink(total: 100, strategy: null)
            );
        }
        
        [Fact]
        public void Ctor_Total_100() 
        {
            IProgressSink<long> progressSink = new Int64ProgressSink(total: 100L, strategy: Strategies.Never64.Instance);
            Assert.Equal(100L, progressSink.Total);
            Assert.Equal(0L, progressSink.Current);
        }

        [Fact]
        public void Ctor_Total_MaxValue() 
        {
            IProgressSink<long> progressSink = new Int64ProgressSink(total: Int64.MaxValue, strategy: Strategies.Never64.Instance);
            Assert.Equal(Int64.MaxValue, progressSink.Total);
            Assert.Equal(0L, progressSink.Current);
        }

        [Fact]
        public void Ctor_Total_0() 
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int64ProgressSink(total: 0, strategy: Strategies.Never64.Instance)
            );
        }

        [Fact]
        public void Ctor_Total_Negative1() 
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Int64ProgressSink(total: -1, strategy: Strategies.Never64.Instance)
            );
        }
    }
}
