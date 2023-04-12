using System.Collections.Generic;

namespace HSZ.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：插槽模型
    /// </summary>
    public class SlotModel
    {
        /// <summary>
        /// 前
        /// </summary>
        public string prepend { get; set; }

        /// <summary>
        /// 后
        /// </summary>
        public string append { get; set; }

        /// <summary>
        /// 默认名称
        /// </summary>
        public string defaultName { get; set; }

        /// <summary>
        /// 配置项
        /// </summary>
        public List<Dictionary<string, object>> options { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string appOptions { get; set; }

        /// <summary>
        /// 默认
        /// </summary>
        public string @default { get; set; }
    }
}
