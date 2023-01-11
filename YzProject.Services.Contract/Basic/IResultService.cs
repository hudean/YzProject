using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain.ResultModel;

namespace YzProject.Services.Contract
{
    public interface IResultService : IBaseService
    {
        //ResultData Ok(object data = null, string msg = "");

        //ResultData Error(int code, string errMsg);

        #region 后台管理系统

        /// <summary>
        /// 后台专用
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <param name="redirect"></param>
        /// <returns></returns>
        JsonResponse BgOk(object data, string msg = "", string redirect = "");

        /// <summary>
        /// 后台专用
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        JsonResponse BgError(string errMsg);

        #endregion
    }
}
