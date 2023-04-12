using HSZ;
using HSZ.JsonSerialization;
using Microsoft.Extensions.Options;
using Serilog;
using ServiceReferenceLES;
using ServiceReferenceLESLogin;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Calb.Client.DTO;

namespace ZJN.Calb.Client
{
    public class LESServerClient
    {

        private readonly tokenRequestDTO _TokenRequest;
        private readonly WebSerivcesConfig _WebConfig;
        private readonly SqlSugarScope _db;
        private readonly string WarehouseNo = App.Configuration[$"sysno"];


        public LESServerClient(IOptionsSnapshot<WebSerivcesConfig> config, IOptionsSnapshot<LESLoginConfig> login)
        {
            _WebConfig = config.Value;
            _TokenRequest = new tokenRequestDTO();
            _TokenRequest.clientId = "calb-mes";
            _TokenRequest.clientSecret = "calb-mes";
            _TokenRequest.grantType = "password";
            _TokenRequest.password = login.Value.Password;
            _TokenRequest.userName = login.Value.UserName;
            _db = DbScoped.SugarScope;

        }

        public async Task<T?> SoapClient<T>(object payload) where T : class
        {
            requestPayloadDTO dto = new requestPayloadDTO();
            dto.payload = payload.Serialize();
            try
            {
                var tokenResult = await Login();
                Task<invokeResponse1> responseTask;
                if (tokenResult.status == "200")
                {
                    ApiSoapInvokeControllerClient client = new ApiSoapInvokeControllerClient(_WebConfig.LES);
                    using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
                    {
                        var httpRequestProperty = new HttpRequestMessageProperty();
                        httpRequestProperty.Headers[tokenResult.key] = tokenResult.value;
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                        responseTask = client.invokeAsync("CALB", "WAREHOUSE_WS", "calb-mes.wmswebserviceapi.wareHouseInterFace", dto);
                    }
                }
                else
                {
                    throw new Exception(tokenResult.msg);
                }

                var response = await responseTask;
                var result = response.@return;
                if (result.status == "200")
                {
                    var resultMessage = result.payload?.Deserialize<ResponsePayload>();
                    if (resultMessage.code == "200")
                    {
                        if (string.IsNullOrEmpty(resultMessage.output))
                        {
                            return null;
                        }
                        else
                        {
                            Log.Information(resultMessage.output);
                            return resultMessage.output.Deserialize<T>();
                        }
                    }
                    else
                    {
                        throw new Exception(resultMessage.message);
                    }
                }
                else
                {
                    throw new Exception(result.message);
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<tokenResponseDTO> Login()
        {
            try
            {
                tokenInfoGetClient client = new tokenInfoGetClient(_WebConfig.LESLogin);
                var tokenResult = await client.tokenInfoAsync(_TokenRequest);
                Log.Information($"账号：{tokenResult.@return.key} Token:{tokenResult.@return.value}");
                return tokenResult.@return;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        /// <summary>
        /// 出入库申请接口
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        public async Task<ContainerIntoApplyResponse> ContainerIntoApply(ContainerIntoApplyRequest requst)
        {
            var payload = PayloadRequest.Payload(requst, "CONTAINER_INTO_APPLY");
            #region 与LES模拟测试代码
            Log.Information($"出入库申请接口H");
            var _payload = await SoapClient<ContainerIntoApplyResponse>(payload);
            //模拟硬件执行10s
            System.Threading.Thread.Sleep(10000);
            Log.Information($"{WarehouseNo}出入库{_payload.code}申请接口H1{_payload.instructionNum}");
            //调出入库申请接口成功
            if (_payload.code == "0")
            {
                //调出入库确认接口
                if (requst.operationDirection.ToUpper() == "INTO")
                {
                    Log.Information($"出入库申请接口2");
                    //空托
                    if (requst.operationType.ToUpper() == "EMPTYCONTAINER")
                    {
                        Log.Information($"出入库申请接口3");
                        InoutConFirmRequest dto = new InoutConFirmRequest();
                        dto = new InoutConFirmRequest();
                        dto.requestId = YitIdHelper.NextId().ToString();
                        //默认输入emptyContainer【空托】 production【产品】
                        dto.operationType = "emptyContainer";
                        //立库服务器IP
                        dto.toChannelId = "1642053037569";
                        //立库代码
                        dto.clientCode = requst.clientCode;
                        dto.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //锁定lock 到达arrive
                        dto.type = "arrive";
                        Log.Information($"出入库申请接口333");
                        //任务唯一码
                        dto.instructionNum = _payload.instructionNum;
                        dto.lines = new List<InoutConFirmLine>();
                        Log.Information($"出入库申请接口4");
                        dto.lines.Add(new InoutConFirmLine()
                        {
                            //来源位置
                            fromLocator = requst.location,
                            //目标货位
                            toLocator = WarehouseNo,
                            //容器编码
                            container = requst.containerCode,
                        });
                        Log.Information($"出入库确认，请求入参【{dto.Serialize()}】");
                        var takeResult = InoutConFirm(dto);
                        takeResult.Wait();
                        var _result = takeResult.Result;
                        Log.Information($"出入库申请接口5");
                    }
                    //产品
                    if (requst.operationType.ToUpper() == "PRODUCTION")
                    {
                        int _state = 2;
                        //托盘与产品绑定
                        _db.Ado.ExecuteCommand($"insert into zjn_wms_tray_goods(F_GoodsId,F_GoodsCode,F_Quantity,F_Unit,F_TrayNo,F_State,F_BatchNo,F_BatteryGradeNo) values('{_payload.responsesLine[0].productionCode}','{_payload.materialCode}',{Convert.ToDecimal(_payload.responsesLine[0].qty)},'','{requst.containerCode}','{_state}','{_payload.lot}','{_payload.responsesLine[0].batterGradeNo}')");
                        //托盘与货位绑定
                        _db.Ado.ExecuteCommand($"update zjn_wms_location set F_LocationStatus = 1,F_TrayNo = '{requst.containerCode}',F_CreateTime = GETDATE() where F_LocationNo = (select top 1 F_LocationNo from zjn_wms_location where F_LocationStatus = 0)");
                        //托盘有料
                        _db.Ado.ExecuteCommand($"update zjn_wms_tray set F_TrayStates = 1 where F_TrayNo = '{requst.containerCode}'");
                        Log.Information("入库9");
                        Log.Information($"出入库申请接口6");
                        InoutConFirmRequest dto = new InoutConFirmRequest();
                        dto = new InoutConFirmRequest();
                        dto.requestId = YitIdHelper.NextId().ToString();
                        //默认输入emptyContainer【空托】 production【产品】
                        dto.operationType = "production";
                        //立库服务器IP
                        dto.toChannelId = "1642053037569";
                        //立库代码  ZJN05
                        dto.clientCode = requst.clientCode;
                        dto.requestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //锁定lock 到达arrive
                        dto.type = "arrive";
                        //任务唯一码
                        dto.instructionNum = _payload.instructionNum;
                        Log.Information($"出入库申请接口7");
                        dto.lines = new List<InoutConFirmLine>();
                        dto.lines.Add(new InoutConFirmLine()
                        {
                            //来源位置
                            fromLocator = requst.location,
                            //目标货位
                            toLocator = WarehouseNo,
                            //容器编码
                            container = requst.containerCode,
                        });
                        Log.Information($"出入库确认，请求入参【{dto.Serialize()}】");
                        var takeResult = InoutConFirm(dto);
                        takeResult.Wait();
                        var _result = takeResult.Result;
                        Log.Information($"出入库申请接口8");
                    }
                }
            }
            #endregion
            return _payload;//await SoapClient<ContainerIntoApplyResponse>(payload);
        }

        /// <summary>
        /// 出入库确认接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<InoutConFirmResponse> InoutConFirm(InoutConFirmRequest request)
        {
            var payload = PayloadRequest.Payload(request, "INOUT_CONFIRM");
            return await SoapClient<InoutConFirmResponse>(payload);
        }


        /// <summary>
        /// 材料称重接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<MaterialWeighingResponse> MaterialWeighing(MaterialWeighingRequest request)
        {
            var payload = PayloadRequest.Payload(request, "MATERIAL_WEIGHING");
            return await SoapClient<MaterialWeighingResponse>(payload);
        }


        /// <summary>
        /// 容器清洗状态更新接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ContainerCleaningStatusChangeResponse> ContainerCleaningStatusChange(ContainerCleaningStatusChangeRequest request)
        {

            var payload = PayloadRequest.Payload(request, "CONTAINER_CLEANING_STATUS_CHANGE");
            return await SoapClient<ContainerCleaningStatusChangeResponse>(payload);
        }


        /// <summary>
        /// 指令执行异常反馈接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ErrorFeedBackResponse> ErrorFeedBack(ErrorFeedBackRequest request)
        {
            var payload = PayloadRequest.Payload(request, "ERROR_FEED_BACK");
            return await SoapClient<ErrorFeedBackResponse>(payload);
        }

        /// <summary>
        /// 搬运状态反馈-AGV
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<StatusFeedBackResponse> StatusFeedBack(StatusFeedBackRequest request)
        {
            var payload = PayloadRequest.Payload(request, "STATUS_FEED_BACK");
            return await SoapClient<StatusFeedBackResponse>(payload);
        }
    }
}
