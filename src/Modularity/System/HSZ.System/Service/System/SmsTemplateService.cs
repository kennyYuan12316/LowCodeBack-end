using HSZ.Common.Enum;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Expand.Thirdparty.Sms;
using HSZ.Expand.Thirdparty.Sms.Model;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.SmsTemplate;
using HSZ.System.Entitys.Dto.System.SmsTemplate;
using HSZ.System.Entitys.Dto.System.SysConfig;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.System.Service.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：base_sms_template服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "SmsTemplate", Order = 200)]
    [Route("api/system/[controller]")]
    public class SmsTemplateService : ISmsTemplateService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<SmsTemplateEntity> _smsTemplateRepository;
        private readonly ISysConfigService _sysConfigService;

        /// <summary>
        /// 初始化一个<see cref="SmsTemplateService"/>类型的新实例
        /// </summary>
        public SmsTemplateService(ISqlSugarRepository<SmsTemplateEntity> smsTemplateRepository,
            ISysConfigService sysConfigService)
        {
            _smsTemplateRepository = smsTemplateRepository;
            _sysConfigService=sysConfigService;
        }

        #region Get
        /// <summary>
		/// 获取base_sms_template列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] SmsTemplateListQueryInput input)
        {
            var data = await _smsTemplateRepository.AsSugarClient().Queryable<SmsTemplateEntity,UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.CreatorUserId == b.Id))
                .WhereIF(!string.IsNullOrEmpty(input.keyword),
                a => a.TemplateName.Contains(input.keyword) || a.TemplateId.Contains(input.keyword))
                .Where(a => a.DeleteMark == null)
                .OrderBy(a => a.CreatorTime, OrderByType.Desc).OrderByIF(!string.IsNullOrEmpty(input.keyword), a => a.LastModifyTime, OrderByType.Desc)
                .Select((a,b) => new SmsTemplateListOutput
                {
                    id = a.Id,
                    company = SqlFunc.IIF(a.Company==1,"阿里","腾讯"),
                    templateId = a.TemplateId,
                    signContent = a.SignContent,
                    enabledMark = a.EnabledMark,
                    templateName = a.TemplateName,
                    creatorTime = a.CreatorTime,
                    lastModifyTime=a.LastModifyTime,
                    creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                }).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<SmsTemplateListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
		/// 获取base_sms_template列表
		/// </summary>
		/// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector([FromQuery] PageInputBase input)
        {
            var data = await _smsTemplateRepository.AsSugarClient().Queryable<SmsTemplateEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.CreatorUserId == b.Id))
                 .WhereIF(!string.IsNullOrEmpty(input.keyword),
                a => a.TemplateName.Contains(input.keyword) || a.TemplateId.Contains(input.keyword))
                .OrderBy(a => a.CreatorTime, OrderByType.Desc)
                .Select((a,b) => new SmsTemplateListOutput
                {
                    id = a.Id,
                    company = SqlFunc.IIF(a.Company == 1, "阿里", "腾讯"),
                    templateId = a.TemplateId,
                    signContent = a.SignContent,
                    enabledMark = a.EnabledMark,
                    templateName = a.TemplateName,
                    creatorTime = a.CreatorTime,
                    lastModifyTime = a.LastModifyTime,
                    creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                }).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<SmsTemplateListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 获取base_sms_template
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _smsTemplateRepository.GetFirstAsync(p => p.Id == id && p.DeleteMark == null)).Adapt<SmsTemplateInfoOutput>();
            return output;
        }

        /// <summary>
        /// 获取模板字段
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("getTemplate/{id}")]
        public async Task<dynamic> GetTemplate(string id)
        {
            var sysconfig = await _sysConfigService.GetInfo();
            var entity = await _smsTemplateRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            var smsModel = new SmsModel()
            {
                keyId = sysconfig.smsKeyId,
                keySecret = sysconfig.smsKeySecret,
                region = sysconfig.region,
                domain = sysconfig.domain,
                templateId = entity.TemplateId
            };

            if (entity.Company == 0)
            {
                return Sms.GetTemplateByAli(smsModel);
            }
            else
            {
                return Sms.GetTemplateByTencent(smsModel);
            }
        }
        #endregion

        #region Post
        /// <summary>
        /// 新建base_sms_template
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] SmsTemplateCrInput input)
        {
            if (await _smsTemplateRepository.IsAnyAsync(x => x.TemplateId == input.templateId && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<SmsTemplateEntity>();
            var isOk = await _smsTemplateRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新base_sms_template
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] SmsTemplateUpInput input)
        {
            if (await _smsTemplateRepository.IsAnyAsync(x => x.Id != id && x.TemplateId == input.templateId && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<SmsTemplateEntity>();
            var isOk = await _smsTemplateRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除base_sms_template
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _smsTemplateRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _smsTemplateRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
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
            var entity = await _smsTemplateRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await _smsTemplateRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1003);

        }

        /// <summary>
        /// 获取模板字段
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("getTemplate")]
        public async Task<dynamic> GetTemplate([FromBody] SmsTemplateCrInput input)
        {
            var sysconfig = await _sysConfigService.GetInfo();
            var smsModel = new SmsModel()
            {
                keyId = sysconfig.smsKeyId,
                keySecret = sysconfig.smsKeySecret,
                region = sysconfig.region,
                domain= sysconfig.domain,
                templateId=input.templateId
            };

            if (input.company == 1)
            {
                return Sms.GetTemplateByAli(smsModel);
            }
            else
            {
                return Sms.GetTemplateByTencent(smsModel);
            }
        }

        

        /// <summary>
        /// 获取模板字段
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("testSent")]
        public async Task SendTest([FromBody] SmsTemplateSendTestInput input)
        {
            var sysconfig = await _sysConfigService.GetInfo();
            var smsModel = new SmsModel()
            {
                keyId = sysconfig.smsKeyId,
                keySecret = sysconfig.smsKeySecret,
                region = sysconfig.region,
                domain = sysconfig.domain,
                templateId = input.templateId,
                appId=input.appId,
                signName=input.signContent
            };
            var msg = "";
            if (input.company == 1)
            {
                smsModel.mobileAli = input.phoneNumbers;
                smsModel.templateParamAli = input.parameters.ToJson();
                msg= Sms.SendSmsByAli(smsModel);
            }
            else
            {
                smsModel.mobileTx = new string[] { input.phoneNumbers };
                List<string> mList=new List<string>();
                foreach (string data in input.parameters.Values)
                {
                    mList.Add(data);
                }
                smsModel.templateParamTx = mList.ToArray();
                msg= Sms.SendSmsByTencent(smsModel);
            }
            if (msg.Equals("短信发送失败"))
                throw HSZException.Oh(msg);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetSmsTemplateFields(string id)
        {
            var entity= await _smsTemplateRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            var sysconfig = await _sysConfigService.GetInfo();
            var smsModel = new SmsModel()
            {
                keyId = sysconfig.smsKeyId,
                keySecret = sysconfig.smsKeySecret,
                region = sysconfig.region,
                domain = sysconfig.domain,
                templateId = entity.TemplateId
            };

            if (entity.Company == 0)
            {
                return Sms.GetTemplateByAli(smsModel);
            }
            else
            {
                return Sms.GetTemplateByTencent(smsModel);
            }
        }

        /// <summary>
        /// 工作流发送短信
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sysconfig"></param>
        /// <param name="phoneNumbers"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [NonAction]
        public async Task FlowTaskSend(string id,SysConfigOutput sysconfig,List<string> phoneNumbers, Dictionary<string, string> parameters)
        {
            var entity = await _smsTemplateRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            var smsModel = new SmsModel()
            {
                keyId = sysconfig.smsKeyId,
                keySecret = sysconfig.smsKeySecret,
                region = sysconfig.region,
                domain = sysconfig.domain,
                templateId = entity.TemplateId,
                appId = entity.AppId,
                signName = entity.SignContent
            };
            if (entity.Company == 1)
            {
                smsModel.mobileAli = string.Join(",", phoneNumbers);
                smsModel.templateParamAli = parameters.ToJson();
                Sms.SendSmsByAli(smsModel);
            }
            else
            {
                smsModel.mobileTx = phoneNumbers.ToArray();
                List<string> mList = new List<string>();
                var fields= await GetSmsTemplateFields(id);
                foreach (string item in fields)
                {
                    if (parameters.ContainsKey(item))
                    mList.Add(parameters[item]);
                }
                smsModel.templateParamTx = mList.ToArray();
                Sms.SendSmsByTencent(smsModel);
            }
        }

    }
}


