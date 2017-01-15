using Microsoft.Extensions.Logging;

namespace AWTY.Logging
{
    /// <summary>
    ///     Event Ids for AWTY progress-related log entries.
    /// </summary>
    public static class ProgressEventIds
    {
        /// <summary>
        ///     The Id of the event logged when progress has started.
        /// </summary>
        public static readonly EventId Started = new EventId(13001, name: "ProgressStarted");

        /// <summary>
        ///     The Id of the event logged when progress data is observed by the adapter.
        /// </summary>
        public static readonly EventId Data = new EventId(13002, name: "ProgressData");

        /// <summary>
        ///     The Id of the event logged when progress has started.
        /// </summary>
        public static readonly EventId Ended = new EventId(13003, name: "ProgressEnded");

        /// <summary>
        ///     The Id of the event logged when progress has started.
        /// </summary>
        public static readonly EventId Error = new EventId(13004, name: "ProgressError");
    }
}