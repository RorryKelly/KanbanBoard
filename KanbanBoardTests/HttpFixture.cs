using System;
using System.Net.Http;
using System.Reflection;
using KanbanBoard2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace KanbanBoardTests
{
    public class HttpFixture : IDisposable
    {
        private IHost _host;
        private TestServer? _server;
        private HttpClient? _client;

        private TestServer Server => _server ??= _host.GetTestServer();
        protected HttpClient Client => _client ??= CreateClient();
        
        protected HttpFixture()
        {
            _host = CreateAndStartHost();
        }

        private IHost CreateAndStartHost()
        {
            var configuration = new ConfigurationBuilder()
                .SetFileProvider(new EmbeddedFileProvider(Assembly.GetExecutingAssembly()))
                .Build();

            var host = new HostBuilder()
                .ConfigureWebHost(builder =>
                {
                    builder.UseConfiguration(configuration)
                        .ConfigureTestServices(ConfigureTestServices)
                        .UseTestServer()
                        .UseStartup<Startup>();
                }).Start();

            return host;
        }

        private HttpClient CreateClient()
        {
            var client = Server.CreateClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }
        
        private void ConfigureTestServices(IServiceCollection services)
        {
            services.AddControllers().AddApplicationPart(Assembly.GetExecutingAssembly());
        }
        public void Dispose()
        {
        }
    }
}