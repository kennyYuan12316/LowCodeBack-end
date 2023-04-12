using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using System.Collections.Generic;

namespace HSZ.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：代码生成Index查询列设计
    /// </summary>
    public class CodeGenConvIndexSearchColumnDesign
    {
        /// <summary>
        /// 真实名字
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 首字母小写列名
        /// </summary>
        public string LowerName { get; set; }

        /// <summary>
        /// 控件名
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 可清除的
        /// </summary>
        public string Clearable { get; set; }

        /// <summary>
        /// 时间格式化
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string @Type { get; set; }

        /// <summary>
        /// 时间输出类型
        /// </summary>
        public string ValueFormat { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 标题名
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 查询控件Key
        /// </summary>
        public string QueryControlsKey { get; set; }

        /// <summary>
        /// 选项配置
        /// </summary>
        public PropsBeanModel Props { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 输入框中是否显示选中值的完整路径
        /// </summary>
        public string ShowAllLevels { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
    }
}