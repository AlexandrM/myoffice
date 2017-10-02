using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web;
using System.Data.Entity.SqlServer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASE.EF
{
    public static class ExtensionsEF
    {
        public static IOrderedQueryable<T> OrderBySortOrder<T>(this IQueryable<T> source, string sortOrder)
        {
            return OrderBy(source, sortOrder.Replace("_asc", "").Replace("_desc", ""), sortOrder.IndexOf("asc") != -1);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property, bool ascendancy)
        {
            if (ascendancy)
                return ApplyOrder(source, property, "OrderBy");

            return ApplyOrder(source, property, "OrderByDescending");
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenByDescending");
        }

        #region Temp

        public static IOrderedEnumerable<T> OrderBySortOrder<T>(this IEnumerable<T> source, string sortOrder)
        {
            return OrderBy(source, sortOrder.Replace("_asc", "").Replace("_desc", ""), sortOrder.IndexOf("asc") != -1);
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string property, bool ascendancy)
        {
            if (ascendancy)
                return ApplyOrder(source.AsQueryable(), property, "OrderBy").AsEnumerable().OrderBy(x => 0);

            return ApplyOrder(source.AsQueryable(), property, "OrderByDescending").AsEnumerable().OrderBy(x => 0);
        }

        #endregion

        public static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                PropertyInfo pi = type.GetProperty(prop);
                if (pi == null)
                    pi = type.GetProperties().FirstOrDefault(x => x.Name.ToLower() == prop.ToLower());
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }

        public static MemberExpression CreateNestedParameterExpression(Expression parameter, MemberExpression memberExpression)
        {
            if (memberExpression.Expression is MemberExpression)
                parameter = CreateNestedParameterExpression(parameter, (MemberExpression)memberExpression.Expression);

            return Expression.Property(parameter, (PropertyInfo)memberExpression.Member);
        }

        public static IQueryable<T> ApplyFilterPatern<T>(this IQueryable<T> source, string searchString, params Expression<Func<T, object>>[] properties)
        {
            if (String.IsNullOrEmpty(searchString))
                return source;

            var parameter = Expression.Parameter(typeof(T), "x");

            Expression result = null;
            foreach (var item in properties)
            {
                var lambda = item as LambdaExpression;
                MemberExpression memberExpression;
                if (lambda.Body is UnaryExpression)
                {
                    var unaryExpression = (UnaryExpression)lambda.Body;
                    memberExpression = (MemberExpression)unaryExpression.Operand;
                }
                else
                {
                    memberExpression = (MemberExpression)lambda.Body;
                }

                memberExpression = CreateNestedParameterExpression(parameter, memberExpression);

                Expression expr;
                if ((memberExpression.Type == typeof(DateTime)) | memberExpression.Type == typeof(Nullable<DateTime>))
                {
                    var fDatePart = ((Func<string, DateTime?, int?>)SqlFunctions.DatePart).Method;
                    var fRight = ((Func<string, long?, string>)DbFunctions.Right).Method;
                    var fConcat = ((Func<string[], string>)String.Concat).Method;

                    Expression exprY = Expression.Call(fDatePart, Expression.Constant("yyyy"), Expression.Convert(memberExpression, typeof(DateTime?)));
                    if (exprY.Type.IsGenericType && exprY.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        exprY = Expression.Property(exprY, "Value");
                    exprY = Expression.Call(exprY, "ToString", null, null);

                    Expression exprM = Expression.Call(fDatePart, Expression.Constant("m"), Expression.Convert(memberExpression, typeof(Nullable<DateTime>)));
                    if (exprM.Type.IsGenericType && exprM.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        exprM = Expression.Property(exprM, "Value");
                    exprM = Expression.Call(exprM, "ToString", null, null);
                    exprM = Expression.Call(fConcat, Expression.NewArrayInit(typeof(String), Expression.Constant("0"), exprM));
                    exprM = Expression.Call(fRight, exprM, Expression.Convert(Expression.Constant(2), typeof(Nullable<Int64>)));

                    Expression exprD = Expression.Call(fDatePart, Expression.Constant("d"), Expression.Convert(memberExpression, typeof(Nullable<DateTime>)));
                    if (exprD.Type.IsGenericType && exprD.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        exprD = Expression.Property(exprD, "Value");
                    exprD = Expression.Call(exprD, "ToString", null, null);
                    exprD = Expression.Call(fConcat, Expression.NewArrayInit(typeof(String), Expression.Constant("0"), exprD));
                    exprD = Expression.Call(fRight, exprD, Expression.Convert(Expression.Constant(2), typeof(Nullable<Int64>)));

                    var arrayExpr = Expression.NewArrayInit(typeof(String),
                        exprD
                        , Expression.Constant(".")
                        , exprM
                        , Expression.Constant(".")
                        , exprY
                        );

                    expr = Expression.Call(fConcat, arrayExpr);

                    expr = Expression.Call(expr, "ToLower", null, null);
                    expr = Expression.Call(expr, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(searchString));
                }
                else if (memberExpression.Type != typeof(string))
                {
                    if (memberExpression.Type.IsGenericType && memberExpression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        Expression exprFalse = Expression.Constant(false);

                        Expression exprTrue;
                        exprTrue = Expression.Property(memberExpression, "Value");
                        exprTrue = Expression.Call(exprTrue, "ToString", null, null);
                        exprTrue = Expression.Call(exprTrue, "ToLower", null, null);
                        exprTrue = Expression.Call(exprTrue, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(searchString));

                        expr = Expression.Condition(
                            Expression.Property(memberExpression, "HasValue"),
                            exprTrue,
                            exprFalse
                            );
                    }
                    else
                    {
                        expr = Expression.Call(memberExpression, "ToString", null, null);
                        expr = Expression.Call(expr, "ToLower", null, null);
                        expr = Expression.Call(expr, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(searchString));
                    }
                }
                else
                {
                    expr = Expression.Call(memberExpression, "ToLower", null, null);
                    expr = Expression.Call(expr, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(searchString));
                }

                if (result == null)
                    result = expr;
                else
                    result = Expression.OrElse(result, expr);
            }

            MethodInfo mWhere =
            (
                from mi in typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                where mi.Name == "Where"
                let paramInfos = mi.GetParameters()
                where paramInfos.Length == 2
                where (paramInfos[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                let expressionGenericArgs = paramInfos[1].ParameterType.GetGenericArguments()
                where expressionGenericArgs.Length == 1
                where expressionGenericArgs[0].GetGenericTypeDefinition() == typeof(Func<,>)
                select mi
            )
            .Single();

            mWhere = mWhere.MakeGenericMethod(typeof(T));

            var r = Expression.Lambda<Func<T, bool>>(result, parameter);

            return (IQueryable<T>)mWhere.Invoke(null, new object[] { source, r });
        }

        public static void AttachModel<TEntity>(this DbContext context, TEntity entity, HttpRequestBase request) where TEntity : class
        {
            context.Entry(entity).State = EntityState.Modified;
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach(var prop in props)
                if (!request.Params.AllKeys.Contains(prop.Name))
                    if (context.Entry(entity).Member(prop.Name) is DbPropertyEntry && !(prop.GetCustomAttributes(typeof(NotMappedAttribute),false).Length > 0))
                        context.Entry(entity).Property(prop.Name).IsModified = false;
        }

        public static TEntity AttachModel<TEntity>(this DbContext context, TEntity entity, params Expression<Func<TEntity, object>>[] properties) where TEntity : class
        {
            var key = entity.GetType().GetProperties().FirstOrDefault(p => p.CustomAttributes.Any(attr => attr.AttributeType == typeof(KeyAttribute)));
            var dbModel = context.Set<TEntity>().Find(entity.GetType().GetProperty(key.Name).GetValue(entity));

            PropertyInfo[] props = entity.GetType().GetProperties();
            for (int i = 0; i <properties.Length; i++)
            {
                var lambda = properties[i] as LambdaExpression;
                MemberExpression memberExpression;

                if (lambda.Body is UnaryExpression)
                {
                    var unaryExpression = (UnaryExpression)lambda.Body;
                    memberExpression = (MemberExpression)unaryExpression.Operand;
                }
                else
                {
                    memberExpression = (MemberExpression)lambda.Body;
                }
                dbModel.GetType().GetProperty(memberExpression.Member.Name).SetValue(dbModel, entity.GetType().GetProperty(memberExpression.Member.Name).GetValue(entity));
                context.Entry(dbModel).Property(memberExpression.Member.Name).IsModified = true;
            }
            return dbModel;
        }

        public static string RealEFType(this object obj)
        {
            if (obj == null)
                return "";

            if (obj.GetType().BaseType == typeof(object))
                return obj.GetType().FullName;
            else
                return obj.GetType().BaseType.FullName;
        }
    }
}