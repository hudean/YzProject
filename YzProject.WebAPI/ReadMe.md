1、健康检查
2、事件总线
3、redis缓存
4、全球化/多语言
5、日志
6、jwt和鉴权
7、仓储
8、升级到6.0/7.0：包的版本升级和框架升级
9、代码生成器
10、多租户
11、定时任务 Quartz.NET/Hangfire/Timer/BackgroundService 
12、分词搜素 Elasticsearch 
13、远程过程调用 gRPC 
14、实时应用 SignalR
15、容器docker
16、Api网关 Ocelot/Envoy
17、服务发现服务治理 Consul
18、Web服务器 Nginx/IIS

nginx文章
https://zhuanlan.zhihu.com/p/364588916
授权
控制器 AllowAnonymous  匿名验证，不需要授权就可以访问
控制器 Authorize Unauthorized 需要授权访问
控制器 Authorize 操作 AllowAnonymous   可以访问
控制器 AllowAnonymous 操作 Authorize 可以访问

AuthorizationFilter 就不要使用Authorize特性
使用 AuthorizationFilter : IAuthorizationFilter 的话不会进入Authorize特性标记的控制器,会进入AllowAnonymous特性标记的控制器
使用 AuthorizationFilter : AuthorizeAttribute,IAuthorizationFilter 的话不会进入Authorize特性标记的控制器,会进入AllowAnonymous特性标记的控制器

绑定源参数推理
绑定源特性定义可找到操作参数值的位置。 存在以下绑定源特性：
https://learn.microsoft.com/zh-cn/aspnet/core/web-api/?view=aspnetcore-5.0
	特性			绑定源
[FromBody]			请求正文
[FromForm]			请求正文中的表单数据
[FromHeader]		请求标头
[FromQuery]			请求查询字符串参数
[FromRoute]			当前请求中的路由数据
[FromServices]		作为操作参数插入的请求服务

FromBody 推理说明
对于简单类型（例如 string 或 int），推断不出 [FromBody]。 因此，如果需要该功能，对于简单类型，应使用 [FromBody] 属性。

对于参数是(string param1,string param2)默认使用的是FromQuery

对于参数是(class param)默认使用的是FromBody