using System.Data.Entity;
using CadastroPessoas.Api.Models;

namespace CadastroPessoas.Api.Data
{
    public class CadastroPessoasContext : DbContext
    {
        public CadastroPessoasContext()
            : base("name=CadastroPessoasContext")
        {
        }

        public DbSet<Pessoa> Pessoas { get; set; }

        public DbSet<Cnpj> Cnpjs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cnpj>()
                .HasRequired(cnpj => cnpj.Pessoa)
                .WithMany(pessoa => pessoa.Cnpjs)
                .HasForeignKey(cnpj => cnpj.PessoaId)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}
