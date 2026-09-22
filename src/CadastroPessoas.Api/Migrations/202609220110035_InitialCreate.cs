namespace CadastroPessoas.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cnpjs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Numero = c.String(),
                        PessoaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pessoas", t => t.PessoaId, cascadeDelete: true)
                .Index(t => t.PessoaId);
            
            CreateTable(
                "dbo.Pessoas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(),
                        Tipo = c.Int(nullable: false),
                        Cpf = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cnpjs", "PessoaId", "dbo.Pessoas");
            DropIndex("dbo.Cnpjs", new[] { "PessoaId" });
            DropTable("dbo.Pessoas");
            DropTable("dbo.Cnpjs");
        }
    }
}
