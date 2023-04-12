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
using HSZ.wms.Entitys.Dto.ZjnRecordMaterialInventory;
using HSZ.wms.Interfaces.ZjnRecordMaterialInventory;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnRecordMaterialInventory
{
    /// <summary>
    /// 库存流水服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnRecordMaterialInventory", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnRecordMaterialInventoryService : IZjnRecordMaterialInventoryService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnRecordMaterialInventoryEntity> _zjnRecordMaterialInventoryRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnRecordMaterialInventoryService"/>类型的新实例
        /// </summary>
        public ZjnRecordMaterialInventoryService(ISqlSugarRepository<ZjnRecordMaterialInventoryEntity> zjnRecordMaterialInventoryRepository,
            IUserManager userManager)
        {
            _zjnRecordMaterialInventoryRepository = zjnRecordMaterialInventoryRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取库存流水
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnRecordMaterialInventoryRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnRecordMaterialInventoryInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取库存流水列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnRecordMaterialInventoryListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Batch" : input.sidx;
            var data = await _zjnRecordMaterialInventoryRepository.AsSugarClient().Queryable<ZjnRecordMaterialInventoryEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .WhereIF(!string.IsNullOrEmpty(input.F_Quality), a => a.Quality.Contains(input.F_Quality))
                .WhereIF(!string.IsNullOrEmpty(input.F_Order), a => a.Order.Contains(input.F_Order))
                .WhereIF(!string.IsNullOrEmpty(input.F_BusinessType), a => a.BusinessType.Contains(input.F_BusinessType))
                .WhereIF(!string.IsNullOrEmpty(input.F_EntryOrder), a => a.EntryOrder.Contains(input.F_EntryOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_OutOrder), a => a.OutOrder.Contains(input.F_OutOrder))
                .Select((a
)=> new ZjnRecordMaterialInventoryListOutput
                {
                    F_Id = a.Id,
                    F_Description = a.Description,
                    F_CreateTime = a.CreateTime,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_Batch = a.Batch,
                    F_Quality = a.Quality,
                    F_Location = a.Location,
                    F_EntryTime = a.EntryTime,
                    F_Order = a.Order,
                    F_BusinessType = a.BusinessType,
                    F_EntryOrder = a.EntryOrder,
                    F_OutOrder = a.OutOrder,
                    F_Operation = a.Operation,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnRecordMaterialInventoryListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建库存流水
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnRecordMaterialInventoryCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnRecordMaterialInventoryEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnRecordMaterialInventoryRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取库存流水无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnRecordMaterialInventoryListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Batch" : input.sidx;
            var data = await _zjnRecordMaterialInventoryRepository.AsSugarClient().Queryable<ZjnRecordMaterialInventoryEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .WhereIF(!string.IsNullOrEmpty(input.F_Quality), a => a.Quality.Contains(input.F_Quality))
                .WhereIF(!string.IsNullOrEmpty(input.F_Order), a => a.Order.Contains(input.F_Order))
                .WhereIF(!string.IsNullOrEmpty(input.F_BusinessType), a => a.BusinessType.Contains(input.F_BusinessType))
                .WhereIF(!string.IsNullOrEmpty(input.F_EntryOrder), a => a.EntryOrder.Contains(input.F_EntryOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_OutOrder), a => a.OutOrder.Contains(input.F_OutOrder))
                .Select((a
)=> new ZjnRecordMaterialInventoryListOutput
                {
                    F_Id = a.Id,
                    F_Description = a.Description,
                    F_CreateTime = a.CreateTime,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_Batch = a.Batch,
                    F_Quality = a.Quality,
                    F_Location = a.Location,
                    F_EntryTime = a.EntryTime,
                    F_Order = a.Order,
                    F_BusinessType = a.BusinessType,
                    F_EntryOrder = a.EntryOrder,
                    F_OutOrder = a.OutOrder,
                    F_Operation = a.Operation,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出库存流水
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnRecordMaterialInventoryListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnRecordMaterialInventoryListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnRecordMaterialInventoryListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"32位批次号\",\"field\":\"batch\"},{\"value\":\"物料编码\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"物料数量\",\"field\":\"productsQuantity\"},{\"value\":\"质量状态\",\"field\":\"quality\"},{\"value\":\"位置\",\"field\":\"location\"},{\"value\":\"入库时间\",\"field\":\"entryTime\"},{\"value\":\"订单\",\"field\":\"order\"},{\"value\":\"业务类型\",\"field\":\"businessType\"},{\"value\":\"入库单\",\"field\":\"entryOrder\"},{\"value\":\"出库单\",\"field\":\"outOrder\"},{\"value\":\"操作\",\"field\":\"operation\"},{\"value\":\"描述\",\"field\":\"description\"},{\"value\":\"更新时间\",\"field\":\"createTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "库存流水.xls";
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
            ExcelExportHelper<ZjnRecordMaterialInventoryListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除库存流水
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnRecordMaterialInventoryRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除库存流水
                    await _zjnRecordMaterialInventoryRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新库存流水
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnRecordMaterialInventoryUpInput input)
        {
            var entity = input.Adapt<ZjnRecordMaterialInventoryEntity>();
            entity.CreateTime = DateTime.Now;
            var isOk = await _zjnRecordMaterialInventoryRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除库存流水
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnRecordMaterialInventoryRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnRecordMaterialInventoryRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


