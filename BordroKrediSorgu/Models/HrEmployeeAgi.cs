using System;
using System.ComponentModel.DataAnnotations;

namespace DeltaWebApi.Models
{
    public class HrEmployeeAgi //!AGI
    {
        public long EMP_IDENTITY { get; set; }

        public long EMP_REL_ID { get; set; }       // SREAL

        [StringLength(101)]
        public string REL_NAME { get; set; }

        public long REL_IDENTITY { get; set; }     // REAL

        public DateTime REL_BIRTHDATE { get; set; }

        [StringLength(6)]
        public string REL_TYPE { get; set; }       // E:Es C:Çocuk

        [StringLength(11)]
        public string WORK_STATUS { get; set; }   // Çalışıyor, Çalışmıyor

        [StringLength(9)]
        public string EDU_STATUS { get; set; }   // Okuyor, Okumuyor

        [StringLength(50)]
        public string SGK_NUMBER { get; set; }

        public DateTime LAST_UPDATE { get; set; }

        [StringLength(20)]
        public string DELTA_DURUM { get; set; }

        public DateTime DELTA_KAYIT { get; set; }

        //KeyV_REL KEY(EMP_IDENTITY, REL_IDENTITY, LAST_UPDATE),PRIMARY

    }
}