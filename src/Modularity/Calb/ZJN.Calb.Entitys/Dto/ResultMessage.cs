using HSZ.JsonSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ZJN.Calb.Entitys.Dto
{
    public class ResultMessage
    {
        /// <summary>
        /// 代码
        /// </summary>
        public int code { get; set; } = 0;


        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; } = "Success";
    }


    public class ResultMessageHead : AttributeExt
    {
        /// <summary>
        /// 代码
        /// </summary>
        public int code { get; set; } = 0;


        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; } = "Success";
    }

    public class AttributeExt
    {
        public string attribute1 { get; set; }
        public string attribute2 { get; set; }
        public string attribute3 { get; set; }
        public string attribute4 { get; set; }
        public string attribute5 { get; set; }
        public string attribute6 { get; set; }
        public string attribute7 { get; set; }
        public string attribute8 { get; set; }
        public string attribute9 { get; set; }
        public string attribute10 { get; set; }
        public string attribute11 { get; set; }
        public string attribute12 { get; set; }
        public string attribute13 { get; set; }
        public string attribute14 { get; set; }
        public string attribute15 { get; set; }

    }

    public class ResultPayload
    {

        public string payload { get; set; } = string.Empty;

        public static ResultPayload GetResult<T>(T oPayload)
        {
            ResultPayload result = new ResultPayload();
            result.payload = oPayload.Serialize();
            return result;
        }
    }

}
