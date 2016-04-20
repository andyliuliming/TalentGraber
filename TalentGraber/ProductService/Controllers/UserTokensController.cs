using Macrodeek.Model;
using Macrodeek.StarDustServiceBase;
using Macrodeek.StarDustServiceBase.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Macrodeek.StarDustProductService.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Token>("Tokens");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class UserTokensController : GoldODataController
    {
        private UserToken GenerateUserToken()
        {
            byte[] time = BitConverter.GetBytes(DateTimeOffset.UtcNow.UtcTicks);
            byte[] tokenKey = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(tokenKey).ToArray());

            byte[] refreshKey = Guid.NewGuid().ToByteArray();
            string refreshToken = Convert.ToBase64String(time.Concat(refreshKey).ToArray());

            UserToken userToken = new UserToken();
            userToken.Token = token;
            userToken.Password = string.Empty;
            userToken.Timestamp = DateTime.UtcNow;
            return userToken;
        }

        /// <summary>
        /// if the userTokenToAuth's Token and RefreshToken is not null, it's the refresh token scenario.
        /// </summary>
        /// <param name="userTokenToAuth"></param>
        /// <returns></returns>
        public IHttpActionResult Post(UserToken userTokenToAuth)
        {
            TalentGraberUser user = db.TalentGraberUsers.Where(usr => usr.UserName == userTokenToAuth.UserName && usr.Password == userTokenToAuth.Password).FirstOrDefault();

            if (user != null)
            {
                UserToken userTokenInDb = db.UserTokens.Where(ut => ut.UserName == user.UserName).FirstOrDefault();
                if (userTokenInDb != null)
                {
                    userTokenInDb.Token = this.GenerateUserToken().Token;
                    userTokenInDb.Timestamp = DateTime.UtcNow;
                    db.SaveChanges();
                    return Created(userTokenInDb);
                }
                else
                {
                    UserToken userToken = this.GenerateUserToken();
                    userToken.UserName = userTokenToAuth.UserName;
                    db.UserTokens.Add(userToken);

                    db.SaveChanges();

                    return Created(userToken);
                }
            }
            else
            {
                return Unauthorized();
            }

        }
    }
}