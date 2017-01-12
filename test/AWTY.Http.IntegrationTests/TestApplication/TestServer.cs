using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using Xunit.Abstractions;

namespace AWTY.Http.IntegrationTests.TestApplication
{
    /// <summary>
    ///     A local web server for use in integration-test scenarios.
    /// </summary>
    /// <remarks>
    ///     We use this instead of an in-memory test server because we want actual network connectivity as part of our tests.
    /// </remarks>
    public class TestServer
        : IDisposable
    {
        /// <summary>
        ///     Object used to synchronise access to server state.
        /// </summary>
        object _stateLock = new object();

        /// <summary>
        ///     The server's ASP.NET web host.
        /// </summary>
        IWebHost _host;

        /// <summary>
        ///     Create a new test server.
        /// </summary>
        public TestServer()
        {
            Console.WriteLine("TestServer constructor");

            Port = 15123;
            BaseAddress = new Uri($"http://localhost:{Port}/");
        }

        /// <summary>
        ///     The TCP port that the server listens on.
        /// </summary>
        public int Port { get; }

        /// <summary>
        ///     The base address that the server listens on.
        /// </summary>
        public Uri BaseAddress { get; }

        /// <summary>
        ///     Is the server running?
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///     Start the test server.
        /// </summary>
        /// <param name="testOutput">
        ///     The test output helper (if available).
        /// </param>
        public void Start(ITestOutputHelper testOutput = null)
        {
            lock(_stateLock)
            {
                if (IsRunning)
                    return;

                _host = new WebHostBuilder()
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton<ITestOutputHelper>(testOutput);
                    })
                    .UseKestrel()
                    .UseStartup<TestServerStartup>()
                    .UseUrls(BaseAddress.AbsoluteUri)
                    .Build();

                _host.Start();

                IsRunning = true;
            }
        }

        /// <summary>
        ///     Stop the test server.
        /// </summary>
        public void Stop()
        {
            lock(_stateLock)
            {
                if (!IsRunning)
                    return;

                if (_host == null)
                    return;

                _host.Dispose();
                _host = null;

                IsRunning = false;
            }
        }

        /// <summary>
        ///     Dispose of resources being used by the test server.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        ///     Create an <see cref="HttpClient"/> that targets the test server.
        /// </summary>
        /// <returns>
        ///     The configured <see cref="HttpClient"/> (base address will be set to the server's base address).
        /// </returns>
        public HttpClient CreateClient()
        {
            return CreateClient(
                new HttpClientHandler()
            );
        }

        /// <summary>
        ///     Create an <see cref="HttpClient"/> that targets the test server.
        /// </summary>
        /// <param name="clientHandler">
        ///     The client message handler that acts as the <see cref="HttpClient"/>'s message pipeline terminus.
        /// </param>
        /// <returns>
        ///     The configured <see cref="HttpClient"/> (base address will be set to the server's base address).
        /// </returns>
        public HttpClient CreateClient(HttpMessageHandler clientHandler)
        {
            return new HttpClient(clientHandler)
            {
                BaseAddress = BaseAddress
            };
        }
    }
}