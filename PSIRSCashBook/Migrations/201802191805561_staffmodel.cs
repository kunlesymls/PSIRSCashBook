namespace PSIRSCashBook.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class staffmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Staffs",
                c => new
                    {
                        StaffId = c.String(nullable: false, maxLength: 128),
                        Designation = c.String(),
                        MaritalStatus = c.String(),
                        IsActiveStaff = c.Boolean(nullable: false),
                        ActiveStatus = c.String(),
                        FirstName = c.String(maxLength: 50),
                        MiddleName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Gender = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        Religion = c.String(),
                        TownOfBirth = c.String(),
                        StateOfOrigin = c.String(),
                        Nationality = c.String(),
                        CountryOfBirth = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Age = c.Int(nullable: false),
                        Passport = c.Binary(),
                    })
                .PrimaryKey(t => t.StaffId);
            
            AddColumn("dbo.CashBooks", "StaffId", c => c.String(maxLength: 128));
            CreateIndex("dbo.CashBooks", "StaffId");
            AddForeignKey("dbo.CashBooks", "StaffId", "dbo.Staffs", "StaffId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CashBooks", "StaffId", "dbo.Staffs");
            DropIndex("dbo.CashBooks", new[] { "StaffId" });
            DropColumn("dbo.CashBooks", "StaffId");
            DropTable("dbo.Staffs");
        }
    }
}
