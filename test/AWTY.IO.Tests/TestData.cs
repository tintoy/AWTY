using System;
using System.Collections.Generic;

namespace AWTY.IO.Tests
{
    /// <summary>
    ///     Data for unit tests.
    /// </summary>
    public static class TestData
    {
        /// <summary>
        ///     Create a pseudo-random number generator for use in tests.
        /// </summary>
        /// <returns>
        ///     The new <see cref="Random"/>.
        /// </returns>
        public static Random CreateRandom()
        {
            return new Random(19123 /* hur, dur, magic */);
        }

        /// <summary>
        ///     Data for theory tests.
        /// </summary>
        public static class Theory
        {
            /// <summary>
            ///     Data for <see cref="AWTY.IO.ProgressStream"/> read theory tests.
            /// </summary>
            public static IEnumerable<object[]> ProgressStreamRead
            {
                get
                {
                    yield return Row.ProgressStreamRead(
                        total: 100,
                        increment: 1,
                        chunkSize: 10,
                        expectedPercentages: new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }
                    );

                    yield return Row.ProgressStreamRead(
                        total: 100,
                        increment: 5,
                        chunkSize: 10,
                        expectedPercentages: new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }
                    );

                    yield return Row.ProgressStreamRead(
                        total: 100,
                        increment: 20,
                        chunkSize: 10,
                        expectedPercentages: new int[] { 20, 40, 60, 80, 100 }
                    );

                    yield return Row.ProgressStreamRead(
                        total: 50,
                        increment: 10,
                        chunkSize: 5,
                        expectedPercentages: new int[] { 20, 40, 60, 80, 100 }
                    );

                    yield return Row.ProgressStreamRead(
                        total: 70,
                        increment: 3,
                        chunkSize: 5,
                        expectedPercentages: new int[] { 8, 17, 25, 30, 38, 47, 55, 60, 68, 77, 85, 90, 98, 100 }
                    );
                }
            }
        }

        /// <summary>
        ///     Data row factories for <see cref="AWTY.IO.ProgressStream"/> theory tests.
        /// </summary>
        static class Row
        {
            /// <summary>
            ///     Create a new data row for the <see cref="AWTY.IO.ProgressStream"/> read test theory.
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
            public static object[] ProgressStreamRead(long total, long increment, int chunkSize, int[] expectedPercentages)
            {
                return new object[4] { total, increment, chunkSize, expectedPercentages };
            }
        }
    }
}