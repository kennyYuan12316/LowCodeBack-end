using HSZ.Apps.Entitys.Dto;
using HSZ.ChangeDataBase;
using HSZ.Common.Configuration;
using HSZ.Common.Core.Manager;
using HSZ.Common.Extension;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Permission;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System.Threading.Tasks;

namespace HSZ.Apps
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：App版本信息
    /// </summary>
    [ApiDescriptionSettings(Tag = "App", Name = "Version", Order = 806)]
    [Route("api/App/[controller]")]
    public class AppVersion : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<SysConfigEntity> _sysConfigRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope Db;

        /// <summary>
        ///
        /// </summary>
        /// <param name="sysConfigRepository"></param>
        /// <param name="userManager"></param>
        public AppVersion(ISqlSugarRepository<SysConfigEntity> sysConfigRepository,
            IChangeDataBase changeDataBase,
            IUserManager userManager)
        {
            _sysConfigRepository = sysConfigRepository;
            _userManager = userManager;
            Db = DbScoped.SugarScope;
        }

        #region Get
        /// <summary>
        /// 版本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetInfo()
        {
            var data = new SysConfigEntity();

            if (KeyVariable.MultiTenancy)
            {
                Db.ChangeDatabase(App.Configuration["ConnectionStrings:ConfigId"]);

                data = await Db.Queryable<SysConfigEntity>().Where(x => x.Category.Equals("SysConfig") && x.Key== "sysVersion").FirstAsync();
            }
            else
            {
                data = await _sysConfigRepository.AsQueryable().Where(x => x.Category.Equals("SysConfig") && x.Key == "sysVersion").FirstAsync();
            }

            return new { sysVersion = data.Value };
        }

        #endregion

    }
}
