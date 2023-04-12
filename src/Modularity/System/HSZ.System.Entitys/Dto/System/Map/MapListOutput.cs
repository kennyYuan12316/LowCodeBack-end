using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.System.Map
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：地图列表
    /// </summary>
    [SuppressSniffer]
    public class MapListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 地图名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 地图编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// 添加者
        /// </summary>
        public string creatorUser { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        ///修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
    }
}
