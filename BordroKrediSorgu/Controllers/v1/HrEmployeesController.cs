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
    public class HrEmployeesController : ApiController
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

        public HrEmployeesController()
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


        public HttpResponseMessage GetForm(FormDataCollection form)
        {
            string errorMessage = "";

            string authPass = form.Get("authPass");
            string authUser = form.Get("authUser");
            string userNameForKey = form.Get("userNameForKey");

            //check auth
            if (_authUser != authUser || _authPass != authPass)
            {
                logger.Error("HrEmployees Hatalı Şifre" + " : " + authUser + "-" + authPass);
                errorMessage = "HrEmployees Hatalı Şifre";
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            //check IP
            string remoteIpAddress = GetIp();
            if (!DeltaWebApiUtils.IsIpAddressAllowed(remoteIpAddress, _ApiAllowedIpAddresLists))
            {
                errorMessage = "HrEmployees Yetkiniz Yok" + " :" + remoteIpAddress;
                logger.Error("HrEmployees Yetkiniz Yok");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            var dtResult = db.HrEmployees.ToList();

            return Request.CreateResponse(DataSourceLoader.Load(JToken.FromObject(dtResult), new DataSourceLoadOptionsBase()));
        }

        // GET: api/HrEmployees
        public HttpResponseMessage GetHrEmployees()
        {
            //check IP
            string remoteIpAddress = GetIp();
            if (!DeltaWebApiUtils.IsIpAddressAllowed(remoteIpAddress, _ApiAllowedIpAddresLists))
            {
                errorMessage = "Yetkiniz Yok" + " :" + remoteIpAddress;
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            IEnumerable<HrEmployee> x = db.HrEmployees.ToList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, x);
            return response;
        }

        // GET: api/HrEmployees/5
        [ResponseType(typeof(HrEmployee))]
        public IHttpActionResult GetHrEmployee(Guid id)
        {
            HrEmployee hrEmployee = db.HrEmployees.Find(id);
            if (hrEmployee == null)
            {
                return NotFound();
            }

            return Ok(hrEmployee);
        }

        // PUT: api/HrEmployees/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHrEmployee(Guid id, HrEmployee hrEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != hrEmployee.Id)
            {
                return BadRequest();
            }

            db.Entry(hrEmployee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HrEmployeeExists(id))
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

        // POST: api/HrEmployees
        [ResponseType(typeof(HrEmployee))]
        public IHttpActionResult PostHrEmployee(HrEmployee hrEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HrEmployees.Add(hrEmployee);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (HrEmployeeExists(hrEmployee.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = hrEmployee.Id }, hrEmployee);
        }

        // DELETE: api/HrEmployees/5
        [ResponseType(typeof(HrEmployee))]
        public IHttpActionResult DeleteHrEmployee(Guid id)
        {
            HrEmployee hrEmployee = db.HrEmployees.Find(id);
            if (hrEmployee == null)
            {
                return NotFound();
            }

            db.HrEmployees.Remove(hrEmployee);
            db.SaveChanges();

            return Ok(hrEmployee);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HrEmployeeExists(Guid id)
        {
            return db.HrEmployees.Count(e => e.Id == id) > 0;
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