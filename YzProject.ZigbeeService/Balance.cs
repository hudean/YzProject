using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.ZigbeeService
{
    public class Balance
    {
        #region 定义常量
        //通道名表
        public static readonly Dictionary<byte, string> ChannelNameTable = new Dictionary<byte, string>{
        {0xe1, "一个通道电子称"}, {0xe2, "两个通道电子称"}, {0xe3, "三个通道电子称"}};
        //设备型号表
        public static readonly Dictionary<byte, string> DeviceModelTable = new Dictionary<byte, string> {
        {0x0c, "SN-D10"}};
        //供电方式表
        public static readonly Dictionary<byte, string> SupplyPowerStyleTable = new Dictionary<byte, string>{
        {0x00, "电池"}, {0x01, "市电"}};
        //电池电量表
        public static readonly Dictionary<byte, string> BatteryLevelTable = new Dictionary<byte, string>{
        {0x02, "一格"}, {0x03, "两格"}, {0x04, "三格"}, {0x01, "欠压"}, {0x07, "电池耗尽"}};
        //机器状态表
        public static readonly Dictionary<byte, string> MachineStatusTable = new Dictionary<byte, string>{
        {0x00, "自检状态"}, {0x01, "设置状态"}, {0x04, "标定状态"}, {0x05, "关机状态"}, {0x06, "保留状态"},
        {0x07, "报警状态"}, {0x0d, "休眠状态"}, {0x0f, "读取历史存档状态"}};
        //报警类型表
        public static readonly Dictionary<UInt16, string> AlarmTypeTable = new Dictionary<UInt16, string>{
        {0x0000, "无报警"}, {0x0100, "重量超限"}, {0x0200, "压力传感器断线"}, {0x0001, "网电掉电"}, {0x0002, "电池欠压"},
        {0x0004, "按键错误"},{0x0008, "存储错误"}, {0x0010, "压力传感器自检错误"}, {0x0020, "电池耗尽"}};
        #endregion

        #region 定义变量、属性
        //设备ID
        private string deviceID = "";
        public string DeviceID { get { return deviceID; } }
        //通道名
        private string channelName = "";
        public string ChannelName { get { return channelName; } }
        //设备型号
        private string deviceModel = "";
        public string DeviceModel { get { return deviceModel; } }
        //供电方式
        private string supplyPowerStyle = "";
        public string SupplyPowerStyle { get { return supplyPowerStyle; } }
        //电池电量
        private string batteryLevel = "";
        public string BatteryLevel { get { return batteryLevel; } }
        //机器软件版本
        private string machineSoftVersion = "";
        public string MachineSoftVersion { get { return machineSoftVersion; } }
        //机器状态
        private string machineStatus = "";
        public string MachineStatus { get { return machineStatus; } }
        //报警类型
        private string alarmType = "";
        public string AlarmType { get { return alarmType; } }
        //通道1重量
        private string channel1Weight = "";
        public string Channel1Weight { get { return channel1Weight; } }
        //通道2重量
        private string channel2Weight = "";
        public string Channel2Weight { get { return channel2Weight; } }
        //通道3重量
        private string channel3Weight = "";
        public string Channel3Weight { get { return channel3Weight; } }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造称数据结构，初始化设备数据
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <param name="channelName">通道名</param>
        /// <param name="deviceModel">设备型号</param>
        /// <param name="supplyPowerStyle">供电试</param>
        /// <param name="batteryLevel">电池电量</param>
        /// <param name="machineSoftVersion">机器软件版本</param>
        /// <param name="machineStatus">机器状态</param>
        /// <param name="alarmType">报警类型</param>
        /// <param name="channel1Weight">通道1重量</param>
        /// <param name="channel2Weight">通道2重量</param>
        /// <param name="channel3Weight">通道3重量</param>
        public Balance(string deviceID, string channelName, string deviceModel, string supplyPowerStyle,
                       string batteryLevel, string machineSoftVersion, string machineStatus, string alarmType,
                       string channel1Weight, string channel2Weight, string channel3Weight)
        {
            this.deviceID = deviceID;
            this.channelName = channelName;
            this.deviceModel = deviceModel;
            this.supplyPowerStyle = supplyPowerStyle;
            this.batteryLevel = batteryLevel;
            this.machineSoftVersion = machineSoftVersion;
            this.machineStatus = machineStatus;
            this.alarmType = alarmType;
            this.channel1Weight = channel1Weight;
            this.channel2Weight = channel2Weight;
            this.channel3Weight = channel3Weight;
        }
        #endregion
    }
    public class PumpDic
    {
        #region 定义常量
        //通道名表
        public static readonly Dictionary<byte, string> DeviceTypeTable = new Dictionary<byte, string>{
        {0xa1, "单通道注射泵"},
        {0xb1, "双通道注射泵1通道"}, {0xb2, "双通道注射泵2通道"},
        {0xc1, "三通道注射泵1通道"}, {0xc2, "三通道注射泵2通道"}, {0xc3, "三通道注射泵3通道"},
        {0xd1, "横式输液泵"}, {0xd2, "立式输液泵"}, {0xd3, "营养泵"},{0xd4, "半挤压（输血）泵"}};
        //设备型号表
        public static readonly Dictionary<byte, string> DeviceModelTable = new Dictionary<byte, string> {
        {0x01, "SN-50C6R"}, {0x02, "SN-50C6TR"}, {0x03, "SN-50F66R"}, {0x04, "SN-50T66R"}, {0x05, "SN-1500HR"}, {0x00, "SN-50C66TR"},
        {0x06, "SN-TCI10R"}, {0x07, "SN-TCI20R"}, {0x08, "SN-1600VR"}, {0x09, "SN-1800VR"}, {0x0a, "SN-600NR"},
        {0x0b, "SN-2000VR"}};
        //供电方式表
        //public static readonly Dictionary<byte, string> SupplyPowerStyleTable = new Dictionary<byte, string>{
        //{0x00, "电池"}, {0x01, "市电"}};
        #region 2021-07-29修改
        public static readonly Dictionary<byte, string> SupplyPowerStyleTable = new Dictionary<byte, string>{
        {0x00, "电池"}, {0x01, "外接电源"}};
        #endregion
        //电池电压表
        public static readonly Dictionary<byte, string> BatteryLevelTable = new Dictionary<byte, string>{
        {0x01, "欠压"}, {0x02, "一格"}, {0x03, "两格"}, {0x04, "三格"},  {0x07, "电池耗尽"}};
        //文字压力档位表
        public static readonly Dictionary<byte, string> PressureLevelTable = new Dictionary<byte, string>{
        {0x01, "低档"}, {0x02, "中档"}, {0x03, "高档"}};
        //数字压力档位表
        public static readonly Dictionary<byte, string> NumPressureLevelTable = new Dictionary<byte, string>{
        {0x01, "1"}, {0x02, "2"}, {0x03, "3"}, {0x04, "4"}, {0x05, "5"}, {0x06, "6"}, {0x07, "7"}, {0x08, "8"}, {0x09, "9"}, {0x0a, "10"}};
        //机器状态表
        //public static readonly Dictionary<byte, string> DeviceStatusTable = new Dictionary<byte, string>{
        //{0x00, "自检状态"}, {0x01, "设置状态"}, {0x02, "注射/输液状态"},{0x03, "注射/输液准备状态"}, {0x04, "校准状态"},
        //{0x05, "关机状态"}, {0x06, "保留状态"}, {0x07, "报警状态"}, {0x08, "设置速度超限报警状态"},{0x09, "超时无操作报警状态"},
        //{0x0a, "设置状态下快进状态"}, {0x0b, "压力释放状态"}, {0x0c, "KVO注射状态"}, {0x0d, "休眠状态"}, {0x0e, "注射前闪烁注射器编号"},
        //{0x0f, "读取历史存档状态"}, {0x10, "管路冲洗"}};
        #region 2021-07-29修改，参考C++文档
        //机器状态表
        public static readonly Dictionary<byte, string> DeviceStatusTable = new Dictionary<byte, string>{
        {0x00, "开机自检"}, {0x01, "设置参数"}, {0x02, "正在输注"},{0x03, "注射/输液准备状态"}, {0x04, "标定状态"},
        {0x05, "关机"}, {0x06, "保留状态"}, {0x07, "高优先级报警"}, {0x08, "设置速度超限报警状态"},{0x09, "超时无操作报警状态"},
        {0x0a, "设置状态下快进状态"}, {0x0b, "压力释放状态"}, {0x0c, "KVO注射状态"}, {0x0d, "休眠"}, {0x0e, "注射前闪烁注射器编号"},
        {0x0f, "读取历史存档状态"}, {0x10, "管路冲洗"}};
        #endregion

        //输液泵报警类型表
        public static readonly Dictionary<UInt16, string> AlarmTypeOfTransPumpTable = new Dictionary<UInt16, string>{
            {0x0000, "无报警"},
            {0x0100, "泵门打开"},
            {0x0200, "气泡报警"},
            {0x0400, "速度超限"},
            {0x0800, "管路堵塞"},
            {0x1000, "KVO运行"},
            {0x2000, "KVO完成"},
            {0x4000, "输液完成"},
            {0x8000, "遗忘操作"},
            {0x0020, "网电掉电"},
            {0x0001, "按键错误"},
            {0x0002, "电机错误1"},
            {0x0004, "电机错误2"},
            {0x0008, "存储错误"},
            {0x0010, "气泡传感器自检错误"},
            {0x0040, "点滴异常"},
            {0x0080, "电池耗尽"}
        };
        //营养泵报警类型表
        public static readonly Dictionary<UInt16, string> AlarmTypeOfNutritionPumpTable = new Dictionary<UInt16, string>{
        {0x0000, "无报警"}, {0x0400, "速度超限"}, {0x0800, "管路堵塞"}, {0x4000, "输液完成"}, {0x8000, "遗忘操作"}, {0x0020, "网电掉电"}, {0x0010, "电池欠压"},
        {0x0001, "按键错误"}, {0x0002, "电机错误1"}, {0x0004, "电机错误2"}, {0x0008, "存储错误"}, {0x0040, "点滴异常"}, {0x0080, "电池耗尽"},{0x0006,"电机丢步"}};
        //注射泵报警类型表
        public static readonly Dictionary<UInt16, string> AlarmTypeOfInjectionPumpTable = new Dictionary<UInt16, string>{
        {0x0000, "无报警"}, {0x0001, "推杆脱离"}, {0x0002, "针筒脱离"}, {0x0004, "速度超限"}, {0x0008, "管路堵塞"}, {0x0010, "注射完成"}, {0x0020, "注射完毕"},
        {0x0040, "残留提示"}, {0x0080, "遗忘操作"}, {0x0100, "网电掉电"}, {0x0200, "电池欠压"}, {0x1000, "按键错误"}, {0x2000, "电机自检错误"},
        {0x4000, "电机运行错误"}, {0x8000, "存储错误"}};
        //针筒型号表
        public static readonly Dictionary<byte, string> InjectorModelTable = new Dictionary<byte, string>{
        {0x07, "5ml针筒"},{0x00, "10ml针筒"}, {0x01, "20ml针筒"}, {0x02, "30ml针筒"}, {0x03, "50ml针筒"}, {0x04, "针筒空"}};
        //输液器型号表
        public static readonly Dictionary<byte, string> InfusionModelTable = new Dictionary<byte, string>{
        {0x00, "20滴/ml"}, {0x01, "60滴/ml"}};
        //输液、注射模式表
        public static readonly Dictionary<byte, string> InjectionModeTable = new Dictionary<byte, string>{
        {0x00, "未知"}, {0x01, "流速模式"}, {0x03, "体重模式"}, {0x02, "时间模式"}, {0x04, "点滴模式"}, {0x05, "输液方案"},
        {0x06, "连续模式"}, {0x07, "间歇模式"},{0x08,"药库流速注射模式"},{0x09,"药库体重注射模式"}};
        //是否加热表
        public static readonly Dictionary<byte, string> HeatTable = new Dictionary<byte, string>{
        {0x00, "未知"}, {0x01, "加热"}, {0x02, "未加热"}};
        #endregion
    }

    public class ReturnData
    {
        public ReturnData() { }
     
        #region 定义数据属性
        /// <summary>
        ///设备编号
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 通道号
        /// </summary>
        public string ChannelNum { get; set; }
        /// <summary>
        ///设备型号 
        /// </summary>
        public string DeviceModel { get; set; }
        /// <summary>
        ///电源类型 
        /// </summary>
        public string PowerType { get; set; }
        /// <summary>
        ///电池电量 
        /// </summary>
        public string BatteryLevel { get; set; }
        /// <summary>
        ///压力限制
        /// </summary>
        public string PressureLevel { get; set; }
        /// <summary>
        ///床号 
        /// </summary>
        public string BedNubmer { get; set; }
        /// <summary>
        ///泵对应的软件版本 
        /// </summary>
        public string SystemVersion { get; set; }
        /// <summary>
        /// 机器状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        ///泵对应的报警类型
        /// </summary>
        public string AlarmType { get; set; }
        /// <summary>
        ///速率
        /// </summary>
        public string Rate { get; set; }
        /// <summary>
        ///瞬时速度
        /// </summary>
        public string Speed { get; set; }
        /// <summary>
        ///当前已注射（输液）量
        /// </summary>
        public string CurrentInjectedSum { get; set; }
        /// <summary>
        ///累计注射（输液）总量
        /// </summary>
        public string AccInjectionTotal { get; set; }
        /// <summary>
        ///注射限制量
        /// </summary>
        public string InjectionLimit { get; set; }
        /// <summary>
        ///针筒（输液器）型号
        /// </summary>
        public string InjectorModel { get; set; }
        /// <summary>
        ///针筒（输液器）品牌编号
        /// </summary>
        public string InjectorBrandId { get; set; }
        /// <summary>
        ///输液/注射模式
        /// </summary>
        public string InjectionMode { get; set; }
        /// <summary>
        ///加热（输液泵）
        /// </summary>
        public string Heat { get; set; }
        /// <summary>
        /// 时间类型
        /// </summary>
        public string TimeType { get; set; }
        /// <summary>
        /// 写入时间
        /// </summary>
        public DateTime LastTime { get; set; }
        #endregion
    }
}
