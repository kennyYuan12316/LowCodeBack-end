using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 线体物料绑定履历表
    /// </summary>
    [SugarTable("zjn_wms_line_settinglog")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsLineSettinglogEntity
    {
        /// <summary>
        /// 唯一Id
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 线体编号
        /// </summary>
        [SugarColumn(ColumnName = "F_LineNo")]        
        public string LineNo { get; set; }
        
        /// <summary>
        /// 线体名称
        /// </summary>
        [SugarColumn(ColumnName = "F_LineName")]        
        public string LineName { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        [SugarColumn(ColumnName = "F_TrayNo")]        
        public string TrayNo { get; set; }
        
        /// <summary>
        /// 物料类型
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsType")]        
        public string GoodsType { get; set; }
        
        /// <summary>
        /// 物料编号
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsCode")]        
        public string GoodsCode { get; set; }

        /// <summary>
        /// 线体缓存起点
        /// </summary>
        [SugarColumn(ColumnName = "F_LineStart")]        
        public string LineStart { get; set; }

        /// <summary>
        /// 线体缓存终点
        /// </summary>
        [SugarColumn(ColumnName = "F_LineEnd")]        
        public string LineEnd { get; set; }

        /// <summary>
        /// 线体层
        /// </summary>
        [SugarColumn(ColumnName = "F_LineLayer")]        
        public string LineLayer { get; set; }

        /// <summary>
        /// 线体最大任务(缓存)数
        /// </summary>
        [SugarColumn(ColumnName = "F_LineMaxWork")]        
        public int? LineMaxWork { get; set; }

        /// <summary>
        /// 当前任务(缓存)数量
        /// </summary>
        [SugarColumn(ColumnName = "F_LineNowWork")]        
        public int? LineNowWork { get; set; }

        /// <summary>
        /// 线体描述
        /// </summary>
        [SugarColumn(ColumnName = "F_Description")]        
        public string Description { get; set; }

        /// <summary>
        /// josn预留字段
        /// </summary>
        [SugarColumn(ColumnName = "F_Expand")]        
        public string Expand { get; set; }
        
        /// <summary>
        /// 1-在线，2-已出
        /// </summary>
        [SugarColumn(ColumnName = "F_Status")]        
        public int? Status { get; set; }
        
        /// <summary>
        /// 线体出库时间
        /// </summary>
        [SugarColumn(ColumnName = "F_OutTime")]        
        public DateTime? OutTime { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateUser")]        
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
    }
}