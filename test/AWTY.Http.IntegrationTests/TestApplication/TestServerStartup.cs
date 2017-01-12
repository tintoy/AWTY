using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AWTY.Http.IntegrationTests.TestApplication
{
    /// <summary>
    ///     Application configuration for the test server.
    /// </summary>
    public class TestServerStartup
    {
        /// <summary>
        ///     Configure application services.
        /// </summary>
        /// <param name="services">
        ///     The application service collection.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddLogging();
        }

        /// <summary>
        ///     Configure the application.
        /// </summary>
        /// <param name="app">
        ///     The application pipeline builder.
        /// </param>
        /// <param name="loggerFactory">
        ///     The application logger factory.
        /// </param>
        /// <param name="testOutput">
        ///     XUnit test output (if available).
        /// </param>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, ITestOutputHelper testOutput)
        {
            // Test output helper may not necessarily be available.
            if (testOutput != null)
            {
                loggerFactory.AddProvider(
                    new TestOutputLoggerProvider(testOutput, LogLevel.Trace)
                );
            }

            app.UseMvc();
        }
    }
}