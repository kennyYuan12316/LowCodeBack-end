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
using HSZ.wms.Entitys.Dto.HltHrRegular;
using HSZ.wms.Interfaces.HltHrRegular;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrRegular
{
    /// <summary>
    /// 定时任务服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrRegular", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrRegularService : IHltHrRegularService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrRegularEntity> _hltHrRegularRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrRegularService"/>类型的新实例
        /// </summary>
        public HltHrRegularService(ISqlSugarRepository<HltHrRegularEntity> hltHrRegularRepository,
            IUserManager userManager)
        {
            _hltHrRegularRepository = hltHrRegularRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取定时任务
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrRegularRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrRegularInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取定时任务列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrRegularListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrRegularRepository.AsSugarClient().Queryable<HltHrRegularEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.i2), a => a.I2.Contains(input.i2))
                .WhereIF(!string.IsNullOrEmpty(input.i3), a => a.I3.Contains(input.i3))
                .WhereIF(!string.IsNullOrEmpty(input.i4), a => a.I4.Contains(input.i4))
                .WhereIF(!string.IsNullOrEmpty(input.i6), a => a.I6.Contains(input.i6))
                .Select((a
)=> new HltHrRegularListOutput
                {
                    i1 = a.I1,
                    i2 = a.I2,
                    i3 = a.I3,
                    i4 = a.I4,
                    i5 = a.I5,
                    i6 = a.I6,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrRegularListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建定时任务
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrRegularCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrRegularEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrRegularRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取定时任务无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrRegularListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrRegularRepository.AsSugarClient().Queryable<HltHrRegularEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.i2), a => a.I2.Contains(input.i2))
                .WhereIF(!string.IsNullOrEmpty(input.i3), a => a.I3.Contains(input.i3))
                .WhereIF(!string.IsNullOrEmpty(input.i4), a => a.I4.Contains(input.i4))
                .WhereIF(!string.IsNullOrEmpty(input.i6), a => a.I6.Contains(input.i6))
                .Select((a
)=> new HltHrRegularListOutput
                {
                    i1 = a.I1,
                    i2 = a.I2,
                    i3 = a.I3,
                    i4 = a.I4,
                    i5 = a.I5,
                    i6 = a.I6,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出定时任务
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrRegularListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrRegularListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrRegularListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"编号\",\"field\":\"i1\"},{\"value\":\"任务名称\",\"field\":\"i2\"},{\"value\":\"映射名称\",\"field\":\"i3\"},{\"value\":\"系统名称\",\"field\":\"i4\"},{\"value\":\"任务时间表达式\",\"field\":\"i5\"},{\"value\":\"任务状态\",\"field\":\"i6\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "定时任务.xls";
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
            ExcelExportHelper<HltHrRegularListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除定时任务
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrRegularRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除定时任务
                    await _hltHrRegularRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新定时任务
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrRegularUpInput input)
        {
            var entity = input.Adapt<HltHrRegularEntity>();
            var isOk = await _hltHrRegularRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除定时任务
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrRegularRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrRegularRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


