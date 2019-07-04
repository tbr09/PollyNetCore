using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Extensions;
using Client.HttpClients;
using Client.HttpHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Serilog;

namespace Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            LoggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }
        public ILoggerFactory LoggerFactory { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Named client
            //services.AddHttpClient("CustomerService", client =>
            //{
            //    client.BaseAddress = new Uri("http://localhost:6001/");
            //    client.DefaultRequestHeaders.Add("Accept", "application/json");
            //});

            // Typed client with extension method
            //services.AddHttpClient<CustomerClient>(client => client.BaseAddress = new Uri("http://localhost:6000"))
            //    .AddPolicyHandlers(LoggerFactory, "PollyPolicyConfig", Configuration);

            // Typed client with inline configuration
            //services.AddHttpClient<CustomerClient>(client => client.BaseAddress = new Uri("http://localhost:6000"))
            //    .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            // Typed client with HttpMessageHandler
            services.AddTransient<PollyMessageHandler>();
            services.AddHttpClient<CustomerClient>()
                .AddHttpMessageHandler<PollyMessageHandler>();

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
