//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace YzProject.SocketService.SuperSocketHelper
//{
//    /// <summary>
//    /// （3）、建立一个数据接收过滤器，继承ReceiveFilterHelper类，过来接收并过滤指定的信息内容。
//    /// 数据接收过滤器
//    /// </summary>
//    public class DTReceiveFilter : ReceiveFilterHelper<DTRequestInfo>
//    {
//        /// <summary>
//        /// 重写方法
//        /// </summary>
//        /// <param name="readBuffer">过滤之后的数据缓存</param>
//        /// <param name="offset">数据起始位置</param>
//        /// <param name="length">数据缓存长度</param>
//        /// <returns></returns>
//        protected override DTRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
//        {
//            //返回构造函数指定的数据格式
//            return new DTRequestInfo(Encoding.UTF8.GetString(readBuffer, offset, length), readBuffer);
//        }
//    }
//}
