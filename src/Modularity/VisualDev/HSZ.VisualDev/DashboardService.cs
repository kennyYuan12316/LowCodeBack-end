using HSZ.Common.Core.Manager;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Extend.Interfaces;
using HSZ.Message.Interfaces.Message;
using HSZ.VisualDev.Entitys.Dto.Dashboard;
using HSZ.WorkFlow.Interfaces.FLowDelegate;
using HSZ.WorkFlow.Interfaces.FlowTask.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.VisualDev
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：主页显示
    /// </summary>
    [ApiDescriptionSettings(Tag = "VisualDev", Name = "Dashboard", Order = 174)]
    [Route("api/visualdev/[controller]")]
    public class DashboardService : IDynamicApiController, ITransient
    {
        private readonly IFlowTaskRepository _flowTaskRepository;
        private readonly IFlowDelegateService _flowDelegateService;
        private readonly IMessageService _messageService;
        private readonly IEmailService _emailService;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="DashboardService"/>类型的新实例
        /// </summary>
        public DashboardService(IFlowTaskRepository flowTaskRepository, IFlowDelegateService flowDelegateService, IMessageService messageService, IEmailService emailService, IUserManager userManager)
        {
            _flowTaskRepository = flowTaskRepository;
            _flowDelegateService = flowDelegateService;
            _messageService = messageService;
            _emailService = emailService;
            _userManager = userManager;
        }

        #region Get

        /// <summary>
        /// 获取我的待办
        /// </summary>
        [HttpGet("FlowTodoCount")]
        public async Task<dynamic> GetFlowTodoCount()
        {
            var userId = _userManager.UserId;
            var waitList = await _flowTaskRepository.GetWaitList();
            var flowList = await _flowDelegateService.GetList(userId);
            var trialList = await _flowTaskRepository.GetTrialList();
            var data = new FlowTodoCountOutput()
            {
                toBeReviewed = waitList.Count(),
                entrust = flowList.Count(),
                flowDone = trialList.Count()
            };
            return data;
        }

        /// <summary>
        /// 获取通知公告
        /// </summary>
        [HttpGet("Notice")]
        public async Task<dynamic> GetNotice()
        {
            List<NoticeOutput> list = new List<NoticeOutput>();
            (await _messageService.GetList(1)).FindAll(x => x.EnabledMark == 1).OrderByDescending(x => x.LastModifyTime).ToList().ForEach(l =>
                  {
                      list.Add(new NoticeOutput()
                      {
                          id = l.Id,
                          fullName = l.Title,
                          creatorTime = l.CreatorTime
                      });
                  });
            return new { list = list };
        }

        /// <summary>
        /// 获取待办事项
        /// </summary>
        [HttpGet("FlowTodo")]
        public async Task<dynamic> GetFlowTodo()
        {
            var list=await _flowTaskRepository.GetPortalWaitList();
            return new { list = list };
        }

        /// <summary>
        /// 获取我的待办事项
        /// </summary>
        [HttpGet("MyFlowTodo")]
        public async Task<dynamic> GetMyFlowTodo()
        {
            List<FlowTodoOutput> list = new List<FlowTodoOutput>();
            (await _flowTaskRepository.GetWaitList()).ForEach(l =>
            {
                list.Add(new FlowTodoOutput()
                {
                    id = l.Id,
                    fullName = l.FlowName,
                    creatorTime = l.CreatorTime
                });
            });
            return new { list = list };
        }

        /// <summary>
        /// 获取未读邮件
        /// </summary>
        [HttpGet("Email")]
        public async Task<dynamic> GetEmail()
        {
            return new { list = await _emailService.GetUnreadList() };
        }

        #endregion
    }
}
