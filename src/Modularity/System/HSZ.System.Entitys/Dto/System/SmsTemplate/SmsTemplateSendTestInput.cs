using HSZ.Dependency;
using HSZ.System.Entitys.Dto.SmsTemplate;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.System.SmsTemplate
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class SmsTemplateSendTestInput: SmsTemplateCrInput
    {
        /// <summary>
        /// 接收号码
        /// </summary>
        public string phoneNumbers { get; set; }

        public Dictionary<string,string> parameters { get; set; }=new Dictionary<string,string>();
    }
}
