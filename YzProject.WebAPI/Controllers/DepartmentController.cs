using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using YzProject.Domain.RequestModel;
using YzProject.Domain.ResultModel;
using YzProject.Services.Contract;

namespace YzProject.WebAPI.Controllers
{
    /// <summary>
    /// 部门控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaginatedListAsync([FromQuery] ParamQueryPageDepartment param)
        {
            var list = await _departmentService.GetPaginatedListAsync(param.DepartmentName, param.PageIndex, param.PageSize);
            ResultData resultData = new(1, "", list);
            return Ok(resultData);
        }

        [HttpPost("addOrEdit")]
        public async Task<IActionResult> AddOrEditAsync(ParamDepartment param)
        {
            ResultData resultData = new();
            if (await _departmentService.ExistAsync(param))
            {
                resultData.Code = 0;
                resultData.Message = "新增/修改失败，原因：已存在该部门";
                return Ok(resultData);
            }
            var isSuccess = await _departmentService.InsertOrUpdateAsync(param);
            if (isSuccess)
            {
                resultData.Code = 1;
                resultData.Message = "添加/修改成功";
            }
            else
            {
                resultData.Code = 0;
                resultData.Message = "添加/修改失败";
            }
            return Ok(resultData);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var list = await _departmentService.GetAllListAsync();
            ResultData resultData = new(1, "", list);
            return Ok(resultData);
        }

        [HttpGet("test")]
        [AllowAnonymous]
        //[Authorize]
        public  IActionResult Test()
        {
            return Content("ok");
        }
    }
}
