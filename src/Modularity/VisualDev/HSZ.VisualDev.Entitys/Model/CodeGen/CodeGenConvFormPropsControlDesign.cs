using HSZ.VisualDev.Entitys.Model.VisualDevModelData;

namespace HSZ.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：代码生成常规表单选项配置控件配置
    /// </summary>
    public class CodeGenConvFormPropsControlDesign
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 首字母小写列名
        /// </summary>
        public string LowerName => string.IsNullOrWhiteSpace(Name)
                                      ? null
                                      : Name.Substring(0, 1).ToLower() + Name[1..];

        /// <summary>
        /// 选项配置
        /// </summary>
        public PropsBeanModel Props { get; set; }
    }
}