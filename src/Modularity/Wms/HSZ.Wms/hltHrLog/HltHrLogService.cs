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
using HSZ.wms.Entitys.Dto.HltHrLog;
using HSZ.wms.Interfaces.HltHrLog;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrLog
{
    /// <summary>
    /// 日志记录服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrLog", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrLogService : IHltHrLogService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrLogEntity> _hltHrLogRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrLogService"/>类型的新实例
        /// </summary>
        public HltHrLogService(ISqlSugarRepository<HltHrLogEntity> hltHrLogRepository,
            IUserManager userManager)
        {
            _hltHrLogRepository = hltHrLogRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取日志记录
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrLogRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrLogInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取日志记录列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrLogListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<string> queryJ9 = input.j9 != null ? input.j9.Split(',').ToObject<List<string>>() : null;
            DateTime? startJ9 = queryJ9 != null ? Ext.GetDateTime(queryJ9.First()) : null;
            DateTime? endJ9 = queryJ9 != null ? Ext.GetDateTime(queryJ9.Last()) : null;
            var data = await _hltHrLogRepository.AsSugarClient().Queryable<HltHrLogEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.j2), a => a.J2.Contains(input.j2))
                .WhereIF(!string.IsNullOrEmpty(input.j3), a => a.J3.Contains(input.j3))
                .WhereIF(!string.IsNullOrEmpty(input.j5), a => a.J5.Contains(input.j5))
                .WhereIF(!string.IsNullOrEmpty(input.j8), a => a.J8.Contains(input.j8))
                .WhereIF(queryJ9 != null, a => a.J9 >= new DateTime(startJ9.ToDate().Year, startJ9.ToDate().Month, startJ9.ToDate().Day, 0, 0, 0))
                .WhereIF(queryJ9 != null, a => a.J9 <= new DateTime(endJ9.ToDate().Year, endJ9.ToDate().Month, endJ9.ToDate().Day, 23, 59, 59))
                .Select((a
)=> new HltHrLogListOutput
                {
                    j1 = a.J1,
                    j2 = a.J2,
                    j3 = a.J3,
                    j4 = a.J4,
                    j5 = a.J5,
                    j6 = a.J6,
                    j7 = a.J7,
                    j8 = a.J8,
                    j9 = a.J9,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrLogListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建日志记录
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrLogCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrLogEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrLogRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取日志记录无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrLogListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<string> queryJ9 = input.j9 != null ? input.j9.Split(',').ToObject<List<string>>() : null;
            DateTime? startJ9 = queryJ9 != null ? Ext.GetDateTime(queryJ9.First()) : null;
            DateTime? endJ9 = queryJ9 != null ? Ext.GetDateTime(queryJ9.Last()) : null;
            var data = await _hltHrLogRepository.AsSugarClient().Queryable<HltHrLogEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.j2), a => a.J2.Contains(input.j2))
                .WhereIF(!string.IsNullOrEmpty(input.j3), a => a.J3.Contains(input.j3))
                .WhereIF(!string.IsNullOrEmpty(input.j5), a => a.J5.Contains(input.j5))
                .WhereIF(!string.IsNullOrEmpty(input.j8), a => a.J8.Contains(input.j8))
                .WhereIF(queryJ9 != null, a => a.J9 >= new DateTime(startJ9.ToDate().Year, startJ9.ToDate().Month, startJ9.ToDate().Day, 0, 0, 0))
                .WhereIF(queryJ9 != null, a => a.J9 <= new DateTime(endJ9.ToDate().Year, endJ9.ToDate().Month, endJ9.ToDate().Day, 23, 59, 59))
                .Select((a
)=> new HltHrLogListOutput
                {
                    j1 = a.J1,
                    j2 = a.J2,
                    j3 = a.J3,
                    j4 = a.J4,
                    j5 = a.J5,
                    j6 = a.J6,
                    j7 = a.J7,
                    j8 = a.J8,
                    j9 = a.J9,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出日志记录
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrLogListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrLogListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrLogListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"日志编号\",\"field\":\"j1\"},{\"value\":\"系统模块\",\"field\":\"j2\"},{\"value\":\"操作类型\",\"field\":\"j3\"},{\"value\":\"请求方式\",\"field\":\"j4\"},{\"value\":\"操作人员\",\"field\":\"j5\"},{\"value\":\"操作地址\",\"field\":\"j6\"},{\"value\":\"操作地点\",\"field\":\"j7\"},{\"value\":\"操作状态\",\"field\":\"j8\"},{\"value\":\"操作日期\",\"field\":\"j9\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "日志记录.xls";
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
            ExcelExportHelper<HltHrLogListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除日志记录
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrLogRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除日志记录
                    await _hltHrLogRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新日志记录
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrLogUpInput input)
        {
            var entity = input.Adapt<HltHrLogEntity>();
            var isOk = await _hltHrLogRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除日志记录
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrLogRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrLogRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


