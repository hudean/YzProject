using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YzProject.Domain.RequestModel;
using YzProject.Services;
using YzProject.Services.Contract;

namespace YzProject.WebMVC.Controllers
{
    /// <summary>
    /// 角色控制器
    /// </summary>
    public class RoleController : Controller
    {

        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }


        public IActionResult Index()
        {
            return View();
        }

        #region 分页列表、添加/修改、删除、批量删除

        /// <summary>
        /// 分页获取角色列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetPaginatedListAsync(ParamQueryPageRole param)
        {
            return Json(_roleService.BgOk(await _roleService.GetPaginatedListAsync(param.RoleName,param.PageIndex, param.PageSize)));
        }

        /// <summary>
        /// 添加/修改
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddOrEditAsync(ParamRole param)
        {
            if (await _roleService.ExistAsync(param))
            {
                return Json(_roleService.BgError("新增失败，原因：该角色名称已存在"));
            }

            var isSuccess = await _roleService.InsertOrUpdateAsync(param);
            return Json(_roleService.BgOk("添加成功"));
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteManyAsync(List<int> ids)
        {
            bool isSuccess = await _roleService.DeleteManyAsync(ids);
            return Json(_roleService.BgOk(isSuccess ? "删除成功" : "删除失败"));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var res = await _roleService.DeleteAsync(id);
            return Json(_roleService.BgOk("删除成功"));
        }

        #endregion

        #region 与用户相关的角色功能

        /// <summary>
        /// 获取所有的角色
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetAllRoleListAsync()
        {
            return Json(_roleService.BgOk(await _roleService.GetAllListAsync()));
        }

        /// <summary>
        /// 获取所有的角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetConfigurationRoleListAsync(int userId)
        {
            return Json(_roleService.BgOk(await _roleService.GetConfigurationRoleListAsync(userId)));
        }

        /// <summary>
        /// 根据用户Id获取对应的角色
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetRoleListByUserIdAsync(int userId)
        {
            return Json(_roleService.BgOk(await _roleService.GetRoleListByUserIdAsync(userId)));
        }

        /// <summary>
        /// 给用户授予对应的角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task<IActionResult> SetRolesByUserIdAsync(int userId, List<int> roleIds)
        {
            return Json(_roleService.BgOk(await _roleService.SetRolesByUserIdAsync(userId, roleIds)));
        }

        #endregion


    }
}
