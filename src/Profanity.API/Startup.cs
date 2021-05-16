using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Profanity.API.Helper;
using Profanity.API.Model;
using Profanity.Data;
using Profanity.Data.Entities;
using Profanity.Data.Repositories;
using Profanity.Service;
using Profanity.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Profanity.API
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

            services.AddControllers()
                .AddFluentValidation(fv =>
                {
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                })
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddScoped<IProfanityWord, ProfanityWord>();
            services.AddScoped<IProfanityService, ProfanityService>();
            services.AddDbContext<ProfanityServiceDbContext>(options =>
            {
                options.UseSqlite(
                    Configuration.GetSection("ProfanityDatabaseSettings:ConnectionString")?.Value ?? throw new Exception("Null connection string."),
                    a => a.MigrationsAssembly("Profanity.Data"));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Profanity.API", Version = "v1" });
                c.OperationFilter<SwaggerFileOperationFilter>();
                c.SchemaFilter<EnumSchemaFilter>();

            });
            services.AddTransient<IValidator<RequestModel>, RequestModelValidator>();
            services.AddTransient<IValidator<ProfanityEntity>, ProfanityEntityValidator>();
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
