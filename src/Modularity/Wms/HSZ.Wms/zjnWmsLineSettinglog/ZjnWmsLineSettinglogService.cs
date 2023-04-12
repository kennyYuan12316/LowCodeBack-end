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
using HSZ.wms.Entitys.Dto.ZjnWmsLineSettinglog;
using HSZ.wms.Interfaces.ZjnWmsLineSettinglog;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.wms.ZjnWmsLineSettinglog
{
    /// <summary>
    /// 线体物料绑定履历表服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms",Name = "ZjnWmsLineSettinglog", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnWmsLineSettinglogService : IZjnWmsLineSettinglogService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsLineSettinglogEntity> _zjnWmsLineSettinglogRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnWmsLineSettinglogService"/>类型的新实例
        /// </summary>
        public ZjnWmsLineSettinglogService(ISqlSugarRepository<ZjnWmsLineSettinglogEntity> zjnWmsLineSettinglogRepository,
            IUserManager userManager)
        {
            _zjnWmsLineSettinglogRepository = zjnWmsLineSettinglogRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取线体物料绑定履历表
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWmsLineSettinglogRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnWmsLineSettinglogInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取线体物料绑定履历表列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsLineSettinglogListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_CreateTime" : input.sidx;
            var data = await _zjnWmsLineSettinglogRepository.AsSugarClient().Queryable<ZjnWmsLineSettinglogEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_LineNo), a => a.LineNo.Contains(input.F_LineNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_LineName), a => a.LineName.Contains(input.F_LineName))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsType), a => a.GoodsType.Contains(input.F_GoodsType))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsCode), a => a.GoodsCode.Contains(input.F_GoodsCode))
                .WhereIF(input.Status.HasValue, a => a.Status == input.Status)
                .Select((a
)=> new ZjnWmsLineSettinglogListOutput
                {
                    F_Id = a.Id,
                    F_LineNo = a.LineNo,
                    F_LineName = a.LineName,
                    F_TrayNo = a.TrayNo,
                    F_GoodsType = a.GoodsType,
                    F_GoodsCode = a.GoodsCode,
                    F_LineStart = a.LineStart,
                    F_LineEnd = a.LineEnd,
                    F_LineLayer = a.LineLayer,
                    F_LineMaxWork = a.LineMaxWork,
                    F_LineNowWork = a.LineNowWork,
                    F_Description = a.Description,
                    F_Expand = a.Expand,
                    F_Status = a.Status,
                    F_OutTime = a.OutTime,
                    F_CreateTime = a.CreateTime,
                }).OrderBy(sidx+" "+input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnWmsLineSettinglogListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
		/// 获取线体物料绑定履历表无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnWmsLineSettinglogListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_CreateTime" : input.sidx;
            var data = await _zjnWmsLineSettinglogRepository.AsSugarClient().Queryable<ZjnWmsLineSettinglogEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_LineNo), a => a.LineNo.Contains(input.F_LineNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_LineName), a => a.LineName.Contains(input.F_LineName))
                .WhereIF(!string.IsNullOrEmpty(input.F_TrayNo), a => a.TrayNo.Contains(input.F_TrayNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsType), a => a.GoodsType.Contains(input.F_GoodsType))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsCode), a => a.GoodsCode.Contains(input.F_GoodsCode))
                .Select((a
)=> new ZjnWmsLineSettinglogListOutput
                {
                    F_Id = a.Id,
                    F_LineNo = a.LineNo,
                    F_LineName = a.LineName,
                    F_TrayNo = a.TrayNo,
                    F_GoodsType = a.GoodsType,
                    F_GoodsCode = a.GoodsCode,
                    F_LineStart = a.LineStart,
                    F_LineEnd = a.LineEnd,
                    F_LineLayer = a.LineLayer,
                    F_LineMaxWork = a.LineMaxWork,
                    F_LineNowWork = a.LineNowWork,
                    F_Description = a.Description,
                    F_Expand = a.Expand,
                    F_Status = a.Status,
                    F_OutTime = a.OutTime,
                    F_CreateTime = a.CreateTime,
                }).OrderBy(sidx+" "+input.sort).ToListAsync();
                return data;
        }

        /// <summary>
		/// 导出线体物料绑定履历表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnWmsLineSettinglogListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnWmsLineSettinglogListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnWmsLineSettinglogListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"线体编号\",\"field\":\"lineNo\"},{\"value\":\"线体名称\",\"field\":\"lineName\"},{\"value\":\"物料类型\",\"field\":\"goodsType\"},{\"value\":\"物料编号\",\"field\":\"goodsCode\"},{\"value\":\"托盘编号\",\"field\":\"trayNo\"},{\"value\":\"线体缓存起点\",\"field\":\"lineStart\"},{\"value\":\"线体缓存终点\",\"field\":\"lineEnd\"},{\"value\":\"线体层\",\"field\":\"lineLayer\"},{\"value\":\"线体最大任务数\",\"field\":\"lineMaxWork\"},{\"value\":\"当前任务数量\",\"field\":\"lineNowWork\"},{\"value\":\"线体描述\",\"field\":\"description\"},{\"value\":\"预留字段\",\"field\":\"expand\"},{\"value\":\"状态\",\"field\":\"status\"},{\"value\":\"出库时间\",\"field\":\"outTime\"},{\"value\":\"创建时间\",\"field\":\"createTime\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "线体物料绑定履历表.xls";
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
            ExcelExportHelper<ZjnWmsLineSettinglogListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }
    }
}


