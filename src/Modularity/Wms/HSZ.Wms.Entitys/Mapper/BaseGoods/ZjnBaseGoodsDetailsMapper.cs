using HSZ.Common.Helper;
using HSZ.Entitys.wms;
using HSZ.wms.Entitys.Dto.ZjnBaseGoods;
using Mapster;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Mapper.ZjnBaseGoodsDetails
{
	public class Mapper : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.ForType<ZjnBaseGoodsCrInput, ZjnBaseGoodsDetailsEntity>()
				.Map(dest => dest.EnabledMark, src => src.hsz_zjn_base_goods_details_hsz_enabledMark)
				.Map(dest => dest.Batch, src => src.hsz_zjn_base_goods_details_hsz_batch)
				.Map(dest => dest.Specifications, src => src.hsz_zjn_base_goods_details_hsz_specifications)
				.Map(dest => dest.GoodsCreateData, src => src.hsz_zjn_base_goods_details_hsz_goodsCreateData)
				.Map(dest => dest.GoodsState, src => src.hsz_zjn_base_goods_details_hsz_goodsState)
				.Map(dest => dest.GoodsLocationNo, src => src.hsz_zjn_base_goods_details_hsz_goodsLocationNo)
				.Map(dest => dest.CustomerId, src => src.hsz_zjn_base_goods_details_hsz_customerId)
				.Map(dest => dest.PalledNo, src => src.hsz_zjn_base_goods_details_hsz_palledNo)
				.Map(dest => dest.VendorId, src => src.hsz_zjn_base_goods_details_hsz_vendorId)
				.Map(dest => dest.CheckDate, src => src.hsz_zjn_base_goods_details_hsz_checkDate)
				.Map(dest => dest.CheckType, src => src.hsz_zjn_base_goods_details_hsz_checkType)
				.Map(dest => dest.GoodsGrade, src => src.hsz_zjn_base_goods_details_hsz_goodsGrade)
				.Map(dest => dest.Remarks, src => src.hsz_zjn_base_goods_details_hsz_remarks)
			;
			config.ForType<ZjnBaseGoodsDetailsEntity, ZjnBaseGoodsInfoOutput>()
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_enabledMark, src => src.EnabledMark)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_batch, src => src.Batch)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_specifications, src => src.Specifications)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_goodsCreateData, src => src.GoodsCreateData)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_goodsState, src => src.GoodsState)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_goodsLocationNo, src => src.GoodsLocationNo)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_customerId, src => src.CustomerId)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_palledNo, src => src.PalledNo)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_vendorId, src => src.VendorId)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_checkDate, src => src.CheckDate)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_checkType, src => src.CheckType)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_goodsGrade, src => src.GoodsGrade)
				.Map(dest => dest.hsz_zjn_base_goods_details_hsz_remarks, src => src.Remarks)
			;
		}
	}
}
