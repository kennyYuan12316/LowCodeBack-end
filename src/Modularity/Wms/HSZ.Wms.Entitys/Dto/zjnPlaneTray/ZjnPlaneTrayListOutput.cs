using System;

namespace HSZ.wms.Entitys.Dto.ZjnPlaneTray
{
    /// <summary>
    /// 平面库托盘信息维护输入参数
    /// </summary>
    public class ZjnPlaneTrayListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }
        
        /// <summary>
        /// 托盘类型（根据数据字典来）
        /// </summary>
        public int? F_Type { get; set; }
        /// <summary>
        /// 托盘类型名称（根据数据字典来）
        /// </summary>
        public string TypeNmae { get; set; }

        /// <summary>
        /// 托盘状态（根据数据字典来）
        /// </summary>
        public int? F_TrayState { get; set; }

        /// <summary>
        /// 托盘状态名称（根据数据字典来）
        /// </summary>
        public string TrayStateName { get; set; }

        /// <summary>
        /// 是否删除 0未删除 1删除
        /// </summary>
        public int? F_IsDelete { get; set; }
        /// <summary>
        /// 禁用原因
        /// </summary>
        public string F_DisableMark { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 修改人
        /// </summary>
        public string F_LastModifyUserId { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? F_LastModifyTime { get; set; }
        
    }
}