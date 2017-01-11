using System.Collections.Generic;
using Xunit;

namespace AWTY.Core.Tests
{
    using Sinks;

    /// <summary>
    ///     Tests for <see cref="Int32ProgressSink"/>.
    /// </summary>
    public partial class Int32ProgressSinkTests
    {
        [Theory]
        [MemberData(nameof(AddTheoryData))]
        public void Add(int total, int initialValue, int amountToAdd, int expectedCurrent)
        {
            IProgressSink<int> progressSink = new Int32ProgressSink(total, initialValue);
            progressSink.Add(amountToAdd);

            Assert.Equal(total, progressSink.Total);
            Assert.Equal(expectedCurrent, progressSink.Current);
        }

        /// <summary>
        ///     Data for the <see cref="Int32ProgressSink"/> <see cref="Add"/> theory.
        /// </summary>
        public static IEnumerable<object[]> AddTheoryData => TestData.Theory.Int32SinkAdd;
    }
}
