using HSZ.Dependency;

namespace HSZ.WorkFlow.Entitys.Model.Properties
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：定时器
    /// </summary>
    [SuppressSniffer]
    public class TimerProperties
    {

        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int day { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int hour { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int minute { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int second { get; set; }
        /// <summary>
        /// 定时器节点的上一节点编码
        /// </summary>
        public string upNodeCode { get; set; }
    }
}
