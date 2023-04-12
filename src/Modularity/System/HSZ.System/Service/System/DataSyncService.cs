using HSZ.ChangeDataBase;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.System.Entitys.Dto.System.DataSync;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.System;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HSZ.System.Core.Service.DataSync
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据同步
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataSync", Order = 209)]
    [Route("api/system/[controller]")]
    public class DataSyncService : IDataSyncService, IDynamicApiController, ITransient
    {
        private readonly IChangeDataBase _changeDataBase;
        private readonly IDbLinkService _dbLinkService;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 
        /// </summary>
        public DataSyncService(IChangeDataBase changeDataBase,
            IDbLinkService dbLinkService,
            IUserManager userManager)
        {
            _changeDataBase = changeDataBase;
            _dbLinkService = dbLinkService;
            _userManager = userManager;
        }

        /// <summary>
        /// 同步判断
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<dynamic> Estimate([FromBody] DbSyncActionsExecuteInput input)
        {
            var linkFrom = await _dbLinkService.GetInfo(input.dbConnectionFrom);
            var linkTo = await _dbLinkService.GetInfo(input.dbConnectionTo);

            if (linkTo == null)
            {
                linkTo = new DbLinkEntity
                {
                    Id = _userManager.TenantId,
                    ServiceName = _userManager.TenantDbName,
                    DbType = App.Configuration["ConnectionStrings:DBType"],
                    Host = App.Configuration["ConnectionStrings:Host"],
                    Port = App.Configuration["ConnectionStrings:Port"].ToInt(),
                    UserName = App.Configuration["ConnectionStrings:UserName"],
                    Password = App.Configuration["ConnectionStrings:Password"]
                };
            }

            if (!IsNullDataByTable(linkFrom, input.dbTable))
            {
                //初始表有数据
                return 1;
            }
            else if (!_changeDataBase.IsAnyTable(linkTo, input.dbTable))
            {
                //目的表不存在
                return 2;
            }
            else if (IsNullDataByTable(linkTo, input.dbTable))
            {
                //目的表有数据
                return 3;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 执行同步
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("Actions/Execute")]
        public async Task Execute([FromBody] DbSyncActionsExecuteInput input)
        {
            var linkFrom = await _dbLinkService.GetInfo(input.dbConnectionFrom);
            var linkTo = await _dbLinkService.GetInfo(input.dbConnectionTo);

            if (linkTo == null)
            {
                linkTo = new DbLinkEntity
                {
                    Id = _userManager.TenantId,
                    ServiceName = _userManager.TenantDbName,
                    DbType = App.Configuration["ConnectionStrings:DBType"],
                    Host = App.Configuration["ConnectionStrings:Host"],
                    Port = App.Configuration["ConnectionStrings:Port"].ToInt(),
                    UserName = App.Configuration["ConnectionStrings:UserName"],
                    Password = App.Configuration["ConnectionStrings:Password"]
                };
            }
            _changeDataBase.SyncTable(linkFrom, linkTo, input.dbTable, input.type);
            var isOk = ImportTableData(linkFrom, linkTo, input.dbTable);
            if (!isOk)
                throw HSZException.Oh(ErrorCode.COM1006);
        }

        #region PrivateMethod

        /// <summary>
        /// 判断表中是否有数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private bool IsNullDataByTable(DbLinkEntity entity, string table)
        {
            var data = _changeDataBase.GetData(entity, table);
            if (data.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 批量写入
        /// </summary>
        /// <param name="linkFrom">数据库连接 From</param>
        /// <param name="linkTo">数据库连接To</param>
        /// <param name="table"></param>
        private bool ImportTableData(DbLinkEntity linkFrom, DbLinkEntity linkTo, string table)
        {
            try
            {
                //取同步数据
                var syncData = _changeDataBase.GetData(linkFrom, table);
                //插入同步数据
                var isOk = _changeDataBase.SyncData(linkTo, syncData, table);
                return isOk;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
