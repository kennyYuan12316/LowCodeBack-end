using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.wms.Entitys.Dto.ZjnWmsTrayLocationLog;
using HSZ.wms.Interfaces.ZjnWmsTrayGoodsLog;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsTrayLocationLog
{
    /// <summary>
    /// 托盘货位绑定履历表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWmsTrayLocationLog", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsTrayLocationLogService : IZjnWmsTrayGoodsLogService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTrayLocationLogEntity> _zjnRecordTrayLocationLogRepository;
        private readonly ISqlSugarRepository<ZjnWmsTrayGoodsLogEntity> __zjnRecordTrayGoodsLogRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsTrayLocationLogService"/>类型的新实例
        /// </summary>
        public ZjnWmsTrayLocationLogService(ISqlSugarRepository<ZjnWmsTrayLocationLogEntity> zjnRecordTrayLocationLogRepository,
            IUserManager userManager)
        {
            _zjnRecordTrayLocationLogRepository = zjnRecordTrayLocationLogRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取托盘货位绑定履历表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnRecordTrayLocationLogRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsTrayLocationLogInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取托盘货位绑定履历表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsTrayLocationLogListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnRecordTrayLocationLogRepository.AsSugarClient().Queryable<ZjnWmsTrayLocationLogEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsCode), a => a.GoodsCode.Contains(input.F_GoodsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_LocationNo), a => a.LocationNo.Contains(input.F_LocationNo))
                .Select((a
)=> new ZjnWmsTrayLocationLogListOutput
                {
                    F_Id = a.Id,
                    F_GoodsCode = a.GoodsCode,
                    F_Quantity = a.Quantity,
                    F_Unit = a.Unit,
                    F_TrayNo = a.TrayNo,
                    F_LocationNo = a.LocationNo,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = SqlFunc.IIF(a.EnabledMark == 0, "无效", "有效"),
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWmsTrayLocationLogListOutput>.SqlSugarPageResult(data);
        }
       
    }
}


