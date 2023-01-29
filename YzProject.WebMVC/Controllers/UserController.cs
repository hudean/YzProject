using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using YzProject.Domain.RequestModel;
using YzProject.Services.Contract;

namespace YzProject.WebMVC.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    public class UserController : Controller
    {

        public readonly IUserService _userService;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="userService"></param>
        public UserController(IUserService userService,
              IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration)
        {
            _userService = userService;
            _hostingEnv = hostingEnvironment;
            _configuration = configuration; 
        }

        public IActionResult Index()
        {
            return View();
        }

        #region 分页列表、添加/修改、删除、批量删除

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetPaginatedListAsync(ParamQueryPageUser param)
        {
            return Json(_userService.BgOk(await _userService.GetPaginatedListAsync(param.UserName, param.PageIndex, param.PageSize)));
        }

        /// <summary>
        /// 添加或修改
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddOrEditAsync(ParamUser param)
       {
            try
            {
                if (await _userService.ExistAsync(param))
                {
                    return Json(_userService.BgError("新增失败，原因：已存在该用户"));
                }
                if (param.ConfirmPassword != param.Password)
                {
                    return Json(_userService.BgError("失败，两次密码不一致"));
                }
                //if (param.Id > 0)
                //{
                //    HttpContext.Session.SetString("AddUserId", param.Id.ToString());
                //    //可以用 automap强转
                //    var user = await _userService.GetByIdAsync(param.Id);
                //    if (user != null)
                //    {
                //        user.UserName = param.UserName;
                //        return Json(_userService.BgOk(await _userService.UpdateAsync(user)));
                //    }
                //    return Json(_userService.BgError("失败，该用户不存在"));
                //}

                var model = await _userService.InsertOrUpdateAsync(param);
                HttpContext.Session.SetString("AddUserId", model.Id.ToString());
                return Json(_userService.BgOk("添加成功"));
            }
            catch (Exception ex)
            {
                return Json(_userService.BgError("新增失败，原因：" + ex.ToString()));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteAsync(int id)
        {
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

                return Json(_userService.BgOk("删除成功"));
            }
            catch (Exception ex)
            {
                return Json(_userService.BgError("删除失败，" + ex.ToString()));
            }
        }

        #endregion

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditPassWordAsync(ParamUserPassword param)
        {
            try
            {
                if (param.ConfirmPassword != param.Password)
                {
                    return Json(_userService.BgError("失败，两次密码不一致"));
                }
                return Json(_userService.BgOk(await _userService.EditPassWordAsync(param)));
            }
            catch (Exception ex)
            {
                return Json(_userService.BgError("新增失败，原因：" + ex.ToString()));
            }
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="action"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<IActionResult> UploadImgAsync(string action, IFormFile file)
        {
            try
            {
                string fileName = file.FileName.Trim().Substring(0, file.FileName.Length - Path.GetExtension(file.FileName).Length);
                //新的文件名
                string newFileName = fileName + DateTime.Now.ToString("_yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + Path.GetExtension(file.FileName);
                //文件上传路径
                string filePath = _hostingEnv.WebRootPath + "/images/User/";
                //图片网路地址
                string fileUrl = _configuration["ImgDomain"] + "/User/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var model = await _userService.GetUserAsync(int.Parse(HttpContext.Session.GetString("AddUserId")));

                if (action == "edit")
                {
                    if (!string.IsNullOrEmpty(model.HardImgUrl))
                    {
                        var oldFileName = model.HardImgUrl.Substring(model.HardImgUrl.LastIndexOf("/") + 1);
                        oldFileName = filePath + oldFileName;
                        //删除原图片
                        if (System.IO.File.Exists(oldFileName))
                        {
                            System.IO.File.Delete(oldFileName);
                        }
                    }
                }

                filePath = filePath + $@"{newFileName}";//指定文件上传路径
                fileUrl = fileUrl + $@"{newFileName}";//指定文件上传路径
                model.HardImgUrl = fileUrl;
                if (await _userService.EditUserAsync(model))
                {
                    using (FileStream fs = System.IO.File.Create(filePath))//创建文件流
                    {
                        file.CopyTo(fs);//将上载文件的内容复制到目标流
                        fs.Flush();//清除此流的缓冲区并导致将任何缓冲数据写入
                    }
                }
                return Json(_userService.BgOk("上传成功"));
            }
            catch (Exception ex)
            {
                return Json(_userService.BgError("文件上传失败，" + ex.ToString()));
            }

        }

    }
}
