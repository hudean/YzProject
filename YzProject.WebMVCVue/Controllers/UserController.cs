using Microsoft.AspNetCore.Mvc;

namespace YzProject.WebMVCVue.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
