using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.RetryPolicies
{
    public interface ICircuitBreakerPolicyConfig
    {
        int RetryCount { get; set; }
        int BreakDuration { get; set; }
    }
}
