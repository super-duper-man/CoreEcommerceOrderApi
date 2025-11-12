using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;
using Resource.Share.Lib.Logs;

namespace OrderApi.Application.DependencyInjection
{
    public static class ApplicationServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection service, IConfiguration config)
        {
            service.AddHttpClient<IOrderService, OrderService>(options =>
            {
                options.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
                options.Timeout = TimeSpan.FromSeconds(2);
            });

            //Create retry strategy
            var retryStrategy = new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>()
                                                    .Handle<HttpRequestException>(),
                BackoffType = DelayBackoffType.Constant,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                OnRetry = arg =>
                {
                    string message = $"OnRetry Attempt: {arg.AttemptNumber} Outcome {arg.Outcome}";

                    LogException.LogToConsole(message);
                    LogException.LogToDebugger(message);

                    return ValueTask.CompletedTask;
                }
            };
            service.AddResiliencePipeline("retry-pipline", builder =>
            {
                builder.AddRetry(retryStrategy);
            });

            return service;
        }
    }
}
