using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YzProject.Model;
using YzProject.Services.Contract;

namespace YzProject.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        public AuthenticateService(IOptions<JwtTokenConfig> jwtTokenConfig)
        {
            _jwtTokenConfig = jwtTokenConfig.Value;
        }

        /// <summary>
        /// 生成token（令牌）
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<string> GenerateJwtToken(string userName)
        {
            await Task.CompletedTask;
            bool isDefault = true;
            if (isDefault)
            {
                string token = string.Empty;
                var claims = new Claim[] { new Claim(ClaimTypes.Name, userName) };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var jwtToken = new JwtSecurityToken(_jwtTokenConfig.Issuer, _jwtTokenConfig.Audience, claims, expires: DateTime.Now.AddMinutes(_jwtTokenConfig.AccessExpiration), signingCredentials: credentials);

                token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                return token;
            }
            else
            {
                // 文章地址 https://zhuanlan.zhihu.com/p/364445582
                //现在，是时候定义 jwt token 了，它将负责创建我们的 tokens
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                // 从 appsettings 中获得我们的 secret 
                var key = Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret);
                // 定义我们的 token descriptor
                // 我们需要使用 claims （token 中的属性）给出关于 token 的信息，它们属于特定的用户，
                // 因此，可以包含用户的 Id、名字、邮箱等。
                // 好消息是，这些信息由我们的服务器和 Identity framework 生成，它们是有效且可信的。
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                            //new Claim("Id", id),
                            new Claim(ClaimTypes.Name,userName),
                            // new Claim(JwtRegisteredClaimNames.UniqueName, email),
                            //new Claim(JwtRegisteredClaimNames.Email, email),
                            new Claim(JwtRegisteredClaimNames.Sub, userName),
                            // Jti 用于刷新 token，我们将在下一篇中讲到
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    }),
                    // token 的过期时间需要缩短，并利用 refresh token 来保持用户的登录状态，
                    // 不过由于这只是一个演示应用，我们可以对其进行延长以适应我们当前的需求
                    Expires = DateTime.UtcNow.AddHours(6),
                    // 这里我们添加了加密算法信息，用于加密我们的 token
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = jwtTokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = jwtTokenHandler.WriteToken(token);
                return jwtToken;
            }

        }


    }
}
