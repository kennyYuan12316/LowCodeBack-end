using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using ZJN.Calb.Client;
using ZJN.Calb.Client.DTO;

namespace ZJN.Calb
{
    /// <summary>
    /// LES服务
    /// </summary>
    [AllowAnonymous]
    [NonUnify]
    [ApiDescriptionSettings(Tag = "Callb", Name = "CallbByLES", Order = 200)]
    [Route("api/Callb/[controller]")]
    public class CallbByLESService : LESServerClient,IDynamicApiController, ITransient
    {

        /// <summary>
        /// 初始化一个<see cref="CallbByMESService"/>类型的新实例
        /// </summary>
        public CallbByLESService(IOptionsSnapshot<WebSerivcesConfig> config, IOptionsSnapshot<LESLoginConfig> login) :base(config, login)
        {
            
        }
    }
}


