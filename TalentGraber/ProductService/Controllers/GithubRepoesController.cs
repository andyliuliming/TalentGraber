﻿using Macrodeek.HappyZLServiceBase;
using Macrodeek.Model;
using Macrodeek.StarDustServiceBase;
using Macrodeek.StarDustServiceBase.Controllers;
using Microsoft.OData.Core;
using StarDustCommon.Util;
using StarDustCommon.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

namespace Macrodeek.StarDustProductService.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Macrodeek.StarDustModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<AzureImage>("GithubRepoes");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class GithubRepoesController : GoldODataController
    {
        // GET: odata/GithubRepoes
        // the format of the query could be 
        // odata/GithubRepoes?$FileName=a.jpg&$Tag=small1
        // odata/GithubRepoes?$FileName=b.jpg&$Tag=watermark1
        [MacroDeekAuthorize]
        public async Task<IHttpActionResult> GetGithubRepoes(ODataQueryOptions<GithubRepo> queryOptions)
        {
            return Ok<IEnumerable<GithubRepo>>((IQueryable<GithubRepo>)queryOptions.ApplyTo(db.GithubRepoes));
        }

        [MacroDeekAuthorize]
        public IHttpActionResult Post(GithubRepo githubRepo)
        {
            string urlToAdd = githubRepo.Url.Trim();
            GithubRepo repo = db.GithubRepoes.Where(gr => gr.Url == urlToAdd).FirstOrDefault();
            if (repo == null)
            {
                db.GithubRepoes.Add(githubRepo);
                db.SaveChanges();
                return Created(githubRepo);
            }
            else
            {
                return Conflict();
            }
        }
    }
}