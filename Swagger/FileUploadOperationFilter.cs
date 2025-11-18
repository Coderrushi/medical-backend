using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace medical_record_system_backend.Swagger
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody != null &&
                operation.RequestBody.Content.ContainsKey("multipart/form-data"))
            {
                operation.RequestBody.Content["multipart/form-data"].Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties =
                {
                    ["file"] = new OpenApiSchema
                    {
                        Description = "Upload PDF, image or document",
                        Type = "string",
                        Format = "binary"
                    }
                },
                    Required = { "file" }
                };
            }
        }
    }
}
