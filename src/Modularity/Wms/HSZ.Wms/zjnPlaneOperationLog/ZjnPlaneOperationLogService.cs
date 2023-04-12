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
using HSZ.wms.Entitys.Dto.ZjnPlaneOperationLog;
using HSZ.wms.Interfaces.ZjnPlaneOperationLog;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnPlaneOperationLog
{
    /// <summary>
    /// 平面库操作日志信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnPlaneOperationLog", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnPlaneOperationLogService : IZjnPlaneOperationLogService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnPlaneOperationLogService"/>类型的新实例
        /// </summary>
        public ZjnPlaneOperationLogService(ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository,
            IUserManager userManager)
        {
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取平面库操作日志信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnPlaneOperationLogRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnPlaneOperationLogInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取平面库操作日志信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnPlaneOperationLogListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnPlaneOperationLogRepository.AsSugarClient().Queryable<ZjnWmsOperationLogEntity>()
                .WhereIF(input.F_Type!=null, a => a.Type == input.F_Type)
                .WhereIF(!string.IsNullOrEmpty(input.F_Describe), a => a.Describe.Contains(input.F_Describe))
                .WhereIF(input.F_WorkPath!=null, a => a.WorkPath == input.F_WorkPath)
                .Select((a
)=> new ZjnPlaneOperationLogListOutput
                {
                    F_Id = a.Id,
                    F_Type = a.Type.ToString(),
                    F_Describe = a.Describe,
                    F_WorkPath = a.WorkPath,
                    F_BeforeDate = a.BeforeDate,
                    F_AfterDate = a.AfterDate,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnPlaneOperationLogListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
		/// 获取平面库操作日志信息无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnPlaneOperationLogListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnPlaneOperationLogRepository.AsSugarClient().Queryable<ZjnWmsOperationLogEntity>()
                .WhereIF(input.F_Type != -1, a => a.Type == 1)
                .WhereIF(!string.IsNullOrEmpty(input.F_Describe), a => a.Describe.Contains(input.F_Describe))
                .WhereIF(input.F_WorkPath != -1, a => a.WorkPath == input.F_WorkPath)
                .Select((a
)=> new ZjnPlaneOperationLogListOutput
                {
                    F_Id = a.Id,
                    F_Type = a.Type.ToString(),
                    F_Describe = a.Describe,
                    F_WorkPath = a.WorkPath,
                    F_BeforeDate = a.BeforeDate,
                    F_AfterDate = a.AfterDate,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出平面库操作日志信息
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnPlaneOperationLogListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnPlaneOperationLogListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnPlaneOperationLogListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"操作类型\",\"field\":\"F_Type\"},{\"value\":\"操作描述\",\"field\":\"F_Describe\"},{\"value\":\"操作路径\",\"field\":\"F_WorkPath\"},{\"value\":\"操作前数据\",\"field\":\"F_BeforeDate\"},{\"value\":\"操作后数据\",\"field\":\"F_AfterDate\"},{\"value\":\"操作人\",\"field\":\"F_CreateUser\"},{\"value\":\"操作时间\",\"field\":\"F_CreateTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "操作日志信息.xls";
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
            ExcelExportHelper<ZjnPlaneOperationLogListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 新增日记
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Create([FromBody] ZjnPlaneOperationLogCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsOperationLogEntity>();
            

            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = userInfo.userId;
            entity.CreateTime = DateTime.Now;
            

            var isOk = await _zjnPlaneOperationLogRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }


    }
}


