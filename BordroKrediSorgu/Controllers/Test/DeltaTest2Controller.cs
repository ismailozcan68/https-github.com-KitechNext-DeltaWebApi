using DeltaWebApi.DBContext;
using DeltaWebApi.Helpers;
using DeltaWebApi.Util;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;

namespace DeltaWebApi.Controllers.Test
{
    public class DeltaTest2Controller : ApiController
    {
        private static string _customerApiAllowedIpAddreses = "::1;127.0.0.1";
        private static string[] _customerApiAllowedIpAddresLists = { "::1", "127.0.0.1" };
        private static string _matixInfoDataCustomerService = "authUser=XXXuser;authPass=XXXpass12345";
        private static string[] _matixInfoDataCustomerServiceParams = { };

        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string _authUser;
        private string _authPass;
        private string _serviceRepositoryId;
        private string _serviceCompanyId;

        public DeltaTest2Controller()
        {
            if (ConfigurationManager.AppSettings["CustomerApiAllowedIpAddresList"] != null)
            {
                _customerApiAllowedIpAddreses = ConfigurationManager.AppSettings["CustomerApiAllowedIpAddresList"].GetString();
            }

            _customerApiAllowedIpAddresLists = _customerApiAllowedIpAddreses.Split(';');


            if (ConfigurationManager.AppSettings["OsmanliMenkulMatrixDataCustomerService"] != null)
            {
                _matixInfoDataCustomerService = ConfigurationManager.AppSettings["OsmanliMenkulMatrixDataCustomerService"].GetString();
            }

            _matixInfoDataCustomerServiceParams = _matixInfoDataCustomerService.Split(';');

            foreach (string item in _matixInfoDataCustomerServiceParams)
            {
                if (item.Contains("authUser"))
                {
                    _authUser = item.Split('=')[1];
                }
                if (item.Contains("authPass"))
                {
                    _authPass = item.Split('=')[1];
                }
                if (item.Contains("serviceRepositoryId"))
                {
                    _serviceRepositoryId = item.Split('=')[1];
                }
                if (item.Contains("serviceCompanyId"))
                {
                    _serviceCompanyId = item.Split('=')[1];
                }
            }
        }

        //[HttpPost]
        //[Route("api/CustomerServiceApi/GetInfoData", Name = "GetInfoData")]
        //public HttpResponseMessage GetInfoData(FormDataCollection form)
        //{
        //    string errorMessage = "";

        //    string authPass = form.Get("authPass");
        //    string authUser = form.Get("authUser");
        //    string userNameForKey = form.Get("userNameForKey");

        //    //check policy
        //    if (!PasswordPolicy.IsValid(authPass, out errorMessage))
        //    {
        //        logger.Error("Hatalı Şifre" + " : " + authUser + "-" + authPass);
        //        errorMessage = "Hatalı Şifre";
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
        //    }

        //    //check auth
        //    if (_authUser != authUser || _authPass != authPass)
        //    {
        //        logger.Error("Hatalı Şifre" + " : " + authUser + "-" + authPass);
        //        errorMessage = "Hatalı Şifre";
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
        //    }

        //    //check IP
        //    string remoteIpAddress = GetIp();

        //    if (!IsIpAddressAllowed(remoteIpAddress))
        //    {
        //        errorMessage = "Yetkiniz Yok" + " :" + remoteIpAddress;
        //        logger.Error("Yetkiniz Yok");
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
        //    }

        //    JObject json = new JObject();
        //    json["CompanyId"] = _serviceCompanyId;
        //    json["SeachKey"] = userNameForKey;

        //    long repoId = long.Parse(_serviceRepositoryId);

        //    DBClass Definitions = new DBClass();
        //    DataSet ds = Definitions.ExecuteDataSet(repoId, "sp_Delta", Version.Parse("1.0.0.1"), "TR", out errorMessage, json.ToString(Formatting.None), 0);
        //    DataTable dtResult = new DataTable();
        //    if (DBUtils.ExistsRecord(ds))
        //    {
        //        dtResult = ds.Tables[0];
        //    }
        //    else
        //    {
        //        errorMessage = "Hata Oluştu";
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
        //    }

        //    return Request.CreateResponse(DataSourceLoader.Load(JToken.FromObject(dtResult), new DataSourceLoadOptions()));
        //}

        private bool IsIpAddressAllowed(string ipAddr)
        {
            foreach (string item in _customerApiAllowedIpAddresLists)
            {
                if (item.Equals(ipAddr))
                {
                    return true;
                }
                else if (ipAddr.StartsWith(item))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetIp()
        {
            return GetClientIp();
        }

        private string GetClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }

        //// GET: api/DeltaTest2
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/DeltaTest2/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/DeltaTest2
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/DeltaTest2/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/DeltaTest2/5
        //public void Delete(int id)
        //{
        //}
    }
}
