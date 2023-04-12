using HSZ.Common.Cache;
using HSZ.Common.Const;
using HSZ.Common.Enum;
using HSZ.Dependency;
using HSZ.Entitys.wms;
using HSZ.Message.Entitys.Model.IM;
using HSZ.System.Entitys.Dto.Permission.Position;
using HSZ.System.Entitys.Dto.Permission.Role;
using HSZ.System.Entitys.Model.Permission.User;
using HSZ.wms.Entitys.Dto.ZjnWmsTaskDetails;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Renci.SshNet.Security;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.Common.Core.Manager
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：缓存管理
    /// </summary>
    public class CacheManager : ICacheManager, IScoped
    {
        private readonly CacheOptions _cacheOptions;
        private readonly ICache _cache;

        /// <summary>
        /// 初始化一个<see cref="CacheManager"/>类型的新实例
        /// </summary>
        public CacheManager(IOptions<CacheOptions> cacheOptions,
            Func<string, ISingleton, object> resolveNamed)
        {
            _cacheOptions = cacheOptions.Value;
            _cache = resolveNamed(_cacheOptions.CacheType.ToString(), default) as ICache;
        }


        /// <summary>
        /// 用户是否存在
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>        
        public async Task<bool> ExistsUserAsync(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_USER + $"{userId}";
            return await ExistsAsync(cacheKey);
        }

        /// <summary>
        /// 获取数据范围缓存（机构Id）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>        
        public async Task<string> GetDataScope(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_DATASCOPE + $"{userId}";
            return await GetAsync<string>(cacheKey);
        }

        /// <summary>
        /// 缓存数据范围（机构Id集合）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="dataScopes">数据作用域</param>
        /// <returns></returns>        
        public async Task SetDataScope(long userId, string dataScopes)
        {
            var cacheKey = CommonConst.CACHE_KEY_DATASCOPE + $"{userId}";
            await SetAsync(cacheKey, dataScopes);
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>        
        public async Task<string> GetCode(string timestamp)
        {
            var cacheKey = CommonConst.CACHE_KEY_CODE + $"{timestamp}";
            return await GetAsync<string>(cacheKey);
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="code">验证码</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>        
        public async Task<bool> SetCode(string timestamp, string code, TimeSpan timeSpan)
        {
            var cacheKey = CommonConst.CACHE_KEY_CODE + $"{timestamp}";
            return await SetAsync(cacheKey, code, timeSpan);
        }

        /// <summary>
        /// 删除验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>        
        public Task<bool> DelCode(string timestamp)
        {
            var cacheKey = CommonConst.CACHE_KEY_CODE + $"{timestamp}";
            DelAsync(cacheKey);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取许可
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>        
        public async Task<List<dynamic>> GetPermission(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_PERMISSION + $"{userId}";
            return await GetAsync<List<dynamic>>(cacheKey);
        }

        /// <summary>
        /// 用户登录信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>        
        public async Task<bool> SetPermission(string userId, UserInfo userInfo)
        {
            var cacheKey = CommonConst.CACHE_KEY_PERMISSION + $"{userId}";
            return await SetAsync(cacheKey, userInfo);
        }

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>        
        public async Task<UserInfo> GetUserInfo(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_USER + $"{userId}";
            return await GetAsync<UserInfo>(cacheKey);
        }

        /// <summary>
        /// 用户登录信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userInfo"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public async Task<bool> SetUserInfo(string userId, UserInfo userInfo, TimeSpan timeSpan)
        {
            var cacheKey = CommonConst.CACHE_KEY_USER + $"{userId}";
            return await SetAsync(cacheKey, userInfo, timeSpan);
        }

        /// <summary>
        /// 删除用户登录信息缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Task<bool> DelUserInfo(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_USER + $"{userId}";
            DelAsync(cacheKey);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取岗位列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public List<PositionCacheListOutput> GetPositionList(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_POSITION + $"{userId}";
            return Get<List<PositionCacheListOutput>>(cacheKey);
        }

        /// <summary>
        /// 保存岗位列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="positionList">岗位列表</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        public bool SetPositionList(string userId, List<PositionCacheListOutput> positionList, TimeSpan timeSpan)
        {
            var cacheKey = CommonConst.CACHE_KEY_POSITION + $"{userId}";
            return Set(cacheKey, positionList, timeSpan);
        }

        /// <summary>
        /// 删除岗位列表缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Task<bool> DelPosition(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_POSITION + $"{userId}";
            DelAsync(cacheKey);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取角色列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public List<RoleCacheListOutput> GetRoleList(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_ROLE + $"{userId}";
            return Get<List<RoleCacheListOutput>>(cacheKey);
        }

        /// <summary>
        /// 保存角色列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleList">角色列表</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        public bool SetRoleList(string userId, List<RoleCacheListOutput> roleList, TimeSpan timeSpan)
        {
            var cacheKey = CommonConst.CACHE_KEY_ROLE + $"{userId}";
            return Set(cacheKey, roleList, timeSpan);
        }

        /// <summary>
        /// 删除角色列表缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Task<bool> DelRole(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_ROLE + $"{userId}";
            DelAsync(cacheKey);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <returns></returns>
        public List<UserOnlineModel> GetOnlineUserList(string tenantId)
        {
            var cacheKey = CommonConst.CACHE_KEY_ONLINE_USER + $"{tenantId}";
            return Get<List<UserOnlineModel>>(cacheKey);
        }

        /// <summary>
        /// 保存在线用户列表
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <param name="onlineList">在线用户列表</param>
        /// <returns></returns>
        public bool SetOnlineUserList(string tenantId, List<UserOnlineModel> onlineList)
        {
            var cacheKey = CommonConst.CACHE_KEY_ONLINE_USER + $"{tenantId}";
            return Set(cacheKey, onlineList);
        }

        /// <summary>
        /// 删除在线用户ID
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns></returns>
        public bool DelOnlineUser(string Id)
        {
            var tenantId = Id.Split('_').ToList().First();
            var userId = Id.Split('_').ToList().Last();
            var cacheKey = CommonConst.CACHE_KEY_ONLINE_USER + $"{tenantId}";
            var list = Get<List<UserOnlineModel>>(cacheKey);
            var online = list.Find(it => it.userId == userId);
            list.RemoveAll((x) => x.connectionId == online.connectionId);
            return Set(cacheKey, list);
        }

        /// <summary>
        /// 获取所有缓存关键字
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllCacheKeys()
        {
            var cacheItems = _cache.GetAllKeys();
            if (cacheItems == null) return new List<string>();
            return cacheItems.Where(u => !u.ToString().StartsWith("mini-profiler")).Select(u => u).ToList();
        }

        /// <summary>
        /// 删除指定关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool Del(string key)
        {
            _cache.Del(key);
            return true;
        }

        /// <summary>
        /// 删除指定关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public Task<bool> DelAsync(string key)
        {
            _cache.DelAsync(key);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 删除指定关键字数组缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public Task<bool> DelAsync(string[] key)
        {
            _cache.DelAsync(key);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 删除某特征关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public Task<bool> DelByPatternAsync(string key)
        {
            _cache.DelByPatternAsync(key);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Set(string key, object value)
        {
            return _cache.Set(key, value);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        public bool Set(string key, object value, TimeSpan timeSpan)
        {
            return _cache.Set(key, value, timeSpan);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<bool> SetAsync(string key, object value)
        {
            return await _cache.SetAsync(key, value);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        public async Task<bool> SetAsync(string key, object value, TimeSpan timeSpan)
        {
            return await _cache.SetAsync(key, value, timeSpan);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public string Get(string key)
        {
            return _cache.Get(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key)
        {
            return await _cache.GetAsync(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public Task<T> GetAsync<T>(string key)
        {
            return _cache.GetAsync<T>(key);
        }

        /// <summary>
        /// 获取缓存过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public DateTime GetCacheOutTime(string key)
        {
            return _cache.GetCacheOutTime(key);
        }

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return _cache.Exists(key);
        }

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public Task<bool> ExistsAsync(string key)
        {
            return _cache.ExistsAsync(key);
        }


        #region 任务
        /// <summary>
        /// 获取唯一任务号
        /// </summary>
        /// <returns></returns>
        public int GettaskId()
        {
            RedisHelper.SetNx(CommonConst.CACHE_KEY_TASKID, 1000000000);
            long TaskId = RedisHelper.IncrBy(CommonConst.CACHE_KEY_TASKID);
            if (TaskId == Int32.MaxValue) RedisHelper.Del(CommonConst.CACHE_KEY_TASKID);
            return Convert.ToInt32(TaskId);
        }

        /// <summary>
        /// 更新子任务状态 
        /// </summary>
        /// <param name="id">子任务id</param>
        /// <param name="state">任务状态</param>
        /// <returns></returns>
        public bool TaskReset(string id, int state)
        {
            var TaskDetails = RedisHelper.HGet<ZjnWmsTaskDetailsInfoOutput>(CommonConst.CACHE_KEY_TASK_LIST, id);
            if (TaskDetails == null) return true;
            TaskDetails.taskDetailsStates = state;
            RedisHelper.HSet(CommonConst.CACHE_KEY_TASK_LIST, id, TaskDetails);
            return true;
            
        }
        /// <summary>
        /// 更新子任务状态 
        /// </summary>
        /// <param name="id">子任务id</param>
        /// <param name="siteId">站点id</param>
        /// <returns></returns>
        public bool TaskUpadteSite(string id, string siteId,int process, ZjnWmsTaskDetailsEntity wmsTask)
        {
            var TaskDetails = RedisHelper.HGet<ZjnWmsTaskDetailsInfoOutput>(CommonConst.CACHE_KEY_TASK_LIST, id);
            if (process!=6)
            {
                TaskDetails.taskDetailsEnd = siteId;
                TaskDetails.rowEnd= wmsTask.RowEnd;
                TaskDetails.cellEnd = wmsTask.CellEnd;
                TaskDetails.layerEnd=wmsTask.LayerEnd;
                TaskDetails.cellStart= wmsTask.CellStart;
                TaskDetails.layerStart= wmsTask.LayerStart;
                TaskDetails.rowStart= wmsTask.RowStart;
            }
            RedisHelper.HSet(CommonConst.CACHE_KEY_TASK_LIST, id, TaskDetails);
            return true;
        }

        /// <summary>
        /// 获取所有子任务List 
        /// </summary>
        /// <returns></returns>
        public async Task<List<ZjnWmsTaskDetailsInfoOutput>> GetTaskList()
        {
            var tasklist = await RedisHelper.HGetAllAsync<ZjnWmsTaskDetailsInfoOutput>(CommonConst.CACHE_KEY_TASK_LIST);
            List<ZjnWmsTaskDetailsInfoOutput> tList = new List<ZjnWmsTaskDetailsInfoOutput>(tasklist.Values);
            return tList;
        }

        /// <summary>
        /// 更新货位状态
        /// </summary>
        /// <param name="locationNo">货位编码</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public async Task<ZjnWmsLocationEntity> UpdateLocationStatus(string locationNo, LocationStatus status)
        {
            var location = await RedisHelper.HGetAsync<ZjnWmsLocationEntity>(CommonConst.CACHE_KEY_LOCATION_LIST, locationNo);
            location.LocationStatus = ((int)status).ToString();
            await RedisHelper.HSetAsync(CommonConst.CACHE_KEY_LOCATION_LIST, locationNo, location);
            return location;
        }

        #endregion

        /// <summary>
        /// 压入Redis队列,避免出入同条线体时更新当前存储数量死锁(左边存储，右边取,必须和获取同个队列)
        /// </summary>
        /// <param name="zjnWmsLineSettingEntity"></param>
        public async Task<long> LineStoragePush<T>(string queueKey, T obj)
        {
            //将每条线体的出入信息存入到Redis队列中,这里采用List方式存储,每一种组盘信息就是一个队列
            return await RedisHelper.LPushAsync(queueKey, JsonConvert.SerializeObject(obj));            
        }
        /// <summary>
        /// 从线体存储List队列中获取排队的信息(左边存储，右边取,必须和存入同个队列)
        /// </summary>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        public async Task<T> LineStorageRPop<T>(string queueKey)
        {
            //从排队List中获取排队的信息
            var obj = await RedisHelper.RPopAsync(queueKey);
            if (obj == null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(obj);
        }
    }
}