using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using YzGraduationProject.Common;
using YzProject.Domain.RequestModel;
using YzProject.Redis;
using YzProject.Services.Contract;

namespace YzProject.WebAPI.Controllers
{
    //[ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRedisCacheRepository _redisCacheRepository;
        public AccountController(IUserService userService,
            IRedisCacheRepository redisCacheRepository)
        {
            _userService = userService;
            _redisCacheRepository = redisCacheRepository;
        }

        #region jwt
        // 文章：https://www.jianshu.com/p/e4d9dcda0558
        //https://codewithmukesh.com/blog/aspnet-core-api-with-jwt-authentication/（推荐）
        // https://codewithmukesh.com/blog/refresh-tokens-in-aspnet-core/（推荐）
        //https://www.cnblogs.com/xwc1996/p/14058115.html
        //https://blog.csdn.net/liangmengbk/article/details/121733491
        //https://blog.csdn.net/u010796249/article/details/109487453
        //https://www.jianshu.com/p/f01cfab68c7b（推荐）
        //https://www.cnblogs.com/danvic712/p/10331976.html
        //优点：
        //安全性高，防止token被伪造和篡改
        //自包含，减少存储开销
        //跨语言，支持多种语言实现
        //支持过期，发布者等校验

        //缺点：
        //JWT不适用存放大量信息，会造成token过长
        //无法作废未过期的JWT，所以需要搭配Redis使用，达到用户登出操作token即失效的要求。
        /// <summary>
        /// 生成 JWT 令牌
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync(TokenRequestModel model)
        {
            var result = await _userService.GetTokenAsync(model);
            SetRefreshTokenInCookie(result.RefreshToken);
            #region 把访问令牌存起来用来校验其权限和同一个用户只保留一个有效的token
            // 用户名称 tokeen
            await _redisCacheRepository.HashSetAsync(CachingKeys.AccessTokenHashKey, Encrypt.Md5Encrypt(model.UserName), result.Token);
            #endregion
            return Ok(result);
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            //从我们的 cookie 中获取刷新令牌。
            var refreshToken = Request.Cookies["refreshToken"];
            //从服务方法返回响应对象。
            var response = await _userService.RefreshTokenAsync(refreshToken);
            //将新的刷新令牌设置为我们的 Cookie。
            if (!string.IsNullOrEmpty(response.RefreshToken))
            {
                SetRefreshTokenInCookie(response.RefreshToken);
                #region 把访问令牌存起来用来校验其权限和同一个用户只保留一个有效的token
                // 用户名称 tokeen
                await _redisCacheRepository.HashSetAsync(CachingKeys.AccessTokenHashKey, Encrypt.Md5Encrypt(response.UserName), response.Token);
                #endregion
            }
            
            return Ok(response);
        }

        /// <summary>
        /// 将 refreshTokens 保存为 cookie
        /// </summary>
        /// <param name="refreshToken"></param>
        private void SetRefreshTokenInCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(10),
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        /// <summary>
        /// 撤销令牌
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeTokenAsync([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });
            var response = await _userService.RevokeTokenAsync(token);
            if (!response)
                return NotFound(new { message = "Token not found" });
            return Ok(new { message = "Token revoked" });
        }

        #endregion

    }
}
