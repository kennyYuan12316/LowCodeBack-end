using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.WorkFlow.Entitys.Dto.FlowDelegete;
using HSZ.WorkFlow.Entitys;
using HSZ.WorkFlow.Interfaces.FLowDelegate;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.WorkFlow.Core.Service.FLowDelegate
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程委托
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowDelegate", Order = 300)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowDelegateService : IFlowDelegateService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<FlowDelegateEntity> _flowDelegateRepository;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowDelegateRepository"></param>
        /// <param name="userManager"></param>
        public FlowDelegateService(ISqlSugarRepository<FlowDelegateEntity> flowDelegateRepository, IUserManager userManager)
        {
            _flowDelegateRepository = flowDelegateRepository;
            _userManager = userManager;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] PageInputBase input)
        {
            var list = await _flowDelegateRepository.AsQueryable().Where(x => x.CreatorUserId == _userManager.UserId && x.DeleteMark == null)
                .WhereIF(!input.keyword.IsNullOrEmpty(), m => m.FlowName.Contains(input.keyword) || m.FlowCategory.Contains(input.keyword)).OrderBy(t=>t.SortCode)
                .OrderBy(x => x.CreatorTime, OrderByType.Desc).OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.LastModifyTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<FlowDelegeteListOutput>()
            {
                list = list.list.Adapt<List<FlowDelegeteListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<FlowDelegeteListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = (await GetInfo(id)).Adapt<FlowDelegeteInfoOutput>();
            return data;
        }
        #endregion

        #region POST
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await GetInfo(id);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] FlowDelegeteCrInput input)
        {
            if (_userManager.UserId.Equals(input.toUserId))
                throw HSZException.Oh(ErrorCode.WF0001);
            var entity = input.Adapt<FlowDelegateEntity>();
            var isOk = await Create(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] FlowDelegeteUpInput input)
        {
            if (_userManager.UserId.Equals(input.toUserId))
                throw HSZException.Oh(ErrorCode.WF0001);
            var entity = input.Adapt<FlowDelegateEntity>();
            var isOk = await Update(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1001);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(FlowDelegateEntity entity)
        {
            return await _flowDelegateRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(FlowDelegateEntity entity)
        {
            return await _flowDelegateRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取所有委托给当前用户的委托人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetDelegateUserId(string userId,string flowId)
        {
            var list = await _flowDelegateRepository.AsQueryable().Where(x => x.ToUserId == userId&&x.FlowId==flowId && x.EndTime > DateTime.Now && x.DeleteMark == null).OrderBy(o => o.CreatorTime, OrderByType.Desc).ToListAsync();
            return list.Select(x => x.CreatorUserId).ToList();
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowDelegateEntity> GetInfo(string id)
        {
            return await _flowDelegateRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 当前用户所有委托
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowDelegateEntity>> GetList(string userId)
        {
            return await _flowDelegateRepository.AsQueryable().Where(x => x.CreatorUserId == userId && x.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(FlowDelegateEntity entity)
        {
            return await _flowDelegateRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }
        #endregion
    }
}
