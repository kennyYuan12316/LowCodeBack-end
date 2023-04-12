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
using HSZ.wms.Entitys.Dto.ZjnBaseChangeorder;
using HSZ.wms.Interfaces.ZjnBaseChangeorder;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnBaseChangeorder
{
    /// <summary>
    /// 变更列表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseChangeorder", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseChangeorderService : IZjnBaseChangeorderService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseChangeorderEntity> _zjnBaseChangeorderRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseChangeorderService"/>类型的新实例
        /// </summary>
        public ZjnBaseChangeorderService(ISqlSugarRepository<ZjnBaseChangeorderEntity> zjnBaseChangeorderRepository,
            IUserManager userManager)
        {
            _zjnBaseChangeorderRepository = zjnBaseChangeorderRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取变更列表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseChangeorderRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseChangeorderInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取变更列表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseChangeorderListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_ChangeOrder" : input.sidx;
            var data = await _zjnBaseChangeorderRepository.AsSugarClient().Queryable<ZjnBaseChangeorderEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ChangeOrder), a => a.ChangeOrder.Contains(input.F_ChangeOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_BusinessType), a => a.BusinessType.Contains(input.F_BusinessType))
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouse), a => a.WareHouse.Contains(input.F_WareHouse))
                .Select((a
)=> new ZjnBaseChangeorderListOutput
                {
                    F_Id = a.Id,
                    F_ChangeOrder = a.ChangeOrder,
                    F_CreateTime = a.CreateTime,
                    F_CreateUser = a.CreateUser,
                    F_BusinessType = a.BusinessType,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_Location = a.Location,
                    F_LocationName = a.LocationName,
                    F_WareHouse = a.WareHouse,
                    F_ProductsCodeAgo = a.ProductsCodeAgo,
                    F_ProductsNameAgo = a.ProductsNameAgo,
                    F_BatchAgo = a.BatchAgo,
                    F_InventoryStatusAgo = a.InventoryStatusAgo,
                    F_ProductsCodeAfter = a.ProductsCodeAfter,
                    F_ProductsNameAfter = a.ProductsNameAfter,
                    F_BatchAfter = a.BatchAfter,
                    F_InventoryStatusAfter = a.InventoryStatusAfter,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseChangeorderListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建变更列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseChangeorderCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseChangeorderEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            
            var isOk = await _zjnBaseChangeorderRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取变更列表无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnBaseChangeorderListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_ChangeOrder" : input.sidx;
            var data = await _zjnBaseChangeorderRepository.AsSugarClient().Queryable<ZjnBaseChangeorderEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ChangeOrder), a => a.ChangeOrder.Contains(input.F_ChangeOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_BusinessType), a => a.BusinessType.Contains(input.F_BusinessType))
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouse), a => a.WareHouse.Contains(input.F_WareHouse))
                .Select((a
)=> new ZjnBaseChangeorderListOutput
                {
                    F_Id = a.Id,
                    F_ChangeOrder = a.ChangeOrder,
                    F_CreateTime = a.CreateTime,
                    F_CreateUser = a.CreateUser,
                    F_BusinessType = a.BusinessType,
                    F_ProductsQuantity = a.ProductsQuantity,
                    F_ProductsUnit = a.ProductsUnit,
                    F_Location = a.Location,
                    F_LocationName = a.LocationName,
                    F_WareHouse = a.WareHouse,
                    F_ProductsCodeAgo = a.ProductsCodeAgo,
                    F_ProductsNameAgo = a.ProductsNameAgo,
                    F_BatchAgo = a.BatchAgo,
                    F_InventoryStatusAgo = a.InventoryStatusAgo,
                    F_ProductsCodeAfter = a.ProductsCodeAfter,
                    F_ProductsNameAfter = a.ProductsNameAfter,
                    F_BatchAfter = a.BatchAfter,
                    F_InventoryStatusAfter = a.InventoryStatusAfter,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出变更列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnBaseChangeorderListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnBaseChangeorderListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnBaseChangeorderListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"变更单\",\"field\":\"changeOrder\"},{\"value\":\"业务类型\",\"field\":\"businessType\"},{\"value\":\"物料数量\",\"field\":\"productsQuantity\"},{\"value\":\"物料单位\",\"field\":\"productsUnit\"},{\"value\":\"位置\",\"field\":\"location\"},{\"value\":\"位置名\",\"field\":\"locationName\"},{\"value\":\"所属仓库\",\"field\":\"wareHouse\"},{\"value\":\"旧物料编码\",\"field\":\"productsCodeAgo\"},{\"value\":\"旧物料名称\",\"field\":\"productsNameAgo\"},{\"value\":\"旧批次号\",\"field\":\"batchAgo\"},{\"value\":\"旧库存状态\",\"field\":\"inventoryStatusAgo\"},{\"value\":\"新物料编码\",\"field\":\"productsCodeAfter\"},{\"value\":\"新物料名称\",\"field\":\"productsNameAfter\"},{\"value\":\"新批次号\",\"field\":\"batchAfter\"},{\"value\":\"新库存状态\",\"field\":\"inventoryStatusAfter\"},{\"value\":\"创建用户\",\"field\":\"createUser\"},{\"value\":\"更新时间\",\"field\":\"createTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "变更列表.xls";
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
            ExcelExportHelper<ZjnBaseChangeorderListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除变更列表
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseChangeorderRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除变更列表
                    await _zjnBaseChangeorderRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新变更列表
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseChangeorderUpInput input)
        {
            var entity = input.Adapt<ZjnBaseChangeorderEntity>();
            entity.CreateTime = DateTime.Now;
            var isOk = await _zjnBaseChangeorderRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除变更列表
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseChangeorderRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseChangeorderRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


