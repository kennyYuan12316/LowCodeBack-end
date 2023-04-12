using HSZ.ChangeDataBase;
using HSZ.Common.Configuration;
using HSZ.Common.Core.Manager;
using HSZ.Common.Enum;
using HSZ.Common.Extension;
using HSZ.Common.Filter;
using HSZ.Common.Helper;
using HSZ.Dependency;
using HSZ.DynamicApiController;
using HSZ.FriendlyException;
using HSZ.JsonSerialization;
using HSZ.System.Entitys.Dto.System.Database;
using HSZ.System.Entitys.Model.System.DataBase;
using HSZ.System.Entitys.System;
using HSZ.System.Interfaces.Common;
using HSZ.System.Interfaces.Permission;
using HSZ.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.System.Core.Service.DataBase
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：数据管理
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataModel", Order = 208)]
    [Route("api/system/[controller]")]
    public class DataBaseService : IDataBaseService, IDynamicApiController, ITransient
    {
        private readonly SqlSugarScope Db;
        private readonly IUserManager _userManager;
        private readonly IDbLinkService _dbLinkService;
        private readonly IAuthorizeService _authorizeService;
        private readonly IFileService _fileService;
        private readonly IChangeDataBase _changeDataBase;

        /// <summary>
        ///
        /// </summary>
        public DataBaseService(IChangeDataBase changeDataBase, IDbLinkService dbLinkService,
            IUserManager userManager, IAuthorizeService authorizeService,
            IFileService fileService)
        {
            _changeDataBase = changeDataBase;
            _dbLinkService = dbLinkService;
            _userManager = userManager;
            _authorizeService = authorizeService;
            _fileService = fileService;
            Db = DbScoped.SugarScope;
        }

        #region GET
        /// <summary>
        /// 表名列表
        /// </summary>
        /// <param name="id">连接Id</param>
        /// <param name="input">过滤条件</param>
        /// <returns></returns>
        [HttpGet("{id}/Tables")]
        public async Task<dynamic> GetList_Api(string id, [FromQuery] KeywordInput input)
        {
            try
            {
                var link = (await _dbLinkService.GetInfo(id));
                if (link == null)
                {
                    link = GetTenantDbLink();
                }
                var tables = _changeDataBase.GetTableInfos(link);
                tables = tables.Where((x, i) => tables.FindIndex(z => z.Name == x.Name) == i).ToList();
                var output = tables.Adapt<List<DatabaseTableListOutput>>();
                if (!string.IsNullOrEmpty(input.keyword))
                    output = output.FindAll(d => d.table.ToLower().Contains(input.keyword.ToLower()) || (d.tableName.IsNotEmptyOrNull() && d.tableName.ToLower().Contains(input.keyword.ToLower())));
                GetTableCount(output);
                return new { list = output.OrderBy(x => x.table).ToList() };
            }
            catch (Exception ex)
            {
                var data = new List<DatabaseTableListOutput>();
                return new { list = data };
            }
        }

        /// <summary>
        /// 预览数据
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="DBId">连接Id</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        [HttpGet("{DBId}/Table/{tableName}/Preview")]
        public async Task<dynamic> GetData_Api([FromQuery] DatabaseTablePreviewQuery input, string DBId, string tableName)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            if (string.IsNullOrEmpty(tableName))
                return new PageResult();
            if (link == null)
            {
                link = GetTenantDbLink();
            }
            StringBuilder dbSql = new StringBuilder();
            dbSql.AppendFormat("SELECT * FROM {0} WHERE 1=1", tableName);
            if (!string.IsNullOrEmpty(input.field) && !string.IsNullOrEmpty(input.keyword))
                dbSql.AppendFormat(" AND {0} like '%{1}%'", input.field, input.keyword);

            return await _changeDataBase.GetDataTablePage(link, dbSql.ToString(), input.currentPage, input.pageSize);
        }

        /// <summary>
        /// 字段列表
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        [HttpGet("{DBId}/Tables/{tableName}/Fields")]
        public async Task<dynamic> GetFieldList_Api(string DBId, string tableName,[FromQuery]string type)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            if (link == null)
            {
                link = GetTenantDbLink();
            }
            var data = _changeDataBase.GetFieldList(link, tableName).Adapt<List<DatabaseTableFieldsListOutput>>();
            if (type.Equals("1"))
            {
                foreach (var item in data)
                {
                    var field = item.field.Replace("F_", "").Replace("f_", "").ToPascalCase();
                    item.field = field.Substring(0, 1).ToLower() + field[1..];
                }
            }
            return new { list = data };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="tableName">主键值</param>
        /// <returns></returns>
        [HttpGet("{DBId}/Table/{tableName}")]
        public async Task<dynamic> GetInfo_Api(string DBId, string tableName)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            if (link == null)
            {
                link = GetTenantDbLink();
            }
            var data = _changeDataBase.GetDataBaseTableInfo(link, tableName);
            return data;
        }

        /// <summary>
        /// 获取数据库表字段下拉框列表
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        [HttpGet("{DBId}/Tables/{tableName}/Fields/Selector")]
        public async Task<dynamic> SelectorData_Api(string DBId, string tableName)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            if (link == null)
            {
                link = GetTenantDbLink();
            }
            var data = _changeDataBase.GetDBTableList(link).FindAll(m => m.table == tableName).Adapt<List<DatabaseTableFieldsSelectorOutput>>();
            return new { list = data };
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="linkId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{linkId}/Table/{id}/Action/Export")]
        public async Task<dynamic> ActionsExport(string linkId, string id)
        {
            var link = await _dbLinkService.GetInfo(linkId);
            if (link == null)
            {
                link = GetTenantDbLink();
            }
            var data = _changeDataBase.GetDataBaseTableInfo(link, id);
            var jsonStr = data.Serialize();
            return _fileService.Export(jsonStr, data.tableInfo.table);
        }
        #endregion

        #region POST
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        [HttpDelete("{DBId}/Table/{tableName}")]
        public async Task Delete_Api(string DBId, string tableName)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            if (link == null)
            {
                link = GetTenantDbLink();
            }
            var data = _changeDataBase.GetData(link, tableName);
            if (data.Rows.Count > 0)
                throw HSZException.Oh(ErrorCode.D1508);
            if (IsSysTable(tableName))
                throw HSZException.Oh(ErrorCode.D1504);
            var isOk = _changeDataBase.Delete(link, tableName);
            if (!isOk)
                throw HSZException.Oh(ErrorCode.D1500);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("{DBId}/Table")]
        public async Task Create_Api(string DBId, [FromBody] DatabaseTableCrInput input)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            if (link == null)
            {
                link = GetTenantDbLink();
            }
            if (_changeDataBase.IsAnyTable(link, input.tableInfo.newTable))
                throw HSZException.Oh(ErrorCode.D1503);
            var tableInfo = input.tableInfo.Adapt<DbTableModel>();
            tableInfo.table = input.tableInfo.newTable;
            var tableFieldList = input.tableFieldList.Adapt<List<DbTableFieldModel>>();
            var isOk = await _changeDataBase.Create(link, tableInfo, tableFieldList);
            if (!isOk)
                throw HSZException.Oh(ErrorCode.D1501);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPut("{DBId}/Table")]
        public async Task Update_Api(string DBId, [FromBody] DatabaseTableUpInput input)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            if (link == null)
            {
                link = GetTenantDbLink();
            }
            var oldFieldList = _changeDataBase.GetFieldList(link, input.tableInfo.table).Adapt<List<TableFieldList>>();
            oldFieldList = _changeDataBase.ViewDataTypeConversion(oldFieldList, (SqlSugar.DbType)Enum.Parse(typeof(SqlSugar.DbType), link.DbType));
            var oldTableInfo = _changeDataBase.GetTableInfos(link).Find(m => m.Name == input.tableInfo.table).Adapt<DbTableModel>();
            try
            {
                var data = _changeDataBase.GetData(link, input.tableInfo.table);
                if (data.Rows.Count > 0)
                    throw HSZException.Oh(ErrorCode.D1508);
                var tableInfo = input.tableInfo.Adapt<DbTableModel>();
                tableInfo.table = input.tableInfo.newTable;
                var tableFieldList = input.tableFieldList.Adapt<List<DbTableFieldModel>>();
                if (IsSysTable(tableInfo.table))
                    throw HSZException.Oh(ErrorCode.D1504);
                if (!input.tableInfo.table.Equals(input.tableInfo.newTable) && _changeDataBase.IsAnyTable(link, input.tableInfo.newTable))
                    throw HSZException.Oh(ErrorCode.D1503);
                var isOk = await _changeDataBase.Update(link, input.tableInfo.table, tableInfo, tableFieldList);
                if (!isOk)
                    throw HSZException.Oh(ErrorCode.D1502);
            }
            catch (Exception ex)
            {
                await _changeDataBase.Create(link, oldTableInfo, oldFieldList.Adapt<List<DbTableFieldModel>>());
                throw HSZException.Oh(ErrorCode.D1502);
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="linkid"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{linkid}/Action/Import")]
        public async Task ActionsImport(string linkid,IFormFile file)
        {
            var fileType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!fileType.ToLower().Equals("json"))
                throw HSZException.Oh(ErrorCode.D3006);
            var josn = _fileService.Import(file);
            var data = josn.Deserialize<DatabaseTableCrInput>();
            if (data == null||data.tableFieldList==null||data.tableInfo==null)
                throw HSZException.Oh(ErrorCode.D3006);
            data.tableInfo.newTable = data.tableInfo.table;
            await Create_Api(linkid, data);
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 获取多租户Link
        /// </summary>
        /// <returns></returns>
        private DbLinkEntity GetTenantDbLink()
        {
            return new DbLinkEntity
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

        /// <summary>
        /// 是否系统表
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private bool IsSysTable(string table)
        {
            string[] byoTable = "zjn_base_authorize,zjn_base_billrule,zjn_base_db_backup,zjn_base_db_link,zjn_base_dictionary_data,zjn_base_dictionary_type,zjn_base_im_content,base_languagemap,base_languagetype,base_menu,zjn_base_message,zjn_base_message_receive,zjn_base_module,zjn_base_module_button,zjn_base_module_column,zjn_base_module_authorize,zjn_base_module_scheme,zjn_base_organize,zjn_base_position,zjn_base_province,zjn_base_role,zjn_base_sys_config,zjn_base_sys_log,zjn_base_time_task,zjn_base_time_task_log,zjn_base_user,zjn_base_user_relation,crm_busines,crm_businesproduct,crm_clue,crm_contract,crm_contractinvoice,crm_contractmoney,crm_contractproduct,crm_customer,crm_customercontacts,crm_followlog,crm_invoice,crm_product,crm_receivable,zjn_ext_big_data,zjn_ext_document,zjn_ext_document_share,zjn_ext_email_config,zjn_ext_email_receive,zjn_ext_email_send,zjn_ext_employee,zjn_ext_order,zjn_ext_orderentry,zjn_ext_order_receivable,zjn_ext_project_gantt,zjn_ext_schedule,zjn_ext_table_example,zjn_ext_work_log,zjn_ext_worklog_share,zjn_flow_delegate,zjn_flow_engine,zjn_flow_engine_form,zjn_flow_engine_visible,zjn_flow_task,zjn_flow_task_circulate,zjn_flow_task_node,zjn_flow_task_operator,zjn_flow_task_operator_record,wechat_mpeventcontent,wechat_mpmaterial,wechat_mpmessage,wechat_qydepartment,wechat_qymessage,wechat_qyuser,zjn_wform_banquet,zjn_wform_deliver_goods,zjn_wform_deliver_goods_entry,zjn_wform_meeting,zjn_wform_archival_borrow,zjn_wform_article_warehous,zjn_wform_batch_pack,zjn_wform_batch_table,zjn_wform_contract_billing,zjn_wform_contract_approval,zjn_wform_contract_approval_sheet,zjn_wform_debit_bill,zjn_wform_document_approval,zjn_wform_document_signing,zjn_wform_expense_expenditure,zjn_wform_finished_product,zjn_wform_finished_product_entry,zjn_wform_incomere_cognition,zjn_wform_leave_apply,zjn_wform_letter_service,zjn_wform_material_requisition,zjn_wform_material_requisition_entry,zjn_wform_monthly_report,zjn_wform_office_supplies,zjn_wform_outbound_order,zjn_wform_outbound_order_entry,zjn_wform_outgoing_apply,zjn_wform_pay_distribution,zjn_wform_payment_apply,zjn_wform_post_batch_tab,zjn_wform_procurement_material,zjn_wform_procurement_material_entry,zjn_wform_purchase_list,zjn_wform_purchase_list_entry,zjn_wform_quotation_approval,zjn_wform_receipt_processing,zjn_wform_receipt_sign,zjn_wform_reward_punishment,zjn_wform_sale_order,zjn_wform_sale_order_entry,zjn_wform_sale_support,zjn_wform_staff_overtime,zjn_wform_supplement_card,zjn_wform_travel_apply,zjn_wform_travel_reimbursement,zjn_wform_vehicle_apply,zjn_wform_violation_handling,zjn_wform_warehouse_receipt,zjn_wform_warehouse_receipt_entry,zjn_wform_work_contact_sheet".Split(',');
            bool exists = ((IList)byoTable).Contains(table.ToLower());
            return exists;
        }

        /// <summary>
        /// 获取表条数
        /// </summary>
        /// <param name="tableList"></param>
        private void GetTableCount(List<DatabaseTableListOutput> tableList)
        {
            foreach (var item in tableList)
            {
                try
                {
                    item.sum = Db.Queryable<dynamic>().AS(item.table).Count();
                }
                catch (Exception ex)
                {
                    item.sum = 0;
                }
            }
        }
        #endregion
    }
}
