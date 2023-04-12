using HSZ.Common.Util;
using HSZ.Message.Entitys.Model.IM;
using HSZ.Message.Handler;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UAParser;

namespace HSZ.Message.Middlewares
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：websocket中间件配置
    /// </summary>
    public class SocketsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SocketsHandler _handler;

        /// <summary>
        /// 初始化一个<see cref="SocketsMiddleware"/>类型的新实例
        /// </summary>
        public SocketsMiddleware(RequestDelegate next, SocketsHandler handler)
        {
            _next = next;
            _handler = handler;
        }

        /// <summary>
        /// 异步调用
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                // 转换当前连接为一个 ws 连接
                var socket = await context.WebSockets.AcceptWebSocketAsync();
                // 客户端信息
                var clientInfo = Parser.GetDefault().Parse(context.Request.Headers["User-Agent"]);
                var wsClient = new WebSocketClient
                {
                    ConnectionId = Guid.NewGuid().ToString("N"),
                    WebSocket = socket,
                    LoginIpAddress = NetUtil.Ip,
                    LoginPlatForm = String.Format("{0}-{1}", clientInfo.OS.ToString(), clientInfo.UA.ToString())
                };
                await _handler.OnConnected(wsClient);

                // 接收消息的 buffer
                var buffer = new byte[1024 * 4];
                // 判断连接类型，并执行相应操作
                while (socket.State == WebSocketState.Open)
                {
                    // buffer 就是接收到的消息体，可以根据需要进行转换。
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            await _handler.Receive(wsClient, result, buffer);
                            break;
                        case WebSocketMessageType.Close:
                            await _handler.OnDisconnected(wsClient);
                            break;
                        case WebSocketMessageType.Binary:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}