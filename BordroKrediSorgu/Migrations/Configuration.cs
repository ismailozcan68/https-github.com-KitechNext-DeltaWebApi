namespace DeltaWebApi.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DeltaWebApi.Models.DeltaWebApiContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DeltaWebApi.Models.DeltaWebApiContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            //ornek
            context.ZDeltaTests.AddOrUpdate(x => x.Id,
             new Models.ZDeltaTest { Id = 1, Name = "Kayıt 1" },
             new Models.ZDeltaTest { Id = 2, Name = "Kayıt 2" },
             new Models.ZDeltaTest { Id = 3, Name = "Kayıt 3" }
            );

        }
    }
}
