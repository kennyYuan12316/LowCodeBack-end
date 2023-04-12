using HSZ.Common.Configuration;
using HSZ.Common.Const;
using HSZ.Common.Core.Captcha.General;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Model;
using HSZ.Common.Util;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.EventBus;
using HSZ.EventHandler;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.Logging.Attributes;
using HSZ.OAuth.Service.Dto;
using HSZ.RemoteRequest.Extensions;
using HSZ.System.Entitys.Dto.System.SysConfig;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Permission;
using HSZ.System.Interfaces.System;
using HSZ.UnifyResult;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UAParser;
using Yitter.IdGenerator;

namespace HSZ.OAuth.Service
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：身份认证模块
    /// </summary>
    [ApiDescriptionSettings(Tag = "OAuth", Name = "OAuth", Order = 160)]
    [Route("api/[controller]")]
    public class OAuthService : IDynamicApiController, ITransient
    {
        private readonly IGeneralCaptcha _captchaHandle;// 验证码服务
        private readonly IEventPublisher _eventPublisher;
        private readonly IUsersService _userService; // 用户表仓储
        private readonly IModuleService _moduleService;//功能模块
        private readonly IModuleColumnService _columnService; //功能列
        private readonly IModuleButtonService _moduleButtonService;//功能按钮
        private readonly IModuleFormService _formService;//表单
        private readonly IModuleDataAuthorizeSchemeService _moduleDataAuthorizeSchemeService;
        private readonly IUserManager _userManager; // 用户管理
        private readonly ICacheManager _cacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISysConfigService _sysConfigService;

        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="OAuthService"/>类型的新实例
        /// </summary>
        public OAuthService(
            IGeneralCaptcha captchaHandle,
            IUsersService userService,
            IModuleService moduleService,
            IModuleColumnService columnService,
            IModuleButtonService moduleButtonService,
            IModuleFormService formService,
            IModuleDataAuthorizeSchemeService moduleDataAuthorizeSchemeService,
            IUserManager userManager,
            ICacheManager cacheManager,
            IHttpContextAccessor httpContextAccessor,
            IEventPublisher eventPublisher, ISysConfigService sysConfigService)
        {
            _captchaHandle = captchaHandle;
            _userService = userService;
            _moduleService = moduleService;
            _columnService = columnService;
            _formService = formService;
            _moduleButtonService = moduleButtonService;
            _moduleDataAuthorizeSchemeService = moduleDataAuthorizeSchemeService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _sysConfigService = sysConfigService;
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <param name="codeLength">验证码长度</param>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        [HttpGet("ImageCode/{codeLength}/{timestamp}")]
        [AllowAnonymous, IgnoreLog]
        [NonUnify]
        public IActionResult GetCode(int codeLength,string timestamp)
        {
            return new FileContentResult(_captchaHandle.CreateCaptchaImage(timestamp, 120, 40, codeLength > 0 ? codeLength : 4), "image/jpeg");
        }

        /// <summary>
        /// 首次登录 根据账号获取数据库配置
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns></returns>
        [HttpGet("getConfig/{account}")]
        [AllowAnonymous, IgnoreLog]
        public async Task<dynamic> GetConfigCode(string account)
        {
            if (KeyVariable.MultiTenancy)
            {
                string tenantDbName = App.Configuration["ConnectionStrings:DBName"];
                string tenantId = App.Configuration["ConnectionStrings:ConfigId"];
                string tenantAccout = string.Empty;

                //分割账号
                var tenantAccount = account.Split('@');
                tenantId = tenantAccount.FirstOrDefault();

                if (tenantAccount.Length == 1) account = "admin";
                else account = tenantAccount[1];

                tenantAccout = account;

                var interFace = App.Configuration["HSZ_App:MultiTenancyDBInterFace"] + tenantId;
                var response = await interFace.GetAsStringAsync();
                var result = JSON.Deserialize<RESTfulResult<TenantInterFaceOutput>>(response);
                if (result.code != 200)
                    throw HSZException.Oh(result.msg);
                else if (result.data.dbName == null)
                    throw HSZException.Oh(ErrorCode.D1025);
                else
                    tenantDbName = result.data.dbName;

                _db.AddConnection(new ConnectionConfig()
                {
                    DbType = (DbType)Enum.Parse(typeof(DbType), App.Configuration["ConnectionStrings:DBType"]),
                    ConfigId = tenantId,//设置库的唯一标识
                    IsAutoCloseConnection = true,
                    ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", tenantDbName)
                });
                _db.ChangeDatabase(tenantId);
            }

            //读取配置文件
            var array = new Dictionary<string, string>();
            var data = await _db.Queryable<SysConfigEntity>().Where(x => x.Category.Equals("SysConfig")).ToListAsync();
            foreach (var item in data)
            {
                if (!array.ContainsKey(item.Key)) array.Add(item.Key, item.Value);
            }
            var sysConfig = array.Serialize().Deserialize<SysConfigOutput>();

            //返回给前端 是否开启验证码 和 验证码长度
            return new { enableVerificationCode = sysConfig.enableVerificationCode, verificationCodeNumber = sysConfig.verificationCodeNumber > 0 ? sysConfig.verificationCodeNumber : 4 };
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="input">登录输入参数</param>
        /// <returns></returns>
        [HttpPost("Login"), Consumes("application/x-www-form-urlencoded"), AllowAnonymous, IgnoreLog]
        public async Task<LoginOutput> Login([FromForm] LoginInput input)
        {
            string tenantDbName = App.Configuration["ConnectionStrings:DBName"];
            string tenantId = App.Configuration["ConnectionStrings:ConfigId"];
            string tenantAccout = string.Empty;

            if (KeyVariable.MultiTenancy)
            {
                //分割账号
                var tenantAccount = input.account.Split('@');
                tenantId = tenantAccount.FirstOrDefault();
                if (tenantAccount.Length == 1)
                    input.account = "admin";
                else
                    input.account = tenantAccount[1];
                tenantAccout = input.account;

                var interFace = App.Configuration["HSZ_App:MultiTenancyDBInterFace"] + tenantId;
                var response = await interFace.GetAsStringAsync();
                var result = JSON.Deserialize<RESTfulResult<TenantInterFaceOutput>>(response);
                if (result.code != 200)
                    throw HSZException.Oh(result.msg);
                else if (result.data.dbName == null)
                    throw HSZException.Oh(ErrorCode.D1025);
                else
                    tenantDbName = result.data.dbName;

                _db.AddConnection(new ConnectionConfig()
                {
                    DbType = (DbType)Enum.Parse(typeof(DbType), App.Configuration["ConnectionStrings:DBType"]),
                    ConfigId = tenantId,//设置库的唯一标识
                    IsAutoCloseConnection = true,
                    ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", tenantDbName)
                });
                _db.ChangeDatabase(tenantId);
            }

            //读取配置文件
            var array = new Dictionary<string, string>();
            var data = await _db.Queryable<SysConfigEntity>().Where(x => x.Category.Equals("SysConfig")).ToListAsync();
            foreach (var item in data)
            {
                if (!array.ContainsKey(item.Key)) array.Add(item.Key, item.Value);
            }
            var sysConfig = array.Serialize().Deserialize<SysConfigOutput>();

            //是否开启验证码
            if (input.origin != "code")
            {
                if (sysConfig.enableVerificationCode == 1)
                {
                    if (string.IsNullOrWhiteSpace(input.timestamp) || string.IsNullOrWhiteSpace(input.code))
                        throw HSZException.Oh(ErrorCode.D1029);

                    var imageCode = await _cacheManager.GetCode(input.timestamp);
                    if (imageCode.IsNullOrEmpty())
                        throw HSZException.Oh(ErrorCode.D1030);
                    if (!input.code.ToLower().Equals(imageCode.ToLower()))
                        throw HSZException.Oh(ErrorCode.D1029);
                }
            }

            //根据用户账号获取用户秘钥
            var user = await _db.Queryable<UserEntity>().SingleAsync(it => it.Account.Equals(input.account) && it.DeleteMark == null);
            _ = user ?? throw HSZException.Oh(ErrorCode.D5002);

            // 验证账号是否未被激活
            if (user.EnabledMark == null) throw HSZException.Oh(ErrorCode.D1018);
            // 验证账号是否被禁用
            if (user.EnabledMark == 0) throw HSZException.Oh(ErrorCode.D1019);
            // 验证账号是否被删除
            if (user.DeleteMark == 1) throw HSZException.Oh(ErrorCode.D1017);

            //是否 延迟登录
            if (sysConfig.lockType==2 && user.UnLockTime.IsNotEmptyOrNull() && user.UnLockTime > DateTime.Now)
            {
                var t3 = (user.UnLockTime - DateTime.Now)?.TotalMinutes.ToInt();
                if (t3 < 1) t3 = 1;
                throw HSZException.Oh(ErrorCode.D1027, t3?.ToString());
            }
            if (sysConfig.lockType == 2 && user.UnLockTime.IsNotEmptyOrNull() && user.UnLockTime <= DateTime.Now)
            {
                user.EnabledMark = 1;
                user.LogErrorCount = 0;
                await _db.Updateable(user).UpdateColumns(it => new { it.LogErrorCount, it.EnabledMark }).ExecuteCommandAsync();
            }

            //是否锁定
            if (user.EnabledMark == 2) throw HSZException.Oh(ErrorCode.D1031);

            //获取加密后的密码
            var encryptPasswod = MD5Encryption.Encrypt(input.password + user.Secretkey);

            //账户密码是否匹配
            var userAnyPwd = await _db.Queryable<UserEntity>().SingleAsync(u => u.Account == input.account && u.Password == encryptPasswod && u.DeleteMark == null);
            if (userAnyPwd.IsNullOrEmpty())
            {
                //如果是密码错误 记录账号的密码错误次数
                await UpdateErrorLog(await _db.Queryable<UserEntity>().SingleAsync(u => u.Account == input.account && u.DeleteMark == null), sysConfig);
            }
            else
            {
                userAnyPwd.LogErrorCount = 0;//清空记录错误次数
                userAnyPwd.EnabledMark = 1;//解除锁定
                var result = await _db.Updateable(userAnyPwd).UpdateColumns(it => new { it.LogErrorCount }).ExecuteCommandAsync();
            }
            _ = userAnyPwd ?? throw HSZException.Oh(ErrorCode.D1000);

            // app权限验证
            if (NetUtil.isMobileBrowser && user.IsAdministrator == 0 && !ExistRoleByApp(user.RoleId))
                throw HSZException.Oh(ErrorCode.D1022);

            //登录成功时 判断单点登录信息
            var whitelistSwitch = sysConfig.whitelistSwitch;// await _db.Queryable<SysConfigEntity>().SingleAsync(s => s.Category.Equals("SysConfig") && s.Key.ToLower().Equals("whitelistswitch"));
            var whiteListIp = sysConfig.whiteListIp; //await _db.Queryable<SysConfigEntity>().SingleAsync(s => s.Category.Equals("SysConfig") && s.Key.ToLower().Equals("whitelistip"));
            if (whitelistSwitch.Value.Equals("1") && user.IsAdministrator == 0)
            {
                var ipList = whiteListIp.Split(",").ToList();
                if (!ipList.Contains(App.HttpContext.GetLocalIpAddressToIPv4()))
                    throw HSZException.Oh(ErrorCode.D9002);
            }
            //token过期时间
            var tokenTimeout = sysConfig.tokenTimeout; //await _db.Queryable<SysConfigEntity>().SingleAsync(s => s.Category.Equals("SysConfig") && s.Key.ToLower().Equals("tokentimeout"));
            var accessToken = string.Empty;
            // 生成Token令牌
            if (KeyVariable.MultiTenancy)
            {
                accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
                {
                    { ClaimConst.CLAINM_USERID, userAnyPwd.Id },
                    { ClaimConst.CLAINM_ACCOUNT, userAnyPwd.Account },
                    { ClaimConst.CLAINM_REALNAME, userAnyPwd.RealName },
                    { ClaimConst.CLAINM_ADMINISTRATOR, userAnyPwd.IsAdministrator },
                    { ClaimConst.TENANT_ID, tenantId },
                    { ClaimConst.TENANT_DB_NAME, tenantDbName },
                    { ClaimConst.SINGLELOGIN, sysConfig.singleLogin }
                }, long.Parse(tokenTimeout));
            }
            else
            {
                accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
                {
                    { ClaimConst.CLAINM_USERID, userAnyPwd.Id },
                    { ClaimConst.CLAINM_ACCOUNT, userAnyPwd.Account },
                    { ClaimConst.CLAINM_REALNAME, userAnyPwd.RealName },
                    { ClaimConst.CLAINM_ADMINISTRATOR, userAnyPwd.IsAdministrator },
                    { ClaimConst.TENANT_ID, tenantId },
                    { ClaimConst.TENANT_DB_NAME, tenantDbName },
                    { ClaimConst.SINGLELOGIN, sysConfig.singleLogin }
                }, long.Parse(tokenTimeout));
            }

            // 设置Swagger自动登录
            _httpContextAccessor.HttpContext.SigninToSwagger(accessToken);

            // 生成刷新Token令牌
            var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken, 30);

            // 设置刷新Token令牌
            _httpContextAccessor.HttpContext.Response.Headers["x-access-token"] = refreshToken;

            var httpContext = App.HttpContext;
            var ip = NetUtil.Ip;

            // 修改用户登录信息
            await _eventPublisher.PublishAsync(new UserEventSource("User:UpdateUserLogin", tenantId, tenantDbName, new UserEntity
            {
                Id = user.Id,
                FirstLogIP = user.FirstLogIP ?? ip,
                FirstLogTime = user.FirstLogTime ?? DateTime.Now,
                PrevLogTime = user.LastLogTime,
                PrevLogIP = user.LastLogIP,
                LastLogTime = DateTime.Now,
                LastLogIP = ip,
                LogSuccessCount = user.LogSuccessCount + 1
            }));

            //登录时间
            var clientInfo = Parser.GetDefault().Parse(httpContext.Request.Headers["User-Agent"]);

            // 增加登录日志
            await _eventPublisher.PublishAsync(new LogEventSource("Log:CreateVisLog", tenantId, tenantDbName, new SysLogEntity
            {
                Id = YitIdHelper.NextId().ToString(),
                UserId = user.Id,
                UserName = user.RealName,
                Category = 1,
                IPAddress = ip,
                Abstracts = "登录成功",
                PlatForm = String.Format("{0}-{1}", clientInfo.OS.ToString(), clientInfo.UA.ToString()),
                CreatorTime = DateTime.Now
            }));

            return new LoginOutput()
            {
                theme = user.Theme == null ? "classic" : user.Theme,
                token = "Bearer " + accessToken
            };
        }

        /// <summary>
        /// 锁屏解锁登录
        /// </summary>
        /// <param name="input">登录输入参数</param>
        /// <returns></returns>
        [HttpPost("LockScreen")]
        public async Task LockScreen([Required] LoginInput input)
        {
            //根据用户账号获取用户秘钥
            var secretkey = (await _userService.GetInfoByAccount(input.account)).Secretkey;

            //获取加密后的密码
            var encryptPasswod = MD5Encryption.Encrypt(input.password + secretkey);

            var user = await _userService.GetInfoByLogin(input.account, encryptPasswod);
            _ = user ?? throw HSZException.Oh(ErrorCode.D1000);
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <param name="type">Web和App</param>
        /// <returns></returns>
        [HttpGet("CurrentUser")]
        public async Task<CurrentUserOutput> GetCurrentUser(string type)
        {
            if (type.IsNullOrEmpty()) type = "Web";//默认为Web端菜单目录

            var user = await _userManager.GetUserInfo();
            var userId = user.userId;
            var userContext = App.User;
            var httpContext = _httpContextAccessor.HttpContext;
            var tenantId = userContext?.FindFirstValue(ClaimConst.TENANT_ID);
            var tenantDbName = userContext?.FindFirstValue(ClaimConst.TENANT_DB_NAME);

            var loginOutput = new CurrentUserOutput();
            loginOutput.userInfo = user;

            //菜单
            loginOutput.menuList = await _moduleService.GetUserTreeModuleList(_userManager.IsAdministrator, userId, type);

            var currentUserModel = new CurrentUserModelOutput();
            currentUserModel.moduleList = await _moduleService.GetUserModueList(_userManager.IsAdministrator, userId, type);
            currentUserModel.buttonList = await _moduleButtonService.GetUserModuleButtonList(_userManager.IsAdministrator, userId);
            currentUserModel.columnList = await _columnService.GetUserModuleColumnList(_userManager.IsAdministrator, userId);
            currentUserModel.formList = await _formService.GetUserModuleFormList(_userManager.IsAdministrator, userId);
            currentUserModel.resourceList = await _moduleDataAuthorizeSchemeService.GetResourceList(_userManager.IsAdministrator, userId);

            //权限信息
            var permissionList = new List<PermissionModel>();
            currentUserModel.moduleList.ForEach(menu =>
            {
                var permissionModel = new PermissionModel();
                permissionModel.modelId = menu.id;
                permissionModel.moduleName = menu.fullName;
                permissionModel.button = currentUserModel.buttonList.FindAll(t => t.moduleId.Equals(menu.id)).Adapt<List<AuthorizeModuleButtonModel>>();
                permissionModel.column = currentUserModel.columnList.FindAll(t => t.moduleId.Equals(menu.id)).Adapt<List<AuthorizeModuleColumnModel>>();
                permissionModel.form = currentUserModel.formList.FindAll(t => t.moduleId.Equals(menu.id)).Adapt<List<AuthorizeModuleFormModel>>();
                permissionModel.resource = currentUserModel.resourceList.FindAll(t => t.moduleId.Equals(menu.id)).Adapt<List<AuthorizeModuleResourceModel>>();
                permissionList.Add(permissionModel);
            });

            //await _sysCacheService.SetAsync(CommonConst.CACHE_KEY_PERMISSION + "");
            loginOutput.permissionList = permissionList;

            //系统配置信息
            var sysInfo = await _sysConfigService.GetInfo();
            loginOutput.sysConfigInfo = sysInfo.Adapt<SysConfigInfo>();

            return loginOutput;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logout")]
        public async Task Logout()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.SignoutToSwagger();

            var tenantId = _userManager.TenantId;

            //清除IM中的webSocket
            var list = _cacheManager.GetOnlineUserList(tenantId);
            if (list != null)
            {
                var onlineUser = list.Find(it => it.tenantId == tenantId && it.userId == _userManager.UserId);
                if (onlineUser != null)
                {
                    list.RemoveAll((x) => x.connectionId == onlineUser.connectionId);
                    _cacheManager.SetOnlineUserList(tenantId, list);
                    await _cacheManager.DelUserInfo(tenantId + "_" + _userManager.UserId);
                }
            }
        }

        #region PrivateMethod

        /// <summary>
        /// 判断app用户角色是否存在且有效
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        private bool ExistRoleByApp(string roleIds)
        {
            if (roleIds.IsEmpty())
            {
                return false;
            }
            var roleIdList1 = roleIds.Split(",").ToList();
            var roleIdList2 = _db.Queryable<RoleEntity>().Where(x => x.DeleteMark == null && x.EnabledMark == 1).Select(x => x.Id).ToList();
            return roleIdList1.Intersect(roleIdList2).ToList().Count > 0;
        }

        /// <summary>
        /// 记录密码错误次数
        /// </summary>
        /// <param name="userEntity"></param>
        /// <param name="sysConfigOutput"></param>
        /// <returns></returns>
        private async Task UpdateErrorLog(UserEntity userEntity, SysConfigOutput sysConfigOutput)
        {
            if (userEntity != null)
            {
                if (userEntity.EnabledMark == 1 && userEntity.Account.ToLower() != "admin" && sysConfigOutput.lockType > 0 && sysConfigOutput.passwordErrorsNumber > 2)
                {
                    if (sysConfigOutput.lockType == 1)
                    {
                        userEntity.EnabledMark = userEntity.LogErrorCount >= sysConfigOutput.passwordErrorsNumber - 1 ? 2 : 1;
                    }
                    else
                    {
                        userEntity.UnLockTime = userEntity.LogErrorCount >= sysConfigOutput.passwordErrorsNumber - 1 ? DateTime.Now.AddMinutes(sysConfigOutput.lockTime) : null;
                        userEntity.EnabledMark = userEntity.LogErrorCount >= sysConfigOutput.passwordErrorsNumber - 1 ? 2 : 1;
                    }
                    userEntity.LogErrorCount = userEntity.LogErrorCount + 1;
                    var result = await _db.Updateable(userEntity).UpdateColumns(it => new { it.EnabledMark, it.UnLockTime, it.LogErrorCount }).ExecuteCommandAsync();
                }
            }
        }

        #endregion
    }
}
