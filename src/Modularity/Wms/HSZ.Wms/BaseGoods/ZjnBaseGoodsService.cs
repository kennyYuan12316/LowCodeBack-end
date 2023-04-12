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
using HSZ.wms.Entitys.Dto.ZjnBaseGoods;
using HSZ.wms.Interfaces.ZjnBaseGoods;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Yitter.IdGenerator;
using HSZ.Wms.Entitys.Dto.BaseGoods;
using HSZ.System.Entitys.System;
using HSZ.System.Entitys.Entity.System;

namespace HSZ.wms.ZjnBaseGoods
{
    /// <summary>
    /// 货物信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnBaseGoods", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnBaseGoodsService : IZjnBaseGoodsService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnBaseGoodsEntity> _zjnBaseGoodsRepository;
        private readonly ISqlSugarRepository<ZjnBaseGoodsDetailsEntity> _zjnBaseGoodsDetailsRepository;
        private readonly ISqlSugarRepository<DictionaryTypeEntity> _dictionaryTypeRepository;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnBaseGoodsService"/>类型的新实例
        /// </summary>
        public ZjnBaseGoodsService(ISqlSugarRepository<ZjnBaseGoodsEntity> zjnBaseGoodsRepository,
            ISqlSugarRepository<ZjnBaseGoodsDetailsEntity> zjnBaseGoodsDetailsRepository,
            IUserManager userManager)
        {
            _zjnBaseGoodsRepository = zjnBaseGoodsRepository;
            _zjnBaseGoodsDetailsRepository = zjnBaseGoodsDetailsRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取货物信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnBaseGoodsRepository.AsSugarClient().
            Queryable<ZjnBaseGoodsEntity, ZjnBaseGoodsDetailsEntity>((a, a1) => new JoinQueryInfos(JoinType.Left, a1.ParentId == a.Id)).Where(a => a.Id == id)
            .Select((a, a1) => new ZjnBaseGoodsInfoOutput
            {
                id = a.Id,
                goodsId = a.GoodsId,
                goodsCode = a.GoodsCode,
                goodsName = a.GoodsName,
                unit = a.Unit,
                trayNo = a.TrayNo,
                goodstype = a.GoodsType,
                enabledMark = a.EnabledMark,
                parentId = a1.ParentId,
                hsz_zjn_base_goods_details_hsz_enabledMark = a1.EnabledMark,
                hsz_zjn_base_goods_details_hsz_batch = a1.Batch,
                hsz_zjn_base_goods_details_hsz_specifications = a1.Specifications,
                hsz_zjn_base_goods_details_hsz_goodsCreateData = a1.GoodsCreateData,
                hsz_zjn_base_goods_details_hsz_goodsState = a1.GoodsState,
                hsz_zjn_base_goods_details_hsz_goodsLocationNo = a1.GoodsLocationNo,
                hsz_zjn_base_goods_details_hsz_customerId = a1.CustomerId,
                hsz_zjn_base_goods_details_hsz_palledNo = a1.PalledNo,
                hsz_zjn_base_goods_details_hsz_vendorId = a1.VendorId,
                hsz_zjn_base_goods_details_hsz_checkDate = a1.CheckDate,
                hsz_zjn_base_goods_details_hsz_checkType = a1.CheckType,
                hsz_zjn_base_goods_details_hsz_goodsGrade = a1.GoodsGrade,
                hsz_zjn_base_goods_details_hsz_remarks = a1.Remarks,
            }).Mapper((output) =>{}).ToListAsync()).FirstOrDefault();
            return output;
        }

        /// <summary>
		/// 获取货物信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnBaseGoodsListQueryInput input)
        {
            Regex r = new Regex("_");
            var sidx = input.sidx == null ? "a.F_Id" : r.Replace(input.sidx, ".", 1);
            List<string> queryAuxiliaryGoodsCreateData1 = input.a_CreateTime != null ? input.a_CreateTime.Split(',').ToObject<List<string>>() : null;
            DateTime? startAuxiliaryGoodsCreateData = queryAuxiliaryGoodsCreateData1 != null ? Ext.GetDateTime(queryAuxiliaryGoodsCreateData1.First()) : null;
            DateTime? endAuxiliaryGoodsCreateData = queryAuxiliaryGoodsCreateData1 != null ? Ext.GetDateTime(queryAuxiliaryGoodsCreateData1.Last()) : null;
            var data = await _zjnBaseGoodsRepository.AsSugarClient()
            .Queryable<ZjnBaseGoodsEntity, ZjnBaseGoodsDetailsEntity,ZjnPlaneLocationEntity>((a, a1,a2) => new JoinQueryInfos(JoinType.Left, a1.ParentId == a.Id, JoinType.Left,a2.Id == a1.GoodsLocationNo))
            .WhereIF(!string.IsNullOrEmpty(input.a_F_GoodsCode), a => a.GoodsCode.Contains(input.a_F_GoodsCode))
            .WhereIF(!string.IsNullOrEmpty(input.a1_F_batch), (a, a1) => a1.Batch.Contains(input.a1_F_batch))
            .WhereIF(queryAuxiliaryGoodsCreateData1 != null, a=> a.CreateTime >= new DateTime(startAuxiliaryGoodsCreateData.ToDate().Year, startAuxiliaryGoodsCreateData.ToDate().Month, startAuxiliaryGoodsCreateData.ToDate().Day, 0, 0, 0))
            .WhereIF(queryAuxiliaryGoodsCreateData1 != null, a => a.CreateTime <= new DateTime(endAuxiliaryGoodsCreateData.ToDate().Year, endAuxiliaryGoodsCreateData.ToDate().Month, endAuxiliaryGoodsCreateData.ToDate().Day, 23, 59, 59))
            
            .Select((a, a1,a2) => new ZjnBaseGoodsListOutput
            {
                a_F_Id = a.Id,
                a_F_GoodsId = a.GoodsId,
                a_F_GoodsCode = a.GoodsCode,
                a_goodsName = a.GoodsName,
                a_F_Unit = a.Unit,
                a_F_TrayNo = a.TrayNo,
                a_F_EnabledMark = a.EnabledMark,
                a_CreateTime = a.CreateTime,
                a_CreateUser = a.CreateUser,
                a_GoodsType = a.GoodsType,
                a1_F_EnabledMark = a1.EnabledMark,
                a1_F_batch = a1.Batch,
                a1_F_specifications = a1.Specifications,
                a1_F_GoodsCreateData = a1.GoodsCreateData,
                a1_F_GoodsState = a1.GoodsState,
                a1_F_GoodsLocationNo = a1.GoodsLocationNo,
                a1_F_CustomerId = a1.CustomerId,
                a1_F_PalledNo = a1.PalledNo,
                a1_F_VendorId = a1.VendorId,
                a1_F_CheckDate = a1.CheckDate,
                a1_F_CheckType = a1.CheckType,
                a1_F_GoodsGrade = a1.GoodsGrade,
                a1_F_Remarks = a1.Remarks,
                a1_F_LastModifyTime = a1.LastModifyTime,
                a1_F_LastModifyUserId = a1.LastModifyUserId,
                a2_GoodsLocationNo = a2.LocationNo
            }).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<ZjnBaseGoodsListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建货物信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnBaseGoodsCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseGoodsEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = userInfo.userId;
            entity.CreateTime = DateTime.Now;
            var entity1 = input.Adapt<ZjnBaseGoodsDetailsEntity>();
            entity1.Id = YitIdHelper.NextId().ToString();
            entity1.CreatorUserId = userInfo.userId;
            entity1.CreatorTime = DateTime.Now;
            entity1.LastModifyUserId = userInfo.userId;
            entity1.LastModifyTime = DateTime.Now;

            try
            {
                _db.BeginTran();
                var newEntity = await _zjnBaseGoodsRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteReturnEntityAsync();

                entity1.ParentId = newEntity.Id;
                await _zjnBaseGoodsDetailsRepository.AsInsertable(entity1).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();

                _db.CommitTran();
            }
            catch (Exception e)
            {

                _db.RollbackTran();
                throw HSZException.Oh(e.Message + ErrorCode.COM1000);
            }
        }

        /// <summary>
		/// 获取货物信息无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnBaseGoodsListQueryInput input)
        {
            Regex r = new Regex("_");
            var sidx = input.sidx == null ? "a.F_Id" : r.Replace(input.sidx, ".", 1);
            List<string> queryAuxiliaryGoodsCreateData1 = input.a1_F_GoodsCreateData != null ? input.a1_F_GoodsCreateData.Split(',').ToObject<List<string>>() : null;
            DateTime? startAuxiliaryGoodsCreateData = queryAuxiliaryGoodsCreateData1 != null ? Ext.GetDateTime(queryAuxiliaryGoodsCreateData1.First()) : null;
            DateTime? endAuxiliaryGoodsCreateData = queryAuxiliaryGoodsCreateData1 != null ? Ext.GetDateTime(queryAuxiliaryGoodsCreateData1.Last()) : null;
            var data = await _zjnBaseGoodsRepository.AsSugarClient().Queryable<ZjnBaseGoodsEntity
, ZjnBaseGoodsDetailsEntity
>((a
, a1
) => new JoinQueryInfos(
JoinType.Left, a1.ParentId == a.Id
))
                .WhereIF(!string.IsNullOrEmpty(input.a_F_GoodsId), a => a.GoodsId.Contains(input.a_F_GoodsId))
                .WhereIF(!string.IsNullOrEmpty(input.a_F_GoodsCode), a => a.GoodsCode.Contains(input.a_F_GoodsCode))
                .WhereIF(!string.IsNullOrEmpty(input.a1_F_batch), (a
, a1
) => a1.Batch.Contains(input.a1_F_batch))
                .WhereIF(queryAuxiliaryGoodsCreateData1 != null, (a
, a1
) => a1.GoodsCreateData >= new DateTime(startAuxiliaryGoodsCreateData.ToDate().Year, startAuxiliaryGoodsCreateData.ToDate().Month, startAuxiliaryGoodsCreateData.ToDate().Day, 0, 0, 0))
                .WhereIF(queryAuxiliaryGoodsCreateData1 != null, (a
, a1
) => a1.GoodsCreateData <= new DateTime(endAuxiliaryGoodsCreateData.ToDate().Year, endAuxiliaryGoodsCreateData.ToDate().Month, endAuxiliaryGoodsCreateData.ToDate().Day, 23, 59, 59))
                .Select((a
, a1
) => new ZjnBaseGoodsListOutput
{
    a_F_Id = a.Id,
    a_F_GoodsId = a.GoodsId,
    a_F_GoodsCode = a.GoodsCode,
    a_goodsName = a.GoodsName,
    a_F_Unit = a.Unit,
    a_F_TrayNo = a.TrayNo,
    a_F_EnabledMark = a.EnabledMark,
    a1_F_EnabledMark = a1.EnabledMark,
    a1_F_batch = a1.Batch,
    a1_F_specifications = a1.Specifications,
    a1_F_GoodsCreateData = a1.GoodsCreateData,
    a1_F_GoodsState = a1.GoodsState,
    a1_F_GoodsLocationNo = a1.GoodsLocationNo,
    a1_F_CustomerId = a1.CustomerId,
    a1_F_PalledNo = a1.PalledNo,
    a1_F_VendorId = a1.VendorId,
    a1_F_CheckDate = a1.CheckDate,
    a1_F_CheckType = a1.CheckType,
    a1_F_GoodsGrade = a1.GoodsGrade,
    a1_F_Remarks = a1.Remarks,
}).OrderBy(sidx + " " + input.sort).ToListAsync();
            return data;
        }

        /// <summary>
		/// 导出货物信息
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnBaseGoodsListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnBaseGoodsListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnBaseGoodsListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"货物ID\",\"field\":\"goodsId\"},{\"value\":\"货物编码\",\"field\":\"goodsCode\"},{\"value\":\"数量\",\"field\":\"quantity\"},{\"value\":\"货物单位\",\"field\":\"unit\"},{\"value\":\"托盘编号\",\"field\":\"trayNo\"},{\"value\":\"有效标志\",\"field\":\"enabledMark\"},{\"value\":\"货物批次\",\"field\":\"hsz_zjn_base_goods_details_hsz_batch\"},{\"value\":\"货物规格\",\"field\":\"hsz_zjn_base_goods_details_hsz_specifications\"},{\"value\":\"生产日期\",\"field\":\"hsz_zjn_base_goods_details_hsz_goodsCreateData\"},{\"value\":\"货物状态\",\"field\":\"hsz_zjn_base_goods_details_hsz_goodsState\"},{\"value\":\"货位ID\",\"field\":\"hsz_zjn_base_goods_details_hsz_goodsLocationNo\"},{\"value\":\"客户ID\",\"field\":\"hsz_zjn_base_goods_details_hsz_customerId\"},{\"value\":\"检验日期\",\"field\":\"hsz_zjn_base_goods_details_hsz_checkDate\"},{\"value\":\"货物等级\",\"field\":\"hsz_zjn_base_goods_details_hsz_goodsGrade\"},{\"value\":\"检验类型\",\"field\":\"hsz_zjn_base_goods_details_hsz_checkType\"},{\"value\":\"供应商ID\",\"field\":\"hsz_zjn_base_goods_details_hsz_vendorId\"},{\"value\":\"托盘ID\",\"field\":\"hsz_zjn_base_goods_details_hsz_palledNo\"},{\"value\":\"货物描述\",\"field\":\"hsz_zjn_base_goods_details_hsz_remarks\"},{\"value\":\"有效标志\",\"field\":\"hsz_zjn_base_goods_details_hsz_enabledMark\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "货物信息.xls";
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
            ExcelExportHelper<ZjnBaseGoodsListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除货物信息
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnBaseGoodsRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除货物信息
                    await _zjnBaseGoodsRepository.AsDeleteable().In(d => d.Id, ids).ExecuteCommandAsync();

                    //请空副表数据
                    await _zjnBaseGoodsDetailsRepository.AsDeleteable().In(it => it.ParentId, entitys.Select(s => s.Id).ToArray()).ExecuteCommandAsync();
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
        /// 更新货物信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnBaseGoodsUpInput input)
        {

            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnBaseGoodsEntity>();
            var entity1 = input.Adapt<ZjnBaseGoodsDetailsEntity>();

            entity1.LastModifyUserId = userInfo.userId;
            entity1.LastModifyTime = DateTime.Now;

            try
            {
                //开启事务
                _db.BeginTran();

                await _zjnBaseGoodsRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();

                await _zjnBaseGoodsDetailsRepository.AsUpdateable(entity1).Where(it => it.ParentId.Equals(entity.Id)).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();


                //关闭事务
                _db.CommitTran();
            }
            catch (Exception)
            {
                //回滚事务
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);
            }
        }

        /// <summary>
        /// 删除货物信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnBaseGoodsRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            try
            {
                //开启事务
                _db.BeginTran();

                await _zjnBaseGoodsRepository.AsDeleteable().Where(it => it.Id == id).ExecuteCommandAsync();

                await _zjnBaseGoodsDetailsRepository.AsDeleteable().Where(it => it.ParentId.Equals(entity.Id)).ExecuteCommandAsync();

                //关闭事务
                _db.CommitTran();
            }
            catch (Exception)
            {
                //回滚事务
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);
            }
        }


        /// <summary>
        /// 原材料模板下载
        /// </summary>
        /// <returns></returns>
        [HttpGet("TheRawMaterial")]
        public dynamic TheRawMaterial()
        {
            var dataList = new List<ZjnBaseGoodsCrInput>() { new ZjnBaseGoodsCrInput() { } };//初始化 一条空数据
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "物料基础导入模板.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetTheRawMaterial();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBaseGoodsCrInput>.Export(dataList, excelconfig, addPath);
            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };

        }

        private Dictionary<string, string> GetTheRawMaterial(List<string> fields = null)
        {
            var res = new Dictionary<string, string>();
            res.Add("goodsId", "货物ID");
            res.Add("goodsCode", "货物代码");
            res.Add("goodstype", "货物类型");
            res.Add("goodsName", "名称");
            res.Add("unit", "单位");
            res.Add("trayNo", "托盘号");
            res.Add("enabledMark", "有效标志"); 
            res.Add("hsz_zjn_base_goods_details_hsz_batch", "货物批次");
            res.Add("hsz_zjn_base_goods_details_hsz_specifications", "货物规格");
            res.Add("hsz_zjn_base_goods_details_hsz_goodsCreateData", "生产日期");
            res.Add("hsz_zjn_base_goods_details_hsz_goodsState", "货物状态");
            res.Add("hsz_zjn_base_goods_details_hsz_goodsLocationNo", "货位ID");
            res.Add("hsz_zjn_base_goods_details_hsz_customerId", "客户ID");
            res.Add("hsz_zjn_base_goods_details_hsz_palledNo", "托盘ID");
            res.Add("hsz_zjn_base_goods_details_hsz_vendorId", "供应商ID");
            res.Add("hsz_zjn_base_goods_details_hsz_checkDate", "检验日期");
            res.Add("hsz_zjn_base_goods_details_hsz_checkType", "检验类型");
            res.Add("hsz_zjn_base_goods_details_hsz_goodsGrade", "货物等级");
            res.Add("hsz_zjn_base_goods_details_hsz_remarks", "货物描述");
            
            if (fields == null || !fields.Any()) return res;

            var result = new Dictionary<string, string>();

            foreach (var item in res)
            {
                if (fields.Contains(item.Key))
                    result.Add(item.Key, item.Value);
            }

            return result;

        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ImportData")]
        public async Task<dynamic> ImportData(List<ZjnBaseGoodsCrInput> list)
        {
            var res = await ImportTheRawMaterialDatas(list);
            var addlist = res.First() as List<ZjnBaseGoodsCrInput>;
            var errorlist = res.Last() as List<ZjnBaseGoodsCrInput>;
            return new ZjnBaseGoodsCrInputResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }

        /// <summary>
        /// 导入供应商数据函数
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>[成功列表,失败列表]</returns>
        private async Task<object[]> ImportTheRawMaterialDatas(List<ZjnBaseGoodsCrInput> list)
        {
            List<ZjnBaseGoodsCrInput> billsHistoryList = list;

            #region 排除错误数据
            if (billsHistoryList == null || billsHistoryList.Count() < 1)
                throw HSZException.Oh(ErrorCode.D5019);

            //必填字段验证 
            var errorList = billsHistoryList.Where(x =>  !x.goodsCode.IsNotEmptyOrNull()).ToList();
            var _zjnBaseGoodsRepostoryList = await _zjnBaseGoodsRepository.GetListAsync();//数据集

            var repeat = _zjnBaseGoodsRepostoryList.Where(u => billsHistoryList.Select(x => x.goodsCode).Contains(u.GoodsCode)).ToList();//已存在的编号

           if (repeat.Any())
              errorList.AddRange(billsHistoryList.Where(u => repeat.Select(x => x.GoodsCode).Contains(u.goodsCode)));//已存在的编号 列入 错误列表

            billsHistoryList = billsHistoryList.Except(errorList).ToList();
            #endregion
            var ZjnBaseGoodsList = new List<ZjnBaseGoodsEntity>();
            var ZjnBaseGoodsDetailsList = new List<ZjnBaseGoodsDetailsEntity>();
           
            var userInfo = await _userManager.GetUserInfo();

            foreach (var item in billsHistoryList)
            {
                var Bnum = YitIdHelper.NextId().ToString();
                var BaseGoods = new ZjnBaseGoodsEntity();
                BaseGoods.CreateTime = DateTime.Now;
                BaseGoods.CreateUser = userInfo.userId;
                BaseGoods.EnabledMark =Convert.ToInt32(item.enabledMark);
                BaseGoods.GoodsCode = item.goodsCode;
                BaseGoods.GoodsId = item.goodsId;
                BaseGoods.GoodsName = item.goodsName;
                BaseGoods.GoodsType = item.goodstype;
                BaseGoods.Id= Bnum;
                BaseGoods.TrayNo = item.trayNo;
                BaseGoods.Unit = item.unit;
                ZjnBaseGoodsList.Add(BaseGoods);
                var uentity = new ZjnBaseGoodsDetailsEntity();
                uentity.CheckDate = item.hsz_zjn_base_goods_details_hsz_checkDate;
                uentity.Batch = item.hsz_zjn_base_goods_details_hsz_batch;
                uentity.CheckType = item.hsz_zjn_base_goods_details_hsz_checkType;
                uentity.CreatorTime = DateTime.Now;
                uentity.CreatorUserId= userInfo.userId;
                uentity.CustomerId = item.hsz_zjn_base_goods_details_hsz_customerId;
                uentity.EnabledMark = Convert.ToInt32(item.enabledMark);
                uentity.GoodsCreateData = item.hsz_zjn_base_goods_details_hsz_goodsCreateData;
                uentity.GoodsGrade = item.hsz_zjn_base_goods_details_hsz_goodsGrade;
                uentity.GoodsLocationNo = item.hsz_zjn_base_goods_details_hsz_goodsLocationNo;
                uentity.GoodsState = item.hsz_zjn_base_goods_details_hsz_goodsState;
                uentity.Id= YitIdHelper.NextId().ToString();
                uentity.PalledNo = item.hsz_zjn_base_goods_details_hsz_palledNo;
                uentity.ParentId = Bnum;
                uentity.Remarks = item.hsz_zjn_base_goods_details_hsz_remarks;
                uentity.Specifications = item.hsz_zjn_base_goods_details_hsz_specifications;
                uentity.VendorId = item.hsz_zjn_base_goods_details_hsz_vendorId;
                uentity.LastModifyUserId=userInfo.userId;
                uentity.LastModifyTime = DateTime.Now;
                ZjnBaseGoodsDetailsList.Add(uentity);
            }

            if (ZjnBaseGoodsList.Any())
            {
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增记录
                    var newEntity = await _zjnBaseGoodsRepository.AsInsertable(ZjnBaseGoodsList).ExecuteReturnEntityAsync();
                    await _zjnBaseGoodsDetailsRepository.AsInsertable(ZjnBaseGoodsDetailsList).ExecuteReturnEntityAsync();
                    _db.CommitTran();
                }
                catch (Exception e)
                {
                    string es = e.Message;
                    _db.RollbackTran();
                    errorList.AddRange(billsHistoryList);
                }
            }
            return new object[] { billsHistoryList, errorList };
        }

        /// <summary>
        /// 导出错误报告
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ExportExceptionData")]
        public async Task<dynamic> ExportExceptionData(List<ZjnBaseGoodsCrInput> list)
        {
            var res = await ImportTheRawMaterialDatas(list);
            var errorlist = res.Last() as List<ZjnBaseGoodsCrInput>;//错误数据

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "物料基础信息导入错误报告.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetTheRawMaterial();
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBaseGoodsCrInput>.Export(errorlist, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }


        /// <summary>
        /// 导入预览
        /// </summary>
        /// <returns></returns>
        [HttpGet("ImportPreview")]
        public dynamic ImportPreview(string fileName)
        {
            try
            {
                var FileEncode = GetTheRawMaterial();

                var filePath = FileVariable.TemporaryFilePath;
                var savePath = filePath + fileName;
                //得到数据
                var excelData = ExcelImportHelper.ToDataTable(savePath);
                foreach (var item in excelData.Columns)
                {
                    excelData.Columns[item.ToString()].ColumnName = FileEncode.Where(x => x.Value == item.ToString()).FirstOrDefault().Key;
                }

                //返回结果
                return new { dataRow = excelData };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw HSZException.Oh(ErrorCode.D1801);
            }
        }



        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("ExportExcel")]
        public async Task<dynamic> ExportExcel([FromQuery] ZjnBaseGoodsListQueryInput input)
        {
            //供应商信息列表
            var userList = new List<ZjnBaseGoodsListOutput>();
            var sidx = input.sidx == null ? "a.a_CreateTime" : input.sidx;

            if (input.dataType == 0)
            {
                Regex r = new Regex("_");
                List<string> queryAuxiliaryGoodsCreateData1 = input.a_CreateTime != null ? input.a_CreateTime.Split(',').ToObject<List<string>>() : null;
                DateTime? startAuxiliaryGoodsCreateData = queryAuxiliaryGoodsCreateData1 != null ? Ext.GetDateTime(queryAuxiliaryGoodsCreateData1.First()) : null;
                DateTime? endAuxiliaryGoodsCreateData = queryAuxiliaryGoodsCreateData1 != null ? Ext.GetDateTime(queryAuxiliaryGoodsCreateData1.Last()) : null;
                userList = await _zjnBaseGoodsRepository.AsSugarClient()
                .Queryable<ZjnBaseGoodsEntity>()
                .LeftJoin<ZjnBaseGoodsDetailsEntity>((a, a1) => a1.ParentId == a.Id)
                .LeftJoin<ZjnPlaneLocationEntity>((a, a1, a2) => a2.Id == a1.GoodsLocationNo)
                .WhereIF(!string.IsNullOrEmpty(input.a_F_GoodsCode), a => a.GoodsCode.Contains(input.a_F_GoodsCode))
                .WhereIF(!string.IsNullOrEmpty(input.a1_F_batch), (a, a1) => a1.Batch.Contains(input.a1_F_batch))
                .WhereIF(queryAuxiliaryGoodsCreateData1 != null, a => a.CreateTime >= new DateTime(startAuxiliaryGoodsCreateData.ToDate().Year, startAuxiliaryGoodsCreateData.ToDate().Month, startAuxiliaryGoodsCreateData.ToDate().Day, 0, 0, 0))

                .WhereIF(queryAuxiliaryGoodsCreateData1 != null, a => a.CreateTime <= new DateTime(endAuxiliaryGoodsCreateData.ToDate().Year, endAuxiliaryGoodsCreateData.ToDate().Month, endAuxiliaryGoodsCreateData.ToDate().Day, 23, 59, 59))
                  .Select((a, a1, a2) => new ZjnBaseGoodsListOutput
                  {
                      a_F_Id = a.Id,
                      a_F_GoodsCode = a.GoodsCode,
                      a_goodsName = a.GoodsName,
                      a_GoodsType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.GoodsType && s.DictionaryTypeId == "325449144728552709").Select(s => s.FullName),
                      a1_F_specifications = a1.Specifications,
                      a1_F_GoodsState = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a1.GoodsState && s.DictionaryTypeId == "326590282281780485").Select(s => s.FullName),
                      a_F_Unit = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.Unit && s.DictionaryTypeId == "326384591566800133").Select(s => s.FullName),
                      a1_F_CheckType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a1.CheckType && s.DictionaryTypeId == "325448312364729605").Select(s => s.FullName),
                      a_CreateTime = a.CreateTime,
                      a1_F_LastModifyTime = a1.LastModifyTime,
                      a_F_EnabledMark2 = a.EnabledMark == 1 ? "启用" : "禁用",
                      a1_F_Remarks = a1.Remarks,
                  }).ToListAsync();

            }
            else
            {
                userList = await _zjnBaseGoodsRepository.AsSugarClient()
                .Queryable<ZjnBaseGoodsEntity>() 
                .LeftJoin<ZjnBaseGoodsDetailsEntity>((a, a1) => a1.ParentId == a.Id)
                .LeftJoin<ZjnPlaneLocationEntity>((a, a1, a2) => a2.Id == a1.GoodsLocationNo)
                .WhereIF(!string.IsNullOrEmpty(input.a_F_GoodsCode), a => a.GoodsCode.Contains(input.a_F_GoodsCode))
                .WhereIF(!string.IsNullOrEmpty(input.a1_F_batch), (a, a1) => a1.Batch.Contains(input.a1_F_batch))                
                  .Select((a, a1, a2) => new ZjnBaseGoodsListOutput
                  {
                      a_F_Id = a.Id,
                      a_F_GoodsCode = a.GoodsCode,
                      a_goodsName = a.GoodsName,
                      a_GoodsType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.GoodsType && s.DictionaryTypeId == "325449144728552709").Select(s => s.FullName),
                      a1_F_specifications = a1.Specifications,
                      a1_F_GoodsState = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a1.GoodsState && s.DictionaryTypeId == "326590282281780485").Select(s => s.FullName),
                      a_F_Unit = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.Unit && s.DictionaryTypeId == "326384591566800133").Select(s => s.FullName),
                      a1_F_CheckType = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a1.CheckType && s.DictionaryTypeId == "325448312364729605").Select(s => s.FullName),
                      a_CreateTime = a.CreateTime,
                      a1_F_LastModifyTime = a1.LastModifyTime,
                      a_F_EnabledMark2 = a.EnabledMark == 1 ? "启用" : "禁用",
                      a1_F_Remarks = a1.Remarks,
                  }).ToListAsync();
            }

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = DateTime.Now.ToDateString() + "物料基础信息导出数据.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetUserInfoFieldToTitle(input.selectKey.Split(',').ToList());
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnBaseGoodsListOutput>.Export(userList, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }


        /// <summary>
        /// 原材料信息 字段对应 列名称
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetUserInfoFieldToTitle(List<string> fields = null)
        {
            var res = new Dictionary<string, string>();
            res.Add("a_F_GoodsId", "货物ID");
            res.Add("a_F_GoodsCode", "货物编码");
            res.Add("a_goodsName", "货物名称");
            res.Add("a_GoodsType", "货物类型");
            res.Add("a1_F_specifications", "货物规格");
            res.Add("a1_F_GoodsState", "货物状态");
            res.Add("a_F_Unit", "货物单位");
            res.Add("a1_F_CheckType", "检验类型");
            res.Add("a_F_EnabledMark2", "有效标志");
            res.Add("a1_F_GoodsCreateData", "生产日期");
            res.Add("a1_F_CustomerId", "客户ID");          
            res.Add("a1_F_VendorId", "供应商ID");
            res.Add("a1_F_CheckDate", "检验日期");
            
            res.Add("a1_F_GoodsGrade", "货物等级");
            res.Add("a1_F_Remarks", "货物描述");
           
            if (fields == null || !fields.Any()) return res;

            var result = new Dictionary<string, string>();

            res.Add("a_F_EnabledMark", "有效标志");
            res.Add("a_CreateUser", "创建者");
            res.Add("a_CreateTime", "创建时间");
            res.Add("a1_F_LastModifyUserId", "修改者");
            res.Add("a1_F_LastModifyTime", "修改时间");

            foreach (var item in res)
            {
                if (fields.Contains(item.Key))
                    result.Add(item.Key, item.Value);
            }

            return result;
        }

    }
}


