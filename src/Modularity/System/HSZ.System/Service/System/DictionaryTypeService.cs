using HSZ.Common.Enum;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.System.DictionaryType;
using HSZ.System.Entitys.Entity.System;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.Common.Core.Service.DictionaryType
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：字典分类
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DictionaryType", Order = 202)]
    [Route("api/system/[controller]")]
    public class DictionaryTypeService : IDictionaryTypeService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DictionaryTypeEntity> _dictionaryTypeRepository;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionaryTypeRepository"></param>
        /// <param name="dictionaryDataRepository"></param>
        public DictionaryTypeService(ISqlSugarRepository<DictionaryTypeEntity> dictionaryTypeRepository, ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository)
        {
            _dictionaryTypeRepository = dictionaryTypeRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
        }

        #region Get

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">请求参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await GetInfo(id);
            var output = data.Adapt<DictionaryTypeInfoOutput>();
            return output;
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList_Api()
        {
            var data = await GetList();
            var output = data.Adapt<List<DictionaryTypeListOutput>>();
            return new { list = output.ToTree("-1") };
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector/{id}")]
        public async Task<dynamic> GetSelector(string id)
        {
            var data = await GetList();
            if (!id.Equals("0"))
            {
                data.RemoveAll(x => x.Id == id);
            }
            var output = data.Adapt<List<DictionaryTypeSelectorOutput>>();
            return new { list = output.ToTree("-1") };
        }
        #endregion

        #region Post
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] DictionaryTypeCrInput input)
        {
            if (await _dictionaryTypeRepository.IsAnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null) || await _dictionaryTypeRepository.IsAnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D3001);
            var entity = input.Adapt<DictionaryTypeEntity>();
            var isOk = await Create(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1000);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">请求参数</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete_Api(string id)
        {
            var entity = await _dictionaryTypeRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.D3000);
            if (await AllowDelete(id))
            {
                var isOk = await Delete(entity);
                if (isOk < 1)
                    throw HSZException.Oh(ErrorCode.COM1002);
            }
            else
            {
                throw HSZException.Oh(ErrorCode.D3002);
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update_Api(string id, [FromBody] DictionaryTypeUpInput input)
        {
            if (await _dictionaryTypeRepository.IsAnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null) || await _dictionaryTypeRepository.IsAnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.D3001);
            var entity = input.Adapt<DictionaryTypeEntity>();
            var isOk = await Update(entity);
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1001);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">请求参数</param>
        /// <returns></returns>
        [NonAction]
        public async Task<DictionaryTypeEntity> GetInfo(string id)
        {
            var data = await _dictionaryTypeRepository.GetFirstAsync(x => (x.Id == id || x.EnCode == id) && x.DeleteMark == null);
            return data;
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DictionaryTypeEntity>> GetList()
        {
            return await _dictionaryTypeRepository.AsQueryable().Where(x => x.DeleteMark == null).OrderBy(x => x.SortCode).OrderBy(x => x.CreatorTime,OrderByType.Desc).ToListAsync();
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">请求参数</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(DictionaryTypeEntity entity)
        {
            return await _dictionaryTypeRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(DictionaryTypeEntity entity)
        {
            return await _dictionaryTypeRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(DictionaryTypeEntity entity)
        {
            return await _dictionaryTypeRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 递归获取所有分类
        /// </summary>
        /// <param name="id"></param>
        /// <param name="typeList"></param>
        /// <returns></returns>
        [NonAction]
        public async Task GetListAllById(string id, List<DictionaryTypeEntity> typeList)
        {
            var entity = await GetInfo(id);
            if (entity != null)
            {
                typeList.Add(entity);
                if (await _dictionaryTypeRepository.IsAnyAsync(x => x.ParentId == entity.Id && x.DeleteMark == null))
                {
                    var list = await _dictionaryTypeRepository.AsQueryable().Where(x => x.ParentId == entity.Id && x.DeleteMark == null).ToListAsync();
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            await GetListAllById(item.Id, typeList);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 重复判断(分类)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public bool IsExistType(DictionaryTypeEntity entity)
        {
            return _dictionaryTypeRepository.IsAny(x => (x.Id == entity.Id || x.EnCode == entity.EnCode || x.FullName == entity.FullName) && x.DeleteMark == null);
        }

        /// <summary>
        /// 重复判断(字典)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public bool IsExistData(DictionaryDataEntity entity)
        {
            //var typeFlag = _dictionaryTypeRepository.Any(x => x.Id == entity.DictionaryTypeId && x.DeleteMark == null);
            var dataFalg= _dictionaryDataRepository.IsAny(x => (  x.EnCode == entity.EnCode || x.FullName == entity.FullName)&& x.DictionaryTypeId == entity.DictionaryTypeId && x.DeleteMark == null);
            return  dataFalg;
        }

        /// <summary>
        /// 是否存在上级
        /// </summary>
        /// <param name="Entities"></param>
        /// <returns></returns>
        public bool IsExistParent(List<DictionaryTypeEntity> Entities)
        {
            foreach (var item in Entities)
            {
                if (_dictionaryTypeRepository.IsAny(x=>x.Id==item.ParentId&&x.DeleteMark==null))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 是否可以删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<bool> AllowDelete(string id)
        {
            var flag = true;
            if (await _dictionaryTypeRepository.IsAnyAsync(o => o.ParentId.Equals(id) && o.DeleteMark == null))
                return false;
            if (await _dictionaryDataRepository.IsAnyAsync(p => p.DictionaryTypeId.Equals(id) && p.DeleteMark == null))
                return false;
            return flag;
        }
        #endregion
    }
}
