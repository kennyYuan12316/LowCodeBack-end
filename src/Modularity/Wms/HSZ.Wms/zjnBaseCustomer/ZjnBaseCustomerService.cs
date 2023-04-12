using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.wms.Entitys.Dto.ZjnBaseCustomer;
using HSZ.wms.Interfaces.ZjnBaseCustomer;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnBaseCustomer
{
    /// <summary>
    /// 客户信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnBaseCustomer", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseCustomerService : IZjnBaseCustomerService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseCustomerEntity> _zjnBaseCustomerRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseCustomerService"/>类型的新实例
        /// </summary>
        public ZjnBaseCustomerService(ISqlSugarRepository<ZjnBaseCustomerEntity> zjnBaseCustomerRepository,
            IUserManager userManager)
        {
            _zjnBaseCustomerRepository = zjnBaseCustomerRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取客户信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseCustomerRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnBaseCustomerInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取客户信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseCustomerListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnBaseCustomerRepository.AsSugarClient().Queryable<ZjnBaseCustomerEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_CustomerNo), a => a.CustomerNo.Contains(input.F_CustomerNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_CustomerName), a => a.CustomerName.Contains(input.F_CustomerName))
                .WhereIF(!string.IsNullOrEmpty(input.F_ContactName), a => a.ContactName.Contains(input.F_ContactName))
                .Select((a
)=> new ZjnBaseCustomerListOutput
                {
                    F_Id = a.Id,
                    F_CustomerNo = a.CustomerNo,
                    F_CustomerName = a.CustomerName,
                    F_CustomerPhone = a.CustomerPhone,
                    F_CustomerAddress = a.CustomerAddress,
                    F_ContactName = a.ContactName,
                    F_Description = a.Description,
                    F_EnabledMark = a.EnabledMark,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseCustomerListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建客户信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseCustomerCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseCustomerEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.LastModifyUserId = userInfo.userId;
            entity.LastModifyTime = DateTime.Now;
            entity.CreateUser = userInfo.userId;
            entity.CreateTime = DateTime.Now;

            var isOk = await _zjnBaseCustomerRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 批量删除客户信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseCustomerRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除客户信息
                    await _zjnBaseCustomerRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception)
                {
                    //回滚事务
                    _db.RollbackTran();
                    throw HSZException.Oh(ErrorCode.COM1002);
                }
            }
        }

        /// <summary>
        /// 更新客户信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseCustomerUpInput input)
        {
            var entity = input.Adapt<ZjnBaseCustomerEntity>();
            var isOk = await _zjnBaseCustomerRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除客户信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseCustomerRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _zjnBaseCustomerRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


