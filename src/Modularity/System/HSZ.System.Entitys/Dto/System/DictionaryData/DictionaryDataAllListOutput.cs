using HSZ.Dependency;
using System;

namespace HSZ.System.Entitys.Dto.System.DictionaryData
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DictionaryDataAllListOutput
    {
        /// <summary>
        /// 字典分类id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 是否树形
        /// </summary>
        public int isTree { get; set; }
        /// <summary>
        /// 字典分类编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        ///字典分类名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Object dictionaryList { get; set; }
    }
}
