using System.Data.Entity;

namespace DeltaWebApi.Models
{
    public class DeltaWebApiContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public DeltaWebApiContext() : base("name=DeltaWebApiContext")
        {
        }

        public System.Data.Entity.DbSet<DeltaWebApi.Models.ZDeltaTest> ZDeltaTests { get; set; }

        public System.Data.Entity.DbSet<DeltaWebApi.Models.HrEmployeeAgi> HrEmployeeAgis { get; set; }

        public System.Data.Entity.DbSet<DeltaWebApi.Models.HrEmployee> HrEmployees { get; set; }
    }
}
