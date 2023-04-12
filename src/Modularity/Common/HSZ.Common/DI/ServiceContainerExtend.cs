using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Common.DI
{
    public static class ServiceContainerExtend
    {
        public static IServiceCollection AddWareService(this IServiceCollection services, WareType wareType)
        {
            #region 处理依赖注入
            //使用加载器技术，遍历所有的类，筛选标记注解DI的类，并过滤掉抽象类,并按照注解方式自动注入类
            AppDomain.CurrentDomain.GetAssemblies()
                                                  //找到自己的类
                                                  .Where(t => t.FullName.StartsWith("ZJN.Calb") || t.FullName.StartsWith("HSZ.Wms"))
                                                  .SelectMany(t => t.GetTypes())
                                                  //查找是否包含DI特性并且查看是否是抽象类
                                                  .Where(a => a.GetCustomAttributes(true).Select(t => t.GetType()).Contains(typeof(WareDIAttribute)) && !a.IsAbstract)
                                                  //转换成list
                                                  .ToList()
                                                  //循环
                                                  .ForEach(t =>
                                                  {
                                                      var Di = (WareDIAttribute)t.GetCustomAttributes(true).FirstOrDefault(d => d.GetType() == typeof(WareDIAttribute));

                                                      if (Di.WareType == wareType)
                                                      {
                                                          services.RemoveAll(t);
                                                          foreach (var intfType in t.GetInterfaces())
                                                          {
                                                              switch (Di.DIType)
                                                              {
                                                                  case DIType.SINGLETON:
                                                                      services.AddSingleton(intfType, t);
                                                                      break;
                                                                  case DIType.OTHER:
                                                                      services.AddScoped(intfType, t);
                                                                      break;
                                                                  default:
                                                                      services.AddScoped(intfType, t);
                                                                      break;
                                                              }
                                                          }
                                                      }
                                                      else if (Di.WareType == WareType.Default)
                                                      {
                                                          foreach (var intfType in t.GetInterfaces())
                                                          {

                                                              var type = services.FirstOrDefault(t => t.GetType() == intfType);
                                                              if (type != null) continue;
                                                              switch (Di.DIType)
                                                              {
                                                                  case DIType.SINGLETON:
                                                                      services.AddSingleton(intfType, t);
                                                                      break;
                                                                  case DIType.OTHER:
                                                                      services.AddScoped(intfType, t);
                                                                      break;
                                                                  default:
                                                                      services.AddScoped(intfType, t);
                                                                      break;
                                                              }

                                                          }
                                                      }
                                                  });
            #endregion
            return services;
        }
    }
}
