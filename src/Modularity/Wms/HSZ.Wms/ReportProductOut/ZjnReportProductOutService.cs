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
using HSZ.wms.Entitys.Dto.ZjnReportProductOut;
using HSZ.wms.Interfaces.ZjnReportProductOut;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnReportProductOut
{
    /// <summary>
    /// 成品出库单服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnReportProductOut", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnReportProductOutService : IZjnReportProductOutService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnReportProductOutEntity> _zjnReportProductOutRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnReportProductOutService"/>类型的新实例
        /// </summary>
        public ZjnReportProductOutService(ISqlSugarRepository<ZjnReportProductOutEntity> zjnReportProductOutRepository,
            IUserManager userManager)
        {
            _zjnReportProductOutRepository = zjnReportProductOutRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取成品出库单
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnReportProductOutRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnReportProductOutInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取成品出库单列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnReportProductOutListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_OutOrder" : input.sidx;
            var data = await _zjnReportProductOutRepository.AsSugarClient().Queryable<ZjnReportProductOutEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_Tray), a => a.Tray.Contains(input.F_Tray))
                .WhereIF(!string.IsNullOrEmpty(input.F_BatteryCode), a => a.BatteryCode.Contains(input.F_BatteryCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_OutOrder), a => a.OutOrder.Contains(input.F_OutOrder))
                .Select((a
)=> new ZjnReportProductOutListOutput
                {
                    F_Id = a.Id,
                    F_Tray = a.Tray,
                    F_BatteryCode = a.BatteryCode,
                    F_ProductionOrder = a.ProductionOrder,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductionTime = a.ProductionTime,
                    F_OutOrder = a.OutOrder,
                    F_BusinessType = a.BusinessType,
                    F_Batch = a.Batch,
                    F_OutTime = a.OutTime,
                    F_OutTimeConfirm = a.OutTimeConfirm,
                    F_OutStation = a.OutStation,
                    F_OutQuantity = a.OutQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_ProductionMark = a.ProductionMark,
                    F_InventoryMark = a.InventoryMark,
                    F_ClassMark = a.ClassMark,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnReportProductOutListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建成品出库单
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnReportProductOutCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnReportProductOutEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnReportProductOutRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取成品出库单无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnReportProductOutListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_OutOrder" : input.sidx;
            var data = await _zjnReportProductOutRepository.AsSugarClient().Queryable<ZjnReportProductOutEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_Tray), a => a.Tray.Contains(input.F_Tray))
                .WhereIF(!string.IsNullOrEmpty(input.F_BatteryCode), a => a.BatteryCode.Contains(input.F_BatteryCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_OutOrder), a => a.OutOrder.Contains(input.F_OutOrder))
                .Select((a
)=> new ZjnReportProductOutListOutput
                {
                    F_Id = a.Id,
                    F_Tray = a.Tray,
                    F_BatteryCode = a.BatteryCode,
                    F_ProductionOrder = a.ProductionOrder,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_ProductionTime = a.ProductionTime,
                    F_OutOrder = a.OutOrder,
                    F_BusinessType = a.BusinessType,
                    F_Batch = a.Batch,
                    F_OutTime = a.OutTime,
                    F_OutTimeConfirm = a.OutTimeConfirm,
                    F_OutStation = a.OutStation,
                    F_OutQuantity = a.OutQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_ProductionMark = a.ProductionMark,
                    F_InventoryMark = a.InventoryMark,
                    F_ClassMark = a.ClassMark,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出成品出库单
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnReportProductOutListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnReportProductOutListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnReportProductOutListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"出库单\",\"field\":\"outOrder\"},{\"value\":\"所属托盘\",\"field\":\"tray\"},{\"value\":\"电芯条码\",\"field\":\"batteryCode\"},{\"value\":\"生产单号\",\"field\":\"productionOrder\"},{\"value\":\"物料编码\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"生产时间\",\"field\":\"productionTime\"},{\"value\":\"业务类型\",\"field\":\"businessType\"},{\"value\":\"32位批次号\",\"field\":\"batch\"},{\"value\":\"出库时间\",\"field\":\"outTime\"},{\"value\":\"出库确认时间\",\"field\":\"outTimeConfirm\"},{\"value\":\"出库站台\",\"field\":\"outStation\"},{\"value\":\"出库数量\",\"field\":\"outQuantity\"},{\"value\":\"物料单位\",\"field\":\"productsUnit\"},{\"value\":\"产品标识\",\"field\":\"productionMark\"},{\"value\":\"库存标识\",\"field\":\"inventoryMark\"},{\"value\":\"等级标识\",\"field\":\"classMark\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "成品出库单.xls";
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
            ExcelExportHelper<ZjnReportProductOutListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除成品出库单
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnReportProductOutRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除成品出库单
                    await _zjnReportProductOutRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新成品出库单
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnReportProductOutUpInput input)
        {
            var entity = input.Adapt<ZjnReportProductOutEntity>();
            var isOk = await _zjnReportProductOutRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除成品出库单
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnReportProductOutRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnReportProductOutRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


