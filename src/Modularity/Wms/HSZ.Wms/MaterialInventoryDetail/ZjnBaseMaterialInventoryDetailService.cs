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
using HSZ.wms.Entitys.Dto.ZjnBaseMaterialInventoryDetail;
using HSZ.wms.Interfaces.ZjnBaseMaterialInventoryDetail;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnBaseMaterialInventoryDetail
{
    /// <summary>
    /// 库存明细服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseMaterialInventoryDetail", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseMaterialInventoryDetailService : IZjnBaseMaterialInventoryDetailService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseMaterialInventoryDetailEntity> _zjnBaseMaterialInventoryDetailRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseMaterialInventoryDetailService"/>类型的新实例
        /// </summary>
        public ZjnBaseMaterialInventoryDetailService(ISqlSugarRepository<ZjnBaseMaterialInventoryDetailEntity> zjnBaseMaterialInventoryDetailRepository,
            IUserManager userManager)
        {
            _zjnBaseMaterialInventoryDetailRepository = zjnBaseMaterialInventoryDetailRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取库存明细
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseMaterialInventoryDetailRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseMaterialInventoryDetailInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取库存明细列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseMaterialInventoryDetailListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Batch" : input.sidx;
            List<string> queryEntryTime = input.F_EntryTime != null ? input.F_EntryTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startEntryTime = queryEntryTime != null ? Ext.GetDateTime(queryEntryTime.First()) : null;
            DateTime? endEntryTime = queryEntryTime != null ? Ext.GetDateTime(queryEntryTime.Last()) : null;
            var data = await _zjnBaseMaterialInventoryDetailRepository.AsSugarClient().Queryable<ZjnBaseMaterialInventoryDetailEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .WhereIF(!string.IsNullOrEmpty(input.F_Quality), a => a.Quality.Contains(input.F_Quality))
                .WhereIF(!string.IsNullOrEmpty(input.F_Freeze), a => a.Freeze.Contains(input.F_Freeze))
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouse), a => a.WareHouse.Contains(input.F_WareHouse))
                .WhereIF(queryEntryTime != null, a => a.EntryTime >= new DateTime(startEntryTime.ToDate().Year, startEntryTime.ToDate().Month, startEntryTime.ToDate().Day, startEntryTime.ToDate().Hour, startEntryTime.ToDate().Minute, startEntryTime.ToDate().Second))
                .WhereIF(queryEntryTime != null, a => a.EntryTime <= new DateTime(endEntryTime.ToDate().Year, endEntryTime.ToDate().Month, endEntryTime.ToDate().Day, endEntryTime.ToDate().Hour, endEntryTime.ToDate().Minute, endEntryTime.ToDate().Second))
                .Select((a
)=> new ZjnBaseMaterialInventoryDetailListOutput
                {
                    F_Id = a.Id,
                    F_Description = a.Description,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_Batch = a.Batch,
                    F_Quality = a.Quality,
                    F_Freeze = a.Freeze,
                    F_Location = a.Location,
                    F_LocationName = a.LocationName,
                    F_WareHouse = a.WareHouse,
                    F_Tray = a.Tray,
                    F_EntryTime = a.EntryTime,
                    F_Label = a.Label,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseMaterialInventoryDetailListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建库存明细
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseMaterialInventoryDetailCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseMaterialInventoryDetailEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnBaseMaterialInventoryDetailRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取库存明细无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnBaseMaterialInventoryDetailListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Batch" : input.sidx;
            List<string> queryEntryTime = input.F_EntryTime != null ? input.F_EntryTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startEntryTime = queryEntryTime != null ? Ext.GetDateTime(queryEntryTime.First()) : null;
            DateTime? endEntryTime = queryEntryTime != null ? Ext.GetDateTime(queryEntryTime.Last()) : null;
            var data = await _zjnBaseMaterialInventoryDetailRepository.AsSugarClient().Queryable<ZjnBaseMaterialInventoryDetailEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .WhereIF(!string.IsNullOrEmpty(input.F_Quality), a => a.Quality.Contains(input.F_Quality))
                .WhereIF(!string.IsNullOrEmpty(input.F_Freeze), a => a.Freeze.Contains(input.F_Freeze))
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouse), a => a.WareHouse.Contains(input.F_WareHouse))
                .WhereIF(queryEntryTime != null, a => a.EntryTime >= new DateTime(startEntryTime.ToDate().Year, startEntryTime.ToDate().Month, startEntryTime.ToDate().Day, startEntryTime.ToDate().Hour, startEntryTime.ToDate().Minute, startEntryTime.ToDate().Second))
                .WhereIF(queryEntryTime != null, a => a.EntryTime <= new DateTime(endEntryTime.ToDate().Year, endEntryTime.ToDate().Month, endEntryTime.ToDate().Day, endEntryTime.ToDate().Hour, endEntryTime.ToDate().Minute, endEntryTime.ToDate().Second))
                .Select((a
)=> new ZjnBaseMaterialInventoryDetailListOutput
                {
                    F_Id = a.Id,
                    F_Description = a.Description,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_Batch = a.Batch,
                    F_Quality = a.Quality,
                    F_Freeze = a.Freeze,
                    F_Location = a.Location,
                    F_LocationName = a.LocationName,
                    F_WareHouse = a.WareHouse,
                    F_Tray = a.Tray,
                    F_EntryTime = a.EntryTime,
                    F_Label = a.Label,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出库存明细
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnBaseMaterialInventoryDetailListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnBaseMaterialInventoryDetailListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnBaseMaterialInventoryDetailListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"32位批次号\",\"field\":\"batch\"},{\"value\":\"物料编码\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"物料数量\",\"field\":\"productsQuantity\"},{\"value\":\"物料单位\",\"field\":\"productsUnit\"},{\"value\":\"质量状态\",\"field\":\"quality\"},{\"value\":\"是否冻结\",\"field\":\"freeze\"},{\"value\":\"位置\",\"field\":\"location\"},{\"value\":\"位置名\",\"field\":\"locationName\"},{\"value\":\"所属仓库\",\"field\":\"wareHouse\"},{\"value\":\"所属托盘\",\"field\":\"tray\"},{\"value\":\"标签条码\",\"field\":\"label\"},{\"value\":\"入库时间\",\"field\":\"entryTime\"},{\"value\":\"描述\",\"field\":\"description\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "库存明细.xls";
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
            ExcelExportHelper<ZjnBaseMaterialInventoryDetailListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除库存明细
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseMaterialInventoryDetailRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除库存明细
                    await _zjnBaseMaterialInventoryDetailRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新库存明细
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseMaterialInventoryDetailUpInput input)
        {
            var entity = input.Adapt<ZjnBaseMaterialInventoryDetailEntity>();
            entity.CreateTime = DateTime.Now;


            var isOk = await _zjnBaseMaterialInventoryDetailRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除库存明细
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseMaterialInventoryDetailRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseMaterialInventoryDetailRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


