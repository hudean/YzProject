using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.WebAPI.Filters
{
    /// <summary>
    /// 操作过滤器
    /// （文档地址）https://learn.microsoft.com/zh-cn/aspnet/core/mvc/controllers/filters?view=aspnetcore-5.0
    /// </summary>
    public class ActionFilter : IActionFilter//IAsyncActionFilter//
    {
        /// <summary>
        /// 操作之后执行
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// 操作之前执行
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            #region
            //async
            //HttpRequest request = context.HttpContext.Request;
            //if (request.Method.ToLower().Equals("post"))
            //{
            //    Stream stream = request.Body;
            //    byte[] buffer = new byte[request.ContentLength.Value];
            //    await stream.ReadAsync(buffer, 0, buffer.Length);
            //    string data = Encoding.UTF8.GetString(buffer);
            //}
            //else if (request.Method.ToLower().Equals("get"))
            //{
            //    string data = request.QueryString.Value;
            //}

            #endregion
        }

        ///// <summary>
        ///// 异步 操作之前执行
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="next"></param>
        ///// <returns></returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
