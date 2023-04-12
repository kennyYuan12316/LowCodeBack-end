using System;

namespace HSZ.wms.Entitys.Dto.ZjnWmsTray
{
    /// <summary>
    /// 托盘信息输入参数
    /// </summary>
    public class ZjnWmsTrayListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string F_Id { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string F_TrayNo { get; set; }
        
        /// <summary>
        /// 托盘名称
        /// </summary>
        public string F_TrayName { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public int? F_Type { get; set; }

        /// <summary>
        /// 托盘状态
        /// </summary>
        public int? F_TrayStates { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string F_TypeName { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string F_CreateUser { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? F_EnabledMark { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public string EnabledMark { get; set; }

        /// <summary>
        /// 托盘属性
        /// </summary>
        public int? F_TrayAttr { get; set; }

    }
}