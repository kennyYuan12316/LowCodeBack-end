using HSZ.Message.Handler;
using HSZ.Message.Manager;
using HSZ.Message.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace HSZ.Message.Extensions
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：websocket模块注册
    /// </summary>
    public static class SocketsCollectionExtensions
    {
        /// <summary>
        /// 注入WebSocketg管理配置
        /// </summary>
        /// <param name="services">服务集合</param>
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<SocketsManager>();
            var backgroundServiceTypes = App.EffectiveTypes.Where(u => typeof(SocketsHandler).IsAssignableFrom(u));
            if (backgroundServiceTypes == null) return services;

            foreach (var type in backgroundServiceTypes)
                if (type.GetTypeInfo().BaseType == typeof(SocketsHandler))
                    services.AddSingleton(type);

            return services;
        }

        /// <summary>
        /// 配置 websocket 中间件拓展
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path"></param>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static IApplicationBuilder MapSockets(this IApplicationBuilder app, PathString path,
            SocketsHandler socket)
        {
            return app.Map(path, x => x.UseMiddleware<SocketsMiddleware>(socket));
        }
    }
}
