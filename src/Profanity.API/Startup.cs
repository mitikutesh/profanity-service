using AutoWrapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Profanity.API.Helper;
using Profanity.API.Model;
using Profanity.Data;
using Profanity.Data.Entities;
using Profanity.Data.Repositories;
using Profanity.Service;
using Profanity.Service.Interfaces;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using System;
using Profanity.API.HealthCheck.SwaggerHelper;
using Profanity.API.HealthCheck;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

namespace Profanity.API
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public readonly ApiVersion MainApiVersion = new ApiVersion(1, 0);

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers()
                .AddFluentValidation(fv =>
                {
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                })
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddScoped<IProfanityWord, ProfanityWord>();
            services.AddScoped<IProfanityService, ProfanityService>();

            // Add database context to services Provides controllers access to the SqlServer data
            // Reads Database ConnectionString from AppSettings Define custom ConnectionString in appsettings.local.json
            string connString = System.Environment.GetEnvironmentVariable("DefaultConnection") ?? Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connString))
            {
                throw new Exception("ConnectionString for the database was not provided. Ensure ConnectionString is defined either in environment variables or appsettings.");
            }
            services.AddDbContext<ProfanityServiceDbContext>(options =>
            {
                options.UseSqlServer(connString, b => b.MigrationsAssembly("Profanity.Data"));
            });

            //healthcheck
            services.AddHealthChecks()
             .AddDbContextCheck<ProfanityServiceDbContext>("Context", null, new[] { "Database", "SQL" })
             .AddCheck("Profanity service healthceck", () => DbHealthCheckProvider.CheckProfanityTables(connString), new[] { "Database", "SQL" });
            


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Profanity.API", Version = "v1" });
                c.OperationFilter<SwaggerFileOperationFilter>();
                c.SchemaFilter<EnumSchemaFilter>();
                c.SchemaFilter<SwaggerSchemaFilter>();
                c.DocumentFilter<DatabaseHealthChecksFilter>();
                c.DocumentFilter<ServiceHealthChecksFilter>();
                c.DocumentFilter<QuickHealthChecksFilter>();

            });
            services.AddTransient<IValidator<RequestModel>, RequestModelValidator>();
            services.AddTransient<IValidator<ProfanityEntity>, ProfanityEntityValidator>();

            services.AddAutoMapper(typeof(Startup));
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Profanity.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions {
                UseApiProblemDetailsException = true ,
                ShowApiVersion = true, 
                ApiVersion = "2.0",
                IsApiOnly = false,
                WrapWhenApiPathStartsWith = "/api"
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //just check if this api is up
                endpoints.MapHealthChecks("api/heatlh", new HealthCheckOptions()
                {
                    Predicate = _ => false,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHealthChecks("api/health/services", new HealthCheckOptions()
                {
                    Predicate = reg => reg.Tags.Contains("Service"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });// .RequireAuthorization(); in case i added authentication
                endpoints.MapHealthChecks("api/health/database", new HealthCheckOptions()
                {
                    Predicate = reg => reg.Tags.Contains("Database"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });// .RequireAuthorization(); in case i added authentication

                endpoints.MapControllers();
            });
        }
    }
}
