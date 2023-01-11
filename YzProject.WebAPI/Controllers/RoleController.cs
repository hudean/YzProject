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
    /// 角色控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// 分页获取角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaginatedListAsync([FromQuery] ParamQueryPageRole param)
        {
            var list = await _roleService.GetPaginatedListAsync(param.RoleName, param.PageIndex, param.PageSize);
            ResultData resultData = new(1, "", list);
            return Ok(resultData);
        }

        /// <summary>
        /// 添加/修改
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("addOrEdit")]
        public async Task<IActionResult> AddOrEditAsync([FromForm] ParamRole param)
        {
            ResultData resultData = new();
            if (await _roleService.ExistAsync(param))
            {
                resultData.Code = 0;
                resultData.Message = "新增失败，原因：该角色名称已存在";
                return Ok(resultData);
            }

            var isSuccess = await _roleService.InsertOrUpdateAsync(param);
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

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        [HttpPost("deleteMany")]
        public async Task<IActionResult> DeleteManyAsync(List<int> ids)
        {
            bool isSuccess = await _roleService.DeleteManyAsync(ids);
            ResultData resultData = new();
            if (isSuccess)
            {
                resultData.Code = 1;
                resultData.Message = "批量删除成功";
            }
            else
            {
                resultData.Code = 0;
                resultData.Message = "批量删除失败";
            }
            return Ok(resultData);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var isSuccess = await _roleService.DeleteAsync(id);
            ResultData resultData = new();
            if (isSuccess)
            {
                resultData.Code = 1;
                resultData.Message = "删除成功";
            }
            else
            {
                resultData.Code = 0;
                resultData.Message = "删除失败";
            }
            return Ok(resultData);
        }

        #region 与用户相关的角色功能

        /// <summary>
        /// 获取所有的角色
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllRoleList")]
        public async Task<IActionResult> GetAllRoleListAsync()
        {
            var list = await _roleService.GetAllListAsync();
            ResultData resultData = new(1, "", list);
            return Ok(resultData);
        }

        /// <summary>
        /// 根据角色Id获取对应的所有菜单和按钮权限列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("getRoleListByUserId")]
        public async Task<IActionResult> GetRoleListByUserIdAsync(int userId)
        {
            var list = await _roleService.GetRoleListByUserIdAsync(userId);
            ResultData resultData = new(1, "", list);
            return Ok(resultData);
        }

        /// <summary>
        /// 给角色授予对应的权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        [HttpGet("setRolesByUserId")]
        public async Task<IActionResult> SetRolesByUserIdAsync(int userId, List<int> roleIds)
        {
            var isSuccess = await _roleService.SetRolesByUserIdAsync(userId, roleIds);
            ResultData resultData = new();
            if (isSuccess)
            {
                resultData.Code = 1;
                resultData.Message = "设置成功";
            }
            else
            {
                resultData.Code = 0;
                resultData.Message = "设置失败";
            }
            return Ok(resultData);
        }

        #endregion

    }
}
