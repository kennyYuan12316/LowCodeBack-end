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
using HSZ.wms.Entitys.Dto.HltHrUnusual;
using HSZ.wms.Interfaces.HltHrUnusual;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrUnusual
{
    /// <summary>
    /// 实时异常服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrUnusual", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrUnusualService : IHltHrUnusualService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrUnusualEntity> _hltHrUnusualRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrUnusualService"/>类型的新实例
        /// </summary>
        public HltHrUnusualService(ISqlSugarRepository<HltHrUnusualEntity> hltHrUnusualRepository,
            IUserManager userManager)
        {
            _hltHrUnusualRepository = hltHrUnusualRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取实时异常
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrUnusualRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrUnusualInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取实时异常列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrUnusualListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrUnusualRepository.AsSugarClient().Queryable<HltHrUnusualEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.m1), a => a.M1.Contains(input.m1))
                .WhereIF(!string.IsNullOrEmpty(input.m3), a => a.M3.Contains(input.m3))
                .WhereIF(!string.IsNullOrEmpty(input.m4), a => a.M4.Contains(input.m4))
                .WhereIF(!string.IsNullOrEmpty(input.m5), a => a.M5.Contains(input.m5))
                .Select((a
)=> new HltHrUnusualListOutput
                {
                    m1 = a.M1,
                    m2 = a.M2,
                    m3 = a.M3,
                    m4 = a.M4,
                    m5 = a.M5,
                    m6 = a.M6,
                    m7 = a.M7,
                    m8 = a.M8,
                    m9 = a.M9,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrUnusualListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建实时异常
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrUnusualCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrUnusualEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrUnusualRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取实时异常无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrUnusualListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrUnusualRepository.AsSugarClient().Queryable<HltHrUnusualEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.m1), a => a.M1.Contains(input.m1))
                .WhereIF(!string.IsNullOrEmpty(input.m3), a => a.M3.Contains(input.m3))
                .WhereIF(!string.IsNullOrEmpty(input.m4), a => a.M4.Contains(input.m4))
                .WhereIF(!string.IsNullOrEmpty(input.m5), a => a.M5.Contains(input.m5))
                .Select((a
)=> new HltHrUnusualListOutput
                {
                    m1 = a.M1,
                    m2 = a.M2,
                    m3 = a.M3,
                    m4 = a.M4,
                    m5 = a.M5,
                    m6 = a.M6,
                    m7 = a.M7,
                    m8 = a.M8,
                    m9 = a.M9,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出实时异常
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrUnusualListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrUnusualListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrUnusualListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"错误信息标识 \",\"field\":\"m1\"},{\"value\":\"实时信息标识\",\"field\":\"m2\"},{\"value\":\"系统名称\",\"field\":\"m3\"},{\"value\":\"映射名称\",\"field\":\"m4\"},{\"value\":\"信息类型名称\",\"field\":\"m5\"},{\"value\":\"异常信息编码\",\"field\":\"m6\"},{\"value\":\"异常信息内容\",\"field\":\"m7\"},{\"value\":\"消息创建时间\",\"field\":\"m8\"},{\"value\":\"消息变更时间\",\"field\":\"m9\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "实时异常.xls";
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
            ExcelExportHelper<HltHrUnusualListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除实时异常
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrUnusualRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除实时异常
                    await _hltHrUnusualRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新实时异常
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrUnusualUpInput input)
        {
            var entity = input.Adapt<HltHrUnusualEntity>();
            var isOk = await _hltHrUnusualRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除实时异常
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrUnusualRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrUnusualRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


