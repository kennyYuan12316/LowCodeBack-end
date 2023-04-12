using HSZ.Dependency;
using System;

namespace HSZ.WorkFlow.Entitys.Dto.WorkFlowForm.TravelReimbursement
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：
    /// </summary>
    [SuppressSniffer]
    public class TravelReimbursementInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? applyDate { get; set; }
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
        public string billsNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? breakdownFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string businessMission { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string departmental { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string destination { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? fare { get; set; }
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
        public decimal? getAccommodation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? loanAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? mealAllowance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? other { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? parkingRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? planeTicket { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? railTransit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? reimbursementAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reimbursementId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? returnDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? roadFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? setOutDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? sumOfMoney { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? travelAllowance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string travelerUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? vehicleMileage { get; set; }
    }
}
