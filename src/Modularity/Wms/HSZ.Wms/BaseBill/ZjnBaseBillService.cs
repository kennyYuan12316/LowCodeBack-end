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
using HSZ.wms.Entitys.Dto.ZjnBaseBill;
using HSZ.wms.Interfaces.ZjnBaseBill;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnBaseBill
{
    /// <summary>
    /// 单据信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseBill", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseBillService : IZjnBaseBillService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseBillEntity> _zjnBaseBillRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseBillService"/>类型的新实例
        /// </summary>
        public ZjnBaseBillService(ISqlSugarRepository<ZjnBaseBillEntity> zjnBaseBillRepository,
            IUserManager userManager)
        {
            _zjnBaseBillRepository = zjnBaseBillRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取单据信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseBillRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseBillInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取单据信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseBillListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<object> queryType = input.F_Type != null ? input.F_Type.Split(',').ToObject<List<object>>() : null;
            var startType = input.F_Type != null && !string.IsNullOrEmpty(queryType.First().ToString()) ? queryType.First() : decimal.MinValue;
            var endType = input.F_Type != null && !string.IsNullOrEmpty(queryType.Last().ToString()) ? queryType.Last() : decimal.MaxValue;
            var data = await _zjnBaseBillRepository.AsSugarClient().Queryable<ZjnBaseBillEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_BillNo), a => a.BillNo.Contains(input.F_BillNo))
                .WhereIF(queryType != null, a => SqlFunc.Between(a.Type, startType, endType))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Select((a
)=> new ZjnBaseBillListOutput
                {
                    F_Id = a.Id,
                    F_BillNo = a.BillNo,
                    F_BillName = a.BillName,
                    F_Type = a.Type,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = a.EnabledMark,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseBillListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建单据信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseBillCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseBillEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            
            var isOk = await _zjnBaseBillRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取单据信息无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnBaseBillListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<object> queryType = input.F_Type != null ? input.F_Type.Split(',').ToObject<List<object>>() : null;
            var startType = input.F_Type != null && !string.IsNullOrEmpty(queryType.First().ToString()) ? queryType.First() : decimal.MinValue;
            var endType = input.F_Type != null && !string.IsNullOrEmpty(queryType.Last().ToString()) ? queryType.Last() : decimal.MaxValue;
            var data = await _zjnBaseBillRepository.AsSugarClient().Queryable<ZjnBaseBillEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_BillNo), a => a.BillNo.Contains(input.F_BillNo))
                .WhereIF(queryType != null, a => SqlFunc.Between(a.Type, startType, endType))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Select((a
)=> new ZjnBaseBillListOutput
                {
                    F_Id = a.Id,
                    F_BillNo = a.BillNo,
                    F_BillName = a.BillName,
                    F_Type = a.Type,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = a.EnabledMark,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出单据信息
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnBaseBillListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnBaseBillListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnBaseBillListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"单据编号\",\"field\":\"billNo\"},{\"value\":\"单据名称\",\"field\":\"billName\"},{\"value\":\"类型\",\"field\":\"type\"},{\"value\":\"创建者\",\"field\":\"createUser\"},{\"value\":\"创建时间\",\"field\":\"createTime\"},{\"value\":\"有效标志\",\"field\":\"enabledMark\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "单据信息.xls";
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
            ExcelExportHelper<ZjnBaseBillListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除单据信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseBillRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除单据信息
                    await _zjnBaseBillRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新单据信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseBillUpInput input)
        {
            var entity = input.Adapt<ZjnBaseBillEntity>();
            var isOk = await _zjnBaseBillRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除单据信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseBillRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseBillRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


