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
using HSZ.wms.Entitys.Dto.HltHrT4;
using HSZ.wms.Interfaces.HltHrT4;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrT4
{
    /// <summary>
    /// 系统管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrT4", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrT4Service : IHltHrT4Service, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrT4Entity> _hltHrT4Repository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrT4Service"/>类型的新实例
        /// </summary>
        public HltHrT4Service(ISqlSugarRepository<HltHrT4Entity> hltHrT4Repository,
            IUserManager userManager)
        {
            _hltHrT4Repository = hltHrT4Repository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取系统管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrT4Repository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrT4InfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取系统管理列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrT4ListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrT4Repository.AsSugarClient().Queryable<HltHrT4Entity>()
                .WhereIF(!string.IsNullOrEmpty(input.t2), a => a.T2.Contains(input.t2))
                .Select((a
)=> new HltHrT4ListOutput
                {
                    F_Id = a.Id,
                    t1 = a.T1,
                    t2 = a.T2,
                    t3 = a.T3,
                    t4 = a.T4,
                    t5 = a.T5,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrT4ListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建系统管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrT4CrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrT4Entity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrT4Repository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取系统管理无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrT4ListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrT4Repository.AsSugarClient().Queryable<HltHrT4Entity>()
                .WhereIF(!string.IsNullOrEmpty(input.t2), a => a.T2.Contains(input.t2))
                .Select((a
)=> new HltHrT4ListOutput
                {
                    F_Id = a.Id,
                    t1 = a.T1,
                    t2 = a.T2,
                    t3 = a.T3,
                    t4 = a.T4,
                    t5 = a.T5,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出系统管理
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrT4ListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrT4ListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrT4ListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"编号\",\"field\":\"t1\"},{\"value\":\"系统名称\",\"field\":\"t2\"},{\"value\":\"系统编码\",\"field\":\"t3\"},{\"value\":\"同步序列组\",\"field\":\"t4\"},{\"value\":\"下一个同步序列\",\"field\":\"t5\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "系统管理.xls";
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
            ExcelExportHelper<HltHrT4ListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除系统管理
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrT4Repository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除系统管理
                    await _hltHrT4Repository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新系统管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrT4UpInput input)
        {
            var entity = input.Adapt<HltHrT4Entity>();
            var isOk = await _hltHrT4Repository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除系统管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrT4Repository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrT4Repository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


