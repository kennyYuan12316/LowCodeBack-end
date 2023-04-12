using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTask
{
    /// <summary>
    /// 主任务输入参数
    /// </summary>
    public class ZjnWmsTaskListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 任务号
        /// </summary>
        public string F_TaskNo { get; set; }
        
        /// <summary>
        /// 任务名称
        /// </summary>
        public string F_TaskName { get; set; }
        
        /// <summary>
        /// 起点工位
        /// </summary>
        public string F_PositionFrom { get; set; }

        /// <summary>
        /// 起点工位名称
        /// </summary>
        public string F_PositionFromName { get; set; }

        /// <summary>
        /// 终点工位
        /// </summary>
        public string F_PositionTo { get; set; }

        /// <summary>
        /// 终点工位名称
        /// </summary>
        public string F_PositionToName { get; set; }

        /// <summary>
        /// 当前工位
        /// </summary>
        public string F_PositionCurrent { get; set; }
        
        /// <summary>
        /// 路径编号
        /// </summary>
        public string F_RouteNo { get; set; }
        
        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        public string F_LastModifyUserId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime? F_LastModifyTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? F_EnabledMark { get; set; }
        
        /// <summary>
        /// 任务来源
        /// </summary>
        public string F_TaskFrom { get; set; }
        
        /// <summary>
        /// 任务状态
        /// </summary>
        public int? F_TaskState { get; set; }
        
        /// <summary>
        /// 业务类型编号
        /// </summary>
        public string F_TaskTypeNum { get; set; }

        /// <summary>
        /// 业务类型名称
        /// </summary>
        public string F_TaskTypeName { get; set; }

        /// <summary>
        /// 任务状态描述
        /// </summary>
        public string F_TaskDescribe { get; set; }


        /// <summary>
        /// 单据指令
        /// </summary>
        public string F_OrderNo { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string F_MaterialCode { get; set; }


        /// <summary>
        /// 数量
        /// </summary>
        public decimal? F_Quantity { get; set; }


        /// <summary>
        /// 单据号
        /// </summary>
        public string F_BillNo { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? F_Priority { get; set; }

        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }

    }
}