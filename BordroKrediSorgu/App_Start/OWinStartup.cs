using System;
using System.Configuration;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;


[assembly: OwinStartup(typeof(DeltaWebApi.App_Start.OWinStartup))]

namespace DeltaWebApi.App_Start
{
    public class OWinStartup
    {
        //delta 
        private static int _DeltatokenExpiry = 15;

        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            HttpConfiguration config = new HttpConfiguration();
            ConfigureOAuth(app);
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }

        private void ConfigureOAuth(IAppBuilder appBuilder)
        {
            try
            {
                if (ConfigurationManager.AppSettings["ApiTokenExpiry"] != null)
                    _DeltatokenExpiry = Convert.ToInt32(ConfigurationManager.AppSettings["ApiTokenExpiry"]);

                OAuthAuthorizationServerOptions oAuthAuthorizationServerOptions = new OAuthAuthorizationServerOptions()
                {
                    TokenEndpointPath = new PathString("/token"), // token alacağımız path'i belirtiyoruz
                    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(_DeltatokenExpiry),
                    AllowInsecureHttp = true,
                    Provider = new DeltaSimpleAuthorizationServerProvider(),
                    //RefreshTokenProvider = new DeltaSimpleRefreshTokenProvider(),
                };
                // AppBuilder'a token üretimini gerçekleştirebilmek için ilgili authorization ayarlarımızı veriyoruz.
                appBuilder.UseOAuthAuthorizationServer(oAuthAuthorizationServerOptions);

                // Authentication type olarak ise Bearer Authentication'ı kullanacağımızı belirtiyoruz.
                // Bearer token OAuth 2.0 ile gelen standartlaşmış token türüdür.
                // Herhangi kriptolu bir veriye ihtiyaç duymadan client tarafından token isteğinde bulunulur ve server belirli bir expire date'e sahip bir access_token üretir.
                // Bearer token üzerinde güvenlik SSL'e dayanır.
                // Bir diğer tip ise MAC token'dır. OAuth 1.0 versiyonunda kullanılıyor, hem client'a, hemde server tarafına implementasyonlardan dolayı ek maliyet çıkartmaktadır. 
                //Bu maliyetin yanı sıra ise Bearer token'a göre kaynak alış verişinin biraz daha güvenli olduğu söyleniyor çünkü client her request'inde veriyi hmac 
                //ile imzalayıp verileri kriptolu bir şekilde göndermeleri gerektiği için.
                appBuilder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            }
            catch (Exception)
            {
                OwinResponse response = new OwinResponse();
                response.Context.TraceOutput.ToString();
            }
        }
    }
}
