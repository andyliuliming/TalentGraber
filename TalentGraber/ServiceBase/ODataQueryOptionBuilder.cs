using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;

namespace Macrodeek.StarDustServiceBase
{
    public class ODataQueryOptionBuilder
    {
        public static ODataQueryOptions<T> BuildIt<T>(IEdmModel iedmModel, HttpRequestMessage message)
        {
            EntitySetPathSegment segment = new EntitySetPathSegment(typeof(T).Name);
            ODataPath path = new ODataPath(new ODataPathSegment[] { segment });
            ODataQueryContext oDataQueryContext = new ODataQueryContext(iedmModel, typeof(T), path);
            ODataQueryOptions<T> opts = new ODataQueryOptions<T>(oDataQueryContext, message);
            return opts;
        }
    }
}
