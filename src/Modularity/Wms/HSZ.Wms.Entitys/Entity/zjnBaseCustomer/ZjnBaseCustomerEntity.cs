using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 客户信息
    /// </summary>
    [SugarTable("zjn_base_customer")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnBaseCustomerEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
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
        /// 客户编号
        /// </summary>
        [SugarColumn(ColumnName = "F_CustomerNo")]        
        public string CustomerNo { get; set; }
        
        /// <summary>
        /// 客户名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CustomerName")]        
        public string CustomerName { get; set; }
        
        /// <summary>
        /// 联系方式
        /// </summary>
        [SugarColumn(ColumnName = "F_CustomerPhone")]        
        public string CustomerPhone { get; set; }
        
        /// <summary>
        /// 联系地址
        /// </summary>
        [SugarColumn(ColumnName = "F_CustomerAddress")]        
        public string CustomerAddress { get; set; }
        
        /// <summary>
        /// 联系人姓名
        /// </summary>
        [SugarColumn(ColumnName = "F_ContactName")]        
        public string ContactName { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_Description")]        
        public string Description { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyUserId")]        
        public string LastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyTime")]        
        public DateTime? LastModifyTime { get; set; }
        
    }
}