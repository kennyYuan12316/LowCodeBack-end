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
using HSZ.wms.Entitys.Dto.HltHrSynchronize;
using HSZ.wms.Interfaces.HltHrSynchronize;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrSynchronize
{
    /// <summary>
    /// 同步日志服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrSynchronize", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrSynchronizeService : IHltHrSynchronizeService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrSynchronizeEntity> _hltHrSynchronizeRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrSynchronizeService"/>类型的新实例
        /// </summary>
        public HltHrSynchronizeService(ISqlSugarRepository<HltHrSynchronizeEntity> hltHrSynchronizeRepository,
            IUserManager userManager)
        {
            _hltHrSynchronizeRepository = hltHrSynchronizeRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取同步日志
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrSynchronizeRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrSynchronizeInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取同步日志列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrSynchronizeListQueryInput input)
        {
            var sidx = input.sidx == null ? "n10" : input.sidx;
            List<string> queryN10 = input.n10 != null ? input.n10.Split(',').ToObject<List<string>>() : null;
            DateTime? startN10 = queryN10 != null ? Ext.GetDateTime(queryN10.First()) : null;
            DateTime? endN10 = queryN10 != null ? Ext.GetDateTime(queryN10.Last()) : null;
            var data = await _hltHrSynchronizeRepository.AsSugarClient().Queryable<HltHrSynchronizeEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.n2), a => a.N2.Contains(input.n2))
                .WhereIF(!string.IsNullOrEmpty(input.n4), a => a.N4.Contains(input.n4))
                .WhereIF(!string.IsNullOrEmpty(input.n6), a => a.N6.Contains(input.n6))
                .WhereIF(!string.IsNullOrEmpty(input.n8), a => a.N8.Contains(input.n8))
                .WhereIF(queryN10 != null, a => a.N10 >= new DateTime(startN10.ToDate().Year, startN10.ToDate().Month, startN10.ToDate().Day, startN10.ToDate().Hour, startN10.ToDate().Minute, startN10.ToDate().Second))
                .WhereIF(queryN10 != null, a => a.N10 <= new DateTime(endN10.ToDate().Year, endN10.ToDate().Month, endN10.ToDate().Day, endN10.ToDate().Hour, endN10.ToDate().Minute, endN10.ToDate().Second))
                .Select((a
)=> new HltHrSynchronizeListOutput
                {
                    n1 = a.N1,
                    n2 = a.N2,
                    n3 = a.N3,
                    n4 = a.N4,
                    n5 = a.N5,
                    n6 = a.N6,
                    n7 = a.N7,
                    n8 = a.N8,
                    n9 = a.N9,
                    n10 = a.N10,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrSynchronizeListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建同步日志
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrSynchronizeCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrSynchronizeEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrSynchronizeRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取同步日志无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrSynchronizeListQueryInput input)
        {
            var sidx = input.sidx == null ? "n10" : input.sidx;
            List<string> queryN10 = input.n10 != null ? input.n10.Split(',').ToObject<List<string>>() : null;
            DateTime? startN10 = queryN10 != null ? Ext.GetDateTime(queryN10.First()) : null;
            DateTime? endN10 = queryN10 != null ? Ext.GetDateTime(queryN10.Last()) : null;
            var data = await _hltHrSynchronizeRepository.AsSugarClient().Queryable<HltHrSynchronizeEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.n2), a => a.N2.Contains(input.n2))
                .WhereIF(!string.IsNullOrEmpty(input.n4), a => a.N4.Contains(input.n4))
                .WhereIF(!string.IsNullOrEmpty(input.n6), a => a.N6.Contains(input.n6))
                .WhereIF(!string.IsNullOrEmpty(input.n8), a => a.N8.Contains(input.n8))
                .WhereIF(queryN10 != null, a => a.N10 >= new DateTime(startN10.ToDate().Year, startN10.ToDate().Month, startN10.ToDate().Day, startN10.ToDate().Hour, startN10.ToDate().Minute, startN10.ToDate().Second))
                .WhereIF(queryN10 != null, a => a.N10 <= new DateTime(endN10.ToDate().Year, endN10.ToDate().Month, endN10.ToDate().Day, endN10.ToDate().Hour, endN10.ToDate().Minute, endN10.ToDate().Second))
                .Select((a
)=> new HltHrSynchronizeListOutput
                {
                    n1 = a.N1,
                    n2 = a.N2,
                    n3 = a.N3,
                    n4 = a.N4,
                    n5 = a.N5,
                    n6 = a.N6,
                    n7 = a.N7,
                    n8 = a.N8,
                    n9 = a.N9,
                    n10 = a.N10,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出同步日志
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrSynchronizeListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrSynchronizeListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrSynchronizeListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"日志标识\",\"field\":\"n1\"},{\"value\":\"任务标识\",\"field\":\"n2\"},{\"value\":\"系统编码\",\"field\":\"n3\"},{\"value\":\"系统名称\",\"field\":\"n4\"},{\"value\":\"映射编码\",\"field\":\"n5\"},{\"value\":\"映射名称\",\"field\":\"n6\"},{\"value\":\"同步数量\",\"field\":\"n7\"},{\"value\":\"错误标识\",\"field\":\"n8\"},{\"value\":\"耗时\",\"field\":\"n9\"},{\"value\":\"创建时间\",\"field\":\"n10\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "同步日志.xls";
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
            ExcelExportHelper<HltHrSynchronizeListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除同步日志
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrSynchronizeRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除同步日志
                    await _hltHrSynchronizeRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新同步日志
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrSynchronizeUpInput input)
        {
            var entity = input.Adapt<HltHrSynchronizeEntity>();
            var isOk = await _hltHrSynchronizeRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除同步日志
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrSynchronizeRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrSynchronizeRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


