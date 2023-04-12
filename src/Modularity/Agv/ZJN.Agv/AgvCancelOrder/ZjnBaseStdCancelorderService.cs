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
using ZJN.Agv.Entitys.Dto.AgvCancelOrder;
using ZJN.Agv.Entitys.Entity;
using ZJN.Agv.Interfaces;

namespace ZJN.Agv.AgvCancelOrder
{
    /// <summary>
    /// 立库取消订单服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "Agv",Name = "ZjnBaseStdCancelorder", Order = 200)]
    [Route("api/agv/[controller]")]
    public class ZjnBaseStdCancelorderService : IZjnBaseStdCancelorderService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseStdCancelorderEntity> _zjnBaseStdCancelorderRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseStdCancelorderService"/>类型的新实例
        /// </summary>
        public ZjnBaseStdCancelorderService(ISqlSugarRepository<ZjnBaseStdCancelorderEntity> zjnBaseStdCancelorderRepository,
            IUserManager userManager)
        {
            _zjnBaseStdCancelorderRepository = zjnBaseStdCancelorderRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取立库取消订单
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseStdCancelorderRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseStdCancelorderInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取立库取消订单列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseStdCancelorderListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_OuterOrderId" : input.sidx;
            List<string> queryCreateTime = input.F_CreateTime != null ? input.F_CreateTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startCreateTime = queryCreateTime != null ? Ext.GetDateTime(queryCreateTime.First()) : null;
            DateTime? endCreateTime = queryCreateTime != null ? Ext.GetDateTime(queryCreateTime.Last()) : null;
            var data = await _zjnBaseStdCancelorderRepository.AsSugarClient().Queryable<ZjnBaseStdCancelorderEntity>()
                .WhereIF(queryCreateTime != null, a => a.CreateTime >= new DateTime(startCreateTime.ToDate().Year, startCreateTime.ToDate().Month, startCreateTime.ToDate().Day, 0, 0, 0))
                .WhereIF(queryCreateTime != null, a => a.CreateTime <= new DateTime(endCreateTime.ToDate().Year, endCreateTime.ToDate().Month, endCreateTime.ToDate().Day, 23, 59, 59))
                .WhereIF(!string.IsNullOrEmpty(input.F_OuterOrderId), a => a.OuterOrderId.Contains(input.F_OuterOrderId))
                .Select((a
)=> new ZjnBaseStdCancelorderListOutput
                {
                    F_Id = a.Id,
                    F_CreateTime = a.CreateTime,
                    F_OuterOrderId = a.OuterOrderId,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseStdCancelorderListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建立库取消订单
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseStdCancelorderCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseStdCancelorderEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateTime = DateTime.Now;
            
            var isOk = await _zjnBaseStdCancelorderRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新立库取消订单
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseStdCancelorderUpInput input)
        {
            var entity = input.Adapt<ZjnBaseStdCancelorderEntity>();
            var isOk = await _zjnBaseStdCancelorderRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除立库取消订单
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseStdCancelorderRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseStdCancelorderRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


