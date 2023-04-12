using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 主任务
    /// </summary>
    [SugarTable("zjn_wms_task")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsTaskEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 任务号
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskNo")]
        public string TaskNo { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskName")]
        public string TaskName { get; set; }

        /// <summary>
        /// 起点工位
        /// </summary>
        [SugarColumn(ColumnName = "F_PositionFrom")]
        public string PositionFrom { get; set; }

        /// <summary>
        /// 终点工位
        /// </summary>
        [SugarColumn(ColumnName = "F_PositionTo")]
        public string PositionTo { get; set; }

        /// <summary>
        /// 当前工位
        /// </summary>
        [SugarColumn(ColumnName = "F_PositionCurrent")]
        public string PositionCurrent { get; set; }

        /// <summary>
        /// 路径编号
        /// </summary>
        [SugarColumn(ColumnName = "F_RouteNo")]
        public string RouteNo { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyUserId")]
        public string LastModifyUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyTime")]
        public DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]
        public int? EnabledMark { get; set; }

        /// <summary>
        /// 任务来源
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskFrom")]
        public string TaskFrom { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskState")]
        public int? TaskState { get; set; }

        /// <summary>
        /// 业务类型编号
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskTypeNum")]
        public string TaskTypeNum { get; set; }

        /// <summary>
        /// 任务状态描述
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskDescribe")]
        public string TaskDescribe { get; set; }

        /// <summary>
        /// 单据指令
        /// </summary>
        [SugarColumn(ColumnName = "F_OrderNo")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [SugarColumn(ColumnName = "F_MaterialCode")]
        public string MaterialCode { get; set; }


        /// <summary>
        /// 数量
        /// </summary>
        [SugarColumn(ColumnName = "F_Quantity")]
        public decimal? Quantity { get; set; }


        /// <summary>
        /// 单据号
        /// </summary>
        [SugarColumn(ColumnName = "F_BillNo")]
        public string BillNo { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [SugarColumn(ColumnName = "F_Priority")]
        public int? Priority { get; set; }

        /// <summary>
        /// 托盘编号
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayNo")]
        public string TrayNo { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        [SugarColumn(ColumnName = "F_BatchNo")]
        public string BatchNo { get; set; }

    }
}