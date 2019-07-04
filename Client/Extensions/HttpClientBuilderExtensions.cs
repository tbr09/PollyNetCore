using Client.RetryPolicies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Client.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddPolicyHandlers(this IHttpClientBuilder builder,
            ILoggerFactory loggerFactory,
            string policiesSectionName,
            IConfiguration configuration)
        {
            var logger = loggerFactory.CreateLogger("PollyLogger");
            
            var policyConfig = new PollyPolicyConfig();
            configuration.Bind(policiesSectionName, policyConfig);

            var circuitBreakerPolicyConfig = (ICircuitBreakerPolicyConfig)policyConfig;
            var retryPolicyConfig = (IRetryPolicyConfig)policyConfig;

            return builder.AddRetryPolicyHandler(logger, retryPolicyConfig)
                    .AddCircuitBreakerHandler(logger, circuitBreakerPolicyConfig);

        }

        public static IHttpClientBuilder AddRetryPolicyHandler(this IHttpClientBuilder builder, ILogger logger, IRetryPolicyConfig config)
        {
            return builder.AddPolicyHandler(HttpRetryPolicies.GetHttpRetryPolicy(logger, config));
        }

        public static IHttpClientBuilder AddCircuitBreakerHandler(this IHttpClientBuilder builder, ILogger logger, ICircuitBreakerPolicyConfig config)
        {
            return builder.AddPolicyHandler(HttpCircuitBreakerPolicies.GetHttpCircuitBreakerPolicy(logger, config));
        }
    }
}
