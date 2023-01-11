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
    /// 菜单控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// 分页获取菜单列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaginatedListAsync([FromQuery] ParamQueryPageMenu param)
        {
            var list = await _menuService.GetPaginatedListAsync(param.MenuName, param.MenuType, param.PageIndex, param.PageSize);
            ResultData resultData = new(1, "", list);
            return Ok(resultData);
        }

        /// <summary>
        /// 添加/修改
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("addOrEdit")]
        public async Task<IActionResult> AddOrEditAsync(ParamMenu param)
        {
            ResultData resultData = new();
            if (await _menuService.ExistAsync(param))
            {
                resultData.Code = 0;
                resultData.Message = "新增失败，原因：菜单编码和菜单名称必须唯一";
                return Ok(resultData);
            }

            var isSuccess = await _menuService.InsertOrUpdateAsync(param);
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
            ResultData resultData = new();
            bool isSuccess = await _menuService.DeleteManyAsync(ids);
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
            ResultData resultData = new();
            var isSuccess = await _menuService.DeleteAsync(id);
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

        #region 与角色相关的权限（permission）授权功能

        /// <summary>
        /// 获取所有的菜单和按钮列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("getPermissionList")]
        public async Task<IActionResult> GetPermissionListAsync()
        {
            var list = await _menuService.GetAllListAsync();
            ResultData resultData = new(1, "", list);
            return Ok(resultData);
        }

        /// <summary>
        /// 根据角色Id获取对应的所有菜单和按钮权限列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("getPermissionListByRoleId")]
        public async Task<IActionResult> GetPermissionListByRoleIdAsync(int roleId)
        {
            var list = await _menuService.GetPermissionListByRoleIdAsync(roleId);
            ResultData resultData = new(1, "", list);
            return Ok(resultData);
        }

        /// <summary>
        /// 给角色授予对应的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        [HttpPost("permissionAuthorizeByRoleId")]
        public async Task<IActionResult> PermissionAuthorizeByRoleIdAsync(int roleId, List<int> permissionIds)
        {
            var isSuccess = await _menuService.PermissionAuthorizeByRoleIdAsync(roleId, permissionIds);
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
