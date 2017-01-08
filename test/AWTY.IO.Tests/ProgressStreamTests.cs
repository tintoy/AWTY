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

        [Fact]
        public void Read_100_1_ChunkedPercentage_10() 
        {
            const long total = 100;
            const long increment = 1;
            const int chunkSize = 10;
            int[] expectedPercentages = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

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

        [Fact]
        public void Read_100_5_ChunkedPercentage_10() 
        {
            const long total = 100;
            const long increment = 5;
            const int chunkSize = 10;
            int[] expectedPercentages = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

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

        [Fact]
        public void Read_100_20_ChunkedPercentage_10() 
        {
            const long total = 100;
            const long increment = 20;
            const int chunkSize = 10;
            int[] expectedPercentages = { 20, 40, 60, 80, 100 };

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

        [Fact]
        public void Read_50_10_ChunkedPercentage_5() 
        {
            const long total = 50;
            const long increment = 10;
            const int chunkSize = 5;
            int[] expectedPercentages = { 20, 40, 60, 80, 100 };

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

        [Fact]
        public void Read_70_3_ChunkedPercentage_5() 
        {
            const long total = 70;
            const long increment = 3;
            const int chunkSize = 5;
            int[] expectedPercentages = { 8, 17, 25, 30, 38, 47, 55, 60, 68, 77, 85, 90, 98, 100 };

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
    }
}
