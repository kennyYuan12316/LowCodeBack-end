using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 货位分组信息
    /// </summary>
    [SugarTable("zjn_wms_location_group_details")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsLocationGroupDetailsEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 货位分组明细编号
        /// </summary>
        [SugarColumn(ColumnName = "F_GroupDetailsNo")]        
        public string GroupDetailsNo { get; set; }
        
        /// <summary>
        /// 货位分组编号
        /// </summary>
        [SugarColumn(ColumnName = "F_GroupCode")]        
        public string GroupCode { get; set; }
        
        /// <summary>
        /// 货位起始行
        /// </summary>
        [SugarColumn(ColumnName = "F_Start_Row")]        
        public int? StartRow { get; set; }
        
        /// <summary>
        /// 货位终止行
        /// </summary>
        [SugarColumn(ColumnName = "F_End_Row")]        
        public int? EndRow { get; set; }
        
        /// <summary>
        /// 货位起始列
        /// </summary>
        [SugarColumn(ColumnName = "F_Start_Cell")]        
        public int? StartCell { get; set; }
        
        /// <summary>
        /// 货位终止列
        /// </summary>
        [SugarColumn(ColumnName = "F_End_Cell")]        
        public int? EndCell { get; set; }
        
        /// <summary>
        /// 货位起始层
        /// </summary>
        [SugarColumn(ColumnName = "F_Start_Layer")]        
        public int? StartLayer { get; set; }
        
        /// <summary>
        /// 货位终止层
        /// </summary>
        [SugarColumn(ColumnName = "F_End_Layer")]        
        public int? EndLayer { get; set; }
        
        /// <summary>
        /// 货位起始位深
        /// </summary>
        [SugarColumn(ColumnName = "F_Start_Depth")]        
        public int? StartDepth { get; set; }
        
        /// <summary>
        /// 货位终止位深
        /// </summary>
        [SugarColumn(ColumnName = "F_End_Depth")]        
        public int? EndDepth { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public string CreateTime { get; set; }
        
    }
}