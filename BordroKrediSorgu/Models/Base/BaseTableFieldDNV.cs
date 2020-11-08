using System;
using System.ComponentModel.DataAnnotations;

namespace DeltaWebApi.Models.Base
{
    public class BaseTableFieldDNV  
    {
        public long Id { get; set; }

        public Guid GuidId { get; set; }

        public int RecVersion { get; set; }

        [StringLength(50)]
        public string Company { get; set; }

        [StringLength(10)]
        public string Version { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string DeletedBy { get; set; }

        public int IsDeleted { get; set; } = 0;

        public bool Show { get; set; } = true;

    }
}