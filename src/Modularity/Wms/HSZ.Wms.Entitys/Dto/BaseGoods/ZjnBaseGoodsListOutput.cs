using System;

namespace HSZ.wms.Entitys.Dto.ZjnBaseGoods
{
    /// <summary>
    /// 货物信息输入参数
    /// </summary>
    public class ZjnBaseGoodsListOutput
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string a_F_Id { get; set; }
        
        /// <summary>
        /// 货物ID
        /// </summary>
        public string a_F_GoodsId { get; set; }
        
        /// <summary>
        /// 货物代码
        /// </summary>
        public string a_F_GoodsCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string a_goodsName { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string a_F_Unit { get; set; }
        
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string a_F_TrayNo { get; set; }
        
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? a_F_EnabledMark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? a_CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string a_CreateUser { get; set; }

        /// <summary>
        /// 货物类型
        /// </summary>
        public string a_GoodsType { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int? a1_F_EnabledMark { get; set; }
        
        /// <summary>
        /// 货物批次
        /// </summary>
        public string a1_F_batch { get; set; }
        
        /// <summary>
        /// 货物规格
        /// </summary>
        public string a1_F_specifications { get; set; }
        
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? a1_F_GoodsCreateData { get; set; }
        
        /// <summary>
        /// 货物状态
        /// </summary>
        public string a1_F_GoodsState { get; set; }
        
        /// <summary>
        /// 货位ID
        /// </summary>
        public string a1_F_GoodsLocationNo { get; set; }
        
        /// <summary>
        /// 客户ID
        /// </summary>
        public string a1_F_CustomerId { get; set; }
        
        /// <summary>
        /// 托盘ID
        /// </summary>
        public string a1_F_PalledNo { get; set; }
        
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string a1_F_VendorId { get; set; }
        
        /// <summary>
        /// 检验日期
        /// </summary>
        public DateTime? a1_F_CheckDate { get; set; }
        
        /// <summary>
        /// 检验类型
        /// </summary>
        public string a1_F_CheckType { get; set; }
        
        /// <summary>
        /// 货物等级
        /// </summary>
        public string a1_F_GoodsGrade { get; set; }
        
        /// <summary>
        /// 货物描述
        /// </summary>
        public string a1_F_Remarks { get; set; }

        /// <summary>
        /// 货位 
        /// </summary>
        public string a2_GoodsLocationNo { get; set; }

        /// <summary>
        /// 修改用户 
        /// </summary>
        public string a1_F_LastModifyUserId { get; set; }

        /// <summary>
        /// 修改时间 
        /// </summary>
        public DateTime? a1_F_LastModifyTime { get; set; }
         
        public string a_F_EnabledMark2 { get; set; }

    }
}