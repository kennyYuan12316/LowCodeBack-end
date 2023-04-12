using HSZ.Common.Configuration;
using HSZ.Common.Const;
using HSZ.Common.Core.Manager;
using HSZ.Common.Extension;
using HSZ.Common.Helper;
using HSZ.Data.SqlSugar.Extensions;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.JsonSerialization;
using HSZ.Message.Entitys;
using HSZ.Message.Entitys.Dto.IM;
using HSZ.Message.Entitys.Model.IM;
using HSZ.Message.Manager;
using HSZ.System.Entitys.Permission;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Serilog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.Message.Handler
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：WebSocket消息帮助类
    /// </summary>
    public class WebSocketMessageHandler : SocketsHandler
    {
        /// <summary>
        /// SqlSugarScope操作数据库是线程安的可以单例
        /// </summary>
        public static SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
        {
            ConfigId = App.Configuration["ConnectionStrings:ConfigId"],
            DbType = (DbType)Enum.Parse(typeof(DbType), App.Configuration["ConnectionStrings:DBType"]),
            ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", App.Configuration["ConnectionStrings:DBName"]),
            IsAutoCloseConnection = true,
            ConfigureExternalServices = new ConfigureExternalServicesExtenisons()
            {
                EntityNameServiceType = typeof(SugarTable)//这个不管是不是自定义都要写，主要是用来获取所有实体
            }
        }, db =>
        {
            //如果用单例配置要统一写在这儿
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Log.Information($"ContextID:{db.ContextID.ToString()}");
                if (sql.StartsWith("SELECT"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (sql.StartsWith("DELETE"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                //在控制台输出sql语句
                Console.WriteLine(SqlProfiler.ParameterFormat(sql, pars));
                Console.WriteLine();
                //App.PrintToMiniProfiler("SqlSugar", "Info", SqlProfiler.ParameterFormat(sql, pars));
            };
        });

        /// <summary>
        /// 初始化一个<see cref="WebSocketMessageHandler"/>类型的新实例
        /// </summary>
        public WebSocketMessageHandler(SocketsManager sockets) : base(sockets)
        {
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public override async Task OnConnected(WebSocketClient socket)
        {
            await base.OnConnected(socket);
            var socketId = _sockets.GetId(socket);
            Log.Information($"{socketId}已加入.");
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public override async Task OnDisconnected(WebSocketClient socket)
        {
            var socketId = _sockets.GetId(socket);
            Log.Information($"WebSocketMessageHandler打印需要断开的连接ID:{socketId}");
            if (socketId.IsNotEmptyOrNull())
            {
                await Scoped.Create(async (_, scope) =>
                {
                    var services = scope.ServiceProvider;
                    var _cacheManager = App.GetService<ICacheManager>(services);
                    if (socket != null && !string.IsNullOrEmpty(socket.TenantId))
                    {
                        var list = _cacheManager.GetOnlineUserList(socket.TenantId);
                        if (list == null)
                        {
                            list = new List<UserOnlineModel>();
                        }
                        list.RemoveAll((x) => x.connectionId == socketId);
                        _cacheManager.SetOnlineUserList(socket.TenantId, list);
                    }
                });
                await base.OnDisconnected(socket);
            }
        }

        /// <summary>
        /// 消息接收
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="result"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public override async Task Receive(WebSocketClient socket, WebSocketReceiveResult result, byte[] buffer)
        {
            try
            {
                var msgString = Encoding.UTF8.GetString(buffer, 0, result.Count).Replace("\0", "");
                Log.Information($"Websocket client ReceiveAsync message {msgString}.");
                var message = msgString.ToObject<MessageInput>();
                var socketId = _sockets.GetId(socket);
                if (!string.IsNullOrEmpty(message.token))
                {
                    if (message.token.ToLower().IndexOf("bearer") < 0)
                    {
                        Log.Information($"{socketId}无效ToKen,离开了.IP:{socket.LoginIpAddress}");
                        await base.OnDisconnected(socket);
                    }
                    else
                    {
                        var token = new JsonWebToken(message.token.Replace("Bearer ", "").Replace("bearer ", ""));
                        var httpContext = (DefaultHttpContext)App.HttpContext;
                        httpContext.Request.Headers["Authorization"] = message.token;
                        if (!JWTEncryption.ValidateJwtBearerToken(httpContext, out token))
                        {
                            Log.Information($"{socketId}无效ToKen,离开了.IP:{socket.LoginIpAddress}");
                            await base.OnDisconnected(socket);
                        }
                        else
                        {
                            var claims = JWTEncryption.ReadJwtToken(message.token.Replace("Bearer ", "").Replace("bearer ", ""))?.Claims;
                            socket.UserId = claims.FirstOrDefault(e => e.Type == ClaimConst.CLAINM_USERID)?.Value;
                            socket.TenantId = claims.FirstOrDefault(e => e.Type == ClaimConst.TENANT_ID)?.Value;
                            socket.Account = claims.FirstOrDefault(e => e.Type == ClaimConst.CLAINM_ACCOUNT)?.Value;
                            socket.UserName = claims.FirstOrDefault(e => e.Type == ClaimConst.CLAINM_REALNAME)?.Value;
                            socket.SingleLogin = claims.FirstOrDefault(e => e.Type == ClaimConst.SINGLELOGIN)?.Value;
                            socket.LoginTime = string.Format("{0:yyyy-MM-dd HH:mm}", Ext.GetDateTime(claims.FirstOrDefault(e => e.Type == "iat")?.Value + "000"));
                            socket.LoginIpAddress = socket.LoginIpAddress;
                            socket.Token = message.token;
                            socket.IsMobileDevice = message.mobileDevice;
                            if (socket.WebSocket.State != WebSocketState.Open) return;
                            await base.OnConnected(socket);
                            if (KeyVariable.MultiTenancy)
                            {
                                var dbList = Db.DbMaintenance.GetDataBaseList(Db.ScopedContext);
                                var tenantDbName = claims.FirstOrDefault(e => e.Type == ClaimConst.TENANT_DB_NAME)?.Value;
                                //服务器存在这个数据库
                                if (dbList.Any(it => it.Equals(tenantDbName)))
                                {
                                    message.sendClientId = socketId;
                                    MessageRoute(message);
                                }
                                else
                                {
                                    await base.OnDisconnected(socket);
                                    Log.Information($"{socketId}数据库没有{tenantDbName}这个库,离开了.");
                                }
                            }
                            else
                            {
                                message.sendClientId = socketId;
                                MessageRoute(message);
                            }
                        }
                    }
                }
                else
                {
                    await base.OnDisconnected(socket);
                    Log.Information($"{socketId}没有ToKen,离开了.");
                }
            }
            catch (Exception ex)
            {
                Log.Information($"错误1：{ex.StackTrace}");
                throw;
            }
            //var message = $"{socketId} 发送了消息：{Encoding.UTF8.GetString(buffer, 0, result.Count)}";
            //await SendMessage(socketId, message);
        }

        /// <summary>
        /// 消息通道
        /// </summary>
        /// <param name="message"></param>
        private async void MessageRoute(MessageInput message)
        {
            try
            {
                var client = _sockets.GetSocketById(message.sendClientId);
                var claims = JWTEncryption.ReadJwtToken(message.token.Replace("Bearer ", "").Replace("bearer ", ""))?.Claims;
                if (string.IsNullOrEmpty(client.UserId))
                {
                    await SendMessage(client.ConnectionId, new { method = "logout" }.Serialize());
                    return;
                }

                if (KeyVariable.MultiTenancy)
                {
                    Db.AddConnection(new ConnectionConfig()
                    {
                        DbType = (DbType)Enum.Parse(typeof(DbType), App.Configuration["ConnectionStrings:DBType"]),
                        ConfigId = client.TenantId,//设置库的唯一标识
                        IsAutoCloseConnection = true,
                        ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", claims.FirstOrDefault(e => e.Type == ClaimConst.TENANT_DB_NAME)?.Value)
                    });
                    Db.ChangeDatabase(client.TenantId);
                }

                if (string.IsNullOrEmpty(client.HeadIcon))
                {
                    var userEntity = await Db.Queryable<UserEntity>().SingleAsync(it => it.Id == client.UserId);
                    if (userEntity != null)
                    {
                        client.HeadIcon = "/api/file/Image/userAvatar/" + userEntity.HeadIcon;
                        await base.OnConnected(client);
                    }
                }

                switch (message.method)
                {
                    case "OnConnection":
                        {
                            #region 建立连接

                            //添加在线用户缓存与单体登录
                            await Scoped.Create(async (_, scope) =>
                            {
                                var services = scope.ServiceProvider;
                                var _cacheManager = App.GetService<ICacheManager>(services);
                                var list = _cacheManager.GetOnlineUserList(client.TenantId);
                                if (list == null)
                                {
                                    list = new List<UserOnlineModel>();
                                }

                                if (client.SingleLogin == "2")//根据配置文件判断是否是同时登录
                                {
                                    var user = list.Find(it => it.token != null && it.token.Equals(message.token));
                                    if (user != null)
                                    {
                                        var onlineUser = _sockets.GetSocketById(user.connectionId);
                                        if (onlineUser != null)
                                        {
                                            await SendMessage(onlineUser.ConnectionId, new { method = "closeSocket", msg = "" }.Serialize());
                                        }
                                        list.RemoveAll((x) => x.connectionId == user.connectionId);
                                    }
                                    list.Add(new UserOnlineModel()
                                    {
                                        connectionId = client.ConnectionId,
                                        userId = client.UserId,
                                        account = client.Account,
                                        userName = client.UserName,
                                        lastTime = DateTime.Now,
                                        lastLoginIp = client.LoginIpAddress,
                                        tenantId = client.TenantId,
                                        lastLoginPlatForm = client.LoginPlatForm,
                                        isMobileDevice = client.IsMobileDevice,
                                        token = message.token
                                    });
                                    _cacheManager.SetOnlineUserList(client.TenantId, list);
                                }
                                else
                                {
                                    var user = list.Find(it => it.userId.Equals(client.UserId) && it.isMobileDevice.Equals(client.IsMobileDevice));
                                    if (user == null)
                                    {
                                        list.Add(new UserOnlineModel()
                                        {
                                            connectionId = client.ConnectionId,
                                            userId = client.UserId,
                                            account = client.Account,
                                            userName = client.UserName,
                                            lastTime = DateTime.Now,
                                            lastLoginIp = client.LoginIpAddress,
                                            tenantId = client.TenantId,
                                            lastLoginPlatForm = client.LoginPlatForm,
                                            isMobileDevice = client.IsMobileDevice,
                                            token = message.token
                                        });
                                        _cacheManager.SetOnlineUserList(client.TenantId, list);
                                    }
                                    //不同浏览器
                                    else if (user != null && !string.IsNullOrEmpty(user.token) && !user.token.Equals(message.token))
                                    {
                                        var onlineUser = _sockets.GetSocketById(user.connectionId);
                                        if (onlineUser != null)
                                        {
                                            await SendMessage(onlineUser.ConnectionId, new { method = "logout", msg = "此账号已在其他地方登陆" }.Serialize());
                                        }
                                        list.RemoveAll((x) => x.connectionId == user.connectionId);

                                        list.Add(new UserOnlineModel()
                                        {
                                            connectionId = client.ConnectionId,
                                            userId = client.UserId,
                                            account = client.Account,
                                            userName = client.UserName,
                                            lastTime = DateTime.Now,
                                            lastLoginIp = client.LoginIpAddress,
                                            tenantId = client.TenantId,
                                            lastLoginPlatForm = client.LoginPlatForm,
                                            isMobileDevice = client.IsMobileDevice,
                                            token = message.token
                                        });
                                        _cacheManager.SetOnlineUserList(client.TenantId, list);
                                    }
                                }
                            });

                            var onlineUserList = _sockets.GetTenantSocketUserList(client.TenantId);

                            #region 反馈信息给登录者


                            //获取接收者为当前用户的聊天且未读的信息
                            var list = Db.Queryable<IMContentEntity>().Where(x => x.ReceiveUserId.Equals(client.UserId) && x.State.Equals(0)).GroupBy(x => new { x.SendUserId, x.ReceiveUserId }).Select(x => new IMContentEntity
                            {
                                State = SqlFunc.AggregateSum(SqlFunc.IIF(x.State == 0, 1, 0)),
                                SendUserId = x.SendUserId,
                                ReceiveUserId = x.ReceiveUserId
                            }).ToList();

                            var list1 = Db.Queryable<IMContentEntity>().Where(x => x.ReceiveUserId == client.UserId).OrderBy(x => x.SendTime, OrderByType.Desc).ToList();
                            var unreadNums = list.Adapt<List<IMUnreadNumModel>>();
                            foreach (var item in unreadNums)
                            {
                                var entity = list1.FirstOrDefault(x => x.SendUserId == item.sendUserId);
                                item.defaultMessage = entity.Content;
                                item.defaultMessageType = entity.ContentType;
                                item.defaultMessageTime = entity.SendTime.ToString();
                            }

                            var unreadNoticeCount = await Db.Queryable<MessageEntity, MessageReceiveEntity>((m, mr) => new JoinQueryInfos(JoinType.Left, m.Id == mr.MessageId)).Where((m, mr) => m.Type == 1 && m.DeleteMark == null && mr.UserId == client.UserId && mr.IsRead == 0).Select((m, mr) => new { mr.Id, mr.UserId, mr.IsRead, m.Type, m.DeleteMark }).CountAsync();
                            var unreadMessageCount = await Db.Queryable<MessageEntity, MessageReceiveEntity>((m, mr) => new JoinQueryInfos(JoinType.Left, m.Id == mr.MessageId)).Select((m, mr) => new { mr.Id, mr.UserId, mr.IsRead, m.Type, m.DeleteMark }).MergeTable().Where(x => x.Type == 2 && x.DeleteMark == null && x.UserId == client.UserId && x.IsRead == 0).CountAsync();
                            var noticeDefault = await Db.Queryable<MessageEntity>().Where(x => x.Type == 1 && x.DeleteMark == null && x.EnabledMark == 1).OrderBy(x => x.CreatorTime, OrderByType.Desc).FirstAsync();
                            var noticeDefaultText = noticeDefault == null ? "" : noticeDefault.Title;
                            var messageDefault = await Db.Queryable<MessageEntity, MessageReceiveEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.Id == b.MessageId)).Where((a, b) => a.Type == 2 && a.DeleteMark == null && b.UserId == client.UserId).OrderBy(a => a.CreatorTime, OrderByType.Desc).Select(a => a).FirstAsync();
                            var messageDefaultText = messageDefault == null ? "" : messageDefault.Title;
                            var messageDefaultTime = messageDefault == null ? DateTime.Now : messageDefault.CreatorTime;
                            var noticeDefaultTime = noticeDefault == null ? DateTime.Now : noticeDefault.CreatorTime;
                            await SendMessage(client.ConnectionId, new { method = "initMessage", onlineUserList, unreadNums, unreadNoticeCount, noticeDefaultText, unreadMessageCount, messageDefaultText, messageDefaultTime, noticeDefaultTime }.Serialize());

                            #endregion

                            #region 通知所有在线用户，有用户在线

                            await SendMessageToTenantAll(client.TenantId, client.ConnectionId, new { method = "online", userId = client.UserId }.Serialize());

                            #endregion

                            #endregion
                        }
                        break;
                    case "SendMessage":
                        {
                            #region 发送消息

                            var toUserId = message.toUserId;
                            var messageType = message.messageType;
                            var messageContent = message.messageContent;
                            var fileName = "";

                            if (messageType == "image")
                            {
                                var directoryPath = FileVariable.IMContentFilePath;
                                if (!Directory.Exists(directoryPath))
                                    Directory.CreateDirectory(directoryPath);
                                var imageInput = messageContent.ToObject<MessagetImageInput>();
                                fileName = imageInput.name;
                            }

                            var onlineToUser = _sockets.GetUserSocketList(client.TenantId, toUserId);

                            var toUserEntity = await Db.Queryable<UserEntity>().SingleAsync(it => it.Id == toUserId);

                            //将发送消息对象信息补全
                            var toAccount = toUserEntity.Account;
                            var toHeadIcon = toUserEntity.HeadIcon;
                            var toRealName = toUserEntity.RealName;

                            #region saveMessage

                            var entity = new IMContentEntity();
                            var toMessage = new object();
                            switch (messageType)
                            {
                                case "text":
                                    entity = CreateIMContent(client.UserId, toUserId, messageContent.ToString(), messageType);
                                    break;
                                case "image":
                                    var imageInput = messageContent.ToObject<MessagetImageInput>();
                                    toMessage = new { path = "/api/file/Image/IM/" + fileName, width = imageInput.width, height = imageInput.height };

                                    entity = CreateIMContent(client.UserId, toUserId, toMessage.ToJson(), messageType);
                                    break;
                                case "voice":
                                    var voiceInput = messageContent.ToObject<MessageVoiceInput>();
                                    toMessage = new { path = "/api/file/Image/IM/" + voiceInput.name, length = voiceInput.length };
                                    entity = CreateIMContent(client.UserId, toUserId, toMessage.ToJson(), messageType);
                                    break;
                                default:
                                    break;
                            }

                            //写入到会话表中
                            var isExist = await Db.Queryable<ImReplyEntity>().AnyAsync(it => it.UserId == client.UserId && it.ReceiveUserId == toUserId);
                            if (isExist)
                            {
                                var imReplyEntity = await Db.Queryable<ImReplyEntity>().SingleAsync(it => it.UserId == client.UserId && it.ReceiveUserId == toUserId);
                                imReplyEntity.ReceiveTime = entity.SendTime;
                                await Db.Updateable(imReplyEntity).ExecuteCommandAsync();
                            }
                            else
                            {
                                var imReplyEntity = new ImReplyEntity()
                                {
                                    Id = YitIdHelper.NextId().ToString(),
                                    UserId = client.UserId,
                                    ReceiveUserId = toUserId,
                                    ReceiveTime = entity.SendTime
                                };
                                await Db.Insertable(imReplyEntity).ExecuteCommandAsync();
                            }
                            await Db.Insertable(entity).ExecuteCommandAsync();

                            #endregion

                            #region sendMessage

                            if (messageType == "text")
                            {
                                await SendMessage(client.ConnectionId, new { method = "sendMessage", client.UserId, account = client.Account, headIcon = client.HeadIcon, realName = client.UserName, toAccount, toHeadIcon, messageType, toUserId, toRealName, toMessage = messageContent, dateTime = DateTime.Now, latestDate = DateTime.Now }.Serialize());
                            }
                            else if (messageType == "image")
                            {
                                var imageInput = messageContent.ToObject<MessagetImageInput>();
                                toMessage = new { path = "/api/file/Image/IM/" + fileName, width = imageInput.width, height = imageInput.height };
                                await SendMessage(client.ConnectionId, new { method = "sendMessage", client.UserId, account = client.Account, headIcon = client.HeadIcon, realName = client.UserName, toAccount, toHeadIcon, messageType, toUserId, toMessage, dateTime = DateTime.Now, latestDate = DateTime.Now }.Serialize());
                            }
                            else if (messageType == "voice")
                            {
                                var voiceInput = messageContent.ToObject<MessageVoiceInput>();
                                toMessage = new { path = "/api/file/Image/IM/" + voiceInput.name, length = voiceInput.length };
                                await SendMessage(client.ConnectionId, new { method = "sendMessage", client.UserId, account = client.Account, headIcon = client.HeadIcon, realName = client.UserName, toAccount, toHeadIcon, messageType, toUserId, toMessage, dateTime = DateTime.Now }.Serialize());
                            }

                            #endregion

                            //考虑多端
                            if (onlineToUser != null && onlineToUser.Count > 0)
                            {
                                #region receiveMessage

                                if (messageType == "text")
                                {
                                    await SendMessageToUserAll(client.TenantId + "_" + toUserId, new { method = "receiveMessage", messageType, formUserId = client.UserId, formMessage = messageContent, dateTime = DateTime.Now, latestDate = DateTime.Now, headIcon = client.HeadIcon, realName = client.UserName }.Serialize());
                                }
                                else if (messageType == "image")
                                {
                                    var imageInput = messageContent.ToObject<MessagetImageInput>();
                                    var formMessage = new { path = "/api/file/Image/IM/" + fileName, width = imageInput.width, height = imageInput.height };
                                    await SendMessageToUserAll(client.TenantId + "_" + toUserId, new { method = "receiveMessage", messageType, formUserId = client.UserId, formMessage, dateTime = DateTime.Now, latestDate = DateTime.Now, headIcon = client.HeadIcon, realName = client.UserName }.Serialize());
                                }
                                else if (messageType == "voice")
                                {
                                    var voiceInput = messageContent.ToObject<MessageVoiceInput>();
                                    toMessage = new { path = "/api/file/Image/IM/" + voiceInput.name, length = voiceInput.length };
                                    await SendMessageToUserAll(client.TenantId + "_" + toUserId, new { method = "receiveMessage", messageType, formUserId = client.UserId, formMessage = toMessage, dateTime = DateTime.Now, latestDate = DateTime.Now, headIcon = client.HeadIcon, realName = client.UserName }.Serialize());
                                }

                                #endregion
                            }

                            #endregion
                        }
                        break;
                    case "UpdateReadMessage":
                        {
                            var fromUserId = message.formUserId;
                            await Db.Updateable<IMContentEntity>()
                                .SetColumns(x => new IMContentEntity()
                                {
                                    State = 1,
                                    ReceiveTime = DateTime.Now
                                }).Where(x => x.State == 0 && x.SendUserId == fromUserId && x.ReceiveUserId == client.UserId).ExecuteCommandAsync();
                        }
                        break;
                    case "MessageList":
                        {
                            var sendUserId = message.toUserId;                //发送者
                            var receiveUserId = message.formUserId;           //接收者

                            var data = await Db.Queryable<IMContentEntity>().WhereIF(!string.IsNullOrEmpty(message.keyword), it => it.Content.Contains(message.keyword))
                                    .Where(i => (i.SendUserId == message.toUserId && i.ReceiveUserId == message.formUserId) || (i.SendUserId == message.formUserId && i.ReceiveUserId == message.toUserId)).OrderBy(it => it.SendTime, message.sord == "asc" ? OrderByType.Asc : OrderByType.Desc)
                                    .Select(it => new IMContentListOutput
                                    {
                                        id = it.Id,
                                        sendUserId = it.SendUserId,
                                        sendTime = it.SendTime,
                                        receiveUserId = it.ReceiveUserId,
                                        receiveTime = it.ReceiveTime,
                                        content = it.Content,
                                        contentType = it.ContentType,
                                        state = it.State
                                    }).ToPagedListAsync(message.currentPage, message.pageSize);

                            await SendMessage(client.ConnectionId, new { method = "messageList", list = data.list.OrderBy(x => x.sendTime), pagination = data.pagination }.Serialize());
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Information($"错误2：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 创建IM内容
        /// </summary>
        /// <returns></returns>
        private IMContentEntity CreateIMContent(string sendUserId, string receiveUserId, string message, string messageType)
        {
            return new IMContentEntity()
            {
                Id = YitIdHelper.NextId().ToString(),
                SendUserId = sendUserId,
                SendTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                ReceiveUserId = receiveUserId,
                State = 0,
                Content = message,
                ContentType = messageType
            };
        }
    }
}
