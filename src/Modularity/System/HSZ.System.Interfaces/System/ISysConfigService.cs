using HSZ.System.Entitys.Dto.System.SysConfig;
using HSZ.System.Entitys.System;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：系统配置
    /// </summary>
    public interface ISysConfigService
    {
        /// <summary>
        /// 系统配置信息
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<SysConfigEntity> GetInfo(string category, string key);

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <returns></returns>
        Task<SysConfigOutput> GetInfo();
    }
}
