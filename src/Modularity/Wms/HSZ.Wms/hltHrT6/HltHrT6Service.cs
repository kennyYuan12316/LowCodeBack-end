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
using HSZ.wms.Entitys.Dto.HltHrT6;
using HSZ.wms.Interfaces.HltHrT6;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrT6
{
    /// <summary>
    /// 接收类型模板服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrT6", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrT6Service : IHltHrT6Service, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrT6Entity> _hltHrT6Repository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrT6Service"/>类型的新实例
        /// </summary>
        public HltHrT6Service(ISqlSugarRepository<HltHrT6Entity> hltHrT6Repository,
            IUserManager userManager)
        {
            _hltHrT6Repository = hltHrT6Repository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取接收类型模板
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrT6Repository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrT6InfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取接收类型模板列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrT6ListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrT6Repository.AsSugarClient().Queryable<HltHrT6Entity>()
                .WhereIF(!string.IsNullOrEmpty(input.t2), a => a.T2.Contains(input.t2))
                .WhereIF(!string.IsNullOrEmpty(input.t3), a => a.T3.Contains(input.t3))
                .Select((a
)=> new HltHrT6ListOutput
                {
                    F_Id = a.Id,
                    t1 = a.T1,
                    t2 = a.T2,
                    t3 = a.T3,
                    t4 = a.T4,
                    t5 = a.T5,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrT6ListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建接收类型模板
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrT6CrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrT6Entity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrT6Repository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取接收类型模板无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrT6ListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrT6Repository.AsSugarClient().Queryable<HltHrT6Entity>()
                .WhereIF(!string.IsNullOrEmpty(input.t2), a => a.T2.Contains(input.t2))
                .WhereIF(!string.IsNullOrEmpty(input.t3), a => a.T3.Contains(input.t3))
                .Select((a
)=> new HltHrT6ListOutput
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
		/// 导出接收类型模板
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrT6ListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrT6ListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrT6ListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"编号\",\"field\":\"t1\"},{\"value\":\"接收类型\",\"field\":\"t2\"},{\"value\":\"接收属性模板名称\",\"field\":\"t3\"},{\"value\":\"接收属性模板原始编码\",\"field\":\"t4\"},{\"value\":\"接收属性模板转换编码\",\"field\":\"t5\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "接收类型模板.xls";
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
            ExcelExportHelper<HltHrT6ListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除接收类型模板
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrT6Repository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除接收类型模板
                    await _hltHrT6Repository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新接收类型模板
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrT6UpInput input)
        {
            var entity = input.Adapt<HltHrT6Entity>();
            var isOk = await _hltHrT6Repository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除接收类型模板
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrT6Repository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrT6Repository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


