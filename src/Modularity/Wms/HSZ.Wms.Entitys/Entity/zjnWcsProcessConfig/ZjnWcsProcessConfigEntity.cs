using HSZ.Common.Const;
using SqlSugar;
using System;

namespace HSZ.Entitys.wms
{
    /// <summary>
    /// 业务路径配置表
    /// </summary>
    [SugarTable("zjn_wcs_process_config")]
    [Tenant(ClaimConst.TENANT_ID)]
    public partial class ZjnWcsProcessConfigEntity
    {
        /// <summary>
        /// 任务路径配置主键
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 业务编号
        /// </summary>
        [SugarColumn(ColumnName = "F_work_no")]
        public string WorkNo { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        [SugarColumn(ColumnName = "F_work_name")]
        public string WorkName { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [SugarColumn(ColumnName = "F_work_type")]
        public int? WorkType { get; set; }

        /// <summary>
        /// 所属库位
        /// </summary>
        [SugarColumn(ColumnName = "F_work_warehouse")]
        public string WorkWarehouse { get; set; }

        /// <summary>
        /// 站点数
        /// </summary>
        [SugarColumn(ColumnName = "F_work_pathcount")]
        public int? WorkPathcount { get; set; }

        /// <summary>
        /// 是否存在动态站点
        /// </summary>
        [SugarColumn(ColumnName = "F_work_nodes")]
        public bool WorkNodes { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsType")]
        public string GoodsType { get; set; }

        /// <summary>
        /// 起点工位
        /// </summary>
        [SugarColumn(ColumnName = "F_work_start")]
        public string WorkStart { get; set; }

        /// <summary>
        /// 终点工位
        /// </summary>
        [SugarColumn(ColumnName = "F_work_end")]
        public string WorkEnd { get; set; }

        /// <summary>
        /// 业务完整路径
        /// </summary>
        [SugarColumn(ColumnName = "F_work_path")]
        public string WorkPath { get; set; }

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

    public partial class ZjnWcsProcessConfigEntity
    {
        /// <summary>
        /// 起点工位
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string[] WorkStarts
        {
            get
            {
                return WorkStart.Split(',');
            }
            set
            {
                WorkStart = string.Join(',', value);
            }
        }

        /// <summary>
        /// 终点工位
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string[] WorkEnds
        {
            get
            {
                return WorkEnd.Split(',');
            }
            set
            {
                WorkEnd = string.Join(',', value);
            }
        }
    }
}