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
using HSZ.wms.Entitys.Dto.ZjnWcsWorkDevice;
using HSZ.wms.Interfaces.ZjnWcsWorkDevice;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Entitys.wcs;

namespace HSZ.wms.ZjnWcsWorkDevice
{
    /// <summary>
    /// 设备信息管理服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWcsWorkDevice", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWcsWorkDeviceService : IZjnWcsWorkDeviceService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWcsWorkDeviceEntity> _zjnWcsWorkDeviceRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;

        /// <summary>
        /// 初始化一个<see cref="ZjnWcsWorkDeviceService"/>类型的新实例
        /// </summary>
        public ZjnWcsWorkDeviceService(ISqlSugarRepository<ZjnWcsWorkDeviceEntity> zjnWcsWorkDeviceRepository,
            IUserManager userManager,
            ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository)
        {
            _zjnWcsWorkDeviceRepository = zjnWcsWorkDeviceRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
        }


        /// <summary>
        /// 判断设备编号是否存在
        /// </summary>
        /// <param name="DeviceId"></param>
        /// <returns></returns>
        [HttpGet("ExistDeviceId")]
        public async Task<bool> ExistDeviceId(string DeviceId)
        {
            var output = await _zjnWcsWorkDeviceRepository.IsAnyAsync(p => p.DeviceId == DeviceId && p.IsDelete == 0);
            return output;
        }

        /// <summary>
        /// 获取堆垛机列表（下拉框使用）--by ljt
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetStackerListToBox")]
        public async Task<List<ZjnWcsWorkDeviceEntity>> GetStackerListToBox()
        {
            return await _zjnWcsWorkDeviceRepository.AsQueryable().Where(q => new string[] { "1", "7" }.Contains(q.DeviceType) && q.IsActive == 1 && q.IsDelete == 0).OrderBy(x => x.CreateTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 获取设备信息管理
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWcsWorkDeviceRepository.GetFirstAsync(p => p.DeviceId == id)).Adapt<ZjnWcsWorkDeviceInfoOutput>();
            return output;
        }

        /// <summary>
        /// 获取设备信息管理列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWcsWorkDeviceListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWcsWorkDeviceRepository.AsSugarClient().Queryable<ZjnWcsWorkDeviceEntity>()
                          
                .Where(a => a.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.DeviceID), a => a.DeviceId.Contains(input.DeviceID))
                .WhereIF(!string.IsNullOrEmpty(input.caption), a => a.Caption.Contains(input.caption))
                .WhereIF(!string.IsNullOrEmpty(input.DeviceType), a => a.DeviceType.Equals(input.DeviceType))
                .Select((a
) => new ZjnWcsWorkDeviceListOutput
{
    F_Id = a.Id,
    DeviceID = a.DeviceId,
    Caption = a.Caption,
    IsActive = SqlFunc.IIF(a.IsActive == 0, "关", "开"),
    Region = a.Region,
    IsController = SqlFunc.IIF(a.IsController == "0", "关", "开"),
    ControllerType = a.ControllerType,
    IsAlone = SqlFunc.IIF(a.IsAlone == 0, "关", "开"),
    ThreadGroup = a.ThreadGroup,
    Descrip = a.Descrip,
    DeviceType = a.DeviceType,
    F_PlcID = a.PlcID,
    F_CreateUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.CreateUser).Select(s => s.RealName),
    F_CreateTime = a.CreateTime,
    F_ModifiedUser = SqlFunc.Subqueryable<UserEntity>().Where(s => s.Id == a.ModifiedUser).Select(s => s.RealName),
    F_ModifiedTime = a.ModifiedTime,
    StackerGroup= SqlFunc.Subqueryable<ZjnWcsPlcObjectEntity>().Where(s => s.PlcId == a.PlcID&&!string.IsNullOrEmpty(s.StackerGroup)).Select(s => s.StackerGroup),
}).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWcsWorkDeviceListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建设备信息管理
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWcsWorkDeviceCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWcsWorkDeviceEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            entity.IsDelete = 0;
            var isOk = await _zjnWcsWorkDeviceRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新设备信息管理
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWcsWorkDeviceUpInput input)
        {
            var entity = input.Adapt<ZjnWcsWorkDeviceEntity>();
            entity.ModifiedUser = _userManager.UserId;
            entity.ModifiedTime = DateTime.Now;
            var isOk = await _zjnWcsWorkDeviceRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除设备信息管理
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            //var entity = await _zjnWcsWorkDeviceRepository.GetFirstAsync(p => p.Id.Equals(id));
            //_ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //var isOk = await _zjnWcsWorkDeviceRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            //if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);

            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            var entity = await _zjnWcsWorkDeviceRepository.GetFirstAsync(p => p.Id.Equals(id));

            operationLogEntity.BeforeDate = entity.ToJson();//修改前的数据
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            //取消数据状态
            entity.IsDelete = 1;
            //添加日志
            operationLogEntity.CreateUser = _userManager.UserId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "设备管理信息删除";
            operationLogEntity.AfterDate = entity.ToJson();//修改后的数据            
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 2;
            operationLogEntity.WorkPath = 2;
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnWcsWorkDeviceRepository.AsUpdateable(entity).ExecuteCommandAsync();
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


