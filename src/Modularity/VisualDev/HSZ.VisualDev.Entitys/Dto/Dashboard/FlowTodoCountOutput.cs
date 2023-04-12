namespace HSZ.VisualDev.Entitys.Dto.Dashboard
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：我的待办输出实体类
    /// </summary>
    public class FlowTodoCountOutput
    {
        /// <summary>
        /// 待我审核
        /// </summary>
        public int toBeReviewed { get; set; }

        /// <summary>
        /// 流程委托
        /// </summary>
        public int entrust { get; set; }

        /// <summary>
        /// 已办事宜
        /// </summary>
        public int flowDone { get; set; }
    }
}
