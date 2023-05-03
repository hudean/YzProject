解决redis经常超时掉线的问题
1、项目文件csproj增加配置最小线程数，由原200 改为 1500：

```
 <ThreadPoolMinThreads>500</ThreadPoolMinThreads>
```

理由：跟踪RabbitMQ发现在瞬时状态下，未消费的消息会突然激增到2K-10K（重要）
2、修改redis重连尝试，由3次改为10次
3、修改redis KeepAlive 为 180秒
4、增加redis允许的最大客户端连接数为50000个（重要）
查询命令：CONFIG GET maxclients
设置命令：CONFIG set maxclients 50000 
5、升级Sino.Caching 的 redis包由原 2.0.571 升级至最新 2.6.70
6、引入 StackExchange.Redis.Extensions ，由原单例模式改成实现 ConnectionMultiplexer 对象池（重要）
7、检查COMPlus_Thread_UseAllCpuGroups环境变量， 是否已设置为1
8、消息队列要消费者端限流进行消费

https://www.bookstack.cn/read/StackExchange.Redis-docs-cn/Timeouts.md
 https://www.coder.work/article/1578168
 https://github.com/imperugo/StackExchange.Redis.Extensions

对象池详细使用参考https://github.com/imperugo/StackExchange.Redis.Extensions/tree/master/samples/StackExchange.Redis.Samples.Web.Mvc