using HSZ.Common.Util;
using HSZ.Dependency;
using System;
using System.Text.Json.Serialization;

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
    public class PrintDevListOutput:TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string lastModifyUser { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        /// <summary>
        /// 状态(0-关闭，1-开启)
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// 流程分类(数据字典-工作流-流程分类)
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 流程分类(数据字典-工作流-流程分类)
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int? type { get; set; }
        [JsonIgnore]
        public string description { get; set; }
        [JsonIgnore]
        public string dictionaryTypeId { get; set; }
        [JsonIgnore]
        public int? deleteMark { get; set; }
    }
}
