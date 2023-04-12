using System.Collections.Generic;

namespace HSZ.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：代码生成前端配置模型
    /// </summary>
    public class CodeGenFrontendConfigModel
    {
        /// <summary>
        /// 表单ref
        /// </summary>
        public string FormRef { get; set; }

        /// <summary>
        /// 表单Model
        /// </summary>
        public string FromModel { get; set; }

        /// <summary>
        /// 表单宽度
        /// </summary>
        public string LabelWidth { get; set; }

        /// <summary>
        /// 表单位置
        /// </summary>
        public string LabelPosition { get; set; }
    }
}