using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 出入库指令接口 请求
    /// </summary>

    public class TaskWarehouseRequest
    {
        [Required()]
        public string clientCode { get; set; }

        [Required()]
        [DisplayName("操作方向")]
        public string operationDirection { get; set; }

        [Required()]
        [DisplayName("操作类型")]
        public string operationType { get; set; }

        public string taskType { get; set; }

        public string orderNo { get; set; }

        public string materialCode { get; set; }
        public string fromLocNo { get; set; }
        public string toLocNo { get; set; }
        public string fromWhouseNo { get; set; }
        public string toWhouseNo { get; set; }
        public string fromPoint { get; set; }
        public string toPoint { get; set; }
        public string batchNo { get; set; }
        public string batteryGradeNo { get; set; }
        public string status { get; set; }
        public string productLineNo { get; set; }
        public string qty { get; set; }
        public string priority { get; set; }

        public List<TaskWarehouseRequestLine> diskInfoList { get; set; }
    }


    public class TaskWarehouseRequestLine : AttributeExt
    {
        public string trayBarcode { get; set; }

        public string positionNo { get; set; }

        public string barcode { get; set; }
        public string inspectionResult { get; set; }
        public string qty { get; set; }
        public string productDate { get; set; }
        public string expDate { get; set; }
        public string cProductSign { get; set; }
        public string cProductOrder { get; set; }
        public string cBatterGradeNo { get; set; }
        public int? cCapacity { get; set; }
        public int? cVoltageOcv1 { get; set; }
        public int? cVoltageOcv2 { get; set; }
        public int? cResistanceAc { get; set; }
        public int? cResistanceDc { get; set; }
        public int? ckValue { get; set; }
        public string productLineNo { get; set; }
        public string cOcv2Date { get; set; }
        public string cProdline { get; set; }
    }
}
