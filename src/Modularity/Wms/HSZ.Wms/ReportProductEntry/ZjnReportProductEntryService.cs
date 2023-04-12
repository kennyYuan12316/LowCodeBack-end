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
using HSZ.wms.Entitys.Dto.ZjnReportProductEntry;
using HSZ.wms.Interfaces.ZjnReportProductEntry;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnReportProductEntry
{
    /// <summary>
    /// 成品入库单服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnReportProductEntry", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnReportProductEntryService : IZjnReportProductEntryService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnReportProductEntryEntity> _zjnReportProductEntryRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnReportProductEntryService"/>类型的新实例
        /// </summary>
        public ZjnReportProductEntryService(ISqlSugarRepository<ZjnReportProductEntryEntity> zjnReportProductEntryRepository,
            IUserManager userManager)
        {
            _zjnReportProductEntryRepository = zjnReportProductEntryRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取成品入库单
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnReportProductEntryRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnReportProductEntryInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取成品入库单列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnReportProductEntryListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_EntryOrder" : input.sidx;
            var data = await _zjnReportProductEntryRepository.AsSugarClient().Queryable<ZjnReportProductEntryEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_Tray), a => a.Tray.Contains(input.F_Tray))
                .WhereIF(!string.IsNullOrEmpty(input.F_BatteryCode), a => a.BatteryCode.Contains(input.F_BatteryCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductionOrder), a => a.ProductionOrder.Contains(input.F_ProductionOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_EntryOrder), a => a.EntryOrder.Contains(input.F_EntryOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_BusinessType), a => a.BusinessType.Contains(input.F_BusinessType))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .Select((a
)=> new ZjnReportProductEntryListOutput
                {
                    F_Id = a.Id,
                    F_Tray = a.Tray,
                    F_BatteryCode = a.BatteryCode,
                    F_ProductionOrder = a.ProductionOrder,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductionTime = a.ProductionTime,
                    F_EntryOrder = a.EntryOrder,
                    F_ComboTime = a.ComboTime,
                    F_EntryTime = a.EntryTime,
                    F_BusinessType = a.BusinessType,
                    F_Batch = a.Batch,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_Quality = a.Quality,
                    F_ProductionMark = a.ProductionMark,
                    F_InventoryMark = a.InventoryMark,
                    F_ClassMark = a.ClassMark,
                    F_Voltage = a.Voltage,
                    F_Ah = a.Ah,
                    F_ACR = a.Acr,
                    F_DCR = a.Dcr,
                    F_KValue = a.KValue,
                    F_LineNum = a.LineNum,
                    F_ChannelNum = a.ChannelNum,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnReportProductEntryListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建成品入库单
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnReportProductEntryCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnReportProductEntryEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnReportProductEntryRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取成品入库单无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnReportProductEntryListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_EntryOrder" : input.sidx;
            var data = await _zjnReportProductEntryRepository.AsSugarClient().Queryable<ZjnReportProductEntryEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_Tray), a => a.Tray.Contains(input.F_Tray))
                .WhereIF(!string.IsNullOrEmpty(input.F_BatteryCode), a => a.BatteryCode.Contains(input.F_BatteryCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductionOrder), a => a.ProductionOrder.Contains(input.F_ProductionOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_EntryOrder), a => a.EntryOrder.Contains(input.F_EntryOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_BusinessType), a => a.BusinessType.Contains(input.F_BusinessType))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .Select((a
)=> new ZjnReportProductEntryListOutput
                {
                    F_Id = a.Id,
                    F_Tray = a.Tray,
                    F_BatteryCode = a.BatteryCode,
                    F_ProductionOrder = a.ProductionOrder,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductionTime = a.ProductionTime,
                    F_EntryOrder = a.EntryOrder,
                    F_ComboTime = a.ComboTime,
                    F_EntryTime = a.EntryTime,
                    F_BusinessType = a.BusinessType,
                    F_Batch = a.Batch,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_Quality = a.Quality,
                    F_ProductionMark = a.ProductionMark,
                    F_InventoryMark = a.InventoryMark,
                    F_ClassMark = a.ClassMark,
                    F_Voltage = a.Voltage,
                    F_Ah = a.Ah,
                    F_ACR = a.Acr,
                    F_DCR = a.Dcr,
                    F_KValue = a.KValue,
                    F_LineNum = a.LineNum,
                    F_ChannelNum = a.ChannelNum,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出成品入库单
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnReportProductEntryListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnReportProductEntryListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnReportProductEntryListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"入库单\",\"field\":\"entryOrder\"},{\"value\":\"所属托盘\",\"field\":\"tray\"},{\"value\":\"电芯条码\",\"field\":\"batteryCode\"},{\"value\":\"物料编码\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"生产单号\",\"field\":\"productionOrder\"},{\"value\":\"生产时间\",\"field\":\"productionTime\"},{\"value\":\"组盘时间\",\"field\":\"comboTime\"},{\"value\":\"入库时间\",\"field\":\"entryTime\"},{\"value\":\"业务类型\",\"field\":\"businessType\"},{\"value\":\"32位批次号\",\"field\":\"batch\"},{\"value\":\"物料数量\",\"field\":\"productsQuantity\"},{\"value\":\"物料单位\",\"field\":\"productsUnit\"},{\"value\":\"质量状态\",\"field\":\"quality\"},{\"value\":\"产品标识\",\"field\":\"productionMark\"},{\"value\":\"库存标识\",\"field\":\"inventoryMark\"},{\"value\":\"等级标识\",\"field\":\"classMark\"},{\"value\":\"电压\",\"field\":\"voltage\"},{\"value\":\"安时\",\"field\":\"ah\"},{\"value\":\"交流电阻\",\"field\":\"acr\"},{\"value\":\"直流电阻\",\"field\":\"dcr\"},{\"value\":\"K值\",\"field\":\"kValue\"},{\"value\":\"通道号\",\"field\":\"channelNum\"},{\"value\":\"生产线\",\"field\":\"lineNum\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "成品入库单.xls";
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
            ExcelExportHelper<ZjnReportProductEntryListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除成品入库单
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnReportProductEntryRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除成品入库单
                    await _zjnReportProductEntryRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新成品入库单
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnReportProductEntryUpInput input)
        {
            var entity = input.Adapt<ZjnReportProductEntryEntity>();
            var isOk = await _zjnReportProductEntryRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除成品入库单
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnReportProductEntryRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnReportProductEntryRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


