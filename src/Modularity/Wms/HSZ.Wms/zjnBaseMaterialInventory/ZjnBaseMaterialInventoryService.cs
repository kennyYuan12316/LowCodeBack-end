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
using HSZ.wms.Entitys.Dto.ZjnBaseMaterialInventory;
using HSZ.wms.Interfaces.ZjnBaseMaterialInventory;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using HSZ.wms.Entitys.Dto.zjnWmsMaterialInventory;

namespace HSZ.wms.ZjnBaseMaterialInventory
{
    /// <summary>
    /// 立库库存信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseMaterialInventory", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseMaterialInventoryService : IZjnBaseMaterialInventoryService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseMaterialInventoryEntity> _zjnBaseMaterialInventoryRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseMaterialInventoryService"/>类型的新实例
        /// </summary>
        public ZjnBaseMaterialInventoryService(ISqlSugarRepository<ZjnBaseMaterialInventoryEntity> zjnBaseMaterialInventoryRepository,
            IUserManager userManager)
        {
            _zjnBaseMaterialInventoryRepository = zjnBaseMaterialInventoryRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取立库库存信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseMaterialInventoryRepository.GetFirstAsync(p => p.Id == id)).Adapt<zjnWmsMaterialInventoryInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取立库库存信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] zjnWmsMaterialInventoryListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_CreateTime" : input.sidx;
            var data = await _zjnBaseMaterialInventoryRepository.AsSugarClient().Queryable<ZjnBaseMaterialInventoryEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsUnit), a => a.ProductsUnit.Contains(input.F_ProductsUnit))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsType), a => a.ProductsType.Contains(input.F_ProductsType))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsState), a => a.ProductsState.Contains(input.F_ProductsState))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsBatch), a => a.ProductsBatch.Contains(input.F_ProductsBatch))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                .Select((a
)=> new zjnWmsMaterialInventoryListOutput
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
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<zjnWmsMaterialInventoryListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建立库库存信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] zjnWmsMaterialInventoryCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseMaterialInventoryEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnBaseMaterialInventoryRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取立库库存信息无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] zjnWmsMaterialInventoryListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_CreateTime" : input.sidx;
            var data = await _zjnBaseMaterialInventoryRepository.AsSugarClient().Queryable<ZjnBaseMaterialInventoryEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsUnit), a => a.ProductsUnit.Contains(input.F_ProductsUnit))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsType), a => a.ProductsType.Contains(input.F_ProductsType))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsState), a => a.ProductsState.Contains(input.F_ProductsState))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsBatch), a => a.ProductsBatch.Contains(input.F_ProductsBatch))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsLocation), a => a.ProductsLocation.Contains(input.F_ProductsLocation))
                .Select((a
)=> new zjnWmsMaterialInventoryListOutput
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
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出立库库存信息
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] zjnWmsMaterialInventoryListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<zjnWmsMaterialInventoryListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<zjnWmsMaterialInventoryListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"物料编码\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"物料规格\",\"field\":\"productsStyle\"},{\"value\":\"物料数量\",\"field\":\"productsQuantity\"},{\"value\":\"物料单位\",\"field\":\"productsUnit\"},{\"value\":\"物料类型\",\"field\":\"productsType\"},{\"value\":\"物料等级\",\"field\":\"productsGrade\"},{\"value\":\"物料状态\",\"field\":\"productsState\"},{\"value\":\"物料批次\",\"field\":\"productsBatch\"},{\"value\":\"物料货位\",\"field\":\"productsLocation\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "立库库存信息.xls";
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
            ExcelExportHelper<zjnWmsMaterialInventoryListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除立库库存信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseMaterialInventoryRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除立库库存信息
                    await _zjnBaseMaterialInventoryRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新立库库存信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] zjnWmsMaterialInventoryUpInput input)
        {
            var entity = input.Adapt<ZjnBaseMaterialInventoryEntity>();
            var isOk = await _zjnBaseMaterialInventoryRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除立库库存信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseMaterialInventoryRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseMaterialInventoryRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


