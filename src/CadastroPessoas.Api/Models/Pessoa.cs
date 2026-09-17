using System.Collections.Generic;

namespace CadastroPessoas.Api.Models
{
    public class Pessoa
    {
        public Pessoa()
        {
            Cnpjs = new List<Cnpj>();
        }

        public int Id { get; set; }

        public string Nome { get; set; }

        public TipoPessoa Tipo { get; set; }

        public string Cpf { get; set; }

        public ICollection<Cnpj> Cnpjs { get; set; }
    }
}
