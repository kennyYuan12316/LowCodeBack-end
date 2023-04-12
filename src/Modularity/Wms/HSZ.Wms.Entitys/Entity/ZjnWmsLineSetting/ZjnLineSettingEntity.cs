using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 线体信息配置
    /// </summary>
    [SugarTable("zjn_wms_line_setting")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsLineSettingEntity
    {
        /// <summary>
        /// 主键ID
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
        /// 电芯类型
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsType")]
        public string GoodsType { get; set; }
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
        /// 线体最大任务（缓存）数
        /// </summary>
        [SugarColumn(ColumnName = "F_LineMaxWork")]        
        public int? LineMaxWork { get; set; }
        
        /// <summary>
        /// 当前任务（缓存）数量
        /// </summary>
        [SugarColumn(ColumnName = "F_LineNowWork")]        
        public int? LineNowWork { get; set; }
        
        /// <summary>
        /// 线体描述
        /// </summary>
        [SugarColumn(ColumnName = "F_Description")]        
        public string Description { get; set; }
        
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
        /// 创建名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CreateTime")]        
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 更新用户
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyUserId")]        
        public string LastModifyUserId { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyTime")]        
        public DateTime? LastModifyTime { get; set; }
        
        /// <summary>
        /// josn预留字段
        /// </summary>
        [SugarColumn(ColumnName = "F_Expand")]        
        public string Expand { get; set; }
        
    }
}