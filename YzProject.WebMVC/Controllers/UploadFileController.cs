using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace YzProject.WebMVC.Controllers
{

    public class UploadFileController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IConfiguration _configuration;

        public UploadFileController(
             IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration)
        {
            _hostingEnv = hostingEnvironment;
            _configuration = configuration;
        }

        public IActionResult UploadEditorImg(string action)
        {
            try
            {
                var files = Request.Form.Files;
                var file = files[0];
                string fileName = file.FileName.Trim().Substring(0, file.FileName.Length - Path.GetExtension(file.FileName).Length);
                //新的文件名
                string newFileName = fileName + DateTime.Now.ToString("_yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + Path.GetExtension(file.FileName);
                //文件上传路径
                string filePath = _hostingEnv.WebRootPath + "/images/Editor/";
                //图片网路地址
                string fileUrl = _configuration["ImgDomain"] + "/Editor/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                filePath = filePath + $@"{newFileName}";//指定文件上传路径
                fileUrl = fileUrl + $@"{newFileName}";//指定文件上传路径
                using (FileStream fs = System.IO.File.Create(filePath))//创建文件流
                {
                    file.CopyTo(fs);//将上载文件的内容复制到目标流
                    fs.Flush();//清除此流的缓冲区并导致将任何缓冲数据写入
                }

                var result = new
                {
                    error = 0,
                    url = fileUrl
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    error = 1,
                    url = ""
                };
                return Json(result);
            }
        }



        //public async Task<IActionResult> UploadFileAsync(IFormFile file)
        //{
        //    await Task.CompletedTask;
        //    // 最多 1G
        //    if (file != null && file.Length > 0)
        //    {

        //        try
        //        {
        //            //获取扩展名
        //            string fileExtension = Path.GetExtension(file.FileName)?.ToLower();


        //            long fileLength = file.Length;
        //            if (fileLength / 1024 > 1 * 1024 * 1024)
        //            {
        //                //return _articleService.Error((int)HttpStatusCode.InternalServerError, "添加失败,视频超过2G");
        //            }

        //            string fileName = file.FileName.Trim().Substring(0, file.FileName.Length - Path.GetExtension(file.FileName).Length);
        //            //新的文件名
        //            string newFileName = fileName + DateTime.Now.ToString("_yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + Path.GetExtension(file.FileName);
        //            //文件上传路径
        //            string filePath = _hostingEnv.WebRootPath + "/Files/";
        //            //string webRootPath = string.IsNullOrEmpty(_configuration["UploadFilePath"]) ? _hostingEnv.WebRootPath : _configuration["UploadFilePath"];
        //            //string filePath = webRootPath + "/File/";
        //            //图片网路地址
        //            string fileUrl = _configuration["ServiceUrl"] + "/Files/";

        //            if (!Directory.Exists(filePath))
        //            {
        //                Directory.CreateDirectory(filePath);
        //            }
        //            filePath = filePath + $@"{newFileName}";//指定文件上传路径
        //            string uploadFileUrl = fileUrl + $@"{newFileName}";//指定文件上传路径
        //            using (FileStream fs = System.IO.File.Create(filePath))//创建文件流
        //            {
        //                file.CopyTo(fs);//将上载文件的内容复制到目标流
        //                fs.Flush();//清除此流的缓冲区并导致将任何缓冲数据写入
        //            }

        //            //return _articleService.Ok(new { imgUrl = uploadFileUrl });
        //            var result = new
        //            {
        //                error = 0,
        //                url = uploadFileUrl
        //            };

        //            return Json(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            var result = new
        //            {
        //                error = 1,
        //                url = ""
        //            };
        //            return Json(result);
        //        }


        //    }
        //    else
        //    {
        //        var result = new
        //        {
        //            error = 1,
        //            url = ""
        //        };
        //        return Json(result);
        //    }



        //}



    }
}
