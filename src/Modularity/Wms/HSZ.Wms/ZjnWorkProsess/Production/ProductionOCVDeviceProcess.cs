using HSZ.Common.Core.Manager;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.Wms.Interfaces.ZjnWorkProsess;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Tls;
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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace HSZ.Wms.ZjnWorkProsess.Production
{
    public class ProductionOCVDeviceProcess : IProductionOCVDeviceProcess,IDynamicApiController, ITransient
    {
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly LESServerClient _ESServer;
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayEntity> _zjnWmsWmsTrayRepository;
        private readonly ISqlSugarRepository<ZjnWmsOcvbatteriesEntity> _zjnWmsOcvbatteriesRepository;
        private readonly ISqlSugarRepository<ZjnWmsOcvbatteriesResponsesLineEntity> _zWmsOcvbatteriesResponsesRepository;
        public ProductionOCVDeviceProcess(
            //SqlSugarScope db,
            LESServerClient ESServer,
            ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository,
            ISqlSugarRepository<ZjnWmsOcvbatteriesEntity> zjnWmsOcvbatteriesRepository,
            ISqlSugarRepository<ZjnWmsTrayEntity> zjnWmsWmsTrayRepository,
            ISqlSugarRepository<ZjnWmsOcvbatteriesResponsesLineEntity> zWmsOcvbatteriesResponsesRepository,
        IUserManager userManager)
        {
            _zWmsOcvbatteriesResponsesRepository = zWmsOcvbatteriesResponsesRepository;
            _zjnWmsOcvbatteriesRepository = zjnWmsOcvbatteriesRepository;
            _zjnWmsWmsTrayRepository = zjnWmsWmsTrayRepository;
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
            _ESServer = ESServer;
            //_db = db;
            _db = DbScoped.SugarScope;
            _userManager = userManager;
        }
        /// <summary>
        ///  分选扫码申请接口
        /// </summary>
        /// <param name="requst">传入参数</param>
        /// <returns></returns>
        public async Task<bool> RequestForCellPanel(ContainerIntoApplyRequest requst)
        {
            try
            {



                


                requst.operationDirection = "INTO";//操作方向 out
                requst.operationType = "PRODUCTION";//默认输入emptyContainer【空托】；production【产品】
                requst.requestId = DateTime.Now.ToString();//防止数据重复提交
                requst.clientCode = "ZJN05";//客户端编码（ZJN）
                requst.channelId = "10.79.5.14";//链路通道唯一编码（socket），目标服务器编码，便于反馈
                requst.quantity = "1";//出空托时必输，出库数量；入库时默认为1
                requst.requestTime =DateTime.Now.ToString();
                requst.location = "";//操作货位

                var payload = PayloadRequest.Payload(requst, "CONTAINER_INTO_APPLY");
                //调用接口返回参数
                var _payload = await _ESServer.SoapClient<ContainerIntoApplyResponse>(payload);

                //测试阶段托盘过来，自动添加托盘信息
                var tray = await _zjnWmsWmsTrayRepository.AsSugarClient().Queryable<ZjnWmsTrayEntity>().FirstAsync(x => x.TrayNo == _payload.containerCode);
                if (tray == null)
                {
                    ZjnWmsTrayEntity trayEntity = new ZjnWmsTrayEntity();
                    trayEntity.Id = YitIdHelper.NextId().ToString();
                    trayEntity.TrayNo = _payload.containerCode;
                    trayEntity.TrayName = "测试添加-" + _payload.containerCode;
                    trayEntity.Type = 1;
                    trayEntity.CreateUser = "WCS";
                    trayEntity.CreateTime = DateTime.Now;
                    trayEntity.EnabledMark = 1;
                    trayEntity.IsDelete = 0;
                    trayEntity.TrayStates = 1;
                    await _zjnWmsWmsTrayRepository.AsSugarClient().Insertable(trayEntity).ExecuteCommandAsync();
                }


                if (_payload.code == "0")
                {
                    // 调出入库确认接口
                    if (requst.operationDirection.ToUpper() == "INTO")
                    {
                        if (requst.operationType.ToUpper() == "PRODUCTION")
                        {
                            var ocv = await _zjnWmsOcvbatteriesRepository.GetFirstAsync(x => x.InstructionNum == _payload.instructionNum && x.IsDeleted == 0);
                            if (ocv == null)
                            {
                                ZjnWmsOcvbatteriesEntity zjnWmsOcvbatteries = new ZjnWmsOcvbatteriesEntity();
                                zjnWmsOcvbatteries.Id = YitIdHelper.NextId().ToString();
                                zjnWmsOcvbatteries.InstructionNum = _payload.instructionNum;
                                zjnWmsOcvbatteries.IsDeleted = 0;
                                zjnWmsOcvbatteries.Lot = _payload.lot;
                                zjnWmsOcvbatteries.ProductionCode = _payload.productionCode;
                                zjnWmsOcvbatteries.MaterialCode = _payload.materialCode;
                                zjnWmsOcvbatteries.ContainerCode = _payload.containerCode;
                                List<ZjnWmsOcvbatteriesResponsesLineEntity> list = new List<ZjnWmsOcvbatteriesResponsesLineEntity>();
                                foreach (var item in _payload.responsesLine)
                                {
                                    ZjnWmsOcvbatteriesResponsesLineEntity responsesLineEntity = new ZjnWmsOcvbatteriesResponsesLineEntity()
                                    {
                                        Id = YitIdHelper.NextId().ToString(),
                                        BatteriesId = zjnWmsOcvbatteries.Id,
                                        ProductOrder = item.productOrder,
                                        Prodline = item.prodline,
                                        ProductionLine = item.productionLine,
                                        ProductSign = item.productSign,
                                        Attribute1 = item.attribute1,
                                        Capacity = item.capacity,
                                        KValue = item.kValue,
                                        BatterGradeNo = item.batterGradeNo,
                                        Container = item.container,
                                        InspectionResult=item.inspectionResult,
                                        IsDeleted=0,
                                        Ocv2Date=Convert.ToDateTime(item.ocv2Date),
                                        VoltageOcv1=item.voltageOcv1,
                                        VoltageOcv2=item.voltageOcv2,
                                        ProductionCode=item.productionCode,
                                        PositionNo=item.positionNo,
                                        ResistanceAc=item.resistanceAc,
                                        ProductDate=item.productDate,
                                        Qty=Convert.ToInt32(item.qty),
                                        ResistanceDc=item.resistanceDc,
                                    };

                                    list.Add(responsesLineEntity);
                                }

                                //开启事务
                                _db.BeginTran();
                                //新增记录
                                 await _zWmsOcvbatteriesResponsesRepository.AsInsertable(list).ExecuteReturnEntityAsync();
                                //新增记录
                                 await _zjnWmsOcvbatteriesRepository.AsInsertable(zjnWmsOcvbatteries).ExecuteReturnEntityAsync();

                                _db.CommitTran();
                               
                            }

                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ex.Message);
            }
        }

    }
}
