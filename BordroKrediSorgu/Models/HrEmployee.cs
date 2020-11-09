using DeltaWebApi.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace DeltaWebApi.Models
{
    public class HrEmployee : BaseTableFieldDNV  // Sicil kaydi 
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(100)]
        public string EMP_NAME { get; set; }

        public long EMP_IDENTITY { get; set; }

        public DateTime BIRTH_DATE { get; set; }

        [StringLength(100)]
        public string BIRTH_PLACE { get; set; }

        [StringLength(2)]
        public string GENDER { get; set; }

        [StringLength(21)]
        public string NATIONALITY { get; set; }

        [StringLength(21)]
        public string SGK_TYPE { get; set; }                 // E:Emekli N:Normal

        public int HANDICAPE_PERCENTAGE { get; set; }       // REAL     1,2,3

        [StringLength(21)]
        public string MAIDEN_NAME { get; set; }

        public DateTime EMP_START_DATE { get; set; }         // SIC:ILKGIRISTRH

        [StringLength(256)]
        public string LOC_NAME { get; set; }

        [StringLength(51)]
        public string SGK_NUMBER { get; set; }

        [StringLength(51)]
        public string  CITY_NAME { get; set; }

        [StringLength(1001)]
        public string TOWN_NAME { get; set; }

        [StringLength(256)]
        public string DIST_NAME { get; set; }

        [StringLength(256)]
        public string SUBDIST_NAME { get; set; }

        [StringLength(256)]
        public string POST_CODE { get; set; }

        [StringLength(256)]
        public string BUILDING_NO { get; set; }

        [StringLength(256)]
        public string PHONE { get; set; }

        [StringLength(256)]
        public string EMAIL1 { get; set; }

        [StringLength(11)]
        public string EMP_CLASS { get; set; }         //        M:Mavi Yaka B:Beyaz yaka

        [StringLength(5)]
        public string GRAD_YEAR { get; set; }

        [StringLength(101)]
        public string SCHOOL_NAME { get; set; }

        [StringLength(101)]
        public string BRANCH_NAME { get; set; }

        [StringLength(8)]
        public string SGK_OCC { get; set; }

        public DateTime LOC_START_DATE { get; set; }   //  SIC:GirisTrh

        public DateTime EMP_END_DATE { get; set; }     //  SIC:CikisTrh

        public long BANK_ID { get; set; }

        public long BRANCH_CODE { get; set; }

        [StringLength(21)]
        public string ACCOUNT_CODE { get; set; }

        [StringLength(51)]
        public string IBAN { get; set; }

        [StringLength(2)]
        public string MAR_STATUS { get; set; }

        [StringLength(100)]
        public string FATHER_NAME { get; set; }

        [StringLength(100)]
        public string MOTHER_NAME { get; set; }

        [StringLength(20)]
        public string PAY_TYPE { get; set; }          // SIC:NetBrut N:Net B:Brut

        [StringLength(20)]
        public string PAY_KIND { get; set; }          // SIC:AYLIKSAAT G:Gun S:Saat

        public long END_REASON { get; set; }          // SIC:CikisNedeni

        public DateTime LAST_UPDATE { get; set; }

        [StringLength(20)]
        public string DELTA_DURUM { get; set; }

        public DateTime DELTA_KAYIT { get; set; }

        //KeyV_EMP KEY(EMP_IDENTITY, EMP_START_DATE, LAST_UPDATE),PRIMARY
    }
}