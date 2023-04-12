using HSZ.Common.FileManage;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.JsonSerialization;
using HSZ.System.Interfaces.System;
using HSZ.WorkFlow.Entitys;
using HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.SalesOrder;
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
    /// 描 述：销售订单
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowForm", Name = "SalesOrder", Order = 532)]
    [Route("api/workflow/Form/[controller]")]
    public class SalesOrderService : ISalesOrderService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<SalesOrderEntity> _sqlSugarRepository;
        private readonly ISqlSugarRepository<SalesOrderEntryEntity> _sqlItemSugarRepository;
        private readonly IBillRullService _billRuleService;
        private readonly IFlowTaskService _flowTaskService;
        private readonly SqlSugarScope Db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlSugarRepository"></param>
        /// <param name="billRuleService"></param>
        /// <param name="flowTaskService"></param>
        public SalesOrderService(ISqlSugarRepository<SalesOrderEntity> sqlSugarRepository, ISqlSugarRepository<SalesOrderEntryEntity> sqlItemSugarRepository,
            IBillRullService billRuleService, IFlowTaskService flowTaskService)
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
            var data = (await _sqlSugarRepository.GetFirstAsync(x => x.Id == id)).Adapt<SalesOrderInfoOutput>();
            data.entryList = (await _sqlItemSugarRepository.AsQueryable().Where(x => x.SalesOrderId == id).ToListAsync()).Adapt<List<EntryListItem>>();
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
        public async Task Save([FromBody] SalesOrderCrInput input)
        {
            var entity = input.Adapt<SalesOrderEntity>();
            var entityList = input.entryList.Adapt<List<SalesOrderEntryEntity>>();
            if (input.status == 1)
            {
                await Save(entity.Id, entity, entityList);
            }
            else
            {
                await Submit(entity.Id, entity, entityList, input.candidateList);
            }

        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="id">表单信息</param>
        /// <param name="input">表单信息</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Submit(string id, [FromBody] SalesOrderUpInput input)
        {
            input.id = id;
            var entity = input.Adapt<SalesOrderEntity>();
            var entityList = input.entryList.Adapt<List<SalesOrderEntryEntity>>();
            if (input.status == 1)
            {
                await Save(entity.Id, entity, entityList);
            }
            else
            {
                await Submit(entity.Id, entity, entityList, input.candidateList);
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
        private async Task Save(string id, SalesOrderEntity entity, List<SalesOrderEntryEntity> itemList, int type = 0)
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
        private async Task Submit(string id, SalesOrderEntity entity, List<SalesOrderEntryEntity> itemList, Dictionary<string, List<string>> candidateList)
        {
            try
            {
                Db.BeginTran();

                #region 表单信息
                await HandleForm(id, entity, itemList);
                #endregion

                #region 流程信息
                await _flowTaskService.Submit(id, entity.FlowId, entity.Id, entity.FlowTitle, entity.FlowUrgent, entity.BillNo, entity.Adapt<SalesOrderUpInput>(), 0, 0, true, false, candidateList);
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
        private async Task HandleForm(string id, SalesOrderEntity entity, List<SalesOrderEntryEntity> itemList)
        {
            if (string.IsNullOrEmpty(id))
            {
                entity.Id = YitIdHelper.NextId().ToString();
                foreach (var item in itemList)
                {
                    item.Id = YitIdHelper.NextId().ToString();
                    item.SalesOrderId = entity.Id;
                    item.SortCode = itemList.IndexOf(item);
                }
                await _sqlItemSugarRepository.AsInsertable(itemList).ExecuteCommandAsync();
                await _sqlSugarRepository.InsertAsync(entity);
                _billRuleService.UseBillNumber("WF_SalesOrderNo");
                FileHelper.CreateFile(JsonHelper.ToList<FileModel>(entity.FileJson));
            }
            else
            {
                entity.Id = id;
                foreach (var item in itemList)
                {
                    item.Id = YitIdHelper.NextId().ToString();
                    item.SalesOrderId = entity.Id;
                    item.SortCode = itemList.IndexOf(item);
                }
                await _sqlItemSugarRepository.DeleteAsync(x => x.SalesOrderId == id);
                await _sqlItemSugarRepository.AsInsertable(itemList).ExecuteCommandAsync();
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
                var input = obj.Serialize().Deserialize<SalesOrderUpInput>();
                var entity = input.Adapt<SalesOrderEntity>();
                var entityList = input.entryList.Adapt<List<SalesOrderEntryEntity>>();
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
