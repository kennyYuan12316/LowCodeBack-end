using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Mapster;
using ZJN.Entitys.wcs;
using HSZ.Entitys.wms;

namespace ZJN.Calb.Entitys.Dto
{

    /// <summary>
    /// 物料主数据 请求
    /// </summary>
    public class GoodsDetailsRequest : AttributeExt
    {
        [Required()]
        [DisplayName("物料编号")]
        [MaxLength(50)]
        [AdaptMember(nameof(ZjnBaseLesGoodsEntity.Code))]
        public string Code { get; set; }


        [Required()]
        [DisplayName("物料名称")]
        [MaxLength(50)]
        [AdaptMember(nameof(ZjnBaseLesGoodsEntity.XName))]
        public string xName { get; set; }


        [Required()]
        [DisplayName("物料类型")]
        [MaxLength(50)]
        [AdaptMember(nameof(ZjnBaseLesGoodsEntity.XType))]
        public string xType { get; set; }

        [Required()]
        [DisplayName("基本单位")]
        [MaxLength(50)]
        [AdaptMember(nameof(ZjnBaseLesGoodsEntity.DefaultUnit))]
        public string DefaultUnit { get; set; }

        [Required()]
        [DisplayName("总有效期(天）")]
        [Range(0, 9999)]
        [AdaptMember(nameof(ZjnBaseLesGoodsEntity.ValidDays))]
        public int? ValidDays { get; set; }

        [Required()]
        [DisplayName("静置时间")]
        [Range(0, 9999)]
        [AdaptMember(nameof(ZjnBaseLesGoodsEntity.StayHours))]
        public int? StayHours { get; set; }

        [Required()]
        [DisplayName("是否批次管理")]
        [Range(0, 9999)]
        [AdaptMember(nameof(ZjnBaseLesGoodsEntity.BatchManageFlag))]
        public int? BatchManageFlag { get; set; }


        [Required()]
        [DisplayName("规格型号")]
        [MaxLength(50)]
        [AdaptMember(nameof(ZjnBaseLesGoodsEntity.Specification))]
        public string Specification { get; set; }

        //[Required()]
        //[DisplayName("物料编号")]
        //[MaxLength(50)]
        //[AdaptMember(nameof(BaseGoodsEntity.GoodsCode))]
        //public string Code { get; set; }


        //[Required()]
        //[DisplayName("物料名称")]
        //[MaxLength(50)]
        //[AdaptMember(nameof(BaseGoodsEntity.GoodsName))]
        //public string xName { get; set; }


        //[Required()]
        //[DisplayName("物料类型")]
        //[MaxLength(50)]
        //[AdaptMember(nameof(BaseGoodsEntity.GoodsType))]
        //public string xType { get; set; }

        //[Required()]
        //[DisplayName("基本单位")]
        //[MaxLength(50)]
        //[AdaptMember(nameof(BaseGoodsEntity.Unit))]
        //public string DefaultUnit { get; set; }

        //[Required()]
        //[DisplayName("总有效期(天）")]
        //[Range(0, 9999)]
        //public string ValidDays { get; set; }

        //[Required()]
        //[DisplayName("静置时间")]
        //public string StayHours { get; set; }

        //[Required()]
        //[DisplayName("是否批次管理")]
        //public string BatchManageFlag { get; set; }


        //[Required()]
        //[DisplayName("规格型号")]
        //public string Specification { get; set; }
    }


}
