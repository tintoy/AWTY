using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace AWTY.IO.Tests
{
    /// <summary>
    ///     Tests for <see cref="ProgressStream"/>.
    /// </summary>
    public class ProgressStreamTests
    {
        /// <summary>
        ///     The pseudo-random number generator for data used in tests.
        /// </summary>
        readonly Random _random = new Random(19123 /* hur, dur, magic */);

        // TODO: Change these from Facts to Theories with inline data.

        [Theory]
        [MemberData(nameof(ReadTestData))]
        public void Read_ChunkedPercentage(long total, long increment, int chunkSize, int[] expectedPercentages) 
        {
            List<int> actualPercentages = new List<int>();

            using (MemoryStream input = FillMemoryStream(total))
            using (ProgressStream progress = input.WithReadProgress(ChunkedPercentageStrategy(chunkSize)))
            {
                progress.ProgressChanged += (sender, args) =>
                {
                    actualPercentages.Add(args.PercentComplete);
                };

                byte[] buffer = new byte[increment];
                int bytesRead = 0;
                do
                {
                    bytesRead = progress.Read(buffer, offset: 0, count: buffer.Length);
                } while(bytesRead > 0);
            }

            Assert.Equal(expectedPercentages.Length, actualPercentages.Count);
            Assert.Equal(expectedPercentages, actualPercentages);
        }

        /// <summary>
        ///     Data for the stream-read test theory.
        /// </summary>
        public static IEnumerable<object> ReadTestData
        {
            get
            {
                yield return ReadTestDataRow(
                    total: 100,
                    increment: 1,
                    chunkSize: 10,
                    expectedPercentages: new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }
                );

                yield return ReadTestDataRow(
                    total: 100,
                    increment: 5,
                    chunkSize: 10,
                    expectedPercentages: new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }
                );

                yield return ReadTestDataRow(
                    total: 100,
                    increment: 20,
                    chunkSize: 10,
                    expectedPercentages: new int[] { 20, 40, 60, 80, 100 }
                );

                yield return ReadTestDataRow(
                    total: 50,
                    increment: 10,
                    chunkSize: 5,
                    expectedPercentages: new int[] { 20, 40, 60, 80, 100 }
                );

                yield return ReadTestDataRow(
                    total: 70,
                    increment: 3,
                    chunkSize: 5,
                    expectedPercentages: new int[] { 8, 17, 25, 30, 38, 47, 55, 60, 68, 77, 85, 90, 98, 100 }
                );
            }
        }

        // TODO: Make these into fixtures.

        /// <summary>
        ///     Create a <see cref="MemoryStream"/> and fill it with random data.
        /// </summary>
        /// <param name="size">
        ///     The number of bytes that the stream will contain.
        /// </param>
        /// <returns>
        ///     The <see cref="MemoryStream"/>.
        /// </returns>
        MemoryStream FillMemoryStream(long size)
        {
            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(size), size, "Size cannot be less than 1.");

            byte[] data = new byte[size];
            _random.NextBytes(data);

            return new MemoryStream(data, writable: true);
        }

        /// <summary>
        ///     Create a chunked percentage progress notification strategy.
        /// </summary>
        /// <param name="chunkSize">
        ///     The minimum change in progress to report.
        /// </param>
        /// <returns>
        ///     The configured <see cref="IProgressStrategy{TValue}"/>.
        /// </returns>
        IProgressStrategy<long> ChunkedPercentageStrategy(int chunkSize)
        {
            return ProgressStrategy.PercentComplete.Chunked.Int64(chunkSize);
        }

        /// <summary>
        ///     Create a new data row for the stream-read test theory.
        /// </summary>
        /// <param name="total">
        ///     The total number of bytes in the stream.
        /// </param>
        /// <param name="increment">
        ///     The number of bytes to read from the stream in each iteration.
        /// </param>
        /// <param name="chunkSize">
        ///     The minimum expected change in percentage completion.
        /// </param>
        /// <param name="expectedPercentages">
        ///     An array of expected percentage values.
        /// </param>
        /// <returns>
        ///     An array containing the row's data.
        /// </returns>
        static object[] ReadTestDataRow(long total, long increment, int chunkSize, int[] expectedPercentages)
        {
            return new object[4] { total, increment, chunkSize, expectedPercentages };
        }
    }
}
