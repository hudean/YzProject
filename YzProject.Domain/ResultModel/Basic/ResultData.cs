using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.ResultModel
{
    public class ResultData//<T>
    {
        public ResultData()
        { 
        
        }

        // public ResultData(int code, string message, T data)
        public ResultData(int code, string message, object data)
        {
            //code = (int)System.Net.HttpStatusCode.OK;
            Code = code;
            Message = message;
            Data = data;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        //public T Data { get; set; }
    }
}
