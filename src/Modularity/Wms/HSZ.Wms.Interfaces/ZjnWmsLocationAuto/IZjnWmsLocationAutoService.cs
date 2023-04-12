using HSZ.Common.Enum;
using HSZ.Entitys.wms;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.wms.Interfaces.ZjnWmsLocation
{
    public interface IZjnWmsLocationAutoService
    {
        /// <summary>
        /// 获取所有货位信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <param name="aisleNo">巷道号</param>
        /// <returns></returns>
        public Task<Dictionary<string, object>> GetList(int id, string aisleNo);

        /// <summary>
        /// 更新货位状态
        /// </summary>
        /// <param name="locationNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Task UpdateLocationStatus(string locationNo, LocationStatus status);
    }
}