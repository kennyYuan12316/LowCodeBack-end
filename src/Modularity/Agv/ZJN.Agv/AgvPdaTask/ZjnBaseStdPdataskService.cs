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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Agv.Entitys.Dto.AgvPdaTask;
using ZJN.Agv.Entitys.Entity;
using ZJN.Agv.Interfaces;

namespace ZJN.Agv.AgvPdaTask
{
    /// <summary>
    /// Agv上传PDA任务服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "Agv",Name = "ZjnBaseStdPdatask", Order = 200)]
    [Route("api/agv/[controller]")]
    public class ZjnBaseStdPdataskService : IZjnBaseStdPdataskService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseStdPdataskEntity> _zjnBaseStdPdataskRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseStdPdataskService"/>类型的新实例
        /// </summary>
        public ZjnBaseStdPdataskService(ISqlSugarRepository<ZjnBaseStdPdataskEntity> zjnBaseStdPdataskRepository,
            IUserManager userManager)
        {
            _zjnBaseStdPdataskRepository = zjnBaseStdPdataskRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取Agv上传PDA任务
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseStdPdataskRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseStdPdataskInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取Agv上传PDA任务列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseStdPdataskListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            List<string> queryRequestTime = input.F_RequestTime != null ? input.F_RequestTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startRequestTime = queryRequestTime != null ? Ext.GetDateTime(queryRequestTime.First()) : null;
            DateTime? endRequestTime = queryRequestTime != null ? Ext.GetDateTime(queryRequestTime.Last()) : null;
            List<object> queryTaskType = input.F_TaskType != null ? input.F_TaskType.Split(',').ToObject<List<object>>() : null;
            var startTaskType = input.F_TaskType != null && !string.IsNullOrEmpty(queryTaskType.First().ToString()) ? queryTaskType.First() : decimal.MinValue;
            var endTaskType = input.F_TaskType != null && !string.IsNullOrEmpty(queryTaskType.Last().ToString()) ? queryTaskType.Last() : decimal.MaxValue;
            var data = await _zjnBaseStdPdataskRepository.AsSugarClient().Queryable<ZjnBaseStdPdataskEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_RequestId), a => a.RequestId.Contains(input.F_RequestId))
                .WhereIF(queryRequestTime != null, a => a.RequestTime >= new DateTime(startRequestTime.ToDate().Year, startRequestTime.ToDate().Month, startRequestTime.ToDate().Day, startRequestTime.ToDate().Hour, startRequestTime.ToDate().Minute, startRequestTime.ToDate().Second))
                .WhereIF(queryRequestTime != null, a => a.RequestTime <= new DateTime(endRequestTime.ToDate().Year, endRequestTime.ToDate().Month, endRequestTime.ToDate().Day, endRequestTime.ToDate().Hour, endRequestTime.ToDate().Minute, endRequestTime.ToDate().Second))
                .WhereIF(queryTaskType != null, a => SqlFunc.Between(a.TaskType, startTaskType, endTaskType))
                .WhereIF(!string.IsNullOrEmpty(input.F_StartAreaCode), a => a.StartAreaCode.Contains(input.F_StartAreaCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_StartLocCode), a => a.StartLocCode.Contains(input.F_StartLocCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_EndAreaCode), a => a.EndAreaCode.Contains(input.F_EndAreaCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_EndLocCode), a => a.EndLocCode.Contains(input.F_EndLocCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsCode), a => a.GoodsCode.Contains(input.F_GoodsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_ContainerCode), a => a.ContainerCode.Contains(input.F_ContainerCode))
                .Select((a
)=> new ZjnBaseStdPdataskListOutput
                {
                    F_Id = a.Id,
                    F_RequestId = a.RequestId,
                    F_ClientCode = a.ClientCode,
                    F_ChannelId = a.ChannelId,
                    F_RequestTime = a.RequestTime,
                    F_TaskType = a.TaskType,
                    F_StartAreaCode = a.StartAreaCode,
                    F_StartLocCode = a.StartLocCode,
                    F_EndAreaCode = a.EndAreaCode,
                    F_EndLocCode = a.EndLocCode,
                    F_GoodsCode = a.GoodsCode,
                    F_ContainerCode = a.ContainerCode,
                    F_Quantity = a.Quantity,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseStdPdataskListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建Agv上传PDA任务
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        [AllowAnonymous]
        public async Task<dynamic> Create([FromBody] ZjnBaseStdPdataskCrInput input)
        {
            //var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseStdPdataskEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateTime = DateTime.Now;
            
            var isOk = await _zjnBaseStdPdataskRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            RESTfulResult_AgvPdaTask result_AgvPdaTask = new RESTfulResult_AgvPdaTask();
            if (!(isOk > 0))
            {
                throw HSZException.Oh(ErrorCode.COM1000);
            }
            else
            {
                result_AgvPdaTask.Code = "200";
                result_AgvPdaTask.Msg = "Success";
            }
            return result_AgvPdaTask;
        }

        /// <summary>
        /// 更新Agv上传PDA任务
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseStdPdataskUpInput input)
        {
            var entity = input.Adapt<ZjnBaseStdPdataskEntity>();
            var isOk = await _zjnBaseStdPdataskRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除Agv上传PDA任务
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseStdPdataskRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseStdPdataskRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


