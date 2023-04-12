using HSZ.ClayObject;
using HSZ.Common.Configuration;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Common.Model.NPOI;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.wms.Entitys.Dto.ZjnWmsEquipmentList;
using HSZ.wms.Interfaces.ZjnWmsEquipmentList;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsEquipmentList
{
    /// <summary>
    /// 设备入库管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsEquipmentList", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsEquipmentListService : IZjnWmsEquipmentListService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsEquipmentListEntity> _zjnWmsEquipmentListRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsEquipmentListService"/>类型的新实例
        /// </summary>
        public ZjnWmsEquipmentListService(ISqlSugarRepository<ZjnWmsEquipmentListEntity> zjnWmsEquipmentListRepository,
            IUserManager userManager)
        {
            _zjnWmsEquipmentListRepository = zjnWmsEquipmentListRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取设备入库管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWmsEquipmentListRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsEquipmentListInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取设备入库管理列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsEquipmentListListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_id" : input.sidx;
            var data = await _zjnWmsEquipmentListRepository.AsSugarClient().Queryable<ZjnWmsEquipmentListEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_EquipmentSerialNumber), a => a.EquipmentSerialNumber.Contains(input.F_EquipmentSerialNumber))
                .WhereIF(!string.IsNullOrEmpty(input.F_DeviceName), a => a.DeviceName.Contains(input.F_DeviceName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Type), a => a.Type.Equals(input.F_Type))
                .Select((a
) => new ZjnWmsEquipmentListListOutput
{
    F_id = a.Id,
    F_EquipmentSerialNumber = a.EquipmentSerialNumber,
    F_DeviceName = a.DeviceName,
    F_Type = a.Type,
    F_TheBinding = a.TheBinding,
    F_PositionTo = a.PositionTo
}).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWmsEquipmentListListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建设备入库管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsEquipmentListCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsEquipmentListEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateTime = DateTime.Now;
            entity.CreateUser = userInfo.userId;

            var isOk = await _zjnWmsEquipmentListRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取设备入库管理无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnWmsEquipmentListListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_id" : input.sidx;
            var data = await _zjnWmsEquipmentListRepository.AsSugarClient().Queryable<ZjnWmsEquipmentListEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_EquipmentSerialNumber), a => a.EquipmentSerialNumber.Contains(input.F_EquipmentSerialNumber))
                .WhereIF(!string.IsNullOrEmpty(input.F_DeviceName), a => a.DeviceName.Contains(input.F_DeviceName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Type), a => a.Type.Equals(input.F_Type))
                .Select((a
) => new ZjnWmsEquipmentListListOutput
{
    F_id = a.Id,
    F_EquipmentSerialNumber = a.EquipmentSerialNumber,
    F_DeviceName = a.DeviceName,
    F_Type = a.Type,
}).OrderBy(sidx + " " + input.sort).ToListAsync();
            return data;
        }

        /// <summary>
		/// 导出设备入库管理
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnWmsEquipmentListListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnWmsEquipmentListListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnWmsEquipmentListListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"设备编号\",\"field\":\"equipmentSerialNumber\"},{\"value\":\"设备名称\",\"field\":\"deviceName\"},{\"value\":\"类型\",\"field\":\"type\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "设备入库管理.xls";
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
            ExcelExportHelper<ZjnWmsEquipmentListListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除设备入库管理
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnWmsEquipmentListRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除设备入库管理
                    await _zjnWmsEquipmentListRepository.AsDeleteable().In(d => d.Id, ids).ExecuteCommandAsync();
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
        /// 更新设备入库管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsEquipmentListUpInput input)
        {
            var entity = input.Adapt<ZjnWmsEquipmentListEntity>();
            var isOk = await _zjnWmsEquipmentListRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除设备入库管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWmsEquipmentListRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnWmsEquipmentListRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


