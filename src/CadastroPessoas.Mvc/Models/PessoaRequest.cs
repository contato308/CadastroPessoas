using System.Collections.Generic;

namespace CadastroPessoas.Mvc.Models
{
    public class PessoaRequest
    {
        public string Nome { get; set; }

        public int Tipo { get; set; }

        public string Cpf { get; set; }

        public List<string> Cnpjs { get; set; }
    }
}
