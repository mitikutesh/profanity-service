using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profanity.API.HealthCheck.SwaggerHelper
{
    public class DatabaseHealthChecksFilter : BaseHealthChecksFilter, IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            => Fun(swaggerDoc, context, @"/api/health/database");
    }
    public class ServiceHealthChecksFilter : BaseHealthChecksFilter, IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            => Fun(swaggerDoc, context, @"/api/health/services");
    }
    public class QuickHealthChecksFilter : BaseHealthChecksFilter, IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            => Fun(swaggerDoc, context, @"/api/heatlh");
    }
    // DatabaseHealthChecksFilter
    public class BaseHealthChecksFilter
    {
        public void Fun(OpenApiDocument openApiDocument, DocumentFilterContext context, string HealthCheckEndpoint)
        {
            var pathItem = new OpenApiPathItem();

            var operation = new OpenApiOperation();
            operation.Tags.Add(new OpenApiTag { Name = "HealthCheck" });

            var properties = new System.Collections.Generic.Dictionary<string, OpenApiSchema>();
            properties.Add("status", new OpenApiSchema() { Type = "string" });
            properties.Add("errors", new OpenApiSchema() { Type = "array" });

            var response = new OpenApiResponse();
            response.Content.Add("application/json", new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    AdditionalPropertiesAllowed = true,
                    Properties = properties,
                }
            });

            operation.Responses.Add("200", response);
            pathItem.AddOperation(OperationType.Get, operation);
            openApiDocument?.Paths.Add(HealthCheckEndpoint, pathItem);
        }
    }

}
