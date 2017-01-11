using System;
using System.Threading;

namespace AWTY
{
    /// <summary>
    ///     Provides a thread-local / async-local context for progress reporting.
    /// </summary>
    public sealed class ProgressContext
    {
        /// <summary>
        ///     Object used to synchronise access to <see cref="ProgressContext"/> state.
        /// </summary>
        static readonly object                      _stateLock = new object();

        /// <summary>
        ///     The current progress context.
        /// </summary>
        static readonly AsyncLocal<ProgressContext> _current = new AsyncLocal<ProgressContext>();

        /// <summary>
        ///     The current progress context.
        /// </summary>
        public static ProgressContext Current
        {
            get
            {
                lock(_stateLock)
                {
                    if (_current.Value == null)
                        _current.Value = new ProgressContext();

                    return _current.Value;
                }
            }
        }

        /// <summary>
        ///     Create a new progress context.
        /// </summary>
        ProgressContext()
        {
        }

        /// <summary>
        ///     An arbitrary identifier for the progress context.
        /// </summary>
        /// <remarks>
        ///     Use this value to correlate progress notifications with requests made as part of the current logical call.
        /// </remarks>
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
