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
using HSZ.wms.Entitys.Dto.ZjnBaseLesGoods;
using HSZ.wms.Interfaces.ZjnBaseLesGoods;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnBaseLesGoods
{
    /// <summary>
    /// LES物料原始数据服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseLesGoods", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseLesGoodsService : IZjnBaseLesGoodsService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseLesGoodsEntity> _zjnBaseLesGoodsRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseLesGoodsService"/>类型的新实例
        /// </summary>
        public ZjnBaseLesGoodsService(ISqlSugarRepository<ZjnBaseLesGoodsEntity> zjnBaseLesGoodsRepository,
            IUserManager userManager)
        {
            _zjnBaseLesGoodsRepository = zjnBaseLesGoodsRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取LES物料原始数据
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseLesGoodsRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseLesGoodsInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取LES物料原始数据列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseLesGoodsListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<object> queryStayHours = input.F_StayHours != null ? input.F_StayHours.Split(',').ToObject<List<object>>() : null;
            var startStayHours = input.F_StayHours != null && !string.IsNullOrEmpty(queryStayHours.First().ToString()) ? queryStayHours.First() : decimal.MinValue;
            var endStayHours = input.F_StayHours != null && !string.IsNullOrEmpty(queryStayHours.Last().ToString()) ? queryStayHours.Last() : decimal.MaxValue;
            List<string> queryCreateTime = input.F_CreateTime != null ? input.F_CreateTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startCreateTime = queryCreateTime != null ? Ext.GetDateTime(queryCreateTime.First()) : null;
            DateTime? endCreateTime = queryCreateTime != null ? Ext.GetDateTime(queryCreateTime.Last()) : null;
            var data = await _zjnBaseLesGoodsRepository.AsSugarClient().Queryable<ZjnBaseLesGoodsEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_Code), a => a.Code.Contains(input.F_Code))
                .WhereIF(!string.IsNullOrEmpty(input.F_xName), a => a.XName.Contains(input.F_xName))
                .WhereIF(!string.IsNullOrEmpty(input.F_xType), a => a.XType.Contains(input.F_xType))
                .WhereIF(queryStayHours != null, a => SqlFunc.Between(a.StayHours, startStayHours, endStayHours))
                .WhereIF(queryCreateTime != null, a => a.CreateTime >= new DateTime(startCreateTime.ToDate().Year, startCreateTime.ToDate().Month, startCreateTime.ToDate().Day, 0, 0, 0))
                .WhereIF(queryCreateTime != null, a => a.CreateTime <= new DateTime(endCreateTime.ToDate().Year, endCreateTime.ToDate().Month, endCreateTime.ToDate().Day, 23, 59, 59))
                .Select((a
)=> new ZjnBaseLesGoodsListOutput
                {
                    F_Id = a.Id,
                    F_Code = a.Code,
                    F_xName = a.XName,
                    F_xType = a.XType,
                    F_DefaultUnit = a.DefaultUnit,
                    F_ValidDays = a.ValidDays,
                    F_StayHours = a.StayHours,
                    F_BatchManageFlag = a.BatchManageFlag,
                    F_Specification = a.Specification,
                    F_CreateTime = a.CreateTime,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseLesGoodsListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建LES物料原始数据
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseLesGoodsCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseLesGoodsEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateTime = DateTime.Now;
            
            var isOk = await _zjnBaseLesGoodsRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取LES物料原始数据无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnBaseLesGoodsListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<object> queryStayHours = input.F_StayHours != null ? input.F_StayHours.Split(',').ToObject<List<object>>() : null;
            var startStayHours = input.F_StayHours != null && !string.IsNullOrEmpty(queryStayHours.First().ToString()) ? queryStayHours.First() : decimal.MinValue;
            var endStayHours = input.F_StayHours != null && !string.IsNullOrEmpty(queryStayHours.Last().ToString()) ? queryStayHours.Last() : decimal.MaxValue;
            List<string> queryCreateTime = input.F_CreateTime != null ? input.F_CreateTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startCreateTime = queryCreateTime != null ? Ext.GetDateTime(queryCreateTime.First()) : null;
            DateTime? endCreateTime = queryCreateTime != null ? Ext.GetDateTime(queryCreateTime.Last()) : null;
            var data = await _zjnBaseLesGoodsRepository.AsSugarClient().Queryable<ZjnBaseLesGoodsEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_Code), a => a.Code.Contains(input.F_Code))
                .WhereIF(!string.IsNullOrEmpty(input.F_xName), a => a.XName.Contains(input.F_xName))
                .WhereIF(!string.IsNullOrEmpty(input.F_xType), a => a.XType.Contains(input.F_xType))
                .WhereIF(queryStayHours != null, a => SqlFunc.Between(a.StayHours, startStayHours, endStayHours))
                .WhereIF(queryCreateTime != null, a => a.CreateTime >= new DateTime(startCreateTime.ToDate().Year, startCreateTime.ToDate().Month, startCreateTime.ToDate().Day, 0, 0, 0))
                .WhereIF(queryCreateTime != null, a => a.CreateTime <= new DateTime(endCreateTime.ToDate().Year, endCreateTime.ToDate().Month, endCreateTime.ToDate().Day, 23, 59, 59))
                .Select((a
)=> new ZjnBaseLesGoodsListOutput
                {
                    F_Id = a.Id,
                    F_Code = a.Code,
                    F_xName = a.XName,
                    F_xType = a.XType,
                    F_DefaultUnit = a.DefaultUnit,
                    F_ValidDays = a.ValidDays,
                    F_StayHours = a.StayHours,
                    F_BatchManageFlag = a.BatchManageFlag,
                    F_Specification = a.Specification,
                    F_CreateTime = a.CreateTime,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出LES物料原始数据
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnBaseLesGoodsListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnBaseLesGoodsListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnBaseLesGoodsListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"物料编号\",\"field\":\"code\"},{\"value\":\"物料名称\",\"field\":\"xName\"},{\"value\":\"物料类型\",\"field\":\"xType\"},{\"value\":\"基本单位\",\"field\":\"defaultUnit\"},{\"value\":\"总有效期(天）\",\"field\":\"validDays\"},{\"value\":\"静置时间\",\"field\":\"stayHours\"},{\"value\":\"是否批次管理 \",\"field\":\"batchManageFlag\"},{\"value\":\"规格型号\",\"field\":\"specification\"},{\"value\":\"最后更新时间\",\"field\":\"createTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "LES物料原始数据.xls";
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
            ExcelExportHelper<ZjnBaseLesGoodsListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除LES物料原始数据
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseLesGoodsRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除LES物料原始数据
                    await _zjnBaseLesGoodsRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新LES物料原始数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseLesGoodsUpInput input)
        {
            var entity = input.Adapt<ZjnBaseLesGoodsEntity>();
            var isOk = await _zjnBaseLesGoodsRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除LES物料原始数据
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseLesGoodsRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseLesGoodsRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


