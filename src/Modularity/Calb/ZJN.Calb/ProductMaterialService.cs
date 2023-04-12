using HSZ.Common.Core.Manager;
using HSZ.Common.DI;
using HSZ.Common.Extension;
using HSZ.Entitys.wms;
using HSZ.JsonSerialization;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using Mapster;
using Serilog;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Calb.Client;
using ZJN.Calb.Client.DTO;
using ZJN.Calb.Entitys.Dto;

namespace ZJN.Calb
{
    [WareDI(WareType.Production)]
    public class ProductMaterialService : ILesService
    {
        private readonly ISqlSugarRepository<ZjnWmsTrayEntity> _zjnBaseTrayRepository;
        private readonly ISqlSugarRepository _Repository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly LESServerClient _LESServer;
        private readonly IProductionCreateTaskByLesProcess _ProductionCreateTaskByLesProcess;

        private string ext = "入参请求格式问题";
        /// <summary>
        /// 初始化一个<see cref="ProductMaterialService"/>类型的新实例
        /// </summary>
        public ProductMaterialService(ISqlSugarRepository repository, LESServerClient lESServerClient, ISqlSugarRepository<ZjnWmsTrayEntity> zjnBaseTrayRepository, IProductionCreateTaskByLesProcess IProductionCreateTaskByLesProcess, IUserManager userManager)
        {
            _Repository = repository;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _LESServer = lESServerClient;
            _ProductionCreateTaskByLesProcess = IProductionCreateTaskByLesProcess;
            _zjnBaseTrayRepository = zjnBaseTrayRepository;
            _userManager = userManager;
        }
        /// <summary>
        /// 物料主数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string MainMaterialData_Line(string json)
        {
            GoodsDetailsResponse response = new GoodsDetailsResponse();
            var request = json?.Deserialize<List<GoodsDetailsRequest>>();
            if (request == null)
            {
                InstructMsg("", ext, "MainMaterialData_Line");
                throw new Exception(ext);
            }

            var entity = request.Adapt<List<ZjnBaseLesGoodsEntity>>();

            ZjnPlaneGoodsEntity zjnPlaneGoodsEntity = new ZjnPlaneGoodsEntity();

            string _cod = string.Empty;
            string _unit = string.Empty;
            foreach (var itemEntity in entity)
            {
                itemEntity.Id = YitIdHelper.NextId().ToString();
                itemEntity.CreateTime = DateTime.Now;
                _cod = itemEntity.Code;
                _unit = itemEntity.DefaultUnit;

                zjnPlaneGoodsEntity.Id = YitIdHelper.NextId().ToString();
                zjnPlaneGoodsEntity.GoodsCode = itemEntity.Code;
                zjnPlaneGoodsEntity.GoodsName = itemEntity.XName;
                zjnPlaneGoodsEntity.GoodsType = itemEntity.XType;
                zjnPlaneGoodsEntity.GoodsState = 1;
                zjnPlaneGoodsEntity.ShelfLife = itemEntity.ValidDays.ToString();
                zjnPlaneGoodsEntity.StayHours = itemEntity.StayHours.ToString();
                zjnPlaneGoodsEntity.BatchManageFlag = itemEntity.BatchManageFlag;
                zjnPlaneGoodsEntity.Specifications = itemEntity.Specification;
                zjnPlaneGoodsEntity.CreateTime = DateTime.Now;
                zjnPlaneGoodsEntity.CreateUser = "Admin";//_userManager.UserId;
                zjnPlaneGoodsEntity.IsDelete = 0;
            }
            _unit = _db.Ado.GetString($"select F_EnCode from ZJN_BASE_DICTIONARY_DATA where F_DictionaryTypeId = '326384591566800133' and F_FullName = '{_unit}'");
            int gunit;
            int.TryParse(_unit, out gunit);
            zjnPlaneGoodsEntity.Unit = gunit;

            _cod = _db.Ado.GetString($"select top 1 F_Code from zjn_base_les_goods where F_Code = '{_cod}'");
            if (!_cod.Any())
            {
                _db.Insertable(entity).ExecuteCommand();
                //_db.Insertable(zjnPlaneGoodsEntity).ExecuteCommand();
            }
            else
            {
                //response.code = "1";
                //response.message = $"物料重复！{_cod}";
                //_db.Ado.ExecuteCommand($"update zjn_wms_tray set F_CleanCycle = '{request.cleanCycle}',F_CreateTime = GETDATE() where F_Type = '{request.containerTypeCode}'");
            }
            var result = ResultPayload.GetResult(response);

            InstructMsg(result.payload, "物料主数据", "MainMaterialData_Line");

            return result.payload;
        }

        /// <summary>
        /// 配送任务指令-AGV
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string CONTAINER_PICK_PUT(string json)
        {
            Log.Information($"配送任务指令-AGV，请求入参【{json}】");

            var request = json?.Deserialize<AgvTaskRequest>();
            if (request == null)
            {
                InstructMsg("", ext, "CONTAINER_PICK_PUT");
                throw new Exception(ext);
            }


            AgvTaskResponse response = new AgvTaskResponse();
            var result = ResultPayload.GetResult(response);

            Log.Information($"配送任务指令-AGV，请求出参【{result.payload}】");
            InstructMsg(result.payload, "配送任务指令-AGV", "CONTAINER_PICK_PUT");
            return result.payload;
        }
        /// <summary>
        /// 容器清洗周期同步
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string ContainerCleanCycleSync(string json)
        {
            Log.Information($"容器清洗周期同步，请求入参【{json}】");
            ContainerWashCycleResponse response = new ContainerWashCycleResponse();
            var request = json?.Deserialize<ContainerWashCycleRequest>();
            if (request == null)
            {
                InstructMsg("", ext, "ContainerCleanCycleSync");
                throw new Exception(ext);
            }
            string _cod = string.Empty;
            _cod = _db.Ado.GetString($"select top 1 F_EnCode from ZJN_BASE_DICTIONARY_DATA where F_EnCode = '{request.containerTypeCode}'");
            if (string.IsNullOrEmpty(_cod))
            {
                throw new Exception("容器类型不存在！");
            }

            _db.Ado.ExecuteCommand($"update zjn_wms_tray set F_CleanCycle = '{request.cleanCycle}',F_CreateTime = GETDATE() where F_Type = '{request.containerTypeCode}'");

            var result = ResultPayload.GetResult(response);

            Log.Information($"容器清洗周期同步，请求出参【{result.payload}】");
            InstructMsg(result.payload, "容器清洗周期同步", "ContainerCleanCycleSync");
            return result.payload;
        }
        /// <summary>
        /// 质量状态变更接口
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string CHANGE_STATUS(string json)
        {
            Log.Information($"质量状态变更接口，请求入参【{json}】");

            var request = json?.Deserialize<GoodsStatusUpRequest>();
            if (request == null)
            {
                InstructMsg("", ext, "CHANGE_STATUS");
                throw new Exception(ext);
            }
            int st = 0;
            //状态：0解冻(UNFROZEN) 1冻结(FROZEN) 2合格(OK) 3不合格(NG) 4待检验(PENDING) 5有效(ENABLE) 6失效(UNENABLE) 7盘点冻结(STOCKTAKE)
            switch (request.tstatus.ToUpper())
            {
                case "OK":
                    st = 2;
                    break;
                case "NG":
                    st = 3;
                    break;
                case "UNFROZEN":
                    st = 0;
                    break;
                case "FROZEN":
                    st = 1;
                    break;
                case "PENDING":
                    st = 4;
                    break;
                case "ENABLE":
                    st = 5;
                    break;
                case "UNENABLE":
                    st = 6;
                    break;
                case "STOCKTAKE":
                    st = 7;
                    break;
                default:
                    st = 0;
                    break;
            }
            string _productionCode = string.Empty;
            if (request.lineList.IsNotEmptyOrNull())
            {
                for (int i = 0; i < request.lineList.Count; i++)
                {
                    string ssql = $"update zjn_wms_tray_goods set F_State = {st} where F_GoodsId = '{request.lineList[i].productionCode}'";
                    int exe = _db.Ado.ExecuteCommand(ssql);
                    Log.Information("质量状态更新 " + request.lineList[i].productionCode + ":" + exe);
                    if (exe <= 0)
                    {
                        _productionCode = _productionCode + " " + request.lineList[i].productionCode;
                    }
                }

                if (_productionCode.IsNotEmptyOrNull())
                {
                    throw new Exception("以下productionCode不在库存：" + _productionCode);
                }
            }
            else
            {
                throw new Exception("入参productionCode为空");
            }
            //_db.Ado.ExecuteCommand($"update zjn_wms_tray set F_TrayStates = {st},F_CreateTime = GETDATE() where F_TrayNo = '{request.container}'");
            //_db.Ado.ExecuteCommand($"update zjn_wms_materialInventory set F_ProductsState = '{st}',F_LastModifyTime = GETDATE() where F_ProductsCode = '{request.lineList[0].materialCode}'");
            //_db.Ado.ExecuteCommand($"update zjn_wms_tray_goods set F_State = {st} where F_GoodsCode = '{request.lineList[0].materialCode}'");


            GoodsStatusUpResponse response = new GoodsStatusUpResponse();
            var result = ResultPayload.GetResult(response);

            Log.Information($"质量状态变更接口，请求出参【{result.payload}】");
            InstructMsg(result.payload, "质量状态变更接口", "CHANGE_STATUS");
            return result.payload;
        }

        /// <summary>
        /// 立库库存报表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string WmsGoodsReport(string json)
        {
            Log.Information($"立库库存报表，请求入参【{json}】");

            var request = json?.Deserialize<WmsGoodsReportRequest>();
            if (request == null)
            {
                InstructMsg("", ext, "WmsGoodsReport");
                throw new Exception(ext);
            }

            WmsGoodsReportResponse response = new WmsGoodsReportResponse();
            var result = ResultPayload.GetResult(response);

            Log.Information($"立库库存报表，请求出参【{result.payload}】");
            InstructMsg(result.payload, "立库库存报表", "WmsGoodsReport");
            return result.payload;
        }
        /// <summary>
        /// 出入库指令取消接口
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string CONTAINER_INTO_STOP(string json)
        {
            Log.Information($"出入库指令取消接口，请求入参【{json}】");

            var request = json?.Deserialize<TaskCancelRequest>();
            if (request == null)
            {
                //InstructMsg("", ext, "CONTAINER_INTO_STOP");
                throw new Exception(ext);
            }
            string _cod = string.Empty;
            _cod = _db.Ado.GetString($"select top 1 F_TaskState from zjn_wms_task where F_TaskNo = '{request.instuctionNum}'");
            Log.Information($"任务号：【{_cod}】");
            if (string.IsNullOrEmpty(_cod))
            {
                Log.Information($"任务不存在1：");
                throw new Exception("任务不存在！");
            }
            Log.Information($"任务不存在2：");
            switch (_cod)
            {
                case "4":
                case "1":
                    _db.Ado.ExecuteCommand($"update zjn_wms_task set F_EnabledMark = 4 where F_TaskNo = '{request.orderNo}'");
                    break;
                case "2":
                    throw new Exception("任务执行中，无法取消！");
                case "3":
                    throw new Exception("任务已完成，取消失败！");
            }

            TaskCancelResponse response = new TaskCancelResponse();
            var result = ResultPayload.GetResult(response);

            Log.Information($"出入库指令取消接口，请求出参【{result.payload}】");
            InstructMsg(result.payload, "出入库指令取消接口", "CONTAINER_INTO_STOP");
            return result.payload;
        }
        /// <summary>
        /// 空托呼叫
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string ARTIFICIAL_CALLEMPTYTRAY(string json)
        {
            Log.Information($"空托呼叫，请求入参【{json}】");

            var request = json?.Deserialize<EmptyTrustCellRequest>();
            if (request == null)
            {
                InstructMsg("", ext, "ARTIFICIAL_CALLEMPTYTRAY");
                throw new Exception(ext);
            }

            InoutConFirmRequest dto = new InoutConFirmRequest();
            dto.requestId = YitIdHelper.NextId().ToString();
            dto.operationType = "emptyContainer";
            dto.toChannelId = "1642053037569";
            dto.clientCode = "ZJN";
            dto.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            dto.type = "arrive";
            string _cod = string.Empty;
            //trayCode = _db.Ado.GetString($"select top 1 F_TrayNo from zjn_wms_tray where F_Type = '{request.materialCode}'");
            //if (string.IsNullOrEmpty(trayCode))
            //{
            //    throw new Exception("容器类型不存在！");
            //}
            _cod = _db.Ado.GetString($"select top 1 F_Code from zjn_base_les_goods where F_Code = '{request.materialCode}'");
            if (!_cod.Any())
            {
                throw new Exception("物料不存在！");
            }
            _cod = _db.Ado.GetString($"select top 1 f_type from zjn_wms_goods_traytype where f_code = '{request.materialCode}'");
            if (!_cod.Any())
            {
                throw new Exception(request.materialCode + "物料没有对应的容器类型！");
            }
            else
            {
                _cod = _db.Ado.GetString($"select top 1 F_TrayNo from zjn_wms_tray where F_Type = '{_cod}' and F_TrayStates = 0");
            }

            dto.lines = new List<InoutConFirmLine>();
            dto.lines.Add(new InoutConFirmLine()
            {
                fromLocator = "h001",
                toLocator = request.locationCode,//目标库口
                container = _cod,//托盘条码
                materialBarcode = ""
            });
            var takeResult = _LESServer.InoutConFirm(dto);
            takeResult.Wait();
            var bb = takeResult.Result;
            Log.Information($"出入库确认，请求入参【{dto.Serialize()}】");



            EmptyTrustCellResponse response = new EmptyTrustCellResponse();
            var result = ResultPayload.GetResult(response);

            Log.Information($"空托呼叫，请求出参【{result.payload}】");
            InstructMsg(result.payload, "空托呼叫", "ARTIFICIAL_CALLEMPTYTRAY");
            return result.payload;
        }
        /// <summary>
        /// 取料放料接口-AGV
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string AgvGoods(string json)
        {
            Log.Information($"取放料接口，请求入参【{json}】");
            var request = json?.Deserialize<AgvGoodsRequest>();
            if (request == null)
            {
                InstructMsg("", ext, "AgvGoods");
                throw new Exception(ext);
            }

            AgvGoodsResponse response = new AgvGoodsResponse();
            var result = ResultPayload.GetResult(response);

            Log.Information($"取放料接口，请求出参【{result.payload}】");
            InstructMsg(result.payload, "取放料接口", "AgvGoods");
            return result.payload;
        }
        /// <summary>
        /// 一体化取放料接口（立库）
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string AgvGrabGoods(string json)
        {
            Log.Information($"一体化取放料接口（立库）请求，请求入参【{json}】");
            var request = json?.Deserialize<AgvGrabGoodsRequest>();
            if (request == null)
            {
                InstructMsg("", ext, "AgvGrabGoods");
                throw new Exception(ext);
            }

            AgvGrabGoodsResponse response = new AgvGrabGoodsResponse();
            var result = ResultPayload.GetResult(response);

            Log.Information($"一体化取放料接口（立库）请求，请求出参【{result.payload}】");
            InstructMsg(result.payload, "一体化取放料接口（立库）请求", "AgvGrabGoods");
            return result.payload;
        }



        /// <summary>
        /// 一体化出入库指令接口 by lit
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string TaskWarehouse(string json)
        {
            //调用Lse分发业务层
            _ProductionCreateTaskByLesProcess.CreateTaskByLesInit(json);

            TaskWarehouseResponse response = new TaskWarehouseResponse();
            var result = ResultPayload.GetResult(response);
            return result.payload;
        }


        /// <summary>
        /// 一体化出入库指令接口
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        //public string TaskWarehouse(string json)
        //{
        //    Log.Information($"一体化出入库指令接口，请求入参【{json}】");
        //    string srt = string.Empty;
        //    var request = json?.Deserialize<TaskWarehouseRequest>();
        //    if (request == null)
        //    {
        //        srt = ext;
        //        InstructMsg("", ext, "TaskWarehouse");
        //    }
        //    else
        //    {
        //        switch (request.operationDirection.ToUpper())
        //        {
        //            case "OUT"://出库

        //                string _cod = string.Empty;

        //                if (request.operationType == "emptyContainer")//emptyContainer【空托】
        //                {
        //                    string _trayType = request.materialCode;
        //                    string[] _trayTypeA;
        //                    if (_trayType.IsNullOrEmpty())
        //                    {
        //                        //直接给托盘条码
        //                    }
        //                    else
        //                    {
        //                        if (_trayType.IndexOf("-") > 0)
        //                        {
        //                            _trayTypeA = _trayType.Split('-');
        //                        }
        //                        else
        //                        {
        //                            throw new Exception("托盘类型必须带-");
        //                        }
        //                        _cod = _db.Ado.GetString($"select top 1 F_TrayNo from zjn_wms_tray where F_Type = '{_trayTypeA[1]}' and F_TrayStates = 0");
        //                        Log.Information($"查空托盘22：{_cod}");
        //                        if (_cod.IsNullOrEmpty())
        //                        {
        //                            Log.Information($"查空托盘33");
        //                            throw new Exception("没有可用空托盘");
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    string ssql = $" where F_GoodsCode = '{request.materialCode}' and F_Lock = 0 and F_State = 2";



        //                    if (request.batchNo.IsNotEmptyOrNull())
        //                    {
        //                        ssql = ssql + $" and F_BatchNo = '{request.batchNo}'";
        //                    }
        //                    if (request.batteryGradeNo.IsNotEmptyOrNull())
        //                    {
        //                        ssql = ssql + $" and F_BatteryGradeNo = '{request.batteryGradeNo}'";
        //                    }

        //                    Log.Information("出库12345");
        //                    string _trayNo = string.Empty;
        //                    if (request.diskInfoList.IsNotEmptyOrNull())
        //                    {
        //                        for (int i = 0; i < request.diskInfoList.Count; i++)
        //                        {
        //                            if (request.diskInfoList[i].trayBarcode.IsNotEmptyOrNull())
        //                            {
        //                                if (_trayNo.IsNullOrEmpty())
        //                                {
        //                                    _trayNo = "'" + request.diskInfoList[i].trayBarcode + "'";
        //                                }
        //                                else
        //                                {
        //                                    _trayNo = _trayNo + ",'" + request.diskInfoList[i].trayBarcode + "'";
        //                                }

        //                            }
        //                        }
        //                    }

        //                    if (_trayNo.IsNotEmptyOrNull())
        //                    {
        //                        ssql = $" where F_GoodsCode = '{request.materialCode}' and F_Lock = 0 and F_State = 2";
        //                        ssql = ssql + $" and F_TrayNo in({_trayNo}" + ")";
        //                        System.Data.DataTable dt = _db.Ado.GetDataTable($"select distinct f_trayno from zjn_wms_tray_goods {ssql}");
        //                        if (dt.Rows.Count <= 0)
        //                        {
        //                            throw new Exception($"库存没有指定的托盘可出库");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Log.Information($"ssql:{ssql}");
        //                        _cod = _db.Ado.GetString($"select SUM(F_Quantity)qty from zjn_wms_tray_goods {ssql}");
        //                        Double sd, qty;
        //                        Double.TryParse(_cod, out sd);
        //                        Double.TryParse(request.qty, out qty);
        //                        if (sd < qty)
        //                        {
        //                            var owe = qty - sd;
        //                            throw new Exception($"库存不足,差：{owe}");
        //                        }
        //                    }
        //                }
        //                TaskWarehouse_Out(json);
        //                break;
        //            case "INTO"://入库
        //                Log.Information("入库，开始所有校验。");
        //                if (request.diskInfoList.IsNotEmptyOrNull())
        //                {
        //                    if (request.diskInfoList[0].trayBarcode.IsNotEmptyOrNull())
        //                    {
        //                        var tray = _zjnBaseTrayRepository.AsQueryable().Where(w => w.TrayNo == request.diskInfoList[0].trayBarcode);
        //                        if (!tray.Any())
        //                        {
        //                            throw new Exception("容器不存在");
        //                        }
        //                        _cod = string.Empty;
        //                        _cod = _db.Ado.GetString($"select F_TrayNo from zjn_wms_tray_goods where F_TrayNo = '{request.diskInfoList[0].trayBarcode}' and f_lock <> 2");
        //                        if (_cod.Any())
        //                        {
        //                            throw new Exception("该托盘已在库存：" + request.diskInfoList[0].trayBarcode);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    throw new Exception("入参容器为空");
        //                }
        //                if (request.operationType == "production")//产品入库要做校验
        //                {
        //                    _cod = string.Empty;
        //                    _cod = _db.Ado.GetString($"select top 1 F_Code from zjn_base_les_goods where F_Code = '{request.materialCode}'");
        //                    if (!_cod.Any())
        //                    {
        //                        throw new Exception("物料不存在");
        //                    }
        //                }
        //                TaskWarehouse_Into(json);
        //                break;
        //            case "MOVE"://搬运
        //                if (request.diskInfoList.Count == 0)
        //                {
        //                    throw new Exception("入参没有搬运容器，任务失败");
        //                }
        //                TaskWarehouse_Move(json);
        //                break;
        //            case "DISPTCH"://调拨
        //                TaskWarehouse_Disptch(json);
        //                break;
        //        }
        //    }
        //    TaskWarehouseResponse response = new TaskWarehouseResponse();
        //    var result = ResultPayload.GetResult(response);
        //    Log.Information($"一体化出入库指令接口，请求出参【{result.payload}】");
        //    InstructMsg(result.payload, "一体化出入库指令接口", "TaskWarehouse");
        //    return result.payload;
        //}

        private async void TaskWarehouse_Out(string json)
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(10000);
            });

            //1.锁定托盘调出入库确认接口
            //2.到达立库出库口调出入库确认接口
            //3.AGV在库口开始搬运状态返馈接口
            //4.到达目的地调出入库确认接口。
            var request = json?.Deserialize<TaskWarehouseRequest>();
            if (request.operationType == "emptyContainer")//emptyContainer【空托】
            {
                Log.Information($"查空托盘1");
                string _trayType = request.materialCode;
                string[] _trayTypeA;
                string _cod = string.Empty;
                if (_trayType.IsNullOrEmpty())
                {
                    //直接给托盘条码
                }
                else
                {
                    if (_trayType.IndexOf("-") > 0)
                    {
                        _trayTypeA = _trayType.Split('-');
                    }
                    else
                    {
                        throw new Exception("物料类型必须带-");
                    }
                    _cod = _db.Ado.GetString($"select top 1 F_TrayNo from zjn_wms_tray where F_Type = '{_trayTypeA[1]}' and F_TrayStates = 0");
                    //1.锁定托盘调出入库确认接口
                    _db.Ado.ExecuteCommand($"update zjn_wms_tray set F_TrayStates = 2 where F_TrayNo = '{_cod}'");

                }

                InoutConFirmRequest dto = new InoutConFirmRequest();

                //模拟硬件执行10s
                System.Threading.Thread.Sleep(10000);


                dto = new InoutConFirmRequest();
                dto.requestId = YitIdHelper.NextId().ToString();
                dto.operationType = "emptyContainer";
                dto.toChannelId = "1642053037569";
                dto.clientCode = "ZJN05";
                dto.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dto.type = "arrive";//锁定lock 到货arrive
                dto.instructionNum = request.orderNo;
                dto.lines = new List<InoutConFirmLine>();
                dto.lines.Add(new InoutConFirmLine()
                {
                    fromLocator = "5001-W301-LK",//request.fromLocNo,
                    toLocator = request.toLocNo,//目标货位
                    container = _cod,//request.diskInfoList[0].trayBarcode,
                    materialBarcode = request.materialCode//"10000048-0"
                });
                Log.Information($"出入库确认，请求入参【{dto.Serialize()}】");
                var takeResult = _LESServer.InoutConFirm(dto);
                takeResult.Wait();
                var bb = takeResult.Result;
                //4.到达目的地调出入库确认接口。
                _db.Ado.ExecuteCommand($"update zjn_wms_tray set F_TrayStates = 3 where F_TrayNo = '{_cod}'");
            }
            else//production【产品】
            {
                //任务创建
                _db.Ado.ExecuteCommand($"insert into zjn_wms_task(F_TaskNo,F_TaskState) values('{request.orderNo}',1)");
                string _cod = string.Empty;


                string ssql = $" where F_GoodsCode = '{request.materialCode}' and F_Lock = 0 and F_State <> 1";
                if (request.batchNo.IsNotEmptyOrNull())
                {
                    ssql = ssql + $" and F_BatchNo = '{request.batchNo}'";
                }
                if (request.batteryGradeNo.IsNotEmptyOrNull())
                {
                    ssql = ssql + $" and F_BatteryGradeNo = '{request.batteryGradeNo}'";
                }

                //string ssql = "select top 1 F_TrayNo from zjn_wms_tray_goods where F_GoodsCode = '' and F_State = 0 and F_BatchNo = '' and F_BatteryGradeNo = ''";

                Log.Information("出库12345");
                string _trayNo = string.Empty;
                if (request.diskInfoList.IsNotEmptyOrNull())
                {
                    //string _trayNo = string.Empty;
                    for (int i = 0; i < request.diskInfoList.Count; i++)
                    {
                        if (request.diskInfoList[i].trayBarcode.IsNotEmptyOrNull())
                        {
                            //and F_TrayNo in('10000012202210WHZG000000000240', '10000012202210WHZG000000000244', '10000012202210WHZG000000000241')
                            if (_trayNo.IsNullOrEmpty())
                            {
                                _trayNo = "'" + request.diskInfoList[i].trayBarcode + "'";
                            }
                            else
                            {
                                _trayNo = _trayNo + ",'" + request.diskInfoList[i].trayBarcode + "'";
                            }

                        }
                    }


                }
                if (_trayNo.IsNotEmptyOrNull())
                {
                    ssql = $" where F_GoodsCode = '{request.materialCode}' and F_Lock = 0 and F_State = 2";
                    ssql = ssql + $" and F_TrayNo in({_trayNo}" + ")";
                    ssql = $"select distinct f_trayno from zjn_wms_tray_goods {ssql}";
                }
                else
                {
                    if (Convert.ToDecimal(request.qty) > 0)
                    {
                        //ssql = $"select F_TrayNo,F_Quantity,F_CreateTime from zjn_wms_tray_goods {ssql}";
                        ssql = $"select f_trayno,f_createtime,SUM(F_Quantity) OVER(ORDER BY f_createtime,f_quantity)qty from zjn_wms_tray_goods {ssql}";
                        Log.Information($"执行1SQL:{ssql}");
                        ssql = $"select distinct c.f_trayno from ({ssql})c,(select top 1 b.qty from ({ssql})b where b.qty >= {Convert.ToDecimal(request.qty)})d where c.qty <= d.qty";
                        Log.Information($"执行2SQL:{ssql}");
                    }
                    else
                    {
                        ssql = $"select distinct F_TrayNo from zjn_wms_tray_goods {ssql}";
                        Log.Information($"执行3SQL:{ssql}");
                    }
                }

                System.Data.DataTable dt = _db.Ado.GetDataTable($"{ssql}");
                int _LocNo = 0;
                foreach (System.Data.DataRow rw in dt.Rows)
                {
                    _LocNo = _LocNo + 1;
                    _cod = rw[0].ToString();
                    //1.锁定托盘调出入库确认接口
                    InoutConFirmRequest dto = new InoutConFirmRequest();
                    dto.requestId = YitIdHelper.NextId().ToString();
                    dto.operationType = "production";
                    dto.toChannelId = "1642053037569";
                    dto.clientCode = "ZJN05";
                    dto.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    dto.type = "lock";//锁定lock 到货arrive
                    dto.instructionNum = request.orderNo;
                    dto.lines = new List<InoutConFirmLine>();
                    string _toLocator = request.toLocNo;
                    if (_LocNo > 20)//限定每个出库口上限10
                    {
                        _toLocator = "5001-W301-003-002";
                    }
                    dto.lines.Add(new InoutConFirmLine()
                    {
                        fromLocator = request.fromWhouseNo,//"5001-W101-1000",//request.fromLocNo,
                        toLocator = _toLocator,//request.toLocNo,//"5001-W101-003-001",//立库出库口编码
                        container = _cod,//request.diskInfoList[0].trayBarcode,
                        materialBarcode = request.materialCode//"10000048-0"
                    });
                    Log.Information($"出入库确认，请求入参【{dto.Serialize()}】");
                    var takeResult = _LESServer.InoutConFirm(dto);
                    takeResult.Wait();
                    var bb = takeResult.Result;
                    if (bb.code != "0")
                    {
                        _db.Ado.ExecuteCommand($"update zjn_wms_task set F_EnabledMark = 4 where F_TaskNo = '{request.orderNo}'");
                        throw new Exception("调LES出库确认lock异常");
                    }
                    //锁定
                    _db.Ado.ExecuteCommand($"update zjn_wms_tray_goods set F_Lock = 1 where F_TrayNo = '{_cod}'");

                    //模拟硬件执行10s
                    System.Threading.Thread.Sleep(10000);

                    if (request.toLocNo.ToString() == "5001-W101-1000-02")//二楼出库口编号
                    {
                        //3.AGV在库口开始搬运状态返馈接口
                        Log.Information("AGV搬运1");
                        StatusFeedBackRequest _request = new StatusFeedBackRequest();
                        _request.requestId = YitIdHelper.NextId().ToString(); //"b7c00a81-238c-4c9d-b1e3-6ce528da6292";
                        _request.channelId = "LES";
                        _request.clientCode = "ZJN05";
                        _request.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //"2022-09-29 06:33:08";
                        _request.taskNum = request.orderNo;//"PC202210080002-00";
                        _request.taskStatus = "start";//开始“start”、暂停“suspend”、完成“complete”
                        _request.agvNum = "AGV001";
                        _request.fromLocatorCode = "W1-2F-W1001";//"JS-C129-JG1-M34-LY5";
                        _request.locatorCode = "W1-2F-W1002";//"5001-P101-100-001";
                        _request.location = "W1-2F-W1002-001";//"JS-P404-C01-P404-001";
                        _request.containerCode = _cod;//request.diskInfoList[0].trayBarcode;//"10000012202210WHZG000000000040";
                        var results = _LESServer.StatusFeedBack(_request);
                        results.Wait();
                        var cc = results.Result;
                        Log.Information("AGV搬运2");
                        //模拟硬件执行10s
                        System.Threading.Thread.Sleep(10000);
                        Log.Information("AGV搬运4");
                        _request.requestId = YitIdHelper.NextId().ToString(); //"b7c00a81-238c-4c9d-b1e3-6ce528da6292";
                        _request.channelId = "LES";
                        _request.clientCode = "ZJN05";
                        _request.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //"2022-09-29 06:33:08";
                        _request.taskNum = request.orderNo; //"PC202210080002-00";
                        _request.taskStatus = "complete";//开始“start”、暂停“suspend”、完成“complete”
                        _request.agvNum = "AGV001";
                        _request.fromLocatorCode = "W1-2F-W1001"; //"JS-C129-JG1-M34-LY5";
                        _request.locatorCode = "W1-2F-W1002"; //"5001-P101-100-001";
                        _request.location = "W1-2F-W1002-001"; //"JS-P404-C01-P404-001";
                        _request.containerCode = _cod;//request.diskInfoList[0].trayBarcode; //"10000012202210WHZG000000000040";
                        results = _LESServer.StatusFeedBack(_request);
                        results.Wait();
                        cc = results.Result;
                        Log.Information("AGV搬运3");
                    }

                    //4.到达目的地调出入库确认接口。
                    dto = new InoutConFirmRequest();
                    dto.requestId = YitIdHelper.NextId().ToString();
                    dto.operationType = "production";
                    dto.toChannelId = "1642053037569";
                    dto.clientCode = "ZJN05";
                    dto.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    dto.type = "arrive";//锁定lock 到货arrive
                    dto.instructionNum = request.orderNo;
                    dto.lines = new List<InoutConFirmLine>();
                    dto.lines.Add(new InoutConFirmLine()
                    {
                        fromLocator = "5001-W301-LK",//request.fromLocNo,
                        toLocator = _toLocator,//request.toLocNo,//目标货位
                        container = _cod,//request.diskInfoList[0].trayBarcode,
                        materialBarcode = request.materialCode//"10000048-0"
                    });
                    Log.Information($"出入库确认，请求入参【{dto.Serialize()}】");
                    takeResult = _LESServer.InoutConFirm(dto);
                    takeResult.Wait();
                    bb = takeResult.Result;
                    //已出库
                    _db.Ado.ExecuteCommand($"update zjn_wms_tray_goods set F_Lock = 2 where F_TrayNo = '{_cod}'");
                }
            }
        }
        private async void TaskWarehouse_Into(string json)
        {
            Log.Information("入库1");
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(10000);
            });
            var request = json?.Deserialize<TaskWarehouseRequest>();

            //创建任务
            //await _zjnTaskService.CreateByTaskWarehouse(request);
            Log.Information("入库2");
            if (request.operationType == "emptyContainer")//emptyContainer【空托】
            {
                //任务创建
                _db.Ado.ExecuteCommand($"insert into zjn_wms_task(F_TaskNo,F_TaskState) values('{request.orderNo}',1)");

                //模拟硬件执行10s
                System.Threading.Thread.Sleep(15000);

                //任务已发送
                _db.Ado.ExecuteCommand($"update zjn_wms_task set F_TaskState = 2 where F_TaskNo = '{request.orderNo}'");



                //插入库存
                _db.Ado.ExecuteCommand($"insert into zjn_wms_materialInventory(F_CreateUser,F_CreateTime,F_LastModifyUserId,F_LastModifyTime,F_ProductsCode,F_ProductsName,F_ProductsQuantity) values('admin',GETDATE(),'admin',GETDATE(),'空托垛','物料',44)");
                //托盘有料
                _db.Ado.ExecuteCommand($"update zjn_wms_tray set F_TrayStates = 1 where F_TrayNo = '{request.diskInfoList[0].trayBarcode}'");
                //托盘货物绑定

                //任务完成
                _db.Ado.ExecuteCommand($"update zjn_wms_task set F_TaskState = 3 where F_TaskNo = '{request.orderNo}'");
                //调用出入库确认
                InoutConFirmRequest dto = new InoutConFirmRequest();
                dto.requestId = YitIdHelper.NextId().ToString();
                dto.operationType = "emptyContainer";
                dto.toChannelId = "1642053037569";
                dto.clientCode = "ZJN05";
                dto.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dto.type = "arrive";
                dto.instructionNum = request.orderNo;
                dto.lines = new List<InoutConFirmLine>();
                dto.lines.Add(new InoutConFirmLine()
                {
                    fromLocator = request.fromLocNo,
                    toLocator = "5001-W101-003-001",//立库编码
                    container = request.diskInfoList[0].trayBarcode
                    //materialBarcode = ""//空托出入非必输
                });
                Log.Information($"出入库确认，请求入参【{dto.Serialize()}】");
                var takeResult = _LESServer.InoutConFirm(dto);
                takeResult.Wait();
                var bb = takeResult.Result;
            }
            else//production【产品】
            {
                Log.Information("入库3");

                //任务创建
                _db.Ado.ExecuteCommand($"insert into zjn_wms_task(F_TaskNo,F_TaskState) values('{request.orderNo}',1)");
                //模拟硬件执行10s
                System.Threading.Thread.Sleep(10000);
                //任务已发送
                _db.Ado.ExecuteCommand($"update zjn_wms_task set F_TaskState = 2 where F_TaskNo = '{request.orderNo}'");

                Log.Information("入库4");

                //W101 LES测试环境下代表原材料库，只有原材料库采购入库才有称重验证
                //if (request.fromLocNo.IndexOf("W101") > 0 && request.taskType.IndexOf("LTK") > 0)
                if (request.taskType.ToUpper() == "LTK")
                {
                    Log.Information("入库5");
                    //调用称重验证 
                    MaterialWeighingRequest dto1 = new MaterialWeighingRequest();
                    dto1.weight = "10";
                    dto1.containerCode = request.diskInfoList[0].trayBarcode;
                    dto1.requestId = YitIdHelper.NextId().ToString();
                    dto1.channelId = "1642053037569";
                    dto1.clientCode = "1642053037569";
                    dto1.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    Log.Information($"同步称重信息，请求入参【{dto1.Serialize()}】");
                    var results = _LESServer.MaterialWeighing(dto1);
                    results.Wait();
                    var aa = results.Result;
                    Log.Information($"同步称重信息，返回【{aa}】");
                }
                Log.Information("入库6");


                int _state = 0;
                //状态：0解冻(UNFROZEN) 1冻结(FROZEN) 2合格(OK) 3不合格(NG) 4待检验(PENDING) 5有效(ENABLE) 6失效(UNENABLE) 7盘点冻结(STOCKTAKE)
                switch (request.diskInfoList[0].inspectionResult.ToUpper())
                {
                    case "OK":
                        _state = 2;
                        break;
                    case "NG":
                        _state = 3;
                        break;
                    case "UNFROZEN":
                        _state = 0;
                        break;
                    case "FROZEN":
                        _state = 1;
                        break;
                    case "PENDING":
                        _state = 4;
                        break;
                    case "ENABLE":
                        _state = 5;
                        break;
                    case "UNENABLE":
                        _state = 6;
                        break;
                    case "STOCKTAKE":
                        _state = 7;
                        break;
                    default:
                        _state = 0;
                        break;
                }
                //托盘货物绑定
                for (int i = 0; i < request.diskInfoList.Count; i++)
                {
                    _db.Ado.ExecuteCommand($"insert into zjn_wms_tray_goods(F_GoodsId,F_GoodsCode,F_Quantity,F_Unit,F_TrayNo,F_State,F_BatchNo,F_BatteryGradeNo) values('{request.diskInfoList[i].barcode}','{request.materialCode}',{Convert.ToDecimal(request.diskInfoList[i].qty)},'','{request.diskInfoList[i].trayBarcode}','{_state}','{request.batchNo}','{request.batteryGradeNo}')");
                }
                Log.Information("入库7");
                //托盘货位绑定
                _db.Ado.ExecuteCommand($"update zjn_wms_location set F_LocationStatus = 1,F_TrayNo = '{request.diskInfoList[0].trayBarcode}',F_CreateTime = GETDATE() where F_LocationNo = (select top 1 F_LocationNo from zjn_wms_location where F_LocationStatus = 0)");

                Log.Information("入库77");
                //插入库存
                _db.Ado.ExecuteCommand($"insert into zjn_wms_materialInventory(F_CreateUser,F_CreateTime,F_LastModifyUserId,F_LastModifyTime,F_ProductsCode,F_ProductsName,F_ProductsQuantity) values('admin',GETDATE(),'admin',GETDATE(),'{request.materialCode}','物料',44)");
                Log.Information("入库8");
                //托盘有料
                _db.Ado.ExecuteCommand($"update zjn_wms_tray set F_TrayStates = 1 where F_TrayNo = '{request.diskInfoList[0].trayBarcode}'");
                Log.Information("入库9");
                //任务完成
                _db.Ado.ExecuteCommand($"update zjn_wms_task set F_TaskState = 3 where F_TaskNo = '{request.orderNo}'");

                Log.Information("入库10");
                //模拟硬件执行10s
                System.Threading.Thread.Sleep(10000);
                //调用出入库确认
                InoutConFirmRequest dto = new InoutConFirmRequest();
                dto.requestId = YitIdHelper.NextId().ToString();
                dto.operationType = "production";
                dto.toChannelId = "1642053037569";
                dto.clientCode = "ZJN05";
                dto.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dto.type = "arrive";
                dto.instructionNum = request.orderNo;
                dto.lines = new List<InoutConFirmLine>();
                Log.Information("入库11");
                dto.lines.Add(new InoutConFirmLine()
                {
                    fromLocator = request.fromLocNo,
                    toLocator = "5001-W101-003-001",//"ZHWH-W1",//立库编码
                    container = request.diskInfoList[0].trayBarcode,
                    materialBarcode = request.materialCode//"10000048-0"
                });
                Log.Information($"出入库确认，请求入参【{dto.Serialize()}】");
                var takeResult = _LESServer.InoutConFirm(dto);
                takeResult.Wait();
                var bb = takeResult.Result;
                Log.Information($"出入库确认，返回【{bb}】");

            }
        }
        private async void TaskWarehouse_Move(string json)
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);
                var request = json?.Deserialize<TaskWarehouseRequest>();
            });
            var request = json?.Deserialize<TaskWarehouseRequest>();
            for (int i = 0; i < request.diskInfoList.Count; i++)
            {
                if (request.diskInfoList[i].trayBarcode.IsNotEmptyOrNull())
                {
                    string _cod = request.diskInfoList[i].trayBarcode;
                    Log.Information("AGV搬运1");
                    StatusFeedBackRequest _request = new StatusFeedBackRequest();
                    _request.requestId = YitIdHelper.NextId().ToString(); //"b7c00a81-238c-4c9d-b1e3-6ce528da6292";
                    _request.channelId = "LES";
                    _request.clientCode = "ZJN05";
                    _request.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //"2022-09-29 06:33:08";
                    _request.taskNum = request.orderNo;//"PC202210080002-00";
                    _request.taskStatus = "start";//开始“start”、暂停“suspend”、完成“complete”
                    _request.agvNum = "AGV001";
                    _request.fromLocatorCode = "W3-2F-W1001";//"JS-C129-JG1-M34-LY5";
                    _request.locatorCode = "W3-2F-W1002";//"5001-P101-100-001";
                    _request.location = "W3-2F-W1002-001";//"JS-P404-C01-P404-001";
                    _request.containerCode = _cod;//request.diskInfoList[0].trayBarcode;//"10000012202210WHZG000000000040";
                    var results = _LESServer.StatusFeedBack(_request);
                    results.Wait();
                    var cc = results.Result;
                    Log.Information("AGV搬运2");
                    //模拟硬件执行10s
                    System.Threading.Thread.Sleep(10000);
                    Log.Information("AGV搬运4");
                    _request.requestId = YitIdHelper.NextId().ToString(); //"b7c00a81-238c-4c9d-b1e3-6ce528da6292";
                    _request.channelId = "LES";
                    _request.clientCode = "ZJN05";
                    _request.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //"2022-09-29 06:33:08";
                    _request.taskNum = request.orderNo; //"PC202210080002-00";
                    _request.taskStatus = "complete";//开始“start”、暂停“suspend”、完成“complete”
                    _request.agvNum = "AGV001";
                    _request.fromLocatorCode = "W5-2F-W1001"; //"JS-C129-JG1-M34-LY5";
                    _request.locatorCode = "W5-2F-W1002"; //"5001-P101-100-001";
                    _request.location = "W5-2F-W1002-001"; //"JS-P404-C01-P404-001";
                    _request.containerCode = _cod;//request.diskInfoList[0].trayBarcode; //"10000012202210WHZG000000000040";
                    results = _LESServer.StatusFeedBack(_request);
                    results.Wait();
                    cc = results.Result;
                    Log.Information("AGV搬运3");
                    InoutConFirmRequest dto = new InoutConFirmRequest();

                    //4.到达目的地调出入库确认接口。
                    dto = new InoutConFirmRequest();
                    dto.requestId = YitIdHelper.NextId().ToString();
                    dto.operationType = "production";
                    dto.toChannelId = "1642053037569";
                    dto.clientCode = "ZJN05";
                    dto.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    dto.type = "arrive";//锁定lock 到货arrive
                    dto.instructionNum = request.orderNo;
                    dto.lines = new List<InoutConFirmLine>();
                    dto.lines.Add(new InoutConFirmLine()
                    {
                        fromLocator = "5001-W301-LK",//request.fromLocNo,
                        toLocator = request.toLocNo,//目标货位
                        container = _cod,//request.diskInfoList[0].trayBarcode,
                        materialBarcode = request.materialCode//"10000048-0"
                    });
                    Log.Information($"出入库确认，请求入参【{dto.Serialize()}】");
                    var takeResult = _LESServer.InoutConFirm(dto);
                    takeResult.Wait();
                    var bb = takeResult.Result;
                }
            }
        }
        private async void TaskWarehouse_Disptch(string json)
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(10000);
                var request = json?.Deserialize<TaskWarehouseRequest>();

            });


        }

        private void InstructMsg(string rst, string msg, string interfaces)
        {
            if (string.IsNullOrEmpty(rst))
            {
                _db.Ado.ExecuteCommand($"update zjn_task_interface_apply_log set F_Msg = '{msg}' where F_InterfaceName = '{interfaces}' and F_CreateTime = (select max(F_CreateTime) from zjn_task_interface_apply_log where F_InterfaceName = '{interfaces}')");
            }
            else
            {
                _db.Ado.ExecuteCommand($"update zjn_task_interface_apply_log set F_OutParameter = '{rst}',F_Msg = '{msg}' where F_InterfaceName = '{interfaces}' and F_CreateTime = (select max(F_CreateTime) from zjn_task_interface_apply_log where F_InterfaceName = '{interfaces}')");
            }
        }
    }
}
