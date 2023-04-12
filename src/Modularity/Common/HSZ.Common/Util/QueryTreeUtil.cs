using HSZ.Dependency;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace HSZ.Common.Util
{
    /// <summary>
    /// 版 本 zjn-Standard v1.0.0.0
    /// Copyright (c) 2003-2022 江西合力泰科技股份有限公司
    /// 创建人：合力泰-框架开发组
    /// 日 期：2022.05.05
    /// 描 述：树形结构查询
    /// </summary>
    [SuppressSniffer]
    public static class QueryTreeUtil
    {
        /// <summary>
        /// 递归查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据源</param>
        /// <param name="condition">查询条件</param>
        /// <param name="idSelector">主键</param>
        /// <param name="parentIdSelector">上级</param>
        /// <returns></returns>
        public static List<T> TreeWhere<T>(this List<T> data, Predicate<T> condition, Func<T, string> idSelector, Func<T, string> parentIdSelector)
        {
            List<T> locateList = data.FindAll(condition);
            List<T> treeList = new List<T>();
            foreach (T entity in locateList)
            {
                treeList.Add(entity);
                T currentNode = entity;
                while (true)
                {
                    string parentId = parentIdSelector(currentNode);
                    if (parentId == null)
                        break;
                    T upRecord = data.Find(a => idSelector(a) == parentId);
                    if (upRecord != null)
                    {
                        treeList.Add(upRecord);
                        currentNode = upRecord;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return treeList.Distinct().ToList();
        }
        /// <summary>
        /// 递归查询
        /// </summary>
        /// <param name="data">数据源</param>
        /// <param name="condition">查询条件</param>
        /// <param name="idSelector">主键</param>
        /// <param name="parentIdSelector">上级</param>
        /// <returns></returns>
        public static DataTable TreeWhere(this DataTable data, string condition, string idSelector = "F_Id", string parentIdSelector = "F_ParentId")
        {
            DataRow[] drs = data.Select(condition);
            DataTable treeTable = data.Clone();
            foreach (DataRow dr in drs)
            {
                treeTable.ImportRow(dr);
                string pId = dr[parentIdSelector].ToString();
                while (true)
                {
                    if (string.IsNullOrEmpty(pId) && pId == "0")
                    {
                        break;
                    }
                    DataRow[] pdrs = data.Select(idSelector + "='" + pId + "'");
                    if (pdrs.Length > 0)
                    {
                        treeTable.ImportRow(pdrs[0]);
                        pId = pdrs[0][parentIdSelector].ToString();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return treeTable.DefaultView.ToTable(true);
        }
        /// <summary>
        /// 获取全部子节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据源</param>
        /// <param name="idValue">主键值</param>
        /// <param name="idSelector">主键</param>
        /// <param name="parentIdSelector">上级</param>
        /// <returns></returns>
        public static List<T> TreeChildNode<T>(this List<T> data, string idValue, Func<T, string> idSelector, Func<T, string> parentIdSelector)
        {
            T thisEntity = data.Find(a => idSelector(a) == idValue);
            foreach (PropertyInfo prop in thisEntity.GetType().GetProperties())
            {
                if (prop.Name == "ParentId" || prop.Name == "parentId")
                {
                    prop.SetValue(thisEntity, "0", null);
                    break;
                }
            }
            List<T> treeList = new List<T>();
            treeList.Add(thisEntity);
            ChildNode(data, idValue, idSelector, parentIdSelector, ref treeList);
            return treeList;
        }

        #region Method
        private static void ChildNode<T>(this List<T> data, string idValue, Func<T, string> idSelector, Func<T, string> parentIdSelector, ref List<T> treeNodes)
        {
            List<T> locateList = data.FindAll(a => parentIdSelector(a) == idValue);
            if (locateList.Count > 0)
            {
                foreach (var item in locateList)
                {
                    treeNodes.Add(item);
                    ChildNode(data, idSelector(item), idSelector, parentIdSelector, ref treeNodes);
                }
            }
        }
        #endregion 
    }
}
