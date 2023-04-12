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
using HSZ.wms.Entitys.Dto.ZjnHardSrm;
using HSZ.wms.Interfaces.ZjnHardSrm;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnHardSrm
{
    /// <summary>
    /// 堆垛机信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnHardSrm", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnHardSrmService : IZjnHardSrmService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnHardSrmEntity> _zjnHardSrmRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnHardSrmService"/>类型的新实例
        /// </summary>
        public ZjnHardSrmService(ISqlSugarRepository<ZjnHardSrmEntity> zjnHardSrmRepository,
            IUserManager userManager)
        {
            _zjnHardSrmRepository = zjnHardSrmRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取堆垛机信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnHardSrmRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnHardSrmInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取堆垛机信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnHardSrmListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnHardSrmRepository.AsSugarClient().Queryable<ZjnHardSrmEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_HardNo), a => a.HardNo.Contains(input.F_HardNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Select((a
)=> new ZjnHardSrmListOutput
                {
                    F_Id = a.Id,
                    F_HardNo = a.HardNo,
                    F_HardName = a.HardName,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = a.EnabledMark,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnHardSrmListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建堆垛机信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnHardSrmCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnHardSrmEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            
            var isOk = await _zjnHardSrmRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取堆垛机信息无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnHardSrmListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnHardSrmRepository.AsSugarClient().Queryable<ZjnHardSrmEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_HardNo), a => a.HardNo.Contains(input.F_HardNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Select((a
)=> new ZjnHardSrmListOutput
                {
                    F_Id = a.Id,
                    F_HardNo = a.HardNo,
                    F_HardName = a.HardName,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = a.EnabledMark,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出堆垛机信息
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnHardSrmListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnHardSrmListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnHardSrmListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"设备编号\",\"field\":\"hardNo\"},{\"value\":\"设备名称\",\"field\":\"hardName\"},{\"value\":\"创建者\",\"field\":\"createUser\"},{\"value\":\"创建时间\",\"field\":\"createTime\"},{\"value\":\"有效标志\",\"field\":\"enabledMark\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "堆垛机信息.xls";
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
            ExcelExportHelper<ZjnHardSrmListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除堆垛机信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnHardSrmRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除堆垛机信息
                    await _zjnHardSrmRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新堆垛机信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnHardSrmUpInput input)
        {
            var entity = input.Adapt<ZjnHardSrmEntity>();
            var isOk = await _zjnHardSrmRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除堆垛机信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnHardSrmRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnHardSrmRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


