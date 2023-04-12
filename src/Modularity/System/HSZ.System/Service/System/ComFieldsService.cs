using HSZ.Common.Enum;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.System.ComFields;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.System.Service.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：常用字段
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "CommonFields", Order = 201)]
    [Route("api/system/[controller]")]
    public class ComFieldsService : IComFieldsService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ComFieldsEntity> _comFieldsRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comFieldsRepository"></param>
        public ComFieldsService(ISqlSugarRepository<ComFieldsEntity> comFieldsRepository)
        {
            _comFieldsRepository = comFieldsRepository;
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
            var output = data.Adapt<ComFieldsInfoOutput>();
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
            var output = data.Adapt<List<ComFieldsListOutput>>();
            return new { list = output };
        }
        #endregion

        #region Post
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] ComFieldsCrInput input)
        {
            if (await _comFieldsRepository.IsAnyAsync(x => x.Field.ToLower() == input.field.ToLower() && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ComFieldsEntity>();
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
            var entity = await _comFieldsRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity == null)
                throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(entity);
            if (isOk < 0)
                throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update_Api(string id, [FromBody] ComFieldsUpInput input)
        {
            if (await _comFieldsRepository.IsAnyAsync(x =>x.Id!=id&&x.Field.ToLower() == input.field.ToLower() && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ComFieldsEntity>();
            var isOk = await Update(entity);
            if (isOk < 0)
                throw HSZException.Oh(ErrorCode.COM1001);
        }
        #endregion

        #region PulicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ComFieldsEntity>> GetList()
        {
            return await _comFieldsRepository.AsQueryable().Where(x => x.DeleteMark == null).OrderBy(x => x.SortCode).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<ComFieldsEntity> GetInfo(string id)
        {
            return await _comFieldsRepository.GetFirstAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(ComFieldsEntity entity)
        {
            return await _comFieldsRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(ComFieldsEntity entity)
        {
            return await _comFieldsRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(ComFieldsEntity entity)
        {
            return await _comFieldsRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }
        #endregion
    }
}
