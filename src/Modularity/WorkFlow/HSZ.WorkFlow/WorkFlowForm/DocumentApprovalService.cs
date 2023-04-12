using HSZ.Common.FileManage;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.JsonSerialization;
using HSZ.System.Interfaces.System;
using HSZ.WorkFlow.Entitys;
using HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.DocumentApproval;
using HSZ.WorkFlow.Interfaces.FlowTask;
using HSZ.WorkFlow.Interfaces.WorkFlowForm;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.WorkFlow.WorkFlowForm
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：文件签批意见表
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowForm", Name = "DocumentApproval", Order = 511)]
    [Route("api/workflow/Form/[controller]")]
    public class DocumentApprovalService : IDocumentApprovalService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DocumentApprovalEntity> _sqlSugarRepository;
        private readonly IBillRullService _billRuleService;
        private readonly IFlowTaskService _flowTaskService;
        private readonly SqlSugarScope Db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlSugarRepository"></param>
        /// <param name="billRuleService"></param>
        /// <param name="flowTaskService"></param>
        public DocumentApprovalService(ISqlSugarRepository<DocumentApprovalEntity> sqlSugarRepository, IBillRullService billRuleService, 
            IFlowTaskService flowTaskService)
        {
            _sqlSugarRepository = sqlSugarRepository;
            _billRuleService = billRuleService;
            _flowTaskService = flowTaskService;
            Db = DbScoped.SugarScope;
        }

        #region GET
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = (await _sqlSugarRepository.GetFirstAsync(x => x.Id == id)).Adapt<DocumentApprovalInfoOutput>();
            return data;
        }
        #endregion

        #region POST
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="input">表单信息</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Save([FromBody] DocumentApprovalCrInput input)
        {
            var entity = input.Adapt<DocumentApprovalEntity>();
            if (input.status == 1)
            {
                await Save(entity.Id, entity);
            }
            else
            {
                await Submit(entity.Id, entity, input.candidateList);
            }

        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="id">表单信息</param>
        /// <param name="input">表单信息</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Submit(string id, [FromBody] DocumentApprovalUpInput input)
        {
            input.id = id;
            var entity = input.Adapt<DocumentApprovalEntity>();
            if (input.status == 1)
            {
                await Save(entity.Id, entity);
            }
            else
            {
                await Submit(entity.Id, entity, input.candidateList);
            }

        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task Save(string id, DocumentApprovalEntity entity, int type = 0)
        {
            try
            {
                Db.BeginTran();

                #region 表单信息
                await HandleForm(id, entity);
                #endregion

                #region 流程信息
                await _flowTaskService.Save(id, entity.FlowId, entity.Id, entity.FlowTitle, entity.FlowUrgent, entity.BillNo, null, 1, type, true);
                #endregion

                Db.CommitTran();

            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="entity">实体对象</param>
        private async Task Submit(string id, DocumentApprovalEntity entity, Dictionary<string, List<string>> candidateList)
        {
            try
            {
                Db.BeginTran();

                #region 表单信息
                await HandleForm(id, entity);
                #endregion

                #region 流程信息
                await _flowTaskService.Submit(id, entity.FlowId, entity.Id, entity.FlowTitle, entity.FlowUrgent, entity.BillNo, entity.Adapt<DocumentApprovalUpInput>(), 0, 0, true,false, candidateList);
                #endregion

                Db.CommitTran();

            }
            catch (Exception ex)
            {
                Db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 表单操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task HandleForm(string id, DocumentApprovalEntity entity)
        {
            if (string.IsNullOrEmpty(id))
            {
                entity.Id = YitIdHelper.NextId().ToString();
                await _sqlSugarRepository.InsertAsync(entity);
                _billRuleService.UseBillNumber("WF_DocumentApprovalNo");
                FileHelper.CreateFile(JsonHelper.ToList<FileModel>(entity.FileJson));
            }
            else
            {
                entity.Id = id;
                await _sqlSugarRepository.UpdateAsync(entity);
                FileHelper.UpdateFile(JsonHelper.ToList<FileModel>(entity.FileJson));
            }
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 工作流表单操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <param name="type">0：事前审批，1：创建子流程</param>
        /// <returns></returns>
        [NonAction]
        public async Task Save(string id, object obj, int type)
        {
            try
            {
                var input = obj.Serialize().Deserialize<DocumentApprovalUpInput>();
                var entity = input.Adapt<DocumentApprovalEntity>();
                if (type == 0)
                {
                    await this.HandleForm(id, entity);
                }
                else
                {
                    entity.Id = id;
                    await _sqlSugarRepository.InsertAsync(entity);
                }

            }
            catch (Exception e)
            {

                throw;
            }
        }
        #endregion
    }
}
