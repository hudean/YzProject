using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System;

namespace YzProject.WebAPI.Middlewares
{
    /// <summary>
    /// 从请求标头中读取 Accept-Language 键，并在文化有效时设置当前线程的语言。
    /// </summary>
    public class LocalizationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //这里我们从当前 HTTP 上下文的请求标头中读取 Accept-Language。
            var cultureKey = context.Request.Headers["Accept-Language"];
            if (!string.IsNullOrEmpty(cultureKey))
            {
                //如果找到有效的文化，我们设置当前线程文化。
                if (DoesCultureExist(cultureKey))
                {
                    var culture = new System.Globalization.CultureInfo(cultureKey);
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
            }
            await next(context);
        }
        private static bool DoesCultureExist(string cultureName)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).Any(culture => string.Equals(culture.Name, cultureName,
    StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
