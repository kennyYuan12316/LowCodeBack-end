using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;

namespace HSZ.System.Entitys.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据接口日志
    /// </summary>
    [SugarTable("ZJN_BASE_DATA_INTERFACE_LOG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DataInterfaceLogEntity: EntityBase<string>
    {
        /// <summary>
        /// 调用接口id
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKID")]
        public string InvokId { get; set; }

        /// <summary>
        /// 调用时间
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKTIME")]
        public DateTime? InvokTime { get; set; }

        /// <summary>
        /// 调用者
        /// </summary>
        [SugarColumn(ColumnName = "F_USERID")]
        public string UserId { get; set; }

        /// <summary>
        /// 请求ip
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKIP")]
        public string InvokIp { get; set; }

        /// <summary>
        /// 请求设备
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKDEVICE")]
        public string InvokDevice { get; set; }

        /// <summary>
        /// 请求耗时
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKWASTETIME")]
        public int? InvokWasteTime { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOKTYPE")]
        public string InvokType { get; set; }
    }
}
