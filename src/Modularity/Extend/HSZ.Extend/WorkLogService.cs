using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Extend.Entitys;
using HSZ.Extend.Entitys.Dto.WoekLog;
using HSZ.Extend.Entitys.Dto.WorkLog;
using HSZ.FriendlyException;
using HSZ.System.Interfaces.Permission;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.Extend
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：工作日志
    /// </summary>
    [ApiDescriptionSettings(Tag = "Extend", Name = "WorkLog", Order = 600)]
    [Route("api/extend/[controller]")]
    public class WorkLogService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<WorkLogEntity> _workLogRepository;
        private readonly IUsersService _usersService;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope db;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workLogRepository"></param>
        /// <param name="usersService"></param>
        /// <param name="userManager"></param>
        public WorkLogService(ISqlSugarRepository<WorkLogEntity> workLogRepository, IUsersService usersService, IUserManager userManager)
        {
            _workLogRepository = workLogRepository;
            _usersService = usersService;
            _userManager = userManager;
            db = DbScoped.SugarScope;
        }

        #region Get
        /// <summary>
        /// 列表(我发出的)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Send")]
        public async Task<dynamic> GetSendList([FromQuery] PageInputBase input)
        {
            var list = await _workLogRepository.AsQueryable().Where(x => x.CreatorUserId == _userManager.UserId && x.DeleteMark == null)
                .WhereIF(input.keyword.IsNotEmptyOrNull(), m => m.Title.Contains(input.keyword) || m.Description.Contains(input.keyword))
                .OrderBy(x => x.SortCode).OrderBy(x => x.CreatorTime, OrderByType.Desc)
                .OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.LastModifyTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<WorkLogListOutput>()
            {
                list = list.list.Adapt<List<WorkLogListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<WorkLogListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 列表(我收到的)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Receive")]
        public async Task<dynamic> GetReceiveList([FromQuery] PageInputBase input)
        {
            var list = await _workLogRepository.AsSugarClient().Queryable<WorkLogEntity, WorkLogShareEntity>(
                (a, b) => new JoinQueryInfos(JoinType.Left, a.Id == b.WorkLogId))
                .Where((a, b) => a.DeleteMark == null && b.ShareUserId == _userManager.UserId)
                .WhereIF(input.keyword.IsNotEmptyOrNull(), a => a.Title.Contains(input.keyword))
                .Select(a => new WorkLogListOutput() {
                    id = a.Id,
                    title = a.Title,
                    question = a.Question,
                    creatorTime = a.CreatorTime,
                    todayContent = a.TodayContent,
                    tomorrowContent = a.TomorrowContent,
                    toUserId = a.ToUserId,
                    sortCode = a.SortCode,
                    lastModifyTime = a.LastModifyTime
                }).MergeTable()
                .OrderBy(a => a.sortCode).OrderBy(a => a.creatorTime, OrderByType.Desc)
                .OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.lastModifyTime, OrderByType.Desc)
                .ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<WorkLogListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _workLogRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null)).Adapt<WorkLogInfoOutput>();
            output.userIds = output.toUserId;
            output.toUserId =await _usersService.GetUserName(output.toUserId);
            return output;
        }
        #endregion

        #region Post
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Creater([FromBody] WorkLogCrInput input)
        {
            try
            {
                var entity = input.Adapt<WorkLogEntity>();
                entity.Id= YitIdHelper.NextId().ToString();
                List<WorkLogShareEntity> workLogShareList = new List<WorkLogShareEntity>();
                var toUserIds = entity.ToUserId.Split(',');
                foreach (var userId in toUserIds)
                {
                    var workLogShare = new WorkLogShareEntity();
                    workLogShare.Id = YitIdHelper.NextId().ToString();
                    workLogShare.ShareTime = DateTime.Now;
                    workLogShare.WorkLogId = entity.Id;
                    workLogShare.ShareUserId = userId;
                    workLogShareList.Add(workLogShare);
                }
                db.BeginTran();
                _workLogRepository.AsSugarClient().Insertable(workLogShareList).ExecuteCommand();
                var isOk = await _workLogRepository.AsInsertable(entity).CallEntityMethod(m => m.Create()).ExecuteCommandAsync();
                if (isOk < 1)
                    throw HSZException.Oh(ErrorCode.COM1000);
                db.CommitTran();
            }
            catch (Exception)
            {
                db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1000);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] WorkLogUpInput input)
        {
            try
            {
                var entity = input.Adapt<WorkLogEntity>();
                List<WorkLogShareEntity> workLogShareList = new List<WorkLogShareEntity>();
                var toUserIds = entity.ToUserId.Split(',');
                foreach (var userId in toUserIds)
                {
                    var workLogShare = new WorkLogShareEntity();
                    workLogShare.Id = YitIdHelper.NextId().ToString();
                    workLogShare.ShareTime = DateTime.Now;
                    workLogShare.WorkLogId = entity.Id;
                    workLogShare.ShareUserId = userId;
                    workLogShareList.Add(workLogShare);
                }
                db.BeginTran();
                _workLogRepository.AsSugarClient().Deleteable(workLogShareList).ExecuteCommand();
                _workLogRepository.AsSugarClient().Insertable(workLogShareList).ExecuteCommand(); 
                var isOk = await _workLogRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                if (isOk < 1)
                    throw HSZException.Oh(ErrorCode.COM1001);
                db.CommitTran();
            }
            catch (Exception)
            {
                db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            try
            {
                var entity = await _workLogRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
                if (entity == null)
                    throw HSZException.Oh(ErrorCode.COM1005);
                db.BeginTran();
                _workLogRepository.AsSugarClient().Deleteable<WorkLogShareEntity>(x => x.WorkLogId == id).ExecuteCommand();
                var isOk = await _workLogRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                if (isOk < 1)
                    throw HSZException.Oh(ErrorCode.COM1002);
                db.CommitTran();
            }
            catch (Exception)
            {
                db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1002);
            }
        }
        #endregion
    }
}
