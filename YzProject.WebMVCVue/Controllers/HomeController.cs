using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using YzProject.WebMVCVue.Models;

namespace YzProject.WebMVCVue.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region 菜单测试

        public IActionResult Index1()
        {
            return View();
        }

        public IActionResult Index2()
        {
            return View();
        }

        public IActionResult Index3()
        {
            return View();
        }

        public IActionResult Index4()
        {
            return View();
        }

        public IActionResult Index5()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        // 参考文档 https://blog.csdn.net/awei0130/article/details/90269898
        // https://blog.csdn.net/awei0130/article/details/90318839
        public IActionResult MenuTestIndex()
        {
            List<DemoMenu> menus = new List<DemoMenu>()
            {
                    new DemoMenu("导航一","path","name1", "el-icon-menu","1","0" ),
                        new DemoMenu("分组一","path","name2","el-icon-location","2","1"),
                            //new DemoMenu("选项一","/path","name3","el-icon-location","3","2"),
                            //new DemoMenu("选项二","/path","name4","el-icon-location","4","2"),
                            new DemoMenu("选项一","/home/index2","name3","el-icon-location","3","2"),
                            new DemoMenu("选项二","/home/index3","name4","el-icon-location","4","2"),
                        //new DemoMenu("分组二","/path","name5","el-icon-location","5","1"),
                        new DemoMenu("分组二","/home/index4","name5","el-icon-location","5","1"),
                        new DemoMenu("分组三","path","name6","el-icon-location","6","1"),
                            new DemoMenu("选项一","/path","name7","el-icon-location","7","6"),
                            new DemoMenu("选项二","/path","name8","el-icon-location","8","6"),

                    new DemoMenu("导航二","path","name9", "el-icon-menu","9","0"),
                        new DemoMenu("分组一","path","name10","el-icon-location","10","9"),
                            new DemoMenu("选项一","path","name11","el-icon-location","11","10"),
                            new DemoMenu("选项二","path","name12","el-icon-location","12","10"),
                        new DemoMenu("分组二","/path","name13","el-icon-location","13","9"),

                    //new DemoMenu("导航三","/path","name", "el-icon-menu","14","0")
                    new DemoMenu("导航三","/home/index5","name", "el-icon-menu","14","0")
            };
            ViewData["system_name"] = "XXXXXXXXXXXX系统";
            ViewData["username"] = "美好的心情";
            return View(menus);
        }

        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
