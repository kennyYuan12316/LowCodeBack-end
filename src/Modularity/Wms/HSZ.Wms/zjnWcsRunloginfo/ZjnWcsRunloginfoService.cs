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
using HSZ.wms.Entitys.Dto.ZjnWcsRunloginfo;
using HSZ.wms.Interfaces.ZjnWcsRunloginfo;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using HSZ.wms.Entitys.Dto.BaseSupplier;

namespace HSZ.wms.ZjnWcsRunloginfo
{
    /// <summary>
    /// 运行日志服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWcsRunloginfo", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWcsRunloginfoService : IZjnWcsRunloginfoService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsRunloginfoEntity> _zjnWcsRunloginfoRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWcsRunloginfoService"/>类型的新实例
        /// </summary>
        public ZjnWcsRunloginfoService(ISqlSugarRepository<ZjnWcsRunloginfoEntity> zjnWcsRunloginfoRepository,
            IUserManager userManager)
        {
            _zjnWcsRunloginfoRepository = zjnWcsRunloginfoRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取运行日志
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWcsRunloginfoRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWcsRunloginfoInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取运行日志列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWcsRunloginfoListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWcsRunloginfoRepository.AsSugarClient().Queryable<ZjnWcsRunloginfoEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_EquipmentCode), a => a.EquipmentCode.Contains(input.F_EquipmentCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_RunType), a => a.RunType.Contains(input.F_RunType))
                .Select((a
)=> new ZjnWcsRunloginfoListOutput
                {
                    F_Id = a.Id,
                    F_ContainerBarcode1 = a.ContainerBarcode1,
                    F_ContainerBarcode2 = a.ContainerBarcode2,
                    F_TaskCode1 = a.TaskCode1,
                    F_TaskCode2 = a.TaskCode2,
                    F_EquipmentCode = a.EquipmentCode,
                    F_RunLog = a.RunLog,
                    F_RunType = a.RunType,
                    F_CreateTime = a.CreateTime,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWcsRunloginfoListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建运行日志
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWcsRunloginfoCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWcsRunloginfoEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnWcsRunloginfoRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取运行日志无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnWcsRunloginfoListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWcsRunloginfoRepository.AsSugarClient().Queryable<ZjnWcsRunloginfoEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_EquipmentCode), a => a.EquipmentCode.Contains(input.F_EquipmentCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_RunType), a => a.RunType.Contains(input.F_RunType))
                .Select((a
)=> new ZjnWcsRunloginfoListOutput
                {
                    F_Id = a.Id,
                    F_ContainerBarcode1 = a.ContainerBarcode1,
                    F_ContainerBarcode2 = a.ContainerBarcode2,
                    F_TaskCode1 = a.TaskCode1,
                    F_TaskCode2 = a.TaskCode2,
                    F_EquipmentCode = a.EquipmentCode,
                    F_RunLog = a.RunLog,
                    F_RunType = a.RunType,
                    F_CreateTime = a.CreateTime,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出运行日志
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Export")]
        public async Task<dynamic> Export([FromQuery] ZjnWcsRunloginfoListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnWcsRunloginfoListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnWcsRunloginfoListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"业务类型\",\"field\":\"runType\"},{\"value\":\"设备号\",\"field\":\"equipmentCode\"},{\"value\":\"任务号1\",\"field\":\"taskCode1\"},{\"value\":\"任务号2\",\"field\":\"taskCode2\"},{\"value\":\"托盘条码1\",\"field\":\"containerBarcode1\"},{\"value\":\"托盘条码2\",\"field\":\"containerBarcode2\"},{\"value\":\"日志信息\",\"field\":\"runLog\"},{\"value\":\"创建时间\",\"field\":\"createTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = DateTime.Now.ToDateString() + "_运行日志.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            //List<string> selectKeyList = input.selectKey.Split(',').ToList();
            var filedList = GetInfoFieldToTitle(input.selectKey.Split(',').ToList());
            foreach (var item in filedList)
            {
                //var isExist = paramList.Find(p => p.field == item);
                //if (isExist != null)
                //{
                //    excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = isExist.field, ExcelColumn = isExist.value });
                //}

                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnWcsRunloginfoListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;

            //var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            //ExcelExportHelper<ZjnWcsRunloginfoListOutput>.Export(exportData, excelconfig, addPath);

            //return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };


        }
        /// <summary>
        /// 字段对应 列名称
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetInfoFieldToTitle(List<string> fields = null)
        {
            var res = new Dictionary<string, string>();
            res.Add("F_RunType", "业务类型");
            res.Add("F_EquipmentCode", "设备号");
            res.Add("F_TaskCode1", "任务号1");
            res.Add("F_TaskCode2", "任务号2");
            res.Add("F_ContainerBarcode1", "托盘条码1");
            res.Add("F_ContainerBarcode2", "托盘条码2");
            //res.Add("F_RunLog", "日志信息");
            res.Add("F_CreateTime", "创建时间");

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
        /// 批量删除运行日志
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnWcsRunloginfoRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除运行日志
                    await _zjnWcsRunloginfoRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新运行日志
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWcsRunloginfoUpInput input)
        {
            var entity = input.Adapt<ZjnWcsRunloginfoEntity>();
            var isOk = await _zjnWcsRunloginfoRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除运行日志
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWcsRunloginfoRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnWcsRunloginfoRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


