using HSZ.Common.Const;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Entitys.wms
{

    /// <summary>
    /// 平面库物料基础信息
    /// </summary>
    [SugarTable("zjn_wms_goods")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ZjnWmsGoodsEntity
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsCode")]
        public string GoodsCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsName")]
        public string GoodsName { get; set; }

        /// <summary>
        /// 物料单位（根据数据字典来）
        /// </summary>
        [SugarColumn(ColumnName = "F_Unit")]
        public int? Unit { get; set; }

        /// <summary>
        /// 物料状态 （根据数据字典来）
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsState")]
        public int? GoodsState { get; set; }

        /// <summary>
        /// 物料类型（根据数据字典来）
        /// </summary>
        [SugarColumn(ColumnName = "F_GoodsType")]
        public string GoodsType { get; set; }

        /// <summary>
        /// 物料规格
        /// </summary>
        [SugarColumn(ColumnName = "F_Specifications")]
        public string Specifications { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        [SugarColumn(ColumnName = "F_CustomerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [SugarColumn(ColumnName = "F_VendorId")]
        public string VendorId { get; set; }

        /// <summary>
        /// 检验类型（根据数据字典来）
        /// </summary>
        [SugarColumn(ColumnName = "F_CheckType")]
        public int? CheckType { get; set; }

        /// <summary>
        /// 先入先出 1是 0否
        /// </summary>
        [SugarColumn(ColumnName = "F_IsFirstOut")]
        public int? IsFirstOut { get; set; }

        /// <summary>
        /// 预警周期（天）
        /// </summary>
        [SugarColumn(ColumnName = "F_TellDate")]
        public int? TellDate { get; set; }

        /// <summary>
        /// 禁用原因
        /// </summary>
        [SugarColumn(ColumnName = "F_DisableMark")]
        public string DisableMark { get; set; }

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
        /// 修改用户
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyUserId")]
        public string LastModifyUserId { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LastModifyTime")]
        public DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 是否删除 1删除 0未删除
        /// </summary>
        [SugarColumn(ColumnName = "F_IsDelete")]
        public int? IsDelete { get; set; }

        /// <summary>
        /// 上限
        /// </summary>
        [SugarColumn(ColumnName = "F_Ceiling")]
        public string Ceiling { get; set; }

        /// <summary>
        /// 下限
        /// </summary>
        [SugarColumn(ColumnName = "F_TheLowerLimit")]
        public string TheLowerLimit { get; set; }

        /// <summary>
        /// 保质期
        /// </summary>
        [SugarColumn(ColumnName = "F_ShelfLife")]
        public string ShelfLife { get; set; }

        /// <summary>
        /// 静置时间
        /// </summary>
        [SugarColumn(ColumnName = "F_StayHours")]
        public string StayHours { get; set; }

        /// <summary>
        /// 是否批次管理 1:是；0：否
        /// </summary>
        [SugarColumn(ColumnName = "F_BatchManageFlag")]
        public int? BatchManageFlag { get; set; }

        /// <summary>
        /// 备用字段
        /// </summary>
        [SugarColumn(ColumnName = "case5")]
        public string Case5 { get; set; }


    }

}
