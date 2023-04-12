using HSZ.Dependency;
using HSZ.System.Entitys.Model.System.PrintDev;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.System.PrintDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class PrintDevDataOutput
    {
        /// <summary>
        /// sql数据
        /// </summary>
        public object printData { get; set; }

        /// <summary>
        /// 模板数据
        /// </summary>
        public string printTemplate { get; set; }

        /// <summary>
        /// 审批数据
        /// </summary>
        public List<PrintDevDataModel> flowTaskOperatorRecordList { get; set; } = new List<PrintDevDataModel>();
    }
}
