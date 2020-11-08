using Microsoft.Owin.Security.OAuth;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;


namespace DeltaWebApi.App_Start
{
    public class DeltaSimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private static string _Apiusername = "DeltaWebApi";
        private static string _Apipassword = "DeltaWebApi";

        // OAuthAuthorizationServerProvider sınıfının client erişimine izin verebilmek için ilgili ValidateClientAuthentication metotunu override ediyoruz.
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        // OAuthAuthorizationServerProvider sınıfının kaynak erişimine izin verebilmek için ilgili GrantResourceOwnerCredentials metotunu override ediyoruz.
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // kullanici oku
            if (System.Configuration.ConfigurationManager.AppSettings["Apiusername"] != null)
            {
                _Apiusername = ConfigurationManager.AppSettings["Apiusername"];
            }

            if (System.Configuration.ConfigurationManager.AppSettings["Apipassword"] != null)
            {
                _Apipassword = ConfigurationManager.AppSettings["Apipassword"];
            }

            // CORS ayarlarını set ediyoruz.
            // CORS domain’ler arası kaynak paylaşımını sağlamaya yarayan bir mekanizmadır. Bir domain’in bir başka domain’in kaynağını kullanabilmesini sağlar.
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            // Kullanıcının access_token alabilmesi için gerekli validation işlemlerini yapıyoruz.
            if (context.UserName == _Apiusername && context.Password == _Apipassword)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                identity.AddClaim(new Claim("sub", context.UserName));
                identity.AddClaim(new Claim("role", "user"));

                context.Validated(identity);
            }
            else if (context.UserName == "deltawebapi" && context.Password == "123456")
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                identity.AddClaim(new Claim("sub", context.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role, "yonetici"));// ClaimTypes.role girerek Authorize Roles= "Admin" Diyebiliyoruz. 

                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "Kullanıcı adı veya şifre yanlış.");
            }
        }
    }
}