using System.Collections.Generic;

namespace AWTY.Core.Tests
{
    /// <summary>
    ///     Data for unit tests.
    /// </summary>
    public static class TestData
    {
        /// <summary>
        ///     Data for theory tests.
        /// </summary>
        public static class Theory
        {
            /// <summary>
            ///     Data for the 32-bit progress sink addition theories.
            /// </summary>
            public static IEnumerable<object[]> Int32SinkAdd
            {
                get
                {
                    yield return Row.Int32ProgressSinkAdd(
                        total: 10,
                        initialValue: 0,
                        addToCurrent: 0,
                        expectedCurrent: 0
                    );
                    yield return Row.Int32ProgressSinkAdd(
                        total: 10,
                        initialValue: 0,
                        addToCurrent: 5,
                        expectedCurrent: 5
                    );
                    yield return Row.Int32ProgressSinkAdd(
                        total: 20,
                        initialValue: 5,
                        addToCurrent: 10,
                        expectedCurrent: 15
                    );

                    yield return Row.Int32ProgressSinkAdd(
                        total: 10,
                        initialValue: 10,
                        addToCurrent: 1,
                        expectedCurrent: 11
                    );
                }
            }

            /// <summary>
            ///     Data for 32-bit chunked percentage theory tests.
            /// </summary>
            public static IEnumerable<object[]> Int32ChunkedPercentage
            {
                get
                {
                    yield return Row.Int32ChunkedPercentage(
                        total: 100,
                        increment: 1,
                        chunkSize: 10,
                        expectedPercentages: new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }
                    );

                    yield return Row.Int32ChunkedPercentage(
                        total: 100,
                        increment: 5,
                        chunkSize: 10,
                        expectedPercentages: new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }
                    );

                    yield return Row.Int32ChunkedPercentage(
                        total: 100,
                        increment: 20,
                        chunkSize: 10,
                        expectedPercentages: new int[] { 20, 40, 60, 80, 100 }
                    );

                    yield return Row.Int32ChunkedPercentage(
                        total: 50,
                        increment: 10,
                        chunkSize: 5,
                        expectedPercentages: new int[] { 20, 40, 60, 80, 100 }
                    );

                    yield return Row.Int32ChunkedPercentage(
                        total: 70,
                        increment: 3,
                        chunkSize: 5,
                        expectedPercentages: new int[] { 8, 17, 25, 30, 38, 47, 55, 60, 68, 77, 85, 90, 98, 100 }
                    );
                }
            }

            /// <summary>
            ///     Data for 64-bit chunked percentage theory tests.
            /// </summary>
            public static IEnumerable<object[]> Int64ChunkedPercentage
            {
                get
                {
                    yield return Row.Int64ChunkedPercentage(
                        total: 100,
                        increment: 1,
                        chunkSize: 10,
                        expectedPercentages: new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }
                    );

                    yield return Row.Int64ChunkedPercentage(
                        total: 100,
                        increment: 5,
                        chunkSize: 10,
                        expectedPercentages: new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }
                    );

                    yield return Row.Int64ChunkedPercentage(
                        total: 100,
                        increment: 20,
                        chunkSize: 10,
                        expectedPercentages: new int[] { 20, 40, 60, 80, 100 }
                    );

                    yield return Row.Int64ChunkedPercentage(
                        total: 50,
                        increment: 10,
                        chunkSize: 5,
                        expectedPercentages: new int[] { 20, 40, 60, 80, 100 }
                    );

                    yield return Row.Int64ChunkedPercentage(
                        total: 70,
                        increment: 3,
                        chunkSize: 5,
                        expectedPercentages: new int[] { 8, 17, 25, 30, 38, 47, 55, 60, 68, 77, 85, 90, 98, 100 }
                    );
                }
            }
        }

        /// <summary>
        ///     Data row factories for theory tests.
        /// </summary>
        static class Row
        {
            /// <summary>
            ///     Create a new test data row for 32-bit progress sink Add theories.
            /// </summary>
            /// <param name="total">
            ///     The total number of bytes in the stream.
            /// </param>
            /// <param name="initialValue">
            ///     The initially-current value that the sink will contain.
            /// </param>
            /// <param name="addToCurrent">
            ///     The the progress value to add.
            /// </param>
            /// <param name="expectedPercentages">
            ///     An array of expected percentage values.
            /// </param>
            /// <returns>
            ///     An array containing the row's data.
            /// </returns>
            public static object[] Int32ProgressSinkAdd(int total, int initialValue, int addToCurrent, int expectedCurrent)
            {
                return new object[4] { total, initialValue, addToCurrent, expectedCurrent };
            }

            /// <summary>
            ///     Create a new test data row for 64-bit progress sink Add theories.
            /// </summary>
            /// <param name="total">
            ///     The total number of bytes in the stream.
            /// </param>
            /// <param name="initialCurrent">
            ///     The initially-current value that the sink will contain.
            /// </param>
            /// <param name="addToCurrent">
            ///     The the progress value to add.
            /// </param>
            /// <param name="expectedPercentages">
            ///     An array of expected percentage values.
            /// </param>
            /// <returns>
            ///     An array containing the row's data.
            /// </returns>
            public static object[] Int64ProgressSinkAdd(long total, long initialCurrent, long addToCurrent, int expectedCurrent)
            {
                return new object[4] { total, initialCurrent, addToCurrent, expectedCurrent };
            }
            
            /// <summary>
            ///     Create a new data row for 32-bit chunked percentage theories.
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
            public static object[] Int32ChunkedPercentage(int total, int increment, int chunkSize, int[] expectedPercentages)
            {
                return new object[4] { total, increment, chunkSize, expectedPercentages };
            }

            /// <summary>
            ///     Create a new data row for 64-bit chunked percentage test theories.
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
            public static object[] Int64ChunkedPercentage(long total, long increment, int chunkSize, int[] expectedPercentages)
            {
                return new object[4] { total, increment, chunkSize, expectedPercentages };
            }
        }
    }
}