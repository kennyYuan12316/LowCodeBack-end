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
using HSZ.wms.Entitys.Dto.ZjnWmsLocationGroup;
using HSZ.wms.Interfaces.ZjnWmsLocationGroup;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsLocationGroup
{
    /// <summary>
    /// 货位分组信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWmsLocationGroup", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsLocationGroupService : IZjnWmsLocationGroupService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsLocationGroupEntity> _zjnLocationGroupRepository;
        private readonly ISqlSugarRepository<ZjnWmsLocationGroupDetailsEntity> _zjnLocationGroupDetailsRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsLocationGroupService"/>类型的新实例
        /// </summary>
        public ZjnWmsLocationGroupService(ISqlSugarRepository<ZjnWmsLocationGroupEntity> zjnLocationGroupRepository,
            ISqlSugarRepository<ZjnWmsLocationGroupDetailsEntity> zjnLocationGroupDetailsRepository,
            IUserManager userManager)
        {
            _zjnLocationGroupRepository = zjnLocationGroupRepository;
            _zjnLocationGroupDetailsRepository = zjnLocationGroupDetailsRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取货位分组信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnLocationGroupRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsLocationGroupInfoOutput>();
            
            var zjnLocationGroupDetailsList = await _zjnLocationGroupDetailsRepository.GetListAsync(w => w.GroupCode == output.groupNo);
            output.zjnLocationGroupDetailsList = zjnLocationGroupDetailsList.Adapt<List<ZjnWmsLocationGroupDetailsInfoOutput>>();
            return output;
        }

        /// <summary>
        /// 判断货位组是否存在
        /// </summary>
        /// <param name="GroupNo"></param>
        /// <returns></returns>
        [HttpGet("ExistGroupNo")]
        public async Task<bool> ExistGroupNo(string GroupNo)
        {
            var output = await _zjnLocationGroupRepository.IsAnyAsync(p => p.GroupNo == GroupNo);
            return output;
        }

        /// <summary>
        /// 判断货位分组明细是否存在
        /// </summary>
        /// <param name="GroupDetailsNo"></param>
        /// <returns></returns>
        [HttpGet("ExistGroupDetailsNo")]
        public async Task<bool> ExistGroupDetailsNo(string GroupDetailsNo)
        {
            var output = await _zjnLocationGroupDetailsRepository.IsAnyAsync(p => p.GroupDetailsNo == GroupDetailsNo);
            return output;
        }

        /// <summary>
		/// 获取货位分组信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsLocationGroupListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_GroupNo" : input.sidx;
            var data = await _zjnLocationGroupRepository.AsSugarClient().Queryable<ZjnWmsLocationGroupEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_GroupNo), a => a.GroupNo.Contains(input.F_GroupNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_GroupName), a => a.GroupName.Contains(input.F_GroupName))
                .Select((a
)=> new ZjnWmsLocationGroupListOutput
                {
                    F_Id = a.Id,
                    F_GroupNo = a.GroupNo,
                    F_GroupName = a.GroupName,
                    F_Description = a.Description,
                    F_EnabledMark = SqlFunc.IIF(a.EnabledMark == 0, "禁用", "启用"),
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWmsLocationGroupListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建货位分组信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsLocationGroupCrInput input)
        {
         
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsLocationGroupEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateTime = DateTime.Now.ToString();
            entity.CreateUser = userInfo.userId;
            List<ZjnWmsLocationGroupDetailsCrInput> list = new List<ZjnWmsLocationGroupDetailsCrInput>();
            
            try
            {
                _db.BeginTran();
                var newEntity = await _zjnLocationGroupRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteReturnEntityAsync();
                
                var zjnLocationGroupDetailsEntityList = input.zjnLocationGroupDetailsList.Adapt<List<ZjnWmsLocationGroupDetailsEntity>>();
                if(zjnLocationGroupDetailsEntityList != null)
                {
                    foreach (var item in zjnLocationGroupDetailsEntityList)
                    {
                        item.Id = YitIdHelper.NextId().ToString();
                        item.GroupCode = newEntity.GroupNo;
                        item.CreateTime = DateTime.Now.ToString();
                        item.CreateUser = userInfo.userId;
                       
                    }
                    await _zjnLocationGroupDetailsRepository.AsInsertable(zjnLocationGroupDetailsEntityList).ExecuteCommandAsync();
                }
                
                _db.CommitTran();
            }
            catch (Exception ex)
            {
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1000);
            }
        }

        /// <summary>
        /// 更新货位分组信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsLocationGroupUpInput input)
        {
            var entity = input.Adapt<ZjnWmsLocationGroupEntity>();
            try
            {
                //开启事务
                _db.BeginTran();
                
                await _zjnLocationGroupRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                
                //清空原有数据
                await _zjnLocationGroupDetailsRepository.AsDeleteable().Where(it => it.GroupCode == entity.GroupNo).ExecuteCommandAsync();
                
                //新增新数据
                var zjnLocationGroupDetailsEntityList = input.zjnLocationGroupDetailsList.Adapt<List<ZjnWmsLocationGroupDetailsEntity>>();
                if(zjnLocationGroupDetailsEntityList != null)
                {
                    foreach (var item in zjnLocationGroupDetailsEntityList)
                    {
                        item.Id = YitIdHelper.NextId().ToString();
                        item.GroupCode = entity.GroupNo;
                    }
                    await _zjnLocationGroupDetailsRepository.AsInsertable(zjnLocationGroupDetailsEntityList).ExecuteCommandAsync();
                }
                
                //关闭事务
                _db.CommitTran();
            }
            catch (Exception)
            {
                //回滚事务
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);
            }
        }

        /// <summary>
        /// 删除货位分组信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnLocationGroupRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            try
            {
                //开启事务
                _db.BeginTran();
                
                await _zjnLocationGroupRepository.AsDeleteable().Where(it => it.Id == id).ExecuteCommandAsync();
                
                await _zjnLocationGroupDetailsRepository.AsDeleteable().Where(it => it.GroupCode.Equals(entity.GroupNo)).ExecuteCommandAsync();
                
                //关闭事务
                _db.CommitTran();
            }
            catch (Exception)
            {
                //回滚事务
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);
            }
        }
    }
}


