using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.wms.Entitys.Dto.ZjnBillsHistory
{
    public class ZjnBillsHistoryOutbound
    {

        /// <summary>
        /// 流水号
        /// </summary>
        public string orderNo { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public string orderType { get; set; }
        /// <summary>
        /// 物料id
        /// </summary>
        public string productsNo { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string productsName { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        public string productsType { get; set; }

        /// <summary>
        /// 物料规格
        /// </summary>
        public string productsStyle { get; set; }

        /// <summary>
        /// 物料数量
        /// </summary>
        public decimal productsTotal { get; set; }

        /// <summary>
        /// 物料单位
        /// </summary>
        public string productsUnit { get; set; }

        /// <summary>
        /// 物料等级
        /// </summary>
        public string productsGrade { get; set; }

        /// <summary>
        /// 物料批次
        /// </summary>
        public string productsBach { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string productsUser { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string productsSupplier { get; set; }

        /// <summary>
        /// 物料货位
        /// </summary>
        public string productsLocation { get; set; }
        /// <summary>
        /// 托盘号
        /// </summary>
        public string case3 { get; set; }
        /// <summary>
        /// 出库数量
        /// </summary>
        public decimal theDelivery { get; set; }
    }
}
