using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties.Utils
{
    internal static class PropertyHelper
    {
        internal class MemberAccessInfo
        {
            public readonly object Instance;
            public readonly string MemberName;

            public MemberAccessInfo(object instance, string memberName)
            {
                Instance = instance;
                MemberName = memberName;
            }
        }

        internal static string GetMemberName<T>(Expression<Func<T>> memberAccessExpression)
        {
            LambdaExpression lambda = (LambdaExpression)memberAccessExpression;

            if (lambda.Body.NodeType != ExpressionType.MemberAccess)
                throw new Exception("Expression must be a member access.");

            MemberExpression memberExpr = (MemberExpression)lambda.Body;

            return memberExpr.Member.Name;
        }

        internal static MemberAccessInfo GetMemberAccessInfo<T>(Expression<Func<T>> memberAccessExpression)
        {
            LambdaExpression lambda = (LambdaExpression)memberAccessExpression;

            if (lambda.Body.NodeType != ExpressionType.MemberAccess)
                throw new Exception("Expression must be a member access.");

            MemberExpression memberExpr = (MemberExpression)lambda.Body;

            var instance = Expression.Lambda(memberExpr.Expression).Compile().DynamicInvoke();

            return new MemberAccessInfo(instance, memberExpr.Member.Name);
        }
    }
}
