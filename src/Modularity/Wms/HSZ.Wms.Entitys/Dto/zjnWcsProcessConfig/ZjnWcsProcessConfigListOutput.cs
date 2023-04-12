using System;

namespace HSZ.wms.Entitys.Dto.zjnWcsProcessConfig
{
    /// <summary>
    /// 业务路径配置表输入参数
    /// </summary>
    public class ZjnWcsProcessConfigListOutput
    {
        /// <summary>
        /// 任务路径配置主键
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 业务编号
        /// </summary>
        public string F_work_no { get; set; }
        
        /// <summary>
        /// 业务名称
        /// </summary>
        public string F_work_name { get; set; }
        
        /// <summary>
        /// 业务类型
        /// </summary>
        public int? F_work_type { get; set; }

        /// <summary>
        /// 业务类型名称
        /// </summary>
        public string F_work_typeName { get; set; }


        /// <summary>
        /// 物料类型
        /// </summary>
        public string F_GoodsType { get; set; }

        /// <summary>
        /// 物料类型名称
        /// </summary>
        public string F_GoodsTypeName { get; set; }

        /// <summary>
        /// 业务类型名称
        /// </summary>
        public string F_work_type_Name { get; set; }

        /// <summary>
        /// 所属库位
        /// </summary>
        public string F_work_warehouse { get; set; }

        /// <summary>
        /// 所属库位名称
        /// </summary>
        public string F_work_warehouseName { get; set; }


        /// <summary>
        /// 是否存在动态站点
        /// </summary>
        public string F_work_nodes { get; set; }
        
        /// <summary>
        /// 起点工位
        /// </summary>
        public string F_work_start { get; set; }

        /// <summary>
        /// 起点工位名称
        /// </summary>
        public string F_work_startName { get; set; }

        /// <summary>
        /// 终点工位
        /// </summary>
        public string F_work_end { get; set; }

        /// <summary>
        /// 终点工位名称
        /// </summary>
        public string F_work_endName { get; set; }

        /// <summary>
        /// 业务路径json
        /// </summary>
        public string F_work_Path { get; set; }

        /// <summary>
        /// 站点数
        /// </summary>
        public int workPathcount { get; set; }

        /// <summary>
        /// 业务完整路径
        /// </summary>
        public string workPath { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 修改用户
        /// </summary>
        public string F_LastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? F_LastModifyTime { get; set; }
        
    }
}