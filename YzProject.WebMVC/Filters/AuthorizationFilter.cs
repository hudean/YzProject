using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace YzProject.WebMVC.Filters
{
    /// <summary>
    /// 授权过滤器
    /// </summary>
    public class AuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var endpoint = context?.HttpContext?.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                return;
            }

            var userName = context.HttpContext.Session.GetString("LoginUserName");
            if (string.IsNullOrEmpty(userName))
            {
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    //是ajax请求
                    context.Result = new JsonResult(new { status = "error", message = "你没有登录" });
                }
                else
                {
                    var result = new RedirectResult("~/Account/Login");
                    context.Result = result;
                }
                return;
            }

            #region 严格的权限验证


            #endregion
        }

        /// <summary>
        /// 判断是否是ajax请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }

   
}
