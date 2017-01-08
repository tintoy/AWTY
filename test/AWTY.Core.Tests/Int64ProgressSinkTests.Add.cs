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
        public void Total_10_Add_0() 
        {
            IProgressSink<long> progressSink = new Int64ProgressSink(total: 10L, strategy: Strategies.Never64.Instance);
            progressSink.Add(0L);

            Assert.Equal(10L, progressSink.Total);
            Assert.Equal(0L, progressSink.Current);
        }

        [Fact]
        public void Total_10_Add_5() 
        {
            IProgressSink<long> progressSink = new Int64ProgressSink(total: 10L, strategy: Strategies.Never64.Instance);
            progressSink.Add(5L);

            Assert.Equal(10L, progressSink.Total);
            Assert.Equal(5L, progressSink.Current);
        }

        [Fact]
        public void Total_10_Add_10() 
        {
            IProgressSink<long> progressSink = new Int64ProgressSink(total: 10L, strategy: Strategies.Never64.Instance);
            progressSink.Add(10L);

            Assert.Equal(10L, progressSink.Total);
            Assert.Equal(10L, progressSink.Current);
        }

        [Fact]
        public void Total_10_Add_11() 
        {
            IProgressSink<long> progressSink = new Int64ProgressSink(total: 10L, strategy: Strategies.Never64.Instance);
            progressSink.Add(11L);

            Assert.Equal(10L, progressSink.Total);
            Assert.Equal(11L, progressSink.Current);
        }
    }
}
