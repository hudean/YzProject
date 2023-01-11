using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YzProject.Domain.ResultModel;
using YzProject.Services.Contract;

namespace YzProject.Services
{
    public class ResultService : IResultService
    {
        //public ResultData Ok(object data, string msg = "")
        //{
        //    return new ResultData { Code = (int)HttpStatusCode.OK, Message = msg, Data = data ?? string.Empty };
        //}

        //public ResultData Error(int code, string errMsg)
        //{
        //    return new ResultData { Code = code, Message = errMsg };
        //}

        #region 后台管理系统

        public JsonResponse BgOk(object data, string msg = "", string redirect = "")
        {
            return new JsonResponse { code = ResponseCode.Success, data = data, msg = msg, redirect = redirect };
        }

        public JsonResponse BgError(string errMsg)
        {
            return new JsonResponse { code = ResponseCode.Fail, msg = errMsg };
        }

        #endregion
    }
}
