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
using HSZ.wms.Entitys.Dto.ZjnWmsTask;
using HSZ.wms.Interfaces.ZjnBaseAisle;
using HSZ.wms.Interfaces.ZjnWmsTask;
using HSZ.wms.ZjnWmsTask;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsAisle
{

    /// <summary>
    /// 消防实体-收
    /// </summary>
    public class resultMessige
    {
        //空位编码 
        public string locationCode { get; set; }
        //报警代码 1.报警 99.心跳
        public int status { get; set; }
    }

    /// <summary>
    /// 消防实体-发
    /// </summary>
    public class sendMessige
    {
        //状态  0成功  1.失败
        public int code { get; set; }
        //说明
        public string msg { get; set; }

    }

    /// <summary>
    /// 巷道信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsAisle", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsAisleService : IZjnBaseAisleService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsAisleEntity> _zjnWmsAisleRepository;
        private readonly ISqlSugarRepository<ZjnWmsTaskDetailsEntity> _zjnWmsTaskDetailsRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private IZjnWmsTaskService _ZjnTaskService;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsAisleService"/>类型的新实例
        /// </summary>
        public ZjnWmsAisleService(ISqlSugarRepository<ZjnWmsAisleEntity> zjnWmsAisleRepository,
            ISqlSugarRepository<ZjnWmsTaskDetailsEntity> zjnWmsTaskDetailsRepository,
            IZjnWmsTaskService IZjnWmsTaskService,
            IUserManager userManager)
        {
            _zjnWmsAisleRepository = zjnWmsAisleRepository;
            _zjnWmsTaskDetailsRepository = zjnWmsTaskDetailsRepository;
            _userManager = userManager;
            _ZjnTaskService = IZjnWmsTaskService;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }


        /// <summary>
        /// 消防任务 由DTS主动发起
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost("FireWarning")]
        [AllowAnonymous]
        public async Task<sendMessige> FireWarning(string json)
        {
            sendMessige send = new sendMessige();
            try
            {
                resultMessige resultMessige = json.Deserialize<resultMessige>(json);

                if (resultMessige.status == 99)//心跳
                {
                    send.code = 0;
                    send.msg = "调用成功！";
                    return send;
                }
                else if (resultMessige.status == 1)//报警
                {
                    if (string.IsNullOrEmpty(resultMessige.locationCode) || !resultMessige.locationCode.Contains("-"))
                    {
                        send.code = -1;
                        send.msg = "传递的货位参数不正确，请检查！";
                        return send;
                    }
                    //查找该货位是否已经产生了报警任务
                    var entity = await _zjnWmsTaskDetailsRepository.GetFirstAsync(p => p.TaskDetailsStart == resultMessige.locationCode && p.TaskType == 10 && p.TaskDetailsStates < 3);
                    if (entity != null)
                    {
                        send.code = 0;
                        send.msg = "";
                        return send;
                    }
                    //产生任务
                    ZjnWmsTaskCrInput taskInput = new ZjnWmsTaskCrInput();
                    taskInput.positionFrom = resultMessige.locationCode;
                    taskInput.positionTo = "";
                    taskInput.operationDirection = "Move";
                    taskInput.priority = 9999;
                    string ids = "361800011803002117";//消防任务ID

                    ZjnWmsTaskCrInput res = await _ZjnTaskService.CreateByConfigId(ids, taskInput);
                    if (res == null)
                    {
                        send.code = -1;
                        send.msg = "消防任务创建失败，请人工排查";
                        return send;
                    }
                    send.code = 0;
                    send.msg = "消防任务创建成功!";
                    return send;
                }
                else
                {
                    send.code = -1;
                    send.msg = "参数不明确，无法执行业务！";
                    return send;
                }

            }
            catch (Exception e)
            {
                send.code = -1;
                send.msg = e.Message;
                return send;
            }

        }

        /// <summary>
        /// 获取巷道信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWmsAisleRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsAisleInfoOutput>();
            return output;
        }

        /// <summary>
        /// 判断巷道编号是否存在
        /// </summary>
        /// <param name="AisleNo"></param>
        /// <returns></returns>
        [HttpGet("ExistAisleNo")]
        public async Task<bool> ExistAisleNo(string AisleNo)
        {
            var output = await _zjnWmsAisleRepository.IsAnyAsync(p => p.AisleNo == AisleNo && p.IsDelete == 0);
            return output;
        }

        /// <summary>
		/// 获取巷道信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsAisleListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWmsAisleRepository.AsSugarClient().Queryable<ZjnWmsAisleEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_AisleNo), a => a.AisleNo.Contains(input.F_AisleNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_AisleName), a => a.AisleName.Contains(input.F_AisleName))
                .WhereIF(!string.IsNullOrEmpty(input.F_RegionNo), a => a.RegionNo.Equals(input.F_RegionNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_WarehouseNo), a => a.WarehouseNo.Equals(input.F_WarehouseNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_StackerNo), a => a.StackerNo.Equals(input.F_StackerNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Where(a => a.IsDelete == 0)
                .Select((a
) => new ZjnWmsAisleListOutput
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
}).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWmsAisleListOutput>.SqlSugarPageResult(data);
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
            var entity = input.Adapt<ZjnWmsAisleEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = _userManager.UserId;
            entity.CreateTime = DateTime.Now;
            entity.IsDelete = 0;
            var isOk = await _zjnWmsAisleRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
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
            var entitys = await _zjnWmsAisleRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    foreach (var item in entitys)
                    {
                        ZjnWmsAisleEntity entity = new ZjnWmsAisleEntity();
                        entity = item;
                        entity.IsDelete = 1;
                        await _zjnWmsAisleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
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
        public async Task Update(string id, [FromBody] ZjnWmsAisleUpInput input)
        {
            var entity = input.Adapt<ZjnWmsAisleEntity>();
            var isOk = await _zjnWmsAisleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除巷道信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnWmsAisleRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            entity.IsDelete = 1;
            var isOk = await _zjnWmsAisleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            //var isOk = await _zjnBaseAisleRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


