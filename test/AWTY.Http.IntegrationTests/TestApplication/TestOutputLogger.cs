using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AWTY.Http.IntegrationTests.TestApplication
{
    /// <summary>
    ///     An <see cref="ILogger"/> that writes its output to XUnit's <see cref="ITestOutputHelper"/>.
    /// </summary>
    public class TestOutputLogger
        : ILogger
    {
        /// <summary>
        ///     The logger category name.
        /// </summary>
        readonly string             _categoryName;

        /// <summary>
        ///     The logger's minimum log level.
        /// </summary>
        readonly LogLevel           _level;

        /// <summary>
        ///     The XUnit test output helper.
        /// </summary>
        readonly ITestOutputHelper  _testOutput;

        /// <summary>
        ///     Create a new test output logger.
        /// </summary>
        /// <param name="categoryName">
        ///     The logger category name.
        /// </param>
        /// <param name="level">
        ///     The logger's minimum log level.
        /// </param>
        /// <param name="testOutput">
        ///     The test output helper.
        /// </param>
        public TestOutputLogger(string categoryName, LogLevel level, ITestOutputHelper testOutput)
        {
            if (testOutput == null)
                throw new ArgumentNullException(nameof(testOutput));

            _categoryName = categoryName;
            _level = level;
            _testOutput = testOutput;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // TODO: Support log scopes.

            return Disposable.Empty;
        }

        /// <summary>
        ///     Determine whether the logger is enabled for the specified log level.
        /// </summary>
        /// <param name="logLevel">
        ///     The log level to evaluate.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the logger is enabled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _level;
        }

        /// <summary>
        ///     Output a log entry.
        /// </summary>
        /// <param name="logLevel">
        ///     The log entry's associated level (severity).
        /// </param>
        /// <param name="eventId">
        ///     The event Id associated with the log entry.
        /// </param>
        /// <param name="state">
        ///     State data associated with the log entry.
        /// </param>
        /// <param name="exception">
        ///     The exception (if any) associated with the log entry.
        /// </param>
        /// <param name="formatter">
        ///     A delegate that, given the state data and exception, formats the log entry's message.
        /// </param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);

            try
            {
                _testOutput.WriteLine("[{0}] {1}: {2}", logLevel, _categoryName, message);
            }
            catch (InvalidOperationException)
            {
                // Test has completed before server shutdown.
            }
        }
    }

    /// <summary>
    ///     An <see cref="ILogger"/> that produces loggers which write their output to XUnit's <see cref="ITestOutputHelper"/>.
    /// </summary>
    public class TestOutputLoggerProvider
        : ILoggerProvider
    {
        /// <summary>
        ///     The XUnit test output helper.
        /// </summary>
        readonly ITestOutputHelper  _testOutput;

        /// <summary>
        ///     The minimum log level for loggers produced by the logger provider.
        /// </summary>
        readonly LogLevel           _level;

        /// <summary>
        ///     Create a new <see cref="TestOutputLoggerProvider"/>.
        /// </summary>
        /// <param name="testOutput">
        ///     The XUnit test output helper.
        /// </param>
        /// <param name="level">
        ///     The minimum log level for loggers produced by the logger provider.
        /// </param>
        public TestOutputLoggerProvider(ITestOutputHelper testOutput, LogLevel level)
        {
            if (testOutput == null)
                throw new ArgumentNullException(nameof(testOutput));

            _testOutput = testOutput;
            _level = level;
        }

        /// <summary>
        ///     Dispose of resources being used by the logger provider.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        ///     Create a new logger.
        /// </summary>
        /// <param name="categoryName">
        ///     The logger category name.
        /// </param>
        /// <returns>
        ///     The new <see cref="ILogger"/>.
        /// </returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new TestOutputLogger(categoryName, _level, _testOutput);
        }
    }
}