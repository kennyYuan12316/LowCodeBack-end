using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.System.OnlineUser
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：在线用户批量下线输入
    /// </summary>
    [SuppressSniffer]
    public class BatchOnlineInput
    {
        /// <summary>
        /// 删除id 列表
        /// </summary>
        public List<string> ids { get; set; }
    }
}
