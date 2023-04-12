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
using HSZ.wms.Entitys.Dto.ZjnPlaneMaterialInventory;
using HSZ.wms.Interfaces.ZjnPlaneMaterialInventory;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnPlaneMaterialInventory
{
    /// <summary>
    /// 库存信息数据源服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnPlaneMaterialInventory", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnPlaneMaterialInventoryService : IZjnPlaneMaterialInventoryService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnPlaneMaterialInventoryEntity> _zjnPlaneMaterialInventoryRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnPlaneMaterialInventoryService"/>类型的新实例
        /// </summary>
        public ZjnPlaneMaterialInventoryService(ISqlSugarRepository<ZjnPlaneMaterialInventoryEntity> zjnPlaneMaterialInventoryRepository,
            IUserManager userManager)
        {
            _zjnPlaneMaterialInventoryRepository = zjnPlaneMaterialInventoryRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取库存信息数据源
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnPlaneMaterialInventoryRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnPlaneMaterialInventoryInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取库存信息数据源列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnPlaneMaterialInventoryListQueryInput input)
        {
            var sidx = input.sidx == null ? "a.F_Id" : input.sidx;
            var data = await _zjnPlaneMaterialInventoryRepository.AsSugarClient().Queryable<ZjnPlaneMaterialInventoryEntity>()
                .LeftJoin<ZjnBaseCustomerEntity>((a, b) => a.ProductsCustomer == b.CustomerNo)
                .LeftJoin<ZjnWmsSupplierEntity>((a, b, c) => a.ProductsSupplier == c.SupplierNo)
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsStyle), a => a.ProductsStyle.Contains(input.F_ProductsStyle))
                .Select((a,b,c)=> new ZjnPlaneMaterialInventoryListOutput
                {
                    F_Id = a.Id,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_ProductsType = a.ProductsType,
                    F_ProductsStyle = a.ProductsStyle,
                    F_ProductsGrade = a.ProductsGrade,
                    F_ProductsState = a.ProductsState,
                    F_ProductsBatch = a.ProductsBatch,
                    F_ProductsLocation = a.ProductsLocation,
                    F_ProductsCustomer = b.CustomerName,
                    F_ProductsSupplier = c.SupplierName,
                    F_ProductsCheckType = a.ProductsCheckType,
                    F_ProductsIsLock = a.ProductsIsLock,
                    F_ProductsTakeStockTime = a.ProductsTakeStockTime,
                    F_ProductsTakeCount = a.ProductsTakeCount,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnPlaneMaterialInventoryListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建库存信息数据源
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnPlaneMaterialInventoryCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnPlaneMaterialInventoryEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnPlaneMaterialInventoryRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新库存信息数据源
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnPlaneMaterialInventoryUpInput input)
        {
            var entity = input.Adapt<ZjnPlaneMaterialInventoryEntity>();
            var isOk = await _zjnPlaneMaterialInventoryRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除库存信息数据源
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnPlaneMaterialInventoryRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnPlaneMaterialInventoryRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


