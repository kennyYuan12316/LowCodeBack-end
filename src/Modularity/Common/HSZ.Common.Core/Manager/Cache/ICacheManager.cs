using HSZ.Common.Enum;
using HSZ.Entitys.wms;
using HSZ.Message.Entitys.Model.IM;
using HSZ.System.Entitys.Dto.Permission.Position;
using HSZ.System.Entitys.Dto.Permission.Role;
using HSZ.System.Entitys.Model.Permission.User;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.Common.Core.Manager
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：缓存管理抽象
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
     /// 用户是否存在
     /// </summary>
     /// <param name="userId">用户ID</param>
     /// <returns></returns>
        Task<bool> ExistsUserAsync(string userId);

        /// <summary>
        /// 缓存数据范围（机构Id集合）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="dataScopes">数据作用域</param>
        /// <returns></returns>
        Task SetDataScope(long userId, string dataScopes);

        /// <summary>
        /// 获取数据范围缓存（机构Id）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<string> GetDataScope(string userId);

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        Task<string> GetCode(string timestamp);

        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="code">验证码</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        Task<bool> SetCode(string timestamp, string code, TimeSpan timeSpan);

        /// <summary>
        /// 删除验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        Task<bool> DelCode(string timestamp);

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<UserInfo> GetUserInfo(string userId);

        /// <summary>
        /// 用户登录信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        Task<bool> SetUserInfo(string userId, UserInfo userInfo, TimeSpan timeSpan);

        /// <summary>
        /// 删除用户登录信息缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<bool> DelUserInfo(string userId);

        /// <summary>
        /// 获取岗位列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        List<PositionCacheListOutput> GetPositionList(string userId);

        /// <summary>
        /// 保存岗位列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="positionList">岗位列表</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        bool SetPositionList(string userId, List<PositionCacheListOutput> positionList, TimeSpan timeSpan);

        /// <summary>
        /// 删除岗位列表缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<bool> DelPosition(string userId);

        /// <summary>
        /// 获取角色列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        List<RoleCacheListOutput> GetRoleList(string userId);

        /// <summary>
        /// 保存角色列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleList">角色列表</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        bool SetRoleList(string userId, List<RoleCacheListOutput> roleList, TimeSpan timeSpan);

        /// <summary>
        /// 删除岗位列表缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<bool> DelRole(string userId);

        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <returns></returns>
        List<UserOnlineModel> GetOnlineUserList(string tenantId);

        /// <summary>
        /// 保存在线用户列表
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <param name="onlineList">在线用户列表</param>
        /// <returns></returns>
        bool SetOnlineUserList(string tenantId, List<UserOnlineModel> onlineList);

        /// <summary>
        /// 删除在线用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        bool DelOnlineUser(string userId);

        /// <summary>
        /// 获取所有缓存关键字
        /// </summary>
        /// <returns></returns>
        List<string> GetAllCacheKeys();

        /// <summary>
        /// 删除指定关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool Del(string key);

        /// <summary>
        /// 删除指定关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<bool> DelAsync(string key);

        /// <summary>
        /// 删除指定关键字数组缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<bool> DelAsync(string[] key);

        /// <summary>
        /// 删除某特征关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<bool> DelByPatternAsync(string key);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool Set(string key, object value);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        bool Set(string key, object value, TimeSpan timeSpan);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<bool> SetAsync(string key, object value);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        Task<bool> SetAsync(string key, object value, TimeSpan timeSpan);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        string Get(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<string> GetAsync(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// 获取缓存过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        DateTime GetCacheOutTime(string key);

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// 异步检查给定 key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key);


        /// <summary>
        /// 获取唯一任务号
        /// </summary>
        /// <returns></returns>
        int GettaskId();

        /// <summary>
        /// 更新子任务状态 
        /// </summary>
        /// <param name="id">子任务id</param>
        /// <param name="siteId">站点id</param>
        /// <returns></returns>
        bool TaskUpadteSite(string id, string siteId,int process, ZjnWmsTaskDetailsEntity wmsTask);

        /// <summary>
        /// 更新子任务状态 
        /// </summary>
        /// <param name="id">子任务id</param>
        /// <param name="state">任务状态</param>
        /// <returns></returns>
        bool TaskReset(string id, int state);


        /// <summary>
        /// 获取所有子任务List 
        /// </summary>
        /// <returns></returns>
        Task<List<ZjnWmsTaskDetailsInfoOutput>> GetTaskList();

        /// <summary>
        /// 更新货位状态
        /// </summary>
        /// <param name="locationNo">货位编码</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        Task<ZjnWmsLocationEntity> UpdateLocationStatus(string locationNo, LocationStatus status);

        /// <summary>
        /// 压入Redis队列,避免出入同条线体时更新当前存储数量死锁(左边存储，右边取)
        /// </summary>
        /// <param name="zjnWmsLineSettingEntity"></param>
        /// <returns></returns>
        Task<long> LineStoragePush<T>(string queueKey, T obj);
        /// <summary>
        /// 从线体存储List队列中获取排队的信息(左边存储，右边取)
        /// </summary>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        Task<T> LineStorageRPop<T>(string queueKey);
    }
}