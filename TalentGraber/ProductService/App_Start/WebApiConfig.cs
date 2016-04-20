using StarDustCommon.Util;
using Macrodeek.AzureEnvironment;
using Macrodeek.StarDustServiceBase.Traces;
using Macrodeek.MacrodeekCommon;
using Microsoft.OData.Edm;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Macrodeek.Model;
using Macrodeek.Model.WrapUp;

namespace Macrodeek.StarDustProductService
{
    public static class WebApiConfig
    {
        //TODO save this entity set, so we can rebuild it to support the complex filter.
        // and we need to do a wrapper for the builder/iEdmModel
        public static ODataConventionModelBuilder builder = null;
        public static IEdmModel iEdmModel = null;

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //string allowedDomains = AppSettingsProvider.GetSetting(FieldNameUtil.GetMemberName((AzureSettingsNames c) => c.AllowedCrossDomainHost));
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter("Bearer"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            builder = new ODataConventionModelBuilder();
            builder.EntitySet<GithubRepo>("GithubRepoes");
            builder.EntitySet<TalentCandidate>("TalentCandidates");
            builder.EntitySet<UserToken>("UserTokens");
            builder.EntitySet<ContributerToRepo>("ContributerToRepoes");
            builder.EntitySet<Warmer>("Warmers");
            // External Notify Backs
            iEdmModel = builder.GetEdmModel();
            config.MapODataServiceRoute("odata", "odata", iEdmModel);

            config.AddODataQueryFilter();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //TRACE 
            config.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());
        }
    }
}
