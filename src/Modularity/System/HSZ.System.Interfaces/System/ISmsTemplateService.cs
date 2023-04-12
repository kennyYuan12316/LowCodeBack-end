using HSZ.System.Entitys.Dto.System.SysConfig;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.System.Interfaces.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    public interface ISmsTemplateService
    {
        /// <summary>
        /// 获取短信模板字段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<string>> GetSmsTemplateFields(string id);

        /// <summary>
        /// 工作流发送短信
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sysconfig"></param>
        /// <param name="phoneNumbers"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task FlowTaskSend(string id, SysConfigOutput sysconfig, List<string> phoneNumbers, Dictionary<string, string> parameters);
    }
}