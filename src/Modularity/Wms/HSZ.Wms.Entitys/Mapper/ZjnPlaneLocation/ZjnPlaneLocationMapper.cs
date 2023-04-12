using HSZ.Common.Helper;
using HSZ.Entitys.wms;
using HSZ.wms.Entitys.Dto.ZjnPlaneLocation;
using Mapster;
using System.Collections.Generic;

namespace HSZ.wms.Entitys.Mapper.ZjnPlaneLocation
{
	public class Mapper : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.ForType<ZjnPlaneLocationCrInput, ZjnPlaneLocationEntity>()
				.Map(dest => dest.AisleNo, src => src.aisleNo.ToJson())
			;
			config.ForType<ZjnPlaneLocationEntity, ZjnPlaneLocationInfoOutput>()
				.Map(dest => dest.aisleNo, src => src.AisleNo.ToObject<List<string>>())
			;
		}
	}
}
