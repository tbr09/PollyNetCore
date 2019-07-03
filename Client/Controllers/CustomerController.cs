using Client.Contracts;
using Client.DTO;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;
using RestEase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Client.Controllers
{
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService customerService;
        private static AsyncCircuitBreakerPolicy circuitPolicy = Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 4,
                    durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (ex, ts) =>
                    {
                        Serilog.Log.Information("onBreak");
                        Debug.WriteLine("onBreak");
                    },
                    onReset: () =>
                    {
                        Serilog.Log.Information("onReset");
                        Debug.WriteLine("onReset");
                    });

        public CustomerController()
        {
            customerService = RestClient.For<ICustomerService>($"http://localhost:6001");
        }

        [HttpGet("list/circuitWithDefault")]
        public async Task<IEnumerable<CustomerDTO>> ListCircuitWithDefault()
        {
            return circuitPolicy.CircuitState == CircuitState.Closed ?
                await circuitPolicy.ExecuteAsync(async () =>
                {
                    return await customerService.List();
                }) :
                default(IEnumerable<CustomerDTO>);
        }

        [HttpGet("list/circuit")]
        public async Task<IEnumerable<CustomerDTO>> ListCircuit()
        {
            return await circuitPolicy.ExecuteAsync(async () =>
            {
                return await customerService.List();
            });
        }

        [HttpGet("list/retryAndWait")]
        public async Task<IEnumerable<CustomerDTO>> ListRetryAndWait()
        {
            return await Policy.Handle<ApiException>(ex => ex.StatusCode == HttpStatusCode.InternalServerError)
                .Or<Exception>(ex => ex.InnerException is ArgumentNullException)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(async () =>
                {
                    return await customerService.List();
                });
        }

        [HttpGet("list/retry")]
        public async Task<IEnumerable<CustomerDTO>> ListRetry()
        {
            return await Policy.Handle<Exception>()
                .RetryAsync(3, onRetry: (exception, retryCount, context) =>
                {
                    Serilog.Log.Information("Request to customer service failed.");
                })
                .ExecuteAsync(async () =>
                {
                    return await customerService.List();
                });
        }

        [HttpGet("list")]
        public async Task<IEnumerable<CustomerDTO>> List()
            => await customerService.List();

        [HttpGet("{id}")]
        public async Task<CustomerDTO> Get(int id)
            => await customerService.Get(id);

        [HttpGet("find/{term}")]
        public async Task<IEnumerable<CustomerDTO>> Find(string term)
            => await customerService.Find(term);
    }
}
