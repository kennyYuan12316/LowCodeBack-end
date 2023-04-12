using HSZ.Message.Entitys.Model.IM;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace HSZ.Message.Manager
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：websocket链接总管理
    /// </summary>
    public class SocketsManager
    {
        private readonly ConcurrentDictionary<string, WebSocketClient> _connections = new ConcurrentDictionary<string, WebSocketClient>();

        /// <summary>
        /// 用户对应的ConnectionId
        /// </summary>
        public static ConcurrentDictionary<string, List<string>> _users = new ConcurrentDictionary<string, List<string>>();

        /// <summary>
        /// 租户id对应的ConnectionId
        /// </summary>
        public static ConcurrentDictionary<string, List<string>> _tenant = new ConcurrentDictionary<string, List<string>>();

        /// <summary>
        /// 获取同租户下所有 sockets 的字典集合
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <param name="connectionId">当前连接ID</param>
        /// <returns></returns>
        public ConcurrentDictionary<string, WebSocketClient> GetTenantAllConnections(string tenantId, string connectionId)
        {
            var webSocketList = new ConcurrentDictionary<string, WebSocketClient>();
            var connectionList = _tenant.FirstOrDefault(x => x.Key == tenantId).Value.ToArray();
            foreach (var connection in connectionList)
            {
                if (connectionId.Equals(connection))
                    webSocketList.TryAdd(connection, GetSocketById(connection));
            }
            return webSocketList;
        }

        /// <summary>
        /// 获取同租户下所有 sockets 的字典集合
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <returns></returns>
        public ConcurrentDictionary<string, WebSocketClient> GetTenantAllConnections(string tenantId)
        {
            var webSocketList = new ConcurrentDictionary<string, WebSocketClient>();
            var connectionList = _tenant.FirstOrDefault(x => x.Key == tenantId).Value.ToArray();
            foreach (var connection in connectionList)
            {
                webSocketList.TryAdd(connection, GetSocketById(connection));
            }
            return webSocketList;
        }

        /// <summary>
        /// 获取同用户下所有 sockets 的字典集合
        /// <shummary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ConcurrentDictionary<string, WebSocketClient> GetUserAllConnections(string userId)
        {
            var webSocketList = new ConcurrentDictionary<string, WebSocketClient>();
            var connectionList = _users.FirstOrDefault(x => x.Key == userId).Value;
            if (connectionList != null)
                foreach (var connection in connectionList.ToArray())
                {
                    webSocketList.TryAdd(connection, GetSocketById(connection));
                }
            return webSocketList;
        }

        /// <summary>
        /// 获取所有连接
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<string, WebSocketClient> GetAllConnections()
        {
            return _connections;
        }

        /// <summary>
        /// 获取指定 id 的 socket
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WebSocketClient GetSocketById(string id)
        {
            return _connections.FirstOrDefault(x => x.Key == id).Value;
        }

        /// <summary>
        /// 根据 socket 获取其 id
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public string GetId(WebSocketClient socket)
        {
            return _connections.FirstOrDefault(x => x.Value == socket).Key;
        }

        /// <summary>
        /// 删除指定 id 的 socket，并关闭该链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveSocketAsync(string id)
        {
            _connections.TryRemove(id, out var socket);
            if (!string.IsNullOrEmpty(socket.TenantId) && !string.IsNullOrEmpty(socket.UserId))
            {
                if (_users.ContainsKey(socket.TenantId + "_" + socket.UserId))
                    _users.FirstOrDefault(x => x.Key == socket.TenantId + "_" + socket.UserId).Value.RemoveAll(x => x.Equals(id));
                if (_tenant.ContainsKey(socket.TenantId))
                    _tenant.FirstOrDefault(x => x.Key == socket.TenantId).Value.RemoveAll(x => x.Equals(id));
            }
            if (socket.WebSocket.State != WebSocketState.Open) return;
            await socket.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "socket connection closed", CancellationToken.None);
        }

        /// <summary>
        /// 获取租户下全部 socket
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<WebSocketClient> GetTenantSocketList(string tenantId)
        {
            var webSocketList = new List<WebSocketClient>();
            var connectionList = _tenant.FirstOrDefault(x => x.Key == tenantId).Value;
            if (connectionList != null && connectionList.Count() > 0)
            {
                foreach (var connection in connectionList.ToArray())
                {
                    webSocketList.Add(GetSocketById(connection));
                }
                return webSocketList;
            }
            return null;
        }

        /// <summary>
        /// 获取租户下全部用户
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<string> GetTenantSocketUserList(string tenantId)
        {
            var webSocketList = new List<string>();
            var connectionList = _tenant.FirstOrDefault(x => x.Key == tenantId).Value;
            if (connectionList != null && connectionList.Count() > 0)
            {
                foreach (var connection in connectionList.ToArray())
                {
                    if(!string.IsNullOrEmpty(connection))
                        webSocketList.Add(GetSocketById(connection).UserId);
                }
                return webSocketList.Distinct().ToList();
            }
            return null;
        }

        /// <summary>
        /// 获取某个用户的全部 socket
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<WebSocketClient> GetUserSocketList(string tenantId, string userId)
        {
            var webSocketList = new List<WebSocketClient>();
            var connectionList = _users.FirstOrDefault(x => x.Key == tenantId + "_" + userId).Value;
            if (connectionList != null && connectionList.Count() > 0)
            {
                foreach (var connection in connectionList.ToArray())
                {
                    webSocketList.Add(GetSocketById(connection));
                }
                return webSocketList;
            }
            return null;
        }

        /// <summary>
        /// 添加一个 socket
        /// </summary>
        /// <param name="socket"></param>
        public void AddSocket(WebSocketClient socket)
        {
            _connections.TryAdd(socket.ConnectionId, socket);
            if (!string.IsNullOrEmpty(socket.UserId) && !string.IsNullOrEmpty(socket.TenantId))
            {
                if (!_users.ContainsKey(socket.TenantId + "_" + socket.UserId))
                {
                    _users.TryAdd(socket.TenantId + "_" + socket.UserId, new List<string>());
                }
                if (!_users.FirstOrDefault(x => x.Key == socket.TenantId + "_" + socket.UserId).Value.Contains(socket.ConnectionId))
                    _users.FirstOrDefault(x => x.Key == socket.TenantId + "_" + socket.UserId).Value.Add(socket.ConnectionId);
            }
            if (!string.IsNullOrEmpty(socket.TenantId))
            {
                if (!_tenant.ContainsKey(socket.TenantId))
                {
                    _tenant.TryAdd(socket.TenantId, new List<string>());
                }
                if (!_tenant.FirstOrDefault(x => x.Key == socket.TenantId).Value.Contains(socket.ConnectionId))
                    _tenant.GetValueOrDefault(socket.TenantId).Add(socket.ConnectionId);
            }
        }
    }
}
