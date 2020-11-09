using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Delta.UTL.DBUtil;
using DeltaWebApi.Models;
using DeltaWebApi.Models.Base;
using DeltaWebApi.Util;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json.Linq;

namespace DeltaWebApi.Controllers.Test
{
    public class ZDeltaTestsController : ApiController
    {
        private DeltaWebApiContext db = new DeltaWebApiContext();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static string   _ApiAllowedIpAddreses    = "::1;127.0.0.1";
        private static string[] _ApiAllowedIpAddresLists = { "::1", "127.0.0.1" };

        private static string   _DataCustomerUserPass = "authUser=XXXuser;authPass=XXXpass12345";
        private static string[] _DataCustomerUserPassParams = { };

        private string _authUser;
        private string _authPass;

        private string errorMessage;

        public ZDeltaTestsController()
        {
            //ip
            if (ConfigurationManager.AppSettings["ApiAllowedIpAddresList"] != null)
            {
                _ApiAllowedIpAddreses = ConfigurationManager.AppSettings["ApiAllowedIpAddresList"].GetString();
            }
            _ApiAllowedIpAddresLists = _ApiAllowedIpAddreses.Split(';');  // liste oldu

            //user
            if (ConfigurationManager.AppSettings["DataCustomerUserPass"] != null)
            {
                _DataCustomerUserPass = ConfigurationManager.AppSettings["DataCustomerUserPass"].GetString();
            }
            _DataCustomerUserPassParams = _DataCustomerUserPass.Split(';');

            foreach (string item in _DataCustomerUserPassParams)
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


        // GET: api/ZDeltaTests/GetForm
        public HttpResponseMessage GetForm(FormDataCollection form)
        {
            string errorMessage = "";

            string authPass = form.Get("authPass");
            string authUser = form.Get("authUser");

            //check auth
            if (_authUser != authUser || _authPass != authPass)
            {
                logger.Error("Hatalı Şifre" + " : " + authUser + "-" + authPass);
                errorMessage = "Hatalı Şifre";
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            //check IP
            string remoteIpAddress = GetIp();
            if (!DeltaWebApiUtils.IsIpAddressAllowed(remoteIpAddress, _ApiAllowedIpAddresLists))
            {
                errorMessage = "Yetkiniz Yok" + " :" + remoteIpAddress;
                logger.Error("Yetkiniz Yok");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            var dtResult = db.ZDeltaTests.ToList();
            return Request.CreateResponse(DataSourceLoader.Load(JToken.FromObject(dtResult), new DataSourceLoadOptionsBase()));
        }

        /// <summary>
        /// EF kullanılmadan Sql Server
        /// </summary>        
        // GET: api/ZDeltaTests/GetFormDt
        public HttpResponseMessage GetFormDt(FormDataCollection form)
        {
            string errorMessage = "";

            string authPass = form.Get("authPass");
            string authUser = form.Get("authUser");

            //check auth
            if (_authUser != authUser || _authPass != authPass)
            {
                logger.Error("Hatalı Şifre" + " : " + authUser + "-" + authPass);
                errorMessage = "Hatalı Şifre";
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            //check IP
            string remoteIpAddress = GetIp();
            if (!DeltaWebApiUtils.IsIpAddressAllowed(remoteIpAddress, _ApiAllowedIpAddresLists))
            {
                errorMessage = "Yetkiniz Yok" + " :" + remoteIpAddress;
                logger.Error("Yetkiniz Yok");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            DataTable dtResult = GetZDeltaTestsdt();
            return Request.CreateResponse(DataSourceLoader.Load(JToken.FromObject(dtResult), new DataSourceLoadOptionsBase()));
        }

        /// <summary>
        /// GetZDeltaTests tum kayıtlar
        /// </summary>
        // GET: api/ZDeltaTests/GetZDeltaTests
        public HttpResponseMessage GetZDeltaTests()
        {
            //check IP
            string remoteIpAddress = GetIp();
            if (!DeltaWebApiUtils.IsIpAddressAllowed(remoteIpAddress, _ApiAllowedIpAddresLists))
            {
                errorMessage = "Yetkiniz Yok" + " :" + remoteIpAddress;
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            IEnumerable<ZDeltaTest> x = db.ZDeltaTests.ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, x);
            return response;
        }

        /// <summary>
        /// Looks up some data by ID.
        /// </summary>
        /// <param name="id">The ID of the data.</param>
        // GET: api/ZDeltaTests/5
        [ResponseType(typeof(ZDeltaTest))]
        public IHttpActionResult GetZDeltaTest(long id)
        {
            ZDeltaTest zDeltaTest = db.ZDeltaTests.FirstOrDefault(x=>x.Id == id);
            if (zDeltaTest == null)
            {
                return NotFound();
            }

            return Ok(zDeltaTest);
        }

        /// <summary>
        /// Looks up some data by ID.
        /// </summary>
        /// <param name="id">The ID of the data.</param>
        /// <param name="ZDeltaTest"> for update ZDeltaTest </param>
        // PUT: api/ZDeltaTests/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutZDeltaTest(long id, ZDeltaTest zDeltaTest)
        {
            ResultModel result = IsValid(zDeltaTest);

            if (!result.IsSucceed)
            {
                ModelState.AddModelError("", result.Message);
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != zDeltaTest.Id)
            {
                return BadRequest();
            }

            db.Entry(zDeltaTest).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZDeltaTestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ZDeltaTests
        [ResponseType(typeof(ZDeltaTest))]
        public IHttpActionResult PostZDeltaTest(ZDeltaTest zDeltaTest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ZDeltaTests.Add(zDeltaTest);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = zDeltaTest.Id }, zDeltaTest);
        }

        // DELETE: api/ZDeltaTests/5
        [ResponseType(typeof(ZDeltaTest))]
        public IHttpActionResult DeleteZDeltaTest(long id)
        {
            ZDeltaTest zDeltaTest = db.ZDeltaTests.Find(id);
            if (zDeltaTest == null)
            {
                return NotFound();
            }

            db.ZDeltaTests.Remove(zDeltaTest);
            db.SaveChanges();

            return Ok(zDeltaTest);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ZDeltaTestExists(long id)
        {
            return db.ZDeltaTests.Count(e => e.Id == id) > 0;
        }


        // ekler
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

        private DataTable GetZDeltaTestsdt()
        {
            string message;
            DBClassSingle db = new DBClassSingle();

            DataTable dt = db.ExecuteSql("Select * from ZDeltaTests ", out message);

            if (message != null && message != "")
                throw new Exception(message);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt;
            }
        }

        private ResultModel IsValid(ZDeltaTest zDeltaTest)
        {
            ResultModel result = new ResultModel();

            if (zDeltaTest.Name.Equals(""))
            {
                result.IsSucceed = false;
                result.Message = "Adı alanı dolu olmalıdır"; //DeltaResources.Resources.StartDateMustbeBiggerThanEndDate;
            }

            return result;
        }
    }
}