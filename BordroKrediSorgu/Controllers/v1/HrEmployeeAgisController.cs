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
using DeltaWebApi.Models;
using DeltaWebApi.Util;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json.Linq;

namespace DeltaWebApi.Controllers.v1
{
    public class HrEmployeeAgisController : ApiController
    {
        private DeltaWebApiContext db = new DeltaWebApiContext();

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static string _ApiAllowedIpAddreses = "::1;127.0.0.1";
        private static string[] _ApiAllowedIpAddresLists = { "::1", "127.0.0.1" };

        private string errorMessage;

        private static string _DataCustomerUserPass = "authUser=XXXuser;authPass=XXXpass12345";
        private static string[] _DataCustomerUserPassParams = { };
        private string _authUser;
        private string _authPass;


        public HrEmployeeAgisController()
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

        // GET: api/HrEmployeeAgis
        public HttpResponseMessage GetForm(FormDataCollection form)
        {
            string errorMessage = "";

            string authPass = form.Get("authPass");
            string authUser = form.Get("authUser");
            string userNameForKey = form.Get("userNameForKey");

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

            //DataTable dtResult = new DataTable();
            var dtResult = db.HrEmployeeAgis.ToList();

            return Request.CreateResponse(DataSourceLoader.Load(JToken.FromObject(dtResult), new DataSourceLoadOptionsBase()));
            //return Request.CreateResponse(DataSourceLoader.Load(JToken.FromObject(dtResult), new DataSourceLoadOptions()));
        }

        // GET: api/HrEmployeeAgis
        public HttpResponseMessage GetHrEmployeeAgis()
        {
            //check IP
            string remoteIpAddress = GetIp();
            if (!DeltaWebApiUtils.IsIpAddressAllowed(remoteIpAddress, _ApiAllowedIpAddresLists))
            {
                errorMessage = "Yetkiniz Yok" + " :" + remoteIpAddress;
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            IEnumerable<HrEmployeeAgi> x = db.HrEmployeeAgis.ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, x);
            return response;
        }

        // GET: api/HrEmployeeAgis/5
        [ResponseType(typeof(HrEmployeeAgi))]
        public IHttpActionResult GetHrEmployeeAgi(Guid id)
        {
            HrEmployeeAgi hrEmployeeAgi = db.HrEmployeeAgis.FirstOrDefault(x=>x.Id == id);
            if (hrEmployeeAgi == null)
            {
                return NotFound();
            }

            return Ok(hrEmployeeAgi);
        }

        // PUT: api/HrEmployeeAgis/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHrEmployeeAgi(Guid id, HrEmployeeAgi hrEmployeeAgi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != hrEmployeeAgi.Id)
            {
                return BadRequest();
            }

            db.Entry(hrEmployeeAgi).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HrEmployeeAgiExists(id))
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

        // POST: api/HrEmployeeAgis
        [ResponseType(typeof(HrEmployeeAgi))]
        public IHttpActionResult PostHrEmployeeAgi(HrEmployeeAgi hrEmployeeAgi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HrEmployeeAgis.Add(hrEmployeeAgi);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (HrEmployeeAgiExists(hrEmployeeAgi.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = hrEmployeeAgi.Id }, hrEmployeeAgi);
        }

        // DELETE: api/HrEmployeeAgis/5
        [ResponseType(typeof(HrEmployeeAgi))]
        public IHttpActionResult DeleteHrEmployeeAgi(Guid id)
        {
            HrEmployeeAgi hrEmployeeAgi = db.HrEmployeeAgis.Find(id);
            if (hrEmployeeAgi == null)
            {
                return NotFound();
            }

            db.HrEmployeeAgis.Remove(hrEmployeeAgi);
            db.SaveChanges();

            return Ok(hrEmployeeAgi);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HrEmployeeAgiExists(Guid id)
        {
            return db.HrEmployeeAgis.Count(e => e.Id == id) > 0;
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
    }
}