using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YzProject.Common;
using YzProject.Domain.Entites;
using YzProject.Domain.RequestModel;
using YzProject.Services.Contract;

namespace YzProject.WebMVC.Controllers
{
    /// <summary>
    /// excel导入导出示例
    /// </summary>
    public class ExcelExampleController : Controller
    {
        private readonly IExcelExampleService _excelExampleService;

        public ExcelExampleController(IExcelExampleService excelExampleService)
        {
            _excelExampleService = excelExampleService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> ExportExecelAsync()
        {
            var list = await _excelExampleService.GetAllListAsync();

            if (list?.Any() ?? false)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("姓名", typeof(string));
                dt.Columns.Add("身份证号码", typeof(string));
                dt.Columns.Add("年龄", typeof(string));
                dt.Columns.Add("描述", typeof(string));
                dt.Columns.Add("创建时间", typeof(string));
                dt.Clear();
                foreach (var item in list)
                {
                   DataRow dr = dt.NewRow();
                   dr[0] = item.Name;
                   dr[1] = item.IdentityCardCode;
                   dr[2] = item.Age;
                    dr[3] = item.Description;
                    dr[4] = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    dt.Rows.Add(dr);
                }

                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    dt.Columns[0].ColumnName = "序号";
                //}
                using (MemoryStream ms = ExcelNpoiHelper.RenderDataTableToExcel(dt) as MemoryStream)
                {
                    /*输出文件流，浏览器自动提示下载*/
                    //string filename = fileName;
                    //Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + filename));
                    //Response.BinaryWrite(ms.ToArray());
                    //return File(ms.ToArray(), "application/ms-excel", $"{Guid.NewGuid().ToString()}.xlsx");
                    return File(ms.ToArray(), "application/ms-excel;charset=utf-8", $"{Guid.NewGuid().ToString()}.xls");
                    // return File(new MemoryStream(System.Text.Encoding.Default.GetBytes(info)), "text/plain", "result.txt");
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> GetPaginatedListAsync(ParamQueryPageDepartment param)//(int page, int limit, string departmentName)
        {
            return Json(_excelExampleService.BgOk(await _excelExampleService.GetPaginatedListAsync(param.DepartmentName, param.PageIndex, param.PageSize)));
        }

        public async Task<IActionResult> AddOrEditAsync(ExcelExample param)
        {
            if (await _excelExampleService.ExistAsync(param))
            {
                return Json(_excelExampleService.BgError("新增失败，原因：已存在该部门"));
            }

            var isSuccess = await _excelExampleService.InsertOrUpdateAsync(param);
            return Json(_excelExampleService.BgOk("添加成功"));
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                //var model = await _roleService.GetByIdAsync(id);
                await _excelExampleService.DeleteAsync(id);
                return Json(_excelExampleService.BgOk("删除成功"));
            }
            catch (Exception ex)
            {
                return Json(_excelExampleService.BgError("删除失败，" + ex.ToString()));
            }
        }

        public async Task<IActionResult> DeleteManyAsync(List<int> ids)
        {
            try
            {
                //var model = await _roleService.GetByIdAsync(id);
                await _excelExampleService.DeleteManyAsync(ids);
                return Json(_excelExampleService.BgOk("删除成功"));
            }
            catch (Exception ex)
            {
                return Json(_excelExampleService.BgError("删除失败，" + ex.ToString()));
            }
        }

    }
}
