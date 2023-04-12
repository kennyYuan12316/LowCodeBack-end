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
using HSZ.wms.Entitys.Dto.ZjnWmsRegion;
using HSZ.wms.Interfaces.ZjnWmsRegion;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsRegion
{
    /// <summary>
    /// 区域信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWmsRegion", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsRegionService : IZjnWmsRegionService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsRegionEntity> _zjnWmsRegionRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsRegionService"/>类型的新实例
        /// </summary>
        public ZjnWmsRegionService(ISqlSugarRepository<ZjnWmsRegionEntity> zjnBaseRegionRepository,
            IUserManager userManager)
        {
            _zjnWmsRegionRepository = zjnBaseRegionRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取区域信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWmsRegionRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsRegionInfoOutput>();
            return output;
        }

        /// <summary>
        /// 判断区域编号是否存在
        /// </summary>
        /// <param name="RegionNo"></param>
        /// <returns></returns>
        [HttpGet("ExistRegionNo")]
        public async Task<bool> ExistRegionNo(string RegionNo)
        {
            var output = await _zjnWmsRegionRepository.IsAnyAsync(p => p.RegionNo == RegionNo);
            return output;
        }

        // <summary>
        /// 获取区域信息列表（下拉框使用）--by ljt
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("GetListToBox")]
        public async Task<List<ZjnWmsRegionEntity>> GetListToBox()
        {
            return await _zjnWmsRegionRepository.AsQueryable().OrderBy(x => x.CreateTime, OrderByType.Desc).ToListAsync();
        }


        /// <summary>
		/// 获取区域信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsRegionListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWmsRegionRepository.AsSugarClient().Queryable<ZjnWmsRegionEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_RegionNo), a => a.RegionNo.Contains(input.F_RegionNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_RegionName), a => a.RegionName.Contains(input.F_RegionName))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Select((a
)=> new ZjnWmsRegionListOutput
                {
                    F_Id = a.Id,
                    F_RegionNo = a.RegionNo,
                    F_RegionName = a.RegionName,
                    F_CreateUser = a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = a.EnabledMark,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWmsRegionListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建区域信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsRegionCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsRegionEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            
            var isOk = await _zjnWmsRegionRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 批量删除区域信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnWmsRegionRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除区域信息
                    await _zjnWmsRegionRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception)
                {
                    //回滚事务
                    _db.RollbackTran();
                    throw HSZException.Oh(ErrorCode.COM1002);
                }
            }
        }

        /// <summary>
        /// 更新区域信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsRegionUpInput input)
        {
            var entity = input.Adapt<ZjnWmsRegionEntity>();
            var isOk = await _zjnWmsRegionRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除区域信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWmsRegionRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnWmsRegionRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


