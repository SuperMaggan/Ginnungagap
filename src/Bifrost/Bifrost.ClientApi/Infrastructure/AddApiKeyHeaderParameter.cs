using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bifrost.ClientAPI.Infrastructure
{
  public class AddApiKeyHeaderParameter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      if (operation.Parameters == null)
        operation.Parameters = new List<OpenApiParameter>();


      operation.Parameters.Add(new  OpenApiParameter()
      {
        Name = "ApiKey",
        Required = true
      });
    }
  }
}