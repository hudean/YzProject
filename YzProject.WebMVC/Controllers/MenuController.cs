using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YzProject.Domain.RequestModel;
using YzProject.Services.Contract;

namespace YzProject.WebMVC.Controllers
{
    /// <summary>
    /// 菜单控制器
    /// 菜单只分为一级菜单和二级菜单不搞多个分级(多级很少用)
    /// </summary>
    public class MenuController : Controller
    {

        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }


        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetMenuListAsync()
        {
            return Json(_menuService.BgOk(await _menuService.GetMenuListAsync()));
        }

        /// <summary>
        /// 根据userId获取菜单列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetMenuListByUserIdAsync(int userId)
        {
            //也可以用session获取userid
            //int.TryParse( HttpContext.Session.GetString("LoginUserId"),out userId);
            return Json(_menuService.BgOk(await _menuService.GetMenuListByUserIdAsync(userId)));
        }

        /// <summary>
        /// 获取一级菜单列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetOneLevelMenuListAsync()
        {
            return Json(_menuService.BgOk(await _menuService.GetOneLevelMenuListAsync()));
        }

        #region 分页列表、添加/修改、删除、批量删除

        /// <summary>
        /// 菜单列表分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetPaginatedListAsync(ParamQueryPageMenu param)
        {
            return Json(_menuService.BgOk(await _menuService.GetPaginatedListAsync(param.MenuName, param.MenuType, param.PageIndex, param.PageSize)));
        }

        /// <summary>
        /// 添加/修改
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddOrEditAsync(ParamMenu param)
        {
            if (await _menuService.ExistAsync(param))
            {
                return Json(_menuService.BgError("新增失败，原因：菜单编码和菜单名称必须唯一"));
            }

            var isSuccess = await _menuService.InsertOrUpdateAsync(param);
            return Json(_menuService.BgOk(isSuccess ? "添加成功" : "添加失败"));
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteManyAsync(List<int> ids)
        {
            bool isSuccess = await _menuService.DeleteManyAsync(ids);
            return Json(_menuService.BgOk(isSuccess ? "删除成功" : "删除失败"));
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var res = await _menuService.DeleteAsync(id);
            return Json(_menuService.BgOk("删除成功"));
        }

        #endregion

        #region 与角色相关的权限（permission）授权功能

        /// <summary>
        /// 获取所有的菜单和按钮列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetPermissionListAsync()
        {
            return Json(_menuService.BgOk(await _menuService.GetAllListAsync()));
        }

        /// <summary>
        /// 获取所有的菜单和按钮列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetConfigurationPermissionListAsync(int roleId)
        {
            return Json(_menuService.BgOk(await _menuService.GetConfigurationPermissionListAsync(roleId)));
        }

        /// <summary>
        /// 根据角色Id获取对应的所有菜单和按钮权限列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetPermissionListByRoleIdAsync(int roleId)
        {
            return Json(_menuService.BgOk(await _menuService.GetPermissionListByRoleIdAsync(roleId)));
        }

        /// <summary>
        /// 给角色授予对应的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds">1,2,3,4,5,6</param>
        /// <returns></returns>
        public async Task<IActionResult> PermissionAuthorizeByRoleIdAsync(int roleId,string permissionIds)//ParamMenuAuth param
        {
            //var ids = Request.Form["permissionIds"];
            List<int> ids = new List<int>();
            if (!string.IsNullOrEmpty(permissionIds))
            {
                try
                {
                    var idStrs = permissionIds?.Split(',');
                    if (idStrs != null && idStrs.Length > 0)
                    {
                        foreach (var item in idStrs)
                        {
                            ids.Add(Convert.ToInt32(item));
                        }
                    }
                }
                catch(Exception ex)
                { 
                
                }
                
            }
            return Json(_menuService.BgOk(await _menuService.PermissionAuthorizeByRoleIdAsync(roleId, ids)));
        }

        #endregion
    }
}
