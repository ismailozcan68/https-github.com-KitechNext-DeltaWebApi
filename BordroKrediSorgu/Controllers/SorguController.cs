using DeltaWebApi.Models;
using Delta.UTL.DBUtil;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web.Http;

namespace DeltaWebApi.Controllers
{
    public class SorguController : ApiController
    {
        private static bool isConfigReady = false;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static string processName = "C:\\Source\\Repos\\SamplePayroll\\SamplePayroll\\bin\\Debug\\SamplePayroll.exe";
        private static string payrollWorkDir = "E:\\Delta";

        public SorguController()
        {
            if (!isConfigReady)
            {
                processName = ConfigurationManager.AppSettings["PayrollApp"];
                payrollWorkDir = ConfigurationManager.AppSettings["PayrollWorkDir"];
                isConfigReady = true;
            }
        }

        // GET api/Sorgu/5
        public KrediSorguCevap Get(string hesapno, string maasadet, string kredi)
        {
            if (maasadet.Contains(",") || kredi.Contains(","))
            {
                throw new Exception("Numeric value format exception");
            }

            KrediSorguCevap krediSorguCevap = new KrediSorguCevap();
            KrediSorgu sorgu = new KrediSorgu();
            sorgu.HesapNo = hesapno;

            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";

            sorgu.MaasAdet = double.Parse(maasadet, format);
            sorgu.KrediTutar = double.Parse(kredi, format);

            DoKrediSorgu(hesapno, krediSorguCevap, sorgu);

            return krediSorguCevap;
        }

        // POST api/Sorgu
        public KrediSorguCevap Post([FromBody]KrediSorgu sorgu)
        {
            KrediSorguCevap krediSorguCevap = new KrediSorguCevap();
            krediSorguCevap.Durumu = false;

            DoKrediSorgu(sorgu.HesapNo, krediSorguCevap, sorgu);

            return krediSorguCevap;
        }

        private void DoKrediSorgu(string hesapno, KrediSorguCevap krediSorguCevap, KrediSorgu sorgu)
        {
            try
            {
                //Log Info To DB
                string RequestId = WriteKrediSorgu(sorgu);

                //Todo Bilin Sorgulam
                krediSorguCevap.Durumu = false;
                krediSorguCevap.DurumAciklama = "Bilin Uygun değil";


                krediSorguCevap.Durumu = false;
                krediSorguCevap.DurumAciklama = "Uygun Değil";
                logger.Debug("KrediSorgu Requested");
                //Hesap Bilgi Sorgula
                DataTable dth = GetHesapBilgi(hesapno);
                if (dth != null)
                {
                    //Hep bir önceki ay için sorgu altıyor.                      
                    //DateTime workingYear = DateTime.Today.AddMonths(-1);

                    DateTime workingYear = DateTime.Today;

                    //Update CompanyCode,User Code
                    UpdateKrediSorgu(RequestId, dth.Rows[0]["FIRMAADI"].ToString().Trim(), dth.Rows[0]["TCNO"].ToString().Trim());

                    StringBuilder sb = new StringBuilder();
                    sb.Append("RequestId=").Append(RequestId).Append(" ")
                      .Append("CompanyCode=").Append(dth.Rows[0]["FIRMAADI"].ToString().Trim()).Append(" ")
                      .Append("WorkingYear=").Append(workingYear.Year).Append(" ")
                      .Append("TCNO=").Append(dth.Rows[0]["TCNO"].ToString().Trim()).Append(" ")
                      .Append("MaasAdet=").Append(sorgu.MaasAdet).Append(" ")
                      .Append("KrediTutar=").Append(sorgu.KrediTutar).Append(" ")
                      ;

                    //Execute Exe and Wait unit finish
                    bool hasExited = RunProcess(processName, sb.ToString());
                    if (hasExited)
                    {
                        //After Exe operation Get Status from DB
                        DataTable dt = GetKrediDurum(RequestId);
                        if (dt != null)
                        {
                            if (dt.Rows[0]["Status"].ToString() == "1")
                            {
                                krediSorguCevap.Durumu = true;
                            }
                            else
                            {
                                krediSorguCevap.Durumu = false;
                            }

                            krediSorguCevap.DurumAciklama = dt.Rows[0]["StatusDescription"].ToString();
                        }
                        else
                        {
                            krediSorguCevap.Durumu = false;
                            krediSorguCevap.DurumAciklama = "Kredi bilgisi okunamadı.";
                        }
                    }
                }
                else
                {
                    krediSorguCevap.Durumu = false;
                    krediSorguCevap.DurumAciklama = "Hesap Kod tanımlı değil.";
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw ex;
            }
        }

        private bool RunProcess(string processName, string arguments)
        {
            logger.Trace(processName + " " + arguments);

            Process process = new Process();
            process.StartInfo.FileName = processName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WorkingDirectory = payrollWorkDir;
            process.Start();
            process.WaitForExit();
            return process.HasExited;
        }

        private DataTable GetKrediDurum(string RequestId)
        {
            string message;
            DBClassSingle db = new DBClassSingle();

            DataTable dt = db.ExecuteSql("Select * from CreditRequests where RequestId=@RequestId", out message,
                new Parameter() { ParamName = "RequestId", ParamValue = RequestId });

            if (message!= null && message!="")
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

        private DataTable GetHesapBilgi(string HesapNo)
        {
            string message;
            DBClassSingle db = new DBClassSingle();

            DataTable dt = db.ExecuteSql("SELECT [TCNO],[DEPKOD],[PERKOD],[FIRMAADI],[HSKOD],[ISLEMTARIHI] FROM [dbo].[CreditPersonList] Where HSKOD=@HesapNo", out message,
                new Parameter() { ParamName = "HesapNo", ParamValue = HesapNo });

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

        private string WriteKrediSorgu(KrediSorgu krediSorgu)
        {
            string message;
            string RequestId = "";
            try
            {
                DBClassSingle db = new DBClassSingle();
                RequestId = Guid.NewGuid().ToString();

                string insertSQL= @"INSERT INTO [dbo].[CreditRequests]
                                    ([RequestId]
                                    ,[RequestDate]
                                    ,[CompanyCode]
                                    ,[UserCode]
                                    ,[WorkingYear]
                                    ,[Request]
                                    ,[HesapNo]
                                    ,[MaasAdet]
                                    ,[KrediTutar]
                                    ,[Status]
                                    ,[StatusDescription]
                                    ,[ResponseDate]
                                    ,[TryCount])
                                VALUES
                                    (@RequestId
                                    ,GetDate()
                                    ,@CompanyCode
                                    ,@UserCode
                                    ,@WorkingYear
                                    ,@Request
                                    ,@HesapNo
                                    ,@MaasAdet
                                    ,@KrediTutar
                                    ,0
                                    ,null
                                    ,null
                                    ,0)";

                Parameter[] parameters = {
                    new Parameter() { ParamName = "RequestId", ParamValue = RequestId },
                    new Parameter() { ParamName = "CompanyCode", ParamValue = "" },
                    new Parameter() { ParamName = "UserCode", ParamValue = "" },
                    new Parameter() { ParamName = "WorkingYear", ParamValue = DateTime.Today.Year },
                    new Parameter() { ParamName = "Request", ParamValue = Request.ToString() },
                    new Parameter() { ParamName = "HesapNo", ParamValue = krediSorgu.HesapNo },
                    new Parameter() { ParamName = "MaasAdet", ParamValue = krediSorgu.MaasAdet },
                    new Parameter() { ParamName = "KrediTutar", ParamValue = krediSorgu.KrediTutar },
                };

                DataTable dt = db.ExecuteSql(insertSQL, out message, parameters);
            }
            catch (Exception ex)
            {
                RequestId = "";

                throw ex;
            }

            return RequestId;
        }

        private bool UpdateKrediSorgu(string RequestId, string CompanyCode,string UserCode)
        {
            string message;
            try
            {
                DBClassSingle db = new DBClassSingle();
                DataTable dt = db.ExecuteSql("UPDATE [dbo].[CreditRequests] set CompanyCode=@CompanyCode,UserCode=@UserCode Where " +
                    " RequestId=@RequestId", out message,
                        new Parameter() { ParamName = "RequestId", ParamValue = RequestId },
                        new Parameter() { ParamName = "CompanyCode", ParamValue = CompanyCode },
                        new Parameter() { ParamName = "UserCode", ParamValue = UserCode });

                if (message != null && message != "")
                {
                    throw new Exception(message);
                }

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }

}
