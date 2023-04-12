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
using HSZ.wms.Entitys.Dto.ZjnWmsRunLoginfo;
using HSZ.wms.Interfaces.ZjnWmsRunLoginfo;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using HSZ.wms.Entitys.Dto.ZjnWmsSupplier;

namespace HSZ.wms.ZjnWmsRunLoginfo
{
    /// <summary>
    /// wms运行日志服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWmsRunLoginfo", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsRunLoginfoService : IZjnWmsRunLoginfoService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsRunLoginfoEntity> _zjnWmsRunLoginfoRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsRunLoginfoService"/>类型的新实例
        /// </summary>
        public ZjnWmsRunLoginfoService(ISqlSugarRepository<ZjnWmsRunLoginfoEntity> zjnWmsRunLoginfoRepository,
            IUserManager userManager)
        {
            _zjnWmsRunLoginfoRepository = zjnWmsRunLoginfoRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取wms运行日志
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWmsRunLoginfoRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsRunLoginfoInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取wms运行日志列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsRunLoginfoListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWmsRunLoginfoRepository.AsSugarClient().Queryable<ZjnWmsRunLoginfoEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_TaskType), a => a.TaskType.Contains(input.F_TaskType))
                .WhereIF(!string.IsNullOrEmpty(input.F_MethodName), a => a.MethodName.Contains(input.F_MethodName))
                .WhereIF(!string.IsNullOrEmpty(input.F_TaskNo), a => a.TaskNo.Contains(input.F_TaskNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_DeviceNo), a => a.DeviceNo.Contains(input.F_DeviceNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .Select((a
)=> new ZjnWmsRunLoginfoListOutput
                {
                    F_Id = a.Id,
                    F_TaskType = a.TaskType,
                    F_MethodName = a.MethodName,
                    F_MethodParmes = a.MethodParmes,
                    F_TaskNo = a.TaskNo,
                    F_DeviceNo = a.DeviceNo,
                    F_TrayNo = a.TrayNo,
                    F_IsBug = a.IsBug,
                    F_Message = a.Message,
                    F_CreateTime = a.CreateTime,
                    F_Case1 = a.Case1,
                    F_Case2 = a.Case2,
                    F_Case3 = a.Case3,
                    F_Case4 = a.Case4,
                    F_Case5 = a.Case5,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWmsRunLoginfoListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建wms运行日志
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsRunLoginfoCrInput input)
        {
            //var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsRunLoginfoEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateTime=DateTime.Now;

            var isOk = await _zjnWmsRunLoginfoRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取wms运行日志无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnWmsRunLoginfoListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_TaskType" : input.sidx;
            var data = await _zjnWmsRunLoginfoRepository.AsSugarClient().Queryable<ZjnWmsRunLoginfoEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_TaskType), a => a.TaskType.Contains(input.F_TaskType))
                .WhereIF(!string.IsNullOrEmpty(input.F_MethodName), a => a.MethodName.Contains(input.F_MethodName))
                .WhereIF(!string.IsNullOrEmpty(input.F_TaskNo), a => a.TaskNo.Contains(input.F_TaskNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_DeviceNo), a => a.DeviceNo.Contains(input.F_DeviceNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .Select((a
)=> new ZjnWmsRunLoginfoListOutput
                {
                    F_Id = a.Id,
                    F_TaskType = a.TaskType,
                    F_MethodName = a.MethodName,
                    F_MethodParmes = a.MethodParmes,
                    F_TaskNo = a.TaskNo,
                    F_DeviceNo = a.DeviceNo,
                    F_TrayNo = a.TrayNo,
                    F_IsBug = a.IsBug,
                    F_Message = a.Message,
                    F_CreateTime = a.CreateTime,
                    F_Case1 = a.Case1,
                    F_Case2 = a.Case2,
                    F_Case3 = a.Case3,
                    F_Case4 = a.Case4,
                    F_Case5 = a.Case5,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出wms运行日志
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Export")]
        public async Task<dynamic> Export([FromQuery] ZjnWmsRunLoginfoListQueryInput input)
        {
            //var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnWmsRunLoginfoInfoOutput>();
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            if (input.dataType == 0)
            {
                //var data = Clay.Object(await this.GetList(input));
                //exportData = data.Solidify<PageResult<ZjnWmsRunLoginfoListOutput>>().list;

                exportData = await _zjnWmsRunLoginfoRepository.AsSugarClient().Queryable<ZjnWmsRunLoginfoEntity>()
                 .WhereIF(!string.IsNullOrEmpty(input.F_TaskType), a => a.TaskType.Contains(input.F_TaskType))
                .WhereIF(!string.IsNullOrEmpty(input.F_MethodName), a => a.MethodName.Contains(input.F_MethodName))
                .WhereIF(!string.IsNullOrEmpty(input.F_TaskNo), a => a.TaskNo.Contains(input.F_TaskNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_DeviceNo), a => a.DeviceNo.Contains(input.F_DeviceNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .Select((a) => new ZjnWmsRunLoginfoInfoOutput
                {
                    id = a.Id,
                    taskType = a.TaskType,
                    methodName = a.MethodName,
                    methodParmes = a.MethodParmes,
                    taskNo = a.TaskNo,
                    deviceNo = a.DeviceNo,
                    //isBug = a.IsBug,
                    isBug = SqlFunc.IF(a.IsBug.Equals(0)).Return("否").ElseIF(a.IsBug.Equals(1)).Return("是").End("否"),
                    message = a.Message,
                    createTime = a.CreateTime,
                    case1 = a.Case1,
                    case2 = a.Case2,
                    case3 = a.Case3,
                    case4 = a.Case4,
                    case5 = a.Case5,
                }).OrderBy(sidx + " " + input.sort).ToPageListAsync(input.currentPage, input.pageSize);
            }
            else
            {
                //exportData = await this.GetNoPagingList(input);
                exportData = await _zjnWmsRunLoginfoRepository.AsSugarClient().Queryable<ZjnWmsRunLoginfoEntity>()
                     .WhereIF(!string.IsNullOrEmpty(input.F_TaskType), a => a.TaskType.Contains(input.F_TaskType))
                .WhereIF(!string.IsNullOrEmpty(input.F_MethodName), a => a.MethodName.Contains(input.F_MethodName))
                .WhereIF(!string.IsNullOrEmpty(input.F_TaskNo), a => a.TaskNo.Contains(input.F_TaskNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_DeviceNo), a => a.DeviceNo.Contains(input.F_DeviceNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                    .Select((a) => new ZjnWmsRunLoginfoInfoOutput
                    {
                        id = a.Id,
                        taskType = a.TaskType,
                        methodName = a.MethodName,
                        methodParmes = a.MethodParmes,
                        taskNo = a.TaskNo,
                        deviceNo = a.DeviceNo,
                        //isBug = a.IsBug,
                        isBug = SqlFunc.IF(a.IsBug.Equals(0)).Return("否").ElseIF(a.IsBug.Equals(1)).Return("是").End("否"),
                        message = a.Message,
                        createTime = a.CreateTime,
                        case1 = a.Case1,
                        case2 = a.Case2,
                        case3 = a.Case3,
                        case4 = a.Case4,
                        case5 = a.Case5,
                    }).OrderBy(sidx + " " + input.sort).ToListAsync();
            }
            List<ParamsModel> paramList = "[{\"value\":\"业务类型\",\"field\":\"taskType\"},{\"value\":\"方法名\",\"field\":\"methodName\"},{\"value\":\"方法参数\",\"field\":\"methodParmes\"},{\"value\":\"任务号\",\"field\":\"taskNo\"},{\"value\":\"设备号\",\"field\":\"deviceNo\"},{\"value\":\"托盘号\",\"field\":\"trayNo\"},{\"value\":\"是否报错\",\"field\":\"isBug\"},{\"value\":\"创建时间\",\"field\":\"createTime\"},{\"value\":\"报错信息\",\"field\":\"message\"},{\"value\":\"备注1\",\"field\":\"case1\"},{\"value\":\"备注2\",\"field\":\"case2\"},{\"value\":\"备注3\",\"field\":\"case3\"},{\"value\":\"备注4\",\"field\":\"case4\"},{\"value\":\"备注5\",\"field\":\"case5\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = DateTime.Now.ToDateString() + "_wms运行日志.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetInfoFieldToTitle(input.selectKey.Split(',').ToList());
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnWmsRunLoginfoInfoOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }
        /// <summary>
        /// 字段对应 列名称
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetInfoFieldToTitle(List<string> fields = null)
        {
            var res = new Dictionary<string, string>();
            res.Add("taskType", "业务类型");
            res.Add("methodName", "方法名");
            //res.Add("methodParmes", "方法参数");
            res.Add("taskNo", "任务号");
            res.Add("deviceNo", "设备号");
            res.Add("trayNo", "托盘号");
            res.Add("isBug", "是否报错");
            //res.Add("message", "报错信息");
            res.Add("createTime", "创建时间");
            res.Add("case1", "备注1");
            res.Add("case2", "备注2");
            res.Add("case3", "备注3");
            res.Add("case4", "备注4");
            res.Add("case5", "备注5");

            if (fields == null || !fields.Any()) return res;
            var result = new Dictionary<string, string>();

            foreach (var item in res)
            {
                if (fields.Contains(item.Key))
                    result.Add(item.Key, item.Value);
            }
            return result;
        }

        /// <summary>
        /// 批量删除wms运行日志
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnWmsRunLoginfoRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除wms运行日志
                    await _zjnWmsRunLoginfoRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新wms运行日志
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsRunLoginfoUpInput input)
        {
            var entity = input.Adapt<ZjnWmsRunLoginfoEntity>();
            var isOk = await _zjnWmsRunLoginfoRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除wms运行日志
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWmsRunLoginfoRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnWmsRunLoginfoRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


