using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace DeltaWebApi.App_Start
{
    public class DeltaSimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        //delta 
        private static int _DeltatokenExpiry = 15;

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Create(context);
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            object owinCollection;
            context.OwinContext.Environment.TryGetValue("Microsoft.Owin.Form#collection", out owinCollection);

            var grantType = ((FormCollection)owinCollection)?.GetValues("grant_type").FirstOrDefault();

            if (grantType == null || grantType.Equals("refresh_token")) return;

            if (ConfigurationManager.AppSettings["ApiTokenExpiry"] != null)
                _DeltatokenExpiry = Convert.ToInt32(ConfigurationManager.AppSettings["ApiTokenExpiry"]);

            //Dilerseniz access_token'dan farklı olarak refresh_token'ın expire time'ını da belirleyebilir, uzatabilirsiniz 
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(_DeltatokenExpiry);

            context.SetToken(context.SerializeTicket());
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            if (context.Ticket == null)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "invalid token";
                return;
            }

            context.SetTicket(context.Ticket);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Receive(context);
        }
    }
}