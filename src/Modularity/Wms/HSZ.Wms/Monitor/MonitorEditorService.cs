using HSZ.Common.Core.Manager;
using HSZ.ClayObject;
using HSZ.Common.Configuration;
using HSZ.Common.Model.NPOI;
using HSZ.DataEncryption;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.wms.Entitys.Dto.ZjnBaseMaterialInventoryDetail;
using HSZ.wms.Interfaces.ZjnBaseMaterialInventoryDetail;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using HSZ.Wms.Interfaces.Monitor;

namespace HSZ.wms.Monitor
{
    /// <summary>
    /// 设备监控服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "MonitorEditor", Order = 200)]
    [Route("api/wms/[controller]")]
    public class MonitorEditorService : IMonitorEditorService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<MonitorConfigInfo> _monitorConfigInfoRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="MonitorEditorService"/>类型的新实例
        /// </summary>
        public MonitorEditorService(ISqlSugarRepository<MonitorConfigInfo> monitorConfigInfoRepository,
            IUserManager userManager)
        {
            _monitorConfigInfoRepository = monitorConfigInfoRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取当前楼的配置
        /// </summary>
        /// <param name="floor">楼层</param>
        /// <returns></returns>
        [HttpGet("GetMonitorConfigInfoByFloor")]
        public async Task<List<MonitorConfigInfo>> GetMonitorConfigInfoByFloorAsync(string floor = "a1")
        {
            var monitors = await _monitorConfigInfoRepository.AsQueryable().Where(l => l.Name.Contains(floor)).ToListAsync();

            return monitors;
        }
    }
}


