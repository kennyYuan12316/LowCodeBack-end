using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.PrintDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class PrintDevCrInput
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 类型（1：流程表单，2：功能表单）
        /// </summary>
        public int? type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 数据连接id
        /// </summary>
        public string dbLinkId { get; set; }

        /// <summary>
        /// sql模板
        /// </summary>
        public string sqlTemplate { get; set; }

        /// <summary>
        /// 左侧字段
        /// </summary>
        public string leftFields { get; set; }

        /// <summary>
        /// 打印模板
        /// </summary>
        public string printTemplate { get; set; }
    }
}
