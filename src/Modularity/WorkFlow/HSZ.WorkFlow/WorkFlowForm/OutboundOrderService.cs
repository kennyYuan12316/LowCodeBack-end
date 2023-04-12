using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.JsonSerialization;
using HSZ.System.Interfaces.System;
using HSZ.WorkFlow.Entitys;
using HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.OutboundOrder;
using HSZ.WorkFlow.Entitys.Model;
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
    /// 描 述：出库单
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowForm", Name = "OutboundOrder", Order = 521)]
    [Route("api/workflow/Form/[controller]")]
    public class OutboundOrderService : IOutboundOrderService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<OutboundOrderEntity> _sqlSugarRepository;
        private readonly ISqlSugarRepository<OutboundEntryEntity> _sqlItemSugarRepository;
        private readonly IBillRullService _billRuleService;
        private readonly IFlowTaskService _flowTaskService;
        private readonly SqlSugarScope Db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlSugarRepository"></param>
        /// <param name="sqlItemSugarRepository"></param>
        /// <param name="billRuleService"></param>
        /// <param name="flowTaskService"></param>
        public OutboundOrderService(ISqlSugarRepository<OutboundOrderEntity> sqlSugarRepository,
            ISqlSugarRepository<OutboundEntryEntity> sqlItemSugarRepository, IBillRullService billRuleService,
            IFlowTaskService flowTaskService)
        {
            _sqlSugarRepository = sqlSugarRepository;
            _sqlItemSugarRepository = sqlItemSugarRepository;
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
            var data = (await _sqlSugarRepository.GetFirstAsync(x => x.Id == id)).Adapt<OutboundOrderInfoOutput>();
            data.entryList = (await _sqlItemSugarRepository.AsQueryable().Where(x => x.OutboundId == id).ToListAsync()).Adapt<List<EntryListItem>>();
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
        public async Task Save([FromBody] OutboundOrderCrInput input)
        {
            var entity = input.Adapt<OutboundOrderEntity>();
            var itemList = input.entryList.Adapt<List<OutboundEntryEntity>>();
            if (input.status == 1)
            {
                await Save(entity.Id, entity, itemList);
            }
            else
            {
                await Submit(entity.Id, entity, itemList, input.candidateList);
            }

        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="id">表单信息</param>
        /// <param name="input">表单信息</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Submit(string id, [FromBody] OutboundOrderUpInput input)
        {
            input.id = id;
            var entity = input.Adapt<OutboundOrderEntity>();
            var itemList = input.entryList.Adapt<List<OutboundEntryEntity>>();
            if (input.status == 1)
            {
                await Save(entity.Id, entity, itemList);
            }
            else
            {
                await Submit(entity.Id, entity, itemList, input.candidateList);
            }

        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="itemList"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task Save(string id, OutboundOrderEntity entity, List<OutboundEntryEntity> itemList, int type = 0)
        {
            try
            {
                Db.BeginTran();
                #region 表单信息
                await HandleForm(id, entity, itemList);
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
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="itemList"></param>
        /// <returns></returns>
        private async Task Submit(string id, OutboundOrderEntity entity, List<OutboundEntryEntity> itemList, Dictionary<string, List<string>> candidateList)
        {
            try
            {
                Db.BeginTran();
                #region 表单信息
                await HandleForm(id, entity, itemList);
                #endregion

                #region 流程信息
                await _flowTaskService.Submit(id, entity.FlowId, entity.Id, entity.FlowTitle, entity.FlowUrgent, entity.BillNo, entity.Adapt<OutboundOrderUpInput>(), 0, 0, true, false, candidateList);
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
        /// <param name="itemList"></param>
        /// <returns></returns>
        private async Task HandleForm(string id, OutboundOrderEntity entity, List<OutboundEntryEntity> itemList)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    entity.Id = YitIdHelper.NextId().ToString();
                    foreach (var item in itemList)
                    {
                        item.Id = YitIdHelper.NextId().ToString();
                        item.OutboundId = entity.Id;
                        item.SortCode = itemList.IndexOf(item);
                    }
                    await _sqlItemSugarRepository.AsInsertable(itemList).ExecuteCommandAsync();
                    await _sqlSugarRepository.InsertAsync(entity);
                    _billRuleService.UseBillNumber("WF_OutboundOrderNo");
                }
                else
                {
                    entity.Id = id;
                    foreach (var item in itemList)
                    {
                        item.Id = YitIdHelper.NextId().ToString();
                        item.OutboundId = entity.Id;
                        item.SortCode = itemList.IndexOf(item);
                    }
                    await _sqlItemSugarRepository.DeleteAsync(x => x.OutboundId == id);
                    await _sqlItemSugarRepository.AsInsertable(itemList).ExecuteCommandAsync();
                    await _sqlSugarRepository.UpdateAsync(entity);
                }
            }
            catch (Exception ex)
            {

                throw;
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
                var input = obj.Serialize().Deserialize<OutboundOrderUpInput>();
                var entity = input.Adapt<OutboundOrderEntity>();
                var entityList = input.entryList.Adapt<List<OutboundEntryEntity>>();
                if (type == 0)
                {
                    await this.HandleForm(id, entity, entityList);
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
