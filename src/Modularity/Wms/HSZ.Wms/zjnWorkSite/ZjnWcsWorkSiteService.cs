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
using HSZ.wms.Entitys.Dto.ZjnWcsWorkSite;
using HSZ.wms.Interfaces.ZjnWcsWorkSite;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWcsWorkSite
{
    /// <summary>
    /// 站点信息管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWcsWorkSite", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWcsWorkSiteService : IZjnWcsWorkSiteService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsWorkSiteEntity> _zjnWcsWorkSiteRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;

        /// <summary>
        /// 初始化一个<see cref="ZjnWcsWorkSiteService"/>类型的新实例
        /// </summary>
        public ZjnWcsWorkSiteService(ISqlSugarRepository<ZjnWcsWorkSiteEntity> zjnWcsWorkSiteRepository,
            IUserManager userManager,
            ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository)
        {
            _zjnWcsWorkSiteRepository = zjnWcsWorkSiteRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
        }

        /// <summary>
        /// 判断站点编号是否存在
        /// </summary>
        /// <param name="StationId"></param>
        /// <returns></returns>
        [HttpGet("ExistStationId")]
        public async Task<bool> ExistStationId(string StationId)
        {
            var output = await _zjnWcsWorkSiteRepository.IsAnyAsync(p => p.StationId == StationId && p.IsDelete == 0);
            return output;
        }

        /// <summary>
        /// 获取站点信息管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWcsWorkSiteRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWcsWorkSiteInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取站点信息管理列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWcsWorkSiteListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWcsWorkSiteRepository.AsSugarClient().Queryable<ZjnWcsWorkSiteEntity>()
                .Where(x => x.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.StationID), a => a.StationId.Contains(input.StationID))
                .WhereIF(!string.IsNullOrEmpty(input.Capion), a => a.Capion.Contains(input.Capion))
                .Select((a
)=> new ZjnWcsWorkSiteListOutput
                {
                    F_Id = a.Id,
                    StationID = a.StationId,
                    Capion = a.Capion,
    DeviceID=a.DeviceId,
    DeviceName = SqlFunc.Subqueryable<ZjnWcsWorkDeviceEntity>().Where(s => s.DeviceId == a.DeviceId).Select(s => s.Caption),
                    IsActive = SqlFunc.IIF(a.IsActive == false, "关", "开"),
                    Region = a.Region,
                    Descrip = a.Descrip,
                    Row=a.Row,
                    Cell=a.Cell,
                    Layer=a.Layer,
                    Row2=a.Row2,
                    Cell2=a.Cell2,
                    Layer2=a.Layer2,
                    StationType = a.StationType,
    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName),
    F_CreateTime = a.CreateTime,
    F_ModifiedUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.ModifiedUser).Select(s => s.RealName),
    F_ModifiedTime = a.ModifiedTime,
}).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWcsWorkSiteListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建站点信息管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWcsWorkSiteCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWcsWorkSiteEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            entity.IsDelete = 0;
            var isOk = await _zjnWcsWorkSiteRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新站点信息管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWcsWorkSiteUpInput input)
        {
            var entity = input.Adapt<ZjnWcsWorkSiteEntity>();
            entity.ModifiedUser = _userManager.UserId;
            entity.ModifiedTime = DateTime.Now;
            var isOk = await _zjnWcsWorkSiteRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除站点信息管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            //var entity = await _zjnWcsWorkSiteRepository.GetFirstAsync(p => p.Id.Equals(id));
            //_ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //var isOk = await _zjnWcsWorkSiteRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            //if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);

            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            var entity = await _zjnWcsWorkSiteRepository.GetFirstAsync(p => p.Id.Equals(id));

            operationLogEntity.BeforeDate = entity.ToJson();//修改前的数据
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //取消数据状态
            entity.IsDelete = 1;
            //添加日志
            operationLogEntity.CreateUser = _userManager.UserId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "站点信息管理删除";
            operationLogEntity.AfterDate = entity.ToJson();//修改后的数据            
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 2;
            operationLogEntity.WorkPath = 2;
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnWcsWorkSiteRepository.AsUpdateable(entity).ExecuteCommandAsync();
                //新增日子记录
                //var isOk1 = await _zjnPlaneOperationLogRepository.AsInsertable(operationLogEntity).ExecuteReturnEntityAsync();

                _db.CommitTran();
            }
            catch (Exception e)
            {
                string es = e.Message;
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);

            }
        }
    }
}


