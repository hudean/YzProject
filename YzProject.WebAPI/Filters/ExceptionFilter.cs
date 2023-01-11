using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using YzProject.Domain.ResultModel;

namespace YzProject.WebAPI.Filters
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {
        //https://learn.microsoft.com/zh-cn/aspnet/core/mvc/controllers/filters?view=aspnetcore-5.0#exception-filters
        //https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.mvc.filters.filtercontext?view=aspnetcore-6.0
        private readonly ILogger<ExceptionFilter> _logger;
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 异常时
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception.InnerException == null ? context.Exception : context.Exception.InnerException;
            //myLogger.Error("服务器请求错误：" + exception.Message, exception);
            _logger.LogError(exception, "服务器请求错误：" + exception.Message);
            //context.Result = new JsonResult(new { code = HttpStatusCode.InternalServerError, msg = "服务器请求错误", data = "" });
            context.Result = new OkObjectResult(new ResultData(0, "服务器请求错误",null));
            return;
        }
    }
}
