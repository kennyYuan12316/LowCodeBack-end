using HSZ.Dependency;
using System;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.LeaveApply
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class LeaveApplyInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? applyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyDept { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyPost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string flowTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leaveDayCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? leaveEndTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leaveHour { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leaveReason { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? leaveStartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string leaveType { get; set; }
    }
}
