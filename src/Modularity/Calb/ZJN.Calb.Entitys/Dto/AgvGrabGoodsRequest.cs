using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;

namespace ZJN.Calb.Entitys.Dto
{
    /// <summary>
    /// 取料放料接口（立库）请求
    /// </summary>
    public class AgvGrabGoodsRequest1 : AttributeExt
    {
        [Required()]
        [DisplayName("取放料指令")]
        public string taskCode { get; set; }

        [Required()]
        [DisplayName("取放类型")]
        public string xType { get; set; }

        [Required()]
        [DisplayName("托盘号")]
        public string container { get; set; }

        [Required()]
        [DisplayName("物料编号")]
        public string materialCode { get; set; }

        [Required()]
        [DisplayName("批次号")]
        public string lot { get; set; }


        [DisplayName("库存数量")]
        public string stockNumber { get; set; }

        [Required()]
        [DisplayName("实取/放数量")]
        public string outNumber { get; set; }

        [Required()]
        [DisplayName("取/放货位")]
        public string location { get; set; }


        [DisplayName("生产时间")]
        public string productionTime { get; set; }

        [DisplayName("物料状态")]
        public string qcStatus { get; set; }

        [DisplayName("标签条码")]
        public string polarCode { get; set; }
    }



    public class AgvGrabGoodsRequest
    {
        public string wareHouseNo { get; set; }
        public string operationType { get; set; }
        public string materialCode { get; set; }
        public string batchNo { get; set; }
        public string locNo { get; set; }
        public string isEmptyReturn { get; set; }
        public List<AgvGrabGoodsList> diskInfoList { get; set; }
    }

    public class AgvGrabGoodsList
    {
        public string trayBarcode { get; set; }
        public string barcode { get; set; }
        public string inspectionResult { get; set; }
        public string cProductSign { get; set; }
        public string cProductOrder { get; set; }
        public string cBatterGradeNo { get; set; }
        public string positionNo { get; set; }
        public string cCapacity { get; set; }
        public string cVoltageOcv1 { get; set; }
        public string cVoltageOcv2 { get; set; }
        public string cResistanceAc { get; set; }
        public string cResistanceDc { get; set; }
        public string cKValue { get; set; }
        public string productDate { get; set; }
        public string productLineNo { get; set; }
        public string cOcv2Date { get; set; }
        public string prodline { get; set; }
        public string qty { get; set; }
    }


}
