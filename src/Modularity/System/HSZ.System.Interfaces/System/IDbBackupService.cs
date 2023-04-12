using HSZ.Common.Filter;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据备份
    /// </summary>
    public interface IDbBackupService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        Task<dynamic> GetList(PageInputBase input);
        /// <summary>
        /// 创建备份
        /// </summary>
        /// <returns></returns>
        Task Create();
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task Delete(string id);
    }
}
