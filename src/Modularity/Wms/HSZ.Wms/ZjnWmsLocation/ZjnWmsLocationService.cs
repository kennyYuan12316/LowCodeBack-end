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
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.wms.Entitys.Dto.ZjnPlaneGoods;
using HSZ.wms.Entitys.Dto.ZjnWmsLocation;
using HSZ.wms.Interfaces.ZjnWmsLocation;
using HSZ.Wms.Entitys.Dto.zjnPlaneGoods;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsLocation
{
    /// <summary>
    /// 货位信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsLocation", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsLocationService : IZjnWmsLocationService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        /// <summary>
        /// 初始化一个<see cref="ZjnWmsLocationService"/>类型的新实例
        /// </summary>
        public ZjnWmsLocationService(ISqlSugarRepository<ZjnWmsLocationEntity> zjnBaseLocationRepository,
            IUserManager userManager,
            ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository,
            ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository)
        {
            _dictionaryDataRepository = dictionaryDataRepository;
            _zjnWmsLocationRepository = zjnBaseLocationRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
        }

        /// <summary>
        /// 获取货位信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWmsLocationRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsLocationInfoOutput>();
            return output;
        }
        /// <summary>
        /// 判断仓库编号是否存在
        /// </summary>
        /// <param name="LocationNo"></param>
        /// <returns></returns>
        [HttpGet("ExistLocationNo")]
        public async Task<bool> ExistLocationNo(string LocationNo)
        {
            var output = await _zjnWmsLocationRepository.IsAnyAsync(p => p.LocationNo == LocationNo && p.IsDelete == 0);
            return output;
        }

        /// <summary>
        /// 获取货位信息 -- APP使用
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns> 
        [HttpGet("GetInfoToApp")]
        public async Task<dynamic> GetInfoToApp(string id, string houseId)
        {

            //主表自带货位id
            if (string.IsNullOrEmpty(houseId))
            {
                return (await _zjnWmsLocationRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsLocationInfoOutput>();
            }
            //用户自己输入货位code
            var output = (await _zjnWmsLocationRepository.GetFirstAsync(p => p.LocationNo == id && p.ByWarehouse == houseId)).Adapt<ZjnWmsLocationInfoOutput>();
            if (output == null)
            {
                return (await _zjnWmsLocationRepository.GetFirstAsync(p => p.LocationNo == id)).Adapt<ZjnWmsLocationInfoOutput>();
            }

            return output;
        }



        /// <summary>
		/// 获取货位信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsLocationListQueryInput input)
        {
            var sidx = input.sidx == null ? "a.F_Id" : input.sidx;
            var aisleNo = input.F_AisleNo != null ? input.F_AisleNo.Split(',').ToList().Last() : null;
            List<object> queryRow = input.F_Row != null ? input.F_Row.Split(',').ToObject<List<object>>() : null;
            var startRow = input.F_Row != null && !string.IsNullOrEmpty(queryRow.First().ToString()) ? queryRow.First() : decimal.MinValue;
            var endRow = input.F_Row != null && !string.IsNullOrEmpty(queryRow.Last().ToString()) ? queryRow.Last() : decimal.MaxValue;
            List<object> queryCell = input.F_Cell != null ? input.F_Cell.Split(',').ToObject<List<object>>() : null;
            var startCell = input.F_Cell != null && !string.IsNullOrEmpty(queryCell.First().ToString()) ? queryCell.First() : decimal.MinValue;
            var endCell = input.F_Cell != null && !string.IsNullOrEmpty(queryCell.Last().ToString()) ? queryCell.Last() : decimal.MaxValue;
            List<object> queryLayer = input.F_Layer != null ? input.F_Layer.Split(',').ToObject<List<object>>() : null;
            var startLayer = input.F_Layer != null && !string.IsNullOrEmpty(queryLayer.First().ToString()) ? queryLayer.First() : decimal.MinValue;
            var endLayer = input.F_Layer != null && !string.IsNullOrEmpty(queryLayer.Last().ToString()) ? queryLayer.Last() : decimal.MaxValue;
            var data = await _zjnWmsLocationRepository.AsSugarClient().Queryable<ZjnWmsLocationEntity>()
                .LeftJoin<ZjnWmsWarehouseEntity>((a, i) => a.ByWarehouse == i.WarehouseNo && i.IsDelete == 0)
                .LeftJoin<UserEntity>((a, i, u) => a.CreateUser == u.Id)
                .Where(a => a.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_LocationNo), a => a.LocationNo.Contains(input.F_LocationNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_AisleNo), a => a.AisleNo.Contains(aisleNo))
                .WhereIF(queryRow != null, a => SqlFunc.Between(a.Row, startRow, endRow))
                .WhereIF(queryCell != null, a => SqlFunc.Between(a.Cell, startCell, endCell))
                .WhereIF(queryLayer != null, a => SqlFunc.Between(a.Layer, startLayer, endLayer))
                .WhereIF(!string.IsNullOrEmpty(input.F_LocationStatus), a => a.LocationStatus.Equals(input.F_LocationStatus))
                .OrderBy(a => a.LocationNo)
                .OrderBy((a, i) => i.WarehouseName)
                .Select((a, i, u) => new ZjnWmsLocationListOutput
                {
                    F_Id = a.Id,
                    F_LocationNo = a.LocationNo,
                    F_AisleNo = a.AisleNo,
                    F_Row = a.Row,
                    F_Cell = a.Cell,
                    F_Layer = a.Layer,
                    F_Depth = a.Depth,
                    F_TrayNo = a.TrayNo,
                    F_LastStatus = a.LastStatus,
                    F_LocationStatus = a.LocationStatus,
                    F_Description = a.Description,
                    F_CreateUser = u.RealName,
                    F_CreateTime = a.CreateTime,
                    ByWarehouse = i.WarehouseName,
                    F_EnabledMark = SqlFunc.IIF(a.EnabledMark == 0, "禁用", "启用"),
                }).OrderBy(a => a.F_Id, input.sort == "desc" ? OrderByType.Desc : OrderByType.Asc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWmsLocationListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建货位信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsLocationCrInput input)
        {
            if (await this.ExistLocationNo(input.locationNo)) throw HSZException.Oh(ErrorCode.COM1004);

            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsLocationEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            //entity.AisleNo = input.AisleNos;
            var isOk = await _zjnWmsLocationRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新货位信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsLocationUpInput input)
        {
            var entity = input.Adapt<ZjnWmsLocationEntity>();
            var isOk = await _zjnWmsLocationRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除货位信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWmsLocationRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //var isOk = await _zjnWmsLocationRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            //if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);

            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            //var entity = await _zjnWmsLocationRepository.GetFirstAsync(p => p.Id.Equals(id));

            operationLogEntity.BeforeDate = entity.ToJson();//修改前的数据
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //取消数据状态
            entity.IsDelete = 1;
            //添加日志
            operationLogEntity.CreateUser = _userManager.UserId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "货位管理信息删除";
            operationLogEntity.AfterDate = entity.ToJson();//修改后的数据            
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 2;
            operationLogEntity.WorkPath = 2;
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnWmsLocationRepository.AsUpdateable(entity).ExecuteCommandAsync();
                //新增日子记录
                var isOk1 = await _zjnPlaneOperationLogRepository.AsInsertable(operationLogEntity).ExecuteReturnEntityAsync();

                _db.CommitTran();
            }
            catch (Exception e)
            {
                string es = e.Message;
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);

            }
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
            var dataList = new List<ZjnWmsLocationEntity>() { new ZjnWmsLocationEntity() {
                LocationNo= "货位编号",
                LocationStatus="字段说明:"+GettName(UnitName2),
                ByWarehouse="所属仓库",
                AisleNo="巷道编号",
                Row=1,
                Cell=0,
                Layer=0,
                Depth=0,
                Description ="案例不会导入",

            } };//初始化 一条空数据
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnWmsLocationEntity>.Export(dataList, excelconfig, addPath);
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
            res.Add("locationStatus", "**货位状态**");
            res.Add("ByWarehouse", "**所属仓库**");
            res.Add("AisleNo", "**巷道编号**");
            res.Add("Row", "**行**");
            res.Add("Cell", "**列**");
            res.Add("Layer", "**层**");
            res.Add("Depth", "**深**");
            res.Add("TrayNo", "**托盘号**");
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
        public async Task<dynamic> RawMaterialWarehousingData(List<ZjnWmsLocationEntity> list)
        {
            var res = await ImportTheRawMaterialDatas(list);
            var addlist = res.First() as List<ZjnWmsLocationEntity>;
            var errorlist = res.Last() as List<ZjnWmsLocationEntity>;
            return new BaseLocationImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }


        /// <summary>
        /// 导入数据函数
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>[成功列表,失败列表]</returns>
        private async Task<object[]> ImportTheRawMaterialDatas(List<ZjnWmsLocationEntity> list)
        {
            List<ZjnWmsLocationEntity> billsHistoryList = list;
            var userInfo = await _userManager.GetUserInfo();
            #region 排除错误数据
            if (billsHistoryList == null || billsHistoryList.Count() < 1)
                throw HSZException.Oh(ErrorCode.D5019);
            //必填字段验证 
            var errorList = billsHistoryList.Where(x => string.IsNullOrEmpty(x.LocationStatus) || string.IsNullOrEmpty(x.LocationNo) || string.IsNullOrEmpty(x.ByWarehouse)).ToList();
            billsHistoryList = billsHistoryList.Except(errorList).ToList();
            List<ZjnWmsLocationEntity> listBillsHistory = new List<ZjnWmsLocationEntity>();
            foreach (var item in billsHistoryList)
            {
                var inventoryEntity = (await _zjnWmsLocationRepository.GetFirstAsync(x => x.LocationNo == item.LocationNo)).Adapt<ZjnWmsLocationEntity>();
                if (inventoryEntity != null)
                {
                    errorList.Add(item);
                    continue;
                }
                ZjnWmsLocationEntity locationEntity = new ZjnWmsLocationEntity();
                locationEntity = item;
                locationEntity.Id = YitIdHelper.NextId().ToString(); ;
                locationEntity.IsDelete = 0;
                locationEntity.TrayNo = item.TrayNo == null ? "0" : item.TrayNo;
                locationEntity.Depth = item.Depth == null ? 0 : item.Depth;
                locationEntity.CreateUser = _userManager.UserId;
                locationEntity.CreateTime = DateTime.Now;
                locationEntity.LastStatus = item.LastStatus == null ? 0 : item.LastStatus;
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
                    var newEntity = await _zjnWmsLocationRepository.AsInsertable(listBillsHistory).ExecuteReturnEntityAsync();

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


