using System.Collections.Generic;

namespace HSZ.VisualDev.Entitys.Dto.VisualDevModelData
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：在线功能开发数据创建输入
    /// </summary>
    public class VisualDevModelDataCrInput
    {
        /// <summary>
        /// 数据
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// 1-保存
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 候选人
        /// </summary>
        public Dictionary<string, List<string>> candidateList { get; set; }

    }
}
