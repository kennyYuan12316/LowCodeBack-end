using Aspose.Cells;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.UnifyResult;
using HSZ.wms.Interfaces.ZjnServicePathConfig;

using HSZ.wms.ZjnWmsTask;
using HSZ.Wms.Interfaces.ZjnWmsAgv;
using HSZ.Wms.ZjnWorkProsess;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Spire.Presentation;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Agv.Entitys.Dto.AgvCreateOrder;
using ZJN.Agv.Entitys.Dto.AgvPdaTask;
using ZJN.Agv.Entitys.Entity;
using ZJN.Calb.Entitys.Dto;
using System.Net.Http.Headers;
using HttpStatusCode = System.Net.HttpStatusCode;
using Renci.SshNet;
using ZJN.Agv.Entitys.Dto.AgvTaskStatus;
using ZJN.Agv.Entitys.Dto.AgvLimitGoods;
using HSZ.wms.Entitys.Dto.ZjnWmsTask;

namespace HSZ.Wms.ZjnWmsAgv
{
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsTask", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsAgvService : IZjnWmsAgvService, IDynamicApiController, ITransient
    {
        
        private readonly IUserManager _userManager;
       
        private readonly ISqlSugarRepository<ZjnBaseStdPdataskEntity> _zjnBaseStdPdataskRepository;
        private readonly ISqlSugarRepository<ZjnBaseStdCreateorderEntity> _zjnBaseStdCreateorderRepository;
        private readonly ISqlSugarRepository<ZjnBaseStdLimitgoodsEntity> _zjnBaseStdLimitgoodsRepository;
        private readonly ISqlSugarRepository<ZjnWmsTaskEntity> _zjnTaskListRepository;
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnTaskListDetailsRepository;
        //分发业务类

        private readonly ZjnTaskService _ZjnTaskServiceOutProcess;

        public ZjnWmsAgvService(
          IUserManager userManager,
          
          ISqlSugarRepository<ZjnBaseStdPdataskEntity> zjnBaseStdPdataskRepository, ISqlSugarRepository<ZjnWmsTaskEntity> zjnTaskListRepository,
          ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnTaskListDetailsRepository,
        ISqlSugarRepository<ZjnBaseStdCreateorderEntity> zjnBaseStdCreateorderRepository,
          ICacheManager cacheManager)
        {
            _zjnBaseStdCreateorderRepository= zjnBaseStdCreateorderRepository;
            _zjnBaseStdPdataskRepository = zjnBaseStdPdataskRepository;           
            _userManager = userManager;
           
        }
        /// <summary>
        /// AGV上传立库PDA任务信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AGVUploadPDA([FromBody] ZjnBaseStdPdataskCrInput input) {

            try
            {
                //保存插入信息
                var userInfo = await _userManager.GetUserInfo();
                var entity = input.Adapt<ZjnBaseStdPdataskEntity>();
                entity.Id = YitIdHelper.NextId().ToString();
                entity.CreateTime = DateTime.Now;
                var isOk = await _zjnBaseStdPdataskRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
                if (isOk > 0)
                {
                    TaskWarehouseRequest taskWarehouseRequest = new TaskWarehouseRequest();
                    taskWarehouseRequest.orderNo = input.requestId;
                    taskWarehouseRequest.materialCode = input.goodsCode;
                    taskWarehouseRequest.qty = input.quantity.ToString();
                    taskWarehouseRequest.priority = "1";
                    taskWarehouseRequest.fromLocNo = input.startAreaCode;
                    taskWarehouseRequest.operationDirection = input.taskType.ToString();
                    List<TaskWarehouseRequestLine> diskInfoList = new List<TaskWarehouseRequestLine>();
                    TaskWarehouseRequestLine taskWarehouse = new TaskWarehouseRequestLine();
                    taskWarehouse.trayBarcode = input.containerCode;
                    diskInfoList.Add(taskWarehouse);
                    taskWarehouseRequest.diskInfoList = diskInfoList;
                    //创建任务
                  await _ZjnTaskServiceOutProcess.CreateByTaskWarehouse(taskWarehouseRequest);

                }
                else {

                    throw HSZException.Oh(ErrorCode.COM1000);
                }

            }
            catch (Exception ex)
            {
                throw HSZException.Oh(ex.Message);

            }

        }

        /// <summary>
        /// 请求取放
        /// </summary>
        public async Task PostAgvTaskLimit(ZjnBaseStdLimitgoodsCrInput input)
        {
            //wcs通讯plc获取是否空闲的状态
            var entity = input.Adapt<ZjnBaseStdLimitgoodsEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            var isOk = await _zjnBaseStdLimitgoodsRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            //wcs通讯plc获取是否空闲的状态，返回200就是成功，不是200就 不允许，AGV继续请求

        }
        /// <summary>
        /// 下单接口
        /// </summary>
        /// <param name="brCode"></param>
        /// <param name="HardNo"></param>
        /// <returns></returns>
        public async Task AGVOrderInterface(string brCode,string HardNo,string PositionTo) {

            ZjnBaseStdCreateorderCrInput eStdCreateorderinput = new ZjnBaseStdCreateorderCrInput();
            //呼叫AGV送料的起点为设备编号
            eStdCreateorderinput.startLocCode = HardNo;
            eStdCreateorderinput.brCode = brCode;
           
            //呼叫AGV送料的终点为AGV发送pda任务的终点编码
            eStdCreateorderinput.endAreaCode = PositionTo;
            //url可以保存在数据库
            //string agvUrl = "http://localhost:58504/api/agv/zjnbasestdpdatask";
            string agvUrl = "";
         var rlu=  await PostAsync<RESTfulResult_AgvTaskStatus>(agvUrl, eStdCreateorderinput);
        }

      
        /// <summary>
        /// 调用第三方接口方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="System.Exception"></exception>
        public  async Task<T> PostAsync<T>(string url, object data) where T : class, new()
        {
            try
            {
                var client = CreateHttpClient();
                string content = JsonConvert.SerializeObject(data);
                string startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(url, byteContent).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();

                string strResult = result;
               
                string stopTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
              
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"GetAsync End, url:{url}, HttpStatusCode:{response.StatusCode}, result:{result}");
                }
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)result;
                }

                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string responseContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    throw HSZException.Oh(ex.Message);
                }
                throw;
            }
        }


        /// <summary>
        /// 如果是本地服务，不使用https
        /// </summary>
        /// <returns></returns>
        private static HttpClientHandler GetInsecureHandler()
        {
            var handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;//同时发送默认凭证
            return handler;
        }

        public static HttpClient CreateHttpClient()
        {
            var handler = GetInsecureHandler();
            HttpClient httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            return httpClient;
        }

    }
}
