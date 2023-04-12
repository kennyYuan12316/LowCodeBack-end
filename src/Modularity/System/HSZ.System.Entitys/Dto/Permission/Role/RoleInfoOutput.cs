using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.Permission.Role
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：角色信息输出
    /// </summary>
    [SuppressSniffer]
    public class RoleInfoOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 有效标记
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 组织Id集合
        /// </summary>
        public List<List<string>> organizeIdsTree { get; set; }

        /// <summary>
        /// 全局标识 1:全局 0 组织
        /// </summary>
        public int globalMark { get; set; }
    }
}
