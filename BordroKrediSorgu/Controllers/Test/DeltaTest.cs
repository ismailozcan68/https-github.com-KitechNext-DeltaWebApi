using DeltaWebApi.Helpers;
using DeltaWebApi.Util;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;

namespace DeltaWebApi.Controllers.Test
{
    public class DeltaTest : ApiController
    {
        private static string _customerApiAllowedIpAddreses = "::1;127.0.0.1";
        private static string[] _customerApiAllowedIpAddresLists = { "::1", "127.0.0.1" };
        private static string _matixInfoDataCustomerService = "authUser=XXXuser;authPass=XXXpass12345";
        private static string[] _matixInfoDataCustomerServiceParams = { };

        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private string _authUser;
        private string _authPass;

        public DeltaTest()
        {
            if (ConfigurationManager.AppSettings["CustomerApiAllowedIpAddresList"] != null)
            {
                _customerApiAllowedIpAddreses = ConfigurationManager.AppSettings["CustomerApiAllowedIpAddresList"].GetString();
            }

            _customerApiAllowedIpAddresLists = _customerApiAllowedIpAddreses.Split(';');


            if (ConfigurationManager.AppSettings["OsmanliMenkulMatrixDataCustomerService"] != null)
            {
                _matixInfoDataCustomerService = ConfigurationManager.AppSettings["DataCustomerService"].GetString();
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
            }
        }

        // GET api/<controller>/5
        public HttpResponseMessage GetForm(FormDataCollection form)
        {
            string errorMessage = "";

            string authPass = form.Get("authPass");
            string authUser = form.Get("authUser");
            string userNameForKey = form.Get("userNameForKey");

            //check policy
            if (!PasswordPolicy.IsValid(authPass, out errorMessage))
            {
                logger.Error("Hatalı Şifre" + " : " + authUser + "-" + authPass);
                errorMessage = "Hatalı Şifre";
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            //check auth
            if (_authUser != authUser || _authPass != authPass)
            {
                logger.Error("Hatalı Şifre" + " : " + authUser + "-" + authPass);
                errorMessage = "Hatalı Şifre";
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            //check IP
            string remoteIpAddress = GetIp();
            if (!IsIpAddressAllowed(remoteIpAddress))
            {
                errorMessage = "Yetkiniz Yok" + " :" + remoteIpAddress;
                logger.Error("Yetkiniz Yok");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            DataTable dtResult = new DataTable();

            return Request.CreateResponse(DataSourceLoader.Load(JToken.FromObject(dtResult), new DataSourceLoadOptionsBase()));
            //return Request.CreateResponse(DataSourceLoader.Load(JToken.FromObject(dtResult), new DataSourceLoadOptions()));
        }

        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/<controller>
        //public string GetById(int id)
        //{
        //    return "value";
        //}

        // POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE api/<controller>/5
        //public void Delete(int id)
        //{
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

    }
}