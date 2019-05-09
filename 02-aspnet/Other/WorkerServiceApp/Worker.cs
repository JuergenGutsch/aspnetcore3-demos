using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Greet;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerServiceApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);



                #region hidden

                try
                {
                    var channel = new Channel("localhost:50051", SslCredentials.Insecure);
                    var client = new Greeter.GreeterClient(channel);
                    var response = await client.SayHelloAsync(new HelloRequest
                    {
                        Name = "dotnet Cologne 2019 - From Worker Service"
                    });

                    _logger.LogInformation("gRPC response - {0}", response.Message);

                }
                catch (Exception ex)
                {

                    _logger.LogError(ex.ToString());
                }


                #endregion

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
