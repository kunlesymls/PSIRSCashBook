namespace PSIRSCashBook.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modelling : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CashBooks",
                c => new
                    {
                        CashBookId = c.Int(nullable: false, identity: true),
                        TransactionDate = c.DateTime(nullable: false),
                        Payee = c.String(),
                        Purpose = c.String(),
                        PvCode = c.String(),
                        Amount = c.Double(nullable: false),
                        ItemId = c.Int(nullable: false),
                        PsirsCodeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CashBookId)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.PsirsCodes", t => t.PsirsCodeId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.PsirsCodeId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemId = c.Int(nullable: false, identity: true),
                        ItemName = c.String(),
                        PsirsCodeId = c.Int(),
                    })
                .PrimaryKey(t => t.ItemId)
                .ForeignKey("dbo.PsirsCodes", t => t.PsirsCodeId)
                .Index(t => t.PsirsCodeId);
            
            CreateTable(
                "dbo.PsirsCodes",
                c => new
                    {
                        PsirsCodeId = c.Int(nullable: false, identity: true),
                        CodeName = c.String(),
                    })
                .PrimaryKey(t => t.PsirsCodeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "PsirsCodeId", "dbo.PsirsCodes");
            DropForeignKey("dbo.CashBooks", "PsirsCodeId", "dbo.PsirsCodes");
            DropForeignKey("dbo.CashBooks", "ItemId", "dbo.Items");
            DropIndex("dbo.Items", new[] { "PsirsCodeId" });
            DropIndex("dbo.CashBooks", new[] { "PsirsCodeId" });
            DropIndex("dbo.CashBooks", new[] { "ItemId" });
            DropTable("dbo.PsirsCodes");
            DropTable("dbo.Items");
            DropTable("dbo.CashBooks");
        }
    }
}
