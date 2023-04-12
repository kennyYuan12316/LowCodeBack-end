using HSZ.Common.Filter;

namespace HSZ.VisualDev.Entitys.Dto.VisualDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：在线开发列表查询输入
    /// </summary>
    public class VisualDevListQueryInput : PageInputBase
    {
        /// <summary>
        /// 功能类型
        /// 1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单
        /// </summary>
        public int type { get; set; } = 1;

        /// <summary>
        /// 分类
        /// </summary>
        public string category { get; set; }
    }
}
