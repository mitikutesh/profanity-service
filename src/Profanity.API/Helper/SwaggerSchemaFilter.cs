using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profanity.API.Helper
{
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
                return;
            foreach (var prop in schema.Properties)
            {
                if (prop.Value.Default != null && prop.Value.Example == null)
                    prop.Value.Example = prop.Value.Default;
            }
        }

    }
}
