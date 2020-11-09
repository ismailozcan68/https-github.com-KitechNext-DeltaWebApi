using System.ComponentModel.DataAnnotations;

namespace DeltaWebApi.Models
{
    public class ZDeltaTest
    {
        public long Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
    }
}