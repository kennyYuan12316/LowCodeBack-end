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
using HSZ.wms.Entitys.Dto.ZjnPlaneBills;
using HSZ.wms.Interfaces.ZjnPlaneBills;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using HSZ.LinqBuilder;

namespace HSZ.wms.ZjnPlaneBills
{
    /// <summary>
    /// 出入库单据服务
    /// </summary>
    [ApiDescriptionSettings(Tag = "wms", Name = "ZjnPlaneBills", Order = 200)]
    [Route("api/wms/[controller]")]
    public class ZjnPlaneBillsService : IZjnPlaneBillsService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ZjnPlaneBillsEntity> _zjnPlaneBillsRepository;
        private readonly ISqlSugarRepository<ZjnPlaneBillsInOutOrderEntity> _zjnPlaneBillsInOutOrderRepository;
        private readonly ISqlSugarRepository<ZjnBillsHistoryEntity> _ZjnBillsHistoryRepository;
        private readonly ISqlSugarRepository<ZjnPlaneMaterialInventoryEntity> _ZjnPlaneMaterialInventoryRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ZjnPlaneBillsService"/>类型的新实例
        /// </summary>
        public ZjnPlaneBillsService(ISqlSugarRepository<ZjnPlaneBillsEntity> zjnPlaneBillsRepository,
            ISqlSugarRepository<ZjnPlaneBillsInOutOrderEntity> zjnPlaneBillsInOutOrderRepository,
            ISqlSugarRepository<ZjnBillsHistoryEntity> zjnBillsHistoryRepository,
            ISqlSugarRepository<ZjnPlaneMaterialInventoryEntity> zjnPlaneMaterialInventoryRepository,
            IUserManager userManager)
        {
            _zjnPlaneBillsRepository = zjnPlaneBillsRepository;
            _zjnPlaneBillsInOutOrderRepository = zjnPlaneBillsInOutOrderRepository;
            _ZjnBillsHistoryRepository = zjnBillsHistoryRepository;
            _ZjnPlaneMaterialInventoryRepository = zjnPlaneMaterialInventoryRepository;
            _userManager = userManager;
            //只能作为事务处理
            _db = DbScoped.SugarScope;
        }

        /// <summary>
        /// 获取出入库单据
        /// </summary>
        /// <param name="id">参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _zjnPlaneBillsRepository.GetFirstAsync(p => p.Id == id)).Adapt<ZjnPlaneBillsInfoOutput>();

            var zjnPlaneBillsInOutOrderList = await _zjnPlaneBillsInOutOrderRepository.GetListAsync(w => w.ParentId == output.id);
            output.zjnPlaneBillsInOutOrderList = zjnPlaneBillsInOutOrderList.Adapt<List<ZjnPlaneBillsInOutOrderInfoOutput>>();
            return output;
        }

        /// <summary>
		/// 获取出入库单据列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ZjnPlaneBillsListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnPlaneBillsRepository.AsSugarClient().Queryable<ZjnPlaneBillsEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_BillNo), a => a.BillNo.Contains(input.F_BillNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_BillType), a => a.BillType.Equals(input.F_BillType))
                .WhereIF(!string.IsNullOrEmpty(input.F_BillState), a => a.BillState.Equals(input.F_BillState))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Select(a => new ZjnPlaneBillsListOutput
                {
                    F_Id = a.Id,
                    F_BillNo = a.BillNo,
                    F_BillType = a.BillType,
                    F_BillState = a.BillState,
                    F_EnabledMark = a.EnabledMark,
                }).OrderBy(sidx + " " + input.sort).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<ZjnPlaneBillsListOutput>.SqlSugarPageResult(data);
        }


        /// <summary>
        /// 获取出入库单据列表--APP使用
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("GetListToApp")]
        public async Task<dynamic> GetListToApp([FromQuery] ZjnPlaneBillsListQueryInput input)
        {
            var queryWhere = LinqExpression.And<ZjnPlaneBillsListOutput>();
            //关键字（单据号码）
            if (input.keyword.IsNotEmptyOrNull())
            {
                var keyword = input.keyword.ToString();
                queryWhere = queryWhere.And(m => m.F_BillNo.Contains(keyword));
            }
            //出入库标识
            if (input.F_BillType.IsNotEmptyOrNull())
            {
                var F_BillType = input.F_BillType.ToString();
                queryWhere = queryWhere.And(m => m.F_BillType == F_BillType);
            }
            //起始日期-结束日期
            if (input.startTime.IsNotEmptyOrNull() && input.endTime.IsNotEmptyOrNull())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                queryWhere = queryWhere.And(x => SqlFunc.Between(x.F_CreateTime, start, end));
            }
            var data = await _zjnPlaneBillsRepository.AsSugarClient().Queryable<ZjnPlaneBillsEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_BillNo), a => a.BillNo.Contains(input.F_BillNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_BillType), a => a.BillType.Equals(input.F_BillType))
                .WhereIF(!string.IsNullOrEmpty(input.F_BillState), a => a.BillState.Equals(input.F_BillState))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Select(a => new ZjnPlaneBillsListOutput
                {
                    F_Id = a.Id,
                    F_BillNo = a.BillNo,
                    F_BillType = a.BillType,
                    F_BillState = a.BillState,
                    F_CreateTime = a.CreateTime,
                    F_BillWarehouse = a.BillWarehouse,
                    F_EnabledMark = a.EnabledMark,
                }).Where(queryWhere).ToPagedListAsync(input.currentPage, input.pageSize);
            //.OrderBy(a => a.sortCode)
            //.OrderBy(a => a.creatorTime, OrderByType.Desc)
            return PageResult<ZjnPlaneBillsListOutput>.SqlSugarPageResult(data);
        }


        /// <summary>
        /// 获取出入库单据明细--APP使用
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("GetDetailsToApp")]
        public async Task<dynamic> GetDetailsToApp([FromQuery] ZjnPlaneBillsInOutOrderEntity input)
        {
            var data = await _zjnPlaneBillsInOutOrderRepository.AsQueryable()
                  .LeftJoin<ZjnPlaneLocationEntity>((a, i) => a.ProductsLocation == i.LocationNo)
                 .Where(a => a.ParentId == input.ParentId && a.OrderState != "2")
                  .Select((a, i) => new ZjnPlaneBillsInOutOrderInfoOutput
                  {
                      id = a.Id,
                      ordeerNo = a.OrdeerNo,
                      productsNo = a.ProductsNo,
                      productsName = a.ProductsName,
                      productsTotal = a.ProductsTotal,
                      productsDate = a.ProductsDate,
                      productsLocation = a.ProductsLocation,
                      orderState = a.OrderState,
                      productsStyle = a.ProductsStyle,
                      productsLocationName = i.LocationNo,
                      productsDifference = a.ProductsDifference,
                      productsGrade = a.ProductsGrade,
                      productsType = a.ProductsType,
                      productsSupplier = a.ProductsSupplier,
                      productsBach = a.ProductsBach,
                      productsUnit = a.ProductsUnit,
                      productsUser = a.ProductsUser,

                      submitCount = a.ProductsTotal - a.ProductsDifference
                  })
                 .ToListAsync();
            var list = data.Adapt<List<ZjnPlaneBillsInOutOrderInfoOutput>>();
            return new { list = list };

        }


        /// <summary>
        /// 新建出入库单据
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ZjnPlaneBillsCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnPlaneBillsEntity>();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.CreateTime = DateTime.Now;
            entity.CreateUser = userInfo.userId;
            entity.LastModifyTime = DateTime.Now;
            entity.LastModifyUserId = userInfo.userId;

            try
            {
                _db.BeginTran();
                var newEntity = await _zjnPlaneBillsRepository.AsInsertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteReturnEntityAsync();

                var zjnPlaneBillsInOutOrderEntityList = input.zjnPlaneBillsInOutOrderList.Adapt<List<ZjnPlaneBillsInOutOrderEntity>>();
                if (zjnPlaneBillsInOutOrderEntityList != null)
                {
                    int num = 0;
                    foreach (var item in zjnPlaneBillsInOutOrderEntityList)
                    {
                        num = num + 1;
                        item.Id = YitIdHelper.NextId().ToString();
                        item.OrdeerNo = entity.BillNo + "-" + num.ToString();
                        item.ParentId = newEntity.Id;
                        item.CreateTime = DateTime.Now;
                        item.CreateUser = userInfo.userId;
                        item.LastModifyTime = DateTime.Now;
                        item.LastModifyUserId = userInfo.userId;
                        item.ProductsDifference = 0;
                        item.SubmitCount = 0;

                    }
                    await _zjnPlaneBillsInOutOrderRepository.AsInsertable(zjnPlaneBillsInOutOrderEntityList).ExecuteCommandAsync();
                }

                _db.CommitTran();
            }
            catch (Exception e)
            {
                string ex = e.Message;
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1000);
            }
        }


        /// <summary>
        /// 出入库提交 --- APP使用
        /// </summary>          
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("SumbitOrdersByApp")]
        public async Task SumbitOrdersByApp([FromBody] ZjnPlaneBillsCrInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var list = input.zjnPlaneBillsInOutOrderList.Adapt<List<ZjnPlaneBillsInOutOrderEntity>>();//明细表
            var historyList = new List<ZjnBillsHistoryEntity>();//入库记录表

            var updateList = new List<ZjnPlaneMaterialInventoryEntity>();//更新库存集合
            var addList = new List<ZjnPlaneMaterialInventoryEntity>();//新增库存集合

            try
            {
                _db.BeginTran();

                int isover = 0;
                foreach (var item in list)
                {

                    //复制到入库记录表
                    ZjnBillsHistoryEntity his = new ZjnBillsHistoryEntity();
                    his.CreateTime = DateTime.Now;
                    his.CreateUser = userInfo.userId;
                    his.Id = YitIdHelper.NextId().ToString();
                    his.OrderNo = item.OrdeerNo;
                    his.OrderType = input.billType;
                    his.ProductsType = item.ProductsType;
                    his.ProductsTotal = item.SubmitCount;
                    his.ProductsBach = item.ProductsBach;
                    his.ProductsGrade = item.ProductsGrade;
                    his.ProductsLocation = item.ProductsLocation;
                    his.ProductsName = item.ProductsName;
                    his.ProductsNo = item.ProductsNo;
                    his.ProductsStyle = item.ProductsStyle;
                    his.ProductsSupplier = item.ProductsSupplier;
                    his.ProductsUnit = item.ProductsUnit;
                    his.ProductsUser = item.ProductsUser;
                    his.BillId = input.billNo;
                    historyList.Add(his);

                    //差异数
                    item.ProductsDifference = item.ProductsDifference + item.SubmitCount;
                    //完成数
                    var tempCount = item.SubmitCount + item.ProductsDifference;
                    item.SubmitCount = item.SubmitCount + item.ProductsDifference;
                    if (item.ProductsTotal <= item.SubmitCount)
                    {
                        item.OrderState = "2";//已完成
                        if (input.billType == "001")//入库
                        {
                            item.ProductsInDate = DateTime.Now;
                        }
                        else
                        {
                            item.ProductsOutDate = DateTime.Now;
                        }
                    }
                    else
                    {
                        isover++;
                        item.OrderState = "1";//进行中
                    }
                    item.LastModifyTime = DateTime.Now;
                    item.LastModifyUserId = userInfo.userId;

                    //判断库存表 此处需判断库存又没有相同物料（目前只判断货物编码，后续会加上客户、供应商等）
                    var kucunEntity = await _ZjnPlaneMaterialInventoryRepository.GetFirstAsync(p => p.ProductsCode == item.ProductsNo);

                    if (kucunEntity == null)
                    {
                        ZjnPlaneMaterialInventoryEntity
                            temp = new ZjnPlaneMaterialInventoryEntity();
                        temp.Id = YitIdHelper.NextId().ToString();
                        temp.CreateTime = DateTime.Now;
                        temp.CreateUser = userInfo.userId;
                        temp.LastModifyTime = DateTime.Now;
                        temp.LastModifyUserId = userInfo.userId;
                        temp.ProductsBatch = item.ProductsBach;
                        temp.ProductsCheckType = item.ProductsCheckType;
                        temp.ProductsCode = item.ProductsNo;
                        temp.ProductsCustomer = item.ProductsUser;
                        temp.ProductsGrade = item.ProductsGrade;
                        temp.ProductsIsLock = 0;
                        temp.ProductsLocation = item.ProductsLocation;
                        temp.ProductsName = item.ProductsName;
                        temp.ProductsQuantity = item.SubmitCount;
                        temp.ProductsState = item.OrderState;
                        temp.ProductsStyle = item.ProductsStyle;
                        temp.ProductsSupplier = item.ProductsSupplier;
                        temp.ProductsTakeCount = 0;
                        temp.ProductsTakeStockTime = DateTime.Now;
                        temp.ProductsType = item.ProductsType;
                        temp.ProductsUnit = item.ProductsUnit;
                        addList.Add(temp);
                    }
                    else
                    {
                        if (input.billType == "001")//入库
                        {
                            kucunEntity.ProductsQuantity = kucunEntity.ProductsQuantity + item.SubmitCount;
                        }
                        else
                        {
                            kucunEntity.ProductsQuantity = kucunEntity.ProductsQuantity - item.SubmitCount;
                        }
                        kucunEntity.LastModifyTime = DateTime.Now;
                        kucunEntity.LastModifyUserId = userInfo.userId;
                        updateList.Add(kucunEntity);
                    }
                }

                //更新子表
                await _zjnPlaneBillsInOutOrderRepository.AsUpdateable(list)
                .UpdateColumns(it => new { it.ProductsDifference, it.SubmitCount, it.OrderState, it.LastModifyTime, it.LastModifyUserId }).ExecuteCommandAsync();


                //更新主表
                ZjnPlaneBillsEntity ent = await _zjnPlaneBillsRepository.GetFirstAsync(p => p.Id == input.parentId);
                ent.LastModifyTime = DateTime.Now;
                ent.LastModifyUserId = userInfo.userId;
                if (isover > 0)
                {
                    ent.BillState = "1";//进行中
                }
                else
                {
                    var result = await _zjnPlaneBillsInOutOrderRepository
                                 .GetFirstAsync(p => p.ParentId == input.parentId && p.OrderState != "2");
                    if (result == null)
                    {
                        ent.BillState = "2";//已完成
                    }
                    else
                    {
                        ent.BillState = "1";//进行中
                    }
                }
                await _zjnPlaneBillsRepository.AsUpdateable(ent)
                .UpdateColumns(it => new { it.BillState, it.LastModifyTime, it.LastModifyUserId }).ExecuteCommandAsync();

                //插入入库历史记录表
                await _ZjnBillsHistoryRepository.AsInsertable(historyList).ExecuteCommandAsync();

                if (input.billType == "001")//入库
                {
                    //增加库存表
                    if (addList.Count > 0)
                    {
                        await _ZjnPlaneMaterialInventoryRepository.AsInsertable(addList).ExecuteCommandAsync();
                    }
                }
                else
                {
                    //更新库存表
                    if (updateList.Count > 0)
                    {
                        await _ZjnPlaneMaterialInventoryRepository.AsUpdateable(updateList)
                    .UpdateColumns(it => new { it.ProductsQuantity, it.LastModifyTime, it.LastModifyUserId }).ExecuteCommandAsync();
                    }
                }
                _db.CommitTran();

            }
            catch (Exception e)
            {
                string ex = e.Message;
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1000);
            }
        }


        /// <summary>
        /// 根据单据id获取入库明细 --- APP使用
        /// </summary>          
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("GetOderHistoryList")]
        public async Task<List<ZjnBillsHistoryEntity>> GetOderHistoryList(string id)
        {
            var userInfo = await _userManager.GetUserInfo();
            var historyList = new List<ZjnBillsHistoryEntity>();//入库记录表
            try
            {
                return await _ZjnBillsHistoryRepository.AsQueryable().Where(x => x.BillId == id).OrderBy("F_CreateTime" + " " + "desc").ToListAsync();
            }
            catch (Exception e)
            {
                string ex = e.Message;
                throw HSZException.Oh(ErrorCode.COM1000);
            }
        }

        /// <summary>
		/// 获取出入库单据无分页列表
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetNoPagingList([FromQuery] ZjnPlaneBillsListQueryInput input)
        {
            var sidx = input.sidx == null ? "F_Id" : input.sidx;
            var data = await _zjnPlaneBillsRepository.AsSugarClient().Queryable<ZjnPlaneBillsEntity>()
                .WhereIF(!string.IsNullOrEmpty(input.F_BillNo), a => a.BillNo.Contains(input.F_BillNo))
                .WhereIF(!string.IsNullOrEmpty(input.F_BillType), a => a.BillType.Equals(input.F_BillType))
                .WhereIF(!string.IsNullOrEmpty(input.F_BillState), a => a.BillState.Equals(input.F_BillState))
                .WhereIF(!string.IsNullOrEmpty(input.F_EnabledMark), a => a.EnabledMark.Equals(input.F_EnabledMark))
                .Select((a
) => new ZjnPlaneBillsListOutput
{
    F_Id = a.Id,
    F_BillNo = a.BillNo,
    F_BillType = a.BillType,
    F_BillState = a.BillState,
    F_EnabledMark = a.EnabledMark,
}).OrderBy(sidx + " " + input.sort).ToListAsync();
            return data;
        }

        /// <summary>
		/// 导出出入库单据
		/// </summary>
		/// <param name="input">请求参数</param>
		/// <returns></returns>
        [HttpGet("Actions/Export")]
        public async Task<dynamic> Export([FromQuery] ZjnPlaneBillsListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var exportData = new List<ZjnPlaneBillsListOutput>();
            if (input.dataType == 0)
            {
                var data = Clay.Object(await this.GetList(input));
                exportData = data.Solidify<PageResult<ZjnPlaneBillsListOutput>>().list;
            }
            else
            {
                exportData = await this.GetNoPagingList(input);
            }
            List<ParamsModel> paramList = "[{\"value\":\"单据号码\",\"field\":\"billNo\"},{\"value\":\"单据状态\",\"field\":\"billState\"},{\"value\":\"有效标志\",\"field\":\"enabledMark\"},{\"value\":\"单据类型\",\"field\":\"billType\"},]".ToList<ParamsModel>();
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = "出入库单据.xls";
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
            ExcelExportHelper<ZjnPlaneBillsListOutput>.Export(exportData, excelconfig, addPath);
            var fileName = _userManager.UserId + "|" + addPath + "|xls";
            var output = new
            {
                name = excelconfig.FileName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "HSZ")
            };
            return output;
        }

        /// <summary>
        /// 批量删除出入库单据
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost("batchRemove")]
        public async Task BatchRemove([FromBody] List<string> ids)
        {
            var entitys = await _zjnPlaneBillsRepository.AsQueryable().In(it => it.Id, ids).ToListAsync();
            if (entitys.Count > 0)
            {
                try
                {
                    //开启事务
                    _db.BeginTran();
                    //批量删除出入库单据
                    await _zjnPlaneBillsRepository.AsDeleteable().In(d => d.Id, ids).ExecuteCommandAsync();

                    //清空子表数据
                    await _zjnPlaneBillsInOutOrderRepository.AsDeleteable().In(u => u.ParentId, entitys.Select(s => s.Id).ToArray()).ExecuteCommandAsync();
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
        /// 更新出入库单据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ZjnPlaneBillsUpInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var entity = input.Adapt<ZjnPlaneBillsEntity>();
            var entity1 = input.zjnPlaneBillsInOutOrderList.Adapt<List<ZjnPlaneBillsInOutOrderEntity>>();
            try
            {
                //开启事务
                _db.BeginTran();

                entity.LastModifyTime = DateTime.Now;
                entity.LastModifyUserId = userInfo.userId;
                await _zjnPlaneBillsRepository.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();

                if (entity1 != null)
                {
                    foreach (var item in entity1)
                    {
                        item.ParentId = entity.Id;
                        item.LastModifyUserId = userInfo.userId;
                        item.LastModifyTime = DateTime.Now;
                    }
                    await _zjnPlaneBillsInOutOrderRepository.AsUpdateable(entity1).IgnoreColumns(it => new { it.CreateTime, it.CreateUser }).ExecuteCommandAsync();
                }

                //关闭事务
                _db.CommitTran();
            }
            catch (Exception e)
            {
                var ss = e.Message;
                //回滚事务
                _db.RollbackTran();
                throw HSZException.Oh(ErrorCode.COM1001);
            }
        }

        /// <summary>
        /// 删除出入库单据
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _zjnPlaneBillsRepository.GetFirstAsync(p => p.Id.Equals(id));
            _ = entity ?? throw HSZException.Oh(ErrorCode.COM1005);
            try
            {
                //开启事务
                _db.BeginTran();

                await _zjnPlaneBillsRepository.AsDeleteable().Where(it => it.Id == id).ExecuteCommandAsync();

                await _zjnPlaneBillsInOutOrderRepository.AsDeleteable().Where(it => it.ParentId.Equals(entity.Id)).ExecuteCommandAsync();

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
    }
}


