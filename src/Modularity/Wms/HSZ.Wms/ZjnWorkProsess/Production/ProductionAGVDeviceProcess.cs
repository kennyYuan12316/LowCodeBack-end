using HSZ.Common.Core.Manager;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Crypto.Tls;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Calb.Client;
using ZJN.Calb.Client.DTO;

namespace HSZ.Wms.ZjnWorkProsess.Production
{
    public class ProductionAGVDeviceProcess : IDynamicApiController, ITransient
    {
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly LESServerClient _ESServer;
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _zjnWmsLocationRepository;
        private readonly ISqlSugarRepository<ZjnWmsOcvbatteriesEntity> _zjnWmsOcvbatteriesRepository;
        private readonly ISqlSugarRepository<ZjnWmsOcvbatteriesResponsesLineEntity> _zWmsOcvbatteriesResponsesRepository;
        public ProductionAGVDeviceProcess(
            SqlSugarScope db,
            LESServerClient ESServer,
            ISqlSugarRepository<ZjnWmsLocationEntity> zjnWmsLocationRepository,
            ISqlSugarRepository<ZjnWmsOcvbatteriesEntity> zjnWmsOcvbatteriesRepository,
            ISqlSugarRepository<ZjnWmsOcvbatteriesResponsesLineEntity> zWmsOcvbatteriesResponsesRepository,
        IUserManager userManager)
        {
            _zWmsOcvbatteriesResponsesRepository = zWmsOcvbatteriesResponsesRepository;
            _zjnWmsOcvbatteriesRepository = zjnWmsOcvbatteriesRepository;
            _zjnWmsLocationRepository = zjnWmsLocationRepository;
            _ESServer = ESServer;
            _db = db;
            _userManager = userManager;
        }
        /// <summary>
        ///  调用出入库申请接口
        /// </summary>
        /// <param name="requst">传入参数</param>
        /// <returns></returns>
        public async Task<bool> RequestForCellPanel(ContainerIntoApplyRequest requst)
        {
            try
            {
                var payload = PayloadRequest.Payload(requst, "CONTAINER_INTO_APPLY");
                //调用接口返回参数
                var _payload = await _ESServer.SoapClient<ContainerIntoApplyResponse>(payload);
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

                throw HSZException.Oh(ex.Message);
            }
        }

    }
}
