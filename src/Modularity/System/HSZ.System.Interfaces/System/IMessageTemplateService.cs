using HSZ.System.Entitys.System;
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
    public interface IMessageTemplateService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MessageTemplateEntity> GetInfo(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeList"></param>
        /// <param name="messageTemplateEntity"></param>
        /// <param name="userList"></param>
        /// <param name="parameters"></param>
        /// <param name="bodyDic"></param>
        /// <returns></returns>
        Task SendNodeMessage(List<string> typeList, MessageTemplateEntity messageTemplateEntity, List<string> userList, Dictionary<string, string> parameters, Dictionary<string, object> bodyDic);
    }
}