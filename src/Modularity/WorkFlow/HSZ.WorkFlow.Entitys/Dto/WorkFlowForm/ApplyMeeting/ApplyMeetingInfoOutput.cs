using HSZ.Dependency;
using System;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.ApplyMeeting
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class ApplyMeetingInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string administrator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyMaterial { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string applyUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string attendees { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string billNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string conferenceName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string conferenceRoom { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string conferenceTheme { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string conferenceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? endDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string estimatePeople { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? estimatedAmount { get; set; }
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
        public string lookPeople { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string otherAttendee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? startDate { get; set; }
    }
}
