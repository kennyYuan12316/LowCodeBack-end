using HSZ.Extend.Entitys;
using System.Threading.Tasks;

namespace HSZ.Extend.Interfaces
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：邮件收发
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// 门户未读邮件
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetUnreadList();

        /// <summary>
        /// 信息（配置）
        /// </summary>
        /// <returns></returns>
        Task<EmailConfigEntity> GetConfigInfo();
    }
}
