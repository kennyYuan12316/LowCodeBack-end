using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Extend.Entitys;
using HSZ.Extend.Entitys.Dto.ProjectGantt;
using HSZ.FriendlyException;
using HSZ.System.Interfaces.Permission;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.Extend
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：项目计划
    /// </summary>
    [ApiDescriptionSettings(Tag = "Extend", Name = "ProjectGantt", Order = 600)]
    [Route("api/extend/[controller]")]
    public class ProjectGanttService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ProjectGanttEntity> _projectGanttRepository;
        private readonly IUsersService _usersService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectGanttRepository"></param>
        /// <param name="usersService"></param>
        public ProjectGanttService(ISqlSugarRepository<ProjectGanttEntity> projectGanttRepository, IUsersService usersService)
        {
            _projectGanttRepository = projectGanttRepository;
            _usersService = usersService;
        }
        #region GET
        /// <summary>
        /// 项目列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] KeywordInput input)
        {
            var data = await _projectGanttRepository.AsQueryable().Where(x => x.Type == 1 && x.DeleteMark == null)
                .WhereIF(input.keyword.IsNotEmptyOrNull(), x => x.FullName.Contains(input.keyword))
                .OrderBy(x=>x.SortCode).OrderBy(x => x.CreatorTime, OrderByType.Desc)
                .OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.LastModifyTime, OrderByType.Desc).ToListAsync();
            var output = data.Adapt<List<ProjectGanttListOutput>>();
            await GetManagersInfo(output);
            return new { list = output };
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("{projectId}/Task")]
        public async Task<dynamic> GetTaskList([FromQuery] KeywordInput input, string projectId)
        {
            var data = await _projectGanttRepository.AsQueryable()
                .Where(x => x.Type == 2 && x.ProjectId == projectId && x.DeleteMark == null)
                .OrderBy(x => x.SortCode).OrderBy(x => x.CreatorTime, OrderByType.Desc)
                .OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.LastModifyTime, OrderByType.Desc).ToListAsync();
            data.Add(await _projectGanttRepository.GetFirstAsync(x => x.Id == projectId));
            if (!string.IsNullOrEmpty(input.keyword))
            {
                data = data.TreeWhere(t => t.FullName.Contains(input.keyword), t => t.Id, t => t.ParentId);
            }
            var output = data.Adapt<List<ProjectGanttTaskListOutput>>();
            return new { list = output.ToTree() };
        }

        /// <summary>
        /// 任务树形
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        [HttpGet("{projectId}/Task/Selector/{id}")]
        public async Task<dynamic> GetTaskTreeView(string projectId,string id)
        {
            var data = (await _projectGanttRepository.AsQueryable().Where(x => x.Type == 2 && x.ProjectId == projectId && x.DeleteMark == null).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync());
            data.Add(await _projectGanttRepository.GetFirstAsync(x => x.Id == projectId));
            if (!id.Equals("0"))
            {
                data.RemoveAll(x=>x.Id== id);
            }
            var output = data.Adapt<List<ProjectGanttTaskTreeViewOutput>>();
            return new { list = output.ToTree() };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = (await _projectGanttRepository.GetFirstAsync(x=>x.Id==id&&x.DeleteMark==null)).Adapt<ProjectGanttInfoOutput>();
            return data;
        }

        /// <summary>
        /// 项目任务信息
        /// </summary>
        /// <param name="taskId">主键值</param>
        /// <returns></returns>
        [HttpGet("Task/{taskId}")]
        public async Task<dynamic> GetTaskInfo(string taskId)
        {
            var data = (await _projectGanttRepository.GetFirstAsync(x => x.Id == taskId && x.DeleteMark == null)).Adapt<ProjectGanttTaskInfoOutput>();
            return data;
        }
        #endregion

        #region POST
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            if (await _projectGanttRepository.IsAnyAsync(x => x.ParentId != id&&x.DeleteMark==null))
            {
                var entity = await _projectGanttRepository.GetFirstAsync(x=>x.Id==id&&x.DeleteMark==null);
                if (entity != null)
                {
                   var isOk= await _projectGanttRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                    if (isOk < 1)
                        throw HSZException.Oh(ErrorCode.COM1002);
                }
                else
                {
                    throw HSZException.Oh(ErrorCode.COM1005);
                }
            }
            else
            {
                throw HSZException.Oh(ErrorCode.D1007);
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ProjectGanttCrInput input)
        {
            if (await _projectGanttRepository.IsAnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null) || await _projectGanttRepository.IsAnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ProjectGanttEntity>();
            entity.Type = 1;
            entity.ParentId = "0";
            var isOk= await _projectGanttRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ProjectGanttUpInput input)
        {
            if (await _projectGanttRepository.IsAnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null) || await _projectGanttRepository.IsAnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ProjectGanttEntity>();
            var isOk= await _projectGanttRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("Task")]
        public async Task CreateTask([FromBody] ProjectGanttTaskCrInput input)
        {
            var entity = input.Adapt<ProjectGanttEntity>();
            entity.Type = 2;
            var isOk = await _projectGanttRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("Task/{id}")]
        public async Task UpdateTask(string id, [FromBody] ProjectGanttTaskUpInput input)
        {
            var entity = input.Adapt<ProjectGanttEntity>();
            var isOk = await _projectGanttRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1001);
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 项目参与人员
        /// </summary>
        /// <param name="outputList"></param>
        /// <returns></returns>
        private async Task GetManagersInfo(List<ProjectGanttListOutput> outputList)
        {
            var userList = await _usersService.GetList();
            foreach (var output in outputList)
            {
                List<string> managerIds = new List<string>(output.managerIds.Split(","));
                foreach (var id in managerIds)
                {
                    var managerInfo = new ManagersInfo();
                    var userInfo = userList.Find(x => x.Id == id);
                    if (userInfo != null)
                    {
                        managerInfo.account = userInfo.RealName + "/" + userInfo.Account;
                        managerInfo.headIcon = string.IsNullOrEmpty(userInfo.HeadIcon) ? "" : "/api/file/Image/userAvatar/" + userInfo.HeadIcon;
                        output.managersInfo.Add(managerInfo);
                    }
                }
            }
        }
        #endregion
    }
}
