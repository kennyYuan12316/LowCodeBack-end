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
using HSZ.wms.Entitys.Dto.ZjnWcsWorkPath;
using HSZ.wms.Interfaces.ZjnWcsWorkPath;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWcsWorkPath
{
    /// <summary>
    /// 路径信息管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWcsWorkPath", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWcsWorkPathService : IZjnWcsWorkPathService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsWorkPathEntity> _zjnWcsWorkPathRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;

        /// <summary>
        /// 初始化一个<see cref="ZjnWcsWorkPathService"/>类型的新实例
        /// </summary>
        public ZjnWcsWorkPathService(ISqlSugarRepository<ZjnWcsWorkPathEntity> zjnWcsWorkPathRepository,
            IUserManager userManager,
            ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository)
        {
            _zjnWcsWorkPathRepository = zjnWcsWorkPathRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
        }

        /// <summary>
        /// 判断路径编号是否存在
        /// </summary>
        /// <param name="PathId"></param>
        /// <returns></returns>
        [HttpGet("ExistPathId")]
        public async Task<bool> ExistPathId(string PathId)
        {
            var output = await _zjnWcsWorkPathRepository.IsAnyAsync(p => p.PathId == PathId && p.IsDelete == 0);
            return output;
        }

        /// <summary>
        /// 获取路径信息管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWcsWorkPathRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWcsWorkPathInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取路径信息管理列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWcsWorkPathListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWcsWorkPathRepository.AsSugarClient().Queryable<ZjnWcsWorkPathEntity>()
                .Where(x => x.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.PathID), a => a.PathId.Contains(input.PathID))
                .WhereIF(!string.IsNullOrEmpty(input.StationFrom), a => a.StationFrom.Contains(input.StationFrom))
                .WhereIF(!string.IsNullOrEmpty(input.PathType), a => a.PathType.Equals(input.PathType))
                .WhereIF(!string.IsNullOrEmpty(input.Region), a => a.Region.Equals(input.Region))
                .Select((a
)=> new ZjnWcsWorkPathListOutput
                {
                    F_Id = a.Id,
                    PathID = a.PathId,
                    StationFrom = a.StationFrom,
                    DeviceFrom=a.DeviceFrom,
                    DeviceFromName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.DeviceFrom).Select(s => s.Capion),
                    StationTo=a.StationTo,
                    StationToName = SqlFunc.Subqueryable<ZjnWcsWorkDeviceEntity>().Where(s => s.DeviceId == a.StationTo).Select(s => s.Caption) ,
                    DeviceTo=a.DeviceTo,
                    DeviceToName = SqlFunc.Subqueryable<ZjnWcsWorkSiteEntity>().Where(s => s.StationId == a.DeviceTo).Select(s => s.Capion),
                    PathType = a.PathType,
                    IsActive = SqlFunc.IIF(a.IsActive == 0, "关", "开"),
                    Region = a.Region,
                    Descrip = a.Descrip,
    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName),
    F_CreateTime = a.CreateTime,
    F_ModifiedUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.ModifiedUser).Select(s => s.RealName),
    F_ModifiedTime = a.ModifiedTime,
}).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWcsWorkPathListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建路径信息管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWcsWorkPathCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWcsWorkPathEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            entity.IsDelete = 0;
            var isOk = await _zjnWcsWorkPathRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新路径信息管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWcsWorkPathUpInput input)
        {
            var entity = input.Adapt<ZjnWcsWorkPathEntity>();
            entity.ModifiedUser = _userManager.UserId;
            entity.ModifiedTime = DateTime.Now;
            var isOk = await _zjnWcsWorkPathRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除路径信息管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            //var entity = await _zjnWcsWorkPathRepository.GetFirstAsync(p => p.Id.Equals(id));
            //_ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //var isOk = await _zjnWcsWorkPathRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            //if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);

            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            var entity = await _zjnWcsWorkPathRepository.GetFirstAsync(p => p.Id.Equals(id));

            operationLogEntity.BeforeDate = entity.ToJson();//修改前的数据
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //取消数据状态
            entity.IsDelete = 1;
            //添加日志
            operationLogEntity.CreateUser = _userManager.UserId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "路径信息管理删除";
            operationLogEntity.AfterDate = entity.ToJson();//修改后的数据            
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 2;
            operationLogEntity.WorkPath = 2;
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnWcsWorkPathRepository.AsUpdateable(entity).ExecuteCommandAsync();
                //新增日子记录
                var isOk1 = await _zjnPlaneOperationLogRepository.AsInsertable(operationLogEntity).ExecuteReturnEntityAsync();

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


