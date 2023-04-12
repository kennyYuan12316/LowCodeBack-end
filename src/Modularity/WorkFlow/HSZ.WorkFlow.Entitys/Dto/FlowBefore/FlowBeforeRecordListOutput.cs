using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Dto.FlowBefore
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class FlowBeforeRecordListOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FlowBeforeRecordListModel> list { get; set; }
    }
}
