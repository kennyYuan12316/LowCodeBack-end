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
using HSZ.wms.Entitys.Dto.ZjnBaseGoodsWarning;
using HSZ.wms.Interfaces.ZjnBaseGoodsWarning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnBaseGoodsWarning
{
    /// <summary>
    /// 物料库存预警服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseGoodsWarning", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseGoodsWarningService : IZjnBaseGoodsWarningService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseGoodsWarningEntity> _zjnBaseGoodsWarningRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseGoodsWarningService"/>类型的新实例
        /// </summary>
        public ZjnBaseGoodsWarningService(ISqlSugarRepository<ZjnBaseGoodsWarningEntity> zjnBaseGoodsWarningRepository,
            IUserManager userManager)
        {
            _zjnBaseGoodsWarningRepository = zjnBaseGoodsWarningRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取物料库存预警
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseGoodsWarningRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseGoodsWarningInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取物料库存预警列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseGoodsWarningListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Batch" : input.sidx;
            List<string> queryEntryTime = input.F_EntryTime != null ? input.F_EntryTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startEntryTime = queryEntryTime != null ? Ext.GetDateTime(queryEntryTime.First()) : null;
            DateTime? endEntryTime = queryEntryTime != null ? Ext.GetDateTime(queryEntryTime.Last()) : null;
            List<string> queryFailureTime = input.F_FailureTime != null ? input.F_FailureTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startFailureTime = queryFailureTime != null ? Ext.GetDateTime(queryFailureTime.First()) : null;
            DateTime? endFailureTime = queryFailureTime != null ? Ext.GetDateTime(queryFailureTime.Last()) : null;
            List<string> queryWarningTime = input.F_WarningTime != null ? input.F_WarningTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startWarningTime = queryWarningTime != null ? Ext.GetDateTime(queryWarningTime.First()) : null;
            DateTime? endWarningTime = queryWarningTime != null ? Ext.GetDateTime(queryWarningTime.Last()) : null;
            var data = await _zjnBaseGoodsWarningRepository.AsSugarClient().Queryable<ZjnBaseGoodsWarningEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .WhereIF(queryEntryTime != null, a => a.EntryTime >= new DateTime(startEntryTime.ToDate().Year, startEntryTime.ToDate().Month, startEntryTime.ToDate().Day, startEntryTime.ToDate().Hour, startEntryTime.ToDate().Minute, startEntryTime.ToDate().Second))
                .WhereIF(queryEntryTime != null, a => a.EntryTime <= new DateTime(endEntryTime.ToDate().Year, endEntryTime.ToDate().Month, endEntryTime.ToDate().Day, endEntryTime.ToDate().Hour, endEntryTime.ToDate().Minute, endEntryTime.ToDate().Second))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsType), a => a.ProductsType.Contains(input.F_ProductsType))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsSupplier), a => a.ProductsSupplier.Contains(input.F_ProductsSupplier))
                .WhereIF(queryFailureTime != null, a => a.FailureTime >= new DateTime(startFailureTime.ToDate().Year, startFailureTime.ToDate().Month, startFailureTime.ToDate().Day, startFailureTime.ToDate().Hour, startFailureTime.ToDate().Minute, startFailureTime.ToDate().Second))
                .WhereIF(queryFailureTime != null, a => a.FailureTime <= new DateTime(endFailureTime.ToDate().Year, endFailureTime.ToDate().Month, endFailureTime.ToDate().Day, endFailureTime.ToDate().Hour, endFailureTime.ToDate().Minute, endFailureTime.ToDate().Second))
                .WhereIF(queryWarningTime != null, a => a.WarningTime >= new DateTime(startWarningTime.ToDate().Year, startWarningTime.ToDate().Month, startWarningTime.ToDate().Day, startWarningTime.ToDate().Hour, startWarningTime.ToDate().Minute, startWarningTime.ToDate().Second))
                .WhereIF(queryWarningTime != null, a => a.WarningTime <= new DateTime(endWarningTime.ToDate().Year, endWarningTime.ToDate().Month, endWarningTime.ToDate().Day, endWarningTime.ToDate().Hour, endWarningTime.ToDate().Minute, endWarningTime.ToDate().Second))
                .Select((a
)=> new ZjnBaseGoodsWarningListOutput
                {
                    F_Id = a.Id,
                    F_Description = a.Description,
                    F_CreateTime = a.CreateTime,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_Batch = a.Batch,
                    F_EntryTime = a.EntryTime,
                    F_ProductsType = a.ProductsType,
                    F_ProductsSupplier = a.ProductsSupplier,
                    F_InventoryMax = a.InventoryMax,
                    F_InventoryMin = a.InventoryMin,
                    F_WarningResult = a.WarningResult,
                    F_ExpirationDate = a.ExpirationDate,
                    F_ProductionTime = a.ProductionTime,
                    F_FailureTime = a.FailureTime,
                    F_WarningCycle = a.WarningCycle,
                    F_WarningTime = a.WarningTime,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseGoodsWarningListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建物料库存预警
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseGoodsWarningCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseGoodsWarningEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnBaseGoodsWarningRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新物料库存预警
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseGoodsWarningUpInput input)
        {
            var entity = input.Adapt<ZjnBaseGoodsWarningEntity>();
            entity.CreateTime = DateTime.Now;
            var isOk = await _zjnBaseGoodsWarningRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除物料库存预警
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseGoodsWarningRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseGoodsWarningRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


