using Macrodeek.Model;
using System;
using System.Diagnostics;

namespace Macrodeek.StarDustServiceBase.Controllers
{
    public class GoldODataController : ODataBaseController
    {
        protected GoldModelContainer db = new GoldModelContainer();
        public GoldODataController()
            : base()
        {
            db.Database.Log = new Action<string>(str =>
            {
                Trace.TraceInformation(str);
            });
        }
    }
}