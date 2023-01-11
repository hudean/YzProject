using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YzProject.WebMVC.Filters
{
    /// <summary>
    /// 操作过滤器
    /// </summary>
    public class ActionFilter : IActionFilter
    {
        /// <summary>
        /// Action执行之后执行
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        ///  Action执行之前执行
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //throw new System.NotImplementedException();
        }

        #region

        ///// <summary>
        /////  Action执行之前执行
        ///// </summary>
        ///// <param name="context"></param>
        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    //获取controller和action名字
        //    string controller = context.RouteData.Values["controller"]?.ToString();
        //    string action = context.RouteData.Values["action"]?.ToString();
        //    return;

        //    #region 权限认证

        //    //if ((action.ToLower() == "homepage" && controller.ToLower() == "home") || controller.ToLower() == "account" || (action.ToLower() == "getalllistasync" && controller.ToLower() == "website") || (action.ToLower() == "getalllistasync" && controller.ToLower() == "software"))
        //    //{
        //    //    return;
        //    //}

        //    #endregion


        //    string userId = context.HttpContext.Session.GetString("LoginUserId");
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        if (IsAjaxRequest(context.HttpContext.Request))
        //        {
        //            //是ajax请求
        //            context.Result = new JsonResult(new { status = "error", message = "你没有登录" });
        //        }
        //        else
        //        {
        //            var result = new RedirectResult("~/Account/Login");
        //            context.Result = result;
        //        }
        //        return;
        //    }
        //    //判断权限


        //}

        ///// <summary>
        ///// 判断是否是ajax请求
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //private bool IsAjaxRequest(HttpRequest request)
        //{
        //    string header = request.Headers["X-Requested-With"];
        //    return "XMLHttpRequest".Equals(header);
        //}
        #endregion
    }
}
