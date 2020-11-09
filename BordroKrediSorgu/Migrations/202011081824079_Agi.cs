namespace DeltaWebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Agi : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HrEmployeeAgi",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EMP_IDENTITY = c.Long(nullable: false),
                        EMP_REL_ID = c.Long(nullable: false),
                        REL_NAME = c.String(maxLength: 101),
                        REL_IDENTITY = c.Long(nullable: false),
                        REL_BIRTHDATE = c.DateTime(nullable: false),
                        REL_TYPE = c.String(maxLength: 6),
                        WORK_STATUS = c.String(maxLength: 11),
                        EDU_STATUS = c.String(maxLength: 9),
                        SGK_NUMBER = c.String(maxLength: 50),
                        LAST_UPDATE = c.DateTime(nullable: false),
                        DELTA_DURUM = c.String(maxLength: 20),
                        DELTA_KAYIT = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HrEmployees",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EMP_NAME = c.String(maxLength: 100),
                        EMP_IDENTITY = c.Long(nullable: false),
                        BIRTH_DATE = c.DateTime(nullable: false),
                        BIRTH_PLACE = c.String(maxLength: 100),
                        GENDER = c.String(maxLength: 2),
                        NATIONALITY = c.String(maxLength: 21),
                        SGK_TYPE = c.String(maxLength: 21),
                        HANDICAPE_PERCENTAGE = c.Int(nullable: false),
                        MAIDEN_NAME = c.String(maxLength: 21),
                        EMP_START_DATE = c.DateTime(nullable: false),
                        LOC_NAME = c.String(maxLength: 256),
                        SGK_NUMBER = c.String(maxLength: 51),
                        CITY_NAME = c.String(maxLength: 51),
                        TOWN_NAME = c.String(maxLength: 1001),
                        DIST_NAME = c.String(maxLength: 256),
                        SUBDIST_NAME = c.String(maxLength: 256),
                        POST_CODE = c.String(maxLength: 256),
                        BUILDING_NO = c.String(maxLength: 256),
                        PHONE = c.String(maxLength: 256),
                        EMAIL1 = c.String(maxLength: 256),
                        EMP_CLASS = c.String(maxLength: 11),
                        GRAD_YEAR = c.String(maxLength: 5),
                        SCHOOL_NAME = c.String(maxLength: 101),
                        BRANCH_NAME = c.String(maxLength: 101),
                        SGK_OCC = c.String(maxLength: 8),
                        LOC_START_DATE = c.DateTime(nullable: false),
                        EMP_END_DATE = c.DateTime(nullable: false),
                        BANK_ID = c.Long(nullable: false),
                        BRANCH_CODE = c.Long(nullable: false),
                        ACCOUNT_CODE = c.String(maxLength: 21),
                        IBAN = c.String(maxLength: 51),
                        MAR_STATUS = c.String(maxLength: 2),
                        FATHER_NAME = c.String(maxLength: 100),
                        MOTHER_NAME = c.String(maxLength: 100),
                        PAY_TYPE = c.String(maxLength: 20),
                        PAY_KIND = c.String(maxLength: 20),
                        END_REASON = c.Long(nullable: false),
                        LAST_UPDATE = c.DateTime(nullable: false),
                        DELTA_DURUM = c.String(maxLength: 20),
                        DELTA_KAYIT = c.DateTime(nullable: false),
                        RecVersion = c.Int(nullable: false),
                        Company = c.String(maxLength: 50),
                        Version = c.String(maxLength: 10),
                        CreatedBy = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(maxLength: 50),
                        ModifiedDate = c.DateTime(),
                        DeletedBy = c.String(maxLength: 50),
                        IsDeleted = c.Int(nullable: false),
                        Show = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HrEmployees");
            DropTable("dbo.HrEmployeeAgi");
        }
    }
}
