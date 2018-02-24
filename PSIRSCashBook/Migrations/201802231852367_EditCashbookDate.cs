namespace PSIRSCashBook.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditCashbookDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CashBooks", "TransactionDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CashBooks", "TransactionDate", c => c.DateTime());
        }
    }
}
