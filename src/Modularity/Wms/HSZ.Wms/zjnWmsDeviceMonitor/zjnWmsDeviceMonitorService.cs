using HSZ.Common.Const;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Logging.Attributes;
using HSZ.Wms.Interfaces.zjnWmsDeviceMonitor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ZJN.Wcs.Entitys.Entity.ZjnPlcDto;

namespace HSZ.Wms.zjnWmsDeviceMonitor
{
    [ApiDescriptionSettings(Tag = "wms", Name = "zjnWmsDeviceMonitor", Order = 200)]
    [Route("api/wms/[controller]")]
    public class zjnWmsDeviceMonitorService: IZjnWmsDeviceMonitorService, IDynamicApiController, ITransient
    {

    }
}
