namespace HSZ.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：代码生成常规Index列表页面头部按钮配置
    /// </summary>
    public class CodeGenConvIndexListTopButtonDesign
    {
        /// <summary>
        /// 按钮类型
        /// </summary>
        public string @Type { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 按钮值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 按钮文本
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public string Disabled { get; set; }

        /// <summary>
        /// 是否详情
        /// </summary>
        public bool IsDetail { get; set; }
    }
}