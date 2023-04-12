using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZJN.Agv.Entitys.Dto.AgvGoodsType
{
    public class RESTfulResult_AgvGoodsType
    {
        /// <summary>
        /// 返回码
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// 物料类型列表
        /// </summary>
        [JsonProperty("goodsList")]
        public GoodsList[] GoodsLists { get; set; }

        /// <summary>
        /// 消息说明
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }
    }

    /// <summary>
    /// 物料类型列表
    /// </summary>
    public partial class GoodsList
    {
        /// <summary>
        /// 物料ID
        /// </summary>
        [JsonProperty("goodsId")]
        public string GoodsId { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        [JsonProperty("goodsName")]
        public string GoodsName { get; set; }
    }


}
