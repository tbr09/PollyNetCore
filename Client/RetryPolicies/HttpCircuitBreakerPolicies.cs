using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Net.Http;

namespace Client.RetryPolicies
{
    public class HttpCircuitBreakerPolicies
    {
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetHttpCircuitBreakerPolicy(ILogger logger, ICircuitBreakerPolicyConfig circuitBreakerPolicyConfig)
        {
            return HttpPolicyBuilders.GetBaseBuilder()
                                    .CircuitBreakerAsync(circuitBreakerPolicyConfig.RetryCount,
                                                        TimeSpan.FromSeconds(circuitBreakerPolicyConfig.BreakDuration),
                                                       (result, breakDuration) =>
                                                       {
                                                           OnHttpBreak(result, TimeSpan.FromSeconds(circuitBreakerPolicyConfig.BreakDuration), circuitBreakerPolicyConfig.RetryCount, logger);
                                                       },
                                                       () =>
                                                       {
                                                           OnHttpReset(logger);
                                                       });
        }

        private static void OnHttpBreak(DelegateResult<HttpResponseMessage> result, TimeSpan breakDuration, int retryCount, ILogger logger)
        {
            logger.LogWarning($"Service shutdown during {breakDuration} after {retryCount} failed retries.");
            throw new BrokenCircuitException("Service inoperative. Please try again later");
        }

        public static void OnHttpReset(ILogger logger)
        {
            logger.LogInformation("Service restarted.");
        }
    }
}
