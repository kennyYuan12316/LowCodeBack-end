using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.Message.Interfaces;
using HSZ.System.Entitys.Dto.System.SysCache;
using HSZ.System.Interfaces.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.System.Service.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：缓存管理
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "CacheManage", Order = 100)]
    [Route("api/system/[controller]")]
    public class SysCacheService : ISysCacheService, IDynamicApiController, ITransient
    {
        private readonly ICacheManager _cacheManager;
        private readonly IUserManager _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImReplyService _imReplyService;

        /// <summary>
        /// 初始化一个<see cref="SysCacheService"/>类型的新实例
        /// </summary>
        public SysCacheService(ICacheManager cacheManager,
            IHttpContextAccessor httpContextAccessor,
            IUserManager userManager,
            IImReplyService imReplyService)
        {
            _imReplyService = imReplyService;
            _cacheManager = cacheManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        #region GET

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] KeywordInput input)
        {
            var tenantId= _userManager.TenantId;
            var keys = _cacheManager.GetAllCacheKeys().FindAll(q => q.Contains(tenantId));
            var output = new List<CacheListOutput>();
            foreach (var key in keys)
            {
                var model = new CacheListOutput();
                model.name = key;
                model.overdueTime = _cacheManager.GetCacheOutTime(model.name);
                model.cacheSize = await RedisHelper.StrLenAsync(key);
                output.Add(model);
            }
            
            if (!string.IsNullOrEmpty(input.keyword))
            {
                output = output.FindAll(x => x.name.Contains(input.keyword));
            }
            return new { list = output.OrderBy(o => o.overdueTime) };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="name">缓存名称</param>
        [HttpGet("{name}")]
        public async Task<dynamic> GetInfo(string name)
        {
            try
            {
                var strJson = await _cacheManager.GetAsync(name);
                var cacheInfoOutput = new CacheInfoOutput();
                cacheInfoOutput.name = name;
                cacheInfoOutput.value = strJson;
                return cacheInfoOutput;
            }
            catch (Exception)
            {
                return new CacheInfoOutput();
            }
        }

        #endregion

        #region POST

        /// <summary>
        /// 清空单个缓存
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        [HttpDelete("{name}")]
        public async Task Clear(string name)
        {
            var isOk = await _cacheManager.DelAsync(name);
            if (!isOk)
                throw HSZException.Oh(ErrorCode.D1700);
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost("Actions/ClearAll")]
        public async Task ClearAll()
        {
            var tenantId = _userManager.TenantId;

            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.SignoutToSwagger();

            //清除IM中的webSocket
            var list = _cacheManager.GetOnlineUserList(tenantId);

            var user = list.Find(it => it.tenantId == tenantId && it.userId == _userManager.UserId);
            if (user != null)
            {
                _imReplyService.ForcedOffline(user.connectionId);
                _cacheManager.DelOnlineUser(tenantId + "_" + user.userId);
                await _cacheManager.DelUserInfo(tenantId + "_" + user.userId);
            }

            var keys = _cacheManager.GetAllCacheKeys().FindAll(q => q.Contains(tenantId));
            var isOk = await _cacheManager.DelAsync(keys.ToArray());
            if (!isOk)
                throw HSZException.Oh(ErrorCode.D1700);
        }

        #endregion
    }
}
