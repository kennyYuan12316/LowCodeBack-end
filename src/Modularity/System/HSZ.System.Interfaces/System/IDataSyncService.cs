using HSZ.System.Entitys.Dto.System.DataSync;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据同步
    /// </summary>
    public interface IDataSyncService
    {
        /// <summary>
        /// 执行同步
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        Task Execute(DbSyncActionsExecuteInput input);
    }
}
