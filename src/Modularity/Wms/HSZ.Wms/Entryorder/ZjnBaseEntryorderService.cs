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
using HSZ.wms.Entitys.Dto.ZjnBaseEntryorder;
using HSZ.wms.Interfaces.ZjnBaseEntryorder;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnBaseEntryorder
{
    /// <summary>
    /// 收货列表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseEntryorder", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseEntryorderService : IZjnBaseEntryorderService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseEntryorderEntity> _zjnBaseEntryorderRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseEntryorderService"/>类型的新实例
        /// </summary>
        public ZjnBaseEntryorderService(ISqlSugarRepository<ZjnBaseEntryorderEntity> zjnBaseEntryorderRepository,
            IUserManager userManager)
        {
            _zjnBaseEntryorderRepository = zjnBaseEntryorderRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取收货列表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseEntryorderRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseEntryorderInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取收货列表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseEntryorderListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Batch" : input.sidx;
            var data = await _zjnBaseEntryorderRepository.AsSugarClient().Queryable<ZjnBaseEntryorderEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouse), a => a.WareHouse.Contains(input.F_WareHouse))
                .WhereIF(!string.IsNullOrEmpty(input.F_EntryOrder), a => a.EntryOrder.Contains(input.F_EntryOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsStatus), a => a.ProductsStatus.Contains(input.F_ProductsStatus))
                .Select((a
)=> new ZjnBaseEntryorderListOutput
                {
                    F_Id = a.Id,
                    F_Description = a.Description,
                    F_CreateTime = a.CreateTime,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_Batch = a.Batch,
                    F_WareHouse = a.WareHouse,
                    F_BusinessType = a.BusinessType,
                    F_EntryOrder = a.EntryOrder,
                    F_EntryTime = a.EntryTime,
                    F_CreateUser = a.CreateUser,
                    F_ProductsStatus = a.ProductsStatus,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseEntryorderListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建收货列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseEntryorderCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseEntryorderEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            
            var isOk = await _zjnBaseEntryorderRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取收货列表无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnBaseEntryorderListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Batch" : input.sidx;
            var data = await _zjnBaseEntryorderRepository.AsSugarClient().Queryable<ZjnBaseEntryorderEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsCode), a => a.ProductsCode.Contains(input.F_ProductsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsName), a => a.ProductsName.Contains(input.F_ProductsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Batch), a => a.Batch.Contains(input.F_Batch))
                .WhereIF(!string.IsNullOrEmpty(input.F_WareHouse), a => a.WareHouse.Contains(input.F_WareHouse))
                .WhereIF(!string.IsNullOrEmpty(input.F_EntryOrder), a => a.EntryOrder.Contains(input.F_EntryOrder))
                .WhereIF(!string.IsNullOrEmpty(input.F_ProductsStatus), a => a.ProductsStatus.Contains(input.F_ProductsStatus))
                .Select((a
)=> new ZjnBaseEntryorderListOutput
                {
                    F_Id = a.Id,
                    F_Description = a.Description,
                    F_CreateTime = a.CreateTime,
                    F_ProductsCode = a.ProductsCode,
                    F_ProductsName = a.ProductsName,
                    F_Batch = a.Batch,
                    F_WareHouse = a.WareHouse,
                    F_BusinessType = a.BusinessType,
                    F_EntryOrder = a.EntryOrder,
                    F_EntryTime = a.EntryTime,
                    F_CreateUser = a.CreateUser,
                    F_ProductsStatus = a.ProductsStatus,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出收货列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnBaseEntryorderListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnBaseEntryorderListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnBaseEntryorderListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"32位批次号\",\"field\":\"batch\"},{\"value\":\"物料编码\",\"field\":\"productsCode\"},{\"value\":\"物料名称\",\"field\":\"productsName\"},{\"value\":\"所属仓库\",\"field\":\"wareHouse\"},{\"value\":\"业务类型\",\"field\":\"businessType\"},{\"value\":\"入库单\",\"field\":\"entryOrder\"},{\"value\":\"入库时间\",\"field\":\"entryTime\"},{\"value\":\"物料状态\",\"field\":\"productsStatus\"},{\"value\":\"创建用户\",\"field\":\"createUser\"},{\"value\":\"描述\",\"field\":\"description\"},{\"value\":\"更新时间\",\"field\":\"createTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "收货列表.xls";
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
            ExcelExportHelper<ZjnBaseEntryorderListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除收货列表
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseEntryorderRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除收货列表
                    await _zjnBaseEntryorderRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新收货列表
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseEntryorderUpInput input)
        {
            var entity = input.Adapt<ZjnBaseEntryorderEntity>();
            entity.CreateTime = DateTime.Now;
            var isOk = await _zjnBaseEntryorderRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除收货列表
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseEntryorderRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseEntryorderRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


