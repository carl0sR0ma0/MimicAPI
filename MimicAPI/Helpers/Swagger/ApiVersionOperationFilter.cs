using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MimicAPI.Helpers.Swagger
{
    public class ApiVersionOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionApiVersionModel = context.ApiDescription.ActionDescriptor.GetApiVersion();
            if (actionApiVersionModel is null) return;

            if (actionApiVersionModel.DeclaredApiVersions.Any()) operation.Responses.SelectMany(p => actionApiVersionModel.DeclaredApiVersions.Select(version => $"{p};v={version.ToString()}")).ToList();
            else
                operation.Responses.SelectMany(p => actionApiVersionModel.ImplementedApiVersions.Select(version => $"{p};v={version.ToString()}")).ToList();
        }
    }
}
