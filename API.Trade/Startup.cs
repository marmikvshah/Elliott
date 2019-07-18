using API.Trade.Common;
using API.Trade.Data.Commands;
using API.Trade.Data.Commands.Repositories.Interfaces;
using API.Trade.Data.Queries;
using API.Trade.Data.Queries.Interface;
using API.Trade.Domain.Commands.Allocate;
using API.Trade.Domain.Commands.Allocate.Validators;
using API.Trade.Domain.Commands.Create;
using API.Trade.Domain.Commands.Create.Validators;
using API.Trade.Domain.Queries.Trade.Validators;
using API.Trade.Shared;
using API.Trade.Shared.Logger;
using API.Trade.Shared.Messaging;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Elliott.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var databaseConnectionString = Configuration["ConnectionStrings:connectionString"];

            services.AddOptions();
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            services.AddSingleton(spa => spa.GetService<IOptions<ConnectionStrings>>().Value);

            // Add support for in-memory cache.
            services.AddMemoryCache();
            var sp = services.BuildServiceProvider();

            services.AddScoped<ILogger, TLogger>();
            services.AddScoped<IDapperExtensions, DapperExtensions>();

            services.AddScoped<ITradeAllocateRepository, TradeAllocateRepository>();
            services.AddScoped<ITradeCreateRepository, TradeCreateRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ISecurityRepository, SecurityRepository>();
            services.AddScoped<ITradeGetRepository, TradeGetRepository>();

            services.AddMessagingDependencies();
            services.AddDefaultMessagingBehaviours();
            services.AddMessagingObjects(typeof(AllocateTradeCommandHandler).GetTypeInfo().Assembly);

            services.AddSingleton<IValidator<AllocateTradeCommandRequests>, AllocateTradeCommandRequestsValidator>();
            services.AddSingleton<IValidator<CreateTradeCommandRequests>, CreateTradeCommandRequestsValidator>();

            services.AddSingleton<IValidator<GetTradeAllocationByDayQueryRequest>, GetTradeAllocationByDayQueryRequestValidator>();
            services.AddSingleton<IValidator<GetTradeStatusQueryRequest>, GetTradeStatusQueryRequestValidator>();

            IMvcBuilder MvcBuilder = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            MvcBuilder.AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                fv.RegisterValidatorsFromAssemblyContaining<GetTradeAllocationByDayQueryRequestValidator>();
                fv.ValidatorFactory = new CustomValidatorFactory(sp);
            });
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
