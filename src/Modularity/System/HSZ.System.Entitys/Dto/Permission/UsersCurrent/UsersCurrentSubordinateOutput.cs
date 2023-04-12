using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.Permission.UsersCurrent
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：个人资料下属输出
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentSubordinateOutput
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string avatar { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string department { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 是否有下级
        /// </summary>
        public bool isLeaf { get; set; }
    }
}
