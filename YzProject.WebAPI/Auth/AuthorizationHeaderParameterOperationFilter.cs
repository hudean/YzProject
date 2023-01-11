using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace YzProject.WebAPI.Auth
{
    /// <summary>
    /// net core api swagger 添加自定义请求头（header）参数
    /// </summary>
    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //throw new System.NotImplementedException();
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
            var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

            if (isAuthorized && !allowAnonymous)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();


                //operation.Parameters.Add(new OpenApiParameter
                //{
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Description = "access token",
                //    Required = true
                //});

                //operation.Parameters.Add(new OpenApiParameter
                //{
                //    Name = "UserName",
                //    In = ParameterLocation.Header,
                //    Description = "参数备注:用户名称",
                //    Required = true,
                //    Schema = new OpenApiSchema
                //    {
                //        Type = "string",
                //        //Default = new OpenApiString("Bearer ")
                //        Default = new OpenApiString("admin")
                //    }
                //});


            }


        }
    }
}
