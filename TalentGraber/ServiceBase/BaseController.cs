
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

namespace Macrodeek.StarDustServiceBase.Controllers
{
    public class ODataBaseController : ODataController
    {
        public bool AllAndCheck(BinaryOperatorNode binaryOperatorNode)
        {
            if (binaryOperatorNode.OperatorKind == BinaryOperatorKind.Or)
            {
                return false;
            }
            else
            {
                bool leftIsBinaryNode = binaryOperatorNode.Left is BinaryOperatorNode;
                bool leftIsSinglePropertyNode = binaryOperatorNode.Left is SingleValuePropertyAccessNode;

                bool rightIsBinaryNode = binaryOperatorNode.Right is BinaryOperatorNode;
                bool rightIsConstantNode = binaryOperatorNode.Right is ConstantNode;

                if (leftIsBinaryNode && rightIsBinaryNode)
                {
                    return AllAndCheck(binaryOperatorNode.Left as BinaryOperatorNode) && AllAndCheck(binaryOperatorNode.Right as BinaryOperatorNode);
                }
                if (leftIsSinglePropertyNode && rightIsConstantNode)
                {
                    return true;
                }
                throw new Exception("$filter format not right.");
            }
        }

        public bool PropertyNameExists(BinaryOperatorNode binaryOperatorNode, string propertyName)
        {
            bool leftIsBinaryNode = binaryOperatorNode.Left is BinaryOperatorNode;
            bool leftIsSinglePropertyNode = binaryOperatorNode.Left is SingleValuePropertyAccessNode;

            bool rightIsBinaryNode = binaryOperatorNode.Right is BinaryOperatorNode;
            bool rightIsConstantNode = binaryOperatorNode.Right is ConstantNode;
            if (leftIsBinaryNode && rightIsBinaryNode)
            {
                return PropertyNameExists(binaryOperatorNode.Left as BinaryOperatorNode, propertyName)
                    || PropertyNameExists(binaryOperatorNode.Right as BinaryOperatorNode, propertyName);
            }
            if (leftIsSinglePropertyNode && rightIsConstantNode)
            {
                if (string.Equals((binaryOperatorNode.Left as SingleValuePropertyAccessNode).Property.Name, propertyName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            throw new Exception("$filter format not right");
        }

        /// <summary>
        /// currently we only support the filter like this $filter=(Id eq 1), for the embedded, not support yet.
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool SingleEqualRequired(ODataQueryOptions queryOptions, string propertyName)
        {
            // check we must use the shop id to filter.
            // TODO: improve this, we should check the complex filter, say $filter=((Id eq 1) and (PropertyName1 eq 22ll))
            if (queryOptions != null && queryOptions.Filter != null)
            {
                BinaryOperatorNode binaryOperator = queryOptions.Filter.FilterClause.Expression as BinaryOperatorNode;
                if (binaryOperator != null)
                {
                    if (AllAndCheck(binaryOperator))
                    {
                        return PropertyNameExists(binaryOperator, propertyName);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetFilterValue(ODataQueryOptions queryOptions, string propertyName)
        {
            // TODO: improve this, we should check the complex filter, say $filter=((Id eq 1) and (PropertyName1 eq 22ll))
            if (queryOptions.Filter != null && queryOptions.Filter.FilterClause != null)
            {
                var binaryOperator = queryOptions.Filter.FilterClause.Expression as BinaryOperatorNode;
                if (binaryOperator != null)
                {
                    var property = binaryOperator.Left as SingleValuePropertyAccessNode;
                    var constant = binaryOperator.Right as ConstantNode;

                    if (property != null && property.Property != null && constant != null && constant.Value != null)
                    {
                        if (property.Property.Name == propertyName)
                        {
                            return constant.Value;
                        }
                    }
                }
            }
            return null;
        }
    }
}