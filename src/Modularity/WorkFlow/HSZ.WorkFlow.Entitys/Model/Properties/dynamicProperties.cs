using HSZ.Dependency;
using System.Collections.Generic;

namespace HSZ.WorkFlow.Entitys.Model.Properties
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class DynamicProperties
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 指定路径ID
        /// </summary>
        public string approverPath { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public int? taskType { get; set; }



        /// <summary>
        /// 绑定接驳台
        /// </summary>
        public string[] bindEndSite { get; set; }




    }

   
  


  
}
