using HSZ.Common.Util;
using HSZ.Dependency;
using Newtonsoft.Json;

namespace HSZ.System.Entitys.Dto.Permission.Organize
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：机构成员列表输出
    /// </summary>
    [SuppressSniffer]
    public class OrganizeMemberListOutput : TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 有效标记
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }

        [JsonIgnore]
        public long? SortCode { get; set; }

        [JsonIgnore]
        public string Account { get; set; }

        [JsonIgnore]
        public string RealName { get; set; }

        [JsonIgnore]
        public int? DeleteMark { get; set; }
    }
}
