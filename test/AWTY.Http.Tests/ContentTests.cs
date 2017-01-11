using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AWTY.Http.Tests
{
    /// <summary>
    ///     Tests for <see cref="ProgressContent"/>.
    /// </summary>
    public class ContentTests
    {
        /// <summary>
        ///     Wrap <see cref="StringContent"/> in <see cref="ProgressContent"/>, and read the content as a string.
        /// </summary>
        [Fact]
        public async Task StringContent_ReadAsString()
        {
            const int contentLength = 100;
            int[] expectedPercentages = { 100 };

            List<int> actualPercentages = new List<int>();

            string expectedContent = new String('x', contentLength);
            using (StringContent stringContent = new StringContent(expectedContent))
            using (ProgressContent progressContent = new ProgressContent(stringContent))
            {
                progressContent.Progress.Percentage(10).Subscribe(progress =>
                {
                    actualPercentages.Add(progress.PercentComplete);
                });

                string actualContent = await progressContent.ReadAsStringAsync();
                Assert.Equal(expectedContent, actualContent);

                Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
                Assert.Equal(expectedPercentages, actualPercentages);
            }
        }
    }
}