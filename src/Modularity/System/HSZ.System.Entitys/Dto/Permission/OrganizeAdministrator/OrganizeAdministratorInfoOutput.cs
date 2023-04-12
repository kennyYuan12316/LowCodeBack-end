using HSZ.Dependency;

namespace HSZ.System.Entitys.Dto.Permission.OrganizeAdministrator
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：机构分级管理输出
    /// </summary>
    [SuppressSniffer]
    public class OrganizeAdministratorInfoOutput
    {
        /// <summary>
        /// 自然主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 用户主键
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 机构主键
        /// </summary>
        public string organizeId { get; set; }

        /// <summary>
        /// 机构类型
        /// </summary>
        public string organizeType { get; set; }

        /// <summary>
        /// 本层添加
        /// </summary>
        public int thisLayerAdd { get; set; }

        /// <summary>
        /// 本层编辑
        /// </summary>
        public int thisLayerEdit { get; set; }

        /// <summary>
        /// 本层删除
        /// </summary>
        public int thisLayerDelete { get; set; }

        /// <summary>
        /// 子层添加
        /// </summary>
        public int subLayerAdd { get; set; }

        /// <summary>
        /// 子层编辑
        /// </summary>
        public int subLayerEdit { get; set; }

        /// <summary>
        /// 子层删除
        /// </summary>
        public int subLayerDelete { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
    }
}
