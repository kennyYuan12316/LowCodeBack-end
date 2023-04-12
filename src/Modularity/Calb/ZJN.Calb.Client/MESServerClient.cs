using Microsoft.Extensions.Options;
using Serilog;
using ServiceReferenceMESPolarInfromWs;
using ServiceReferenceMESproPanelWs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZJN.Calb.Client
{
    public class MESServerClient
    {
        private readonly WebSerivcesConfig _WebConfig;

        public MESServerClient(IOptionsSnapshot<WebSerivcesConfig> config)
        {
            _WebConfig = config.Value;
        }

        public async Task<calbPolarPieceReceiveResultVO> GetPolarinformAsync(string matBarcode)
        {
            var vo = new calbPolarMtBarCodeVO();
            vo.matBarcode = matBarcode;
            Log.Information(matBarcode);
            CalbWmesGetPolarInformWebServiceClient client = new CalbWmesGetPolarInformWebServiceClient(_WebConfig.MESPolarinform);
            var response = await client.wmsGetPolarinformAsync(vo);
            return response.@return;
        }


        public async Task<calbProPanelResultVO> GetFinishProPanel(string trayBarcode)
        {
            var vo = new calbProPanelTaryBarCodeVO();
            vo.trayBarcode = trayBarcode;
            Log.Information(trayBarcode);
            CalbWmsGetFinshProPanelWebServiceClient client = new CalbWmsGetFinshProPanelWebServiceClient(_WebConfig.MESFinishProPanel);
            var response = await client.wmsGetFinishProPanelAsync(vo);
            return response.@return;
        }
    }
}
