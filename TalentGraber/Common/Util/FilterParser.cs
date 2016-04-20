using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData;
using System.Web.OData.Query;

namespace StarDustCommon.Util
{
    public class FilterUtil
    {
        public static string Parse(FilterQueryOption queryOption, string target)
        {
            return Parse(queryOption.FilterClause.Expression, target);
        }

        private static string Parse(QueryNode expression, string target)
        {
            if (expression.Kind == QueryNodeKind.BinaryOperator)
            {
                var binaryOperatorExpression = (BinaryOperatorNode)expression;
                if (binaryOperatorExpression.OperatorKind == BinaryOperatorKind.Equal
                    && binaryOperatorExpression.Left.Kind == QueryNodeKind.SingleValuePropertyAccess
                    && binaryOperatorExpression.Right.Kind == QueryNodeKind.Constant)
                {
                    var singleValuePropertyAccessExpression = (SingleValuePropertyAccessNode)(binaryOperatorExpression.Left);
                    if (singleValuePropertyAccessExpression.Property.Name.Equals(target))
                    {
                        var constantExpression = (ConstantNode)(binaryOperatorExpression.Right);
                        return constantExpression.Value.ToString();
                    }
                }
                else if (binaryOperatorExpression.OperatorKind == BinaryOperatorKind.And)
                {
                    var leftResult = Parse(binaryOperatorExpression.Left, target);
                    if (string.IsNullOrEmpty(leftResult))
                    {
                        return Parse(binaryOperatorExpression.Right, target);
                    }
                    else
                    {
                        return leftResult;
                    }
                }
            }
            return string.Empty;
        }
    }
}
