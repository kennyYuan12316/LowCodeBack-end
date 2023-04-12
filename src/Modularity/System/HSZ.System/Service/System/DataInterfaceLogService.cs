using HSZ.Common.Core.Manager;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.System.Entitys.Dto.System.DataInterfaceLog;
using HSZ.System.Entitys.Permission;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UAParser;
using Yitter.IdGenerator;

namespace HSZ.System.Service.System
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据接口日志
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataInterfaceLog", Order = 204)]
    [Route("api/system/[controller]")]
    public class DataInterfaceLogService: IDataInterfaceLogService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DataInterfaceLogEntity> _dataInterfaceLogRepository;
        private readonly IUserManager _userManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataInterfaceLogRepository"></param>
        /// <param name="userManager"></param>
        public DataInterfaceLogService(ISqlSugarRepository<DataInterfaceLogEntity> dataInterfaceLogRepository, IUserManager userManager)
        {
            _dataInterfaceLogRepository = dataInterfaceLogRepository;
            _userManager = userManager;
        }

        #region Get
        [HttpGet("{id}")]
        public async Task<dynamic> GetList(string id,[FromQuery] PageInputBase input)
        {
            var list = await _dataInterfaceLogRepository.AsSugarClient().Queryable<DataInterfaceLogEntity, UserEntity>((a, b) => 
            new JoinQueryInfos(JoinType.Left, b.Id == a.UserId)).Select((a, b) => 
            new DataInterfaceLogListOutput { id = a.Id, invokDevice = a.InvokDevice, invokIp = a.InvokIp, 
                userId = SqlFunc.MergeString(b.RealName, "/", b.Account), invokTime = a.InvokTime, invokType = a.InvokType, 
                invokWasteTime = a.InvokWasteTime,invokeId=a.InvokId}).MergeTable().
                Where(x=>x.invokeId==id).WhereIF(input.keyword.IsNotEmptyOrNull()
                ,m=> m.userId.Contains(input.keyword) || m.invokIp.Contains(input.keyword)).OrderBy(t => t.invokTime)
               .ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<DataInterfaceLogListOutput>.SqlSugarPageResult(list);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="id">接口id</param>
        /// <param name="sw">请求时间</param>
        /// <returns></returns>
        [NonAction]
        public async Task CreateLog(string id, Stopwatch sw)
        {
            var httpContext = App.HttpContext;
            var headers = httpContext.Request.Headers;
            var clientInfo = Parser.GetDefault().Parse(headers["User-Agent"]);
            var log = new DataInterfaceLogEntity()
            {
                Id = YitIdHelper.NextId().ToString(),
                InvokId = id,
                InvokTime = DateTime.Now,
                UserId = _userManager.UserId,
                InvokIp = httpContext.GetLocalIpAddressToIPv4(),
                InvokDevice = String.Format("{0}-{1}", clientInfo.OS.ToString(), clientInfo.UA.ToString()),
                InvokWasteTime = (int)sw.ElapsedMilliseconds,
                InvokType = httpContext.Request.Method
            };
            await _dataInterfaceLogRepository.InsertAsync(log);
        } 
        #endregion
    }
}
