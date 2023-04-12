using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Extend.Entitys;
using HSZ.Extend.Entitys.Dto.TableExample;
using HSZ.Extend.Entitys.Model;
using HSZ.FriendlyException;
using HSZ.LinqBuilder;
using HSZ.System.Entitys.Entity.System;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Permission;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.Extend
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：表格示例数据
    /// </summary>
    [ApiDescriptionSettings(Tag = "Extend", Name = "TableExample", Order = 600)]
    [Route("api/extend/[controller]")]
    public class TableExampleService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<TableExampleEntity> _tableExampleRepository;
        private readonly ISqlSugarRepository<UserEntity> _usersRepository;
        private readonly ISqlSugarRepository<ProvinceEntity> _provinceRepository;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="TableExampleService"/>类型的新实例
        /// </summary>
        public TableExampleService(ISqlSugarRepository<TableExampleEntity> tableExampleRepository,
            ISqlSugarRepository<UserEntity> usersRepository,
            ISqlSugarRepository<ProvinceEntity> provinceRepository,
           IUserManager userManager)
        {
            _tableExampleRepository = tableExampleRepository;
            _usersRepository = usersRepository;
            _provinceRepository = provinceRepository;
            _userManager = userManager;
        }

        #region GET

        /// <summary>
        /// 获取表格数据列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] PageInputBase input)
        {
            return await GetPageList("", input);
        }

        /// <summary>
        /// 列表（树形表格）
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("ControlSample/{typeId}")]
        public async Task<dynamic> GetList(string typeId, [FromQuery] PageInputBase input)
        {
            return await GetPageList(typeId, input);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("All")]
        public async Task<dynamic> GetListAll([FromQuery] KeywordInput input)
        {
            var list = await _tableExampleRepository.AsSugarClient().Queryable<TableExampleEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.Registrant == b.Id))
                .WhereIF(input.keyword.IsNotEmptyOrNull(), a => a.ProjectCode.Contains(input.keyword) || a.ProjectName.Contains(input.keyword) || a.CustomerName.Contains(input.keyword)).OrderBy(a => a.RegisterDate, OrderByType.Desc)
                .Select((a, b) => new TableExampleAllOutput()
                {
                    id = a.Id,
                    interactionDate = a.InteractionDate,
                    projectCode = a.ProjectCode,
                    projectName = a.ProjectName,
                    principal = a.Principal,
                    jackStands = a.JackStands,
                    projectType = a.ProjectType,
                    projectPhase = a.ProjectPhase,
                    customerName = a.CustomerName,
                    costAmount = a.CostAmount,
                    tunesAmount = a.TunesAmount,
                    projectedIncome = a.ProjectedIncome,
                    registerDate = a.RegisterDate,
                    registrant = SqlFunc.MergeString(b.RealName, "/", b.Account),
                    description = a.Description
                }).ToListAsync();
            return new { list = list };
        }

        /// <summary>
        /// 获取延伸扩展列表(行政区划)
        /// </summary>
        /// <returns></returns>
        [HttpGet("IndustryList")]
        public async Task<dynamic> GetIndustryList([FromQuery] KeywordInput input)
        {
            var data = (await _provinceRepository.AsQueryable().WhereIF(input.keyword.IsNotEmptyOrNull(), x => x.EnCode.Contains(input.keyword) || x.FullName.Contains(input.keyword)).Where(m => m.ParentId == "-1" && m.DeleteMark == null).ToListAsync()).Adapt<List<TableExampleIndustryListOutput>>();
            return new { list = data };
        }

        /// <summary>
        /// 获取城市信息列表(获取延伸扩展列表(行政区划))
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CityList/{id}")]
        public async Task<dynamic> GetCityList(string id)
        {
            var data = (await _provinceRepository.GetListAsync(m => m.ParentId == id && m.DeleteMark == null)).Adapt<List<TableExampleCityListOutput>>();
            return new { list = data };
        }

        /// <summary>
        /// 列表（表格树形）
        /// </summary>
        /// <returns></returns>
        [HttpGet("ControlSample/TreeList")]
        public async Task<dynamic> GetTreeList(string isTree)
        {
            var dicData = await _provinceRepository.AsSugarClient().Queryable<DictionaryDataEntity, DictionaryTypeEntity>((a, b) =>
               new JoinQueryInfos(JoinType.Left, a.DictionaryTypeId == b.Id)).Where((a, b) => b.EnCode == "IndustryType" && a.DeleteMark == null
               && b.DeleteMark == null).Select(a => a).ToListAsync();
            var data = dicData.FindAll(x => x.EnabledMark == 1);
            var treeList = data.Select(x => new TableExampleTreeListOutput()
            {
                id = x.Id,
                parentId = x.ParentId,
                text = x.FullName,
                loaded = true,
                expanded = true,
                ht = x.Adapt<Dictionary<string, object>>()
            }).ToList();
            var output = isTree.IsNotEmptyOrNull() && "1".Equals(isTree) ? treeList.ToTree() : treeList;
            return new { list = output };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = (await _tableExampleRepository.GetFirstAsync(x => x.Id == id)).Adapt<TableExampleInfoOutput>();
            return data;
        }

        /// <summary>
        /// 获取批注
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Actions/Postil")]
        public async Task<dynamic> GetPostil(string id)
        {
            var tableExampleEntity = await _tableExampleRepository.GetFirstAsync(x => x.Id == id);
            if (tableExampleEntity == null)
                throw HSZException.Oh(ErrorCode.COM1007);
            return new { postilJson = tableExampleEntity.PostilJson };
        }
        #endregion

        #region POST

        /// <summary>
        /// 删除
         /// </summary>
        /// <param name="id">主键值</param>
         /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _tableExampleRepository.GetFirstAsync(x => x.Id == id);
            if (entity != null)
            {
                await _tableExampleRepository.DeleteAsync(entity);
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] TableExampleCrInput input)
        {
            var entity = input.Adapt<TableExampleEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.RegisterDate = DateTime.Now;
            entity.Registrant = _userManager.UserId;
            entity.CostAmount = entity.CostAmount == null ? 0 : entity.CostAmount;
            entity.TunesAmount = entity.TunesAmount == null ? 0 : entity.TunesAmount;
            entity.ProjectedIncome = entity.ProjectedIncome == null ? 0 : entity.ProjectedIncome;
            entity.Sign = "0000000";
            await _tableExampleRepository.InsertAsync(entity);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] TableExampleUpInput input)
        {
            var entity = input.Adapt<TableExampleEntity>();
            entity.Id = id;
            entity.LastModifyTime = DateTime.Now;
            entity.LastModifyUserId = _userManager.UserId;
            entity.CostAmount = entity.CostAmount == null ? 0 : entity.CostAmount;
            entity.TunesAmount = entity.TunesAmount == null ? 0 : entity.TunesAmount;
            entity.ProjectedIncome = entity.ProjectedIncome == null ? 0 : entity.ProjectedIncome;
            await _tableExampleRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新标签
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("UpdateSign/{id}")]
        public async Task UpdateSign(string id, [FromBody] TableExampleSignUpInput input)
        {
            var tableExampleEntity = await _tableExampleRepository.GetFirstAsync(x => x.Id == id);
            tableExampleEntity.Sign = input.sign;
            tableExampleEntity.Id = id;
            tableExampleEntity.LastModifyTime = DateTime.Now;
            tableExampleEntity.LastModifyUserId = _userManager.UserId;
            await _tableExampleRepository.AsUpdateable(tableExampleEntity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
        }

        /// <summary>
        /// 行编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/RowsEdit")]
        public async Task RowEditing(string id, [FromBody] TableExampleRowUpInput input)
        {
            var entity = input.Adapt<TableExampleEntity>();
            entity.Id = id;
            entity.CostAmount = entity.CostAmount == null ? 0 : entity.CostAmount;
            entity.TunesAmount = entity.TunesAmount == null ? 0 : entity.TunesAmount;
            entity.ProjectedIncome = entity.ProjectedIncome == null ? 0 : entity.ProjectedIncome;
            var tableExampleEntity = await _tableExampleRepository.GetFirstAsync(x => x.Id == id);
            var exampleEntity = BindModelValue(tableExampleEntity, entity);
            exampleEntity.LastModifyTime = DateTime.Now;
            exampleEntity.LastModifyUserId = _userManager.UserId;
            await _tableExampleRepository.AsUpdateable(tableExampleEntity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
        }


        /// <summary>
        /// 发送批注
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{id}/Postil")]
        public async Task SendPostil(string id, [FromBody] TableExamplePostilSendInput input)
        {
            var tableExampleEntity = await _tableExampleRepository.GetFirstAsync(x => x.Id == id);
            if (tableExampleEntity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            var model = new PostilModel()
            {
                userId = await GetUserName(_userManager.UserId),
                text = input.text,
                creatorTime = DateTime.Now
            };
            var list = new List<PostilModel>();
            list.Add(model);
            if (tableExampleEntity.PostilJson.IsNotEmptyOrNull())
            {
                list = list.Concat(tableExampleEntity.PostilJson.ToList<PostilModel>()).ToList();
            }
            var postilJson = list.ToJson();
            tableExampleEntity.PostilJson = postilJson;
            tableExampleEntity.PostilCount = list.Count;
            tableExampleEntity.Id = id;
            tableExampleEntity.LastModifyTime = DateTime.Now;
            tableExampleEntity.LastModifyUserId = _userManager.UserId;
            await _tableExampleRepository.AsUpdateable(tableExampleEntity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除批注
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="index">请求参数</param>
        /// <returns></returns>
        [HttpDelete("{id}/Postil/{index}")]
        public async Task DeletePostil(string id, int index)
        {
            var tableExampleEntity = await _tableExampleRepository.GetFirstAsync(x => x.Id == id);
            if (tableExampleEntity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            var list = tableExampleEntity.PostilJson.ToList<PostilModel>();
            list.Remove(list[index]);
            tableExampleEntity.PostilJson = list.ToJson();
            tableExampleEntity.PostilCount = list.Count;
            tableExampleEntity.Id = id;
            tableExampleEntity.LastModifyTime = DateTime.Now;
            tableExampleEntity.LastModifyUserId = _userManager.UserId;
            await _tableExampleRepository.AsUpdateable(tableExampleEntity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
        }
        #endregion

        #region PrivateMethod

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<dynamic> GetPageList(string typeId, PageInputBase input)
        {
            var queryWhere = LinqExpression.And<TableExampleEntity>();
            var queryParam = input.queryJson.ToObject();
            //if (typeId.IsNotEmptyOrNull())
            //{
            //    queryWhere = queryWhere.And(a => a.ProjectType == typeId);
            //}
            //关键字（项目编码、项目名称、客户名称）
            if (input.keyword.IsNotEmptyOrNull())
            {
                var keyword = input.keyword;
                queryWhere = queryWhere.And(a => a.ProjectCode.Contains(input.keyword) || a.ProjectName.Contains(input.keyword));
            }
            //标签查询
            if (queryParam["sign"].IsNotEmptyOrNull())
            {
                var index = 0;
                var arraySign = queryParam["sign"].ToString().Split(',');
                foreach (var item in arraySign)
                {
                    queryWhere = index == 0 ? queryWhere.And(a => a.Sign.Contains(item)) : queryWhere.Or(a => a.Sign.Contains(item));
                    index++;
                }
            }
            var list = await _tableExampleRepository.AsSugarClient().Queryable<TableExampleEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.Registrant == b.Id))
                .WhereIF(typeId.IsNotEmptyOrNull(), a => a.ProjectType == typeId).Where(queryWhere).OrderBy(a => a.RegisterDate, OrderByType.Desc)
                .Select((a, b) => new TableExampleListOutput()
                {
                    id = a.Id,
                    interactionDate = a.InteractionDate,
                    projectCode = a.ProjectCode,
                    projectName = a.ProjectName,
                    principal = a.Principal,
                    jackStands = a.JackStands,
                    projectType = a.ProjectType,
                    projectPhase = a.ProjectPhase,
                    customerName = a.CustomerName,
                    costAmount = a.CostAmount,
                    tunesAmount = a.TunesAmount,
                    projectedIncome = a.ProjectedIncome,
                    registerDate = a.RegisterDate,
                    registrant = SqlFunc.MergeString(b.RealName, "/", b.Account),
                    description = a.Description,
                    sign = a.Sign,
                    postilJson = a.Sign,
                    postilCount = a.PostilCount.ToString(),
                }).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<TableExampleListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 将实体2的值动态赋值给实体1(名称一样的属性进行赋值)
        /// </summary>
        /// <param name="entity1">实体1</param>
        /// <param name="entity2">实体2</param>
        /// <returns>赋值后的model1</returns>
        private T1 BindModelValue<T1, T2>(T1 entity1, T2 entity2) where T1 : class where T2 : class
        {
            Type t1 = entity1.GetType();
            Type t2 = entity2.GetType();
            PropertyInfo[] property2 = t2.GetProperties();
            //排除字段
            List<string> exclude = new List<string>() { "F_Id", "F_Registrant", "F_RegisterDate", "F_SortCode", "F_Sign", "F_PostilJson", "F_PostilCount" };
            foreach (PropertyInfo p in property2)
            {
                if (exclude.Contains(p.Name)) { continue; }
                t1.GetProperty(p.Name)?.SetValue(entity1, p.GetValue(entity2, null));
            }
            return entity1;
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<string> GetUserName(string userId)
        {
            var entity = await _usersRepository.GetFirstAsync(x => x.Id == userId && x.DeleteMark == null);
            if (entity.IsNullOrEmpty())
                return "";
            return entity.RealName + "/" + entity.Account;
        }
        #endregion
    }
}
