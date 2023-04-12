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
using HSZ.wms.Entitys.Dto.HltHrError;
using HSZ.wms.Interfaces.HltHrError;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrError
{
    /// <summary>
    /// 错误日志管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrError", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrErrorService : IHltHrErrorService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrErrorEntity> _hltHrErrorRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrErrorService"/>类型的新实例
        /// </summary>
        public HltHrErrorService(ISqlSugarRepository<HltHrErrorEntity> hltHrErrorRepository,
            IUserManager userManager)
        {
            _hltHrErrorRepository = hltHrErrorRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取错误日志管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrErrorRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrErrorInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取错误日志管理列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrErrorListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrErrorRepository.AsSugarClient().Queryable<HltHrErrorEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.k2), a => a.K2.Contains(input.k2))
                .WhereIF(!string.IsNullOrEmpty(input.k5), a => a.K5.Contains(input.k5))
                .Select((a
)=> new HltHrErrorListOutput
                {
                    k1 = a.K1,
                    k2 = a.K2,
                    k3 = a.K3,
                    k4 = a.K4,
                    k5 = a.K5,
                    k6 = a.K6,
                    k7 = a.K7,
                    k8 = a.K8,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrErrorListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建错误日志管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrErrorCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrErrorEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrErrorRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取错误日志管理无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrErrorListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrErrorRepository.AsSugarClient().Queryable<HltHrErrorEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.k2), a => a.K2.Contains(input.k2))
                .WhereIF(!string.IsNullOrEmpty(input.k5), a => a.K5.Contains(input.k5))
                .Select((a
)=> new HltHrErrorListOutput
                {
                    k1 = a.K1,
                    k2 = a.K2,
                    k3 = a.K3,
                    k4 = a.K4,
                    k5 = a.K5,
                    k6 = a.K6,
                    k7 = a.K7,
                    k8 = a.K8,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出错误日志管理
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrErrorListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrErrorListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrErrorListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"日志编号\",\"field\":\"k1\"},{\"value\":\"系统模块\",\"field\":\"k2\"},{\"value\":\"系统标识\",\"field\":\"k3\"},{\"value\":\"请求方式\",\"field\":\"k4\"},{\"value\":\"操作人员\",\"field\":\"k5\"},{\"value\":\"请求系统\",\"field\":\"k6\"},{\"value\":\"响应系统\",\"field\":\"k7\"},{\"value\":\"操作日期\",\"field\":\"k8\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "错误日志管理.xls";
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
            ExcelExportHelper<HltHrErrorListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除错误日志管理
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrErrorRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除错误日志管理
                    await _hltHrErrorRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新错误日志管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrErrorUpInput input)
        {
            var entity = input.Adapt<HltHrErrorEntity>();
            var isOk = await _hltHrErrorRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除错误日志管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrErrorRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrErrorRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


