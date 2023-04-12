using HSZ.Common.Const;
using HSZ.Common.Core.Manager;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Message.Entitys.Dto.IM;
using HSZ.Message.Entitys.Model.IM;
using HSZ.Message.Interfaces;
using HSZ.System.Entitys.Dto.System.OnlineUser;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HSZ.System.Service.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：在线用户
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "OnlineUser", Order = 176)]
    [Route("api/system/[controller]")]
    public class OnlineUserService : IDynamicApiController, ITransient
    {
        private readonly IImReplyService _imReplyService;
        private readonly ICacheManager _cacheManager;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="OnlineUserService"/>类型的新实例
        /// </summary>
        public OnlineUserService(ICacheManager cacheManager, IImReplyService imReplyService, IUserManager userManager)
        {
            _imReplyService = imReplyService;
            _cacheManager = cacheManager;
            _userManager = userManager;
        }

        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public dynamic GetList([FromQuery] KeywordInput input)
        {
            var tenantId = _userManager.TenantId;
            var userOnlineList = new List<UserOnlineModel>();
            var onlineKey = CommonConst.CACHE_KEY_ONLINE_USER + $"{tenantId}";
            if (_cacheManager.Exists(onlineKey))
            {
                userOnlineList = _cacheManager.GetOnlineUserList(tenantId);
                if (!input.keyword.IsNullOrEmpty())
                    userOnlineList = userOnlineList.FindAll(x => x.userName.Contains(input.keyword));
            }
            return userOnlineList.Adapt<List<OnlineUserListOutput>>();
        }

        /// <summary>
        /// 强制下线
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void ForcedOffline(string id)
        {
            var tenantId = _userManager.TenantId;
            var list = _cacheManager.GetOnlineUserList(tenantId);
            var user = list.Find(it => it.tenantId == tenantId && it.userId == id);
            if (user != null)
            {
                _imReplyService.ForcedOffline(user.connectionId);
                _cacheManager.DelOnlineUser(tenantId + "_" + user.userId);
                _cacheManager.DelUserInfo(tenantId + "_" + user.userId);
            }
        }

        /// <summary>
        /// 批量下线在线用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input">下线用户信息</param>
        [HttpDelete("")]
        public void Clear(string id,[FromBody] BatchOnlineInput input)
        {
            var tenantId = _userManager.TenantId;
            var list = _cacheManager.GetOnlineUserList(tenantId);
            var userList = list.FindAll(it => it.tenantId == tenantId && input.ids.Contains(it.userId));
            userList.ForEach(item =>
            {
                _imReplyService.ForcedOffline(item.connectionId);
                _cacheManager.DelOnlineUser(tenantId + "_" + item.userId);
                _cacheManager.DelUserInfo(tenantId + "_" + item.userId);
            });

        }
    }
}
