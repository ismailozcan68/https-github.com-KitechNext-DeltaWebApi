using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeltaWebApi.Models
{
    public class KrediSorgu
    {
        public String HesapNo { get; set; }
        public double MaasAdet { get; set; }
        public double KrediTutar { get; set; }
    }
}