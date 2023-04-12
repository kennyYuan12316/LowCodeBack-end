using HSZ.Common.Core.Manager;
using HSZ.ClayObject;
using HSZ.Common.Configuration;
using HSZ.Common.Model.NPOI;
using HSZ.DataEncryption;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.wms.Entitys.Dto.HltHrTransfer;
using HSZ.wms.Interfaces.HltHrTransfer;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.HltHrTransfer
{
    /// <summary>
    /// 实时调用信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "HltHrTransfer", Order = 200)]
    [Route("api/wms/[controller]")]
    public class HltHrTransferService : IHltHrTransferService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<HltHrTransferEntity> _hltHrTransferRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="HltHrTransferService"/>类型的新实例
        /// </summary>
        public HltHrTransferService(ISqlSugarRepository<HltHrTransferEntity> hltHrTransferRepository,
            IUserManager userManager)
        {
            _hltHrTransferRepository = hltHrTransferRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取实时调用信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _hltHrTransferRepository.GetFirstAsync(p => p.Id == id)).Adapt<HltHrTransferInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取实时调用信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] HltHrTransferListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrTransferRepository.AsSugarClient().Queryable<HltHrTransferEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.l2), a => a.L2.Contains(input.l2))
                .WhereIF(!string.IsNullOrEmpty(input.l3), a => a.L3.Contains(input.l3))
                .WhereIF(!string.IsNullOrEmpty(input.l5), a => a.L5.Contains(input.l5))
                .Select((a
)=> new HltHrTransferListOutput
                {
                    l1 = a.L1,
                    l2 = a.L2,
                    l3 = a.L3,
                    l4 = a.L4,
                    l5 = a.L5,
                    l6 = a.L6,
                    l7 = a.L7,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<HltHrTransferListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建实时调用信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] HltHrTransferCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<HltHrTransferEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            
            var isOk = await _hltHrTransferRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取实时调用信息无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] HltHrTransferListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _hltHrTransferRepository.AsSugarClient().Queryable<HltHrTransferEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.l2), a => a.L2.Contains(input.l2))
                .WhereIF(!string.IsNullOrEmpty(input.l3), a => a.L3.Contains(input.l3))
                .WhereIF(!string.IsNullOrEmpty(input.l5), a => a.L5.Contains(input.l5))
                .Select((a
)=> new HltHrTransferListOutput
                {
                    l1 = a.L1,
                    l2 = a.L2,
                    l3 = a.L3,
                    l4 = a.L4,
                    l5 = a.L5,
                    l6 = a.L6,
                    l7 = a.L7,
                    F_Id = a.Id,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出实时调用信息
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] HltHrTransferListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<HltHrTransferListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<HltHrTransferListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"实时信息标识\",\"field\":\"l1\"},{\"value\":\"任务标识\",\"field\":\"l2\"},{\"value\":\"来源系统名称\",\"field\":\"l3\"},{\"value\":\"信息类型编码\",\"field\":\"l4\"},{\"value\":\"信息类型名称\",\"field\":\"l5\"},{\"value\":\"信息总数量\",\"field\":\"l6\"},{\"value\":\"实时接收时间\",\"field\":\"l7\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "实时调用信息.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            List<string> selectKeyList = input.selectKey.Split(',').ToList();
            foreach (var item in selectKeyList)
            {
                var isExist = paramList.Find(p => p.field == item);
                if (isExist != null)
                {
                    excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = isExist.field, ExcelColumn = isExist.value });
                }
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<HltHrTransferListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除实时调用信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _hltHrTransferRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除实时调用信息
                    await _hltHrTransferRepository.AsDeleteable().In(d => d.Id,ids).ExecuteCommandAsync();
                    //关闭事务
                    _db.CommitTran();
                }
                catch (Exception)
                {
                    //回滚事务
                    _db.RollbackTran();
                    throw HSZException.Oh(ErrorCode.COM1002);
                }
            }
        }

        /// <summary>
        /// 更新实时调用信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] HltHrTransferUpInput input)
        {
            var entity = input.Adapt<HltHrTransferEntity>();
            var isOk = await _hltHrTransferRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除实时调用信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _hltHrTransferRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            var isOk = await _hltHrTransferRepository.AsDeleteable().Where(d => d.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1002);
        }
    }
}


