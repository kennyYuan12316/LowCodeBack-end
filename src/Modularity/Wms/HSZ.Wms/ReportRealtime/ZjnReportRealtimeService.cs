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
using HSZ.wms.Entitys.Dto.ZjnReportRealtime;
using HSZ.wms.Interfaces.ZjnReportRealtime;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnReportRealtime
{
    /// <summary>
    /// 实时库存服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnReportRealtime", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnReportRealtimeService : IZjnReportRealtimeService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnReportRealtimeEntity> _zjnReportRealtimeRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnReportRealtimeService"/>类型的新实例
        /// </summary>
        public ZjnReportRealtimeService(ISqlSugarRepository<ZjnReportRealtimeEntity> zjnReportRealtimeRepository,
            IUserManager userManager)
        {
            _zjnReportRealtimeRepository = zjnReportRealtimeRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取实时库存
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnReportRealtimeRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnReportRealtimeInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取实时库存列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnReportRealtimeListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_ProductsCode" : input.sidx;
            var data = await _zjnReportRealtimeRepository.AsSugarClient().Queryable<ZjnReportRealtimeEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .Select((a
)=> new ZjnReportRealtimeListOutput
                {
                    F_Id = a.Id,
                    F_Factory = a.Factory,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_Batch = a.Batch,
                    F_Quality = a.Quality,
                    F_ProductsUnit = a.ProductsUnit,
                    F_SumInside = a.SumInside,
                    F_TrayCountInside = a.TrayCountInside,
                    F_SumOutside = a.SumOutside,
                    F_TrayCountOutside = a.TrayCountOutside,
                    F_SumMoving = a.SumMoving,
                    F_TrayCountMoving = a.TrayCountMoving,
                    F_SumTotal = a.SumTotal,
                    F_TrayCountTotal = a.TrayCountTotal,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnReportRealtimeListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建实时库存
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnReportRealtimeCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnReportRealtimeEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnReportRealtimeRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取实时库存无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnReportRealtimeListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_ProductsCode" : input.sidx;
            var data = await _zjnReportRealtimeRepository.AsSugarClient().Queryable<ZjnReportRealtimeEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .Select((a
)=> new ZjnReportRealtimeListOutput
                {
                    F_Id = a.Id,
                    F_Factory = a.Factory,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_Batch = a.Batch,
                    F_Quality = a.Quality,
                    F_ProductsUnit = a.ProductsUnit,
                    F_SumInside = a.SumInside,
                    F_TrayCountInside = a.TrayCountInside,
                    F_SumOutside = a.SumOutside,
                    F_TrayCountOutside = a.TrayCountOutside,
                    F_SumMoving = a.SumMoving,
                    F_TrayCountMoving = a.TrayCountMoving,
                    F_SumTotal = a.SumTotal,
                    F_TrayCountTotal = a.TrayCountTotal,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出实时库存
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnReportRealtimeListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnReportRealtimeListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnReportRealtimeListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"序号\",\"field\":\"num\"},{\"value\":\"工厂\",\"field\":\"factory\"},{\"value\":\"物料编码\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"32位批次号\",\"field\":\"batch\"},{\"value\":\"质量状态\",\"field\":\"quality\"},{\"value\":\"物料单位\",\"field\":\"productsUnit\"},{\"value\":\"总数库内\",\"field\":\"sumInside\"},{\"value\":\"托盘数库内\",\"field\":\"trayCountInside\"},{\"value\":\"总数库外\",\"field\":\"sumOutside\"},{\"value\":\"托盘数库外\",\"field\":\"trayCountOutside\"},{\"value\":\"总数移动中\",\"field\":\"sumMoving\"},{\"value\":\"托盘数移动中\",\"field\":\"trayCountMoving\"},{\"value\":\"总数全部\",\"field\":\"sumTotal\"},{\"value\":\"托盘数全部\",\"field\":\"trayCountTotal\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "实时库存.xls";
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
            ExcelExportHelper<ZjnReportRealtimeListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除实时库存
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnReportRealtimeRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除实时库存
                    await _zjnReportRealtimeRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新实时库存
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnReportRealtimeUpInput input)
        {
            var entity = input.Adapt<ZjnReportRealtimeEntity>();
            var isOk = await _zjnReportRealtimeRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除实时库存
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnReportRealtimeRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnReportRealtimeRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


