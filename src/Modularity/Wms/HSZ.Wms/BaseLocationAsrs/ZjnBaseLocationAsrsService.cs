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
using HSZ.wms.Entitys.Dto.ZjnBaseLocationAsrs;
using HSZ.wms.Interfaces.ZjnBaseLocationAsrs;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using HSZ.System.Entitys.Permission;
using HSZ.wms.Entitys.Dto.ZjnPlaneGoods;
using HSZ.System.Entitys.System;
using HSZ.Wms.Entitys.Dto.BaseLocationAsrs;

namespace HSZ.wms.ZjnBaseLocationAsrs
{
    /// <summary>
    /// 立库货位信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseLocationAsrs", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseLocationAsrsService : IZjnBaseLocationAsrsService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseLocationAsrsEntity> _zjnBaseLocationAsrsRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        /// <summary>
        /// 初始化一个<see cref="ZjnBaseLocationAsrsService"/>类型的新实例
        /// </summary>
        public ZjnBaseLocationAsrsService(ISqlSugarRepository<ZjnBaseLocationAsrsEntity> zjnBaseLocationAsrsRepository, ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository,
            IUserManager userManager)
        {
            _dictionaryDataRepository = dictionaryDataRepository;
            _zjnBaseLocationAsrsRepository = zjnBaseLocationAsrsRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取立库货位信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseLocationAsrsRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseLocationAsrsInfoOutput>();
            return output;
        }


        /// <summary>
		/// 获取立库货位信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseLocationAsrsListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_LocationNo" : input.sidx;
            List<object> queryRow = input.F_Row != null ? input.F_Row.Split(',').ToObject<List<object>>() : null;
            var startRow = input.F_Row != null && !string.IsNullOrEmpty(queryRow.First().ToString()) ? queryRow.First() : decimal.MinValue;
            var endRow = input.F_Row != null && !string.IsNullOrEmpty(queryRow.Last().ToString()) ? queryRow.Last() : decimal.MaxValue;
            List<object> queryCell = input.F_Cell != null ? input.F_Cell.Split(',').ToObject<List<object>>() : null;
            var startCell = input.F_Cell != null && !string.IsNullOrEmpty(queryCell.First().ToString()) ? queryCell.First() : decimal.MinValue;
            var endCell = input.F_Cell != null && !string.IsNullOrEmpty(queryCell.Last().ToString()) ? queryCell.Last() : decimal.MaxValue;
            List<object> queryLayer = input.F_Layer != null ? input.F_Layer.Split(',').ToObject<List<object>>() : null;
            var startLayer = input.F_Layer != null && !string.IsNullOrEmpty(queryLayer.First().ToString()) ? queryLayer.First() : decimal.MinValue;
            var endLayer = input.F_Layer != null && !string.IsNullOrEmpty(queryLayer.Last().ToString()) ? queryLayer.Last() : decimal.MaxValue;
            var data = await _zjnBaseLocationAsrsRepository.AsSugarClient().Queryable<ZjnBaseLocationAsrsEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_LocationNo), a => a.LocationNo.Contains(input.F_LocationNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_DeviceNo), a => a.DeviceNo.Contains(input.F_DeviceNo))
                .WhereIF(queryRow != null, a => SqlFunc.Between(a.Row, startRow, endRow))
                .WhereIF(queryCell != null, a => SqlFunc.Between(a.Cell, startCell, endCell))
                .WhereIF(queryLayer != null, a => SqlFunc.Between(a.Layer, startLayer, endLayer))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_ByWarehouse), a => a.ByWarehouse.Contains(input.F_ByWarehouse))
                .Where(a=> a.IsDelete==0)
                .Select((a
)=> new ZjnBaseLocationAsrsListOutput
                {
                    F_Id = a.Id,
                    F_LocationNo = a.LocationNo,
                    F_DeviceNo = a.DeviceNo,
                    F_Row = a.Row,
                    F_Cell = a.Cell,
                    F_Layer = a.Layer,
                    F_Depth = a.Depth,
                    F_TrayNo = a.TrayNo,
                    F_LastStatus = a.LastStatus,
                    F_LocationStatus = a.LocationStatus,
                    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName),//a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = SqlFunc.IIF(a.EnabledMark == 0, "未启用", "启用"),
                    F_ByWarehouse = a.ByWarehouse,
                    F_type=a.type,
                    F_Legion=a.Legion,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseLocationAsrsListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建立库货位信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseLocationAsrsCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseLocationAsrsEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            entity.IsDelete = 0;
            var isOk = await _zjnBaseLocationAsrsRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取立库货位信息无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnBaseLocationAsrsListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_LocationNo" : input.sidx;
            List<object> queryRow = input.F_Row != null ? input.F_Row.Split(',').ToObject<List<object>>() : null;
            var startRow = input.F_Row != null && !string.IsNullOrEmpty(queryRow.First().ToString()) ? queryRow.First() : decimal.MinValue;
            var endRow = input.F_Row != null && !string.IsNullOrEmpty(queryRow.Last().ToString()) ? queryRow.Last() : decimal.MaxValue;
            List<object> queryCell = input.F_Cell != null ? input.F_Cell.Split(',').ToObject<List<object>>() : null;
            var startCell = input.F_Cell != null && !string.IsNullOrEmpty(queryCell.First().ToString()) ? queryCell.First() : decimal.MinValue;
            var endCell = input.F_Cell != null && !string.IsNullOrEmpty(queryCell.Last().ToString()) ? queryCell.Last() : decimal.MaxValue;
            List<object> queryLayer = input.F_Layer != null ? input.F_Layer.Split(',').ToObject<List<object>>() : null;
            var startLayer = input.F_Layer != null && !string.IsNullOrEmpty(queryLayer.First().ToString()) ? queryLayer.First() : decimal.MinValue;
            var endLayer = input.F_Layer != null && !string.IsNullOrEmpty(queryLayer.Last().ToString()) ? queryLayer.Last() : decimal.MaxValue;
            var data = await _zjnBaseLocationAsrsRepository.AsSugarClient().Queryable<ZjnBaseLocationAsrsEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_LocationNo), a => a.LocationNo.Contains(input.F_LocationNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_DeviceNo), a => a.DeviceNo.Contains(input.F_DeviceNo))
                .WhereIF(queryRow != null, a => SqlFunc.Between(a.Row, startRow, endRow))
                .WhereIF(queryCell != null, a => SqlFunc.Between(a.Cell, startCell, endCell))
                .WhereIF(queryLayer != null, a => SqlFunc.Between(a.Layer, startLayer, endLayer))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_ByWarehouse), a => a.ByWarehouse.Contains(input.F_ByWarehouse))
                .Where(a => a.IsDelete == 0)
                .Select((a
)=> new ZjnBaseLocationAsrsListOutput
                {
                    F_Id = a.Id,
                    F_LocationNo = a.LocationNo,
                    F_DeviceNo = a.DeviceNo,
                    F_Row = a.Row,
                    F_Cell = a.Cell,
                    F_Layer = a.Layer,
                    F_Depth = a.Depth,
                    F_TrayNo = a.TrayNo,
                    F_LastStatus = a.LastStatus,
                    F_LocationStatus = a.LocationStatus,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = SqlFunc.IIF(a.EnabledMark == 0, "未启用", "启用"),
                    F_ByWarehouse = a.ByWarehouse,
                    F_Legion=a.Legion,
                    F_type=a.type,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出立库货位信息
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnBaseLocationAsrsListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnBaseLocationAsrsListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnBaseLocationAsrsListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"货位编号\",\"field\":\"locationNo\"},{\"value\":\"仓库编号\",\"field\":\"byWarehouse\"},{\"value\":\"设备编号\",\"field\":\"deviceNo\"},{\"value\":\"行\",\"field\":\"row\"},{\"value\":\"列\",\"field\":\"cell\"},{\"value\":\"层\",\"field\":\"layer\"},{\"value\":\"深\",\"field\":\"depth\"},{\"value\":\"托盘编号\",\"field\":\"trayNo\"},{\"value\":\"上次货位状态\",\"field\":\"lastStatus\"},{\"value\":\"货位状态\",\"field\":\"locationStatus\"},{\"value\":\"有效标志\",\"field\":\"enabledMark\"},{\"value\":\"创建者\",\"field\":\"createUser\"},{\"value\":\"创建时间\",\"field\":\"createTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "立库货位信息.xls";
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
            ExcelExportHelper<ZjnBaseLocationAsrsListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除立库货位信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseLocationAsrsRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除立库货位信息

                    foreach (var item in entitys)
                    {
                        ZjnBaseLocationAsrsEntity entity = new ZjnBaseLocationAsrsEntity();
                        entity=item;
                        entity.IsDelete = 1;
                        await _zjnBaseLocationAsrsRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                    }
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
        /// 更新立库货位信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseLocationAsrsUpInput input)
        {
            var entity = input.Adapt<ZjnBaseLocationAsrsEntity>();
            var isOk = await _zjnBaseLocationAsrsRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除立库货位信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseLocationAsrsRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            entity.IsDelete = 1;
            var isOk = await _zjnBaseLocationAsrsRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
             //await _zjnBaseLocationAsrsRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 入库模板下载
        /// </summary>
        /// <returns></returns>
        [HttpGet("TheRawMaterial")]
        public dynamic TheRawMaterial(string fileName)
        {

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = fileName + ".xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetTheRawMaterial();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var UnitName2 = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "343195269594088709").Select(s => new ZjnPlaneGoodsListOutput { UnitName = s.FullName + ":" + s.EnCode }).ToList();
            var dataList = new List<ZjnBaseLocationAsrsEntity>() { new ZjnBaseLocationAsrsEntity() {
                LocationNo= "货位编号",
                type="货位类型",
                Legion="区域",
                ByWarehouse="所属仓库",
                DeviceNo="1",
                Row=0,
                Cell=0,
                Layer=0,
                Depth=0,
                LocationStatus="字段说明:"+GettName(UnitName2),
                LastStatus=0,
                TrayNo="1",
                Description ="案例不会导入",

            } };//初始化 一条空数据
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBaseLocationAsrsEntity>.Export(dataList, excelconfig, addPath);
            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetTheRawMaterial(List<string> fields = null)
        {
            var res = new Dictionary<string, string>();
            res.Add("LocationNo", "**货位编号**");
            res.Add("type", "**货位类型**");
            res.Add("Legion", "**区域**");
            res.Add("ByWarehouse", "**所属仓库**");
            res.Add("DeviceNo", "**设备编号**");           
            res.Add("Row", "**行**");
            res.Add("Cell", "**列**");
            res.Add("Layer", "**层**");
            res.Add("Depth", "**深**");
            res.Add("LocationStatus", "**货位状态**");
            res.Add("LastStatus", "**上次货位状态**");
            res.Add("TrayNo", "托盘号");
            res.Add("Description", "描述");
            if (fields == null || !fields.Any()) return res;

            var result = new Dictionary<string, string>();

            foreach (var item in res)
            {
                if (fields.Contains(item.Key))
                    result.Add(item.Key, item.Value);
            }

            return result;

        }

        public string GettName(List<ZjnPlaneGoodsListOutput> lists)
        {
            string Nmae = "";
            foreach (var item in lists)
            {
                Nmae += "" + item.UnitName;
            }
            return Nmae;
        }

        /// <summary>
        /// 货位导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("RawMaterialWarehousingData")]
        public async Task<dynamic> RawMaterialWarehousingData(List<ZjnBaseLocationAsrsEntity> list)
        {
            var res = await ImportTheRawMaterialDatas(list);
            var addlist = res.First() as List<ZjnBaseLocationAsrsEntity>;
            var errorlist = res.Last() as List<ZjnBaseLocationAsrsEntity>; //BaseLocationAsrsImportResultOutput
            return new BaseLocationAsrsImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }


        /// <summary>
        /// 导入数据函数
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>[成功列表,失败列表]</returns>
        private async Task<object[]> ImportTheRawMaterialDatas(List<ZjnBaseLocationAsrsEntity> list)
        {
            List<ZjnBaseLocationAsrsEntity> billsHistoryList = list;
            var userInfo = await _userManager.GetUserInfo();
            #region 排除错误数据
            if (billsHistoryList == null || billsHistoryList.Count() < 1)
                throw HSZException.Oh(ErrorCode.D5019);
            //必填字段验证 
            var errorList = billsHistoryList.Where(x => string.IsNullOrEmpty(x.LocationStatus) || string.IsNullOrEmpty(x.LocationNo) || string.IsNullOrEmpty(x.ByWarehouse)).ToList();
            billsHistoryList = billsHistoryList.Except(errorList).ToList();
            List<ZjnBaseLocationAsrsEntity> listBillsHistory = new List<ZjnBaseLocationAsrsEntity>();
            foreach (var item in billsHistoryList)
            {
                var inventoryEntity = (await _zjnBaseLocationAsrsRepository.GetFirstAsync(x => x.LocationNo == item.LocationNo)).Adapt<ZjnBaseLocationAsrsEntity>();
                if (inventoryEntity != null)
                {
                    errorList.Add(item);
                    continue;
                }
                ZjnBaseLocationAsrsEntity locationEntity = new ZjnBaseLocationAsrsEntity();
                locationEntity = item;
                locationEntity.Id = YitIdHelper.NextId().ToString(); ;
                locationEntity.IsDelete = 0;
                locationEntity.Legion = item.Legion;
                //locationEntity.TrayNo = item.TrayNo == null ? "0" : item.TrayNo;
                locationEntity.type = item.type;
                locationEntity.CreateUser = _userManager.UserId;
                locationEntity.CreateTime = DateTime.Now;
               // locationEntity.LastStatus = item.LastStatus == null ? 0 : item.LastStatus;
                locationEntity.EnabledMark = 1;
                listBillsHistory.Add(locationEntity);
            }

            #endregion
            //去掉错误的数据
            listBillsHistory = listBillsHistory.Except(errorList).ToList();


            if (listBillsHistory.Any())
            {
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增记录
                    var newEntity = await _zjnBaseLocationAsrsRepository.AsInsertable(listBillsHistory).ExecuteReturnEntityAsync();

                    _db.CommitTran();
                }
                catch (Exception e)
                {
                    string es = e.Message;
                    _db.RollbackTran();

                    errorList.AddRange(billsHistoryList);
                    //去掉错误的数据
                    listBillsHistory = listBillsHistory.Except(errorList).ToList();
                }
            }

            return new object[] { listBillsHistory, errorList };
        }




        /// <summary>
        /// 导入预览
        /// </summary>
        /// <returns></returns>
        [HttpGet("ImportPreview")]
        public dynamic ImportPreview(string fileName)
        {
            try
            {
                var FileEncode = GetTheRawMaterial();
                var filePath = FileVariable.TemporaryFilePath;
                var savePath = filePath + fileName;
                //得到数据
                var excelData = ExcelImportHelper.ToDataTable(savePath);
                foreach (var item in excelData.Columns)
                {
                    excelData.Columns[item.ToString()].ColumnName = FileEncode.Where(x => x.Value == item.ToString()).FirstOrDefault().Key;
                }
                //返回结果
                return new { dataRow = excelData };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw HSZException.Oh(ErrorCode.D1801);
            }
        }

    }
}


