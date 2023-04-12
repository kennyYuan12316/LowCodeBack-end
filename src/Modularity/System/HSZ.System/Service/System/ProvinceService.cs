using HSZ.Common.Enum;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.System.Province;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.System.Core.Service.Province
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：行政区划
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "Area", Order = 206)]
    [Route("api/system/[controller]")]
    public class ProvinceService : IProvinceService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ProvinceEntity> _provinceRepository;

        /// <summary>
        /// 初始化一个<see cref="ProvinceService"/>类型的新实例
        /// </summary>
        /// <param name="provinceRepository"></param>
        public ProvinceService(ISqlSugarRepository<ProvinceEntity> provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        #region GET

        /// <summary>
        /// 获取行政区划列表
        /// </summary>
        /// <param name="nodeid">节点Id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("{nodeId}")]
        public async Task<dynamic> GetList([FromQuery] KeywordInput input, string nodeid)
        {
            var data = await _provinceRepository.AsQueryable().Where(m => m.ParentId == nodeid && m.DeleteMark == null)
                .WhereIF(!string.IsNullOrEmpty(input.keyword), t => t.EnCode.Contains(input.keyword) || t.FullName.Contains(input.keyword))
                .OrderBy(o => o.SortCode).OrderBy(t => t.CreatorTime, OrderByType.Desc).OrderByIF(!string.IsNullOrEmpty(input.keyword), t => t.LastModifyTime, OrderByType.Desc).ToListAsync();
            var output = data.Adapt<List<ProvinceListOutput>>();
            foreach (var item in output)
            {
                var flag = await GetExistsLeaf(item.id);
                item.isLeaf = flag;
                item.hasChildren = !flag;
            }
            return new { list = output };
        }

        /// <summary>
        /// 获取行政区划下拉框数据(异步)
        /// </summary>
        /// <param name="id">当前Id</param>
        /// <returns></returns>
        [HttpGet("{id}/Selector/{areaId}")]
        public async Task<dynamic> GetSelector(string id,string areaId)
        {
            var data = (await GetList(id)).FindAll(x=>x.EnabledMark==1);
            if (!areaId.Equals("0"))
            {
                data.RemoveAll(x => x.Id == areaId);
            }
            var output = data.Adapt<List<ProvinceSelectorOutput>>();
            foreach (var item in output)
            {
                item.isLeaf = await GetExistsLeaf(item.id);
            }
            return new { list = output };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}/Info")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = (await _provinceRepository.GetFirstAsync(m => m.Id == id && m.DeleteMark == null)).Adapt<ProvinceInfoOutput>();
            return data;
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
            var entity = await _provinceRepository.GetFirstAsync(m => m.Id == id && m.DeleteMark == null);
            if (entity == null || (await _provinceRepository.AsQueryable().Where(m => m.ParentId == id && m.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync()).Count > 0)
                throw HSZException.Oh(ErrorCode.D1007);
            var isOk = await _provinceRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (isOk < 1)
                throw HSZException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ProvinceCrInput input)
        {
            if (await _provinceRepository.IsAnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ProvinceEntity>();
            var isOk = await _provinceRepository.AsInsertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (isOk <= 0) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ProvinceUpInput input)
        {
            if (await _provinceRepository.IsAnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null))
                throw HSZException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ProvinceEntity>();
            var isOk = await _provinceRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 0)
                throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 更新行政区划状态
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState(string id)
        {
            var entity = await _provinceRepository.GetFirstAsync(m => m.Id == id && m.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await _provinceRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 0)
                throw HSZException.Oh(ErrorCode.COM1003);
        }

        /// <summary>
        /// 获取省市区 根据 二维数组 id 
        /// </summary>
        /// <param name="input">省市区 二维 数组</param>
        /// <returns></returns>
        [HttpPost("GetAreaByIds")]
        public async Task<dynamic> GetAreaByIds([FromBody] ProvinceGetDataInput input)
        {
            var allIds = new List<string>();
            var res = new List<List<string>>();

            foreach (var item in input.idsList)
            {
                foreach (var it in item)
                {
                    allIds.Add(it);
                }
            }

            var data = await _provinceRepository.AsQueryable().Where(m => allIds.Contains(m.Id) && m.DeleteMark == null).
                Select(m => new ProvinceEntity() { Id = m.Id, FullName = m.FullName }).ToListAsync();

            foreach (var item in input.idsList)
            {
                var itemValueList = data.FindAll(x => item.Contains(x.Id));
                var valueList= new List<string>();
                itemValueList.ForEach(it =>
                {
                    valueList.Add(it.FullName);
                });

                res.Add(valueList);
            }

            return res;
        }

        #endregion

        #region PulicMethod

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ProvinceEntity>> GetList()
        {
            var data = await _provinceRepository.AsQueryable().Select(x => new ProvinceEntity { Id = x.Id, ParentId = x.ParentId, Type = x.Type, FullName = x.FullName }).ToListAsync();
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ProvinceEntity>> GetList(string id)
        {
            return await _provinceRepository.AsQueryable().Where(m => m.ParentId == id && m.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 是否存在子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<bool> GetExistsLeaf(string id)
        {
            return (await _provinceRepository.AsQueryable().Where(m => m.ParentId == id && m.DeleteMark == null).CountAsync()) > 0 ? false : true;
        }

        

        #endregion
    }
}