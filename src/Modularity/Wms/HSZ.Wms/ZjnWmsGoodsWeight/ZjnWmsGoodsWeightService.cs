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
using HSZ.wms.Entitys.Dto.ZjnWmsGoodsWeight;
using HSZ.wms.Interfaces.ZjnWmsGoodsWeight;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsGoodsWeight
{
    /// <summary>
    /// 物料承重配置服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsGoodsWeight", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsGoodsWeightService : IZjnWmsGoodsWeightService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsGoodsWeightEntity> _zjnWmsGoodsWeightRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsGoodsWeightService"/>类型的新实例
        /// </summary>
        public ZjnWmsGoodsWeightService(ISqlSugarRepository<ZjnWmsGoodsWeightEntity> zjnWmsGoodsWeightRepository,
            IUserManager userManager)
        {
            _zjnWmsGoodsWeightRepository = zjnWmsGoodsWeightRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取物料承重配置
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWmsGoodsWeightRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsGoodsWeightInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取物料承重配置列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsGoodsWeightListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_GoodsCode" : input.sidx;
            List<object> queryMax = input.F_Max != null ? input.F_Max.Split(',').ToObject<List<object>>() : null;
            var startMax = input.F_Max != null && !string.IsNullOrEmpty(queryMax.First().ToString()) ? queryMax.First() : decimal.MinValue;
            var endMax = input.F_Max != null && !string.IsNullOrEmpty(queryMax.Last().ToString()) ? queryMax.Last() : decimal.MaxValue;
            var data = await _zjnWmsGoodsWeightRepository.AsSugarClient().Queryable<ZjnWmsGoodsWeightEntity>()
                .Where(a => a.IsDelete == 0)
                .WhereIF(queryMax != null, a => SqlFunc.Between(a.Max, startMax, endMax))
                .WhereIF(!string.IsNullOrEmpty(input.F_Unit), a => a.Unit.Equals(input.F_Unit))
                .Select((a
) => new ZjnWmsGoodsWeightListOutput
{
    F_Id = a.Id,
    F_GoodsCode = a.GoodsCode,
    F_Min = a.Min,
    F_Max = a.Max,
    F_Unit = a.Unit,
}).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWmsGoodsWeightListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建物料承重配置
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsGoodsWeightCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsGoodsWeightEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;

            var isOk = await _zjnWmsGoodsWeightRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新物料承重配置
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsGoodsWeightUpInput input)
        {
            var entity = input.Adapt<ZjnWmsGoodsWeightEntity>();
            var isOk = await _zjnWmsGoodsWeightRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除物料承重配置
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWmsGoodsWeightRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            entity.IsDelete = 1;
            var isOk = await _zjnWmsGoodsWeightRepository.AsUpdateable(entity).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


