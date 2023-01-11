using Microsoft.AspNetCore.Mvc.Filters;

namespace YzProject.WebMVC.Filters
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {
        //private static ILog log = LogManager.GetLogger(typeof(MyExceptionFilter));

        /// <summary>
        /// 统一错误返回格式
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void OnException(ExceptionContext context)
        {
            //记录日志
            //log.Error("出现未处理异常", context.Exception);

            //日志记录
            log4net.ILog myLogger = log4net.LogManager.GetLogger(context.HttpContext.Request.Path.Value);
            //myLogger.Error("服务器请求错误：" + context.Exception.Message, context.Exception);
            var exception = context.Exception.InnerException == null ? context.Exception : context.Exception.InnerException;
            myLogger.Error("服务器请求错误：" + exception.Message, exception);
            //context.Result = new JsonResult(new { code = HttpStatusCode.InternalServerError, msg = "服务器请求错误", data = "" });
            return;
        }
    }
}
