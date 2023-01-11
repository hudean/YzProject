1、CommunicationService文件夹下是通信服务
	1)YzProject.MQTTService   WiFi2.4G MQTT协议通信
	2)YzProject.SocketService WiFi5.0G Socket协议通信
	3)YzProject.ZigbeeService 串口通信服务
2、EventBus文件夹下是事件总线服务也可以使用nuget包里mestransit/MassTransit 的代替 
3、YzProject.Common里面是一些通用的辅助帮助类（如MD5加密等等）
4、YzProject.Domain领域层（目前是贫血模型）
5、YzProject.EntityFrameworkCore efcore与数据库连接处理
6、YzProject.Model 数据传输对象DTO
7、YzProject.Redis redis缓存
8、YzProject.Repository 仓储层
9、YzProject.Services  服务层 业务逻辑服务
10、YzProject.WebAPI 接口层
11、YzProject.WebMVC 表现层
12、YzProject.WebMVCVue vue+element表现层(暂时没有用使用纯前端代替)

Mqtt协议(M2Mqtt、MQTTnet、M2MqttDotnetCore)推荐用MQTTnet 4.0版本跟3.0版本有些不一样 看别人写的文章得注意下
开源代码地址：https://github.com/dotnet/MQTTnet
官方demo地址：https://github.com/dotnet/MQTTnet/tree/master/Samples
官方文档地址：https://github.com/dotnet/MQTTnet/wiki/Client
https://www.cnblogs.com/weskynet/p/16441219.html (.net6 版本的MQTTnet 4.0以上版本看)推荐
下面的文章大致看了，没做测试，不保证正确
https://www.cnblogs.com/weskynet/p/16441219.html
https://www.twle.cn/t/19383
https://www.cnblogs.com/kuige/articles/7724786.html
https://www.cnblogs.com/luoocean/p/11232752.html
https://blog.csdn.net/yuming/article/details/125834921

MQTT协议中文版https://github.com/mcxiaoke/mqtt 
MQTT入门https://blog.csdn.net/u012692537/article/details/80263150 
使用 MQTTnet 实现 MQTT 通信示例https://blog.csdn.net/u012692537/article/details/80255010

串口通信
https://www.cnblogs.com/ElijahZeng/p/7609241.html
https://www.cnblogs.com/Traveller-Lee/p/6940221.html
https://www.cnblogs.com/xingboy/p/11052901.html


这几个协议通信服务，可以使用事件总线（消息队列）订阅一些下发设备（客户端）的消息命令进行下发，
设备（客户端）上报的信息可以发布对应的消息队列里做最后的处理，当然你也可以直接在本服务里处理设备（客户端）上传的信息
