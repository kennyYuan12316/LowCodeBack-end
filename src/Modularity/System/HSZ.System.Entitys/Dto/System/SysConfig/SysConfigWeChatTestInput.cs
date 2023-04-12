using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.System.SysConfig
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：测试企业号连接输入
    /// </summary>
    [SuppressSniffer]
    public class SysConfigWeChatTestInput
    {
        /// <summary>
        /// 应用凭证
        /// </summary>
        public string qyhAgentId { get; set; }

        /// <summary>
        /// 凭证密钥
        /// </summary>
        public string qyhAgentSecret { get; set; }

        /// <summary>
        /// 企业号Id
        /// </summary>
        public string qyhCorpId { get; set; }

        /// <summary>
        /// 同步密钥
        /// </summary>
        public string qyhCorpSecret { get; set; }
    }
}
