namespace HSZ.VisualDev.Entitys.Dto.VisualDevModelData
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：可视化开发模型数据配置输出
    /// </summary>
    public class VisualDevModelDataConfigOutput
    {
        /// <summary>
        /// 表单JSON包
        /// </summary>
        public string formData { get; set; }

        /// <summary>
        /// 列表JSON包
        /// </summary>
        public string columnData { get; set; }

        /// <summary>
        /// 工作流编码
        /// </summary>
        public string flowEnCode { get; set; }

        /// <summary>
        /// 工作流引擎ID
        /// </summary>
        public string flowId { get; set; }

        /// <summary>
        /// 工作流模板JSON
        /// </summary>
        public string flowTemplateJson { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）
        /// </summary>
        public string webType { get; set; }
    }
}
