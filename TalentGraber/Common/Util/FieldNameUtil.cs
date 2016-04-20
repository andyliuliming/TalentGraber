using System;
using System.Linq.Expressions;

namespace StarDustCommon.Util
{
    public class FieldNameUtil
    {
        public static string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess)
        {
            return ((MemberExpression)memberAccess.Body).Member.Name;
        }
    }
}
