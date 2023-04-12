using HSZ.Apps.Entitys.Dto;
using HSZ.Apps.Interfaces;
using HSZ.Common.Util;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSZ.Apps
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：App菜单
    /// </summary>
    [ApiDescriptionSettings(Tag = "App", Name = "Menu", Order = 800)]
    [Route("api/App/[controller]")]
    public class AppMenuService : IDynamicApiController, ITransient
    {
        private readonly IAppDataService _appDataService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appDataService"></param>
        public AppMenuService(IAppDataService appDataService)
        {
            _appDataService = appDataService;
        }

        #region Get
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList()
        {
            var list = (await _appDataService.GetAppMenuList()).Adapt<List<AppMenuListOutput>>();
            var output = list.ToTree("-1");
            return new { list = output };
        }
        #endregion
    }
}
