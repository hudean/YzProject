using Microsoft.AspNetCore.Mvc;

namespace YzProject.WebMVCVue.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
