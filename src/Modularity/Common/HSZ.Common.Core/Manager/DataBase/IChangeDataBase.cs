using HSZ.Common.Filter;
using HSZ.System.Entitys.Dto.System.Database;
using HSZ.System.Entitys.Model.System.DataBase;
using HSZ.System.Entitys.System;
using HSZ.VisualDev.Entitys.Dto.VisualDevModelData;
using HSZ.VisualDev.Entitys.Model.VisualDevModelData;
using SqlSugar;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HSZ.ChangeDataBase
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：切换数据库抽象
    /// </summary>
    public interface IChangeDataBase
    {
        /// <summary>
        /// 根据链接获取分页数据
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="strSql">Sql语句</param>
        /// <param name="pageInput">页数</param>
        /// <param name="columnDesign">列配置</param>
        /// <param name="dataPermissions">数据权限</param>
        /// <returns></returns>
        PageResult<Dictionary<string, object>> GetInterFaceData(DbLinkEntity link, string strSql, VisualDevModelListQueryInput pageInput, ColumnDesignModel columnDesign, List<IConditionalModel> dataPermissions, Dictionary<string, string> outColumnName = null);

        /// <summary>
        /// 执行Sql(查询)
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        Task<int> ExecuteSql(DbLinkEntity link, string strSql);

        /// <summary>
        /// 执行Sql(新增、修改)
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <param name="dicList">数据</param>
        /// <param name="primaryField">主键字段</param>
        /// <returns></returns>
        Task<int> ExecuteSql(DbLinkEntity link, string table, List<Dictionary<string, object>> dicList, string primaryField = "");

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        bool Delete(DbLinkEntity link, string table);

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="tableModel">表对象</param>
        /// <param name="tableFieldList">字段对象</param>
        /// <returns></returns>
        Task<bool> Create(DbLinkEntity link, DbTableModel tableModel, List<DbTableFieldModel> tableFieldList);

        /// <summary>
        /// sqlsugar添加表字段
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableFieldList">表字段集合</param>
        /// <returns></returns>
        void AddTableColumn(string tableName, List<DbTableFieldModel> tableFieldList);

        /// <summary>
        /// 修改表
        /// </summary>
        /// <param name="link"></param>
        /// <param name="oldTable"></param>
        /// <param name="tableModel"></param>
        /// <param name="tableFieldList"></param>
        /// <returns></returns>
        Task<bool> Update(DbLinkEntity link, string oldTable, DbTableModel tableModel, List<DbTableFieldModel> tableFieldList);

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        bool IsAnyTable(DbLinkEntity link, string table);

        /// <summary>
        /// 获取表字段列表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        List<DbTableFieldModel> GetFieldList(DbLinkEntity link, string tableName);

        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        DataTable GetData(DbLinkEntity link, string tableName);

        /// <summary>
        /// 根据链接获取数据
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="strSql">sqlyuju </param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        DataTable GetInterFaceData(DbLinkEntity link, string strSql, params SugarParameter[] parameters);

        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        DatabaseTableInfoOutput GetDataBaseTableInfo(DbLinkEntity link, string tableName);

        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <returns></returns>
        List<DbTableModel> GetDBTableList(DbLinkEntity link);

        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <returns></returns>
        List<DbTableInfo> GetTableInfos(DbLinkEntity link);

        /// <summary>
        /// 获取数据表分页(SQL语句)
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="dbSql">数据SQL</param>
        /// <param name="pageIndex">页数</param>
        /// <param name="pageSize">条数</param>
        /// <returns></returns>
        Task<dynamic> GetDataTablePage(DbLinkEntity link, string dbSql, int pageIndex, int pageSize);

        /// <summary>
        /// 获取数据表分页(实体)
        /// </summary>
        /// <typeparam name="TEntity">T</typeparam>
        /// <param name="link">数据连接</param>
        /// <param name="pageIndex">页数</param>
        /// <param name="pageSize">条数</param>
        /// <returns></returns>
        Task<List<TEntity>> GetDataTablePage<TEntity>(DbLinkEntity link, int pageIndex, int pageSize);

        /// <summary>
        /// 使用存储过程
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="stored">存储过程名称</param>
        /// <param name="parameters">参数</param>
        void UseStoredProcedure(DbLinkEntity link, string stored, List<SugarParameter> parameters);

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <returns></returns>
        bool IsConnection(DbLinkEntity link);

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="dt">同步数据</param>
        /// <param name="table">表</param>
        /// <returns></returns>
        bool SyncData(DbLinkEntity link, DataTable dt, string table);

        /// <summary>
        /// 同步表操作
        /// </summary>
        /// <param name="linkFrom">原数据库</param>
        /// <param name="linkTo">目前数据库</param>
        /// <param name="table">表名称</param>
        /// <param name="type"></param>
        void SyncTable(DbLinkEntity linkFrom, DbLinkEntity linkTo, string table, int type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="databaseType"></param>
        List<TableFieldList> ViewDataTypeConversion(List<TableFieldList> fields, SqlSugar.DbType databaseType);


        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        SqlSugar.DbType ToDbType(string dbType);

        /// <summary>
        /// 转换连接字符串
        /// </summary>
        /// <param name="dbLinkEntity"></param>
        /// <returns></returns>
        string ToConnectionString(DbLinkEntity dbLinkEntity);

        /// <summary>
        /// 执行增删改sql
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="strSql">sqlyuju </param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        void ExecuteCommand(DbLinkEntity link, string strSql, params SugarParameter[] parameters);

        bool WhereDynamicFilter(DbLinkEntity link, string strSql);
    }
}