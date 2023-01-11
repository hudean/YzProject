using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using YzProject.Domain.ResultModel;
using YzProject.Redis;

namespace YzProject.WebAPI.Filters
{
    /// <summary>
    /// 授权过滤器
    /// </summary>
    public class AuthorizationFilter : IAuthorizationFilter//AuthorizeAttribute,//Attribute,
    {

        private readonly IRedisCacheRepository _redisCacheRepository;

        public AuthorizationFilter(IRedisCacheRepository redisCacheRepository)
        {
            _redisCacheRepository = redisCacheRepository;
        }

        /// <summary>
        /// 验证授权
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            #region token + 签名校验
            //匿名请求直接返回，不需要校验签名
            //if (!context.HttpContext.User.Identity.IsAuthenticated || context.Filters.Any(item => item is IAllowAnonymousFilter))
            //{
            //    return;
            //}

            //try
            //{
            //    string token = context.HttpContext.Request.Headers["token"];//用户token值
            //    string nonce = context.HttpContext.Request.Headers["nonce"];//随机字符串
            //    string timestamp = context.HttpContext.Request.Headers["timestamp"];//时间戳
            //    string sign = context.HttpContext.Request.Headers["sign"];//签名

            //    //随机字符串校验
            //    string nonceCache;
            //    var bol = _cache.TryGetValue<string>(nonce, out nonceCache);
            //    //判断缓存是否存在
            //    if (!bol)
            //    {
            //        var cacheEntryOptions = new MemoryCacheEntryOptions()
            //            .SetSlidingExpiration(TimeSpan.FromMinutes(10));
            //        _cache.Set(nonce, nonce, cacheEntryOptions);
            //    }
            //    else
            //    {
            //        context.Result = new JsonResult(_resultService.Error((int)HttpStatusCode.Unauthorized, "请求重复"));
            //        return;
            //    }

            //    //时间戳处理，有效期 60s
            //    double ts1 = Double.Parse(timestamp);
            //    double ts2 = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            //    if (ts2 - ts1 > 600 * 1000)
            //    {
            //        context.Result = new JsonResult(_resultService.Error((int)HttpStatusCode.Unauthorized, "非法调用"));
            //        return;
            //    }

            //    //签名校验
            //    HttpRequest request = context.HttpContext.Request;
            //    string data = "";
            //    if (request.Method.ToLower().Equals("post"))
            //    {
            //        Stream stream = request.Body;
            //        byte[] buffer = new byte[request.ContentLength.Value];
            //        stream.Read(buffer, 0, buffer.Length);
            //        data = Encoding.UTF8.GetString(buffer);
            //    }
            //    else if (request.Method.ToLower().Equals("get"))
            //    {
            //        data = request.QueryString.Value;
            //    }

            //    //Stream stream = context.HttpContext.Request.Body;
            //    //byte[] buffer = new byte[context.HttpContext.Request.ContentLength.Value];
            //    //stream.ReadAsync(buffer, 0, buffer.Length);
            //    //string data = Encoding.UTF8.GetString(buffer);
            //    string resign = GetSignature(token, nonce, timestamp, data);
            //    if (sign != resign)
            //    {
            //        context.Result = new JsonResult(_resultService.Error((int)HttpStatusCode.Unauthorized, "非法调用"));
            //        return;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex.ToString());
            //    context.Result = new JsonResult(_resultService.Error((int)HttpStatusCode.Unauthorized, "非法调用"));
            //}

            #endregion

            //默认使用jwt token校验
            //return;
            #region 权限认证

            ////参考文章 https://www.cnblogs.com/yaopengfei/p/11232921.html
            //https://www.cnblogs.com/RainingNight/p/authorization-in-asp-net-core.html
            //权限过滤的两种思路 认证过滤器和方法过滤器都可以
            //一：在每个控制器和action上添加相应的特性标签，把标签名称存到数据库权限表中，获取当前请求控制器下的action的对应的特性标签 与数据库存储的进行对比，相同就是有权限
            //二：把每个控制器和action以 ControllerName_ActionName 的形式存在数据库权限表中，获取当前访问的控制器名称和Action 名称与当前用户的所在角色下的权限进行比对 ，相同就是有权限

            //匿名标识 无需验证
            //if (context.Filters.Any(e => (e as IAllowAnonymous) != null))
            //{
            //    return;
            //}

            var endpoint = context?.HttpContext?.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                return;
            }

            //不是控制器里的方法 无需验证
            if (!context.ActionDescriptor.IsControllerAction())
            {
                return;
            }

            return;

            #region 只允许账号唯一token登录(需要时取消注释)

            //string userName = context.HttpContext.Request.Headers["UserName"];
            //var accessClaim = context.HttpContext.User.Identities.FirstOrDefault().Claims.FirstOrDefault(r => r.Type == "accessUserName");
            string accessClaimValue = context.HttpContext.User.FindFirstValue("accessUserName");
            string accessToken = context.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(accessClaimValue) || string.IsNullOrEmpty(accessToken))
            {
                context.Result = new OkObjectResult(new ResultData(0, "用户未登录", null));
                return;
            }
            string[] strs = accessToken.Split(' ');
            //判断token是否在缓存上
            var token = _redisCacheRepository.HashGet<string>(CachingKeys.AccessTokenHashKey, accessClaimValue);
            if (strs.Length != 2 || string.IsNullOrEmpty(token) || token != strs[1])
            {
                context.Result = new OkObjectResult(new ResultData(0, "用户未登录", null));
                return;
            }
            //if (!_redisCacheRepository.HashExists(CachingKeys.AccessTokenHashKey, accessClaim.Value))
            //{
            //    context.Result = new OkObjectResult(new ResultData(0, "用户未登录", null));
            //    return;
            //}
            #endregion

            #region 获取访问目标对象上的特性

            ////所有目标对象上所有特性Attribute
            ////var attrs = context.ActionDescriptor.EndpointMetadata.ToList();
            ////获取所有目标对象上所有特性CheckPermissionAttribute
            //var attrs = context.ActionDescriptor.EndpointMetadata.ToList().Where(r => r as CheckPermissionAttribute != null).ToList();
            ////获取CheckPermissionAttribute最下面的一个
            ////var attr = endpoint?.Metadata.GetMetadata<CheckPermissionAttribute>();
            ////获取过滤器特性
            ////var pAttr = context.Filters.Where(r => r as VaCantAuthorizationFilter != null).ToList();


            #endregion

            #region 获取当前访问的区域、控制器和action

            //1. 获取区域、控制器、Action的名称
            //必须在区域里的控制器上加个特性[Area("")]才能获取
            //string areaName =  context.ActionDescriptor.RouteValues["area"]?.ToString();
            //string controllerName =  context.ActionDescriptor.RouteValues["controller"]?.ToString();
            //string actionName =  context.ActionDescriptor.RouteValues["action"]?.ToString();

            //获取请求的区域，控制器，action名称
            var area = context.RouteData.DataTokens["area"]?.ToString();
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();
            #endregion

            #region 当前登入的非admin用户的权限检查
            ////获取当前用户对应角色的所有权限
            //var permissionNames = CacheCommon.GetCacheByPermissionNames();
            //if (permissionNames != null && permissionNames.Count > 0)
            //{
            //    foreach (var item in attrs)
            //    {
            //        if (!permissionNames.Contains(item.ToString()))
            //        {
            //            if (IsAjaxRequest(context.HttpContext.Request))
            //            {
            //                //是ajax请求
            //                context.Result = new JsonResult(new { status = "error", message = "你没有权限" });
            //            }
            //            else
            //            {
            //                var result = new RedirectResult("~/Home/Index");
            //                context.Result = result;
            //            }
            //            return;
            //        }
            //    }
            //}
            //else
            //{
            //    if (IsAjaxRequest(context.HttpContext.Request))
            //    {
            //        //是ajax请求
            //        context.Result = new JsonResult(new { status = "error", message = "你没有任何权限" });
            //    }
            //    else
            //    {
            //        var result = new RedirectResult("~/Home/Index");
            //        context.Result = result;
            //    }
            //    return;
            //}

            #endregion

            #endregion
        }
    }

    public static class ActionDescriptorExtension
    {
        public static bool IsControllerAction(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor is ControllerActionDescriptor;
        }
    }
}
