using HSZ.Dependency;
using Newtonsoft.Json;
using System;

namespace HSZ.System.Entitys.Dto.System.BillRule
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class BillRuleListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 业务编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 当前流水号
        /// </summary>
        public string outputNumber { get; set; }

        /// <summary>
        /// 流水位数
        /// </summary>
        public int? digit { get; set; }

        /// <summary>
        /// 流水起始
        /// </summary>
        public string startNumber { get; set; }

        /// <summary>
        /// 流水状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
    }
}
