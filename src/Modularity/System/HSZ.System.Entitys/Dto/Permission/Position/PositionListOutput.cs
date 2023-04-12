using HSZ.Common.Util;
using HSZ.Dependency;
using System;
using System.Text.Json.Serialization;

namespace HSZ.System.Entitys.Dto.Permission.Position
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：岗位列表输出
    /// </summary>
    [SuppressSniffer]
    public class PositionListOutput : TreeModel
    {

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 岗位编号
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 岗位类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string department { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 组织Id
        /// </summary>
        public string organizeId { get; set; }
    }
}
