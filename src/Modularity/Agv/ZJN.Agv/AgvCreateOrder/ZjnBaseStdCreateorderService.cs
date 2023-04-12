using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Agv.Entitys.Dto.AgvCreateOrder;
using ZJN.Agv.Entitys.Entity;
using ZJN.Agv.Interfaces;

namespace ZJN.Agv.AgvCreateOrder
{
    /// <summary>
    /// 立库下单服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "Agv",Name = "ZjnBaseStdCreateorder", Order = 200)]
    [Route("api/agv/[controller]")]
    public class ZjnBaseStdCreateorderService : IZjnBaseStdCreateorderService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseStdCreateorderEntity> _zjnBaseStdCreateorderRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseStdCreateorderService"/>类型的新实例
        /// </summary>
        public ZjnBaseStdCreateorderService(ISqlSugarRepository<ZjnBaseStdCreateorderEntity> zjnBaseStdCreateorderRepository,
            IUserManager userManager)
        {
            _zjnBaseStdCreateorderRepository = zjnBaseStdCreateorderRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取立库下单
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseStdCreateorderRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseStdCreateorderInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取立库下单列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseStdCreateorderListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_OuterOrderId" : input.sidx;
            var data = await _zjnBaseStdCreateorderRepository.AsSugarClient().Queryable<ZjnBaseStdCreateorderEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_BrCode), a => a.BrCode.Contains(input.F_BrCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_EndAreaCode), a => a.EndAreaCode.Contains(input.F_EndAreaCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_EndLocCode), a => a.EndLocCode.Contains(input.F_EndLocCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_StartAreaCode), a => a.StartAreaCode.Contains(input.F_StartAreaCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_StartLocCode), a => a.StartLocCode.Contains(input.F_StartLocCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayId), a => a.TrayId.Contains(input.F_TrayId))
                .Select((a
                 )=> new ZjnBaseStdCreateorderListOutput
                {
                    F_Id = a.Id,
                    F_BrCode = a.BrCode,
                    F_EndAreaCode = a.EndAreaCode,
                    F_EndLocCode = a.EndLocCode,
                    F_LesOrderId = a.LesOrderId,
                    F_OuterOrderId = a.OuterOrderId,
                    F_StartAreaCode = a.StartAreaCode,
                    F_StartLocCode = a.StartLocCode,
                    F_TrayId = a.TrayId,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseStdCreateorderListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建立库下单
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseStdCreateorderCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseStdCreateorderEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _zjnBaseStdCreateorderRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新立库下单
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseStdCreateorderUpInput input)
        {
            var entity = input.Adapt<ZjnBaseStdCreateorderEntity>();
            var isOk = await _zjnBaseStdCreateorderRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除立库下单
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseStdCreateorderRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseStdCreateorderRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


