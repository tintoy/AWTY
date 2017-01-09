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
        public void Total_10_Add_0() 
        {
            IProgressSink2<int> progressSink = new Int32ProgressSink2(total: 10);
            progressSink.Add(0);

            Assert.Equal(10, progressSink.Total);
            Assert.Equal(0, progressSink.Current);
        }

        [Fact]
        public void Total_10_Add_5() 
        {
            IProgressSink2<int> progressSink = new Int32ProgressSink2(total: 10);
            progressSink.Add(5);

            Assert.Equal(10, progressSink.Total);
            Assert.Equal(5, progressSink.Current);
        }

        [Fact]
        public void Total_10_Add_10() 
        {
            IProgressSink2<int> progressSink = new Int32ProgressSink2(total: 10);
            progressSink.Add(10);

            Assert.Equal(10, progressSink.Total);
            Assert.Equal(10, progressSink.Current);
        }

        [Fact]
        public void Total_10_Add_11() 
        {
            IProgressSink2<int> progressSink = new Int32ProgressSink2(total: 10);
            progressSink.Add(11);

            Assert.Equal(10, progressSink.Total);
            Assert.Equal(11, progressSink.Current);
        }
    }
}
