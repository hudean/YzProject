using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YzProject.EventBus.Events;

namespace YzProject.SocketService.IntegrationEvents.Events
{
    /// <summary>
    /// 订阅下发设备的消息队列
    /// </summary>
    public record R_SMessageEvent : IntegrationEvent
    {
        /// <summary>
        /// 接收对象
        /// </summary>
        public EnumMsgTarget To { get; set; }
        /// <summary>
        /// 下发的是SN
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 协议类型
        /// </summary>
        public string DType { get; set; }
        /// <summary>
        /// MAC
        /// </summary>
        public string DeviceKey { get; set; }
        /// <summary>
        /// 通道号
        /// </summary>
        public string CH { get; set; }
        /// <summary>
        /// GUID值
        /// </summary>
        public string MsgId { get; set; }
        /// <summary>
        /// 消息文本内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 时间Tick值
        /// </summary>
        public long TimeTick { get; set; }
        /// <summary>
        /// Socket上报的通信IP
        /// </summary>
        public string IP { get; set; }
    }



    /// <summary>
    /// 消息对象枚举
    /// </summary>
    public enum EnumMsgTarget
    {
        /// <summary>
        /// 单片机
        /// </summary>
        [Description("单片机")]
        mcu,
        /// <summary>
        /// 设备
        /// </summary>
        [Description("设备")]
        device
    }


    /// <summary>
    /// 消息状态枚举
    /// </summary>
    public enum EnumMsgStatus
    {
        /// <summary>
        /// 待下发
        /// </summary>
        [Description("待下发")]
        Pending = 0,
        /// <summary>
        /// 已发布
        /// </summary>
        [Description("已发布")]
        Published = 1,
        /// <summary>
        /// 已回复
        /// </summary>
        [Description("已回复")]
        Replied = 2,
        /// <summary>
        /// 发送方取消       
        /// </summary>
        [Description("发送方取消")]
        Canceled = 3,
        /// <summary>
        /// 超时       
        /// </summary>
        [Description("超时")]
        TimeOut = 4,
        /// <summary>
        /// 超次       
        /// </summary>
        [Description("超次")]
        OverCount = 5,
        /// <summary>
        /// 异常       
        /// </summary>
        [Description("异常")]
        Exception = 6,
        /// <summary>
        /// 已重发       
        /// </summary>
        [Description("已重发")]
        ReSent = 7,
        /// <summary>
        /// 回调成功       
        /// </summary>
        [Description("回调成功")]
        CallBackSuccess = 8,
        /// <summary>
        /// 回调失败      
        /// </summary>
        [Description("回调失败")]
        CallBackFail = 9
    }

    /// <summary>
    /// 收到回复消息后的回调枚举
    /// </summary>
    public enum EnumMsgCallbakType
    {
        /// <summary>
        /// 不做处理
        /// </summary>
        [Description("不做处理")]
        None = 0,
        /// <summary>
        /// 设备操作缓存
        /// </summary>
        [Description("设备操作缓存")]
        CachingKeys_DEVICE_OPER_INFOL_KEY = 1
    }
}
