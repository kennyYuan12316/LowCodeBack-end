using HSZ.Common.Const;
using HSZ.Common.Util;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.EventBus;
using HSZ.EventHandler;
using HSZ.FriendlyException;
using HSZ.Logging.Attributes;
using HSZ.System.Entitys.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;
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
    /// 描 述：全局异常处理
    /// </summary>
    public class LogExceptionHandler : IGlobalExceptionHandler, ISingleton
    {
        private readonly IEventPublisher _eventPublisher;

        public LogExceptionHandler(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// 异步写入异常日记
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var userContext = App.User;
            var httpContext = context.HttpContext;
            var httpRequest = httpContext.Request;
            var headers = httpRequest.Headers;

            if (!context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(IgnoreLogAttribute)))
            {
                var clientInfo = Parser.GetDefault().Parse(headers["User-Agent"]);
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

                await _eventPublisher.PublishAsync(new LogEventSource("Log:CreateExLog", tenantId, tenantDbName, new SysLogEntity
                {
                    Id = YitIdHelper.NextId().ToString(),
                    UserId = userId,
                    UserName = userName,
                    Category = 4,
                    IPAddress = NetUtil.Ip,
                    RequestURL = httpRequest.Path,
                    RequestMethod = httpRequest.Method,
                    Json = context.Exception.Message + "\n" + context.Exception.StackTrace + "\n" + context.Exception.TargetSite.GetParameters().ToString(),
                    PlatForm = String.Format("{0}-{1}", clientInfo.OS.ToString(), clientInfo.UA.ToString()),
                    CreatorTime = DateTime.Now
                }));
            }

            // 写日志文件
            Log.Error(context.Exception.ToString());
        }
    }
}