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
using HSZ.wms.Entitys.Dto.HltHrTemplate;
using HSZ.wms.Interfaces.HltHrTemplate;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrTemplate
{
    /// <summary>
    /// 系统模板参数管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrTemplate", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrTemplateService : IHltHrTemplateService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrTemplateEntity> _hltHrTemplateRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrTemplateService"/>类型的新实例
        /// </summary>
        public HltHrTemplateService(ISqlSugarRepository<HltHrTemplateEntity> hltHrTemplateRepository,
            IUserManager userManager)
        {
            _hltHrTemplateRepository = hltHrTemplateRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取系统模板参数管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrTemplateRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrTemplateInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取系统模板参数管理列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrTemplateListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrTemplateRepository.AsSugarClient().Queryable<HltHrTemplateEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.g2), a => a.G2.Contains(input.g2))
                .WhereIF(!string.IsNullOrEmpty(input.g3), a => a.G3.Contains(input.g3))
                .Select((a
)=> new HltHrTemplateListOutput
                {
                    g1 = a.G1,
                    g2 = a.G2,
                    g3 = a.G3,
                    g4 = a.G4,
                    g5 = a.G5,
                    g6 = a.G6,
                    g7 = a.G7,
                    g8 = a.G8,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrTemplateListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建系统模板参数管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrTemplateCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrTemplateEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrTemplateRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取系统模板参数管理无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrTemplateListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrTemplateRepository.AsSugarClient().Queryable<HltHrTemplateEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.g2), a => a.G2.Contains(input.g2))
                .WhereIF(!string.IsNullOrEmpty(input.g3), a => a.G3.Contains(input.g3))
                .Select((a
)=> new HltHrTemplateListOutput
                {
                    g1 = a.G1,
                    g2 = a.G2,
                    g3 = a.G3,
                    g4 = a.G4,
                    g5 = a.G5,
                    g6 = a.G6,
                    g7 = a.G7,
                    g8 = a.G8,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出系统模板参数管理
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrTemplateListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrTemplateListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrTemplateListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"编号\",\"field\":\"g1\"},{\"value\":\"系统名称\",\"field\":\"g2\"},{\"value\":\"系统模板名称\",\"field\":\"g3\"},{\"value\":\"调用类型值\",\"field\":\"g4\"},{\"value\":\"超时(毫秒)\",\"field\":\"g5\"},{\"value\":\"超时重派次数\",\"field\":\"g6\"},{\"value\":\"绕过SSL认证\",\"field\":\"g7\"},{\"value\":\"单次发送的数据总量\",\"field\":\"g8\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "系统模板参数管理.xls";
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
            ExcelExportHelper<HltHrTemplateListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除系统模板参数管理
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrTemplateRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除系统模板参数管理
                    await _hltHrTemplateRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新系统模板参数管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrTemplateUpInput input)
        {
            var entity = input.Adapt<HltHrTemplateEntity>();
            var isOk = await _hltHrTemplateRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除系统模板参数管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrTemplateRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrTemplateRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


