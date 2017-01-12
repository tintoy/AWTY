namespace AWTY.Http.IntegrationTests
{
    /// <summary>
    ///     Data for unit tests.
    /// </summary>
    public static class TestData
    {
        /// <summary>
        ///     Create a new data row for an <see cref="AWTY.Http.ProgressHander"/> HTTP request theory.
        /// </summary>
        /// <param name="bufferSize">
        ///     The minimum expected change in percentage completion.
        /// </param>
        /// <param name="responseSize">
        ///     The number of bytes to read from the stream in each iteration.
        /// </param>
        /// <param name="minPercentage">
        ///     The minimum change in percentage to capture.
        /// </param>
        /// <param name="expectedPercentages">
        ///     An array of expected percentage values.
        /// </param>
        /// <returns>
        ///     An array containing the row's data.
        /// </returns>
        public static object[] ProgressHandlerRow(int bufferSize, int responseSize, int minPercentage, int[] expectedPercentages)
        {
            return new object[4] { bufferSize, responseSize, minPercentage, expectedPercentages };
        }
    }
}