using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using YzGraduationProject.Common;
using YzProject.Domain.RequestModel;
using YzProject.Services.Contract;

namespace YzProject.WebMVC.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            var userName = HttpContext.Session.GetString("LoginUserName");
            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(ParamLogin model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(_userService.BgError("输入格式不正确！"));
                }
                string validateCode = (string)TempData["validateCode"];
                if (!model.ValidateCode.Equals(validateCode))
                {
                    return Json(_userService.BgError("验证码输入错误！"));
                }

                var user = await _userService.GetUserByUserNameAsync(model.UserName);

                if (user != null)
                {
                    if (user.Password == Encrypt.Md5Encrypt(model.Password))
                    {
                        HttpContext.Session.SetString("LoginUserId", user.Id.ToString());
                        HttpContext.Session.SetString("LoginUserName", user.UserName);
                        //return RedirectToAction("Index", "Home");
                        return Json(_userService.BgOk(user, "登录成功", "/Home/Index"));
                    }
                }
                return Json(_userService.BgError("用户不存在或密码错误！"));


            }
            catch (Exception ex)
            {
                return Json(_userService.BgError("登录失败,原因：" + ex.ToString()));
            }

            //return Json(_userService.BgOk("user", "登录成功", "/Home/Index"));
            //return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            //销毁Session
            HttpContext.Session.Clear();
            return Redirect("Login");
        }


        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateVerifyCode()
        {
            string validateCode;
            //byte[] buffer = new ValidateCode().GetVerifyCode(out validateCode);
            byte[] buffer = ValidateCode.GetVerifyCode(out validateCode);
            TempData["validateCode"] = validateCode;
            return File(buffer, @"image/Gif");
        }
    }
}
