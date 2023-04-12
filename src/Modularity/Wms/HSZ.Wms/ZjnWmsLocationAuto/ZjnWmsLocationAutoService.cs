using HSZ.Common.Const;
using HSZ.Common.Core.Manager;
using HSZ.Common.DI;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.Message.Handler;
using HSZ.System.Entitys.Permission;
using HSZ.wms.Entitys.Dto.ZjnWmsLocation;
using HSZ.wms.Interfaces.ZjnWmsLocation;
using HSZ.Wms.Entitys.Dto.ZjnWmsLocation;
using HSZ.Wms.Interfaces.zjnLocationGenerator;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsLocationAuto
{
    /// <summary>
    /// 货位信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsLocationAuto", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsLocationAutoService : IZjnWmsLocationAutoService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsLocationEntity> _ZjnWmsLocationAutoRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;
        private readonly WebSocketMessageHandler _handler;
        private readonly ILocationGenerator _locationGenerator;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsLocationAutoService"/>类型的新实例
        /// </summary>
        public ZjnWmsLocationAutoService(ISqlSugarRepository<ZjnWmsLocationEntity> ZjnWmsLocationAutoRepository,
            IUserManager userManager,
            ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository,
            WebSocketMessageHandler handler,
            ILocationGenerator locationGenerator)
        {
            _ZjnWmsLocationAutoRepository = ZjnWmsLocationAutoRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
            _handler = handler;
            _locationGenerator = locationGenerator;
        }

        /// <summary>
        /// 获取所有货位信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <param name="aisleNo">巷道号</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<Dictionary<string, object>> GetList(int id, string aisleNo)
        {
            var referInfo = _locationGenerator.ReferInfo;
            var output = await _ZjnWmsLocationAutoRepository.AsSugarClient().Queryable<ZjnWmsLocationEntity>()
                    .Where(x => x.ByWarehouse == referInfo.WarehouseNo)
                    //.Where(x => x.LocationStatus != "0")
                    .WhereIF(id > 0, x => x.Layer == id)
                    .WhereIF(!string.IsNullOrEmpty(aisleNo), x => x.AisleNo == aisleNo)
                    .ToDictionaryAsync(k => k.LocationNo, v => v.LocationStatus);
            return output;
        }

        /// <summary>
        /// 更新货位状态
        /// </summary>
        /// <param name="locationNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [NonAction]
        public async Task UpdateLocationStatus(string locationNo, LocationStatus status)
        {
            await _ZjnWmsLocationAutoRepository.AsUpdateable()
                .SetColumns(x => new ZjnWmsLocationEntity() { LocationStatus = ((int)status).ToString() })
                .Where(x => x.LocationNo == locationNo).ExecuteCommandAsync();
            await _handler.SendMessageToAll((new { locationNo, status, method = "locationStatusChanged" }).ToJson());
        }

        /// <summary>
        /// 货位监控生成参数
        /// </summary>
        /// <returns></returns>
        [HttpGet("refer")]
        public async Task<ZjnWmsLocationDefineOutput> GetLocationRefer()
        {
            var referInfo = _locationGenerator.ReferInfo;
            var output = await _ZjnWmsLocationAutoRepository.AsSugarClient().Queryable<ZjnWmsLocationEntity>()
                    .Where(x => x.ByWarehouse == referInfo.WarehouseNo)
                    //.Where(x => x.LocationStatus != "0")
                    .Where(x => x.AisleNo == referInfo.AisleNo)
                    .ToDictionaryAsync(k => k.LocationNo, v => v.LocationStatus);
            referInfo.StatusList = output;
            return referInfo;
        }
    }
}


