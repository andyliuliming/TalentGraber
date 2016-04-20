using Macrodeek.AzureEnvironment;
using Macrodeek.HappyZLModel;
using Macrodeek.HappyZLModel.WrapUp.SearchRelated;
using Macrodeek.MacrodeekCommon;
using Macrodeek.Model;
using Nest;
using Newtonsoft.Json.Linq;
using StarDustCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Macrodeek.HappyZLProductService.Controllers.SeachWrapper
{
    public class SearchController : ApiController
    {
        // POST: api/Search
        public IHttpActionResult Post([FromBody]SearchQueryBody body)
        {
            string searchAddress = AppSettingsProvider.GetSetting(FieldNameUtil.GetMemberName((AzureSettingsNames c) => c.SearchServiceAddress));
            var node = new Uri(searchAddress);
            var settings = new ConnectionSettings(node);
            var client = new ElasticClient(settings);

            switch (body.ObjectType)
            {
                case ObjectType.TalentCandidate:

                    SearchRequest<TalentCandidate> userRequest = null;
                    userRequest = new SearchRequest<TalentCandidate>();

                    userRequest.From = body.From;
                    userRequest.Size = body.Size;
                    if (!string.IsNullOrEmpty(body.FullText))
                    {
                        userRequest.Query = new MatchQuery { Field = "_all", Query = body.FullText, MinimumShouldMatch = body.MinimumShouldMatch };
                    }
                    else
                    {
                        //TODO implement this.

                    }
                    ISearchResponse<TalentCandidate> userResponse = client.Search<TalentCandidate>(userRequest);
                    return Json<IEnumerable<TalentCandidate>>(userResponse.Documents);

                default:
                    return null;
            }
        }
    }
}
