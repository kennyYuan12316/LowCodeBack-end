using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Extend.Entitys;
using HSZ.Extend.Entitys.Dto.Schedule;
using HSZ.FriendlyException;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.Extend
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：项目计划
    /// </summary>
    [ApiDescriptionSettings(Tag = "Extend", Name = "Schedule", Order = 600)]
    [Route("api/extend/[controller]")]
    public class ScheduleService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ScheduleEntity> _scheduleRepository;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="ScheduleService"/>类型的新实例
        /// </summary>
        public ScheduleService(ISqlSugarRepository<ScheduleEntity> scheduleRepository,
            IUserManager userManager)
        {
            _scheduleRepository = scheduleRepository;
            _userManager = userManager;
        }

        #region GET

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ScheduleListQuery input)
        {
            var data = await _scheduleRepository.AsQueryable()
                .Where(x => x.CreatorUserId == _userManager.UserId && x.StartTime >= input.startTime.ToDate() && x.EndTime <= input.endTime.ToDate() && x.DeleteMark == null).OrderBy(x => x.StartTime, OrderByType.Desc).ToListAsync();
            var output = data.Adapt<List<ScheduleListOutput>>();
            return new { list = output };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = (await _scheduleRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null)).Adapt<ScheduleInfoOutput>();
            return data;
        }

        /// <summary>
        /// app
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("AppList")]
        public async Task<dynamic> GetAppList([FromQuery] ScheduleListQuery input)
        {
            var days = new Dictionary<string, int>();
            var data = await _scheduleRepository.AsQueryable().Where(x => x.CreatorUserId == _userManager.UserId && x.StartTime >= input.startTime.ToDate() && x.EndTime <= input.endTime.ToDate() && x.DeleteMark == null).OrderBy(x => x.StartTime, OrderByType.Desc).ToListAsync();
            var output = data.Adapt<List<ScheduleListOutput>>();
            foreach (var item in GetAllDays(input.startTime.ToDate(), input.endTime.ToDate()))
            {
                var _startTime = item.ToString("yyyy-MM-dd") + " 23:59";
                var _endTime = item.ToString("yyyy-MM-dd") + " 00:00";
                var count = output.FindAll(m => m.startTime <= _startTime.ToDate() && m.endTime >= _endTime.ToDate()).Count;
                days.Add(item.ToString("yyyyMMdd"), count);
            }
            var today_startTime = input.dateTime + " 23:59";
            var today_endTime = input.dateTime + " 00:00";
            return new
            {
                signList = days,
                todayList = output.FindAll(m => m.startTime <= today_startTime.ToDate() && m.endTime >= today_endTime.ToDate())
            };
        }

        #endregion

        #region POST

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ScheduleCrInput input)
        {
            var entity = input.Adapt<ScheduleEntity>();
            var isOk = await _scheduleRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ScheduleUpInput input)
        {
            var entity = input.Adapt<ScheduleEntity>();
            var isOk = await _scheduleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _scheduleRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _scheduleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync(); ;
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1002);
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 获取固定日期范围内的所有日期，以数组形式返回
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        private DateTime[] GetAllDays(DateTime startTime, DateTime endTime)
        {
            var listDay = new List<DateTime>();
            DateTime dtDay = new DateTime();
            //循环比较，取出日期；
            for (dtDay = startTime; dtDay.CompareTo(endTime) <= 0; dtDay = dtDay.AddDays(1))
            {
                listDay.Add(dtDay);
            }
            return listDay.ToArray();
        }

        #endregion
    }
}
