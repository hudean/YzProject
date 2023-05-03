1、日志
log4net日志文档
https://logging.apache.org/log4net/release/manual/repositories.html
https://zhuanlan.zhihu.com/p/110458265
https://www.cnblogs.com/pinzi/p/15588766.html
https://github.com/huorswords/Microsoft.Extensions.Logging.Log4Net.AspNetCore
Nlog日志文档
https://nlog-project.org/

Dapr 引用应用程序文档 https://docs.microsoft.com/zh-cn/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio
ASP.NET Core文档   https://docs.microsoft.com/zh-cn/dotnet/architecture/dapr-for-net-developers/reference-application
C#文档  https://docs.microsoft.com/zh-cn/dotnet/csharp/tour-of-csharp/features
.NET API说明文档 https://docs.microsoft.com/zh-cn/dotnet/api/system.linq.iqueryprovider?view=net-5.0
ASP.NET MVC文档 https://docs.microsoft.com/zh-cn/dotnet/architecture/modern-web-apps-azure/work-with-data-in-asp-net-core-apps

.NET 文档  https://docs.microsoft.com/zh-cn/dotnet/core/extensions/custom-configuration-provider

使用 JSON JavaScriptSerializer 进行序列化或反序列化时出错。字符串的长度超过了为 maxJsonLength 属性设置的值。

```
return Json(str);
```

改成

```
 return new JsonResult()
 {
     Data = str,
     MaxJsonLength = int.MaxValue,
     ContentType = "application/json"
 };
```

```
 T类型是bool时josnStr 原数据格式是 "XXX":true 
 读取该节点var josn = jObject[nodeName].ToString();时
  JsonConvert.DeserializeObject<T>(josn);会强转失败
```

改用下面的

```
 if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
 {
    return (T)Convert.ChangeType(josn, typeof(T));
 }
```


