using HSZ.Common.Enum;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Expand.Thirdparty;
using HSZ.Expand.Thirdparty.Email;
using HSZ.Expand.Thirdparty.Email.Model;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.Logging.Attributes;
using HSZ.System.Entitys.Dto.System.SysConfig;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.System.Service.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：系统配置
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "SysConfig", Order = 211)]
    [Route("api/system/[controller]")]
    public class SysConfigService : ISysConfigService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<SysConfigEntity> _sysConfigRepository;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        private readonly SqlSugarScope Db;

        /// <summary>
        /// 初始化一个<see cref="SysConfigService"/>类型的新实例
        /// </summary>
        /// <param name="sysConfigRepository"></param>
        /// <param name="userRepository"></param>
        public SysConfigService(ISqlSugarRepository<SysConfigEntity> sysConfigRepository,
            ISqlSugarRepository<UserEntity> userRepository)
        {
            _sysConfigRepository = sysConfigRepository;
            _userRepository = userRepository;
            Db = DbScoped.SugarScope;
        }

        #region GET

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<SysConfigOutput> GetInfo()
        {
            var array = new Dictionary<string, string>();
            var data = await _sysConfigRepository.AsQueryable().Where(x => x.Category.Equals("SysConfig")).ToListAsync();
            foreach (var item in data)
            {
                array.Add(item.Key, item.Value);
            }
            var output = array.Serialize().Deserialize<SysConfigOutput>();
            return output;
        }

        /// <summary>
        /// 获取所有超级管理员
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAdminList")]
        public async Task<List<AdminUserListOutput>> GetAdminList()
        {
            var output = await _userRepository.AsQueryable()
                .Where(x => x.EnabledMark == 1 && x.DeleteMark == null)
                .Where(x => x.IsAdministrator == 1)
                .Select(x => new AdminUserListOutput()
                {
                    id = x.Id,
                    account = x.Account,
                    realName = x.RealName
                }).ToListAsync();

            return output;
        }

        #endregion

        #region Post

        /// <summary>
        /// 邮箱链接测试
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Email/Test")]
        public void EmailTest([FromBody] SysConfigEmailTestInput input)
        {
            var mailAccount = input.Adapt<MailAccount>();
            if ("1".Equals(input.ssl))
            {
                mailAccount.Ssl = true;
            }
            else
            {
                mailAccount.Ssl = false;
            }
            var result = Mail.CheckConnected(mailAccount);
            if (!result)
                throw HSZException.Oh(ErrorCode.D9003);
        }

        /// <summary>
        /// 钉钉链接测试
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("testDingTalkConnect")]
        public void testDingTalkConnect([FromBody] SysConfigDingTestInput input)
        {
            var dingUtil = new Ding(input.dingSynAppKey, input.dingSynAppSecret);
            if (string.IsNullOrEmpty(dingUtil.token))
                throw HSZException.Oh(ErrorCode.D9003);
        }

        /// <summary>
        /// 企业微信链接测试
        /// </summary>
        /// <param name="type"></param>
        /// <param name="input"></param>
        [HttpPost("{type}/testQyWebChatConnect")]
        public void testQyWebChatConnect(int type, [FromBody] SysConfigWeChatTestInput input)
        {
            var weChatUtil = new WeChat(input.qyhCorpId, input.qyhCorpSecret);
            if (string.IsNullOrEmpty(weChatUtil.accessToken))
                throw HSZException.Oh(ErrorCode.D9003);
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task Update([FromBody] SysConfigUpInput input)
        {
            var configDic = input.Adapt<Dictionary<string, object>>();
            var entitys = new List<SysConfigEntity>();
            foreach (var item in configDic.Keys)
            {
                if (configDic[item] != null)
                {
                    if (item == "tokentimeout")
                    {
                        long time = 0;
                        if (long.TryParse(configDic[item].ToString(), out time))
                        {
                            if (time > 8000000) throw HSZException.Oh(ErrorCode.D9008);
                        }
                    }
                    if (item == "verificationCodeNumber")
                    {
                        int codeNum = 3;
                        if (int.TryParse(configDic[item].ToString(), out codeNum))
                        {
                            if (codeNum > 6 || codeNum < 3) throw HSZException.Oh(ErrorCode.D9009);
                        }
                    }
                    SysConfigEntity sysConfigEntity = new SysConfigEntity();
                    sysConfigEntity.Id = YitIdHelper.NextId().ToString();
                    sysConfigEntity.Key = item;
                    sysConfigEntity.Value = configDic[item].ToString();
                    sysConfigEntity.Category = "SysConfig";
                    entitys.Add(sysConfigEntity);
                }
            }
            await Save(entitys, "SysConfig");
        }

        /// <summary>
        /// 更新赋予超级管理员
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("setAdminList")]
        public async Task SetAdminList([FromBody] SetAdminListInput input)
        {
            await _userRepository.AsUpdateable().SetColumns(x => x.IsAdministrator == 0).Where(x => x.IsAdministrator == 1&&!x.Id.Equals("admin")).ExecuteCommandAsync();
            await _userRepository.AsUpdateable().SetColumns(x => x.IsAdministrator == 1).Where(x => input.adminIds.Contains(x.Id)).ExecuteCommandAsync();
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 系统配置信息
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<SysConfigEntity> GetInfo(string category, string key)
        {
            return await _sysConfigRepository.GetFirstAsync(s => s.Category == category && s.Key == key);
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private async Task Save(List<SysConfigEntity> entitys, string category)
        {
            try
            {
                Db.BeginTran();
                await _sysConfigRepository.DeleteAsync(x => x.Category.Equals(category));
                await _sysConfigRepository.AsInsertable(entitys).ExecuteCommandAsync();
                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
            }
        }

        #endregion
    }
}
