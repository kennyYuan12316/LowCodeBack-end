using System;

namespace ZJN.Agv.Entitys.Dto.AgvCreateOrder
{
    /// <summary>
    /// 立库下单输入参数
    /// </summary>
    public class ZjnBaseStdCreateorderListOutput
    {
        /// <summary>
        /// F_Id
        /// </summary>
        public string F_Id { get; set; }

        /// <summary>
        /// 业务关系编码
        /// </summary>
        public string F_BrCode { get; set; }

        /// <summary>
        /// 终点库区编码
        /// </summary>
        public string F_EndAreaCode { get; set; }

        /// <summary>
        /// 终点库位编码
        /// </summary>
        public string F_EndLocCode { get; set; }

        /// <summary>
        /// LES订单ID
        /// </summary>
        public string F_LesOrderId { get; set; }

        /// <summary>
        /// 外部订单ID
        /// </summary>
        public string F_OuterOrderId { get; set; }

        /// <summary>
        /// 起点库区编码
        /// </summary>
        public string F_StartAreaCode { get; set; }

        /// <summary>
        /// 起点库位编码
        /// </summary>
        public string F_StartLocCode { get; set; }

        /// <summary>
        /// 托盘编码
        /// </summary>
        public string F_TrayId { get; set; }

    }
}