using HSZ.Common.Const;
using HSZ.Common.Helper;
using HSZ.Common.Util;
using HSZ.DataEncryption;
using HSZ.EventBus;
using HSZ.EventHandler;
using HSZ.Logging.Attributes;
using HSZ.System.Entitys.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UAParser;
using Yitter.IdGenerator;

namespace HSZ.Common.Core.Filter
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：请求日志拦截
    /// </summary>
    public class RequestActionFilter : IAsyncActionFilter
    {
        private readonly IEventPublisher _eventPublisher;

        public RequestActionFilter(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// 请求日记写入
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userContext = App.User;
            var httpContext = context.HttpContext;
            var httpRequest = httpContext.Request;

            var sw = new Stopwatch();
            sw.Start();
            var actionContext = await next();
            sw.Stop();

            // 判断是否请求成功（没有异常就是请求成功）
            var isRequestSucceed = actionContext.Exception == null;
            var headers = httpRequest.Headers;
            var clientInfo = Parser.GetDefault().Parse(headers["User-Agent"]);
            if (!context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(IgnoreLogAttribute)))
            {
                var tenantId = userContext?.FindFirstValue(ClaimConst.TENANT_ID);
                var tenantDbName = userContext?.FindFirstValue(ClaimConst.TENANT_DB_NAME);
                var userId = userContext?.FindFirstValue(ClaimConst.CLAINM_USERID);
                var userName = userContext?.FindFirstValue(ClaimConst.CLAINM_REALNAME);

                if (!context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(AllowAnonymousAttribute)) && !App.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    var token = Regex.Match(App.HttpContext.Request.QueryString.Value, @"[?&]token=Bearer%20([\w\.-]+)($|&)").Groups[1].Value;
                    var claims = JWTEncryption.ReadJwtToken(token.Replace("Bearer ", "").Replace("bearer ", ""))?.Claims;
                    tenantId = claims.FirstOrDefault(e => e.Type == ClaimConst.TENANT_ID)?.Value;
                    tenantDbName = claims.FirstOrDefault(e => e.Type == ClaimConst.TENANT_DB_NAME)?.Value;
                    userId = claims.FirstOrDefault(e => e.Type == ClaimConst.CLAINM_USERID)?.Value;
                    userName = claims.FirstOrDefault(e => e.Type == ClaimConst.CLAINM_REALNAME)?.Value;
                }

                await _eventPublisher.PublishAsync(new LogEventSource("Log:CreateReLog", tenantId, tenantDbName, new SysLogEntity
                {
                    Id = YitIdHelper.NextId().ToString(),
                    UserId = userId,
                    UserName = userName,
                    Category = 5,
                    IPAddress = NetUtil.Ip,
                    RequestURL = httpRequest.Path,
                    RequestDuration = (int)sw.ElapsedMilliseconds,
                    RequestMethod = httpRequest.Method,
                    PlatForm = String.Format("{0}-{1}", clientInfo.OS.ToString(), clientInfo.UA.ToString()),
                    CreatorTime = DateTime.Now
                }));

                if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(OperateLogAttribute)))
                {
                    //操作参数
                    var args = JsonConvert.SerializeObject(context.ActionArguments);
                    var result = (actionContext.Result as JsonResult).Value;
                    var module = context.ActionDescriptor.EndpointMetadata.Where(x => x.GetType() == typeof(OperateLogAttribute)).ToList().FirstOrDefault() as OperateLogAttribute;

                    await _eventPublisher.PublishAsync(new LogEventSource("Log:CreateOpLog", tenantId, tenantDbName, new SysLogEntity
                    {
                        Id = YitIdHelper.NextId().ToString(),
                        UserId = userId,
                        UserName = userName,
                        Category = 3,
                        IPAddress = NetUtil.Ip,
                        RequestURL = httpRequest.Path,
                        RequestDuration = (int)sw.ElapsedMilliseconds,
                        RequestMethod = module.Action,
                        PlatForm = String.Format("{0}-{1}", clientInfo.OS.ToString(), clientInfo.UA.ToString()),
                        CreatorTime = DateTime.Now,
                        ModuleName = module.ModuleName,
                        Json =string.Format("{0}应用【{1}】【{2}】", module.Action, args, JsonConvert.SerializeObject(result))
                    }));
                }

            }
        }
    }
}