using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Extensions
{
    public class FormFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {            var formFileParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) || 
                          p.ParameterType == typeof(IFormFile[]) || 
                          p.ParameterType == typeof(List<IFormFile>))
                .ToList();

            if (formFileParams.Any())
            {
                // Remove IFormFile parameters from the operation parameters
                var parametersToRemove = operation.Parameters?
                    .Where(p => formFileParams.Any(fp => fp.Name != null && fp.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (parametersToRemove != null && operation.Parameters != null)
                {                    foreach (var param in parametersToRemove)
                    {
                        operation.Parameters.Remove(param);
                    }
                }

                // Add multipart/form-data request body
                operation.RequestBody = new OpenApiRequestBody
                {
                    Required = true,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = formFileParams.ToDictionary(
                                    param => param.Name ?? "file",
                                    param => new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                ),
                                Required = formFileParams.Select(p => p.Name ?? "file").ToHashSet()                            }
                        }
                    }
                };

                // Add other non-file parameters to the schema if they exist
                var otherParams = context.MethodInfo.GetParameters()                    .Where(p => !formFileParams.Contains(p) && 
                               p.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.FromFormAttribute), false).Any())
                    .ToList();

                foreach (var param in otherParams)
                {
                    operation.RequestBody.Content["multipart/form-data"].Schema.Properties ??= new Dictionary<string, OpenApiSchema>();

                    operation.RequestBody.Content["multipart/form-data"].Schema.Properties[param.Name ?? "parameter"] = new OpenApiSchema
                    {
                        Type = GetSchemaType(param.ParameterType)
                    };
                }
            }

            // Also handle parameters from complex types that contain IFormFile properties
            var formFileParamsFromProperties = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType != typeof(IFormFile) && 
                           p.ParameterType != typeof(IFormFile[]) && 
                           p.ParameterType != typeof(List<IFormFile>) &&
                           p.ParameterType.GetProperties().Any(prop => 
                               prop.PropertyType == typeof(IFormFile) ||
                               prop.PropertyType == typeof(IFormFile[]) ||
                               prop.PropertyType == typeof(List<IFormFile>)))
                .ToList();

            if (formFileParamsFromProperties.Any())
            {
                if (operation.RequestBody == null)
                {
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Required = true,
                        Content = new Dictionary<string, OpenApiMediaType>()
                    };
                }

                if (!operation.RequestBody.Content.ContainsKey("multipart/form-data"))
                {
                    operation.RequestBody.Content["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>()
                        }
                    };
                }

                foreach (var param in formFileParamsFromProperties)
                {
                    var fileProperties = param.ParameterType.GetProperties()
                        .Where(prop => prop.PropertyType == typeof(IFormFile) ||
                                      prop.PropertyType == typeof(IFormFile[]) ||
                                      prop.PropertyType == typeof(List<IFormFile>));

                    foreach (var prop in fileProperties)
                    {
                        operation.RequestBody.Content["multipart/form-data"].Schema.Properties[prop.Name] = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        };
                    }
                }
            }
        }

        private static string GetSchemaType(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(int?)) return "integer";
            if (type == typeof(long) || type == typeof(long?)) return "integer";
            if (type == typeof(bool) || type == typeof(bool?)) return "boolean";
            if (type == typeof(double) || type == typeof(double?) || 
                type == typeof(float) || type == typeof(float?) ||
                type == typeof(decimal) || type == typeof(decimal?)) return "number";
            
            return "string"; // Default fallback
        }
    }
}
