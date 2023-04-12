using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;

namespace HSZ.System.Entitys.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：第三方工具对象同步表
    /// </summary>
    [SugarTable("ZJN_BASE_SYN_THIRD_INFO")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class SynThirdInfoEntity : CLEntityBase
    {
        /// <summary>
        /// 第三方类型(1:企业微信;2:钉钉)
        /// </summary>
        [SugarColumn(ColumnName = "F_THIRDTYPE")]
        public int? ThirdType { get; set; }

        /// <summary>
        /// 数据类型(1:公司;2:部门;3:用户)
        /// </summary>
        [SugarColumn(ColumnName = "F_DATATYPE")]
        public int? DataType { get; set; }

        /// <summary>
        /// 系统对象ID
        /// </summary>
        [SugarColumn(ColumnName = "F_SYSOBJID")]
        public string SysObjId { get; set; }

        /// <summary>
        /// 第三对象ID
        /// </summary>
        [SugarColumn(ColumnName = "F_THIRDOBJID")]
        public string ThirdObjId { get; set; }

        /// <summary>
        /// 0:未同步;1:同步成功;2:同步失败
        /// </summary>
        [SugarColumn(ColumnName = "F_SYNSTATE")]
        public string SynState { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
