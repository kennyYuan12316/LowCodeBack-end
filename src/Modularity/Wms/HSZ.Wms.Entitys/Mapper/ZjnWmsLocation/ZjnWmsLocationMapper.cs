using HSZ.Common.Helper;
using HSZ.Entitys.wms;
using HSZ.wms.Entitys.Dto.ZjnWmsLocation;
using Mapster;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Mapper.ZjnWmsLocation
{
	public class Mapper : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			//config.ForType<ZjnWmsLocationCrInput, ZjnWmsLocationEntity>()
			//	.Map(dest => dest.AisleNo, src => src.aisleNo.ToJson())
			//;
			//config.ForType<ZjnWmsLocationEntity, ZjnWmsLocationInfoOutput>()
			//	.Map(dest => dest.aisleNo, src => src.AisleNo.ToObject<List<string>>())
			//;
		}
	}
}
