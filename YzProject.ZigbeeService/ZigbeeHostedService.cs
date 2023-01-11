using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YzProject.ZigbeeService
{
    /// <summary>
    /// Zigbee托管服务
    /// 继承BackgroundService 或者 IHostedService
    /// </summary>
    public class ZigbeeHostedService : IHostedService, IDisposable
    {
        // 串口通信文章地址
        //https://docs.microsoft.com/zh-cn/dotnet/api/system.io.ports.serialport?redirectedfrom=MSDN&view=dotnet-plat-ext-6.0#mainBody
        //https://www.cnblogs.com/Traveller-Lee/p/6940221.html
        //https://docs.microsoft.com/zh-cn/dotnet/api/system.io.ports.serialport.datareceived?view=dotnet-plat-ext-6.0

        private readonly ILogger<ZigbeeHostedService> _logger;
        //private readonly IApplicationLifetime _applicationLifetime;
        //private SerialPort serialPort = new SerialPort();
        private SerialPort serialPort = new();
        private bool IsDisponsed = true;
        public IConfiguration Configuration { get; }

        //private readonly Dictionary<string, ZSession> _sessions = new Dictionary<string, ZSession>();
        //线程安全字典
        private readonly ConcurrentDictionary<string, ZSession> _sessions = new ConcurrentDictionary<string, ZSession>();

        public static int bedNoLength = 3;//床号的位数,默认为两位,有的医院是三位

        public ZigbeeHostedService(ILogger<ZigbeeHostedService> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
            //下面两种任选一种都可以
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            //serialPort.DataReceived += DataReceivedHandler;
        }

        public void Dispose()
        {
            if (!IsDisponsed)
            {
                this.ClosePort();
                IsDisponsed = true;
                _sessions.Clear();
                //_applicationLifetime.StopApplication();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            IsDisponsed = false;
            return Task.Run(() =>
            {
                try
                {
                    this.OpenPort();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            });

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Dispose();
            });
        }

        #region 串口

        /// <summary>
        /// 获取串口开关状态
        /// </summary>
        public bool PortIsOpen { get { return serialPort.IsOpen; } }

        /// <summary>
        /// 打开串口
        /// </summary>
        private void OpenPort()
        {
            try
            {
                //string portName = Configuration.GetSection("PortName").Value;
                string portName=Configuration["PortName"];
                portName=string.IsNullOrEmpty(portName) ? SerialPort.GetPortNames()?.OrderBy(R => R)?.ToArray()?[0] : portName;
                serialPort.PortName = portName;
                //波特率
                serialPort.BaudRate = 57600;
                //serialPort.BaudRate = 9600;
                //serialPort.Parity = Parity.None;
                //serialPort.StopBits = StopBits.One;
                //serialPort.DataBits = 8;
                //serialPort.Handshake = Handshake.None;
                //serialPort.RtsEnable = true;
                //serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                serialPort.Open();
                Console.WriteLine("串口打开");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        private void ClosePort()
        {
            try
            {
                this.serialPort.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }


        /// <summary>  
        /// 发送数据  
        /// </summary>  
        /// <param name="sendData"></param>  
        public void SendData(string sendData)
        {
            try
            {
                this.serialPort.Encoding = Encoding.UTF8;
                this.serialPort.Write(sendData);
            }
            catch (Exception ex)
            {
                //throw e;
            }
        }

        // private List<byte> receivedBuffer = new List<byte>();

        /// <summary>
        /// 指示已通过由 SerialPort 对象表示的端口接收了数据。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedHandler(
                       object sender,
                       SerialDataReceivedEventArgs e)
        {
            //Thread.Sleep(1);
            try
            {
                #region 串口读取数据

                //开辟接收缓冲区
                byte[] buffer = new byte[serialPort.BytesToRead];
                //从串口读取数据
                serialPort.Read(buffer, 0, buffer.Length);
                //直接使用buffer的话要阻塞当前线程才行
                List<byte> receivedBuffer = new List<byte>();
                receivedBuffer.AddRange(buffer);

                #region 
                ////读取串口数据
                //byte[] buffer = new byte[1024];
                //int dataLength = this.serialPort.Read(buffer, 0, buffer.Length);

                ////添加到接收缓存
                //byte[] data = new byte[dataLength];
                //Array.Copy(buffer, data, dataLength);
                //this.receivedBuffer.AddRange(data);
                #endregion

                #endregion

                #region 解析协议,并处理数据(根据自己的协议和报文做对应的处理下面的只是参考)

                //获取协议数据
                List<byte[]> protocolData = GetProtocolData(receivedBuffer);
                //解析协议数据，获取设备数据
                List<ReturnData> deviceData = GetDeviceData(protocolData);
                foreach (var item in deviceData)
                {
                    var json = JsonConvert.SerializeObject(item);
                    if (item != null && !string.IsNullOrEmpty(item.DeviceId) && item.DeviceId != "0000")
                    {
                        //string clientId = $"ZIGBEE{item.DeviceId}0";
                        #region 
                        //0-9通道
                        //带通道号的给标定的通道号，其余的0通道号
                        int channel = 0;
                        if (item.ChannelNum.Contains("1"))
                        {
                            channel = 1;
                        }
                        else if (item.ChannelNum.Contains("2"))
                        {
                            channel = 2;
                        }
                        else if (item.ChannelNum.Contains("3"))
                        {
                            channel = 3;
                        }
                        string clientId = $"ZIGBEE{item.DeviceId}{channel}";

                        #endregion
                        ZSession session;
                        if (_sessions.ContainsKey(clientId))
                        {
                            session = _sessions[clientId];
                            session.ReceivePackage(item);
                        }
                        else
                        {
                            session = new ZSession(clientId);
                            session.ReceivePackage(item);
                            session.SessionStart();
                            _sessions[clientId] = session;
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {

            }
        }

        #region 数据的解码等处理

        /// <summary>
        /// 提取出协议数据
        /// </summary>
        /// <param name="buffer">接收到的数据缓存</param>
        /// <returns>协议数据列表</returns>
        private static List<byte[]> GetProtocolData(List<byte> buffer)
        {
            List<byte[]> protocolData = new List<byte[]>(); //返回的协议数据列表
            int lastEndIndex = 0;
            try
            {
                //提取协议数据
                for (int i = 0; i < buffer.Count; i++)
                {
                    // FF F1 ED  1E(包长) [30个数据字节]  BCC  FE F2(包尾)
                    //确认数据包头：0xff, 0xf1 && buffer.Count > (i + 9)
                    if (buffer[i] == 0xFF)//包头1
                    {
                        if (buffer.Count >= (i + 9) && buffer[i + 1] == 0xF1)//包头2
                        {
                            //关机包
                            if (buffer[i + 2] == 0xEE)
                            {
                                #region 关机包
                                if (buffer.Count >= (i + 6) && buffer[i + 4] == 0xfe && buffer[i + 5] == 0xf2 && i >= 2)
                                {
                                    byte[] package = new byte[8 + 1];
                                    buffer.CopyTo(i - 2, package, 0, 8);
                                    //统一数据格式，增加一个长度字节，值为设备类型
                                    for (int j = 8; j >= 6; j--)
                                    {
                                        package[j] = package[j - 1];
                                    }
                                    protocolData.Add(package);
                                    lastEndIndex += 8;  //记录最后提取出的字节位置
                                    i += 7;             //定位到下一数据包开始位置
                                }
                                #endregion
                            }
                            else if (buffer[i + 2] == 0xED)//包头3
                            {
                                #region 正常数据包
                                //00 01  FF F1 ED  1E  A1 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   BCC   FE F2
                                //确认数据包长度是否足够
                                int dataLength = buffer[i + 3];
                                //减掉：主节点2字节、包头3字节、长度1个字节、校验码1个字节、包尾2个字节 i - 2 - 3 - 1 - 1 - 2
                                if (i >= 2 && (buffer.Count - i - 7) >= dataLength)
                                {
                                    //验证包尾
                                    if (buffer[i + dataLength + 5] == 0xFE && buffer[i + dataLength + 6] == 0xF2)
                                    {
                                        //确认BCC（异或校验）是否正确
                                        byte[] package = new byte[dataLength + 9];
                                        buffer.CopyTo(i, package, 0, dataLength + 4);
                                        byte bccValue = CheckBcc(package, 0x0e);
                                        if (buffer[i + 4 + dataLength] == bccValue)
                                        {
                                            //BCC正确，提取出数据包
                                            buffer.CopyTo(i - 2, package, 0, dataLength + 9);
                                            protocolData.Add(package);
                                            lastEndIndex += dataLength + 9;  //记录最后提取出的字节位置
                                            i += dataLength + 8;             //定位到下一数据包开始位置
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                buffer.RemoveRange(0, lastEndIndex);  //从接收缓存删除已经提取出的数据
                return protocolData;
            }
            catch (Exception exp)
            {
                return protocolData;
            }
        }

        /// <summary>
        /// BCC（异或）校验收到的数据包
        /// </summary>
        /// <param name="data">收到的数据包</param>
        /// <param name="seed">校验初始使值</param>
        /// <returns></returns>
        private static byte CheckBcc(byte[] data, byte seed)
        {
            byte result = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                result ^= data[i];
            }
            return result;
        }

        /// <summary>
        /// 从协议数据中解析出设备数据
        /// </summary>
        /// <param name="protocolData">协议数据列表</param>
        /// <returns>设备数据列表</returns>
        private static List<ReturnData> GetDeviceData(List<byte[]> protocolData)
        {
            List<ReturnData> deviceData = new List<ReturnData>();  //返回设备数据列表
            try
            {
                foreach (byte[] pd in protocolData)
                {
                    Console.WriteLine(BitConverter.ToString(pd, 0).Replace(" ", string.Empty).Replace("-", " ").ToUpper());
                    try
                    {
                        byte ch = pd[6];//设备类型(通道号定义)
                        //泵
                        if (ch == 0xa1 || ch == 0xb1 || ch == 0xb2 ||
                           ch == 0xc1 || ch == 0xc2 || ch == 0xc3 ||
                           ch == 0xd1 || ch == 0xd2 || ch == 0xd3 || ch == 0xd4)
                        {
                            ReturnData pump = new ReturnData();
                           
                            #region 解析数据

                            int index = 0;
                            //设备ID
                            uint uintTemp = (uint)pd[index++] << 8;
                            uintTemp |= pd[index++];
                            pump.DeviceId = uintTemp.ToString("X04");
                            //通道名/设备类型
                            index = 6;
                            byte byteTemp = pd[index++];
                            pump.ChannelNum = PumpDic.DeviceTypeTable.ContainsKey(byteTemp) ? PumpDic.DeviceTypeTable[byteTemp] : "未知";
                            //设备状态
                            pump.Status = "关机包";

                            //确定是否为关机包
                            if (pd.Length != 9)
                            {
                                #region 正常数据包
                                //设备型号
                                byte byteKey = (byte)(pd[index] & 0x0f);
                                pump.DeviceModel = PumpDic.DeviceModelTable.ContainsKey(byteKey) ? PumpDic.DeviceModelTable[byteKey] : "未知";
                                //电池与网电区别
                                byteKey = (byte)((pd[index] >> 4) & 0x01);
                                pump.PowerType = PumpDic.SupplyPowerStyleTable.ContainsKey(byteKey) ? PumpDic.SupplyPowerStyleTable[byteKey] : "未知";
                                //电池电量
                                byteKey = (byte)((pd[index++] >> 5) & 0x07);
                                pump.BatteryLevel = PumpDic.BatteryLevelTable.ContainsKey(byteKey) ? PumpDic.BatteryLevelTable[byteKey] : "未知";
                                //压力限制
                                if (pump.DeviceModel == "SN-50C6R" || pump.DeviceModel == "SN-50C6TR" || pump.DeviceModel == "SN-50F66R" || pump.DeviceModel == "SN-50T66R" || pump.DeviceModel == "SN-50C66TR")
                                {
                                    byteTemp = pd[index++];
                                    pump.PressureLevel = PumpDic.PressureLevelTable.ContainsKey(byteTemp) ? PumpDic.PressureLevelTable[byteTemp] : "未知";
                                }
                                else
                                {
                                    byteTemp = pd[index++];
                                    pump.PressureLevel = PumpDic.NumPressureLevelTable.ContainsKey(byteTemp) ? PumpDic.NumPressureLevelTable[byteTemp] : "未知";
                                }

                                //2017-11-01 Modify By SL 床号按配置的位数进行处理
                                pump.BedNubmer = pd[index++].ToString();
                                if (pump.BedNubmer.Length < bedNoLength)
                                {
                                    pump.BedNubmer = pump.BedNubmer.PadLeft(bedNoLength, '0');
                                }

                                //机器软件版本
                                byteTemp = pd[index++];
                                float floatTemp = (byteTemp >> 4) + (byteTemp & 0x0f) / 10F;
                                pump.SystemVersion = floatTemp.ToString("F1");
                                //设备状态
                                byteTemp = pd[index++];
                                pump.Status = PumpDic.DeviceStatusTable.ContainsKey(byteTemp) ? PumpDic.DeviceStatusTable[byteTemp] : "未知";
                                if ((ch & 0xf0) != 0xd0) //注射泵
                                {
                                    pump.Status = pump.Status.Replace("注射/输液", "注射");
                                }
                                else
                                {
                                    pump.Status = pump.Status.Replace("注射/输液", "输液");
                                }
                                if (byteTemp == 0x07)
                                {
                                    int a = 0;
                                }
                                //报警
                                UInt16 u16Temp = (UInt16)(pd[index++] << 8);
                                u16Temp |= pd[index++];
                                if (pump.ChannelNum.IndexOf("输液泵") > -1)
                                {
                                    pump.AlarmType = PumpDic.AlarmTypeOfTransPumpTable.ContainsKey(u16Temp) ? PumpDic.AlarmTypeOfTransPumpTable[u16Temp] : "未知";
                                }
                                else if (pump.ChannelNum.IndexOf("营养泵") > -1)
                                {
                                    pump.AlarmType = PumpDic.AlarmTypeOfNutritionPumpTable.ContainsKey(u16Temp) ? PumpDic.AlarmTypeOfNutritionPumpTable[u16Temp] : "未知";
                                }
                                else if (pump.ChannelNum.IndexOf("注射泵") > -1)
                                {
                                    pump.AlarmType = PumpDic.AlarmTypeOfInjectionPumpTable.ContainsKey(u16Temp) ? PumpDic.AlarmTypeOfInjectionPumpTable[u16Temp] : "未知";
                                }

                                if (pump.AlarmType.IndexOf("电机") > -1 || pump.AlarmType.IndexOf("按键错误") > -1 || pump.AlarmType.IndexOf("未知") > -1)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    for (int i = 0; i < pd.Length; i++)
                                    {
                                        sb.Append(pd[i] + " ");
                                    }
                                    //报警分类记录日志
                                    //if (alarmType.IndexOf("电机") > -1)
                                    //    CommonTools.SetError(DateTime.Now.ToLongTimeString() + ">>>>>“电机错误”报警16进制数据包>>>>>：" + sb.ToString());
                                    //else if (alarmType.IndexOf("按键错误") > -1)
                                    //    CommonTools.SetError(DateTime.Now.ToLongTimeString() + ">>>>>“按键错误”报警16进制数据包>>>>>：" + sb.ToString());
                                    //else
                                    //    CommonTools.SetError(DateTime.Now.ToLongTimeString() + ">>>>>“未知”报警16进制数据包>>>>>：" + sb.ToString());
                                }

                                //把未知的报警信息，做日志处理后，更改为无报警
                                if (pump.AlarmType.IndexOf("未知") > -1)
                                {
                                    pump.AlarmType = "无报警";
                                }

                                //注射速率
                                u16Temp = (UInt16)(pd[index++] << 8);
                                u16Temp |= pd[index++];
                                floatTemp = u16Temp;
                                floatTemp /= 10;
                                pump.Rate = floatTemp.ToString("F1");
                                //瞬时速度
                                u16Temp = (UInt16)(pd[index++] << 8);
                                u16Temp |= pd[index++];
                                floatTemp = u16Temp;
                                floatTemp /= 10;
                                pump.Speed = floatTemp.ToString("F1");
                                //当前已注射量
                                u16Temp = (UInt16)(pd[index++] << 16);
                                u16Temp |= (UInt16)(pd[index++] << 8);
                                u16Temp |= pd[index++];
                                floatTemp = u16Temp;
                                floatTemp /= 10;
                                pump.CurrentInjectedSum = floatTemp.ToString("F1");
                                //累计注射总量
                                u16Temp = (UInt16)(pd[index++] << 16);
                                u16Temp |= (UInt16)(pd[index++] << 8);
                                u16Temp |= pd[index++];
                                floatTemp = u16Temp;
                                floatTemp /= 10;
                                pump.AccInjectionTotal = floatTemp.ToString("F1");
                                //注射限制量
                                u16Temp = (UInt16)(pd[index++] << 16);
                                u16Temp |= (UInt16)(pd[index++] << 8);
                                u16Temp |= pd[index++];
                                floatTemp = u16Temp;
                                floatTemp /= 10;
                                pump.InjectionLimit = floatTemp.ToString("F1");
                                //针筒（输注器）型号
                                if (pump.DeviceModel == "SN-50C6R" || pump.DeviceModel == "SN-50C6TR" || pump.DeviceModel == "SN-50F66R" || pump.DeviceModel == "SN-50T66R" || pump.DeviceModel == "SN-50C66TR")
                                {
                                    byteKey = (byte)(pd[index++] & 0x07);
                                    pump.InjectorModel = PumpDic.InjectorModelTable.ContainsKey(byteKey) ? PumpDic.InjectorModelTable[byteKey] : "未知";
                                }
                                else
                                {
                                    byteKey = (byte)(pd[index++] & 0x07);
                                    pump.InjectorModel = PumpDic.InfusionModelTable.ContainsKey(byteKey) ? PumpDic.InfusionModelTable[byteKey] : "未知";
                                }
                                //针筒（输液器）品牌编号 
                                pump.InjectorBrandId = (pd[index++] + 1).ToString();
                                //输液/注射模式
                                byteTemp = pd[index++];
                                pump.InjectionMode = PumpDic.InjectionModeTable.ContainsKey(byteTemp) ? PumpDic.InjectionModeTable[byteTemp] : "未知";
                                //加热（输液泵）
                                byteTemp = pd[index++];
                                pump.Heat = PumpDic.HeatTable.ContainsKey(byteTemp) ? PumpDic.HeatTable[byteTemp] : "未知";
                                //时间类型
                                byteTemp = pd[index++];
                                if (byteTemp == 0x00)
                                {
                                    pump.TimeType = "无效时间参数";
                                }
                                else
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        switch (byteTemp & (0x01 << i))
                                        {
                                            case 0x01:
                                                pump.TimeType += "注射（输液）开始时间/";
                                                break;
                                            case 0x02:
                                                pump.TimeType += "注射（输液）停止时间/";
                                                break;
                                            case 0x04:
                                                pump.TimeType += "报警时间/";
                                                break;
                                            case 0x08:
                                                pump.TimeType += "报警解除时间/";
                                                break;
                                            default: break;
                                        }
                                    }
                                }
                                pump.TimeType = pump.TimeType.Trim('/');
                                //时间
                                uintTemp = pd[index++];
                                uintTemp |= (uint)(pd[index++] << 8);
                                uintTemp |= (uint)(pd[index++] << 16);
                                uintTemp |= (uint)(pd[index++] << 24);
                                int year = 2000 + (int)(uintTemp >> 26);
                                int month = (int)(uintTemp >> 22 & 0x0f);
                                int day = (int)(uintTemp >> 17 & 0x1f);
                                int hour = (int)(uintTemp >> 12 & 0x1f);
                                int minute = (int)(uintTemp >> 6 & 0x3f);
                                int second = (int)(uintTemp >> 0 & 0x3f);
                                //dateTimeParam = new DateTime(year, month, day, hour, minute, second);
                                pump.LastTime = DateTime.Now;
                                #endregion
                            }
                            else
                            {
                                //时间类型
                                pump.TimeType = "电脑系统时间";
                            }
                            #endregion
                            //添加一个泵设备数据
                            deviceData.Add(pump);
                          
                            Console.WriteLine($"解析后泵设备数据:{JsonConvert.SerializeObject(pump)}.");
                           

                        }
                       
                    }
                    catch (Exception exp)
                    {
                        continue;
                    }
                }

                return deviceData;
            }
            catch (Exception exp)
            {
                return deviceData;
            }
        }


        #endregion

        #endregion

    }
}
