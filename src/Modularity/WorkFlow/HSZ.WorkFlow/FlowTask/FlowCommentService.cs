using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Permission;
using HSZ.WorkFlow.Entitys;
using HSZ.WorkFlow.Entitys.Dto.FlowComment;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System.Threading.Tasks;

namespace HSZ.WorkFlow.Core.Service.FlowTask
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：流程评论
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowComment", Order = 304)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowCommentService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<FlowCommentEntity> _flowCommentRepository;
        private readonly SqlSugarScope Db;// 核心对象：拥有完整的SqlSugar全部功能
        private readonly IUserManager _userManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowCommentRepository"></param>
        public FlowCommentService(ISqlSugarRepository<FlowCommentEntity> flowCommentRepository, IUserManager userManager)
        {
            _flowCommentRepository = flowCommentRepository;
            _userManager = userManager;
            Db = DbScoped.SugarScope;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] FlowCommentListQuery input)
        {
            var list = await _flowCommentRepository.AsSugarClient().Queryable<FlowCommentEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.CreatorUserId == b.Id))
                .Where((a, b) => a.TaskId == input.taskId && a.DeleteMark == null).Select((a, b) => new FlowCommentListOutput()
                {
                    id = a.Id,
                    taskId = a.TaskId,
                    text = a.Text,
                    image = a.Image,
                    file = a.File,
                    creatorUserId = b.Id,
                    creatorTime = a.CreatorTime,
                    creatorUserName = SqlFunc.MergeString(b.RealName, "/", b.Account),
                    creatorUserHeadIcon = SqlFunc.MergeString("/api/File/Image/userAvatar/", b.HeadIcon),
                    isDel = SqlFunc.IIF(a.CreatorUserId == _userManager.UserId, true, false),
                    lastModifyTime = a.LastModifyTime,
                }).MergeTable().OrderBy(a => a.creatorTime, OrderByType.Desc).OrderByIF(!string.IsNullOrEmpty(input.keyword), a => a.lastModifyTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<FlowCommentListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">请求参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            return (await _flowCommentRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null)).Adapt<FlowCommentInfoOutput>();
        }
        #endregion

        #region Post
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] FlowCommentCrInput input)
        {
            var entity = input.Adapt<FlowCommentEntity>();
            var isOk = await _flowCommentRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] FlowCommentUpInput input)
        {
            var entity = input.Adapt<FlowCommentEntity>();
            var isOk = await _flowCommentRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _flowCommentRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            var isOk = await _flowCommentRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }
        #endregion
    }
}
