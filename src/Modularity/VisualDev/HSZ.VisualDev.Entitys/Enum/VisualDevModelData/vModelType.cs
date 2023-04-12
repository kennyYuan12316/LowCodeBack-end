using System.ComponentModel;

namespace HSZ.VisualDev.Entitys.Enum.VisualDevModelData
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    public enum vModelType
    {
        /// <summary>
        /// 数据字典DataType
        /// </summary>
        [Description("dictionary")]
        DICTIONARY,
        /// <summary>
        /// 静态数据DataType
        /// </summary>
        [Description("static")]
        STATIC,
        /// <summary>
        /// 查询字段数据
        /// </summary>
        [Description("keyJsonMap")]
        KEYJSONMAP,
        /// <summary>
        /// 级联选择静态模板值
        /// </summary>
        [Description("value")]
        VALUE,
        /// <summary>
        /// 远程数据DataType
        /// </summary>
        [Description("dynamic")]
        DYNAMIC,
        /// <summary>
        /// 远程数据
        /// </summary>
        [Description("timeControl")]
        TIMECONTROL,
        /// <summary>
        /// 可视化数据列表结果key
        /// </summary>
        [Description("list")]
        LIST
    }
}
