using HSZ.Entitys.wms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Wms.Entitys.Dto.ZjnWmsGoods
{
   public class BaseLocationImportResultOutput
    {
        // <summary>
        /// 导入成功条数
        /// </summary>
        public int snum { get; set; }

        /// <summary>
        /// 导入失败条数
        /// </summary>
        public int fnum { get; set; }

        /// <summary>
        /// 导入结果状态(0：成功，1：失败)
        /// </summary>
        public int resultType { get; set; }

        /// <summary>
        /// 失败结果集合
        /// </summary>
        public List<ZjnPlaneLocationEntity> failResult { get; set; }
    }
}
