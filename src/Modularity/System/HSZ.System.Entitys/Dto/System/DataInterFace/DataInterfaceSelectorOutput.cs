using HSZ.Common.Util;
using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.System.Entitys.Dto.System.DataInterFace
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DataInterfaceSelectorOutput : TreeModel
    {
        /// <summary>
        /// 分类id
        /// </summary>
        public string categoryId { get; set; }
        /// <summary>
        /// 接口名
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
    }
}
