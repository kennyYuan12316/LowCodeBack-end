using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.Permission.User;
using HSZ.System.Entitys.Dto.Permission.UserRelation;
using HSZ.System.Entitys.Permission;
using HSZ.System.Interfaces.Permission;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSZ.System.Service.Permission
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：业务实现：用户关系
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "UserRelation", Order = 169)]
    [Route("api/permission/[controller]")]
    public class UserRelationService : IUserRelationService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<UserRelationEntity> _userRelationRepository;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope Db;

        /// <summary>
        /// 初始化一个<see cref="UserRelationService"/>类型的新实例
        /// </summary>
        /// <param name="userRelationRepository"></param>
        /// <param name="userRepository"></param>
        public UserRelationService(ISqlSugarRepository<UserRelationEntity> userRelationRepository, IUserManager userManager,
            ISqlSugarRepository<UserEntity> userRepository)
        {
            _userRelationRepository = userRelationRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            Db = DbScoped.SugarScope;
        }

        #region Get

        /// <summary>
        /// 获取岗位/角色成员列表
        /// </summary>
        /// <param name="objectId">岗位id或角色id</param>
        /// <returns></returns>
        [HttpGet("{objectId}")]
        public async Task<dynamic> GetListByObjectId(string objectId)
        {
            var data = await _userRelationRepository.AsQueryable().Where(u => u.ObjectId == objectId).Select(s => s.UserId).ToListAsync();
            return new { ids = data };
        }

        #endregion

        #region Post

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="objectId">功能主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("{objectId}")]
        public async Task Create(string objectId, [FromBody] UserRelationCrInput input)
        {
            //只能超管才能变更
            if (input.objectType == "Role" && !_userManager.IsAdministrator)
                throw HSZException.Oh(ErrorCode.D1612);

            var oldUserIds = await _userRelationRepository.AsQueryable().Where(u => u.ObjectId.Equals(objectId) && u.ObjectType.Equals(input.objectType)).Select(s => s.UserId).ToListAsync();
            try
            {
                //开启事务
                Db.BeginTran();
                //清空原有数据
                await _userRelationRepository.DeleteAsync(u => u.ObjectId.Equals(objectId) && u.ObjectType.Equals(input.objectType));
                //创建新数据
                var dataList = new List<UserRelationEntity>();
                input.userIds.ForEach(item =>
                {
                    dataList.Add(new UserRelationEntity()
                    {
                        UserId = item,
                        ObjectType = input.objectType,
                        ObjectId = objectId,
                        SortCode = input.userIds.IndexOf(item)
                    });
                });
                if (dataList.Count > 0)
                {
                    await _userRelationRepository.AsInsertable(dataList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                }
                // 修改用户
                // 计算旧用户数组与新用户数组差
                var addList = input.userIds.Except(oldUserIds).ToList();
                var delList = oldUserIds.Except(input.userIds).ToList();
                //处理新增用户岗位
                if (addList.Count > 0)
                {
                    var addUserList = await _userRepository.AsQueryable().In(u => u.Id, addList.ToArray()).ToListAsync();
                    addUserList.ForEach(item =>
                    {
                        if (input.objectType.Equals("Position"))
                        {
                            var idList = string.IsNullOrEmpty(item.PositionId) ? new List<string>() : item.PositionId.Split(',').ToList();
                            idList.Add(objectId);

                            #region 获取默认组织下的岗位
                            if (item.PositionId.IsNullOrEmpty())
                            {
                                var pIdList = _userRelationRepository.AsSugarClient().Queryable<PositionEntity>()
                                .Where(x => x.OrganizeId == item.OrganizeId && idList.Contains(x.Id)).Select(x => x.Id).ToList();
                                item.PositionId = pIdList.FirstOrDefault();//多 岗位 默认取第一个
                            }
                            #endregion
                        }
                        else if (input.objectType.Equals("Role"))
                        {
                            var idList = string.IsNullOrEmpty(item.RoleId) ? new List<string>() : item.RoleId.Split(',').ToList();
                            idList.Add(objectId);
                            item.RoleId = string.Join(",", idList.ToArray()).TrimStart(',').TrimEnd(',');
                        }
                        else if(input.objectType.Equals("Group"))
                        {
                            var idList = string.IsNullOrEmpty(item.GroupId) ? new List<string>() : item.GroupId.Split(',').ToList();
                            idList.Add(objectId);
                            item.GroupId = string.Join(",", idList.ToArray()).TrimStart(',').TrimEnd(',');
                        }
                    });
                    await _userRepository.AsUpdateable(addUserList).UpdateColumns(it => new { it.RoleId, it.PositionId,it.GroupId }).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                }

                //出来移除用户
                if (delList.Count > 0)
                {
                    var delUserList = await _userRepository.AsQueryable().In(u => u.Id, delList.ToArray()).ToListAsync();
                    foreach(var item in delUserList)
                    {
                        if (input.objectType.Equals("Position"))
                        {
                            if (item.PositionId.IsNotEmptyOrNull())
                            {
                                var idList = item.PositionId.Split(',').ToList();
                                idList.RemoveAll(x => x == objectId);
                            }

                            #region 获取默认组织下的岗位
                            var pList = await _userRepository.AsSugarClient().Queryable<PositionEntity>().Where(x => x.OrganizeId == item.OrganizeId).Select(x => x.Id).ToListAsync();
                            var pIdList = await _userRelationRepository.AsQueryable().Where(x => x.UserId == item.Id && x.ObjectType == "Position" && pList.Contains(x.ObjectId)).Select(x => x.ObjectId).ToListAsync();

                            item.PositionId = pIdList.FirstOrDefault();//多 岗位 默认取第一个
                            #endregion

                            await _userRepository.AsSugarClient().Updateable<UserEntity>().SetColumns(it => new UserEntity()
                            {
                                PositionId = item.PositionId,
                                LastModifyTime = SqlFunc.GetDate(),
                                LastModifyUserId = item.Id
                            }).Where(it => it.Id == item.Id).ExecuteCommandAsync();
                        }
                        else if (input.objectType.Equals("Role"))
                        {
                            if (item.RoleId.IsNotEmptyOrNull())
                            {
                                var idList = item.RoleId.Split(',').ToList();
                                idList.RemoveAll(x => x == objectId);

                                #region 多组织 优先选择有权限组织
                                var defaultOrgId = item.OrganizeId;
                                item.OrganizeId = "";

                                var roleList = await _userManager.GetUserOrgRoleIds(string.Join(",", idList), item.OrganizeId);
                                //如果该组织下有角色并且有角色权限 则为默认组织
                                if (roleList.Any() && _userRepository.AsSugarClient().Queryable<AuthorizeEntity>().Where(x => x.ObjectType == "Role" && x.ItemType == "module" && roleList.Contains(x.ObjectId)).Any())
                                    item.OrganizeId = defaultOrgId;//多 组织 默认

                                if (item.OrganizeId.IsNullOrEmpty())
                                {
                                    var orgList = await _userRelationRepository.AsQueryable().Where(x => x.ObjectType == "Organize" && x.UserId == item.Id).Select(x => x.ObjectId).ToListAsync();//多 组织
                                    foreach (var it in orgList)
                                    {
                                        roleList = await _userManager.GetUserOrgRoleIds(string.Join(",", idList), it);

                                        //如果该组织下有角色并且有角色权限 则为默认组织
                                        if (roleList.Any() && _userRepository.AsSugarClient().Queryable<AuthorizeEntity>().Where(x => x.ObjectType == "Role" && x.ItemType == "module" && roleList.Contains(x.ObjectId)).Any())
                                        {
                                            item.OrganizeId = it;//多 组织 默认
                                            break;
                                        }
                                    }
                                }
                                if (item.OrganizeId.IsNullOrEmpty())//如果所选组织下都没有角色或者没有角色权限
                                    item.OrganizeId = defaultOrgId;//多 组织 默认
                                #endregion

                                await _userRepository.AsSugarClient().Updateable<UserEntity>().SetColumns(it => new UserEntity()
                                {
                                    RoleId = string.Join(",", idList.ToArray()).TrimStart(',').TrimEnd(','),
                                    LastModifyTime = SqlFunc.GetDate(),
                                    OrganizeId = item.OrganizeId,
                                    LastModifyUserId = _userManager.UserId
                                }).Where(it => it.Id == item.Id).ExecuteCommandAsync();
                            }
                        }
                        else if (input.objectType.Equals("Group"))
                        {
                            if (item.GroupId.IsNotEmptyOrNull())
                            {
                                var idList = item.GroupId.Split(',').ToList();
                                idList.RemoveAll(x => x == objectId);
                                await _userRepository.AsSugarClient().Updateable<UserEntity>().SetColumns(it => new UserEntity()
                                {
                                    GroupId = string.Join(",", idList.ToArray()).TrimStart(',').TrimEnd(','),
                                    LastModifyTime = SqlFunc.GetDate(),
                                    LastModifyUserId = item.Id
                                }).Where(it => it.Id == item.Id).ExecuteCommandAsync();
                            }
                        }
                    }
                }
                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 创建用户岗位关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">岗位ID</param>
        /// <returns></returns>
        [NonAction]
        public List<UserRelationEntity> CreateByPosition(string userId, string ids)
        {
            List<UserRelationEntity> userRelationList = new List<UserRelationEntity>();
            if (!ids.IsNullOrEmpty())
            {
                var position = new List<string>(ids.Split(','));
                position.ForEach(item =>
                {
                    var entity = new UserRelationEntity();
                    entity.ObjectType = "Position";
                    entity.ObjectId = item;
                    entity.SortCode = position.IndexOf(item);
                    entity.UserId = userId;
                    userRelationList.Add(entity);
                });
            }
            return userRelationList;
        }

        /// <summary>
        /// 创建用户角色关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">角色ID</param>
        /// <returns></returns>
        [NonAction]
        public List<UserRelationEntity> CreateByRole(string userId, string ids)
        {
            List<UserRelationEntity> userRelationList = new List<UserRelationEntity>();
            if (!ids.IsNullOrEmpty())
            {
                var position = new List<string>(ids.Split(','));
                position.ForEach(item =>
                {
                    var entity = new UserRelationEntity();
                    entity.ObjectType = "Role";
                    entity.ObjectId = item;
                    entity.SortCode = position.IndexOf(item);
                    entity.UserId = userId;
                    userRelationList.Add(entity);
                });
            }
            return userRelationList;
        }

        /// <summary>
        /// 创建用户组织关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">组织ID</param>
        /// <returns></returns>
        [NonAction]
        public List<UserRelationEntity> CreateByOrganize(string userId, string ids)
        {
            List<UserRelationEntity> userRelationList = new List<UserRelationEntity>();
            if (!ids.IsNullOrEmpty())
            {
                var position = new List<string>(ids.Split(','));
                position.ForEach(item =>
                {
                    var entity = new UserRelationEntity();
                    entity.ObjectType = "Organize";
                    entity.ObjectId = item;
                    entity.SortCode = position.IndexOf(item);
                    entity.UserId = userId;
                    userRelationList.Add(entity);
                });
            }
            return userRelationList;
        }

        /// <summary>
        /// 创建用户分组关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">分组ID</param>
        /// <returns></returns>
        [NonAction]
        public List<UserRelationEntity> CreateByGroup(string userId, string ids)
        {
            List<UserRelationEntity> userRelationList = new List<UserRelationEntity>();
            if (!ids.IsNullOrEmpty())
            {
                var position = new List<string>(ids.Split(','));
                position.ForEach(item =>
                {
                    var entity = new UserRelationEntity();
                    entity.ObjectType = "Group";
                    entity.ObjectId = item;
                    entity.SortCode = position.IndexOf(item);
                    entity.UserId = userId;
                    userRelationList.Add(entity);
                });
            }
            return userRelationList;
        }

        /// <summary>
        /// 创建用户关系
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Create(List<UserRelationEntity> input)
        {
            try
            {
                //开启事务
                Db.BeginTran();

                await _userRelationRepository.AsInsertable(input).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();

                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task Delete(string id)
        {
            try
            {
                //开启事务
                Db.BeginTran();

                await _userRelationRepository.DeleteAsync(u => u.UserId == id);

                Db.CommitTran();
            }
            catch (Exception)
            {
                Db.RollbackTran();
                throw HSZException.Oh(ErrorCode.D5003);
            }
        }

        /// <summary>
        /// 根据用户主键获取列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<UserRelationEntity>> GetListByUserId(string userId)
        {
            return await _userRelationRepository.AsQueryable().Where(m => m.UserId == userId).OrderBy(o => o.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 获取岗位
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetPositionId(string userId)
        {
            var data = await _userRelationRepository.AsQueryable().Where(m => m.UserId == userId && m.ObjectType == "Position").OrderBy(o => o.CreatorTime).ToListAsync();
            return data.Select(m => m.ObjectId).ToList();
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objId"></param>
        /// <returns></returns>
        [NonAction]
        public List<string> GetUserId(string type, string objId)
        {
            var data = _userRelationRepository.AsQueryable().Where(x => x.ObjectId == objId && x.ObjectType == type).Select(x => x.UserId).Distinct().ToList();
            return data;
        }

        /// <summary>
        /// 获取用户(分页)
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="objIds"></param>
        /// <param name="pageInputBase"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetUserIdPage(List<string> userIds, List<string> objIds, PageInputBase pageInputBase)
        {
            var list1 =_userRelationRepository.AsSugarClient().Queryable<UserRelationEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.UserId == b.Id))
               .Where(a => a.ObjectType != "Organize").In(a => a.ObjectId, objIds.ToArray())
               .WhereIF(pageInputBase.keyword.IsNotEmptyOrNull(), (a, b) => b.RealName.Contains(pageInputBase.keyword) || b.Account.Contains(pageInputBase.keyword))
               .Select((a, b) => new UserIdPageListOutput()
               {
                   userId = a.UserId,
                   userName = SqlFunc.MergeString(b.RealName, "/", b.Account),
               });

            var list2 = _userRepository.AsQueryable().Where(x=>x.DeleteMark==null).In(x=>x.Id, userIds.ToArray())
                .Select(a=>new UserIdPageListOutput()
                {
                userId = a.Id,
                userName = SqlFunc.MergeString(a.RealName, "/", a.Account),
                });
            var output = await _userRelationRepository.AsSugarClient().UnionAll(list1, list2).MergeTable().ToPagedListAsync(pageInputBase.currentPage, pageInputBase.pageSize);
           
            return PageResult<UserIdPageListOutput>.SqlSugarPageResult(output);
        }

        #endregion
    }
}
