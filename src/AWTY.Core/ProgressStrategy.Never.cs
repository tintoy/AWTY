namespace AWTY
{
    using Core.Strategies;

    /// <summary>
    ///     Well-known progress strategies.
    /// </summary>
    public static partial class ProgressStrategy
    {
        /// <summary>
        ///     Progress strategies that never notify.
        /// </summary>
        public static class Never
        {
            /// <summary>
            ///     A progress strategy for 32-bit integer values that never notifies.
            /// </summary>
            public static ProgressStrategy<int> Int32() => new Never32();

            /// <summary>
            ///     A progress strategy for 64-bit integer values that never notifies.
            /// </summary>
            public static ProgressStrategy<long> Int64() => new Never64();
        }

#pragma warning disable CS0067

        /// <summary>
        ///     A 32-bit integer progress-notification strategy that never reports progress.
        /// </summary>
        sealed class Never32
            : ProgressStrategy<int>
        {
            /// <summary>
            ///     Default constructor.
            /// </summary>
            public Never32()
            {
            }

            /// <summary>
            ///     The progress value equivalent to 0.
            /// </summary>
            protected override int Zero => 0;

            /// <summary>
            ///     Report the current progress.
            /// </summary>
            /// <param name="current">
            ///     The current progress value.
            /// </param>
            /// <param name="total">
            ///     The total value against which progress is measured.
            /// </param>
            protected override void ReportProgress(int current, int total)
            {
                // Progress never changes.
            }
        }

        /// <summary>
        ///     A 32-bit integer progress-notification strategy that never reports progress.
        /// </summary>
        sealed class Never64
            : ProgressStrategy<long>
        {
            /// <summary>
            ///     Default constructor.
            /// </summary>
            public Never64()
            {
            }

            /// <summary>
            ///     The progress value equivalent to 0.
            /// </summary>
            protected override long Zero => 0;

            /// <summary>
            ///     Report the current progress.
            /// </summary>
            /// <param name="current">
            ///     The current progress value.
            /// </param>
            /// <param name="total">
            ///     The total value against which progress is measured.
            /// </param>
            protected override void ReportProgress(long current, long total)
            {
                // Progress never changes.
            }
        }

#pragma warning restore CS0067
    }
}