using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 托盘信息
    /// </summary>
    [SugarTable("zjn_wms_tray")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsTrayEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayNo")]        
        public string TrayNo { get; set; }
        
        /// <summary>
        /// 托盘名称
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayName")]        
        public string TrayName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "F_Type")]        
        public int? Type { get; set; }

        /// <summary>
        /// 托盘状态
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayStates")]
        public int? TrayStates { get; set; }

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
        /// 有效标志
        /// </summary>
        [SugarColumn(ColumnName = "F_EnabledMark")]        
        public int? EnabledMark { get; set; }

        /// <summary>
        /// 是否删除 0未删除 1删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]
        public int? IsDelete { get; set; }

        /// <summary>
        /// 托盘属性
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayAttr")]
        public int? TrayAttr { get; set; }
    }
}