using HSZ.Common.Const;
using SqlSugar;
using System;

namespace ZJN.Entitys.cabl
{
    /// <summary>
    /// demo信息
    /// </summary>
    [SugarTable("test1")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class Test1Entity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(ColumnName = "F_F2")]        
        public string F2 { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "F_F5")]        
        public string F5 { get; set; }
        
    }
}