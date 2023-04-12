using HSZ.Dependency;
using System.ComponentModel.DataAnnotations;

namespace HSZ.System.Entitys.Dto.System.DbLink
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DbLinkActionsTestInput
    {
        /// <summary>
        /// 连接类型
        /// </summary>
        public string dbType { get; set; }

        /// <summary>
        /// 主机
        /// </summary>
        public string host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public string port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 库名
        /// </summary>
        public string serviceName { get; set; }

        /// <summary>
        /// 模式
        /// </summary>
        public string dbSchema { get; set; }

        /// <summary>
        /// 表空间
        /// </summary>
        public string tableSpace { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
