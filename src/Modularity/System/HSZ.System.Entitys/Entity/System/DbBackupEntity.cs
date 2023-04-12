using HSZ.Common.Const;
using HSZ.Common.Entity;
using SqlSugar;
using System;

namespace HSZ.System.Entitys.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据备份
    /// </summary>
    [SugarTable("ZJN_BASE_DB_BACKUP")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DbBackupEntity : CLDEntityBase
    {
        /// <summary>
        /// 备份库名
        /// </summary>
        [SugarColumn(ColumnName = "F_BACKUPDBNAME")]
        public string BackupDbName { get; set; }

        /// <summary>
        /// 备份时间
        /// </summary>
        [SugarColumn(ColumnName = "F_BACKUPTIME")]
        public DateTime? BackupTime { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FILENAME")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [SugarColumn(ColumnName = "F_FILESIZE")]
        public string FileSize { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEPATH")]
        public string FilePath { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}
