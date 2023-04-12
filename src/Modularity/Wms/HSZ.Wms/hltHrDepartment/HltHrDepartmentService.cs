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
using HSZ.wms.Entitys.Dto.HltHrDepartment;
using HSZ.wms.Interfaces.HltHrDepartment;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrDepartment
{
    /// <summary>
    /// 部门信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrDepartment", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrDepartmentService : IHltHrDepartmentService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrDepartmentEntity> _hltHrDepartmentRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrDepartmentService"/>类型的新实例
        /// </summary>
        public HltHrDepartmentService(ISqlSugarRepository<HltHrDepartmentEntity> hltHrDepartmentRepository,
            IUserManager userManager)
        {
            _hltHrDepartmentRepository = hltHrDepartmentRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrDepartmentRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrDepartmentInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取部门信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrDepartmentListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrDepartmentRepository.AsSugarClient().Queryable<HltHrDepartmentEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.b1), a => a.B1.Contains(input.b1))
                .WhereIF(!string.IsNullOrEmpty(input.b2), a => a.B2.Contains(input.b2))
                .WhereIF(!string.IsNullOrEmpty(input.b3), a => a.B3.Contains(input.b3))
                .Select((a
)=> new HltHrDepartmentListOutput
                {
                    b1 = a.B1,
                    b2 = a.B2,
                    b3 = a.B3,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrDepartmentListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建部门信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrDepartmentCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrDepartmentEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrDepartmentRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取部门信息无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrDepartmentListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrDepartmentRepository.AsSugarClient().Queryable<HltHrDepartmentEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.b1), a => a.B1.Contains(input.b1))
                .WhereIF(!string.IsNullOrEmpty(input.b2), a => a.B2.Contains(input.b2))
                .WhereIF(!string.IsNullOrEmpty(input.b3), a => a.B3.Contains(input.b3))
                .Select((a
)=> new HltHrDepartmentListOutput
                {
                    b1 = a.B1,
                    b2 = a.B2,
                    b3 = a.B3,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出部门信息
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrDepartmentListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrDepartmentListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrDepartmentListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"部门名称\",\"field\":\"b1\"},{\"value\":\"部门编号\",\"field\":\"b2\"},{\"value\":\"状态\",\"field\":\"b3\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "部门信息.xls";
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
            ExcelExportHelper<HltHrDepartmentListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 更新部门信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrDepartmentUpInput input)
        {
            var entity = input.Adapt<HltHrDepartmentEntity>();
            var isOk = await _hltHrDepartmentRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除部门信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrDepartmentRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrDepartmentRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


