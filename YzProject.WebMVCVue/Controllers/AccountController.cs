using Microsoft.AspNetCore.Mvc;

namespace YzProject.WebMVCVue.Controllers
{
    public class AccountController : Controller
    {
        //<!--asp.net razor语法@与element ui中@冲突，需要将@进行转换才能调取el-button的@click事件，在页面中转换可以写为 @{@Html.Raw("@");} 也可以使用@@ -->
        public IActionResult Login()
        {
            return View();
        }
    }
}
