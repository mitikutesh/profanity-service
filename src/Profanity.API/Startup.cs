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
using Profanity.API.Controllers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            services.AddScoped<IAccountService, AccountService>();

            //jwt
            services.AddJwtService(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{MainApiVersion.MajorVersion}", new OpenApiInfo { Title = "Profanity.API", Version = $"V{MainApiVersion.MajorVersion}" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Write bearer token in the field",
                    Scheme = "Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer" },
                        // Scheme = "oauth2",
                        // Name = "Bearer",
                        //In = ParameterLocation.Header
                        },
                    Array.Empty<string>()
                }
                });

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

            //api versioning
            services.AddApiVersioning(config => {
                config.DefaultApiVersion = MainApiVersion;
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.Conventions.Controller<ProfanityController>().HasApiVersion(MainApiVersion);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }

            app.UseHttpsRedirection();
            app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions {
                UseApiProblemDetailsException = true ,
                ShowApiVersion = true, 
                ApiVersion = $"{MainApiVersion.MajorVersion}",
                IsApiOnly = false,
                WrapWhenApiPathStartsWith = "/api"
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint($"v{MainApiVersion.MajorVersion}/swagger.json", $"Prfanity API v{MainApiVersion.MajorVersion}"));

            app.UseEndpoints(endpoints =>
            {
                //just check if this api is up
                endpoints.MapHealthChecks($"api/{EndPoints.HealthQuickCheck}", new HealthCheckOptions()
                {
                    Predicate = _ => false,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHealthChecks($"api/{EndPoints.HealthService}", new HealthCheckOptions()
                {
                    Predicate = reg => reg.Tags.Contains("Service"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                }).RequireAuthorization(); 
                endpoints.MapHealthChecks($"api/{EndPoints.HealthDatabase}", new HealthCheckOptions()
                {
                    Predicate = reg => reg.Tags.Contains("Database"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                }).RequireAuthorization();

                endpoints.MapControllers();
            });

          
        }
    }
}
