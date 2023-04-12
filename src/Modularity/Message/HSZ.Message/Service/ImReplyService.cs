using HSZ.Common.Core.Manager;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.JsonSerialization;
using HSZ.Message.Entitys;
using HSZ.Message.Entitys.Dto.ImReply;
using HSZ.Message.Extensions;
using HSZ.Message.Handler;
using HSZ.Message.Interfaces;
using HSZ.Message.Manager;
using HSZ.System.Entitys.Permission;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.Message.Service
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：消息会话接口
    /// </summary>
    [ApiDescriptionSettings(Tag = "Message", Name = "imreply", Order = 163)]
    [Route("api/message/[controller]")]
    public class ImReplyService : IImReplyService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ImReplyEntity> _imReplyRepository;
        private readonly IUserManager _userManager;
        private readonly WebSocketMessageHandler _handler;

        /// <summary>
        /// 初始化一个<see cref="ImReplyService"/>类型的新实例
        /// </summary>
        public ImReplyService(ISqlSugarRepository<ImReplyEntity> imReplyRepository,
            IUserManager userManager,
            WebSocketMessageHandler handler)
        {
            _imReplyRepository = imReplyRepository;
            _userManager = userManager;
            _handler = handler;
        }

        /// <summary>
        /// 获取消息会话列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList()
        {
            var userInfo = await _userManager.GetUserInfo();
            var newObjectUserList = new List<ImReplyListOutput>();
            //获取全部聊天对象列表
            var objectList = _imReplyRepository.AsSugarClient().UnionAll(_imReplyRepository.AsSugarClient().Queryable<ImReplyEntity>().Where(i => i.ReceiveUserId == userInfo.userId).Select(it => new ImReplyObjectIdOutput { userId = it.UserId, latestDate = it.ReceiveTime }),
                 _imReplyRepository.AsSugarClient().Queryable<ImReplyEntity>().Where(i => i.UserId == userInfo.userId).Select(it => new ImReplyObjectIdOutput { userId = it.ReceiveUserId, latestDate = it.ReceiveTime })).MergeTable().GroupBy(it => new { it.userId }).Select(it => new { it.userId, latestDate = SqlFunc.AggregateMax(it.latestDate) }).ToList();
            var objectUserList = objectList.Adapt<List<ImReplyListOutput>>();
            if (objectUserList.Count > 0)
            {
                var userList = await _imReplyRepository.AsSugarClient().Queryable<UserEntity>().In(it => it.Id, objectUserList.Select(it => it.userId).ToArray()).ToListAsync();
                //将用户信息补齐
                userList.ForEach(item =>
                {
                    objectUserList.ForEach(it =>
                    {
                        if (it.userId == item.Id)
                        {
                            it.account = item.Account;
                            it.id = it.userId;
                            it.realName = item.RealName;
                            it.headIcon = "/api/File/Image/userAvatar/" + item.HeadIcon;

                            var imContent = _imReplyRepository.AsSugarClient().Queryable<IMContentEntity>().Where(i => (i.SendUserId == userInfo.userId && i.ReceiveUserId == it.userId) || (i.SendUserId == it.userId && i.ReceiveUserId == userInfo.userId)).Where(i => i.SendTime.Equals(it.latestDate)).ToList().FirstOrDefault();
                            //获取最信息
                            if (imContent != null)
                            {
                                it.latestMessage = imContent.Content;
                                it.messageType = imContent.ContentType;
                            }
                            it.unreadMessage = _imReplyRepository.AsSugarClient().Queryable<IMContentEntity>().Where(i => i.SendUserId == it.userId && i.ReceiveUserId == userInfo.userId).Where(i => i.State == 0).Count();
                        }
                    });
                });
            }
            var output = objectUserList.OrderByDescending(x => x.latestDate).ToList();
            return new { list = output };
        }

        /// <summary>
        /// 强制下线
        /// </summary>
        /// <param name="connectionId"></param>
        [NonAction]
        public async void ForcedOffline(string connectionId)
        {
            var onlineUser = _handler._sockets.GetSocketById(connectionId);
            if (onlineUser != null)
            {
                await _handler.SendMessage(connectionId, new { method = "logout", msg = "此账号已在其他地方登陆" }.Serialize());
            }
        }
    }
}
