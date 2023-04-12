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
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.wms.Entitys.Dto.ZjnRecordTrayGoods;
using HSZ.wms.Entitys.Dto.zjnWcsProcessConfig;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using HSZ.wms.Entitys.Dto.ZjnWmsTray;
using HSZ.wms.Entitys.Dto.ZjnWmsTrayLocationLog;
using HSZ.wms.Interfaces.ZjnWmsTray;
using HSZ.wms.ZjnWmsTask;
using HSZ.wms.ZjnWmsTrayLocationLog;
using HSZ.Wms.ZjnWmsWorkPathAffirm;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using static Org.BouncyCastle.Math.EC.ECCurve;
using ZJN.Calb.Entitys.Dto;
using HSZ.wms.Interfaces.ZjnServicePathConfig;
using System.ComponentModel;
using Microsoft.CodeAnalysis;
using HSZ.wms.ZjnWmsLocationAuto;
using HSZ.wms.Interfaces.ZjnWmsLocation;
using NPOI.SS.Formula.PTG;
using Aspose.Words.Drawing;

namespace HSZ.wms.ZjnWmsTray
{
    /// <summary>
    /// 托盘信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsTray", Order = 200)]
    [Route("api/wms/[controller]")]
    public class BaseTrayService : IZjnWmsTrayService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTrayEntity> _zjnBaseTrayRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        private readonly ISqlSugarRepository<ZjnWmsTrayLocationLogEntity> _zjnRecordTrayLocationLogRepository;       
        private readonly ISqlSugarRepository<ZjnWmsAisleEntity> _zjnWmsAisleEntity;
        private readonly ISqlSugarRepository<ZjnWmsEquipmentListEntity> _zjnWmsEquipmentListEntity;
        private readonly IZjnWcsProcessConfigService _zjnServicePathConfigService;
        private readonly ISqlSugarRepository<ZjnWmsEquipmentListEntity> _zjnWmsEquipmentListRepository;
        private readonly ISqlSugarRepository<ZjnWmsGoodsEntity> _zjnSugarRepository;
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        //按任务类型区分处理业务
        private readonly ZjnWmsWorkPathAffirm _WmsWorkPathAffirm;
        private readonly ZjnTaskService _taskService;
        private readonly IZjnWmsLocationAutoService _zjnWmsLocationAutoService;
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        /// <summary>
        /// 初始化一个<see cref="BaseTrayService"/>类型的新实例
        /// </summary>
        public BaseTrayService(ISqlSugarRepository<ZjnWmsTrayEntity> zjnBaseTrayRepository,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository,
        ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository,
            IUserManager userManager,
            ISqlSugarRepository<ZjnWmsEquipmentListEntity> zjnWmsEquipmentListRepository,
            ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository,
            ISqlSugarRepository<ZjnWmsTrayGoodsEntity> zjnWmsTrayGoodsRepository,
            ISqlSugarRepository<ZjnWmsTrayLocationLogEntity> zjnRecordTrayLocationLogRepository,
            ISqlSugarRepository<ZjnWmsAisleEntity> zjnWmsAisleEntity,
             ISqlSugarRepository<ZjnWmsEquipmentListEntity> zjnWmsEquipmentListEntity,
            ISqlSugarRepository<ZjnWmsGoodsEntity> zjnSugarRepository,
            ZjnWmsWorkPathAffirm WmsWorkPathAffirm,
            ZjnTaskService taskService,
            IZjnWcsProcessConfigService zjnServicePathConfigService,
        ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository,
        IZjnWmsLocationAutoService zjnWmsLocationAutoService)
        {
            _zjnTaskListDetailsRepository= zjnTaskListDetailsRepository;
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
            _zjnSugarRepository = zjnSugarRepository;
            _zjnWmsEquipmentListRepository = zjnWmsEquipmentListRepository;
            _WmsWorkPathAffirm = WmsWorkPathAffirm;
            _zjnServicePathConfigService = zjnServicePathConfigService;
            _zjnWmsAisleEntity = zjnWmsAisleEntity;
            _zjnWmsEquipmentListEntity = zjnWmsEquipmentListEntity;
            _taskService = taskService;
            _zjnRecordTrayLocationLogRepository = zjnRecordTrayLocationLogRepository;
            _zjnBaseTrayRepository = zjnBaseTrayRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
            _zjnWmsLocationAutoService = zjnWmsLocationAutoService;
        }

        /// <summary>
        /// 获取托盘信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseTrayRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsTrayInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取托盘信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsTrayListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnBaseTrayRepository.AsSugarClient().Queryable<ZjnWmsTrayEntity>()
                .Where(x => x.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayName), a => a.TrayName.Contains(input.F_TrayName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Type), a => a.Type.Equals(input.F_Type))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayStates), a => a.TrayStates.Equals(input.F_TrayStates))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Select((a
) => new ZjnWmsTrayListOutput
{
    F_Id = a.Id,
    F_TrayNo = a.TrayNo,
    F_TrayName = a.TrayName,
    F_Type = a.Type,
    F_TrayStates = a.TrayStates,
    F_TrayAttr=a.TrayAttr,
    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName),
    F_CreateTime = a.CreateTime,
    F_EnabledMark = a.EnabledMark,
}).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWmsTrayListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 获取托盘物料明细列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("GetGoodsDetailsList")]
        public async Task<dynamic> GetGoodsDetailsList([FromQuery] ZjnWmsTrayGoodsListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnBaseTrayRepository.AsSugarClient().Queryable<ZjnWmsTrayGoodsEntity>()                
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .Where(x => x.IsDeleted == 0)
                .Select((a
) => new ZjnWmsTrayGoodsListOutput
{
    F_Id = a.Id,
    F_GoodsCode = a.GoodsCode,
    F_GoodsId = a.GoodsId,
    F_Quantity = a.Quantity,
    F_TrayNo = a.TrayNo,
    F_Unit = a.Unit,
    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName),
    F_CreateTime = a.CreateTime,
    F_EnabledMark = a.EnabledMark,
}).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWmsTrayGoodsListOutput>.SqlSugarPageResult(data);
        }
        /// <summary>
        /// 获取托盘信息无分页列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnWmsTrayListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnBaseTrayRepository.AsSugarClient().Queryable<ZjnWmsTrayEntity>()
                .Where(x => x.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayName), a => a.TrayName.Contains(input.F_TrayName))
                .WhereIF(!string.IsNullOrEmpty(input.F_Type), a => a.Type.Equals(input.F_Type))
                 .WhereIF(!string.IsNullOrEmpty(input.F_TrayAttr), a => a.Type.Equals(input.F_TrayAttr))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Select((a
) => new ZjnWmsTrayListOutput
{
    F_Id = a.Id,
    F_TrayNo = a.TrayNo,
    F_TrayName = a.TrayName,
    F_Type = a.Type,
    F_TrayStates = a.TrayStates,
    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName),
    F_CreateTime = a.CreateTime,
    F_EnabledMark = a.EnabledMark,
}).OrderBy(sidx + " " + input.sort).ToListAsync();
            return data;
        }

        /// <summary>
        /// 新建托盘信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsTrayCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsTrayEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;

            var isOk = await _zjnBaseTrayRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新托盘信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsTrayUpInput input)
        {
            var entity = input.Adapt<ZjnWmsTrayEntity>();
            var isOk = await _zjnBaseTrayRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除托盘信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            //var entity = await _zjnBaseTrayRepository.GetFirstAsync(p => p.Id.Equals(id));
            //_ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //var isOk = await _zjnBaseTrayRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            //if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);

            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            var entity = await _zjnBaseTrayRepository.GetFirstAsync(p => p.Id.Equals(id));

            operationLogEntity.BeforeDate = entity.ToJson();//修改前的数据
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //取消数据状态
            entity.IsDelete = 1;
            //添加日志
            operationLogEntity.CreateUser = _userManager.UserId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "托盘信息管理删除";
            operationLogEntity.AfterDate = entity.ToJson();//修改后的数据            
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 2;
            operationLogEntity.WorkPath = 2;
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnBaseTrayRepository.AsUpdateable(entity).ExecuteCommandAsync();
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
        /// 确认出库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("ConfirmTheDelivery")]
        public async Task ConfirmTheDelivery([FromBody] ZjnWmsTrayLocationLogListOutput input) {
            try
            {
                if(string.IsNullOrWhiteSpace(input.F_EquipmentSerialNumber))
                {
                    throw HSZException.Oh("出库口不能空,请选择");
                }
                //查询货位信息
                var Location = (await _zjnWmsLocationRepository.GetFirstAsync(p => p.EnabledMark == 1 && p.LocationNo == input.F_LocationNo));
                if (Location.LocationStatus == "5" || Location.LocationStatus == "6" || Location.LocationStatus == "7" || Location.LocationStatus == "8")
                {
                    throw HSZException.Oh("此货位，已被预约！不能出库");
                }

                var output = (await _zjnWmsAisleEntity.GetFirstAsync(p => p.AisleNo == input.F_NumberOfRoadway));
                if (output == null)
                {
                    throw HSZException.Oh("巷道不存在，请查正确.");
                }
                string TheBinding = input.F_EquipmentSerialNumber; 
                string PathId = null;
                int workType = 0;//1出库，6空托出库
                var EquipmentList = await _zjnSugarRepository.GetFirstAsync(p => p.GoodsCode == input.F_GoodsCode && p.IsDelete == 0);//查询物料信息
                if (EquipmentList == null)
                {
                    workType = 6;
                }
                else
                    workType = 1;
                #region

                //var EquipmentList = await _zjnSugarRepository.GetFirstAsync(p => p.GoodsCode == input.F_GoodsCode && p.IsDelete == 0);//查询物料信息
                //if (output.StackerNo.Contains("B"))//负极
                //{

                //    if (EquipmentList == null)
                //    {
                //        //查询托盘类型
                //        var BaseTray = await _zjnBaseTrayRepository.GetFirstAsync(x => x.TrayNo == input.F_TrayNo);
                //        if (BaseTray.Type == 2)
                //        {
                //            TheBinding = "A01023";
                //        }
                //        else
                //        {
                //            TheBinding = "A01033";
                //        }
                //        workType = 6;
                //    }
                //    else
                //    {
                //        if (EquipmentList.GoodsType == "2")
                //        {
                //            TheBinding = "A11037";//箔材出库A11041
                //        }
                //        else
                //        {
                //            TheBinding = "A11025";//粉料出库
                //        }
                //        workType = 1;
                //    }
                //}
                //else
                //{
                //    //正极
                //    ////获取设备信息

                //    if (EquipmentList == null)
                //    {
                //        //查询托盘类型
                //        var BaseTray = await _zjnBaseTrayRepository.GetFirstAsync(x => x.TrayNo == input.F_TrayNo);
                //        if (BaseTray.Type == 2)
                //        {
                //            TheBinding = "B01079"; //箔材出库
                //        }
                //        else
                //        {
                //            TheBinding = "B01031";//粉料出库
                //        }
                //        workType = 6;
                //    }
                //    else
                //    {
                //        if (EquipmentList.GoodsType == "1")//负极材料
                //        {
                //            TheBinding = "A11045";//箔材出库
                //        }
                //        else
                //        {
                //            TheBinding = "A11053";//粉料出库
                //        }
                //        workType = 1;
                //    }

                //}
                #endregion


                //2022-11-8 update by yml
                //2022-11-14 update by yml 注释
                //TheBinding获取的是巷道，巷道绑定出库台设备
                //string deviceId = (await _zjnWmsEquipmentListRepository.GetListAsync(x => x.TheBinding == TheBinding && x.Type == "1")).FirstOrDefault()?.EquipmentSerialNumber;

                //获取业务唯一ID

                //PathId = await _WmsWorkPathAffirm.GetWorkPathId(input.F_TrayNo, TheBinding, input.F_GoodsCode, workType); 
                //2022-11-14 update by yml
                PathId = await _WmsWorkPathAffirm.GetWorkPathId(input.F_TrayNo, TheBinding, input.F_GoodsCode, workType);

                //var list= await _taskService.CreateByConfigId(PathId);
                //var TaskDetails= (await _taskService.GetFirstTaskDetails(list.taskList)).Adapt<ZjnWmsTaskDetailsEntity>();
                //TaskDetails.TaskDetailsMove = output.StackerNo;
                //TaskDetails.RowStart = Location.Row;
                //TaskDetails.CellStart = Location.Cell;
                //TaskDetails.LayerStart = Location.Layer;
                //await _zjnTaskListDetailsRepository.AsUpdateable(TaskDetails).ExecuteCommandAsync();

                //2022-11-8 update by yml
                ZjnWmsTaskCrInput taskInput=new ZjnWmsTaskCrInput{
                trayNo=input.F_TrayNo,//托盘
                positionFrom= Location.LocationNo,//起点
                positionTo= TheBinding,//终点 出库口
                operationDirection ="Out",//出库
                operationType=workType==1? "production" : "emptyContainer",//2022-11-14 update by yml
                materialCode = workType == 1 ? input.F_GoodsCode:"",
                quantity = workType == 1 ? input.F_Quantity:null,  
                };
                //传入添加任务必须参数
                var list = await _taskService.CreateByConfigId(PathId, taskInput);


                await _zjnWmsLocationAutoService.UpdateLocationStatus(Location.LocationNo, LocationStatus.Order);
                ////根据业务Id获取到子任务
                //ZjnWcsProcessConfigJsonOutput data = await _zjnServicePathConfigService.PathData(PathId);
                //ZjnWmsTaskCrInput TaskCrInput = data.zjnTaskInfoOutput.Adapt<ZjnWmsTaskCrInput>();

                //TaskCrInput.taskName = "出库" + input.F_TrayNo;
                //TaskCrInput.orderNo = data.zjnTaskInfoOutput.orderNo;
                //TaskCrInput.materialCode = input.F_GoodsCode;
                //TaskCrInput.quantity = input.F_Quantity;
                ////TaskCrInput.billNo = taskWarehouseRequest.attribute1;//待确认

                //TaskCrInput.priority = 1;
                //TaskCrInput.trayNo = input.F_TrayNo;
                //TaskCrInput.positionTo = TheBinding;                
                //TaskCrInput.operationDirection = "1";
                //TaskCrInput.taskFrom = "LES";
                //List<ZjnWmsTaskDetailsEntity> list = new List<ZjnWmsTaskDetailsEntity>();
                //var lsite= data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();
                //foreach (var item in lsite) {
                //    if (item.TaskDetailsName.Contains("库位"))
                //    {
                //        ZjnWmsTaskDetailsEntity zjnWmsss = new ZjnWmsTaskDetailsEntity();
                //        zjnWmsss = item;
                //        zjnWmsss.TaskDetailsMove= output.StackerNo;
                //        zjnWmsss.RowStart = Location.Row;
                //        zjnWmsss.CellStart= Location.Cell;
                //        zjnWmsss.LayerStart = Location.Cell;

                //    }
                //}


                //TaskCrInput.taskList = data.ZjnTaskListDetailsList.Adapt<List<ZjnWmsTaskDetailsEntity>>();
                //await _taskService.Create(TaskCrInput);

            }
            catch (Exception ex)
            {

                throw HSZException.Oh(ex.Message);
            }
           
        }

        /// <summary>
        /// 根据盘号查询货物
        /// </summary>
        /// <param name="TrayNo">托盘号</param>
        /// <returns></returns>
        [HttpGet("GetLocationLog")]
        public async Task<dynamic> GetLocationLog(string TrayNo)
        {
            var data = await _zjnBaseTrayRepository.AsSugarClient().Queryable<ZjnWmsTrayEntity>()
                    .LeftJoin<ZjnWmsMaterialInventoryEntity>((a, b) => a.TrayNo == b.ProductsContainer)
                    .LeftJoin<ZjnWmsGoodsEntity>((a, b, c) => b.ProductsCode == c.GoodsCode)
                    .InnerJoin<ZjnWmsLocationEntity>((a, b, c, e) => b.ProductsLocation == e.LocationNo)
                    .Where((a, b, c, e) => a.TrayNo == TrayNo && a.IsDelete==0)
                    .Select((a, b, c,e) => new ZjnWmsTrayLocationLogListOutput
                    {
                        F_GoodsCodeNmae = c.GoodsName,
                        F_GoodsCode = b.ProductsCode,
                        F_Quantity = b.ProductsQuantity,
                        F_Unit = c.Unit.ToString(),
                        F_TrayNo = a.TrayNo,
                        F_TrayName=a.TrayName,
                        F_LocationNo = b.ProductsLocation,
                        F_GoodsType = c.GoodsType,
                        F_LocationName = e.Description,
                        F_NumberOfRoadway = e.AisleNo,

                    }).ToPagedListAsync(1, 2);
            ZjnWmsTrayLocationLogListOutput zjnWmsTray = new ZjnWmsTrayLocationLogListOutput();
            foreach (var item in data.list)
            {
                zjnWmsTray = item;
            }
            return zjnWmsTray;
        }

       

        /// <summary>
        /// 获取字段名称+
        /// </summary>
        /// <param name="filed"></param>
        /// <returns></returns>
        private string GetFiledName(string filed)
        {
            switch (filed)
            {
                case "F_TrayNo":
                    return "托盘编号";
                case "F_TrayName":
                    return "托盘名称";
                case "F_Type":
                    return "托盘类型";
                case "F_CreateUser":
                    return "创建人";
                case "F_CreateTime":
                    return "创建时间";
                case "F_EnabledMark":
                    return "禁用";
                case "F_IsDelete":
                    return "是否删除";
                default:
                    return "";
            }
        }
        #region 导出
        /// <summary>
        /// 导出
        /// </summary>
        [HttpGet("ExportExcelData")]
        public async Task<dynamic> ExportExcelData([FromQuery] ZjnWmsTrayListQueryInput input)
        {
            var dataList = new List<ZjnWmsTrayListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));

                dataList = data.Solidify<PageResult<ZjnWmsTrayListOutput>>().list;
            }
            else
            {
                dataList = await this.GetNoPagingList(input);
            }
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "托盘信息.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = input.selectKey.Split(",");
            foreach (var item in filedList)
            {
                var column = StringHelper.FunctionStr(item);
                var excelColumn = GetFiledName(item);
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnWmsTrayListOutput>.Export(dataList, excelconfig, addPath);
            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }
        #endregion

        #region 导入
        private Dictionary<string, string> GetUserInfoFieldToTitle(List<string> fields = null)
        {

            var res = new Dictionary<string, string>();
            res.Add("F_TrayNo", "**托盘编号**");
            res.Add("F_TrayName", "**托盘名称**");
            res.Add("F_TypeName", "**托盘类型**");
            res.Add("EnabledMark", "禁用原因");

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
        /// 模板下载
        /// </summary>
        /// <returns></returns>
        [HttpGet("CreateModel")]
        public dynamic CreateModel()
        {
            //var dataList = new List<ZjnBaseTrayCrInput>() { new ZjnBaseTrayCrInput() { } };//初始化 一条空数据

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "托盘信息导入模板.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetUserInfoFieldToTitle();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }

            //插入案例
            var typeName = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "332411089230759173").Select(s => s.FullName + ":" + s.EnCode).ToList();


            var dataList = new List<ZjnWmsTrayListOutput>() { new ZjnWmsTrayListOutput() {

               F_TrayName="托盘名称",
               F_TrayNo="5454",
               F_TypeName="字段说明："+string.Join(',',typeName.ToArray()),
               EnabledMark="案例不会导入"
            } };//初始化 一条空数据

            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnWmsTrayListOutput>.Export(dataList, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
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
                var FileEncode = GetUserInfoFieldToTitle();

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

        /// <summary>
        /// 入库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("RawMaterialWarehousingData")]
        public async Task<dynamic> RawMaterialWarehousingData(List<ZjnWmsTrayListOutput> list)
        {
            var res = await ImportTheRawMaterialDatas(list);
            var addlist = res.First() as List<ZjnWmsTrayListOutput>;
            var errorlist = res.Last() as List<ZjnWmsTrayListOutput>;
            return new ZjnWmsTrayImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }
        /// <summary>
        /// 导入数据函数
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>[成功列表,失败列表]</returns>
        private async Task<object[]> ImportTheRawMaterialDatas(List<ZjnWmsTrayListOutput> list)
        {
            List<ZjnWmsTrayListOutput> PlaneGoodsList = list;

            #region 排除错误数据
            if (PlaneGoodsList == null || PlaneGoodsList.Count() < 1)
                throw HSZException.Oh(ErrorCode.D5019);

            //必填字段验证 
            var errorList = PlaneGoodsList.Where(x => string.IsNullOrEmpty(x.F_TypeName) || string.IsNullOrEmpty(x.F_TrayName.ToString()) || string.IsNullOrEmpty(x.F_TrayNo)).ToList();
            var _zjnBillsHistoryRepositoryList = await _zjnBaseTrayRepository.GetListAsync();//数据集
            var repeat = _zjnBillsHistoryRepositoryList.Where(u => PlaneGoodsList.Select(x => x.F_TrayNo).Contains(u.TrayNo) && u.IsDelete == 0).ToList();//已存在的编号           
            if (repeat.Any())
                errorList.AddRange(PlaneGoodsList.Where(u => repeat.Select(x => x.TrayNo).Contains(u.F_TrayNo) && !errorList.Select(x => x.F_TrayNo).Contains(u.F_TrayNo)));//已存在的编号 列入 错误列表

            PlaneGoodsList = PlaneGoodsList.Except(errorList).ToList();
            #endregion

            var PlaneGoods = new List<ZjnWmsTrayEntity>();
            var userInfo = await _userManager.GetUserInfo();

            foreach (var item in PlaneGoodsList)
            {
                var uentity = new ZjnWmsTrayEntity();
                uentity.Id = YitIdHelper.NextId().ToString();
                uentity.CreateTime = DateTime.Now;
                uentity.CreateUser = userInfo.userId;
                uentity.TrayNo = item.F_TrayNo;
                uentity.Type = Convert.ToInt32(item.F_TypeName);
                uentity.TrayName = item.F_TrayName;
                uentity.EnabledMark = Convert.ToInt32(string.IsNullOrEmpty(item.EnabledMark) ? "1" : "0");
                uentity.IsDelete = 0;
                PlaneGoods.Add(uentity);
            }

            if (PlaneGoods.Any())
            {
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增记录
                    var newEntity = await _zjnBaseTrayRepository.AsInsertable(PlaneGoods).ExecuteReturnEntityAsync();

                    _db.CommitTran();
                }
                catch (Exception e)
                {
                    string es = e.Message;
                    _db.RollbackTran();
                    errorList.AddRange(PlaneGoodsList);
                }
            }
            return new object[] { PlaneGoodsList, errorList };
        }
        /// <summary>
        /// 导出错误报告
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ExportExceptionData")]
        public async Task<dynamic> ExportExceptionData(List<ZjnWmsTrayListOutput> list)
        {
            var res = await ImportTheRawMaterialDatas(list);
            var errorlist = res.Last() as List<ZjnWmsTrayListOutput>;//错误数据

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "托盘信息导入错误报告.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetUserInfoFieldToTitle();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnWmsTrayListOutput>.Export(errorlist, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }
        #endregion
    }
}


