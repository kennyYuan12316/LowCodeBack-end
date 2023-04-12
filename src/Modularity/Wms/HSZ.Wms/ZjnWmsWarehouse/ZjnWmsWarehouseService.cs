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
using HSZ.wms.Entitys.Dto.ZjnWmsWarehouse;
using HSZ.wms.Interfaces.ZjnWmsWarehouse;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsWarehouse
{
    /// <summary>
    /// 仓库信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsWarehouse", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsWarehouseService : IZjnWmsWarehouseService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsWarehouseEntity> _zjnBaseWarehouseRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsWarehouseService"/>类型的新实例
        /// </summary>
        public ZjnWmsWarehouseService(ISqlSugarRepository<ZjnWmsWarehouseEntity> zjnBaseWarehouseRepository,
            IUserManager userManager)
        {
            _zjnBaseWarehouseRepository = zjnBaseWarehouseRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseWarehouseRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsWarehouseInfoOutput>();
            return output;
        }

        /// <summary>
        /// 判断仓库编号是否存在
        /// </summary>
        /// <param name="WarehouseNo"></param>
        /// <returns></returns>
        [HttpGet("ExistWarehouseNo")]
        public async Task<bool> ExistWarehouseNo(string WarehouseNo)
        {
            var output = await _zjnBaseWarehouseRepository.IsAnyAsync(p => p.WarehouseNo == WarehouseNo);
            return output;
        }

        /// <summary>
        /// 获取仓库信息 -- APP使用
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("GetInfoToApp")]
        public async Task<dynamic> GetInfoToApp(string id)
        {
            var output = (await _zjnBaseWarehouseRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsWarehouseInfoOutput>();
            if (output == null)
            {
                return (await _zjnBaseWarehouseRepository.GetFirstAsync(p => p.WarehouseNo == id)).Adapt<ZjnWmsWarehouseInfoOutput>();
            }

            return output;
        }

        // <summary>
        /// 获取仓库信息列表（下拉框使用）--by ljt
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("GetHouseListToBox")]
        public async Task<List<ZjnWmsWarehouseEntity>> GetHouseListToBox()
        {
            return await _zjnBaseWarehouseRepository.AsQueryable().Where(w => w.IsDelete == 0).OrderBy(x => x.CreateTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
		/// 获取仓库信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsWarehouseListQueryInput input)
        {
            var sidx = input.sidx == null ? "a.F_Id" : input.sidx;
            var data = await _zjnBaseWarehouseRepository.AsSugarClient().Queryable<ZjnWmsWarehouseEntity>()
                .LeftJoin<ZjnWmsRegionEntity>((a, i) => a.RegionNo == i.RegionNo)
                .WhereIF(!string.IsNullOrEmpty(input.F_WarehouseNo), a => a.WarehouseNo.Contains(input.F_WarehouseNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_WarehouseName), a => a.WarehouseName.Contains(input.F_WarehouseName))
                .WhereIF(!string.IsNullOrEmpty(input.F_RegionNo), a => a.RegionNo.Equals(input.F_RegionNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Where((a, i) => a.IsDelete == 0)
                .Select((a, i) => new ZjnWmsWarehouseListOutput
                {
                    F_Id = a.Id,
                    F_WarehouseNo = a.WarehouseNo,
                    F_WarehouseName = a.WarehouseName,
                    F_RegionNo = a.RegionNo,
                    F_RegionName = i.RegionName,
                    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName),
                    F_CreateTime = a.CreateTime,
                    F_EnabledMark = a.EnabledMark,
                })
                .OrderBy(a => a.F_Id, input.sort == "desc" ? OrderByType.Desc : OrderByType.Asc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWmsWarehouseListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建仓库信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsWarehouseCrInput input)
        {
            if (await this.ExistWarehouseNo(input.warehouseNo)) throw HSZException.Oh(ErrorCode.COM1004);

            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsWarehouseEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            entity.IsDelete = 0;
            var isOk = await _zjnBaseWarehouseRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 批量删除仓库信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseWarehouseRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            List<ZjnWmsWarehouseEntity> warehouseEntityList = new List<ZjnWmsWarehouseEntity>();
            if (entitys.Count > 0)
            {


                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除仓库信息
                    foreach (var item in entitys)
                    {
                        ZjnWmsWarehouseEntity warehouseEntity = new ZjnWmsWarehouseEntity();
                        warehouseEntity = item;
                        warehouseEntity.IsDelete = 1;
                        await _zjnBaseWarehouseRepository.AsUpdateable(warehouseEntity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                    }
                    //await _zjnBaseWarehouseRepository.AsDeleteable().In(d => d.Id, ids).ExecuteCommandAsync();
                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception ex)
                {
                    //回滚事务
                    _db.RollbackTran();
                    throw HSZException.Oh(ErrorCode.COM1002);
                }
            }
        }

        /// <summary>
        /// 更新仓库信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsWarehouseUpInput input)
        {
            var entity = input.Adapt<ZjnWmsWarehouseEntity>();
            var isOk = await _zjnBaseWarehouseRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除仓库信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseWarehouseRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            entity.IsDelete = 1;
            var isOk = await _zjnBaseWarehouseRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            //var isOk = await _zjnBaseWarehouseRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


