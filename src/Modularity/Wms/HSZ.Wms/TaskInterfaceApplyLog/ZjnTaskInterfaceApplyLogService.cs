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
using HSZ.wms.Entitys.Dto.ZjnTaskInterfaceApplyLog;
using HSZ.wms.Interfaces.ZjnTaskInterfaceApplyLog;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnTaskInterfaceApplyLog
{
    /// <summary>
    /// 接口调用履历表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnTaskInterfaceApplyLog", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnTaskInterfaceApplyLogService : IZjnTaskInterfaceApplyLogService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnTaskInterfaceApplyLogEntity> _zjnTaskInterfaceApplyLogRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnTaskInterfaceApplyLogService"/>类型的新实例
        /// </summary>
        public ZjnTaskInterfaceApplyLogService(ISqlSugarRepository<ZjnTaskInterfaceApplyLogEntity> zjnTaskInterfaceApplyLogRepository,
            IUserManager userManager)
        {
            _zjnTaskInterfaceApplyLogRepository = zjnTaskInterfaceApplyLogRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取接口调用履历表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnTaskInterfaceApplyLogRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnTaskInterfaceApplyLogInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取接口调用履历表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnTaskInterfaceApplyLogListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<string> queryCreateTime = input.F_CreateTime != null ? input.F_CreateTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startCreateTime = queryCreateTime != null ? Ext.GetDateTime(queryCreateTime.First()) : null;
            DateTime? endCreateTime = queryCreateTime != null ? Ext.GetDateTime(queryCreateTime.Last()) : null;
            var data = await _zjnTaskInterfaceApplyLogRepository.AsSugarClient().Queryable<ZjnTaskInterfaceApplyLogEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_InterfaceName), a => a.InterfaceName.Contains(input.F_InterfaceName))
                .WhereIF(queryCreateTime != null, a => a.CreateTime >= new DateTime(startCreateTime.ToDate().Year, startCreateTime.ToDate().Month, startCreateTime.ToDate().Day, 0, 0, 0))
                .WhereIF(queryCreateTime != null, a => a.CreateTime <= new DateTime(endCreateTime.ToDate().Year, endCreateTime.ToDate().Month, endCreateTime.ToDate().Day, 23, 59, 59))
                .Select((a
)=> new ZjnTaskInterfaceApplyLogListOutput
                {
                    F_Id = a.Id,
                    F_Address = a.Address,
                    F_InterfaceName = a.InterfaceName,
                    F_EnterParameter = a.EnterParameter,
                    F_OutParameter = a.OutParameter,
                    F_CreateTime = a.CreateTime,
                    F_Msg = a.Msg,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnTaskInterfaceApplyLogListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建接口调用履历表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnTaskInterfaceApplyLogCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnTaskInterfaceApplyLogEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateTime = DateTime.Now;
            
            var isOk = await _zjnTaskInterfaceApplyLogRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取接口调用履历表无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnTaskInterfaceApplyLogListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<string> queryCreateTime = input.F_CreateTime != null ? input.F_CreateTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startCreateTime = queryCreateTime != null ? Ext.GetDateTime(queryCreateTime.First()) : null;
            DateTime? endCreateTime = queryCreateTime != null ? Ext.GetDateTime(queryCreateTime.Last()) : null;
            var data = await _zjnTaskInterfaceApplyLogRepository.AsSugarClient().Queryable<ZjnTaskInterfaceApplyLogEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_InterfaceName), a => a.InterfaceName.Contains(input.F_InterfaceName))
                .WhereIF(queryCreateTime != null, a => a.CreateTime >= new DateTime(startCreateTime.ToDate().Year, startCreateTime.ToDate().Month, startCreateTime.ToDate().Day, 0, 0, 0))
                .WhereIF(queryCreateTime != null, a => a.CreateTime <= new DateTime(endCreateTime.ToDate().Year, endCreateTime.ToDate().Month, endCreateTime.ToDate().Day, 23, 59, 59))
                .Select((a
)=> new ZjnTaskInterfaceApplyLogListOutput
                {
                    F_Id = a.Id,
                    F_Address = a.Address,
                    F_InterfaceName = a.InterfaceName,
                    F_EnterParameter = a.EnterParameter,
                    F_OutParameter = a.OutParameter,
                    F_CreateTime = a.CreateTime,
                    F_Msg = a.Msg,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出接口调用履历表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnTaskInterfaceApplyLogListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnTaskInterfaceApplyLogListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnTaskInterfaceApplyLogListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"接口名\",\"field\":\"interfaceName\"},{\"value\":\"调用完整地址\",\"field\":\"address\"},{\"value\":\"入参\",\"field\":\"enterParameter\"},{\"value\":\"出参\",\"field\":\"outParameter\"},{\"value\":\"消息\",\"field\":\"msg\"},{\"value\":\"调用时间\",\"field\":\"createTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "接口调用履历表.xls";
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
            ExcelExportHelper<ZjnTaskInterfaceApplyLogListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除接口调用履历表
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnTaskInterfaceApplyLogRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除接口调用履历表
                    await _zjnTaskInterfaceApplyLogRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 删除接口调用履历表
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnTaskInterfaceApplyLogRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnTaskInterfaceApplyLogRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


