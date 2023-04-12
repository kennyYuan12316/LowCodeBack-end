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
using HSZ.wms.Entitys.Dto.ZjnBaseOutorder;
using HSZ.wms.Interfaces.ZjnBaseOutorder;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnBaseOutorder
{
    /// <summary>
    /// 出货列表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseOutorder", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseOutorderService : IZjnBaseOutorderService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseOutorderEntity> _zjnBaseOutorderRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseOutorderService"/>类型的新实例
        /// </summary>
        public ZjnBaseOutorderService(ISqlSugarRepository<ZjnBaseOutorderEntity> zjnBaseOutorderRepository,
            IUserManager userManager)
        {
            _zjnBaseOutorderRepository = zjnBaseOutorderRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取出货列表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseOutorderRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseOutorderInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取出货列表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseOutorderListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_OutOrder" : input.sidx;
            List<string> queryOutTime = input.F_OutTime != null ? input.F_OutTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startOutTime = queryOutTime != null ? Ext.GetDateTime(queryOutTime.First()) : null;
            DateTime? endOutTime = queryOutTime != null ? Ext.GetDateTime(queryOutTime.Last()) : null;
            var data = await _zjnBaseOutorderRepository.AsSugarClient().Queryable<ZjnBaseOutorderEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouse), a => a.WareHouse.Contains(input.F_WareHouse))
                .WhereIF(!string.IsNullOrEmpty(input.F_OutOrder), a => a.OutOrder.Contains(input.F_OutOrder))
                .WhereIF(queryOutTime != null, a => a.OutTime >= new DateTime(startOutTime.ToDate().Year, startOutTime.ToDate().Month, startOutTime.ToDate().Day, startOutTime.ToDate().Hour, startOutTime.ToDate().Minute, startOutTime.ToDate().Second))
                .WhereIF(queryOutTime != null, a => a.OutTime <= new DateTime(endOutTime.ToDate().Year, endOutTime.ToDate().Month, endOutTime.ToDate().Day, endOutTime.ToDate().Hour, endOutTime.ToDate().Minute, endOutTime.ToDate().Second))
                .WhereIF(!string.IsNullOrEmpty(input.F_BusinessType), a => a.BusinessType.Contains(input.F_BusinessType))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsStatus), a => a.ProductsStatus.Contains(input.F_ProductsStatus))
                .Select((a
)=> new ZjnBaseOutorderListOutput
                {
                    F_Id = a.Id,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_Batch = a.Batch,
                    F_WareHouse = a.WareHouse,
                    F_OutOrder = a.OutOrder,
                    F_OutTime = a.OutTime,
                    F_BusinessType = a.BusinessType,
                    F_CreateUser = a.CreateUser,
                    F_ProductsStatus = a.ProductsStatus,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseOutorderListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建出货列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseOutorderCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseOutorderEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            
            var isOk = await _zjnBaseOutorderRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取出货列表无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnBaseOutorderListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_OutOrder" : input.sidx;
            List<string> queryOutTime = input.F_OutTime != null ? input.F_OutTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startOutTime = queryOutTime != null ? Ext.GetDateTime(queryOutTime.First()) : null;
            DateTime? endOutTime = queryOutTime != null ? Ext.GetDateTime(queryOutTime.Last()) : null;
            var data = await _zjnBaseOutorderRepository.AsSugarClient().Queryable<ZjnBaseOutorderEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouse), a => a.WareHouse.Contains(input.F_WareHouse))
                .WhereIF(!string.IsNullOrEmpty(input.F_OutOrder), a => a.OutOrder.Contains(input.F_OutOrder))
                .WhereIF(queryOutTime != null, a => a.OutTime >= new DateTime(startOutTime.ToDate().Year, startOutTime.ToDate().Month, startOutTime.ToDate().Day, startOutTime.ToDate().Hour, startOutTime.ToDate().Minute, startOutTime.ToDate().Second))
                .WhereIF(queryOutTime != null, a => a.OutTime <= new DateTime(endOutTime.ToDate().Year, endOutTime.ToDate().Month, endOutTime.ToDate().Day, endOutTime.ToDate().Hour, endOutTime.ToDate().Minute, endOutTime.ToDate().Second))
                .WhereIF(!string.IsNullOrEmpty(input.F_BusinessType), a => a.BusinessType.Contains(input.F_BusinessType))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsStatus), a => a.ProductsStatus.Contains(input.F_ProductsStatus))
                .Select((a
)=> new ZjnBaseOutorderListOutput
                {
                    F_Id = a.Id,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_Batch = a.Batch,
                    F_WareHouse = a.WareHouse,
                    F_OutOrder = a.OutOrder,
                    //F_OutTime = a.OutTime,
                    F_BusinessType = a.BusinessType,
                    F_CreateUser = a.CreateUser,
                    F_ProductsStatus = a.ProductsStatus,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出出货列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnBaseOutorderListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnBaseOutorderListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnBaseOutorderListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"出库单\",\"field\":\"outOrder\"},{\"value\":\"32位批次号\",\"field\":\"batch\"},{\"value\":\"物料编码\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"所属仓库\",\"field\":\"wareHouse\"},{\"value\":\"入库时间\",\"field\":\"outTime\"},{\"value\":\"业务类型\",\"field\":\"businessType\"},{\"value\":\"物料状态\",\"field\":\"productsStatus\"},{\"value\":\"创建用户\",\"field\":\"createUser\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "出货列表.xls";
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
            ExcelExportHelper<ZjnBaseOutorderListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除出货列表
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseOutorderRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除出货列表
                    await _zjnBaseOutorderRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新出货列表
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseOutorderUpInput input)
        {
            var entity = input.Adapt<ZjnBaseOutorderEntity>();
            entity.CreateTime = DateTime.Now;
            var isOk = await _zjnBaseOutorderRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除出货列表
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseOutorderRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseOutorderRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


