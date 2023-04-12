using HSZ.System.Entitys.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Apps.Interfaces
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：App常用数据
    /// </summary>
    public interface IAppDataService
    {
        /// <summary>
        /// 菜单列表
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleEntity>> GetAppMenuList();

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        Task Delete(string objectId);
    }
}
