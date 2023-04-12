using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSZ.JsonSerialization;

namespace ZJN.Calb.Client
{
    public class PayloadRequest
    {
        public string input { get; set; }

        public string type { get; set; }


        public PayloadRequest(object inputData, string type)
        {
            this.input = inputData.Serialize();
            this.type = type;
        }

        public static PayloadRequest Payload(object inputData, string type)
        {
            return new PayloadRequest(inputData, type);
        }
    }
}
