using Microsoft.AspNetCore.Mvc;

namespace YzProject.WebMVCVue.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
