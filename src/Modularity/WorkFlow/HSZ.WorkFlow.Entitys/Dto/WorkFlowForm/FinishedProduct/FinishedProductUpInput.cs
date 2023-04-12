using HSZ.Dependency;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.FinishedProduct
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class FinishedProductUpInput : FinishedProductCrInput
    {
        public string id { get; set; }
    }
}
