using HSZ.System.Entitys.Dto.System.SysLog;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：系统日志
    /// </summary>
    public interface ISysLogService
    {
        /// <summary>
        /// 获取系统日志列表（分页）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="Type">分类</param>
        /// <returns></returns>
        Task<dynamic> GetList(LogListQuery input, int Type);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        Task Delete(LogDelInput input);
    }
}
