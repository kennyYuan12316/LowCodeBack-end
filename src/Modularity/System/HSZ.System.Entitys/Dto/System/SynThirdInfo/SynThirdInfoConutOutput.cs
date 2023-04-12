using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.System.SynThirdInfo
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class SynThirdInfoConutOutput
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int recordTotal { get; set; }

        /// <summary>
        /// 同步时间
        /// </summary>
        public DateTime? synDate { get; set; }

        /// <summary>
        /// 失败条数
        /// </summary>
        public int synFailCount { get; set; }

        /// <summary>
        /// 成功条数
        /// </summary>
        public int synSuccessCount { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string synType { get; set; }

        /// <summary>
        /// 未同步条数
        /// </summary>
        public int unSynCount { get; set; }
    }
}
