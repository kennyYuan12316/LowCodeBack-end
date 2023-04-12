using HSZ.Common.Configuration;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.ViewEngine;
using HSZ.VisualDev.Entitys;
using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using HSZ.VisualDev.Run.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HSZ.VisualDev.Run.Core
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：在线开发运行服务
    /// </summary>
    public class TemplateService : ITemplateService, ITransient
    {
        private readonly IRunService _runService;
       

        /// <summary>
        /// 初始化一个<see cref="TemplateService"/>类型的新实例
        /// </summary>
        public TemplateService(IRunService runService)
        {
            _runService = runService;
            //
        }

        


        
    }
}
