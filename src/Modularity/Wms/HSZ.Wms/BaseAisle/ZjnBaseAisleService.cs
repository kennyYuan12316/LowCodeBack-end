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
using HSZ.System.Entitys.Permission;
using HSZ.wms.Entitys.Dto.ZjnBaseAisle;
using HSZ.wms.Entitys.Dto.ZjnWmsAisle;
using HSZ.wms.Interfaces.ZjnBaseAisle;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnBaseAisle
{
    /// <summary>
    /// 巷道信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseAisle", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseAisleService : IZjnBaseAisleService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseAisleEntity> _zjnBaseAisleRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseAisleService"/>类型的新实例
        /// </summary>
        public ZjnBaseAisleService(ISqlSugarRepository<ZjnBaseAisleEntity> zjnBaseAisleRepository,
            IUserManager userManager)
        {
            _zjnBaseAisleRepository = zjnBaseAisleRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取巷道信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseAisleRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseAisleInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取巷道信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseAisleListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnBaseAisleRepository.AsSugarClient().Queryable<ZjnBaseAisleEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_AisleNo), a => a.AisleNo.Contains(input.F_AisleNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_AisleName), a => a.AisleName.Contains(input.F_AisleName))
                .WhereIF(!string.IsNullOrEmpty(input.F_RegionNo), a => a.RegionNo.Equals(input.F_RegionNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_WarehouseNo), a => a.WarehouseNo.Equals(input.F_WarehouseNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_StackerNo), a => a.StackerNo.Equals(input.F_StackerNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Where(a=> a.IsDelete == 0)
                .Select((a
)=> new ZjnBaseAisleListOutput
                {
                    F_Id = a.Id,
                    F_AisleNo = a.AisleNo,
                    F_AisleName = a.AisleName,
                    F_RegionNo = a.RegionNo,
                    F_WarehouseNo = a.WarehouseNo,
                    F_StackerNo = a.StackerNo,
                    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName),//a.CreateUser,
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = a.EnabledMark,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseAisleListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建巷道信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsAisleCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseAisleEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            entity.IsDelete = 0;
            var isOk = await _zjnBaseAisleRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 批量删除巷道信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseAisleRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    foreach (var item in entitys)
                    {
                        ZjnBaseAisleEntity entity = new ZjnBaseAisleEntity();
                        entity = item;
                        entity.IsDelete = 1;
                        await _zjnBaseAisleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                    }

                    //批量删除巷道信息
                   // await _zjnBaseAisleRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
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
        /// 更新巷道信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseAisleUpInput input)
        {
            var entity = input.Adapt<ZjnBaseAisleEntity>();
            var isOk = await _zjnBaseAisleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除巷道信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseAisleRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            entity.IsDelete = 1;
            var isOk = await _zjnBaseAisleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            //var isOk = await _zjnBaseAisleRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


