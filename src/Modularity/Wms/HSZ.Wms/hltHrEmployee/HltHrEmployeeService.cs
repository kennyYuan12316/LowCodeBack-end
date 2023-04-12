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
using HSZ.wms.Entitys.Dto.HltHrEmployee;
using HSZ.wms.Interfaces.HltHrEmployee;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrEmployee
{
    /// <summary>
    /// 员工信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrEmployee", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrEmployeeService : IHltHrEmployeeService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrEmployeeEntity> _hltHrEmployeeRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrEmployeeService"/>类型的新实例
        /// </summary>
        public HltHrEmployeeService(ISqlSugarRepository<HltHrEmployeeEntity> hltHrEmployeeRepository,
            IUserManager userManager)
        {
            _hltHrEmployeeRepository = hltHrEmployeeRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取员工信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrEmployeeRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrEmployeeInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取员工信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrEmployeeListQueryInput input)
        {
            var sidx = input.sidx == null ? "aid" : input.sidx;
            List<string> queryA10 = input.a10 != null ? input.a10.Split(',').ToObject<List<string>>() : null;
            DateTime? startA10 = queryA10 != null ? Ext.GetDateTime(queryA10.First()) : null;
            DateTime? endA10 = queryA10 != null ? Ext.GetDateTime(queryA10.Last()) : null;
            var data = await _hltHrEmployeeRepository.AsSugarClient().Queryable<HltHrEmployeeEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.a1), a => a.A1.Contains(input.a1))
                .WhereIF(!string.IsNullOrEmpty(input.a2), a => a.A2.Contains(input.a2))
                .WhereIF(!string.IsNullOrEmpty(input.a3), a => a.A3.Contains(input.a3))
                .WhereIF(!string.IsNullOrEmpty(input.a4), a => a.A4.Contains(input.a4))
                .WhereIF(!string.IsNullOrEmpty(input.a5), a => a.A5.Contains(input.a5))
                .WhereIF(!string.IsNullOrEmpty(input.a9), a => a.A9.Contains(input.a9))
                .WhereIF(queryA10 != null, a => a.A10 >= new DateTime(startA10.ToDate().Year, startA10.ToDate().Month, startA10.ToDate().Day, 0, 0, 0))
                .WhereIF(queryA10 != null, a => a.A10 <= new DateTime(endA10.ToDate().Year, endA10.ToDate().Month, endA10.ToDate().Day, 23, 59, 59))
                .Select((a
)=> new HltHrEmployeeListOutput
                {
                    a1 = a.A1,
                    a2 = a.A2,
                    a3 = a.A3,
                    a4 = a.A4,
                    a5 = a.A5,
                    a6 = a.A6,
                    a7 = a.A7,
                    a8 = a.A8,
                    a9 = a.A9,
                    a10 = a.A10,
                    aid = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrEmployeeListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建员工信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrEmployeeCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrEmployeeEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrEmployeeRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取员工信息无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrEmployeeListQueryInput input)
        {
            var sidx = input.sidx == null ? "aid" : input.sidx;
            List<string> queryA10 = input.a10 != null ? input.a10.Split(',').ToObject<List<string>>() : null;
            DateTime? startA10 = queryA10 != null ? Ext.GetDateTime(queryA10.First()) : null;
            DateTime? endA10 = queryA10 != null ? Ext.GetDateTime(queryA10.Last()) : null;
            var data = await _hltHrEmployeeRepository.AsSugarClient().Queryable<HltHrEmployeeEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.a1), a => a.A1.Contains(input.a1))
                .WhereIF(!string.IsNullOrEmpty(input.a2), a => a.A2.Contains(input.a2))
                .WhereIF(!string.IsNullOrEmpty(input.a3), a => a.A3.Contains(input.a3))
                .WhereIF(!string.IsNullOrEmpty(input.a4), a => a.A4.Contains(input.a4))
                .WhereIF(!string.IsNullOrEmpty(input.a5), a => a.A5.Contains(input.a5))
                .WhereIF(!string.IsNullOrEmpty(input.a9), a => a.A9.Contains(input.a9))
                .WhereIF(queryA10 != null, a => a.A10 >= new DateTime(startA10.ToDate().Year, startA10.ToDate().Month, startA10.ToDate().Day, 0, 0, 0))
                .WhereIF(queryA10 != null, a => a.A10 <= new DateTime(endA10.ToDate().Year, endA10.ToDate().Month, endA10.ToDate().Day, 23, 59, 59))
                .Select((a
)=> new HltHrEmployeeListOutput
                {
                    a1 = a.A1,
                    a2 = a.A2,
                    a3 = a.A3,
                    a4 = a.A4,
                    a5 = a.A5,
                    a6 = a.A6,
                    a7 = a.A7,
                    a8 = a.A8,
                    a9 = a.A9,
                    a10 = a.A10,
                    aid = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出员工信息
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrEmployeeListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrEmployeeListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrEmployeeListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"新工号\",\"field\":\"a1\"},{\"value\":\"适配器工号\",\"field\":\"a2\"},{\"value\":\"嘉扬工号\",\"field\":\"a3\"},{\"value\":\"勤达工号\",\"field\":\"a4\"},{\"value\":\"F3工号\",\"field\":\"a5\"},{\"value\":\"姓名\",\"field\":\"a6\"},{\"value\":\"行政部门\",\"field\":\"a7\"},{\"value\":\"手机号码\",\"field\":\"a8\"},{\"value\":\"在职状态\",\"field\":\"a9\"},{\"value\":\"更新时间\",\"field\":\"a10\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "员工信息.xls";
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
            ExcelExportHelper<HltHrEmployeeListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除员工信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrEmployeeRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除员工信息
                    await _hltHrEmployeeRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新员工信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrEmployeeUpInput input)
        {
            var entity = input.Adapt<HltHrEmployeeEntity>();
            var isOk = await _hltHrEmployeeRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除员工信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrEmployeeRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrEmployeeRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


