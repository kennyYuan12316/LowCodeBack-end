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
using HSZ.wms.Entitys.Dto.HltHrShineupon;
using HSZ.wms.Interfaces.HltHrShineupon;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrShineupon
{
    /// <summary>
    /// 映射管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrShineupon", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrShineuponService : IHltHrShineuponService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrShineuponEntity> _hltHrShineuponRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrShineuponService"/>类型的新实例
        /// </summary>
        public HltHrShineuponService(ISqlSugarRepository<HltHrShineuponEntity> hltHrShineuponRepository,
            IUserManager userManager)
        {
            _hltHrShineuponRepository = hltHrShineuponRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取映射管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrShineuponRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrShineuponInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取映射管理列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrShineuponListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrShineuponRepository.AsSugarClient().Queryable<HltHrShineuponEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.h2), a => a.H2.Contains(input.h2))
                .WhereIF(!string.IsNullOrEmpty(input.h3), a => a.H3.Contains(input.h3))
                .WhereIF(!string.IsNullOrEmpty(input.h4), a => a.H4.Contains(input.h4))
                .Select((a
)=> new HltHrShineuponListOutput
                {
                    h1 = a.H1,
                    h2 = a.H2,
                    h3 = a.H3,
                    h4 = a.H4,
                    h5 = a.H5,
                    h6 = a.H6,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrShineuponListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建映射管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrShineuponCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrShineuponEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrShineuponRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取映射管理无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrShineuponListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrShineuponRepository.AsSugarClient().Queryable<HltHrShineuponEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.h2), a => a.H2.Contains(input.h2))
                .WhereIF(!string.IsNullOrEmpty(input.h3), a => a.H3.Contains(input.h3))
                .WhereIF(!string.IsNullOrEmpty(input.h4), a => a.H4.Contains(input.h4))
                .Select((a
)=> new HltHrShineuponListOutput
                {
                    h1 = a.H1,
                    h2 = a.H2,
                    h3 = a.H3,
                    h4 = a.H4,
                    h5 = a.H5,
                    h6 = a.H6,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出映射管理
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrShineuponListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrShineuponListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrShineuponListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"编号\",\"field\":\"h1\"},{\"value\":\"映射名称\",\"field\":\"h2\"},{\"value\":\"映射编码\",\"field\":\"h3\"},{\"value\":\"系统名称\",\"field\":\"h4\"},{\"value\":\"接口名称\",\"field\":\"h5\"},{\"value\":\"数据起始时间\",\"field\":\"h6\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "映射管理.xls";
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
            ExcelExportHelper<HltHrShineuponListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除映射管理
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrShineuponRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除映射管理
                    await _hltHrShineuponRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新映射管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrShineuponUpInput input)
        {
            var entity = input.Adapt<HltHrShineuponEntity>();
            var isOk = await _hltHrShineuponRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除映射管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrShineuponRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrShineuponRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


