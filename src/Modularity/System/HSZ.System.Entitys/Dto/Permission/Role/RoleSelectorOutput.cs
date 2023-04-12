using HSZ.Common.Util;
using HSZ.Dependency;
using Newtonsoft.Json;

namespace HSZ.System.Entitys.Dto.Permission.Role
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：角色下拉输出
    /// </summary>
    [SuppressSniffer]
    public class RoleSelectorOutput : TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }

        /// <summary>
        /// 有效标记
        /// </summary>
        [JsonIgnore]
        public int? enabledMark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [JsonIgnore]
        public long? sortCode { get; set; }
    }
}
