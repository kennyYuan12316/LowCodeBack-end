using HSZ.Common.Core.Manager;
using HSZ.ClayObject;
using HSZ.Common.Configuration;
using HSZ.Common.Model.NPOI;
using HSZ.DataEncryption;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.wms.Entitys.Dto.ZjnLimitMaterialInventory;
using HSZ.wms.Interfaces.ZjnLimitMaterialInventory;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnLimitMaterialInventory
{
    /// <summary>
    /// 物料库存上下限服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnLimitMaterialInventory", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnLimitMaterialInventoryService : IZjnLimitMaterialInventoryService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnLimitMaterialInventoryEntity> _zjnLimitMaterialInventoryRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnLimitMaterialInventoryService"/>类型的新实例
        /// </summary>
        public ZjnLimitMaterialInventoryService(ISqlSugarRepository<ZjnLimitMaterialInventoryEntity> zjnLimitMaterialInventoryRepository,
            IUserManager userManager)
        {
            _zjnLimitMaterialInventoryRepository = zjnLimitMaterialInventoryRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取物料库存上下限
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnLimitMaterialInventoryRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnLimitMaterialInventoryInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取物料库存上下限列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnLimitMaterialInventoryListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_ProductsCode" : input.sidx;
            var data = await _zjnLimitMaterialInventoryRepository.AsSugarClient().Queryable<ZjnLimitMaterialInventoryEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouseCode), a => a.WareHouseCode.Contains(input.F_WareHouseCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouse), a => a.WareHouse.Contains(input.F_WareHouse))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .Select((a
)=> new ZjnLimitMaterialInventoryListOutput
                {
                    F_Id = a.Id,
                    F_Description = a.Description,
                    F_CreateTime = a.CreateTime,
                    F_WareHouseCode = a.WareHouseCode,
                    F_WareHouse = a.WareHouse,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_InventoryMax = a.InventoryMax,
                    F_InventoryMin = a.InventoryMin,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnLimitMaterialInventoryListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建物料库存上下限
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnLimitMaterialInventoryCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnLimitMaterialInventoryEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnLimitMaterialInventoryRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取物料库存上下限无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnLimitMaterialInventoryListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_ProductsCode" : input.sidx;
            var data = await _zjnLimitMaterialInventoryRepository.AsSugarClient().Queryable<ZjnLimitMaterialInventoryEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouseCode), a => a.WareHouseCode.Contains(input.F_WareHouseCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouse), a => a.WareHouse.Contains(input.F_WareHouse))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .Select((a
)=> new ZjnLimitMaterialInventoryListOutput
                {
                    F_Id = a.Id,
                    F_Description = a.Description,
                    F_CreateTime = a.CreateTime,
                    F_WareHouseCode = a.WareHouseCode,
                    F_WareHouse = a.WareHouse,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_InventoryMax = a.InventoryMax,
                    F_InventoryMin = a.InventoryMin,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出物料库存上下限
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnLimitMaterialInventoryListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnLimitMaterialInventoryListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnLimitMaterialInventoryListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"物料编码\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"所属仓库编码\",\"field\":\"wareHouseCode\"},{\"value\":\"所属仓库\",\"field\":\"wareHouse\"},{\"value\":\"物料数量\",\"field\":\"productsQuantity\"},{\"value\":\"物料单位\",\"field\":\"productsUnit\"},{\"value\":\"库存上限\",\"field\":\"inventoryMax\"},{\"value\":\"库存下限\",\"field\":\"inventoryMin\"},{\"value\":\"描述\",\"field\":\"description\"},{\"value\":\"更新时间\",\"field\":\"createTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "物料库存上下限.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            List<string> selectKeyList = input.selectKey.Split(',').ToList();
            foreach (var item in selectKeyList)
            {
                var isExist = paramList.Find(p => p.field == item);
                if (isExist != null)
                {
                    excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = isExist.field, ExcelColumn = isExist.value });
                }
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnLimitMaterialInventoryListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除物料库存上下限
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnLimitMaterialInventoryRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除物料库存上下限
                    await _zjnLimitMaterialInventoryRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新物料库存上下限
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnLimitMaterialInventoryUpInput input)
        {
            var entity = input.Adapt<ZjnLimitMaterialInventoryEntity>();
            entity.CreateTime = DateTime.Now;
            var isOk = await _zjnLimitMaterialInventoryRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除物料库存上下限
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnLimitMaterialInventoryRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnLimitMaterialInventoryRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


