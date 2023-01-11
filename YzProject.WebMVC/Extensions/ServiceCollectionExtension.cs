using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using YzProject.Services.Contract;
using YzProject.Domain;
using YzProject.Domain.Repositories;

namespace YzProject.WebMVC.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 注入仓储
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        public static IServiceCollection AddInjectionRepositorys(this IServiceCollection services, ServiceLifetime serviceLifetime)
        {
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
