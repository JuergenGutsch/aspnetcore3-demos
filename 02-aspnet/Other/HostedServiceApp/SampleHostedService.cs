
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using Greet;

namespace HostedServiceApp
{
    public class SampleHostedService : IHostedService
    {
        private readonly ILogger<SampleHostedService> _logger;

        // inject a logger
        public SampleHostedService(ILogger<SampleHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hosted service starting");

            return Task.Factory.StartNew(async () =>
            {
                // loop until a cancalation is requested
                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Hosted service executing - {0}", DateTime.Now);
                    try
                    {
                        #region hidden

                        try
                        {
                            var channel = new Channel("localhost:50051", SslCredentials.Insecure);
                            var client = new Greeter.GreeterClient(channel);
                            var response = await client.SayHelloAsync(new HelloRequest
                            {
                                Name = "dotnet Cologne 2019 - From Hosted Service"
                            });

                            _logger.LogInformation("gRPC response - {0}", response.Message);

                        }
                        catch (Exception ex)
                        {

                            _logger.LogError(ex.ToString());
                        }


                        #endregion

                        // wait for 2 seconds
                        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                    }
                    catch (OperationCanceledException) { }
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hosted service stopping");
            return Task.CompletedTask;
        }
    }
}