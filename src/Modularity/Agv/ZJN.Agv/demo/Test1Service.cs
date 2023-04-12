using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using ZJN.Entitys.agv;
//using ZJN.agv.Entitys.Dto.Test1;
using ZJN.agv.Interfaces.Test1;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
//using ZJN.Entitys.agv;
//using ZJN.agv.Interfaces.Test1;

namespace ZJN.agv.Test1
{
    /// <summary>
    /// demo服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "Agv",Name = "Test2", Order = 203)]
    [Route("api/agv/[controller]")]
    public class test1Service : ITest1Service, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<Test1Entity> _test1Repository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="test1Service"/>类型的新实例
        /// </summary>
        public test1Service(ISqlSugarRepository<Test1Entity> test1Repository,
            IUserManager userManager)
        {
            _test1Repository = test1Repository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取demo信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _test1Repository.GetFirstAsync(p => p.Id == id)).Adapt<Test1Entity>();
            return output;
        }
    }
}


