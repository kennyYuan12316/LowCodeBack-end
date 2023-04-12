using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 子任务
    /// </summary>
    [SugarTable("zjn_wms_task_details")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsTaskDetailsEntity
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 子任务id
        /// </summary>
        [SugarColumn(ColumnName = "F_task_details_Id")]
        public string TaskDetailsId { get; set; }


        /// <summary>
        /// 路径id
        /// </summary>
        [SugarColumn(ColumnName = "F_work_PathId")]
        public string WorkPathId { get; set; }

        /// <summary>
        /// 子任务名称
        /// </summary>
        [SugarColumn(ColumnName = "F_task_details_Name")]        
        public string TaskDetailsName { get; set; }
        
        /// <summary>
        /// 主任务id
        /// </summary>
        [SugarColumn(ColumnName = "F_task_Id")]        
        public string TaskId { get; set; }
        
        /// <summary>
        /// 起点工位
        /// </summary>
        [SugarColumn(ColumnName = "F_task_details_Start")]        
        public string TaskDetailsStart { get; set; }
        
        /// <summary>
        /// 终点工位
        /// </summary>
        [SugarColumn(ColumnName = "F_task_details_End")]        
        public string TaskDetailsEnd { get; set; }
        
        /// <summary>
        /// 子任务状态1未执行、2执行中、3完成、4取消
        /// </summary>
        [SugarColumn(ColumnName = "F_task_details_States")]        
        public int? TaskDetailsStates { get; set; }
        
        /// <summary>
        /// 子任务状态描述
        /// </summary>
        [SugarColumn(ColumnName = "F_task_details_Describe")]        
        public string TaskDetailsDescribe { get; set; }
        
        /// <summary>
        /// 创建用户
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
        [SugarColumn(ColumnName = "F_ModifiedUser")]        
        public string ModifiedUser { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_ModifiedTime")]        
        public DateTime? ModifiedTime { get; set; }

        /// <summary>
        /// 托盘号
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayNo")]
        public string TrayNo { get; set; }

        /// <summary>
        /// 开始行
        /// </summary>
        [SugarColumn(ColumnName = "F_Row_Start")]
        public int? RowStart { get; set; }

        /// <summary>
        /// 开始列
        /// </summary>
        [SugarColumn(ColumnName = "F_Cell_Start")]
        public int? CellStart { get; set; }

        /// <summary>
        /// 开始层
        /// </summary>
        [SugarColumn(ColumnName = "F_Layer_Start")]
        public int? LayerStart { get; set; }

        /// <summary>
        /// 结束行
        /// </summary>
        [SugarColumn(ColumnName = "F_Row_End")]
        public int? RowEnd { get; set; }

        /// <summary>
        /// 结束列
        /// </summary>
        [SugarColumn(ColumnName = "F_Cell_End")]
        public int? CellEnd { get; set; }

        /// <summary>
        /// 结束层
        /// </summary>
        [SugarColumn(ColumnName = "F_Layer_End")]
        public int? LayerEnd { get; set; }

        /// <summary>
        /// 当前节点id
        /// </summary>
        [SugarColumn(ColumnName = "F_NodeCode")]
        public string NodeCode { get; set; }

        /// <summary>
        /// 上一个节点id
        /// </summary>
        [SugarColumn(ColumnName = "F_NodeUp")]
        public string NodeUp { get; set; }

        /// <summary>
        /// 下一个节点id
        /// </summary>
        [SugarColumn(ColumnName = "F_NodeNext")]
        public string NodeNext { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        [SugarColumn(ColumnName = "F_NodeType")]
        public string NodeType { get; set; }     

        /// <summary>
        /// json
        /// </summary>
        [SugarColumn(ColumnName = "F_NodePropertyJson")]
        public string NodePropertyJson { get; set; }


        /// <summary>
        /// 返回判断值
        /// </summary>
        [SugarColumn(ColumnName = "F_ResultValue")]
        public string ResultValue { get; set; }


        /// <summary>
        /// 子任务类型
        /// </summary>
        [SugarColumn(ColumnName = "F_TaskType")]
        public int? TaskType { get; set; }

        /// <summary>
        /// 输送设备
        /// </summary>
        [SugarColumn(ColumnName = "F_task_details_Move")]
        public string TaskDetailsMove { get; set; }

        /// <summary>
        /// 产品等级
        /// </summary>
        [SugarColumn(ColumnName = "F_productLevel")]
        public string ProductLevel { get; set; }

    }
}