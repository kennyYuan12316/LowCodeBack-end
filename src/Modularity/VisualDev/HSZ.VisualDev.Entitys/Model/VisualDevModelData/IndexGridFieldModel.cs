namespace HSZ.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：显示列模型
    /// </summary>
    public class IndexGridFieldModel
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string prop { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 对齐
        /// </summary>
        public string align { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public string width { get; set; }

        /// <summary>
        /// 控件KEY
        /// </summary>
        public string hszKey { get; set; }

        /// <summary>
        /// 是否排序
        /// </summary>
        public bool sortable { get; set; }
    }
}
