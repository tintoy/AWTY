using System;
using Microsoft.Extensions.Logging;

namespace AWTY.Logging
{
    /// <summary>
    ///     An adapter that writes progress information to an <see cref="ILogger"/>.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The type of value used to represent progress.
    /// </typeparam>
    public class ProgressLoggingAdapter<TValue>
        : IObserver<ProgressData<TValue>>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        /// <summary>
        ///     Object used to synchronise access to adapter state.
        /// </summary>
        readonly object     _stateLock = new object();

        /// <summary>
        ///     The logger to which progress information will be written.
        /// </summary>
        readonly ILogger    _logger;

        /// <summary>
        ///     The name used to identify progress data in log entries.
        /// </summary>
        readonly string     _name;

        /// <summary>
        ///     The level at which progress data should be logged.
        /// </summary>
        readonly LogLevel   _level;

        /// <summary>
        ///     Create a new <see cref="ProgressLoggingAdapter{TValue}"/>.
        /// </summary>
        /// <param name="logger">
        ///     The logger to which progress information will be written.
        /// </param>
        /// <param name="name">
        ///     The name used to identify progress data in log entries.
        /// </param>
        /// <param name="level">
        ///     The level at which progress data should be logged.
        /// </param>
        public ProgressLoggingAdapter(ILogger logger, string name, LogLevel level = LogLevel.Information)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Must supply a valid name.", nameof(name));

            if (level > LogLevel.Information)
                throw new ArgumentOutOfRangeException(nameof(level), level, "Log level cannot be higher than Information.");

            _logger = logger;
            _name = name;
            _level = level;
        }

        /// <summary>
        ///     Has progress started being reported?
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        ///     Called when the next progress value is available.
        /// </summary>
        /// <param name="value">
        ///     The progress value.
        /// </param>
        void IObserver<ProgressData<TValue>>.OnNext(ProgressData<TValue> value)
        {
            lock (_stateLock)
            {
                if (!Started)
                {
                    _logger.LogTrace(ProgressEventIds.Started, null,
                        "Progress started for '{Name}'.",
                        _name
                    );

                    Started = true;
                }
                
                LogProgress(value);
            }
        }

        /// <summary>
        ///     Called when the sequence of progress data is complete.
        /// </summary>
        void IObserver<ProgressData<TValue>>.OnCompleted()
        {
            lock (_stateLock)
            {
                if (Started)
                {
                    _logger.LogTrace(ProgressEventIds.Ended, null,
                        "Progress ended for '{Name}'.",
                        _name
                    );

                    Started = false;
                }
            }
        }

        /// <summary>
        ///     Called when the publisher of progress data has encountered an error.
        /// </summary>
        /// <param name="error">
        ///     An <see cref="Exception"/> representing the error.
        /// </param>
        void IObserver<ProgressData<TValue>>.OnError(Exception error)
        {
            lock (_stateLock)
            {
                if (error != null)
                {
                    _logger.LogError(ProgressEventIds.Error, error,
                        "Progress for '{Name}' indicates an error was encountered: {ErrorMessage}",
                        _name,
                        error.Message
                    );
                }
                else
                {
                    _logger.LogError(ProgressEventIds.Error,
                        "IObserver<ProgressData<TValue>>.OnError called with a null error (this is almost certainly a bug).",
                        _name
                    );
                }

                
            }
        }

        /// <summary>
        ///     Write progress data to the log.
        /// </summary>
        /// <param name="value">
        ///     The progress value to log.
        /// </param>
        void LogProgress(ProgressData<TValue> value)
        {
            if (value == null)
                return;

            const string messageFormat = "Progress for '{Name}': {PercentComplete}% complete.";
            object[] formatArgs = { _name, value.PercentComplete };

            switch (_level)
            {
                case LogLevel.Information:
                {
                    _logger.LogInformation(ProgressEventIds.Data, messageFormat, formatArgs);

                    break;
                }
                case LogLevel.Debug:
                {
                    _logger.LogDebug(ProgressEventIds.Data, messageFormat, formatArgs);

                    break;
                }
                case LogLevel.Trace:
                {
                    _logger.LogTrace(ProgressEventIds.Data, messageFormat, formatArgs);

                    break;
                }
            }
        }
    }
}