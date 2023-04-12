using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Expand.Thirdparty;
using HSZ.Expand.Thirdparty.DingDing;
using HSZ.Expand.Thirdparty.Email;
using HSZ.Expand.Thirdparty.Email.Model;
using HSZ.FriendlyException;
using HSZ.Message.Interfaces.Message;
using HSZ.System.Entitys.Dto.MessageTemplate;
using HSZ.System.Entitys.Dto.System.SysConfig;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Permission;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace HSZ.System.Service.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：base_message_template服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "MessageTemplate", Order = 200)]
    [Route("api/system/[controller]")]
    public class MessageTemplateService : IMessageTemplateService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<MessageTemplateEntity> _messageTemplateRepository;
        private readonly ISmsTemplateService _smsTemplateService;
        private readonly ISysConfigService _sysConfigService;
        private readonly IMessageService _messageService;
        private readonly IUsersService _usersService;
        private readonly ISynThirdInfoService _synThirdInfoService;

        /// <summary>
        /// 初始化一个<see cref="MessageTemplateService"/>类型的新实例
        /// </summary>
        public MessageTemplateService(ISqlSugarRepository<MessageTemplateEntity> messageTemplateRepository,
            ISmsTemplateService smsTemplateService, 
            ISysConfigService sysConfigService,
            IMessageService messageService,
            IUsersService usersService,
            ISynThirdInfoService synThirdInfoService)
        {
            _messageTemplateRepository = messageTemplateRepository;
            _smsTemplateService = smsTemplateService;
            _sysConfigService=sysConfigService;
            _messageService = messageService;
            _usersService = usersService;
            _synThirdInfoService=synThirdInfoService;
        }

        #region Get
        /// <summary>
		/// 获取base_message_template列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] MessageTemplateListQueryInput input)
        {
            var data = await _messageTemplateRepository.AsSugarClient().Queryable<MessageTemplateEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.CreatorUserId == b.Id))
                .WhereIF(!string.IsNullOrEmpty(input.keyword),
                a => a.Category.Contains(input.keyword) || a.FullName.Contains(input.keyword) || a.Title.Contains(input.keyword))
                .Where(a => a.DeleteMark == null)
                .OrderBy(a => a.CreatorTime, OrderByType.Desc).OrderByIF(!string.IsNullOrEmpty(input.keyword), a => a.LastModifyTime, OrderByType.Desc)
                .Select((a,b) => new MessageTemplateListOutput
                    {
                        id = a.Id,
                        category =SqlFunc.IF(a.Category.Equals("1")).Return("普通").ElseIF(a.Category.Equals("2")).Return("重要").End("紧急"),
                        fullName = a.FullName,
                        title = a.Title,
                        content=a.Content,
                        _noticeMethod=SqlFunc.MergeString(SqlFunc.IIF(a.IsDingTalk==1,"钉钉,",""),
                        SqlFunc.IIF(a.IsEmail == 1, "邮箱,", ""),SqlFunc.IIF(a.IsSms == 1, "短信,", ""),
                        SqlFunc.IIF(a.IsStationLetter == 1, "站内信,", ""), SqlFunc.IIF(a.IsWeCom == 1, "企业微信,", "")),
                        enabledMark=a.EnabledMark,
                        creatorTime = a.CreatorTime,
                        lastModifyTime = a.LastModifyTime,
                        creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                }).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<MessageTemplateListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
		/// 获取base_message_template列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector([FromQuery] PageInputBase input)
        {
            var data = await _messageTemplateRepository.AsSugarClient().Queryable<MessageTemplateEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.CreatorUserId == b.Id))
                .WhereIF(!string.IsNullOrEmpty(input.keyword),
                a => a.Category.Contains(input.keyword) || a.FullName.Contains(input.keyword) || a.Title.Contains(input.keyword))
                .Where(a => a.DeleteMark == null&&a.EnabledMark==1)
                .OrderBy(a => a.CreatorTime, OrderByType.Desc)
                .Select((a,b) => new MessageTemplateSeletorOutput
                {
                    id = a.Id,
                    category = SqlFunc.IF(a.Category.Equals("1")).Return("普通").ElseIF(a.Category.Equals("2")).Return("重要").End("紧急"),
                    fullName = a.FullName,
                    title = a.Title,
                    content = a.Content,
                    templateJson=a.TemplateJson,
                    creatorTime = a.CreatorTime,
                    lastModifyTime = a.LastModifyTime,
                    creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                }).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<MessageTemplateSeletorOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 获取base_message_template
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var output =await _messageTemplateRepository.AsSugarClient().Queryable<MessageTemplateEntity, SmsTemplateEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.SmsId == b.Id))
                .Where((a, b) => a.Id == id && a.DeleteMark == null && b.DeleteMark == null).Select((a, b) => new MessageTemplateInfoOutput()
                {
                    id = a.Id,
                    category = a.Category,
                    isDingTalk = a.IsDingTalk,
                    isEmail = a.IsEmail,
                    isSms = a.IsSms,
                    isStationLetter = a.IsStationLetter,
                    isWecom = a.IsWeCom,
                    fullName = a.FullName,
                    enabledMark = a.EnabledMark,
                    title = a.Title,
                    smsTemplateName = b.TemplateName,
                    content = a.Content,
                    smsId=a.SmsId,
                    templateJson = a.TemplateJson,
                }).FirstAsync();
            return output;
        }

        /// <summary>
        /// 获取base_message_template
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("getTemplate/{id}")]
        public async Task<dynamic> GetTemplate(string id)
        {
            var entity = await _messageTemplateRepository.GetFirstAsync(p => p.Id == id && p.DeleteMark == null);
            var smsFields =await _smsTemplateService.GetSmsTemplateFields(entity.SmsId);
            var dic=entity.TemplateJson.ToObject<Dictionary<string, string>>();
            foreach (var item in smsFields)
            {
                dic[item] = "";
            }
            return dic;
        }
        #endregion

        #region Post
        /// <summary>
        /// 新建base_message_template
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] MessageTemplateCrInput input)
        {
            if (await _messageTemplateRepository.IsAnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<MessageTemplateEntity>();
            var isOk = await _messageTemplateRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新base_message_template
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] MessageTemplateUpInput input)
        {
            if (await _messageTemplateRepository.IsAnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<MessageTemplateEntity>();
            var isOk = await _messageTemplateRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除base_message_template
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _messageTemplateRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _messageTemplateRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 修改单据规则状态
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState_Api(string id)
        {
            var entity = await _messageTemplateRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await _messageTemplateRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1003);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<MessageTemplateEntity> GetInfo(string id)
        {
            return await _messageTemplateRepository.GetFirstAsync(x=>x.Id==id&&x.DeleteMark==null);
        }

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="typeList">推送方式</param>
        /// <param name="messageTemplateEntity">标题</param>
        /// <param name="userList">接收用户</param>
        /// <param name="parameters"></param>
        /// <param name="bodyDic"></param>
        /// <returns></returns>
        [NonAction]
        public async Task SendNodeMessage(List<string> typeList, MessageTemplateEntity messageTemplateEntity, List<string> userList, Dictionary<string, string> parameters, Dictionary<string,object> bodyDic)
        {
            var sysconfig = await _sysConfigService.GetInfo();
            var titile = messageTemplateEntity.Title;
            if (typeList.IsNotEmptyOrNull())
            {
                foreach (var item in typeList)
                {
                    if (item.Equals("1"))
                    {
                        await _messageService.SentMessage(userList, titile, messageTemplateEntity.Content, bodyDic);
                    }
                    if (item.Equals("2"))
                    {
                        EmailSend(titile, userList, messageTemplateEntity.Content, sysconfig);
                    }
                    if (item.Equals("3"))
                    {
                        await SmsSend(messageTemplateEntity, userList, parameters, sysconfig);
                    }
                    if (item.Equals("4"))
                    {
                        var dingIds = await _synThirdInfoService.GetThirdIdList(userList, 2, 3);
                        var dingMsg = new { msgtype = "text", text = new { content = titile } }.ToJson();
                        DingWorkMsgModel dingWorkMsgModel = new DingWorkMsgModel()
                        {
                            toUsers = string.Join(",", dingIds),
                            agentId = sysconfig.dingAgentId,
                            msg = dingMsg
                        };
                        new Ding(sysconfig.dingSynAppKey, sysconfig.dingSynAppSecret).SendWorkMsg(dingWorkMsgModel);
                    }
                    if (item.Equals("5"))
                    {
                        var qyIds = await _synThirdInfoService.GetThirdIdList(userList, 1, 3);
                        var weChat = new WeChat(sysconfig.qyhCorpId, sysconfig.qyhAgentSecret);
                        await weChat.SendText(sysconfig.qyhAgentId, titile, string.Join(",", qyIds));
                    }

                }
            }
        }


        #endregion

        #region PrivateMethod
        /// <summary>
        /// 邮箱
        /// </summary>
        /// <param name="titile"></param>
        /// <param name="userList"></param>
        /// <param name="context"></param>
        /// <param name="sysconfig"></param>
        /// <returns></returns>
        private void EmailSend(string titile, List<string> userList, string context, SysConfigOutput sysconfig)
        {
            try
            {
                var emailList = new List<string>();
                foreach (var item in userList)
                {
                    var user = _usersService.GetInfoByUserId(item);
                    if (user.IsNotEmptyOrNull() && user.Email.IsNotEmptyOrNull())
                    {
                        emailList.Add(user.Email);
                    }
                }
                var mailModel = new MailModel();
                mailModel.To = string.Join(",", emailList);
                mailModel.Subject = titile;
                mailModel.BodyText = HttpUtility.HtmlDecode(context);
                Mail.Send(
                    new MailAccount
                    {
                        AccountName = sysconfig.emailSenderName,
                        Account = sysconfig.emailAccount,
                        Password = sysconfig.emailPassword,
                        SMTPHost = sysconfig.emailSmtpHost,
                        SMTPPort = sysconfig.emailSmtpPort.ToInt()
                    }, mailModel);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 短信
        /// </summary>
        /// <param name="messageTemplateEntity"></param>
        /// <param name="userList"></param>
        /// <param name="parameters"></param>
        /// <param name="sysconfig"></param>
        private async Task SmsSend(MessageTemplateEntity messageTemplateEntity, List<string> userList, Dictionary<string, string> parameters, SysConfigOutput sysconfig)
        {
            try
            {
                var telList = new List<string>();
                foreach (var item in userList)
                {
                    var user = _usersService.GetInfoByUserId(item);
                    if (user.IsNotEmptyOrNull() && user.MobilePhone.IsNotEmptyOrNull())
                    {
                        telList.Add("+86" + user.MobilePhone);
                    }
                }
                await _smsTemplateService.FlowTaskSend(messageTemplateEntity.SmsId, sysconfig, telList, parameters);
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}


