using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZJN.Calb.Client.DTO;

namespace HSZ.Wms.Interfaces.ZjnWorkProsess
{
    public interface  IProductionOCVDeviceProcess
    {
        public  Task<bool> RequestForCellPanel(ContainerIntoApplyRequest requst);
    }
}
