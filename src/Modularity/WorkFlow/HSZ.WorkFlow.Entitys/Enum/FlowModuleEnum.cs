using HSZ.Dependency;

namespace HSZ.WorkFlow.Entitys.Enum
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：功能流程
    /// </summary>
    [SuppressSniffer]
    public class FlowModuleEnum
    {
        /// <summary>
        /// 订单测试
        /// </summary>
        public static string CRM_Order
        {
            get
            {
                return "crmOrder";
            }
        }
        /// <summary>
        /// CRM应用-合同
        /// </summary>
        public static string CRM_Contract
        {
            get
            {
                return "CRM_Contract";
            }
        }
        /// <summary>
        /// CRM应用-回款
        /// </summary>
        public static string CRM_Receivable
        {
            get
            {
                return "CRM_Receivable";
            }
        }
        /// <summary>
        /// CRM应用-发票
        /// </summary>
        public static string CRM_Invoice
        {
            get
            {
                return "CRM_Invoice";
            }
        }
    }
}
