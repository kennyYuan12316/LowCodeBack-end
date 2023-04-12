using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using System.Collections.Generic;

namespace HSZ.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：代码生成表单全部控件配置
    /// </summary>
    public class CodeGenFormAllControlsDesign
    {
        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 首字母小写控件
        /// </summary>
        public string LowerName => string.IsNullOrWhiteSpace(Name)
                                      ? null
                                      : Name.Substring(0, 1).ToLower() + Name[1..];

        /// <summary>
        /// 原名称
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// hszKey
        /// </summary>
        public string hszKey { get; set; }

        /// <summary>
        /// 控件宽度
        /// </summary>
        public int Span { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Gutter { get; set; }

        /// <summary>
        /// 是否显示子表标题
        /// </summary>
        public bool ShowTitle { get; set; }

        /// <summary>
        /// 标题名
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 子表名称
        /// </summary>
        public string ChildTableName { get; set; }

        /// <summary>
        /// 首字母小写列名
        /// </summary>
        public string LowerChildTableName => string.IsNullOrWhiteSpace(ChildTableName)
                                      ? null
                                      : ChildTableName.Substring(0, 1).ToLower() + ChildTableName[1..];


        /// <summary>
        /// 样式
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// 占位提示
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// 是否可清除
        /// </summary>
        public string Clearable { get; set; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public string Readonly { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public string Required { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public string IsRequired => string.Format(":required='requiredList.{0}'", LowerName);

        /// <summary>
        /// 是否禁用
        /// </summary>
        public string Disabled { get; set; }

        /// <summary>
        /// PC端表单权限
        /// </summary>
        public string IsDisabled { get; set; }

        /// <summary>
        /// 是否显示输入字数统计
        /// </summary>
        public string ShowWordLimit { get; set; }

        /// <summary>
        /// 显示绑定值的格式
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 实际绑定值的格式
        /// </summary>
        public string ValueFormat { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 自适应内容高度
        /// </summary>
        public string AutoSize { get; set; }

        /// <summary>
        /// 是否多选
        /// </summary>
        public string Multiple { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 选项配置
        /// </summary>
        public PropsBeanModel Props { get; set; }

        /// <summary>
        /// 控件名
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 设置阴影显示时机
        /// </summary>
        public string Shadow { get; set; }

        /// <summary>
        /// 文案的位置
        /// </summary>
        public string Contentposition { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// 分组标题的内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 文本样式
        /// </summary>
        public object TextStyle { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 是否为时间范围选择，仅对<el-time-picker>有效
        /// </summary>
        public string IsRange { get; set; }

        /// <summary>
        /// 选项样式
        /// </summary>
        public string OptionType { get; set; }

        /// <summary>
        /// 前图标
        /// </summary>
        public string PrefixIcon { get; set; }

        /// <summary>
        /// 后图标
        /// </summary>
        public string SuffixIcon { get; set; }

        /// <summary>
        /// 最大长度
        /// </summary>
        public string MaxLength { get; set; }

        /// <summary>
        /// 计数器步长
        /// </summary>
        public string Step { get; set; }

        /// <summary>
        /// 是否只能输入 step 的倍数
        /// </summary>
        public string StepStrictly { get; set; }

        /// <summary>
        /// 控制按钮位置
        /// </summary>
        public string ControlsPosition { get; set; }

        /// <summary>
        /// 是否显示中文大写
        /// </summary>
        public string ShowChinese { get; set; }

        /// <summary>
        /// 是否显示密码
        /// </summary>
        public string ShowPassword { get; set; }

        /// <summary>
        /// 是否可搜索
        /// </summary>
        public string Filterable { get; set; }

        /// <summary>
        /// 输入框中是否显示选中值的完整路径
        /// </summary>
        public string ShowAllLevels { get; set; }

        /// <summary>
        /// 选项分隔符
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// 选择范围时的分隔符
        /// </summary>
        public string RangeSeparator { get; set; }

        /// <summary>
        /// 范围选择时开始日期/时间的占位内容
        /// </summary>
        public string StartPlaceholder { get; set; }

        /// <summary>
        /// 范围选择时结束日期/时间的占位内容
        /// </summary>
        public string EndPlaceholder { get; set; }

        /// <summary>
        /// 当前时间日期选择器特有的选项
        /// </summary>
        public string PickerOptions { get; set; }

        /// <summary>
        /// 配置选项
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public string Max { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public string Min { get; set; }

        /// <summary>
        /// 是否允许半选
        /// </summary>
        public string AllowHalf { get; set; }

        /// <summary>
        /// 是否显示子表标题
        /// </summary>
        public bool ShowText { get; set; }

        /// <summary>
        /// 是否显示文本
        /// </summary>
        public string ShowTexts { get; set; }

        /// <summary>
        /// 是否显示分数
        /// </summary>
        public string ShowScore { get; set; }

        /// <summary>
        /// 是否支持透明度选择
        /// </summary>
        public string ShowAlpha { get; set; }

        /// <summary>
        /// 颜色的格式
        /// </summary>
        public string ColorFormat { get; set; }

        /// <summary>
        /// switch 打开时的文字描述
        /// </summary>
        public string ActiveText { get; set; }

        /// <summary>
        /// switch 关闭时的文字描述
        /// </summary>
        public string InactiveText { get; set; }

        /// <summary>
        /// switch 打开时的背景色
        /// </summary>
        public string ActiveColor { get; set; }

        /// <summary>
        /// switch 关闭时的背景色
        /// </summary>
        public string InactiveColor { get; set; }

        /// <summary>
        /// switch 打开时的值
        /// </summary>
        public string IsSwitch { get; set; }

        /// <summary>
        /// 是否显示间断点
        /// </summary>
        public string ShowStops { get; set; }

        /// <summary>
        /// 是否为范围选择
        /// 滑块
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        /// 可接受上传类型
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// 是否显示上传提示
        /// </summary>
        public string ShowTip { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// 文件大小单位
        /// </summary>
        public string SizeUnit { get; set; }

        /// <summary>
        /// 最大上传个数
        /// </summary>
        public string Limit { get; set; }

        /// <summary>
        /// 上传按钮文本
        /// </summary>
        public string ButtonText { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 动作文本
        /// </summary>
        public string ActionText { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public string NoShow { get; set; }

        /// <summary>
        /// v-model
        /// </summary>
        public string vModel { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Prepend { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Append { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Accordion { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Active { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string MainProps { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string TabPosition { get; set; }

        /// <summary>
        /// App max属性
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 列宽度
        /// </summary>
        public string ColumnWidth { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// 远端接口ID
        /// </summary>
        public string InterfaceId { get; set; }

        /// <summary>
        /// 显示字段
        /// </summary>
        public string RelationField { get; set; }

        /// <summary>
        /// 存储字段
        /// </summary>
        public string PropsValue { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ColumnOptions { get; set; }

        /// <summary>
        /// 是否分页
        /// </summary>
        public string HasPage { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        public string PageSize { get; set; }

        /// <summary>
        /// 精度
        /// </summary>
        public string Precision { get; set; }

        ///// <summary>
        ///// 开关控件 属性 - 开启展示值
        ///// </summary>
        //public string ActiveTxt { get; set; }

        ///// <summary>
        ///// 开关控件 属性 - 关闭展示值
        ///// </summary>
        //public string InactiveTxt { get; set; }

        /// <summary>
        /// 系统控件 - 所属组织 属性 - 显示内容
        /// all ：显示组织， last ： 显示部门
        /// </summary>
        public string ShowLevel { get; set; }

        /// <summary>
        /// 对齐方式
        /// </summary>
        public string Align { get; set; }

        /// <summary>
        /// 边框
        /// </summary>
        public string Border { get; set; }

        /// <summary>
        /// 标题宽度
        /// </summary>
        public string LabelWidth { get; set; }

        /// <summary>
        /// 是否开启合计
        /// </summary>
        public bool ShowSummary { get; set; }

        /// <summary>
        /// 弹窗类型
        /// </summary>
        public string PopupType { get; set; }

        /// <summary>
        /// 弹窗标题
        /// </summary>
        public string PopupTitle { get; set; }

        /// <summary>
        /// 弹窗宽度
        /// </summary>
        public string PopupWidth { get; set; }

        public string ShowField { get; set; }

        public string Field { get; set; }

        /// <summary>
        /// 控件子项
        /// </summary>
        public ICollection<CodeGenFormAllControlsDesign> Children { get; set; }
    }
}
