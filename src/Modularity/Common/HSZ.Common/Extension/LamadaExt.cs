using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HSZ.Common.Extension.Extensions
{
    public class LamadaExt<Dto>
    {
        private List<Expression> m_Expression = null;
        private ParameterExpression m_Params = null;

        public LamadaExt()
        {
            m_Expression = new List<Expression>();
            m_Params = Expression.Parameter(typeof(Dto), "list");
        }
        public void GetExpression(string strName, object strValue, ExpressionType expressType)
        {
            Expression expRes = null;
            MemberExpression member = Expression.PropertyOrField(m_Params, strName);
            if (expressType == ExpressionType.Equal)
                expRes = Expression.Equal(member, Expression.Constant(strValue, member.Type));
            else if (expressType == ExpressionType.NotEqual)
                expRes = Expression.NotEqual(member, Expression.Constant(strValue, member.Type));
            m_Expression.Add(expRes);
        }

        public void GetAndExpression(string strName, List<object> listValue)
        {
            Expression expRes = null;
            MemberExpression member = Expression.PropertyOrField(m_Params, strName);
            foreach (var oValue in listValue)
            {
                if (expRes == null)
                    expRes = Expression.Equal(member, Expression.Constant(oValue, member.Type));
                else
                    expRes = Expression.And(expRes, Expression.Equal(member, Expression.Constant(oValue, member.Type)));
            }
            m_Expression.Add(expRes);
        }

        public void GetOrExpression(string strName, List<object> listValue)
        {
            Expression expRes = null;
            MemberExpression member = Expression.PropertyOrField(m_Params, strName);
            foreach (var oValue in listValue)
            {
                if (expRes == null)
                    expRes = Expression.Equal(member, Expression.Constant(oValue, member.Type));
                else
                    expRes = Expression.Or(expRes, Expression.Equal(member, Expression.Constant(oValue, member.Type)));
            }
            m_Expression.Add(expRes);
        }

        public void GetAndExpression(List<string> listName)
        {
            Expression expRes = null;
            foreach (var item in listName)
            {
                MemberExpression member = Expression.PropertyOrField(m_Params, item);
                if (expRes == null)
                    expRes = Expression.Constant(member.IsEmpty());
                else
                    expRes = Expression.And(expRes, Expression.Constant(member.IsEmpty()));
            }
            m_Expression.Add(expRes);
        }

        public void GetOrExpression(List<string> listName)
        {
            Expression expRes = null;
            foreach (var item in listName)
            {
                MemberExpression member = Expression.PropertyOrField(m_Params, item);
                MethodInfo method = typeof(StringExt).GetMethod("IsEmpty");
                if (expRes == null)
                    expRes = Expression.Not(member, method);
                else
                    expRes = Expression.Or(expRes, Expression.Constant(member.IsEmpty()));
            }
            m_Expression.Add(expRes);
        }

        //得到Lamada表达式的Expression对象
        public Expression<Func<Dto, bool>> GetLambda()
        {
            Expression whereExpr = null;
            foreach (var expr in this.m_Expression)
            {
                if (whereExpr == null)
                    whereExpr = expr;
                else
                    whereExpr = Expression.And(whereExpr, expr);
            }
            if (whereExpr == null)
                return null;
            return Expression.Lambda<Func<Dto, Boolean>>(whereExpr, m_Params);
        }

    }
}
