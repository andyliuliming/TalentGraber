using Macrodeek.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Controllers;

namespace Macrodeek.HappyZLServiceBase
{
    public static class AuthorizeUtil
    {
        public const string UserNameHeaderName = "UserName";
        public const string TokenHeaderName = "Token";
        public const string CurrentPasswordHeaderName = "CurrentPassword";

        public static long CurrentUserName(HttpActionContext actionContext)
        {
            if (actionContext != null && actionContext.Request != null && actionContext.Request.Headers != null)
            {
                IEnumerable<string> userNames;
                if (actionContext.Request.Headers.TryGetValues(AuthorizeUtil.UserNameHeaderName, out userNames))
                {
                    return long.Parse(userNames.FirstOrDefault());
                }
            }
            throw new UnauthorizedAccessException();
        }

        public static string CurrentPassword(HttpActionContext actionContext)
        {
            if (actionContext != null && actionContext.Request != null && actionContext.Request.Headers != null)
            {
                IEnumerable<string> newPasswords;
                if (actionContext.Request.Headers.TryGetValues(AuthorizeUtil.CurrentPasswordHeaderName, out newPasswords))
                {
                    return newPasswords.FirstOrDefault();
                }
            }
            return null;
        }

        public static bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext != null && actionContext.Request != null && actionContext.Request.Headers != null)
            {
                IEnumerable<string> tokens = null;
                IEnumerable<string> userIds = null;
                if (actionContext.Request.Headers.TryGetValues(TokenHeaderName, out tokens)
                    && actionContext.Request.Headers.TryGetValues(UserNameHeaderName, out userIds))
                {
                    // check the timeout.
                    string userIdStr = userIds.FirstOrDefault();

                    string tokenToVerify = tokens.FirstOrDefault();
                    GoldModelContainer db = new GoldModelContainer();

                    UserToken tokenInCache = db.UserTokens.Where(ut => ut.UserName == userIdStr && ut.Token == tokenToVerify).FirstOrDefault();
                    if (tokenInCache != null)
                    {
                        if (string.Equals(tokenToVerify, tokenInCache.Token))
                        {
                            byte[] data = Convert.FromBase64String(tokenInCache.Token);
                            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                            if (when < DateTimeOffset.UtcNow.AddDays(-30))
                            {
                                db.UserTokens.Remove(tokenInCache);
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            return false;
        }
    }
}
