using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System;
using System.Linq;
using YzProject.Domain;
using YzProject.Services.Contract;
using YzProject.Domain.Repositories;

namespace YzProject.WebAPI.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 依赖注入仓储
        /// 参考 文档https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        public static IServiceCollection AddInjectionRepositorys(this IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            //注入泛型的
            // services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            string assemblyName = "YzProject.Repository";
            var serviceAsm = Assembly.Load(new AssemblyName(assemblyName));
            //排除程序程序集中的接口、私有类、抽象类、通用类型
            var typeList = serviceAsm.GetTypes().Where(t => typeof(IRepository).IsAssignableFrom(t) && t.IsClass && !t.GetTypeInfo().IsAbstract && !t.IsGenericType && !t.IsSealed && !t.IsInterface).ToList();
            foreach (Type serviceType in typeList)
            {

                //查找当前类继承且包含当前类名的接口
                var interfaceType = serviceType.GetInterfaces().Where(o => o.Name.Contains(serviceType.Name)).FirstOrDefault();
                if (interfaceType != null)
                {
                    switch (serviceLifetime)
                    {
                        case ServiceLifetime.Transient:
                            services.AddTransient(interfaceType, serviceType);
                            break;

                        case ServiceLifetime.Scoped:
                            services.AddScoped(interfaceType, serviceType);
                            break;

                        case ServiceLifetime.Singleton:
                            services.AddSingleton(interfaceType, serviceType);
                            break;

                        default:
                            services.AddScoped(interfaceType, serviceType);
                            break;
                    }
                }
            }

            return services;
        }


        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddInjectionServices(this IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            string assemblyName = "YzProject.Services";
            var serviceAsm = Assembly.Load(new AssemblyName(assemblyName));
            //排除程序程序集中的接口、私有类、抽象类、通用类型

            var typeList = serviceAsm.GetTypes().Where(t => typeof(IBaseService).IsAssignableFrom(t) && t.IsClass && !t.GetTypeInfo().IsAbstract && !t.IsGenericType && !t.IsSealed && !t.IsInterface).ToList();
            foreach (Type serviceType in typeList)
            {
                #region 循环注入该累所有继承的接口（不推荐）
                //var interfaceTypes = serviceType.GetInterfaces();
                //foreach (var interfaceType in interfaceTypes)
                //{
                //    //services.AddScoped(interfaceType, serviceType);
                //    switch (serviceLifetime)
                //    {
                //        case ServiceLifetime.Transient:
                //            services.AddTransient(interfaceType, serviceType);
                //            break;

                //        case ServiceLifetime.Scoped:
                //            services.AddScoped(interfaceType, serviceType);
                //            break;

                //        case ServiceLifetime.Singleton:
                //            services.AddSingleton(interfaceType, serviceType);
                //            break;

                //        default:
                //            services.AddScoped(interfaceType, serviceType);
                //            break;
                //    }
                //}

                #endregion

                //查找当前类继承且包含当前类名的接口
                var interfaceType = serviceType.GetInterfaces().Where(o => o.Name.Contains(serviceType.Name)).FirstOrDefault();
                if (interfaceType != null)
                {
                    switch (serviceLifetime)
                    {
                        case ServiceLifetime.Transient:
                            services.AddTransient(interfaceType, serviceType);
                            break;

                        case ServiceLifetime.Scoped:
                            services.AddScoped(interfaceType, serviceType);
                            break;

                        case ServiceLifetime.Singleton:
                            services.AddSingleton(interfaceType, serviceType);
                            break;

                        default:
                            services.AddScoped(interfaceType, serviceType);
                            break;
                    }
                }
            }
            return services;
        }
    }
}
