using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.Map
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：地图明细
    /// </summary>
    [SuppressSniffer]
    public class MapInfoOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 地图编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 地图名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 状态(1-可用,0-禁用)
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 地图数据
        /// </summary>
        public string data { get; set; }
    }
}
