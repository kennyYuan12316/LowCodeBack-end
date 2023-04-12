using HSZ.ClayObject;
using HSZ.Common.Configuration;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Common.Model.NPOI;
using HSZ.DataEncryption;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.Entitys.wms;
using HSZ.FriendlyException;
using HSZ.System.Entitys.System;
using HSZ.wms.Entitys.Dto.ZjnPlaneGoods;
using HSZ.wms.Entitys.Dto.ZjnWmsGoods;
using HSZ.wms.Interfaces.ZjnPlaneGoods;
using HSZ.Wms.Entitys.Dto.zjnPlaneGoods;
using HSZ.Wms.Entitys.Dto.ZjnWmsGoods;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace HSZ.Wms.ZjnWmsGoods
{
    /// <summary>
    /// Wms货物基础信息服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnWmsGoods", Order = 200)]
    [Route("api/wms/[controller]")]
   public class ZjnWmsGoodsWmsService : IZjnWmsGoodsService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnWmsGoodsEntity> _zjnWmsGoodsRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        private readonly ISqlSugarRepository<ZjnWmsOperationLogEntity> _zjnPlaneOperationLogRepository;
        /// <summary>
        /// 初始化一个<see cref="ZjnWmsGoodsWmsService"/>类型的新实例
        /// </summary>
        public ZjnWmsGoodsWmsService(ISqlSugarRepository<ZjnWmsGoodsEntity> zjnWmsGoodsRepository, ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository, ISqlSugarRepository<ZjnWmsOperationLogEntity> zjnPlaneOperationLogRepository,
            IUserManager userManager)
        {
            _zjnPlaneOperationLogRepository = zjnPlaneOperationLogRepository;
            _zjnWmsGoodsRepository = zjnWmsGoodsRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取Wms货物基础信息
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnWmsGoodsRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnPlaneGoodsInfoOutput>();
            return output;
        }

        /// <summary>
		/// 获取Wms货物基础信息列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnWmsGoodsListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWmsGoodsRepository.AsSugarClient().Queryable<ZjnWmsGoodsEntity>().Where(x => x.IsDelete == 0)
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsCode), a => a.GoodsCode.Contains(input.F_GoodsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsName), a => a.GoodsName.Contains(input.F_GoodsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsType), a => a.GoodsType.Equals(input.F_GoodsType))
                .Select((a) => new ZjnWmsGoodsListOutput
                {
                    F_Id = a.Id,
                    F_GoodsCode = a.GoodsCode,
                    F_GoodsName = a.GoodsName,
                    F_Unit = a.Unit,
                    F_GoodsState = a.GoodsState,
                    F_GoodsType = a.GoodsType,
                    F_Specifications = a.Specifications,
                    F_CustomerId = a.CustomerId,
                    F_VendorId = a.VendorId,
                    F_CheckType = a.CheckType,
                    F_IsFirstOut = a.IsFirstOut,
                    F_TellDate = a.TellDate,
                    F_DisableMark = a.DisableMark,
                    F_Ceiling = a.Ceiling,
                    F_TheLowerLimit = a.TheLowerLimit,
                    CreateUser = a.CreateUser,
                    CreateTime = a.CreateTime,
                    F_ShelfLife = a.ShelfLife
                }).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnWmsGoodsListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 新建Wms货物基础信息
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnWmsGoodsCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsGoodsEntity>();
            if (string.IsNullOrEmpty(entity.GoodsCode) || string.IsNullOrEmpty(entity.GoodsName) || string.IsNullOrEmpty(input.unit) || string.IsNullOrEmpty(input.goodsType))
            {
                throw HSZException.Oh(ErrorCode.COM1000);
            }
            var list = _zjnWmsGoodsRepository.AsQueryable().Where(x => x.GoodsCode == entity.GoodsCode);
            if (list.Count() > 0)
            {
                throw HSZException.Oh(ErrorCode.COM1004);
            }

            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateUser = userInfo.userId;
            entity.CreateTime = DateTime.Now;
            entity.IsDelete = 0;

            var isOk = await _zjnWmsGoodsRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw HSZException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
		/// 获取Wms货物基础信息无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnWmsGoodsListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnWmsGoodsRepository.AsSugarClient().Queryable<ZjnWmsGoodsEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsCode), a => a.GoodsCode.Contains(input.F_GoodsCode))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsName), a => a.GoodsName.Contains(input.F_GoodsName))
                .WhereIF(!string.IsNullOrEmpty(input.F_GoodsType), a => a.GoodsType.Equals(input.F_GoodsType))
                .Select((a
) => new ZjnWmsGoodsListOutput
{
    F_Id = a.Id,
    F_GoodsCode = a.GoodsCode,
    F_GoodsName = a.GoodsName,
    UnitName = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.Unit.ToString() && s.DictionaryTypeId == "326384591566800133").Select(s => s.FullName),
    GoodsStateName = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.GoodsState.ToString() && s.DictionaryTypeId == "326590282281780485").Select(s => s.FullName),
    GoodsTypeName = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.GoodsType.ToString() && s.DictionaryTypeId == "325449144728552709").Select(s => s.FullName),
    F_Specifications = a.Specifications,
    F_CustomerId = a.CustomerId,
    F_VendorId = a.VendorId,
    CheckTypeNmae = SqlFunc.Subqueryable<DictionaryDataEntity>().Where(s => s.EnCode == a.CheckType.ToString() && s.DictionaryTypeId == "325448312364729605").Select(s => s.FullName),
    F_IsFirstOut = a.IsFirstOut,
    F_TellDate = a.TellDate,
    F_DisableMark = a.DisableMark,
    F_ShelfLife = a.ShelfLife
}).OrderBy(sidx + " " + input.sort).ToListAsync();
            return data;
        }

        /// <summary>
		/// 导出Wms货物基础信息
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnWmsGoodsListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnWmsGoodsListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnWmsGoodsListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            ///编号转换名称
            List<ZjnWmsGoodsListOutput> listPla = new List<ZjnWmsGoodsListOutput>();
            foreach (var item in exportData)
            {
                ZjnWmsGoodsListOutput zjnPlaneGoods = new ZjnWmsGoodsListOutput();
                zjnPlaneGoods.F_GoodsCode = item.F_GoodsCode;
                zjnPlaneGoods.F_GoodsName = item.F_GoodsName;
                zjnPlaneGoods.F_CustomerId = item.F_CustomerId;
                zjnPlaneGoods.F_VendorId = item.F_VendorId;
                //zjnPlaneGoods.IsFirstOutName = item.F_IsFirstOut == 1 ? "是" : "否";
                zjnPlaneGoods.F_TellDate = item.F_TellDate;
                zjnPlaneGoods.F_DisableMark = item.F_DisableMark;
                zjnPlaneGoods.F_Specifications = item.F_Specifications;
                zjnPlaneGoods.F_TheLowerLimit = item.F_TheLowerLimit;
                zjnPlaneGoods.F_Ceiling = item.F_Ceiling;
                var UnitName = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "326384591566800133" && s.EnCode == item.F_Unit.ToString()).Select(s => s.FullName).ToList();
                var GoodsType = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "325449144728552709" && s.EnCode == item.F_GoodsType.ToString()).Select(s => s.FullName).ToList();
                var GoodsState = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "326590282281780485" && s.EnCode == item.F_GoodsState.ToString()).Select(s => s.FullName).ToList();
                var CheckType = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "325448312364729605" && s.EnCode == item.F_CheckType.ToString()).Select(s => s.FullName).ToList();
                zjnPlaneGoods.UnitName = UnitName[0];
                zjnPlaneGoods.GoodsStateName = GoodsState[0];
                zjnPlaneGoods.GoodsTypeName = GoodsType[0];
                zjnPlaneGoods.CheckTypeNmae = CheckType[0];
                zjnPlaneGoods.F_ShelfLife = item.F_ShelfLife;
                listPla.Add(zjnPlaneGoods);
            }

            //表头
            List<ParamsModel> paramList = "[{\"value\":\"物料编码\",\"field\":\"F_GoodsCode\"},{\"value\":\"物料名称\",\"field\":\"F_GoodsName\"},{\"value\":\"物料类型\",\"field\":\"GoodsTypeName\"},{\"value\":\"物料单位\",\"field\":\"UnitName\"},{\"value\":\"物料状态\",\"field\":\"GoodsStateName\"},{\"value\":\"检验类型\",\"field\":\"CheckTypeNmae\"},{\"value\":\"库存上限\",\"field\":\"F_Ceiling\"},{\"value\":\"库存下限\",\"field\":\"F_TheLowerLimit\"},{\"value\":\"物料规格\",\"field\":\"F_Specifications\"},{\"value\":\"客户\",\"field\":\"F_CustomerId\"},{\"value\":\"供应商\",\"field\":\"F_VendorId\"},{\"value\":\"保质期\",\"field\":\"F_ShelfLife\"},{\"value\":\"预警周期(天)\",\"field\":\"F_TellDate\"},{\"value\":\"禁用原因\",\"field\":\"F_DisableMark\"},]".ToList<ParamsModel>();
            //Excel属性
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "平面库物料基础信息.xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            List<string> selectKeyList = input.selectKey.Split(',').ToList();
            foreach (var item in selectKeyList)
            {
                //显示表头名称
                var value = "";
                if (item == "F_GoodsType")
                {
                    value = "GoodsTypeName";
                }
                else if (item == "F_Unit")
                {
                    value = "UnitName";
                }
                else if (item == "F_GoodsState")
                {
                    value = "GoodsStateName";
                }
                else if (item == "F_CheckType")
                {
                    value = "CheckTypeNmae";
                }
                //else if (item == "F_IsFirstOut")
                //{
                //    value = "IsFirstOutName";
                //}
                else
                {
                    value = item;
                }
                var isExist = paramList.Find(p => p.field == value);
                if (isExist != null)
                {
                    excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = isExist.field, ExcelColumn = isExist.value });
                }
            }
            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnWmsGoodsListOutput>.Export(listPla, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 更新Wms货物基础信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnWmsGoodsUpInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnWmsGoodsEntity>();
            ///查询原先内容
            var entity1 = await _zjnWmsGoodsRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity1 ?? throw HSZException.Oh(ErrorCode.COM1005);
            if (input.goodsCode != entity1.GoodsCode)
            {
                throw HSZException.Oh("物料编号不可以修改");
            }
            if (string.IsNullOrEmpty(entity.GoodsCode) || string.IsNullOrEmpty(entity.GoodsName) || string.IsNullOrEmpty(input.unit) || string.IsNullOrEmpty(input.goodsType))
            {
                throw HSZException.Oh(ErrorCode.COM1000);
            }
            entity.CreateUser = entity1.CreateUser;
            entity.CreateTime = entity1.CreateTime;
            entity.LastModifyTime = DateTime.Now;
            entity.LastModifyUserId = userInfo.userId;
            // 插入日志
            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            operationLogEntity.CreateUser = userInfo.userId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "平面库物料基础信息修改";
            operationLogEntity.AfterDate = entity.ToJson();//修改后的数据
            operationLogEntity.BeforeDate = entity1.ToJson();//修改前的数据
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 1;
            operationLogEntity.WorkPath = 2;
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnWmsGoodsRepository.AsUpdateable(entity).ExecuteCommandAsync();
                //新增日子记录
                var isOk1 = await _zjnPlaneOperationLogRepository.AsInsertable(operationLogEntity).ExecuteReturnEntityAsync();

                _db.CommitTran();
            }
            catch (Exception e)
            {
                string es = e.Message;
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);

            }
        }

        /// <summary>
        /// 删除Wms货物基础信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var userInfo = await _userManager.GetUserInfo();
            ZjnWmsOperationLogEntity operationLogEntity = new ZjnWmsOperationLogEntity();
            var entity = await _zjnWmsGoodsRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            operationLogEntity.AfterDate = entity.ToJson();//修改后的数据
            //取消
            entity.IsDelete = 1;

            // 插入日志           
            operationLogEntity.CreateUser = userInfo.userId;
            operationLogEntity.CreateTime = DateTime.Now;
            operationLogEntity.Describe = "平面库物料基础信息删除";
            operationLogEntity.BeforeDate = entity.ToJson();//修改前的数据
            operationLogEntity.Id = YitIdHelper.NextId().ToString();
            operationLogEntity.Type = 2;
            operationLogEntity.WorkPath = 2;
            try
            {
                //开启事务
                _db.BeginTran();
                //修改数据
                var isOk = await _zjnWmsGoodsRepository.AsUpdateable(entity).ExecuteCommandAsync();
                //新增日子记录
                var isOk1 = await _zjnPlaneOperationLogRepository.AsInsertable(operationLogEntity).ExecuteReturnEntityAsync();

                _db.CommitTran();
            }
            catch (Exception e)
            {
                string es = e.Message;
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);

            }
        }



        /// <summary>
        /// 模板下载
        /// </summary>
        /// <returns></returns>
        [HttpGet("TheRawMaterial")]
        public dynamic TheRawMaterial(string fileName)
        {

            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = fileName + ".xls";
            excelconfig.HeadFont = "微软雅黑";
            excelconfig.HeadPoint = 10;
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnModel = new List<ExcelColumnModel>();
            var filedList = GetTheRawMaterial(null, 1);
            int i = 0;
            foreach (var item in filedList)
            {
                var column = item.Key;
                var excelColumn = item.Value;
                excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = column, ExcelColumn = excelColumn });
            }
            //插入案例
            var UnitName = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "326384591566800133").Select(s => new ZjnWmsGoodsListOutput { UnitName = s.FullName + ":" + s.EnCode + ";" }).ToList();
            var GoodsType = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "325449144728552709").Select(s => new ZjnWmsGoodsListOutput { UnitName = s.FullName + ":" + s.EnCode + ";" }).ToList();
            var GoodsState = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "326590282281780485").Select(s => new ZjnWmsGoodsListOutput { UnitName = s.FullName + ":" + s.EnCode + ";" }).ToList();
            var CheckType = _dictionaryDataRepository.AsQueryable().Where(s => s.DictionaryTypeId == "325448312364729605").Select(s => new ZjnWmsGoodsListOutput { UnitName = s.FullName + ":" + s.EnCode + ";" }).ToList();
            var dataList = new List<ZjnWmsGoodsListOutput>() { new ZjnWmsGoodsListOutput() {

                //以下顺序不能变，切记
                F_GoodsCode="xxxxxx-0",
                F_GoodsName="案例",
                UnitName="说明："+GettName(UnitName),
                GoodsTypeName="说明:原材料库:A01,其他类型请参考数据字典",//+GettName(GoodsType),
                CheckTypeNmae="说明"+GettName(CheckType),
                GoodsStateName="说明:"+GettName(GoodsState),
                F_Specifications="100*8",
                F_Ceiling = "100000",
                F_TheLowerLimit = "1000",
                F_CustomerId="001",
                F_VendorId="002",
                F_ShelfLife="保质期",
                F_TellDate=30,
                F_DisableMark="案例不会导入"
            } };//初始化 一条空数据

            var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
            ExcelExportHelper<ZjnWmsGoodsListOutput>.Export(dataList, excelconfig, addPath);
            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };

        }





        public string GettName(List<ZjnWmsGoodsListOutput> lists)
        {
            string Nmae = "";
            foreach (var item in lists)
            {
                Nmae += "" + item.UnitName;
            }
            return Nmae;
        }

        /// <summary>
        /// type 1.下载模板
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetTheRawMaterial(List<string> fields = null, int type = 0)
        {

            var res = new Dictionary<string, string>();
            res.Add("F_GoodsCode", "物料编码");
            res.Add("F_GoodsName", "物料名称");
            res.Add("UnitName", "物料单位");
            res.Add("GoodsStateName", "物料状态");
            res.Add("GoodsTypeName", "物料类型");
            res.Add("F_Specifications", "物料规格");
            res.Add("F_Ceiling", "库存上限");
            res.Add("F_TheLowerLimit", "库存下限");
            res.Add("CheckTypeNmae", "检验类型");
            res.Add("F_CustomerId", "客户编号");
            res.Add("F_VendorId", "供应商编号");
            res.Add("F_ShelfLife", "保质期");
            res.Add("F_TellDate", "预警周期(天)");
            res.Add("F_DisableMark", "禁用原因");

            var result = new Dictionary<string, string>();

            //模板下载
            if (type == 1)
            {
                var temp = new Dictionary<string, string>();
                //必填字段
                temp.Add("F_GoodsCode", " **物料编码** ");
                temp.Add("F_GoodsName", " **物料名称** ");
                temp.Add("UnitName", " **物料单位** ");
                temp.Add("GoodsTypeName", " **物料类型** ");
                temp.Add("CheckTypeNmae", " **检验类型** ");
                temp.Add("GoodsStateName", "物料状态(不填默认可用)");
                temp.Add("F_Specifications", "物料规格");
                temp.Add("F_Ceiling", "库存上限(不填默认100000)");
                temp.Add("F_TheLowerLimit", "库存下限(不填默认1000)");
                temp.Add("F_CustomerId", "客户编号");
                temp.Add("F_VendorId", "供应商编号");
                temp.Add("F_ShelfLife", "**保质期**");
                temp.Add("F_TellDate", "预警周期(天，不填默认30)");
                temp.Add("F_DisableMark", "禁用原因");
                return temp;
            }

            if (fields == null || !fields.Any())
            {
                return res;
            }

            //其他
            foreach (var item in res)
            {
                if (fields.Contains(item.Key))
                    result.Add(item.Key, item.Value);
            }

            return result;

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
                var FileEncode = GetTheRawMaterial(null, 1);

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
        /// 入库导入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("RawMaterialWarehousingData")]
        public async Task<dynamic> RawMaterialWarehousingData(List<ZjnWmsGoodsListOutput> list)
        {
            var res = await ImportTheRawMaterialDatas(list);
            var addlist = res.First() as List<ZjnWmsGoodsListOutput>;
            var errorlist = res.Last() as List<ZjnWmsGoodsListOutput>;
            return new WmsGoodsImportResultOutput() { snum = addlist.Count, fnum = errorlist.Count, failResult = errorlist, resultType = errorlist.Count < 1 ? 0 : 1 };
        }




        /// <summary>
        /// 导入数据函数
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>[成功列表,失败列表]</returns>
        private async Task<object[]> ImportTheRawMaterialDatas(List<ZjnWmsGoodsListOutput> list)
        {
            List<ZjnWmsGoodsListOutput> PlaneGoodsList = list;

            #region 排除错误数据
            if (PlaneGoodsList == null || PlaneGoodsList.Count() < 1)
                throw HSZException.Oh(ErrorCode.D5019);

            //必填字段验证 
            var errorList = PlaneGoodsList.Where(x => string.IsNullOrEmpty(x.F_GoodsCode) || string.IsNullOrEmpty(x.F_GoodsName) || string.IsNullOrEmpty(x.UnitName) || string.IsNullOrEmpty(x.GoodsTypeName) || string.IsNullOrEmpty(x.CheckTypeNmae.ToString())).ToList();
            var _zjnBillsHistoryRepositoryList = await _zjnWmsGoodsRepository.GetListAsync();//数据集
            var repeat = _zjnBillsHistoryRepositoryList.Where(u => PlaneGoodsList.Select(x => x.F_GoodsCode).Contains(u.GoodsCode)).ToList();//已存在的编号           
            if (repeat.Any())
                errorList.AddRange(PlaneGoodsList.Where(u => repeat.Select(x => x.GoodsCode).Contains(u.F_GoodsCode) && !errorList.Select(x => x.F_GoodsCode).Contains(u.F_GoodsCode)));//已存在的编号 列入 错误列表

            PlaneGoodsList = PlaneGoodsList.Except(errorList).ToList();
            #endregion

            var PlaneGoods = new List<ZjnWmsGoodsEntity>();
            var userInfo = await _userManager.GetUserInfo();

            foreach (var item in PlaneGoodsList)
            {
                var uentity = new ZjnWmsGoodsEntity();
                uentity.Id = YitIdHelper.NextId().ToString();
                uentity.CreateTime = DateTime.Now;
                uentity.CreateUser = userInfo.userId;
                uentity.GoodsCode = item.F_GoodsCode;
                uentity.GoodsName = item.F_GoodsName;
                uentity.Unit = Convert.ToInt32(item.UnitName);
                uentity.GoodsState = Convert.ToInt32(string.IsNullOrEmpty(item.GoodsStateName) ? "1" : item.GoodsStateName);
                uentity.GoodsType = item.GoodsTypeName;
                uentity.Specifications = item.F_Specifications;
                uentity.CustomerId = item.F_CustomerId;
                uentity.VendorId = item.F_VendorId;
                uentity.CheckType = item.CheckTypeNmae == null ? 0 : Convert.ToInt32(item.CheckTypeNmae);
                uentity.IsFirstOut = item.F_IsFirstOut == null ? 0 : item.F_IsFirstOut;
                uentity.TellDate = item.F_TellDate == null ? 30 : item.F_TellDate;
                uentity.DisableMark = item.F_DisableMark;
                uentity.IsDelete = 0;
                uentity.ShelfLife = item.F_ShelfLife;
                if (String.IsNullOrEmpty(item.F_Ceiling) || item.F_Ceiling == "0" || item.F_Ceiling.Contains('-'))
                {
                    uentity.Ceiling = "100000";
                }
                else
                {
                    uentity.Ceiling = item.F_Ceiling;
                }
                if (String.IsNullOrEmpty(item.F_TheLowerLimit) || item.F_TheLowerLimit == "0" || item.F_TheLowerLimit.Contains('-'))
                {
                    uentity.TheLowerLimit = "1000";
                }
                else
                {
                    uentity.TheLowerLimit = item.F_Ceiling;
                }
                PlaneGoods.Add(uentity);
            }

            if (PlaneGoods.Any())
            {
                try
                {
                    //开启事务
                    _db.BeginTran();

                    //新增记录
                    var newEntity = await _zjnWmsGoodsRepository.AsInsertable(PlaneGoods).ExecuteReturnEntityAsync();

                    _db.CommitTran();
                }
                catch (Exception e)
                {
                    string es = e.Message;
                    _db.RollbackTran();
                    errorList.AddRange(PlaneGoodsList);
                }
            }
            return new object[] { PlaneGoodsList, errorList };
        }

        /// <summary>
        /// 导出错误报告
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("ExportExceptionData")]
        public async Task<dynamic> ExportExceptionData(List<ZjnWmsGoodsListOutput> list)
        {
            var res = await ImportTheRawMaterialDatas(list);
            var errorlist = res.Last() as List<ZjnWmsGoodsListOutput>;//错误数据

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
            ExcelExportHelper<ZjnWmsGoodsListOutput>.Export(errorlist, excelconfig, addPath);

            return new { name = excelconfig.FileName, url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(_userManager.UserId + "|" + excelconfig.FileName + "|" + addPath, "HSZ") };
        }

    }
}
