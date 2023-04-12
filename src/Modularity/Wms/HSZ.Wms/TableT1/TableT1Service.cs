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
using HSZ.wms.Entitys.Dto.TableT1;
using HSZ.wms.Interfaces.TableT1;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.TableT1
{
    /// <summary>
    /// 演示12服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "TableT1", Order = 200)]
    [Route("api/wms/[controller]")]
    public class TableT1Service : ITableT1Service, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<TableT1Entity> _tableT1Repository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="TableT1Service"/>类型的新实例
        /// </summary>
        public TableT1Service(ISqlSugarRepository<TableT1Entity> tableT1Repository,
            IUserManager userManager)
        {
            _tableT1Repository = tableT1Repository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取演示12
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _tableT1Repository.GetFirstAsync(p => p.Id == id)).Adapt<TableT1InfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取演示12列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] TableT1ListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _tableT1Repository.AsSugarClient().Queryable<TableT1Entity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_T1), a => a.T1.Contains(input.F_T1))
                .Select((a
)=> new TableT1ListOutput
                {
                    F_Id = a.Id,
                    F_T1 = a.T1,
                    F_T2 = a.T2,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<TableT1ListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建演示12
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] TableT1CrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<TableT1Entity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _tableT1Repository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新演示12
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] TableT1UpInput input)
        {
            var entity = input.Adapt<TableT1Entity>();
            var isOk = await _tableT1Repository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除演示12
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _tableT1Repository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _tableT1Repository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


