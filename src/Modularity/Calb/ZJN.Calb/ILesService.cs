using Microsoft.AspNetCore.Http;
using Pre.Calb.Entitys.Dto;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ZJN.Calb.Entitys.Dto;
using ZJN.Entitys.cabl;

namespace ZJN.Calb
{
    [ServiceContract]
    public interface ILesService
    {
        /// <summary>
        /// 物料主数据
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string MainMaterialData_Line(string json);

        /// <summary>
        /// 配送任务指令-AGV
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string CONTAINER_PICK_PUT(string json);





        /// <summary>
        /// 质量状态变更接口
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string CHANGE_STATUS(string json);



        /// <summary>
        /// 立库库存报表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string WmsGoodsReport(string json);


        /// <summary>
        /// 容器清洗周期同步
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string ContainerCleanCycleSync(string json);

        /// <summary>
        /// 出入库指令取消接口
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string CONTAINER_INTO_STOP(string json);

        /// <summary>
        /// 空托呼叫
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string ARTIFICIAL_CALLEMPTYTRAY(string json);


        /// <summary>
        /// 配送任务指令-AGV
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string AgvGoods(string json);


        /// <summary>
        /// 取料放料接口（立库）请求
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string AgvGrabGoods(string json);



        /// <summary>
        /// 出入库指令接口
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string TaskWarehouse(string json);

    }
}