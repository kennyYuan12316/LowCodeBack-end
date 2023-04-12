using HSZ.Dependency;
using System.Text.Json.Serialization;

namespace HSZ.System.Entitys.Dto.Permission.User
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：用户全列表
    /// </summary>
    [SuppressSniffer]
    public class UserListAllOutput
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string realName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string headIcon { get; set; }

        /// <summary>
        /// 用户性别
        /// </summary>
        public int? gender { get; set; }

        /// <summary>
        /// 用户部门
        /// </summary>
        public string department { get; set; }

        /// <summary>
        /// 用户排序
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 快速查询
        /// </summary>
        public string quickQuery { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [JsonIgnore]
        public int? enabledMark { get; set; }

        /// <summary>
        /// 删除状态
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }

    }
}
