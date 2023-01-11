using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzGraduationProject.Common;
using YzProject.Domain;
using YzProject.Domain.Entites;
using YzProject.Domain.RequestModel;
using YzProject.Domain.ResultModel;
using YzProject.Domain.Repositories;
using YzProject.Repository.Contract;
using YzProject.Domain.Configs;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace YzProject.Repository
{
    public class UserRepository : Repository<User, int>, IUserRepository
    {
        private readonly JwtSetting _jwt;
        public UserRepository(YzProjectContext context, IOptions<JwtSetting> jwt) : base(context)
        {
            _jwt = jwt.Value;
        }

        //public async Task<bool> AddUserAsync(string userName)
        //{
        //    User user = new User()
        //    {
        //        UserName = userName,
        //    };
        //    await _dbContext.Users.AddAsync(user);
        //    int res = await _dbContext.SaveChangesAsync();
        //    return res > 0;
        //}


        public async Task<bool> DeleteManyAsync(List<int> ids)
        {
            var entites = await _dbContext.Users.Where(x => ids.Contains(x.Id)).ToListAsync();
            _dbContext.Users.RemoveRange(entites);
            int res = await _dbContext.SaveChangesAsync();
            return res > 0;
        }

        public async Task<UserViewModel> DetailAsync(int id)
        {
            var data = await (from u in _dbContext.Users.Where(x => x.Id == id)
                              join d in _dbContext.Departments on u.DeptmentId equals d.Id
                              select new UserViewModel
                              {
                                  Id = u.Id,
                                  UserName = u.UserName,
                                  CreateTime = u.CreateTime,
                                  CreateUserId = u.CreateUserId,
                                  DeptmentId = u.DeptmentId,
                                  DeptmentName = d.DepartmentName,
                                  EMail = u.EMail,
                                  Mobile = u.Mobile,
                                  Name = u.Name,
                                  Introduction = u.Introduction,
                                  Address = u.Address,
                                  Birthday = u.Birthday,
                                  HeadImgUrl = u.HeadImgUrl,
                                  ThumbnailHeadImg = u.ThumbnailHeadImgUrl,

                              }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<UserViewModel>> GetAllListAsync()
        {
            var query = from r in _dbContext.Departments
                        orderby r.Id
                        select new UserViewModel
                        {
                            //Id = r.Id,
                            //CreateTime = r.CreateTime,
                            //CreateUserId = r.CreateUserId,
                            //ContactNumber = r.ContactNumber,
                            //DepartmentCode = r.DepartmentCode,
                            //DepartmentManager = r.DepartmentManager,
                            //DepartmentName = r.DepartmentName,
                            //ParentId = r.ParentId,
                            //Remarks = r.Remarks,
                        };
            return await query.ToListAsync();
        }

        public async Task<PaginatedList<UserViewModel>> GetPaginatedListAsync(string userName, int pageIndex, int pageSize)
        {
            var query = from r in _dbContext.Users.WhereIf(!string.IsNullOrEmpty(userName), r => r.UserName.Contains(userName))
                        orderby r.CreateTime descending
                        select new UserViewModel()
                        {
                            Id = r.Id,
                            CreateTime = r.CreateTime,
                            CreateUserId = r.CreateUserId,
                            HeadImgUrl =r.HeadImgUrl,
                             Address=r.Address,
                              Birthday = r.Birthday,
                               DeptmentId = r.DeptmentId,
                                UserName = r.UserName,
                            //ContactNumber = r.ContactNumber,
                            //DepartmentCode = r.DepartmentCode,
                            //DepartmentManager = r.DepartmentManager,
                            //DepartmentName = r.DepartmentName,
                            //ParentId = r.ParentId,
                            //Remarks = r.Remarks,
                        };
            int count = await query.CountAsync();
            var list = new List<UserViewModel>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<UserViewModel>(list, count, pageIndex, pageSize);
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            var user = await _dbContext.Users.Where(x => x.UserName == userName).FirstOrDefaultAsync();
            return user;
        }

        public async Task<bool> InsertOrUpdateAsync(ParamUser param)
        {
            if (param.Id > 0)
            {
                var model = await _dbContext.Users.Where(r => r.Id == param.Id).FirstOrDefaultAsync();
                if (model == null)
                {
                    return false;
                }
                //department.ParentId = param.ParentId;
                //department.DepartmentCode = param.DepartmentCode;
                //department.DepartmentName = param.DepartmentName;
                //department.DepartmentManager = param.DepartmentManager;
                //department.CreateUserId = param.CreateUserId;
                //department.ContactNumber = param.ContactNumber;
                //department.Remarks = param.Remarks;
                _dbContext.Users.Update(model);
            }
            else
            {
                var model = new User()
                {
                    //ParentId = param.ParentId,
                    //DepartmentCode = param.DepartmentCode,
                    //DepartmentName = param.DepartmentName,
                    //DepartmentManager = param.DepartmentManager,
                    //CreateUserId = param.CreateUserId,
                    //ContactNumber = param.ContactNumber,
                    //CreateTime = DateTime.Now,
                    //IsDeleted = false,
                    //Remarks = param.Remarks,
                };
                await _dbContext.Users.AddAsync(model);
            }
            int res = await _dbContext.SaveChangesAsync();
            return res > 0;
        }


        #region jwt

        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model)
        {
            //创建一个新的响应对象
            var authenticationModel = new AuthenticationModel();
            //检查用户名密码是否有效，否则返回一条消息，说明凭据不正确。
            var user = await _dbContext.Users.FirstOrDefaultAsync(r => r.UserName == model.UserName);

            if (user != null && user.Password.Equals(Encrypt.Md5Encrypt(model.Password)))
            {
                authenticationModel.IsAuthenticated = true;
                //调用 CreateJWTToken 函数。
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
                authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authenticationModel.UserName = model.UserName;
                //var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                //authenticationModel.Roles = rolesList.ToList();

                #region
                var refreshTokens = await _dbContext.RefreshTokens.Where(r => r.UserId == user.Id).ToListAsync();
                //检查是否有任何活动的刷新令牌可供经过身份验证的用户使用
                if (refreshTokens.Any(a => a.IsActive))
                {
                    //将可用的活动刷新令牌设置为我们的响应
                    var activeRefreshToken = refreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
                    authenticationModel.RefreshToken = activeRefreshToken.Token;
                    authenticationModel.RefreshTokenExpiration = activeRefreshToken.Expires;
                }
                else
                {
                    //如果没有可用的活动刷新令牌，我们调用 CreateRefreshToken 方法来生成刷新令牌。生成后，我们将刷新令牌的详细信息设置为响应对象。
                    //最后，我们需要将这些令牌添加到我们的 RefreshTokens 表中，以便我们可以重用它们。
                    var refreshToken = CreateRefreshToken(user.Id);
                    authenticationModel.RefreshToken = refreshToken.Token;
                    authenticationModel.RefreshTokenExpiration = refreshToken.Expires;
                    _dbContext.RefreshTokens.Add(refreshToken);
                    int res = await _dbContext.SaveChangesAsync();
                }

                #endregion

                return authenticationModel;//返回响应对象
            }
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Incorrect Credentials for user {user?.UserName}.";
            return authenticationModel;
        }

        /// <summary>
        /// 构建 JWT
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            await Task.CompletedTask;
            //获取用户的所有声明（用户详细信息）
            //var userClaims = await _userManager.GetClaimsAsync(user);
            //获取用户的所有角色
            //var roles = await _userManager.GetRolesAsync(user);
            //var roleClaims = new List<Claim>();
            //for (int i = 0; i < roles.Count; i++)
            //{
            //    roleClaims.Add(new Claim("roles", roles[i]));
            //}
            var claims = new[]
            {
                // new Claim(ClaimTypes.Name,user.UserName)
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id.ToString()),
                new Claim("accessUserName",Encrypt.Md5Encrypt(user.UserName))
            };
            //.Union(userClaims)
            //.Union(roleClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            //创建一个新的 JWT 安全令牌并返回它们
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<AuthenticationModel> RefreshTokenAsync(string token)
        {
            //创建一个新的 Response 对象
            var authenticationModel = new AuthenticationModel();
            //检查我们的数据库中是否有任何与令牌匹配的用户。
            //var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            //获取匹配记录的刷新令牌对象。
            //var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            var refreshToken = await _dbContext.RefreshTokens.SingleAsync(x => x.Token == token);
            //检查所选令牌是否处于活动状态，如果未处于活动状态，则发送消息“令牌未处于活动状态”。
            if (refreshToken == null || !refreshToken.IsActive)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"Token Not Active.";
                return authenticationModel;
            }
            var user = await _dbContext.Users.FindAsync(refreshToken.UserId);
            //如果没有找到匹配的用户，则传递一条消息“令牌不匹配任何用户”
            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"Token did not match any users.";
                return authenticationModel;
            }
            //出于安全原因，我们只能使用一次刷新令牌。因此，每次我们请求新的 JWT 时，我们都必须确保将刷新令牌替换为新令牌。
            //让我们将 Revked 属性设置为当前时间。这会使刷新令牌无效。
            refreshToken.Revoked = DateTime.UtcNow;
            //生成一个新的 Refresh 令牌并将其更新到我们的数据库中。
            var newRefreshToken = CreateRefreshToken(user.Id);
            _dbContext.RefreshTokens.Update(refreshToken);
            await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _dbContext.SaveChangesAsync();
            //让我们为相应的用户生成另一个 JWT，并返回响应对象以及新的刷新令牌。
            //Generates new jwt 生成新的jwt
            authenticationModel.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
            authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            //authenticationModel.Email = user.Email;
            authenticationModel.UserName = user.UserName;
            //var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            //authenticationModel.Roles = rolesList.ToList();
            authenticationModel.RefreshToken = newRefreshToken.Token;
            authenticationModel.RefreshTokenExpiration = newRefreshToken.Expires;
            return authenticationModel;
        }

        /// <summary>
        /// 撤销令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> RevokeTokenAsync(string token)
        {
            var refreshToken = await _dbContext.RefreshTokens.SingleAsync(x => x.Token == token);
            if (refreshToken == null || !refreshToken.IsActive) return false;
            refreshToken.Revoked = DateTime.UtcNow;
            _dbContext.Update(refreshToken);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        /// <summary>
        /// 创建刷新令牌
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private RefreshToken CreateRefreshToken(int userId)
        {
            var randomNumber = new byte[32];

            //using (var generator = new RNGCryptoServiceProvider())//已过时使用下面的
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                return new RefreshToken
                {
                    UserId = userId,
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddDays(10),
                    Created = DateTime.UtcNow
                };
            }
        }

        #endregion
    }
}
