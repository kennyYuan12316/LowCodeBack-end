namespace ZJN.Calb.Client.DTO
{

    public class ContainerIntoApplyRequest : AttributeExt
    {
        /// <summary>
        /// 防止数据重复提交
        /// </summary>
        public string requestId { get; set; }

        /// <summary>
        /// 链路通道唯一编码（socket）
        ///目标服务器编码，便于反馈
        /// </summary>
        public string channelId { get; set; }

        /// <summary>
        /// 客户端编码（ZJN）
        /// </summary>
        public string clientCode { get; set; }

        /// <summary>
        /// 请求发送时间，格式yyyy-MM-dd hh:mm:ss
        /// </summary>
        public string requestTime { get; set; }

        /// <summary>
        /// 默认输入out【出库】，into【入库】
        /// </summary>
        public string operationDirection { get; set; }

        /// <summary>
        /// 默认输入emptyContainer【空托】；production【产品】
        /// </summary>
        public string operationType { get; set; }

        /// <summary>
        /// 出空托时必输，出库数量；入库时默认为1
        /// </summary>
        public string quantity { get; set; }

        /// <summary>
        /// 操作货位
        /// </summary>
        public string location { get; set; }

        /// <summary>
        /// 容器编码 必输，LES确认是否可以出入库，当即反馈是否可以出入库
        /// </summary>
        public string containerCode { get; set; }

        /// <summary>
        /// 产品条码 出库申请时必输
        /// </summary>
        public string productionCode { get; set; }

    }

    public class ContainerIntoApplyRequestLine : AttributeExt
    {
        public string container { get; set; }
        public string productionCode { get; set; }
        public string inspectionResult { get; set; }
        public string productSign { get; set; }
        public string productOrder { get; set; }
        public string batterGradeNo { get; set; }
        public string positionNo { get; set; }
        public string capacity { get; set; }
        public string voltageOcv1 { get; set; }
        public string voltageOcv2 { get; set; }
        public string resistanceAc { get; set; }
        public string resistanceDc { get; set; }
        public string kValue { get; set; }
        public string productDate { get; set; }
        public string productionLine { get; set; }
        public string ocv2Date { get; set; }
        public string prodline { get; set; }
        public string qty { get; set; }
    }

}