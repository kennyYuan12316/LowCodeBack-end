using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.LinqBuilder;
using HSZ.Logging.Attributes;
using HSZ.System.Entitys.Dto.System.SysLog;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HSZ.System.Service.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：系统日志
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "Log", Order = 211)]
    [Route("api/system/[controller]")]
    public class SysLogService : ISysLogService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<SysLogEntity> _sysLogRepository;
        private readonly SqlSugarScope Db;

        /// <summary>
        /// 初始化一个<see cref="SysLogService"/>类型的新实例
        /// </summary>
        /// <param name="sysLogRepository"></param>
        public SysLogService(ISqlSugarRepository<SysLogEntity> sysLogRepository)
        {
            _sysLogRepository = sysLogRepository;
            Db = DbScoped.SugarScope;
        }

        #region GET

        /// <summary>
        /// 获取系统日志列表-登录日志（带分页）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="Type">分类</param>
        /// <returns></returns>
        [HttpGet("{Type}")]
        //[OperateLog("系统日志", "")]
        public async Task<dynamic> GetList([FromQuery] LogListQuery input, int Type)
        {
            var whereLambda = LinqExpression.And<SysLogEntity>();
            whereLambda = whereLambda.And(x => x.Category == Type);
            var start = Ext.GetDateTime(input.startTime.ToString());
            var end = Ext.GetDateTime(input.endTime.ToString());
            if (input.endTime != null && input.startTime != null)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.CreatorTime, start, end));
            }
            //关键字（用户、IP地址、功能名称）
            if (!string.IsNullOrEmpty(input.keyword))
            {
                whereLambda = whereLambda.And(m => m.UserName.Contains(input.keyword) || m.IPAddress.Contains(input.keyword) || m.ModuleName.Contains(input.keyword));
            }

            if (input.ipaddress.IsNotEmptyOrNull())
            {
                whereLambda = whereLambda.And(m => m.IPAddress.Contains(input.ipaddress));
            }
            if (input.userName.IsNotEmptyOrNull())
            {
                whereLambda = whereLambda.And(m => m.UserName.Contains(input.userName));
            }
            if (input.moduleName.IsNotEmptyOrNull())
            {
                whereLambda = whereLambda.And(m => m.ModuleName == input.moduleName);
            }
            if (input.requestMethod.IsNotEmptyOrNull())
            {
                whereLambda = whereLambda.And(m => m.RequestMethod == input.requestMethod);
            }

            var list = await _sysLogRepository.AsQueryable().Where(whereLambda).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            object output = null;
            if (Type == 1)
            {
                var pageList = new SqlSugarPagedList<LogLoginOutput>()
                {
                    list = list.list.Adapt<List<LogLoginOutput>>(),
                    pagination = list.pagination
                };
                return PageResult<LogLoginOutput>.SqlSugarPageResult(pageList);
            }
            if (Type == 4)
            {
                var pageList = new SqlSugarPagedList<LogExceptionOutput>()
                {
                    list = list.list.Adapt<List<LogExceptionOutput>>(),
                    pagination = list.pagination
                };
                return PageResult<LogExceptionOutput>.SqlSugarPageResult(pageList);
            }
            if (Type == 5)
            {
                var pageList = new SqlSugarPagedList<LogRequestOutput>()
                {
                    list = list.list.Adapt<List<LogRequestOutput>>(),
                    pagination = list.pagination
                };
                return PageResult<LogRequestOutput>.SqlSugarPageResult(pageList);
            }
            if (Type == 3)
            {
                var pageList = new SqlSugarPagedList<LogOperationOutput>()
                {
                    list = list.list.Adapt<List<LogOperationOutput>>(),
                    pagination = list.pagination
                };
                return PageResult<LogOperationOutput>.SqlSugarPageResult(pageList);
            }
            return output;
        }

        /// <summary>
        /// 操作模块
        /// </summary>
        /// <returns></returns>
        [HttpGet("ModuleName")]
        public async Task<dynamic> ModuleNameSelector()
        {
            var taskMethods = App.EffectiveTypes
                    .Where(u => u.IsClass && !u.IsInterface && !u.IsAbstract && typeof(IDynamicApiController).IsAssignableFrom(u))
                    .SelectMany(u => u.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                    .Where(x => x.IsDefined(typeof(OperateLogAttribute), false))
                    .Select(x => new { moduleName = x.GetCustomAttribute<OperateLogAttribute>().ModuleName }).Distinct();
            return taskMethods;
        }

        #endregion

        #region POST

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task Delete([FromBody] LogDelInput input)
        {
            try
            {
                //开启事务
                Db.BeginTran();

                await _sysLogRepository.DeleteByIdsAsync(input.ids);

                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);
            }
        }

        /// <summary>
        /// 一键删除
        /// </summary>
        /// <param name="type">请求参数</param>
        /// <returns></returns>
        [HttpDelete("{type}")]
        public async Task Delete(int type)
        {
            try
            {
                //开启事务
                Db.BeginTran();
                await _sysLogRepository.DeleteAsync(x => x.Category == type);
                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);
            }
        }

        #endregion
    }
}
