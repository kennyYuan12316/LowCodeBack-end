using HSZ.Common.Cache;
using HSZ.Common.Core.Filter;
using HSZ.Common.DI;
using HSZ.Data.SqlSugar.Extensions;
using HSZ.EventHandler;
using HSZ.JsonSerialization;
using HSZ.Message.Extensions;
using HSZ.Message.Handler;
using HSZ.TaskScheduler.Interfaces.TaskScheduler;
using HSZ.UnifyResult;
using HSZ.Wms.Interfaces.ZjnStartup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OnceMi.AspNetCore.OSS;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;
using Serilog;
using Spire.Xls.Core.Spreadsheet;
using SqlSugar;
using System;
using System.Collections.Generic;
using Yitter.IdGenerator;
//using ZJN.Wcs.DispatchCommon;
//using ZJN.Wcs.Interfaces.DispatchSingleSc;
//using ZJN.Wcs.Interfaces.InitialDispatch;
//using ZJN.Wcs.Interfaces.PlcCommunication;
//using ZJN.Wcs.ScDispatch;

namespace HSZ.API.Core
{
    /// <summary>
    ///
    /// </summary>
    [AppStartup(9)]
    public class Startup : AppStartup
    {


        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            SqlSugarConfigure(services);



            //解决跨域请求
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllCors",
                    builder => builder.SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });


            services.AddJwt<JwtHandler>(enableGlobalAuthorize: true);

            //services.AddCorsAccessor();

            services.AddRemoteRequest();

            services.AddConfigurableOptions<CacheOptions>();

            services.AddControllersWithViews()
                    .AddMvcFilter<RequestActionFilter>()
                    .AddInjectWithUnifyResult<RESTfulResultProvider>()
                     //.AddJsonOptions(options =>
                     //{
                     //    //options.JsonSerializerOptions.Converters.AddDateFormatString("yyyy-MM-dd HH:mm:ss");
                     //    //格式化日期时间格式
                     //    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
                     //});
                     .AddNewtonsoftJson(options =>
                     {
                         //options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                         //默认命名规则
                         options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                         //设置时区为 UTC
                         options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                         //格式化json输出的日期格式
                         options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                         //格式化json输出的日期格式为时间戳
                         options.SerializerSettings.Converters.Add(new NewtonsoftDateTimeJsonConverter());
                         //空值处理
                         //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                     });

            services.AddViewEngine();
            //脱敏词汇检测
            services.AddSensitiveDetection();

            services.AddTaskScheduler();

            #region OSS
            OSSServiceConfigure(services);
            #endregion

            #region 微信
            services.AddSenparcGlobalServices(App.Configuration)//Senparc.CO2NET 全局注册
                        .AddSenparcWeixinServices(App.Configuration);//Senparc.Weixin 注册（如果使用Senparc.Weixin SDK则添加）
            services.AddSession();
            services.AddMemoryCache();//使用本地缓存必须添加
            services.AddSenparcWeixinServices(App.Configuration);
            #endregion

            // 注册 WebSocket服务
            services.AddWebSocketManager();

            // 注册 EventBus 服务
            services.AddEventBus(buidler =>
            {
                // 注册 Log 事件订阅者
                buidler.AddSubscriber<LogEventSubscriber>();
                buidler.AddSubscriber<UserEventSubscriber>();
                buidler.AddSubscriber<TaskEventSubscriber>();
            });
            //确定约定库服务
            int wareType = 1;
            int.TryParse(App.Configuration["dbsys"].ToString(), out wareType);
            services.AddWareService((WareType)wareType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="senparcSetting"></param>
        /// <param name="senparcWeixinSetting"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //  NGINX 反向代理获取真实IP
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // 添加状态码拦截中间件
            app.UseUnifyResultStatusCodes();

            app.UseHttpsRedirection(); // 强制https

            app.UseWebSockets();

            app.MapSockets("/api/message/websocket", serviceProvider.GetService<WebSocketMessageHandler>());

            app.UseStaticFiles();

            // Serilog请求日志中间件---必须在 UseStaticFiles 和 UseRouting 之间
            app.UseSerilogRequestLogging();

            app.UseRouting();


            //导入全局配置
            app.UseCors("AllowAllCors");

            //app.UseCorsAccessor();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseInject(string.Empty);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // 设置雪花id的workerId，确保每个实例workerId都应不同
            var workerId = ushort.Parse(App.Configuration["SnowId:WorkerId"] ?? "1");
            YitIdHelper.SetIdGenerator(new IdGeneratorOptions { WorkerId = workerId, WorkerIdBitLength = 16 });

            #region 微信

            IRegisterService register = RegisterService.Start(senparcSetting.Value).UseSenparcGlobal();//启动 CO2NET 全局注册，必须！
            register.UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value);//微信全局注册,必须！

            #endregion

            // 开启自启动定时任务
            App.GetService<ITimeTaskService>().StartTimerJob();

            //App.GetService<IZjnStartupService>().StartAsync();
            ////获取PLC列表 / 创建连接池 / plc心跳
            //App.GetService<IPlcCommunicationService>().Initial();
            ////初始化WCS业务
            //App.GetService<IInitialDispatchService>().InitialService();

        }

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="services"></param>
        private void SqlSugarConfigure(IServiceCollection services)
        {
            #region 配置sqlsuagr

            List<ConnectionConfig> connectConfigList = new List<ConnectionConfig>();
            var connectionStr = $"{App.Configuration[$"ConnectionStrings:DefaultConnection"]}";
            var dataBase = $"{App.Configuration[$"ConnectionStrings:DBName"]}";
            var dbType = (DbType)Enum.Parse(typeof(DbType), App.Configuration[$"ConnectionStrings:DBType"]);
            var ConfigId = $"{App.Configuration[$"ConnectionStrings:ConfigId"]}";


            connectConfigList.Add(new ConnectionConfig
            {
                ConnectionString = string.Format(connectionStr, dataBase),
                DbType = dbType,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                ConfigId = ConfigId,
                ConfigureExternalServices = new ConfigureExternalServicesExtenisons()
                {
                    EntityNameServiceType = typeof(SugarTable)//这个不管是不是自定义都要写，主要是用来获取所有实体
                }
            });

            services.AddSqlSugar(connectConfigList.ToArray(), db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    if (sql.StartsWith("SELECT"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (sql.StartsWith("DELETE"))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    //在控制台输出sql语句
                    Console.WriteLine(SqlProfiler.ParameterFormat(sql, pars));
                    string s = SqlProfiler.ParameterFormat(sql, pars);
                    Console.WriteLine();
                    //App.PrintToMiniProfiler("SqlSugar", "Info", SqlProfiler.ParameterFormat(sql, pars));
                };
            });

            #endregion
        }

        /// <summary>
        /// 配置对象储存
        /// </summary>
        /// <param name="services"></param>
        private void OSSServiceConfigure(IServiceCollection services)
        {
            var fileStoreType = App.Configuration["HSZ_App:OSS:Provider"];
            var Provider = (OSSProvider)Enum.Parse(typeof(OSSProvider), App.Configuration["HSZ_App:OSS:Provider"]);
            var Endpoint = $"{App.Configuration["HSZ_App:OSS:Endpoint"]}";
            var AccessKey = $"{App.Configuration["HSZ_App:OSS:AccessKey"]}";
            var SecretKey = $"{App.Configuration["HSZ_App:OSS:SecretKey"]}";
            var IsEnableHttps = Convert.ToBoolean(App.Configuration["HSZ_App:OSS:IsEnableHttps"]);
            var IsEnableCache = Convert.ToBoolean(App.Configuration["HSZ_App:OSS:IsEnableCache"]);

            services.AddOSSService(fileStoreType, option =>
            {
                option.Provider = Provider;//服务器
                option.Endpoint = Endpoint;//地址
                option.AccessKey = AccessKey;//服务访问玥
                option.SecretKey = SecretKey;//服务密钥
                option.IsEnableHttps = IsEnableHttps;//是否启用https
                option.IsEnableCache = IsEnableCache;//是否启用缓存
            });
        }
    }
}
