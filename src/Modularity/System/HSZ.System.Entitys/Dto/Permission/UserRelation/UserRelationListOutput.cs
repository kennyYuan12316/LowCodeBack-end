using HSZ.Dependency;
using Newtonsoft.Json;

namespace HSZ.System.Entitys.Dto.Permission.UserRelation
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用户关系列表
    /// </summary>
    [SuppressSniffer]
    public class UserRelationListOutput
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 获取或设置 启用标识
        /// </summary>
        [JsonIgnore]
        public int enabledMark { get; set; }

        /// <summary>
        /// 获取或设置 删除标志
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [JsonIgnore]
        public long sortCode { get; set; }
    }
}
