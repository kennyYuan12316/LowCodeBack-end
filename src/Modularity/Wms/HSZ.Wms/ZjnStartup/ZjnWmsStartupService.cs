using HSZ.Dependency;
using HSZ.Wms.PLC;
using HSZ.Wms.Interfaces.ZjnStartup;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSZ.Wms.Interfaces.zjnLocationGenerator;

namespace HSZ.Wms.ZjnStartup
{
    public class ZjnWmsStartupService : IZjnStartupService, ITransient
    {
        private readonly PLCObjectService _plcObjectService;
        private readonly ILocationGenerator _locationGenerator;

        public ZjnWmsStartupService(PLCObjectService plcObjectService, ILocationGenerator locationGenerator)
        {
            _plcObjectService = plcObjectService;
            _locationGenerator = locationGenerator;
        }

        public async Task StartAsync()
        {
            await _locationGenerator.GenerateLocation();
        }
    }
}
