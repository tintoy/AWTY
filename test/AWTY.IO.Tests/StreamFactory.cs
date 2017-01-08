using System;
using System.IO;

namespace AWTY.IO.Tests
{
    /// <summary>
    ///     Factory for streams used in tests.
    /// </summary>
    public class StreamFactory
    {
        /// <summary>
        ///     Create a <see cref="MemoryStream"/> and fill it with random data.
        /// </summary>
        /// <param name="size">
        ///     The number of bytes that the stream will contain.
        /// </param>
        /// <returns>
        ///     The <see cref="MemoryStream"/>.
        /// </returns>
        public MemoryStream FillMemoryStream(long size)
        {
            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(size), size, "Size cannot be less than 1.");

            byte[] data = new byte[size];
            TestData.CreateRandom().NextBytes(data);

            return new MemoryStream(data, writable: true);
        }
    }
}