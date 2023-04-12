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
using HSZ.wms.Entitys.Dto.ZjnWmsTrayGoodsLog;
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

namespace HSZ.wms.ZjnWmsTrayGoodsLog
{
    /// <summary>
    /// 托盘绑定履历表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWmsTrayGoodsLog", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsTrayGoodsLogService : IZjnWmsTrayGoodsLogService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsTrayGoodsLogEntity> _zjnRecordTrayGoodsLogRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsTrayGoodsLogService"/>类型的新实例
        /// </summary>
        public ZjnWmsTrayGoodsLogService(ISqlSugarRepository<ZjnWmsTrayGoodsLogEntity> zjnRecordTrayGoodsLogRepository,
            IUserManager userManager)
        {
            _zjnRecordTrayGoodsLogRepository = zjnRecordTrayGoodsLogRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取托盘绑定履历表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnRecordTrayGoodsLogRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsTrayGoodsLogInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取托盘绑定履历表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsTrayGoodsLogListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnRecordTrayGoodsLogRepository.AsSugarClient().Queryable<ZjnWmsTrayGoodsLogEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsCode), a => a.GoodsCode.Contains(input.F_GoodsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .Select((a
)=> new ZjnWmsTrayGoodsLogListOutput
                {
                    F_Id = a.Id,
                    F_GoodsCode = a.GoodsCode,
                    F_Quantity = a.Quantity,
                    F_Unit = a.Unit,
                    F_TrayNo = a.TrayNo,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = SqlFunc.IIF(a.EnabledMark == 0, "无效", "有效"),
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWmsTrayGoodsLogListOutput>.SqlSugarPageResult(data);
        }
    }
}


