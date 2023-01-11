using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using YzProject.Domain.RequestModel;
using YzProject.Services.Contract;
using System.Collections.Generic;
using YzProject.Domain.ResultModel;
using Microsoft.AspNetCore.Authorization;

namespace YzProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService,
            IWebHostEnvironment hostingEnvironment,
          IConfiguration configuration)
        {
            _userService = userService;
        }


        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaginatedListAsync([FromQuery] ParamQueryPageUser param)
        {
            var list = await _userService.GetPaginatedListAsync(param.UserName, param.PageIndex, param.PageSize);
            ResultData resultData = new(1, "", list);
            return Ok(resultData);
        }

        /// <summary>
        /// 添加或修改
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("addOrEdit")]
        public async Task<IActionResult> AddOrEditAsync(ParamUser param)
        {
            ResultData resultData = new();
            resultData.Code = 0;
            try
            {
                if (await _userService.ExistAsync(param))
                {
                    resultData.Message = "新增失败，原因：已存在该用户";
                    return Ok(resultData);
                }
                if (param.ConfirmPassword != param.Password)
                {
                    resultData.Message = "新增失败，两次密码不一致";
                    return Ok(resultData);
                }
                //if (param.Id > 0)
                //{
                //    HttpContext.Session.SetString("UserId", param.Id.ToString());
                //    //可以用 automap强转
                //    var user = await _userService.GetByIdAsync(param.Id);
                //    if (user != null)
                //    {
                //        user.UserName = param.UserName;
                //        return Json(_userService.BgOk(await _userService.UpdateAsync(user)));
                //    }
                //    return Json(_userService.BgError("失败，该用户不存在"));
                //}

                //var model = await _userService.InsertGetEntityAsync(param);
                //HttpContext.Session.SetString("UserId", model.Id.ToString());
                resultData.Code = 1;
                resultData.Message = "添加成功";
                return Ok(resultData);
            }
            catch (Exception ex)
            {
                resultData.Message = "新增失败，原因：" + ex.ToString();
                return Ok(resultData);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            ResultData resultData = new();
            try
            {
                // 不使用真删除、使用软删除
                bool isSuccess = await _userService.DeleteAsync(id);

                #region 真删除，删除后删除头像图片和头像缩略图

                //var model = await _userService.DetailAsync(id);
                //if (await _userService.DeleteAsync(id))
                //{
                //    if (!string.IsNullOrEmpty(model.HeadImgUrl))
                //    {
                //        var fileName = model.HeadImgUrl.Substring(model.HeadImgUrl.LastIndexOf("/") + 1);
                //        string filePath = _hostingEnv.WebRootPath + "/images/User/";
                //        fileName = filePath + fileName;
                //        //删除原图片
                //        if (System.IO.File.Exists(fileName))
                //        {
                //            System.IO.File.Delete(fileName);
                //        }
                //    }
                //    if (!string.IsNullOrEmpty(model.ThumbnailHeadImg))
                //    {
                //        var fileName = model.ThumbnailHeadImg.Substring(model.ThumbnailHeadImg.LastIndexOf("/") + 1);
                //        string filePath = _hostingEnv.WebRootPath + "/images/User/";
                //        fileName = filePath + fileName;
                //        //删除原图片
                //        if (System.IO.File.Exists(fileName))
                //        {
                //            System.IO.File.Delete(fileName);
                //        }
                //    }
                //}

                #endregion

                if (isSuccess)
                {
                    resultData.Code = 1;
                    resultData.Message = "删除成功";
                }
                else
                {
                    resultData.Code = 0;
                    resultData.Message = "删除失败";
                }
                return Ok(resultData);
            }
            catch (Exception ex)
            {
                resultData.Code = 0;
                resultData.Message = "删除失败，" + ex.ToString();
                return Ok(resultData);
            }
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("editPassword")]
        public async Task<IActionResult> EditPasswordAsync(ParamUserPassword param)
        {
            ResultData resultData = new();
            try
            {
                if (param.ConfirmPassword != param.Password)
                {
                    resultData.Code = 0;
                    resultData.Message = "失败，两次密码不一致";
                    return Ok(resultData);
                }
                bool isSuccess = await _userService.EditPassWordAsync(param);
                if (isSuccess)
                {
                    resultData.Code = 1;
                    resultData.Message = "修改密码成功";
                }
                else
                {
                    resultData.Code = 0;
                    resultData.Message = "修改密码失败";
                }
            }
            catch (Exception ex)
            {
                resultData.Code = 0;
                resultData.Message = "修改密码失败，" + ex.ToString();
            }
            return Ok(resultData);
        }

    }
}
