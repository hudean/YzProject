using Microsoft.AspNetCore.Mvc;

namespace YzProject.WebMVCVue.Controllers
{
    public class DepartmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
