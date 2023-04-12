using HSZ.Message.Entitys.Model.IM;
using HSZ.Message.Manager;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HSZ.Message.Handler
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：websocket抽象基类
    /// </summary>
    public abstract class SocketsHandler
    {
        /// <summary>
        /// webSocket管理器
        /// </summary>
        public readonly SocketsManager _sockets;

        /// <summary>
        /// 初始化一个<see cref="SocketsHandler"/>类型的新实例
        /// </summary>
        protected SocketsHandler(SocketsManager sockets)
        {
            _sockets = sockets;
        }

        /// <summary>
        /// 连接一个 socket
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public virtual async Task OnConnected(WebSocketClient socket)
        {
            await Task.Run(() => { _sockets.AddSocket(socket); });
        }

        /// <summary>
        /// 断开指定 socket
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public virtual async Task OnDisconnected(WebSocketClient socket)
        {
            var socketId = _sockets.GetId(socket);
            if (!string.IsNullOrEmpty(socketId)) await _sockets.RemoveSocketAsync(socketId);
        }

        /// <summary>
        /// 发送消息给指定 socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(WebSocketClient socket, string message)
        {
            if (socket == null || socket.WebSocket == null || socket.WebSocket.State != WebSocketState.Open) return;

            await socket.WebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// 发送消息给指定 id 的 socket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string id, string message)
        {
            await SendMessage(_sockets.GetSocketById(id), message);
        }

        /// <summary>
        /// 给同租户下所有 sockets 发送消息
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <param name="connectionId">当前连接ID</param>
        /// <param name="message">内容</param>
        /// <returns></returns>
        public async Task SendMessageToTenantAll(string tenantId, string connectionId, string message)
        {
            foreach (var connection in _sockets.GetTenantAllConnections(tenantId, connectionId)) await SendMessage(connection.Value, message);
        }

        /// <summary>
        /// 给同租户下所有 sockets 发送消息
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <param name="message">内容</param>
        /// <returns></returns>
        public async Task SendMessageToTenantAll(string tenantId, string message)
        {
            foreach (var connection in _sockets.GetTenantAllConnections(tenantId)) await SendMessage(connection.Value, message);
        }

        /// <summary>
        /// 给同用户所有 sockets 发送消息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="message">内容</param>
        /// <returns></returns>
        public async Task SendMessageToUserAll(string userId, string message)
        {
            foreach (var connection in _sockets.GetUserAllConnections(userId)) await SendMessage(connection.Value, message);
        }

        /// <summary>
        /// 广播发送消息给所有人
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToAll(string message)
        {
            foreach (var connection in _sockets.GetAllConnections()) await SendMessage(connection.Value, message);
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="result"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public abstract Task Receive(WebSocketClient socket, WebSocketReceiveResult result,
            byte[] buffer);
    }
}
