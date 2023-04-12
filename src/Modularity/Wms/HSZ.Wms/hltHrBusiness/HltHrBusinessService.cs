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
using HSZ.wms.Entitys.Dto.HltHrBusiness;
using HSZ.wms.Interfaces.HltHrBusiness;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrBusiness
{
    /// <summary>
    /// 业务模板组管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrBusiness", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrBusinessService : IHltHrBusinessService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrBusinessEntity> _hltHrBusinessRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrBusinessService"/>类型的新实例
        /// </summary>
        public HltHrBusinessService(ISqlSugarRepository<HltHrBusinessEntity> hltHrBusinessRepository,
            IUserManager userManager)
        {
            _hltHrBusinessRepository = hltHrBusinessRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取业务模板组管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrBusinessRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrBusinessInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取业务模板组管理列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrBusinessListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrBusinessRepository.AsSugarClient().Queryable<HltHrBusinessEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.d2), a => a.D2.Contains(input.d2))
                .WhereIF(!string.IsNullOrEmpty(input.d3), a => a.D3.Contains(input.d3))
                .Select((a
)=> new HltHrBusinessListOutput
                {
                    d1 = a.D1,
                    d2 = a.D2,
                    d3 = a.D3,
                    d4 = a.D4,
                    d5 = a.D5,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrBusinessListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建业务模板组管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrBusinessCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrBusinessEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrBusinessRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取业务模板组管理无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrBusinessListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrBusinessRepository.AsSugarClient().Queryable<HltHrBusinessEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.d2), a => a.D2.Contains(input.d2))
                .WhereIF(!string.IsNullOrEmpty(input.d3), a => a.D3.Contains(input.d3))
                .Select((a
)=> new HltHrBusinessListOutput
                {
                    d1 = a.D1,
                    d2 = a.D2,
                    d3 = a.D3,
                    d4 = a.D4,
                    d5 = a.D5,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出业务模板组管理
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrBusinessListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrBusinessListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrBusinessListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"编号\",\"field\":\"d1\"},{\"value\":\"模块组名称\",\"field\":\"d2\"},{\"value\":\"模块组编码\",\"field\":\"d3\"},{\"value\":\"所属部门名称\",\"field\":\"d4\"},{\"value\":\"负责人名称\",\"field\":\"d5\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "业务模板组管理.xls";
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
            ExcelExportHelper<HltHrBusinessListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除业务模板组管理
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrBusinessRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除业务模板组管理
                    await _hltHrBusinessRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新业务模板组管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrBusinessUpInput input)
        {
            var entity = input.Adapt<HltHrBusinessEntity>();
            var isOk = await _hltHrBusinessRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除业务模板组管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrBusinessRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrBusinessRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


