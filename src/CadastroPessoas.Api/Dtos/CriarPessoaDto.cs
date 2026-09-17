using System.Collections.Generic;
using CadastroPessoas.Api.Models;

namespace CadastroPessoas.Api.Dtos
{
    public class CriarPessoaDto
    {
        public CriarPessoaDto()
        {
            Cnpjs = new List<string>();
        }

        public string Nome { get; set; }

        public TipoPessoa Tipo { get; set; }

        public string Cpf { get; set; }

        public ICollection<string> Cnpjs { get; set; }
    }
}
