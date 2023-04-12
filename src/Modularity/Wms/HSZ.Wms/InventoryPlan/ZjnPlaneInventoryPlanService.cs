using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.wms.Entitys.Dto.ZjnPlaneInventoryPlan;
using HSZ.wms.Interfaces.ZjnPlaneInventoryPlan;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnPlaneInventoryPlan
{
    /// <summary>
    /// 盘点计划服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnPlaneInventoryPlan", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnPlaneInventoryPlanService : IZjnPlaneInventoryPlanService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnPlaneInventoryPlanEntity> _zjnPlaneInventoryPlanRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnPlaneInventoryPlanService"/>类型的新实例
        /// </summary>
        public ZjnPlaneInventoryPlanService(ISqlSugarRepository<ZjnPlaneInventoryPlanEntity> zjnPlaneInventoryPlanRepository,
            IUserManager userManager)
        {
            _zjnPlaneInventoryPlanRepository = zjnPlaneInventoryPlanRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取盘点计划
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnPlaneInventoryPlanRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnPlaneInventoryPlanInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取盘点计划列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnPlaneInventoryPlanListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_InventoryNo" : input.sidx;
            List<string> queryStartTime = input.F_StartTime != null ? input.F_StartTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startStartTime = queryStartTime != null ? Ext.GetDateTime(queryStartTime.First()) : null;
            DateTime? endStartTime = queryStartTime != null ? Ext.GetDateTime(queryStartTime.Last()) : null;
            List<string> queryEndTime = input.F_EndTime != null ? input.F_EndTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startEndTime = queryEndTime != null ? Ext.GetDateTime(queryEndTime.First()) : null;
            DateTime? endEndTime = queryEndTime != null ? Ext.GetDateTime(queryEndTime.Last()) : null;
            var data = await _zjnPlaneInventoryPlanRepository.AsSugarClient().Queryable<ZjnPlaneInventoryPlanEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_InventoryNo), a => a.InventoryNo.Contains(input.F_InventoryNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_InventoryType), a => a.InventoryType.Equals(input.F_InventoryType))
                .WhereIF(queryStartTime != null, a => a.StartTime >= new DateTime(startStartTime.ToDate().Year, startStartTime.ToDate().Month, startStartTime.ToDate().Day, startStartTime.ToDate().Hour, startStartTime.ToDate().Minute, startStartTime.ToDate().Second))
                .WhereIF(queryStartTime != null, a => a.StartTime <= new DateTime(endStartTime.ToDate().Year, endStartTime.ToDate().Month, endStartTime.ToDate().Day, endStartTime.ToDate().Hour, endStartTime.ToDate().Minute, endStartTime.ToDate().Second))
                .WhereIF(queryEndTime != null, a => a.EndTime >= new DateTime(startEndTime.ToDate().Year, startEndTime.ToDate().Month, startEndTime.ToDate().Day, startEndTime.ToDate().Hour, startEndTime.ToDate().Minute, startEndTime.ToDate().Second))
                .WhereIF(queryEndTime != null, a => a.EndTime <= new DateTime(endEndTime.ToDate().Year, endEndTime.ToDate().Month, endEndTime.ToDate().Day, endEndTime.ToDate().Hour, endEndTime.ToDate().Minute, endEndTime.ToDate().Second))
                .Select((a
)=> new ZjnPlaneInventoryPlanListOutput
                {
                    F_Id = a.Id,
                    F_InventoryNo = a.InventoryNo,
                    F_InventoryType = a.InventoryType,
                    F_StartTime = a.StartTime,
                    F_EndTime = a.EndTime,
                    F_Description = a.Description,
                    F_EnabledMark = a.EnabledMark,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_LastModifyUserId = a.LastModifyUserId,
                    F_LastModifyTime = a.LastModifyTime,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnPlaneInventoryPlanListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建盘点计划
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnPlaneInventoryPlanCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnPlaneInventoryPlanEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            entity.LastModifyUserId= _userManager.UserId;
            entity.LastModifyTime = DateTime.Now;

            var isOk = await _zjnPlaneInventoryPlanRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 批量删除盘点计划
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnPlaneInventoryPlanRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除盘点计划
                    await _zjnPlaneInventoryPlanRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception)
                {
                    //回滚事务
                    _db.RollbackTran();
                    throw HSZException.Oh(ErrorCode.COM1002);
                }
            }
        }

        /// <summary>
        /// 更新盘点计划
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnPlaneInventoryPlanUpInput input)
        {
            var entity = input.Adapt<ZjnPlaneInventoryPlanEntity>();
            entity.LastModifyUserId = _userManager.UserId;
            entity.LastModifyTime = DateTime.Now;
            var isOk = await _zjnPlaneInventoryPlanRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除盘点计划
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnPlaneInventoryPlanRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnPlaneInventoryPlanRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


