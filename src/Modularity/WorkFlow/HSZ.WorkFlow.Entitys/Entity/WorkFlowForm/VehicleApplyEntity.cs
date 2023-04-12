using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;

namespace HSZ.WorkFlow.Entitys
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：车辆申请
    /// </summary>
    [SugarTable("ZJN_WFORM_VEHICLE_APPLY")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VehicleApplyEntity : EntityBase<string>
    {
        /// <summary>
        /// 流程主键
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWTITLE")]
        public string FlowTitle { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWURGENT")]
        public int? FlowUrgent { get; set; }
        /// <summary>
        /// 流程单据
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLNO")]
        public string BillNo { get; set; }
        /// <summary>
        /// 用车人
        /// </summary>
        [SugarColumn(ColumnName = "F_CARMAN")]
        public string CarMan { get; set; }
        /// <summary>
        /// 所在部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENT")]
        public string Department { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [SugarColumn(ColumnName = "F_PLATENUM")]
        public string PlateNum { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTDATE")]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ENDDATE")]
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 目的地
        /// </summary>
        [SugarColumn(ColumnName = "F_DESTINATION")]
        public string Destination { get; set; }
        /// <summary>
        /// 路费金额
        /// </summary>
        [SugarColumn(ColumnName = "F_ROADFEE")]
        public decimal? RoadFee { get; set; }
        /// <summary>
        /// 公里数
        /// </summary>
        [SugarColumn(ColumnName = "F_KILOMETRENUM")]
        public string KilometreNum { get; set; }
        /// <summary>
        /// 随行人数
        /// </summary>
        [SugarColumn(ColumnName = "F_ENTOURAGE")]
        public string Entourage { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
