using HSZ.Wms.Entitys.Dto.ZjnWmsLocation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Wms.Interfaces.zjnLocationGenerator
{
    /// <summary>
    /// 货位生成类
    /// </summary>
    public interface ILocationGenerator
    {
        /// <summary>
        /// 每层最大数，sqlsugar partitionby必须指定数量，否则只返回1条
        /// </summary>
        int MaxPerLayer { get; }
        /// <summary>
        /// 货位参数
        /// </summary>
        ZjnWmsLocationDefineOutput ReferInfo { get; }
        /// <summary>
        /// 生成货位
        /// </summary>
        /// <returns></returns>
        Task GenerateLocation();
    }
}
