using System;
using Microsoft.AspNetCore.Hosting;

namespace AWTY.Http.IntegrationTests.TestApplication
{
    public class TestServer
        : IDisposable
    {
        IWebHost _host;

        public TestServer()
        {
            Port = new Random().Next(9000, 9700);
            BaseUri = new Uri($"http://localhost:{Port}/");

            Start();
        }

        public int Port { get; }

        public Uri BaseUri { get; }

        public void Start()
        {
            Stop();

            _host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<TestApplication.Configuration>()
                .UseUrls(BaseUri.AbsoluteUri)
                .Build();

            _host.Start();
        }

        public void Stop()
        {
            _host?.Dispose();
        }

        public void Dispose()
        {
            _host.Dispose();
        }
    }
}