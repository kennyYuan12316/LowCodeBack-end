namespace HSZ.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：配置属性模型
    /// </summary>
    public class PropsBeanModel
    {
        /// <summary>
        /// 是否多选
        /// </summary>
        public bool multiple { get; set; }

        /// <summary>
        /// 指定选项标签为选项对象的某个属性值
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 指定选项的值为选项对象的某个属性值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 指定选项的子选项为选项对象的某个属性值
        /// </summary>
        public string children { get; set; }
    }
}
