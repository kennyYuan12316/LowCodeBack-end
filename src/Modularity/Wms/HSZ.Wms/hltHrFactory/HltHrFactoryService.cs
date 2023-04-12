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
using HSZ.wms.Entitys.Dto.HltHrFactory;
using HSZ.wms.Interfaces.HltHrFactory;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrFactory
{
    /// <summary>
    /// 厂区管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrFactory", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrFactoryService : IHltHrFactoryService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrFactoryEntity> _hltHrFactoryRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrFactoryService"/>类型的新实例
        /// </summary>
        public HltHrFactoryService(ISqlSugarRepository<HltHrFactoryEntity> hltHrFactoryRepository,
            IUserManager userManager)
        {
            _hltHrFactoryRepository = hltHrFactoryRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取厂区管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrFactoryRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrFactoryInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取厂区管理列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrFactoryListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrFactoryRepository.AsSugarClient().Queryable<HltHrFactoryEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.e2), a => a.E2.Contains(input.e2))
                .WhereIF(!string.IsNullOrEmpty(input.e3), a => a.E3.Contains(input.e3))
                .Select((a
)=> new HltHrFactoryListOutput
                {
                    e1 = a.E1,
                    e2 = a.E2,
                    e3 = a.E3,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrFactoryListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建厂区管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrFactoryCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrFactoryEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrFactoryRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取厂区管理无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrFactoryListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrFactoryRepository.AsSugarClient().Queryable<HltHrFactoryEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.e2), a => a.E2.Contains(input.e2))
                .WhereIF(!string.IsNullOrEmpty(input.e3), a => a.E3.Contains(input.e3))
                .Select((a
)=> new HltHrFactoryListOutput
                {
                    e1 = a.E1,
                    e2 = a.E2,
                    e3 = a.E3,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出厂区管理
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrFactoryListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrFactoryListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrFactoryListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"编号\",\"field\":\"e1\"},{\"value\":\"厂区名称\",\"field\":\"e2\"},{\"value\":\"厂区编码\",\"field\":\"e3\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "厂区管理.xls";
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
            ExcelExportHelper<HltHrFactoryListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 更新厂区管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrFactoryUpInput input)
        {
            var entity = input.Adapt<HltHrFactoryEntity>();
            var isOk = await _hltHrFactoryRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除厂区管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrFactoryRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrFactoryRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


