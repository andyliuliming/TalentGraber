using System.Web.Http.ExceptionHandling;
using System.Diagnostics;
namespace Macrodeek.StarDustServiceBase.Traces
{
    public class TraceExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            Trace.TraceError(context.ExceptionContext.Exception.ToString());
        }
    }
}
