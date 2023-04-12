using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZJN.Agv.Entitys.Dto.AgvLimitGoods
{
    public class RESTfulResult_AgvLimitGoods
    {
        /// <summary>
        /// 返回码
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// 消息说明
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }
    }

    
}
