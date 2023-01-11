using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using YzProject.Domain.Entites;
using YzProject.Domain.RequestModel;
using YzProject.Services.Contract;

namespace YzProject.WebMVC.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GetPaginatedListAsync(ParamQueryPageDepartment param)//(int page, int limit, string departmentName)
        {
            return Json(_departmentService.BgOk(await _departmentService.GetPaginatedListAsync(param.DepartmentName, param.PageIndex, param.PageSize)));
        }

        public async Task<IActionResult> AddOrEditAsync(ParamDepartment param)
        {
            if (await _departmentService.ExistAsync(param))
            {
                return Json(_departmentService.BgError("新增失败，原因：已存在该部门"));
            }

            var isSuccess = await _departmentService.InsertOrUpdateAsync(param);
            return Json(_departmentService.BgOk("添加成功"));
        }


        public async Task<IActionResult> GetAllDepartments()
        {
            return Json(_departmentService.BgOk(await _departmentService.GetAllListAsync()));
        }
    }
}
